using Insight.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace JsonBeautifier
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootDir = Directory.GetCurrentDirectory();

            rootDir = @"..\..\..\..\..\Config\";

            Console.WriteLine("parsing dir: {0}", rootDir);

            ProcessDirectory(rootDir);

            Console.WriteLine("done!");

            Console.ReadKey();
        }

        static void ProcessDirectory(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (file.EndsWith(".json"))
                {
                    string json1 = File.ReadAllText(file);

                    string json2 = JToken.Parse(json1).ToString(Formatting.Indented);

                    Console.WriteLine("file: {0} --- {1} => {2}", file, json1.Length, json2.Length);

                    File.WriteAllText(file, json2);
                }

                if (file.EndsWith(".db"))
                {
                    //ExtractSQL(file);
                }
            }

            foreach (string subDir in Directory.GetDirectories(path))
            {
                ProcessDirectory(subDir);
            }
        }

        static void ExtractSQL(string filePath)
        {
            string databasePath = Path.GetFullPath(filePath);
            Console.WriteLine("Extracting SQL from: " + databasePath);
            string connectionString = "Data Source=" + databasePath + ";Version=3;";
            SqlConnection connection = new SqlConnection(connectionString);
            IList<FastExpando> tables = connection.QuerySql("select * from tables");
            foreach (FastExpando table in tables)
            {
                foreach (string name in table.GetDynamicMemberNames())
                {
                    Console.WriteLine(name);
                }
            }
        }
    }
}
