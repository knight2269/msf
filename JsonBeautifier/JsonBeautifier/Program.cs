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

            rootDir = @"..\..\..\..\..\files\";

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

                    JToken rootToken = JToken.Parse(json1);

                    string json2 = rootToken.ToString(Formatting.Indented);

                    Console.WriteLine("file: {0} --- {1} => {2}", file, json1.Length, json2.Length);

                    File.WriteAllText(file, json2);

                    string csvContent;
                    if (TryConvertJsonToCsv(rootToken, out csvContent) == true)
                    {
                        File.WriteAllText(file + ".csv", csvContent);
                    }
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

        private static bool TryConvertJsonToCsv(JToken rootToken, out string csvContent)
        {
            csvContent = string.Empty;

            RaidConfig raidConfig = rootToken.ToObject<RaidConfig>();

            if (raidConfig.RaidDetails != null)
            {
                foreach (KeyValuePair<string, RoomInfo> roomInfo in raidConfig.RaidDetails.rooms)
                {
                    int missionsCount = roomInfo.Value.missions == null ? 0 : roomInfo.Value.missions.Count;

                    MissionInfo missionInfo = null;

                    if (missionsCount > 0)
                    {
                        missionInfo = new List<MissionInfo>(roomInfo.Value.missions.Values)[0];
                    }

                    csvContent += roomInfo.Key;

                    csvContent += ",";

                    if (missionsCount > 0)
                    {
                        csvContent += TryGetMissionRestriction(missionInfo);
                    }

                    csvContent += ",";

                    if (roomInfo.Value.starting == true)
                    {
                        csvContent += "Start here";
                    }
                    else if (missionsCount == 0)
                    {
                        csvContent += "Empty room";
                    }
                    else if (missionsCount > 1)
                    {
                        csvContent += "ERROR: multimission room";
                    }
                    else if (missionInfo.isBoss == true)
                    {
                        csvContent += "Boss";
                    }

                    csvContent += Environment.NewLine;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private static string TryGetMissionRestriction(MissionInfo missionInfo)
        {
            if (missionInfo.filters.filters.allTraits != null)
            {
                string result = string.Empty;

                foreach (string trait in missionInfo.filters.filters.allTraits)
                {
                    if (string.IsNullOrEmpty(result) == false)
                    {
                        result += " and ";
                    }

                    result += trait;
                }

                return result;
            }

            if (missionInfo.filters.filters.anyTrait != null)
            {
                string result = string.Empty;

                foreach (string trait in missionInfo.filters.filters.anyTrait)
                {
                    if (string.IsNullOrEmpty(result) == false)
                    {
                        result += " or ";
                    }

                    result += trait;
                }

                return result;
            }

            return "any";
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
