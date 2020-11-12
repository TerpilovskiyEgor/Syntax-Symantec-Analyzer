using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTOS.Animator
{
    static class Loger
    {
        static string pathToFile = @"..\..\Logs\CurrentLog.txt";
        static DateTime date = DateTime.Now;

        static StreamWriter SW = new StreamWriter(pathToFile, true, Encoding.Default);

        public static void addToLofFile(string text)
        {
            SW.WriteLine(text);
            SW.Flush();
        }
    }
}