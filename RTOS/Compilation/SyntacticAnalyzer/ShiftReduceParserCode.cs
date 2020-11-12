#define EXPORT_GPPG

using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace QUT.Gppg
{
#if EXPORT_GPPG
    public abstract class ShiftReduceParser<TValue, TSpan>
#else
    internal abstract class ShiftReduceParser<TValue, TSpan>
#endif
 where TSpan : IMerge<TSpan>, new()
    {
        public AbstractScanner<TValue, TSpan> scanner;
        protected AbstractScanner<TValue, TSpan> Scanner
        {
            get { return scanner; }
            set { scanner = value; }
        }
        protected ShiftReduceParser(AbstractScanner<TValue, TSpan> scanner)
        {
            this.scanner = scanner;
        }
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        protected TValue CurrentSemanticValue;

        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        protected TSpan CurrentLocationSpan;

        private TSpan LastSpan;
        private int NextToken;
        private State FsaState;
        private bool recovering;
        private int tokensSinceLastError;

        private PushdownPrefixState<State> StateStack = new PushdownPrefixState<State>();
        private PushdownPrefixState<TValue> valueStack = new PushdownPrefixState<TValue>();
        private PushdownPrefixState<TSpan> locationStack = new PushdownPrefixState<TSpan>();

        protected PushdownPrefixState<TValue> ValueStack { get { return valueStack; } }

        protected PushdownPrefixState<TSpan> LocationStack { get { return locationStack; } }

        private int errorToken;
        private int endOfFileToken;
        private string[] nonTerminals;
        private State[] states;
        private Rule[] rules;

        protected void InitRules(Rule[] rules) { this.rules = rules; }

        protected void InitStates(State[] states) { this.states = states; }

        protected void InitStateTable(int size) { states = new State[size]; }

        protected void InitSpecialTokens(int err, int end)
        {
            errorToken = err;
            endOfFileToken = end;
        }

        protected void InitNonTerminals(string[] names) { nonTerminals = names; }

        #region YYAbort, YYAccept etcetera.
        [Serializable]
        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]

        private class AcceptException : Exception
        {
            internal AcceptException() { }
            protected AcceptException(SerializationInfo i, StreamingContext c) : base(i, c) { }
        }
        [Serializable]
        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]

        private class AbortException : Exception
        {
            internal AbortException() { }
            protected AbortException(SerializationInfo i, StreamingContext c) : base(i, c) { }
        }
        [Serializable]
        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]

        private class ErrorException : Exception
        {
            internal ErrorException() { }
            protected ErrorException(SerializationInfo i, StreamingContext c) : base(i, c) { }
        }

        protected static void YYAccept() { throw new AcceptException(); }

        protected static void YYAbort() { throw new AbortException(); }

        protected static void YYError() { throw new ErrorException(); }

        protected bool YYRecovering { get { return recovering; } }
        #endregion

        protected abstract void Initialize();

        public bool Parse()
        {
            Initialize();

            NextToken = 0;
            FsaState = states[0];

            StateStack.Push(FsaState);
            valueStack.Push(CurrentSemanticValue);
            LocationStack.Push(CurrentLocationSpan);

            while (true)
            {
#if TRACE_ACTIONS
                    Console.Error.WriteLine("Entering state {0} ", FsaState.number);
#endif
                int action = FsaState.defaultAction;

                if (FsaState.ParserTable != null)
                {
                    if (NextToken == 0)
                    {
#if TRACE_ACTIONS
                            Console.Error.Write("Reading a token: ");
#endif
                        LastSpan = scanner.yylloc;
                        NextToken = scanner.yylex();
                    }

#if TRACE_ACTIONS
                        Console.Error.WriteLine("Next token is {0}", TerminalToString(NextToken));
#endif
                    if (FsaState.ParserTable.ContainsKey(NextToken))
                        action = FsaState.ParserTable[NextToken];
                }

                if (action > 0)
                {
                    Shift(action);
                }
                else if (action < 0)
                {
                    try
                    {
                        Reduce(-action);
                        if (action == -1)
                            return true;
                    }
                    catch (Exception x)
                    {
                        if (x is AbortException)
                            return false;
                        else if (x is AcceptException)
                            return true;
                        else if (x is ErrorException && !ErrorRecovery())
                            return false;
                        else
                            throw;

                    }
                }
                else if (action == 0)
                    if (!ErrorRecovery())
                        return false;
            }
        }

        private void Shift(int stateIndex)
        {
#if TRACE_ACTIONS
				Console.Error.Write("Shifting token {0}, ", TerminalToString(NextToken));
#endif
            FsaState = states[stateIndex];

            valueStack.Push(scanner.yylval);
            StateStack.Push(FsaState);
            LocationStack.Push(scanner.yylloc);

            if (recovering)
            {
                if (NextToken != errorToken)
                    tokensSinceLastError++;

                if (tokensSinceLastError > 5)
                    recovering = false;
            }

            if (NextToken != endOfFileToken)
                NextToken = 0;
        }

        private void Reduce(int ruleNumber)
        {
#if TRACE_ACTIONS
				DisplayRule(ruleNumber);
#endif
            Rule rule = rules[ruleNumber];
            if (rule.RightHandSide.Length == 1)
            {
                CurrentSemanticValue = valueStack.TopElement();
                CurrentLocationSpan = LocationStack.TopElement();
            }
            else
            {
                if (rule.RightHandSide.Length == 0)
                {
                    CurrentSemanticValue = default(TValue);

                    CurrentLocationSpan = (scanner.yylloc != null && LastSpan != null ?
                        scanner.yylloc.Merge(LastSpan) :
                        default(TSpan));
                }
                else
                {
                    CurrentSemanticValue = valueStack.TopElement();

                    TSpan at1 = LocationStack[LocationStack.Depth - rule.RightHandSide.Length];
                    TSpan atN = LocationStack[LocationStack.Depth - 1];
                    CurrentLocationSpan =
                        ((at1 != null && atN != null) ? at1.Merge(atN) : default(TSpan));
                }
            }

            DoAction(ruleNumber);

            for (int i = 0; i < rule.RightHandSide.Length; i++)
            {
                StateStack.Pop();
                valueStack.Pop();
                LocationStack.Pop();
            }

#if TRACE_ACTIONS
				DisplayStack();
#endif
            FsaState = StateStack.TopElement();

            if (FsaState.Goto.ContainsKey(rule.LeftHandSide))
                FsaState = states[FsaState.Goto[rule.LeftHandSide]];

            StateStack.Push(FsaState);
            valueStack.Push(CurrentSemanticValue);
            LocationStack.Push(CurrentLocationSpan);
        }

        protected abstract void DoAction(int actionNumber);

        private bool ErrorRecovery()
        {
            bool discard;

            if (!recovering)
                ReportError();

            if (!FindErrorRecoveryState())
                return false;

            ShiftErrorToken();
            discard = DiscardInvalidTokens();
            recovering = true;
            tokensSinceLastError = 0;
            return discard;
        }

        private void ReportError1()
        {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendFormat("Syntax error, unexpected {0}", TerminalToString(NextToken));

            if (FsaState.ParserTable.Count < 7)
            {
                bool first = true;
                foreach (int terminal in FsaState.ParserTable.Keys)
                {
                    if (first)
                        errorMsg.Append(", expecting ");
                    else
                        errorMsg.Append(", or ");

                    errorMsg.Append(TerminalToString(terminal));
                    first = false;
                }
            }
            scanner.yyerror(errorMsg.ToString());
        }

        private void ReportError()
        {
            object[] args = new object[FsaState.ParserTable.Keys.Count + 1];
            args[0] = TerminalToString(NextToken);
            int i = 1;
            foreach (int terminal in FsaState.ParserTable.Keys)
            {
                args[i] = TerminalToString(terminal);
                i++;
            }
            scanner.yyerror("", args);
        }

        private void ShiftErrorToken()
        {
            int old_next = NextToken;
            NextToken = errorToken;

            Shift(FsaState.ParserTable[NextToken]);

#if TRACE_ACTIONS
				Console.Error.WriteLine("Entering state {0} ", FsaState.number);
#endif
            NextToken = old_next;
        }

        private bool FindErrorRecoveryState()
        {
            while (true)
            {
                if (FsaState.ParserTable != null &&
                    FsaState.ParserTable.ContainsKey(errorToken) &&
                    FsaState.ParserTable[errorToken] > 0)
                    return true;

#if TRACE_ACTIONS
					Console.Error.WriteLine("Error: popping state {0}", StateStack.Top().number);
#endif
                StateStack.Pop();
                valueStack.Pop();
                LocationStack.Pop();

#if TRACE_ACTIONS
					DisplayStack();
#endif
                if (StateStack.IsEmpty())
                {
#if TRACE_ACTIONS
                        Console.Error.Write("Aborting: didn't find a state that accepts error token");
#endif
                    return false;
                }
                else
                    FsaState = StateStack.TopElement();
            }
        }

        private bool DiscardInvalidTokens()
        {

            int action = FsaState.defaultAction;

            if (FsaState.ParserTable != null)
            {

                while (true)
                {
                    if (NextToken == 0)
                    {
#if TRACE_ACTIONS
                            Console.Error.Write("Reading a token: ");
#endif                       
                        NextToken = scanner.yylex();
                    }

#if TRACE_ACTIONS
                        Console.Error.WriteLine("Next token is {0}", TerminalToString(NextToken));
#endif
                    if (NextToken == endOfFileToken)
                        return false;

                    if (FsaState.ParserTable.ContainsKey(NextToken))
                        action = FsaState.ParserTable[NextToken];

                    if (action != 0)
                        return true;
                    else
                    {
#if TRACE_ACTIONS
                            Console.Error.WriteLine("Error: Discarding {0}", TerminalToString(NextToken));
#endif
                        NextToken = 0;
                    }
                }
            }
            else if (recovering && tokensSinceLastError == 0)
            {
#if TRACE_ACTIONS
                    Console.Error.WriteLine("Error: panic discard of {0}", TerminalToString(NextToken));
#endif
                if (NextToken == endOfFileToken)
                    return false;
                NextToken = 0;
                return true;
            }
            else
                return true;

        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yyclearin")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yyclearin")]

        protected void yyclearin() { NextToken = 0; }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yyerrok")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yyerrok")]


        protected void yyerrok()
        {
            recovering = false;
        }

        protected void AddState(int stateNumber, State state)
        {
            states[stateNumber] = state;
            state.number = stateNumber;
        }

        private void DisplayStack()
        {
            Console.Error.Write("State now");
            for (int i = 0; i < StateStack.Depth; i++)
                Console.Error.Write(" {0}", StateStack[i].number);
            Console.Error.WriteLine();
        }

        private void DisplayRule(int ruleNumber)
        {
            Console.Error.Write("Reducing stack by rule {0}, ", ruleNumber);
            DisplayProduction(rules[ruleNumber]);
        }

        private void DisplayProduction(Rule rule)
        {
            if (rule.RightHandSide.Length == 0)
                Console.Error.Write("/* empty */ ");
            else
                foreach (int symbol in rule.RightHandSide)
                    Console.Error.Write("{0} ", SymbolToString(symbol));

            Console.Error.WriteLine("-> {0}", SymbolToString(rule.LeftHandSide));
        }

        protected abstract string TerminalToString(int terminal);

        private string SymbolToString(int symbol)
        {
            if (symbol < 0)
                return nonTerminals[-symbol];
            else
                return TerminalToString(symbol);
        }

        protected static string CharToString(char input)
        {
            switch (input)
            {
                case '\a': return @"'\a'";
                case '\b': return @"'\b'";
                case '\f': return @"'\f'";
                case '\n': return @"'\n'";
                case '\r': return @"'\r'";
                case '\t': return @"'\t'";
                case '\v': return @"'\v'";
                case '\0': return @"'\0'";
                default: return string.Format(CultureInfo.InvariantCulture, "'{0}'", input);
            }
        }
    }

#if EXPORT_GPPG
    public interface IMerge<TSpan>
#else
    internal interface IMerge<TSpan>
#endif
    {
        TSpan Merge(TSpan last);
    }

#if EXPORT_GPPG
    public class LexLocation : IMerge<LexLocation>
#else
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class LexLocation : IMerge<LexLocation>
#endif
    {
        private int startLine;
        private int startColumn;
        private int endLine;
        private int endColumn;

        public int StartLine { get { return startLine; } }

        public int StartColumn { get { return startColumn; } }

        public int EndLine { get { return endLine; } }

        public int EndColumn { get { return endColumn; } }

        public LexLocation()
        { }

        public LexLocation(int sl, int sc, int el, int ec)
        { startLine = sl; startColumn = sc; endLine = el; endColumn = ec; }

        public LexLocation Merge(LexLocation last)
        { return new LexLocation(this.startLine, this.startColumn, last.endLine, last.endColumn); }

    }

#if EXPORT_GPPG
    public abstract class AbstractScanner<TValue, TSpan>
#else
    internal abstract class AbstractScanner<TValue, TSpan>
#endif
        where TSpan : IMerge<TSpan>
    {

        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yylval")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yylval")]

        public TValue yylval;

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yylloc")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yylloc")]


        public virtual TSpan yylloc
        {
            get { return default(TSpan); }
            set { /* skip */ }
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yylex")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yylex")]


        public abstract int yylex();

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yyerror")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yyerror")]


        public virtual void yyerror(string format, params object[] args) { }
    }

#if EXPORT_GPPG
    public class State
#else
    internal class State
#endif
    {
        internal int number;
        internal Dictionary<int, int> ParserTable;
        internal Dictionary<int, int> Goto;
        internal int defaultAction;

        public State(int[] actions, int[] goToList)
            : this(actions)
        {
            Goto = new Dictionary<int, int>();
            for (int i = 0; i < goToList.Length; i += 2)
                Goto.Add(goToList[i], goToList[i + 1]);
        }

        public State(int[] actions)
        {
            ParserTable = new Dictionary<int, int>();
            for (int i = 0; i < actions.Length; i += 2)
                ParserTable.Add(actions[i], actions[i + 1]);
        }

        public State(int defaultAction)
        {
            this.defaultAction = defaultAction;
        }

        public State(int defaultAction, int[] goToList)
            : this(defaultAction)
        {
            Goto = new Dictionary<int, int>();
            for (int i = 0; i < goToList.Length; i += 2)
                Goto.Add(goToList[i], goToList[i + 1]);
        }
    }

#if EXPORT_GPPG
    public class Rule
#else
    internal class Rule
#endif
    {
        internal int LeftHandSide;
        internal int[] RightHandSide;

        public Rule(int left, int[] right)
        {
            this.LeftHandSide = left;
            this.RightHandSide = right;
        }
    }

#if EXPORT_GPPG
    public class PushdownPrefixState<T>
#else
    internal class PushdownPrefixState<T>
#endif
    {

        private T[] array = new T[8];
        private int tos = 0;

        public T this[int index] { get { return array[index]; } }

        public int Depth { get { return tos; } }

        internal void Push(T value)
        {
            if (tos >= array.Length)
            {
                T[] newarray = new T[array.Length * 2];
                System.Array.Copy(array, newarray, tos);
                array = newarray;
            }
            array[tos++] = value;
        }

        internal T Pop()
        {
            T rslt = array[--tos];
            array[tos] = default(T);
            return rslt;
        }

        internal T TopElement() { return array[tos - 1]; }

        internal bool IsEmpty() { return tos == 0; }
    }
}