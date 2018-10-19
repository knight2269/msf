using Insight.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
                    string sqlContent = ExtractSQL(file);

                    Console.WriteLine("file: {0}.txt --- {1}", file, sqlContent.Length);

                    File.WriteAllText(file + ".txt", sqlContent);
                }
            }

            foreach (string subDir in Directory.GetDirectories(path))
            {
                ProcessDirectory(subDir);
            }
        }

        static string ExtractSQL(string filePath)
        {
            string result = "";

            string databasePath = Path.GetFullPath(filePath);

            string connectionString = "Data Source=" + databasePath + ";Trusted_Connection=True;Version=3;";
            SQLiteConnection connection = new SQLiteConnection(connectionString);

            foreach (FastExpando tableInfo in connection.QuerySql("SELECT * FROM sqlite_master WHERE type='table'"))
            {
                result += JToken.FromObject(tableInfo).ToString();

                foreach (var tableDetail in tableInfo)
                {
                    if (tableDetail.Key == "tbl_name")
                    {
                        foreach (FastExpando tableRow in connection.QuerySql("SELECT * FROM " + tableDetail.Value))
                        {
                            result += JToken.FromObject(tableRow).ToString();
                        }
                    }
                }
            }

            return result;
        }
    }
}
