using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKSuccess {

    class DKSuccess {
        static void Main(string[] args) {
            List<string> argList = args.ToList();

            if (argList.Count() < 2)
                return;

            Database db = new Database();
            string mainArgument = argList[0].Trim().ToLower();

            if (mainArgument == "add") {
                string gameString = argList[1].Trim();

                if (argList.Count() > 2)
                    gameString += " " + argList[2].Trim();

                Game addedGame = new Game(gameString);
                db.AddGame(addedGame);
                db.Save();
                Console.WriteLine("Added to database: Game with deaths on " + addedGame.GetDeathString() + " and score " + (addedGame.GetScore() == null ? "NULL" : addedGame.GetScore().ToString()));
            }
            else if (mainArgument == "export") {
                DateTime minDate = DateTime.Parse("2018-01-01");
                DateTime maxDate = DateTime.Parse("2018-12-31");
                string granularity = argList[2].Trim().ToUpper();

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
                        dataItem += ((double)result[gDate][s].Item1 / (double)result[gDate][s].Item2).ToString("F3",CultureInfo.InvariantCulture) + ",";
                    }

                    fileContents.Add(dataItem.Substring(0, dataItem.Length - 1));
                }

                File.WriteAllLines("DKSuccess_export_" + DateTime.Now.ToShortDateString() + ".csv", fileContents);
            }


            //Console.ReadKey();
        }

    }
}
