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
                Console.ReadKey();
                return;
            }

            for (int i = 0; i <= 1; i++)
            {
                if (File.Exists(args[i]) == false)
                {
                    Console.WriteLine("file {0} not found", args[i]);
                    Console.ReadKey();
                    return;
                }
            }

            List<string> lines0 = new List<string>(File.ReadAllLines(args[0]));
            List<string> lines1 = new List<string>(File.ReadAllLines(args[1]));

            int _checked = 0;
            int _removed = 0;

            for (int indexFrom1 = lines1.Count - 1; indexFrom1 >= 0; indexFrom1--)
            {
                int indexFrom0 = lines0.FindIndex(x => AreSame(x, lines1[indexFrom1]));

                _checked++;
                if (indexFrom0 >= 0)
                {
                    lines0.RemoveAt(indexFrom0);
                    lines1.RemoveAt(indexFrom1);
                    _removed++;
                }
            }

            File.WriteAllLines(args[0], lines0);
            File.WriteAllLines(args[1], lines1);

            Console.WriteLine("{0} removed of {1} lines", _removed, _checked);
            Console.ReadKey();
        }

        private static bool AreSame(string line1, string line2)
        {
            string[] line1Splitted = line1.Split(',', StringSplitOptions.RemoveEmptyEntries);
            string[] line2Splitted = line2.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (line1Splitted.Length != line2Splitted.Length) { return false; }

            for (int i = 0; i < line1Splitted.Length; i++)
            {
                if (line1Splitted[i] != line2Splitted[i]) { return false; }
            }

            return true;
        }
    }
}
