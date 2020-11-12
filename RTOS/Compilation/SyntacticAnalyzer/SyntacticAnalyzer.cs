using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleScanner;
using SimpleParser;

namespace RTOS.Compilation
{
    class SyntacticAnalyzer
    {
        private string Text;
        private List<string> ErrorList = new List<string>();
        private List<int> ErrorNumberLineList = new List<int>();

        public SyntacticAnalyzer(string Text)
        {
            this.Text = Text;
        }

        public void Analysis()
        {
            try
            {
                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                Parser parser = new Parser(scanner);
                var b = parser.Parse();
                if (!b)
                {
                    Console.WriteLine("Ошибка");
                }
                else
                {
                    Console.WriteLine("Программа распознана");
                }
            }
            catch (LexException e)
            {
                Console.WriteLine("Лексическая ошибка. " + e.Message);
                Reanalysis(e.Message);
            }
            catch (SyntaxException e)
            {
                Console.WriteLine("Синтаксическая ошибка. " + e.Message);
                Reanalysis(e.Message);
            }
        }

        private void Reanalysis(string Error)
        {
            int StringNumber = GetNumber(Error)-1;
            int oldSN = StringNumber;
            
            if (Error.IndexOf("TEXT") < Error.IndexOf("SCRIPT"))
                if (Text.Contains("Script "))
                {
                    TextReplace(StringNumber, "");
                    ErrorController(Error);
                }
                else
                {
                    TextReplace(StringNumber, "Script Analyzer");
                    ErrorController(Error);
                }
            else if (Error.Contains("TEXT") && Error.Contains("BRACEL"))
                if(Error.IndexOf("TEXT") < Error.IndexOf("BRACEL"))
                {
                    TextReplace(StringNumber, "Script Analyzer");
                    ErrorController(Error);
                }
                else
                {
                    TextReplace(StringNumber - 1, "Script Analyzer");
                    ErrorController(Error, 1);
                }
            else if (Error.Contains("BRACEL"))
            {
                TextReplace(StringNumber-1, "{");
                ErrorController(Error, 1);
            }
            else if (Error.Contains("EOF") && Error.Contains("BRACER"))
            {
                TextReplace(StringNumber, "}");
                ErrorController(Error);
            }
            else if (Error.Contains("SEMICOLON"))
            {
                TextReplace(StringNumber - 1, "");
                ErrorController(Error, 1);
            }
            else
            {
                TextReplace(StringNumber, "");
                ErrorController(Error);
            }
            
            if (StringNumber >= Text.Split('\n').Length)
                return;
            Analysis();
        }



        private void ErrorController(string Error)
        {
            //(2,1): Встречено TEXT, а ожидалось NUMBER\\
            Error = Error.Replace("BRACKETL", "'('");
            Error = Error.Replace("BRACKETR", "')'");
            Error = Error.Replace("BRACEL", "'{'");
            Error = Error.Replace("BRACER", "'}'");
            Error = Error.Replace("SEMICOLON", "';'");

            int EN = GetNumber(Error) - 1;
            
            if(EN == Text.Split('\n').Length) { EN--; }
            ErrorNumberLineList.Add(EN);
            ErrorList.Add(Error);
        }

        private void ErrorController(string Error, int I)
        {
            //(2,1): Встречено TEXT, а ожидалось NUMBER\\
            Error = Error.Replace("BRACKETL", "'('");
            Error = Error.Replace("BRACKETR", "')'");
            Error = Error.Replace("BRACEL", "'{'");
            Error = Error.Replace("BRACER", "'}'");
            Error = Error.Replace("SEMICOLON", "';'");

            int EN = GetNumber(Error) - I - 1;

            if (EN == Text.Split('\n').Length) { EN--; }
            ErrorNumberLineList.Add(EN);
            ErrorList.Add(Error);
        }

        public List<string> GetError()
        {
            return ErrorList;
        }

        public List<int> GetErrorNumberLine()
        {
            return ErrorNumberLineList;
        }

        #region Tools

        private string ArrayToString(string[] Arr)
        {
            StringBuilder SB = new StringBuilder();
            foreach (string Line in Arr)
            {
                SB.Append(Line + "\n");
            }
            return SB.ToString();
        }

        private int GetNumber(string S)
        {
            int StartIndex = S.IndexOf(':') + 2;
            if (StartIndex == 0) { return 1; }
            int Length = S.IndexOf(')') - StartIndex;
            if (Length <= 0) return 0;
            int Result = Convert.ToInt32(S.Substring(StartIndex, Length));
            return Result;
        }

        private void TextReplace(int I, string T)
        {
            string[] TextArr = Text.Split('\n');
            if(I >= TextArr.Length) { I--; }
            TextArr[I] = T;
            Text = ArrayToString(TextArr);
        }

        #endregion
    }
}