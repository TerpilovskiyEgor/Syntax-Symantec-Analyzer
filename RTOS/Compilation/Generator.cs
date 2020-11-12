using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTOS.Compilation
{
    class Generator
    {
        public List<string> Lines = new List<string>();
        public System.Windows.Forms.SaveFileDialog SaveFileDialog = new System.Windows.Forms.SaveFileDialog();

        public Generator(string text)
        {
            foreach (var line in text.Split('\n'))
            {
                Lines.Add(line);
            }
            Parse(Lines);
        }

        public void Generate()
        {
            string ClassName = "MyClass1";
            foreach (var line in Lines)
            {
                if (line.Contains("Script"))
                {
                    ClassName = line.Remove(0, 7);
                    break;
                }
            }
            if (SaveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            string filepath = SaveFileDialog.FileName + ".cs";
            File.Create(filepath).Dispose();

            using (StreamWriter SW = new StreamWriter(filepath, true))
            {

                SW.WriteLine(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTOS.Compilation
{
        class " + ClassName + @"
        {
            public static void Main(string[] args)
            {");
                foreach (var comand in Lines)
                {
                    switch (comand.Split('-')[0])
                    {
                        case "Motion":
                            SW.WriteLine(@"                 Move(" + comand.Split('-')[1] + ", " + comand.Split('-')[2] + ");");
                            break;
                        case "Turn":
                            SW.WriteLine(@"                 Turn(" + comand.Split('-')[1] + ", " + comand.Split('-')[2] + ");");
                            break;
                        case "Take":
                            SW.WriteLine(@"                 Take();");
                            break;
                        case "Seal":
                            SW.WriteLine(@"                 Seal();");
                            break;
                        case "Put":
                            SW.WriteLine(@"                 Put();");
                            break;
                    }
                }
                SW.WriteLine(@"            }");
                SW.WriteLine(@"        }");
                SW.WriteLine("}");
            }
        }

        private void Parse(List<string> lines)
        {
            for (int i = Lines.IndexOf(Lines.First()); i <= Lines.IndexOf(Lines.Last()); i++)
            {
                lines[i] = lines[i].Replace(" ", "-");
                lines[i] = lines[i].Replace("---", "");
                lines[i] = lines[i].Replace(";", "");
                lines[i] = lines[i].Replace("On-(", "-");
                lines[i] = lines[i].Replace(")", "");
                lines[i] = lines[i].Replace("--", "-");
                if (lines[i] == "{" || lines[i] == "}")
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
