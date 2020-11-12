using SimpleParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTOS.Compilation.SemanticAnalyzer
{
    class SemanticAnalyzer
    {
        private Dictionary<int, string> Lines = new Dictionary<int, string>();
        private List<string> ErrorList = new List<string>();
        private List<int> ErrorNumberLineList = new List<int>();

        public SemanticAnalyzer(string Text)
        {
            int i = 0;
            string[] textArr = Text.Split('\n');
            foreach (var line in textArr)
                Lines.Add(i++, line);
        }

        public void Analysis()
        {
            Parser();
            Parse(Lines);
            AbstractAnimation();
        }

        public void AbstractAnimation()
        {
            AbstractRobo robo = new AbstractRobo();
            string s;

            foreach (var line in Lines)
            {
                if (line.Value.Contains("-"))
                    s = line.Value.Remove(line.Value.IndexOf('-'));
                else
                    s = line.Value;
                switch (s)
                {
                    case "MotionForward":
                        if (!robo.IfMotionForward(GetCol(line.Value)))
                        {
                            ErrorList.Add("Робот выходит за границу");
                            ErrorNumberLineList.Add(line.Key);
                        }
                        break;

                    case "MotionBack":
                        if (!robo.IfMotionBack(GetCol(line.Value)))
                        {
                            ErrorList.Add("Робот выходит за границу");
                            ErrorNumberLineList.Add(line.Key);
                        }
                        break;

                    case "Take":
                        if (!robo.IfTake())
                        {
                            ErrorList.Add("Робот не может взять предмет");
                            ErrorNumberLineList.Add(line.Key);
                        }
                        break;

                    case "Seal":
                        if (!robo.IfSeal())
                        {
                            ErrorList.Add("Робот не может упаковать предмет");
                            ErrorNumberLineList.Add(line.Key);
                        }
                        break;

                    case "Put":
                        if (!robo.IfPut())
                        {
                            ErrorList.Add("Робот не может погрузить предмет");
                            ErrorNumberLineList.Add(line.Key);
                        }
                        break;

                    case "TurnLeft":
                        robo.TurnLeft();
                        break;

                    case "TurnRight":
                        robo.TurnRight();
                        break;
                }
            }
        }

        private void Parser()
        {
            for (int i = Lines.First().Key; i < Lines.Last().Key; i++)
            {
                if (Lines[i] != "{")
                    Lines.Remove(i);
                else
                {
                    Lines.Remove(i);
                    break;
                }
            }

            for (int i = Lines.Last().Key; i >= Lines.First().Key; i--)
            {
                if (Lines[i] == "}")
                    Lines.Remove(i);
                else
                {
                    Lines.Remove(i);
                    break;
                }
            }
        }

        public void Parse(Dictionary<int,string> lines)
        {
            for (int i = lines.First().Key; i <= lines.Last().Key; i++)
            {
                lines[i] = lines[i].Replace(" ", "");
                lines[i] = lines[i].Replace(";", "");
                lines[i] = lines[i].Replace("On(", "-");
                lines[i] = lines[i].Replace(")", "");
            }
        }

        private int GetCol(string Comand)
        {
            int StartIndex = Comand.IndexOf('-') + 1;
            if (StartIndex == 0) { return 0; }
            int Length = Comand.Length - StartIndex;
            if (Length <= 0) return 0;
            int Result = Convert.ToInt32(Comand.Substring(StartIndex, Length));
            
            return Result;
        }

        public List<string> GetError()
        {
            return ErrorList;
        }

        public List<int> GetErrorNumberLine()
        {
            return ErrorNumberLineList;
        }
    }
}
