using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTOS.Interpreter
{
    static class Parser
    {
        public static List<string> Parse(string Text)
        {
            List<string> ComandList = new List<string>();
            List<string> TextList = Text.Split('\n').ToList();

            TextList.RemoveRange(0,2);
            TextList.RemoveAt(TextList.Count-1);

            foreach (string line in TextList)
            {
                string T = line.Replace(" ", "");
                T = T.Replace(";", "");
                int C = GetCol(ref T);
                for (int i = 0; i < C; i++)
                {
                    ComandList.Add(T);
                }
            }

            return ComandList;
        }

        private static int GetCol(ref string Comand)
        {
            int StartIndex = Comand.IndexOf('(') + 1;
            if(StartIndex == 0) { return 1; }
            int Length = Comand.LastIndexOf(')') - StartIndex;
            if (Length <= 0) return 0;
            int Result = Convert.ToInt32(Comand.Substring(StartIndex, Length));

            int From = Comand.IndexOf('(') - 2;
            int Col = Comand.Length - From;
            Comand = Comand.Remove(From, Col);
            return Result;
        }
    }
}