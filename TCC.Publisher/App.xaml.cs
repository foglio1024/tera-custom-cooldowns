using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Application = System.Windows.Application;

namespace TCC.Publisher
{
    public partial class App
    {
        public static List<string> Exclusions;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Exclusions = File.ReadAllLines("D:/Repos/TCC/TCC.Publisher/exclusions.txt").ToList();
        }
    }
    public static class Logger
    {
        public static void WriteLine(string msg)
        {
            Console.WriteLine(msg);
            (Application.Current.MainWindow as MainWindow)?.AddLine(msg);
        }
        public static void Write(string msg)
        {
            Console.Write(msg);
            (Application.Current.MainWindow as MainWindow)?.AppendToLine(msg);

        }
    }
}
