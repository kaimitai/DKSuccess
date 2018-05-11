using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKSuccess
{

    class DKSuccess
    {
        const string cmdAdd = "add";
        const string cmdExport = "export";

        private static readonly string welcomeText = "DKSuccess: A database-driven command-line tool for keeping track of Donkey Kong progression" + Environment.NewLine + "See DKSuccess.pdf for usage instructions." + Environment.NewLine;
        private static readonly List<string> commands = new List<string>() { cmdAdd, cmdExport };

        static void Main(string[] args) {
            Console.WriteLine(welcomeText);

            Config config = new Config("config.xml");
            List<string> initArgs = new List<string>();

            foreach (string arg in args) {
                string tmpArg = arg.Trim();

                if (tmpArg.StartsWith("@")) {
                    string replacedParams = config.GetParameterValue(tmpArg.Substring(1, tmpArg.Length - 1));
                    while (replacedParams.Contains("  "))
                        replacedParams = replacedParams.Replace("  ", " ");
                    initArgs.Add(replacedParams);
                } else
                    initArgs.Add(tmpArg.Trim());
            }

            string arguments = string.Join(" ", initArgs);

            List<string> argList = arguments.Split(' ').ToList();

            if (argList.Count() == 0 || arguments.Trim() == string.Empty) {
                Console.WriteLine("Missing command and arguments.");
                return;
            } else if (argList.Count() == 1) {
                if (commands.Contains(argList[0].Trim().ToLower()))
                    Console.WriteLine("Primary command " + argList[0].Trim() + " requires parameters.");
                else
                    Console.WriteLine("Primary command " + argList[0].Trim() + " is not valid.");

                return;
            }

            Database db = new Database();
            string mainArgument = argList[0].Trim().ToLower();

            if (mainArgument == cmdAdd) {
                string gameString = argList[1].Trim();

                if (argList.Count() > 2)
                    gameString += " " + argList[2].Trim();

                Game addedGame = new Game(gameString);
                db.AddGame(addedGame);
                db.Save();
                Console.WriteLine("Added to database: Game with deaths on " + addedGame.GetDeathString() + " and score " + (addedGame.GetScore() == null ? "NULL" : addedGame.GetScore().ToString()));
            } else if (mainArgument == cmdExport) {
                string granularity = (argList.Count()>=3 ? argList[2].Trim().ToUpper() : config.GetDefaultGranularity());
                DateTime minDate = (argList.Count() >= 4 ? DateTime.Parse(argList[3].Trim().ToUpper()) : config.GetDefaultFromDate());
                DateTime maxDate = (argList.Count() >= 5 ? DateTime.Parse(argList[4].Trim().ToUpper()) : config.GetDefaultToDate());

                Dictionary<DateTime, Dictionary<string, Tuple<int, int>>> result = db.GetStatistics(argList[1].Trim(), granularity, minDate, maxDate);

                List<string> fileContents = new List<string>();
                string header = "Date,";
                foreach (string item in argList[1].Trim().Split(',').ToList())
                    header += item + ",";

                header = header.Substring(0, header.Length - 1);
                fileContents.Add(header);

                foreach (DateTime gDate in result.Keys) {
                    string dataItem = gDate.ToString("yyyy-MM-dd HH:mm:ss") + ",";

                    foreach (string s in result[gDate].Keys) {
                        dataItem += ((double)result[gDate][s].Item1 / (double)result[gDate][s].Item2).ToString("F3", CultureInfo.InvariantCulture) + ",";
                    }

                    fileContents.Add(dataItem.Substring(0, dataItem.Length - 1));
                }

                File.WriteAllLines("DKSuccess_export_" + DateTime.Now.ToShortDateString() + ".csv", fileContents);
            }


            //Console.ReadKey();
        }

    }
}
