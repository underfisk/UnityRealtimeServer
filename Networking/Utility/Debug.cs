using System;
using static System.Diagnostics.Debug;

namespace Networking.Utility
{
    public static partial class Debug
    {
        public static void Log(string msg)
        {
            Console.WriteLine($"[{DateTime.Now}]: {msg}");
        }

        public static void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Warning at {DateTime.Now} \n \t {msg}");
            Console.ResetColor();
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error occurred at {DateTime.Now} \n \t {msg}");
            Console.ResetColor();
        }

        public static void Assertion(bool condition)
        {
            Assert(condition);
        }
       
    }
}
