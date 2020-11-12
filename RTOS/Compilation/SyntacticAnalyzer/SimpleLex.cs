#define PERSIST

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics.CodeAnalysis;

using SimpleParser;
using QUT.Gppg;
using System.Linq;

namespace SimpleScanner
{      

     public sealed partial class Scanner : ScanBase
    {
        private ScanBuff buffer;
        int currentScOrd;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScanBuff Buffer { get { return buffer; } }
        
        private static int GetMaxParseToken()
        {
            System.Reflection.FieldInfo f = typeof(Tokens).GetField("maxParseToken");
            return (f == null ? int.MaxValue : (int)f.GetValue(null));
        }
        
        static int parserMax = GetMaxParseToken();
        
        enum Result {accept, noMatch, contextFound};

        const int maxAccept = 8;
        const int initial = 9;
        const int eofNum = 0;
        const int goStart = -1;
        const int INITIAL = 0;

        int state;
        int currentStart = startState[0];
        int code;
        int cCol;
        int lNum;

        int tokPos;
        int tokCol;
        int tokLin;
        int tokEPos;
        int tokECol;
        int tokELin;
        string tokTxt;

        #region ScannerTables
        struct Table
        {
        public int min; public int rng; public int dflt;
        public sbyte[] nxt;
        public Table(int m, int x, int d, sbyte[] n)
            {
                min = m; rng = x; dflt = d; nxt = n;
            }
        };

        static int[] startState = new int[] {9, 0};

        #region CompressedCharacterMap

        static sbyte[] mapC0 = new sbyte[126] {
/*     '\0' */ 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 0, 8, 8, 0, 8, 8, 
/*   '\x10' */ 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 
/*   '\x20' */ 0, 8, 8, 8, 8, 8, 8, 8, 4, 5, 8, 8, 8, 8, 8, 8, 
/*      '0' */ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 8, 3, 8, 8, 8, 8, 
/*      '@' */ 8, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*      'P' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 8, 8, 8, 8, 2, 
/*      '`' */ 8, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*      'p' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 6, 8, 7 };

        static sbyte MapC(int code)
        {
            if (code < 126)
                return mapC0[code - 0];
            else
                return (sbyte)8;
        }

        #endregion

        static Table[] NxS = new Table[10]
        {
/* NxS[   0] */ new Table(0, 0, 0, null),
/* NxS[   1] */ new Table(1, 1, -1, new sbyte[] {1}),
/* NxS[   2] */ new Table(1, 2, -1, new sbyte[] {2, 2}),
/* NxS[   3] */ new Table(0, 0, -1, null),
/* NxS[   4] */ new Table(0, 0, -1, null),
/* NxS[   5] */ new Table(0, 0, -1, null),
/* NxS[   6] */ new Table(0, 0, -1, null),
/* NxS[   7] */ new Table(0, 0, -1, null),
/* NxS[   8] */ new Table(0, 0, -1, null),
/* NxS[   9] */ new Table(1, 8, -1, new sbyte[] {1, 2, 3, 4, 5, 6, 
          7, 8}),
        };

        int NextState()
        {
            if (code == ScanBuff.EndOfFile)
                return eofNum;
            else
                unchecked
                {
                    int rslt;
                    int idx = MapC(code) - NxS[state].min;
                    if (idx < 0) idx += 9;
                    if ((uint)idx >= (uint)NxS[state].rng) rslt = NxS[state].dflt;
                    else rslt = NxS[state].nxt[idx];
                    return rslt;
                }
        }

        #endregion

		struct BufferContext
        {
            internal ScanBuff buffSv;
			internal int chrSv;
			internal int cColSv;
			internal int lNumSv;
		}

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        BufferContext MkBuffCtx()
		{
			BufferContext rslt;
			rslt.buffSv = this.buffer;
			rslt.chrSv = this.code;
			rslt.cColSv = this.cCol;
			rslt.lNumSv = this.lNum;
			return rslt;
		}

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        void RestoreBuffCtx(BufferContext value)
		{
			this.buffer = value.buffSv;
			this.code = value.chrSv;
			this.cCol = value.cColSv;
			this.lNum = value.lNumSv;
        }

        public Scanner(Stream file)
        {
            SetSource(file, 0);
        }

        public Scanner(Stream file, string codepage)
        {
            SetSource(file, CodePageHandling.GetCodePage(codepage));
        }

        public Scanner() { }

        private int readPos;

        void GetCode()
        {
            if (code == '\n')
            { 
                cCol = -1;
                lNum++;
            }
            readPos = buffer.Pos;
            code = buffer.Read();
            if (code > ScanBuff.EndOfFile)
            {
                if (code >= 0xD800 && code <= 0xDBFF)
                {
                    int next = buffer.Read();
                    if (next < 0xDC00 || next > 0xDFFF)
                        code = ScanBuff.UnicodeReplacementChar;
                    else
                        code = (0x10000 + (code & 0x3FF << 10) + (next & 0x3FF));
                }

                cCol++;
            }
        }

        void MarkToken()
        {
            tokPos = readPos;
            tokLin = lNum;
            tokCol = cCol;
        }
        
        void MarkEnd()
        {
            tokTxt = null;
            tokEPos = readPos;
            tokELin = lNum;
            tokECol = cCol;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        int Peek()
        {
            int rslt, codeSv = code, cColSv = cCol, lNumSv = lNum, bPosSv = buffer.Pos;
            GetCode(); rslt = code;
            lNum = lNumSv; cCol = cColSv; code = codeSv; buffer.Pos = bPosSv;
            return rslt;
        }
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void SetSource(string source, int offset)
        {
            this.buffer = ScanBuff.GetBuffer(source);
            this.buffer.Pos = offset;
            this.lNum = 0;
            this.code = '\n';
            GetCode();
        }     

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void SetSource(IList<string> source)
        {
            this.buffer = ScanBuff.GetBuffer(source);
            this.code = '\n';
            this.lNum = 0;
            GetCode();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void SetSource(Stream source)
        {
            this.buffer = ScanBuff.GetBuffer(source);
            this.lNum = 0;
            this.code = '\n';
            GetCode();
        }
        

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void SetSource(Stream source, int fallbackCodePage)
        {
            this.buffer = ScanBuff.GetBuffer(source, fallbackCodePage);
            this.lNum = 0;
            this.code = '\n';
            GetCode();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yylex")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yylex")]
        public override int yylex()
        {
            int next;
            do { next = Scan(); } while (next >= parserMax);
            return next;
        }
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        int yypos { get { return tokPos; } }
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        int yyline { get { return tokLin; } }
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        int yycol { get { return tokCol; } }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yytext")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yytext")]
        public string yytext
        {
            get 
            {
                if (tokTxt == null) 
                    tokTxt = buffer.GetString(tokPos, tokEPos);
                return tokTxt;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        void yyless(int n)
        {
            buffer.Pos = tokPos;
            cCol = tokCol - 1; 
            GetCode();
            lNum = tokLin;
            for (int i = 0; i < n; i++) GetCode();
            MarkEnd();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        void _yytrunc(int n) { yyless(yyleng - n); }
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yyleng")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yyleng")]
        public int yyleng
        {
            get
            { 
                if (tokELin == tokLin)
                    return tokECol - tokCol;
                else
                {
                    int ch;
                    int count = 0;
                    int save = buffer.Pos;
                    buffer.Pos = tokPos;
                    do
                    {
                        ch = buffer.Read();
                        if (!char.IsHighSurrogate((char)ch)) count++;
                    }
                    while (buffer.Pos < tokEPos && ch != ScanBuff.EndOfFile);
                    buffer.Pos = save; 
                    return count;
                }
            }
        }


        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal int YY_START
        {
            get { return currentScOrd; }
            set
            {
                currentScOrd = value;
                currentStart = startState[value]; 
            } 
        }
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal void BEGIN(int next)
        {
            currentScOrd = next;
            currentStart = startState[next];
        }

        int Scan()
        {
            try {
                for (; ; )
                {
                    int next;                 
                    state = currentStart;
                    while ((next = NextState()) == goStart)
                        GetCode();              
                    MarkToken();
                    state = next;
                    GetCode();
                    
                    while ((next = NextState()) > eofNum)
                    {
                        state = next;
                        GetCode();
                    }
                    if (state <= maxAccept) 
                    {
                        MarkEnd();

                        #region ActionSwitch

                        #pragma warning disable 162

                        switch (state)
                        {
                            case eofNum:
                                if (yywrap())
                                    return (int)Tokens.EOF;
                                break;
                            case 1:
                                return (int)Tokens.NUMBER;
                            case 2:
                                int res = ScannerHelper.GetIDToken(yytext);
                                return res;
                            case 3:
                                return (int)Tokens.SEMICOLON;
                            case 4:
                                return (int)Tokens.BRACKETL;
                            case 5:
                                return (int)Tokens.BRACKETR;
                            case 6:
                                return (int)Tokens.BRACEL;
                            case 7:
                                return (int)Tokens.BRACER;
                            case 8:
                                LexError();
	                            return (int)Tokens.EOF;
                            default:
                                break;
                        }
                        
                        #pragma warning restore 162
                        
                        #endregion
                    }
                }
            }
            finally
            {
                yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal void ECHO() { Console.Out.Write(yytext); }
        
#region UserCodeSection

public override void yyerror(string format, params object[] args)
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("(Строка: {0}): Встречено {1}, а ожидалось {2}", yyline, args[0], string.Join(" или ", ww));
            throw new SyntaxException(errorMsg);
}

public void LexError()
{
	string errorMsg = string.Format("({0}): Неизвестный символ {1}", yyline, yytext);
            throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,int> keywords;

  static ScannerHelper() 
  {
    keywords = new Dictionary<string,int>();

    keywords.Add("Script",(int)Tokens.SCRIPT);
    keywords.Add("Motion",(int)Tokens.MOTION);
    keywords.Add("Turn", (int)Tokens.TURN);
    keywords.Add("Take", (int)Tokens.TAKE);
    keywords.Add("Put", (int)Tokens.PUT);
    keywords.Add("Seal", (int)Tokens.SEAL);
    keywords.Add("Forward", (int)Tokens.FORWARD);
    keywords.Add("Back", (int)Tokens.BACK);
    keywords.Add("Left", (int)Tokens.LEFT);
    keywords.Add("Right", (int)Tokens.RIGHT);
    keywords.Add("On", (int)Tokens.ON);
  }
  public static int GetIDToken(string s)
  {
    if (keywords.ContainsKey(s))
      return keywords[s];
    else
      return (int)Tokens.TEXT;
  }
}

#endregion
    }
    [Serializable]
    public class BufferException : Exception
    {
        public BufferException() { }
        public BufferException(string message) : base(message) { }
        public BufferException(string message, Exception innerException)
            : base(message, innerException) { }
        protected BufferException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public abstract class ScanBuff
    {
        private string fileNm;

        public const int EndOfFile = -1;
        public const int UnicodeReplacementChar = 0xFFFD;

        public bool IsFile { get { return (fileNm != null); } }
        public string FileName { get { return fileNm; } set { fileNm = value; } }

        public abstract int Pos { get; set; }
        public abstract int Read();
        public virtual void Mark() { }

        public abstract string GetString(int begin, int limit);

        public static ScanBuff GetBuffer(string source)
        {
            return new StringBuffer(source);
        }

        public static ScanBuff GetBuffer(IList<string> source)
        {
            return new LineBuffer(source);
        }

        public static ScanBuff GetBuffer(Stream source)
        {
            return new BuildBuffer(source);
        }

#if (!BYTEMODE)
        public static ScanBuff GetBuffer(Stream source, int fallbackCodePage)
        {
            return new BuildBuffer(source, fallbackCodePage);
        }
#endif
    }

    #region Buffer classes

    sealed class StringBuffer : ScanBuff
    {
        string str;
        int bPos;
        int sLen;

        public StringBuffer(string source)
        {
            this.str = source;
            this.sLen = source.Length;
            this.FileName = null;
        }

        public override int Read()
        {
            if (bPos < sLen) return str[bPos++];
            else if (bPos == sLen) { bPos++; return '\n'; }
            else { bPos++; return EndOfFile; }
        }

        public override string GetString(int begin, int limit)
        {
            if (limit > sLen) limit = sLen;
            if (limit <= begin) return "";
            else return str.Substring(begin, limit - begin);
        }

        public override int Pos
        {
            get { return bPos; }
            set { bPos = value; }
        }

        public override string ToString() { return "StringBuffer"; }
    }

    sealed class LineBuffer : ScanBuff
    {
        IList<string> line;
        int numLines;
        string curLine;
        int cLine;
        int curLen;
        int curLineStart;
        int curLineEnd;
        int maxPos;
        int cPos;

        public LineBuffer(IList<string> lineList)
        {
            line = lineList;
            numLines = line.Count;
            cPos = curLineStart = 0;
            curLine = (numLines > 0 ? line[0] : "");
            maxPos = curLineEnd = curLen = curLine.Length;
            cLine = 1;
            FileName = null;
        }

        public override int Read()
        {
            if (cPos < curLineEnd)
                return curLine[cPos++ - curLineStart];
            if (cPos++ == curLineEnd)
                return '\n';
            if (cLine >= numLines)
                return EndOfFile;
            curLine = line[cLine];
            curLen = curLine.Length;
            curLineStart = curLineEnd + 1;
            curLineEnd = curLineStart + curLen;
            if (curLineEnd > maxPos)
                maxPos = curLineEnd;
            cLine++;
            return curLen > 0 ? curLine[0] : '\n';
        }

        private int cachedPosition;
        private int cachedIxdex;
        private int cachedLineStart;

        private void findIndex(int pos, out int ix, out int lstart)
        {
            if (pos >= cachedPosition)
            {
                ix = cachedIxdex; lstart = cachedLineStart;
            }
            else
            {
                ix = lstart = 0;
            }
            for (; ; )
            {
                int len = line[ix].Length + 1;
                if (pos < lstart + len) break;
                lstart += len;
                ix++;
            }
            cachedPosition = pos;
            cachedIxdex = ix;
            cachedLineStart = lstart;
        }

        public override string GetString(int begin, int limit)
        {
            if (begin >= maxPos || limit <= begin) return "";
            int endIx, begIx, endLineStart, begLineStart;
            findIndex(begin, out begIx, out begLineStart);
            int begCol = begin - begLineStart;
            findIndex(limit, out endIx, out endLineStart);
            int endCol = limit - endLineStart;
            string s = line[begIx];
            if (begIx == endIx)
            {
                return (endCol <= s.Length) ?
                    s.Substring(begCol, endCol - begCol)
                    : s.Substring(begCol) + "\n";
            }

            StringBuilder sb = new StringBuilder();
            if (begCol < s.Length)
                sb.Append(s.Substring(begCol));
            for (; ; )
            {
                sb.Append("\n");
                s = line[++begIx];
                if (begIx >= endIx) break;
                sb.Append(s);
            }
            if (endCol <= s.Length)
            {
                sb.Append(s.Substring(0, endCol));
            }
            else
            {
                sb.Append(s);
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public override int Pos
        {
            get { return cPos; }
            set
            {
                cPos = value;
                findIndex(cPos, out cLine, out curLineStart);
                curLine = line[cLine];
                curLineEnd = curLineStart + curLine.Length;
            }
        }

        public override string ToString() { return "LineBuffer"; }
    }

    class BuildBuffer : ScanBuff
    {
        class BufferElement
        {
            StringBuilder bldr = new StringBuilder();
            StringBuilder next = new StringBuilder();
            int minIx;
            int maxIx;
            int brkIx;
            bool appendToNext;

            internal BufferElement() { }

            internal int MaxIndex { get { return maxIx; } }

            internal char this[int index]
            {
                get
                {
                    if (index < minIx || index >= maxIx)
                        throw new BufferException("Index was outside data buffer");
                    else if (index < brkIx)
                        return bldr[index - minIx];
                    else
                        return next[index - brkIx];
                }
            }

            internal void Append(char[] block, int count)
            {
                maxIx += count;
                if (appendToNext)
                    this.next.Append(block, 0, count);
                else
                {
                    this.bldr.Append(block, 0, count);
                    brkIx = maxIx;
                    appendToNext = true;
                }
            }

            internal string GetString(int start, int limit)
            {
                if (limit <= start)
                    return "";
                if (start >= minIx && limit <= maxIx)
                    if (limit < brkIx)
                        return bldr.ToString(start - minIx, limit - start);
                    else if (start >= brkIx)
                        return next.ToString(start - brkIx, limit - start);
                    else
                        return
                            bldr.ToString(start - minIx, brkIx - start) +
                            next.ToString(0, limit - brkIx);
                else
                    throw new BufferException("String was outside data buffer");
            }

            internal void Mark(int limit)
            {
                if (limit > brkIx + 16)
                {
                    StringBuilder temp = bldr;
                    bldr = next;
                    next = temp;
                    next.Length = 0;
                    minIx = brkIx;
                    brkIx = maxIx;
                }
            }
        }

        BufferElement data = new BufferElement();

        int bPos;
        BlockReader NextBlk;

        private string EncodingName
        {
            get
            {
                StreamReader rdr = NextBlk.Target as StreamReader;
                return (rdr == null ? "raw-bytes" : rdr.CurrentEncoding.BodyName);
            }
        }

        public BuildBuffer(Stream stream)
        {
            FileStream fStrm = (stream as FileStream);
            if (fStrm != null) FileName = fStrm.Name;
            NextBlk = BlockReaderFactory.Raw(stream);
        }

#if (!BYTEMODE)
        public BuildBuffer(Stream stream, int fallbackCodePage)
        {
            FileStream fStrm = (stream as FileStream);
            if (fStrm != null) FileName = fStrm.Name;
            NextBlk = BlockReaderFactory.Get(stream, fallbackCodePage);
        }
#endif
        public override void Mark() { data.Mark(bPos - 2); }

        public override int Pos
        {
            get { return bPos; }
            set { bPos = value; }
        }

        public override int Read()
        {

            if (bPos < data.MaxIndex)
            {
                return (int)data[bPos++];
            }
            else
            {
                char[] chrs = new char[4096];
                int count = NextBlk(chrs, 0, 4096);
                if (count == 0)
                    return EndOfFile;
                else
                {
                    data.Append(chrs, count);
                    return (int)data[bPos++];
                }
            }
        }

        public override string GetString(int begin, int limit)
        {
            return data.GetString(begin, limit);
        }

        public override string ToString()
        {
            return "StringBuilder buffer, encoding: " + this.EncodingName;
        }
    }

    public delegate int BlockReader(char[] block, int index, int number);

    public static class BlockReaderFactory
    {
        public static BlockReader Raw(Stream stream)
        {
            return delegate(char[] block, int index, int number)
            {
                byte[] b = new byte[number];
                int count = stream.Read(b, 0, number);
                int i = 0;
                int j = index;
                for (; i < count; i++, j++)
                    block[j] = (char)b[i];
                return count;
            };
        }

#if (!BYTEMODE)
        public static BlockReader Get(Stream stream, int fallbackCodePage)
        {
            Encoding encoding;
            int preamble = Preamble(stream);

            if (preamble != 0)
                encoding = Encoding.GetEncoding(preamble);
            else if (fallbackCodePage == -1)
                return Raw(stream);
            else if (fallbackCodePage != -2)
                encoding = Encoding.GetEncoding(fallbackCodePage);
            else
            {
                int guess = new Guesser(stream).GuessCodePage();
                stream.Seek(0, SeekOrigin.Begin);
                if (guess == -1)
                    encoding = Encoding.ASCII;
                else if (guess == 65001)
                    encoding = Encoding.UTF8;
                else
                    encoding = Encoding.Default;
            }
            StreamReader reader = new StreamReader(stream, encoding);
            return reader.Read;
        }

        static int Preamble(Stream stream)
        {
            int b0 = stream.ReadByte();
            int b1 = stream.ReadByte();

            if (b0 == 0xfe && b1 == 0xff)
                return 1201;
            if (b0 == 0xff && b1 == 0xfe)
                return 1200;

            int b2 = stream.ReadByte();
            if (b0 == 0xef && b1 == 0xbb && b2 == 0xbf)
                return 65001;

            stream.Seek(0, SeekOrigin.Begin);
            return 0;
        }
#endif
    }
    #endregion Buffer classes

    public static class CodePageHandling
    {
        public static int GetCodePage(string option)
        {
            string command = option.ToUpperInvariant();
            if (command.StartsWith("CodePage:", StringComparison.OrdinalIgnoreCase))
                command = command.Substring(9);
            try
            {
                if (command.Equals("RAW"))
                    return -1;
                else if (command.Equals("GUESS"))
                    return -2;
                else if (command.Equals("DEFAULT"))
                    return 0;
                else if (char.IsDigit(command[0]))
                    return int.Parse(command, CultureInfo.InvariantCulture);
                else
                {
                    Encoding enc = Encoding.GetEncoding(command);
                    return enc.CodePage;
                }
            }
            catch (FormatException)
            {
                Console.Error.WriteLine(
                    "Invalid format \"{0}\", using machine default", option);
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine(
                    "Unknown code page \"{0}\", using machine default", option);
            }
            return 0;
        }
    }
#region guesser
#if (!BYTEMODE)

    internal class Guesser
    {
        ScanBuff buffer;

        public int GuessCodePage() { return Scan(); }

        const int maxAccept = 10;
        const int initial = 0;
        const int eofNum = 0;
        const int goStart = -1;
        const int INITIAL = 0;
        const int EndToken = 0;

        #region user code

        public long utfX;
        public long uppr;
        #endregion user code

        int state;
        int currentStart = startState[0];
        int code;

        #region ScannerTables
        static int[] startState = new int[] { 11, 0 };

        #region CharacterMap
        static sbyte[] map = new sbyte[256] {
/*     '\0' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*   '\x10' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*   '\x20' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      '0' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      '@' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      'P' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      '`' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      'p' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*   '\x80' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\x90' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\xA0' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\xB0' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\xC0' */ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
/*   '\xD0' */ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
/*   '\xE0' */ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 
/*   '\xF0' */ 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5 };
        #endregion

        static sbyte[][] nextState = new sbyte[][] {
            new sbyte[] {0, 0, 0, 0, 0, 0},
            new sbyte[] {-1, -1, 10, -1, -1, -1},
            new sbyte[] {-1, -1, -1, -1, -1, -1},
            new sbyte[] {-1, -1, 8, -1, -1, -1},
            new sbyte[] {-1, -1, 5, -1, -1, -1},
            new sbyte[] {-1, -1, 6, -1, -1, -1},
            new sbyte[] {-1, -1, 7, -1, -1, -1},
            null,
            new sbyte[] {-1, -1, 9, -1, -1, -1},
            null,
            null,
            new sbyte[] {-1, 1, 2, 3, 4, 2}
        };


        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static Guesser()
        {
            nextState[7] = nextState[2];
            nextState[9] = nextState[2];
            nextState[10] = nextState[2];
        }

        int NextState()
        {
            if (code == ScanBuff.EndOfFile)
                return eofNum;
            else
                return nextState[state][map[code]];
        }
        #endregion

        public Guesser(System.IO.Stream file) { SetSource(file); }

        public void SetSource(System.IO.Stream source)
        {
            this.buffer = new BuildBuffer(source);
            code = buffer.Read();
        }

        int Scan()
        {
            for (; ; )
            {
                int next;
                state = currentStart;
                while ((next = NextState()) == goStart)
                    code = buffer.Read();

                state = next;
                code = buffer.Read();

                while ((next = NextState()) > eofNum)
                {
                    state = next;
                    code = buffer.Read();
                }
                if (state <= maxAccept)
                {
                    #region ActionSwitch
#pragma warning disable 162
                    switch (state)
                    {
                        case eofNum:
                            switch (currentStart)
                            {
                                case 11:
                                    if (utfX == 0 && uppr == 0) return -1; /* raw ascii */
                                    else if (uppr * 10 > utfX) return 0;   /* default code page */
                                    else return 65001;                     /* UTF-8 encoding */
                                    break;
                            }
                            return EndToken;
                        case 1: // Recognized '{Upper128}',	Shortest string "\xC0"
                        case 2: // Recognized '{Upper128}',	Shortest string "\x80"
                        case 3: // Recognized '{Upper128}',	Shortest string "\xE0"
                        case 4: // Recognized '{Upper128}',	Shortest string "\xF0"
                            uppr++;
                            break;
                        case 5: // Recognized '{Utf8pfx4}{Utf8cont}',	Shortest string "\xF0\x80"
                            uppr += 2;
                            break;
                        case 6: // Recognized '{Utf8pfx4}{Utf8cont}{2}',	Shortest string "\xF0\x80\x80"
                            uppr += 3;
                            break;
                        case 7: // Recognized '{Utf8pfx4}{Utf8cont}{3}',	Shortest string "\xF0\x80\x80\x80"
                            utfX += 3;
                            break;
                        case 8: // Recognized '{Utf8pfx3}{Utf8cont}',	Shortest string "\xE0\x80"
                            uppr += 2;
                            break;
                        case 9: // Recognized '{Utf8pfx3}{Utf8cont}{2}',	Shortest string "\xE0\x80\x80"
                            utfX += 2;
                            break;
                        case 10: // Recognized '{Utf8pfx2}{Utf8cont}',	Shortest string "\xC0\x80"
                            utfX++;
                            break;
                        default:
                            break;
                    }
#pragma warning restore 162
                    #endregion
                }
            }
        }
    }
    
#endif
#endregion
}
