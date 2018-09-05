using System;
using System.Collections.Generic;
using System.IO;

namespace DiffExtractor
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args == null || args.Length != 2)
            {
                Console.WriteLine("usage: [exe name] [file name 1] [file name 2]");
                return;
            }

            for (int i = 0; i <= 1; i++)
            {
                if (File.Exists(args[i]) == false)
                {
                    Console.WriteLine("file {0} not found", args[i]);
                }
            }

            List<string> lines0 = new List<string>(File.ReadAllLines(args[0]));
            List<string> lines1 = new List<string>(File.ReadAllLines(args[1]));

            for (int indexFrom1 = lines1.Count - 1; indexFrom1 >= 0; indexFrom1--)
            {
                int indexFrom0 = lines0.IndexOf(lines1[indexFrom1]);

                if (indexFrom0 >= 0)
                {
                    lines0.RemoveAt(indexFrom0);
                    lines1.RemoveAt(indexFrom1);
                }
            }

            File.WriteAllLines(args[0], lines0);
            File.WriteAllLines(args[1], lines1);
        }
    }
}
