using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DKSuccess {

    class Database {
        const string DateFormat = "yyyy-MM-dd HH:mm";
        const string DbFilename = "db.xml";
        const string DbVersion = "1.0";

        // list of built-in stat types, used for meta-statistics
        private readonly List<string> BuiltInStats = new List<string>() { "mins", "maxs", "minl", "maxl", "avgs" };

        string Filename;
        List<Game> Games;

        public Database(string filename = DbFilename) {
            XmlDocument doc = new XmlDocument();
            Filename = filename;

            Games = new List<Game>();

            if (File.Exists(Filename)) {
                doc.Load(Filename);

                XmlNode rootNode = doc.SelectSingleNode("/DKSuccess");

                foreach (XmlElement gameNode in rootNode.ChildNodes) {
                    string gameString = gameNode.GetAttribute("Deaths");
                    if (gameNode.HasAttribute("Score"))
                        gameString += " " + gameNode.GetAttribute("Score");
                    DateTime gameTime = DateTime.ParseExact(gameNode.GetAttribute("Date"), DateFormat, CultureInfo.InvariantCulture);

                    Games.Add(new Game(gameString, gameTime));
                }
            }

        }

        public void Save() {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlElement rootNode = doc.CreateElement("DKSuccess");
            rootNode.SetAttribute("version", DbVersion);
            doc.AppendChild(rootNode);

            foreach (Game game in Games) {
                XmlElement gameNode = doc.CreateElement("Game");
                gameNode.SetAttribute("Date", game.GetDate().ToString(DateFormat, CultureInfo.InvariantCulture));
                gameNode.SetAttribute("Deaths", game.GetDeathString());

                if (game.GetScore() != null)
                    gameNode.SetAttribute("Score", game.GetScore().ToString());

                rootNode.AppendChild(gameNode);
            }

            doc.Save(Filename);
        }

        public Dictionary<DateTime, Dictionary<string, Tuple<int, int>>> GetStatistics(string stats, string granularity, DateTime minDate, DateTime maxDate) {
            Dictionary<DateTime, Dictionary<string, Tuple<int, int>>> result = new Dictionary<DateTime, Dictionary<string, Tuple<int, int>>>();

            List<string> allColumns = stats.Split(',').ToList();

            List<string> dynamicColumns = new List<string>();
            List<string> builtInColumns = new List<string>();

            foreach (string col in allColumns) {
                if (BuiltInStats.Contains(col))
                    builtInColumns.Add(col);
                else
                    dynamicColumns.Add(col);
            }

            // do all the dynamic calculations
            foreach (string col in dynamicColumns) {
                Tuple<int, int, char> statType = GetCanonicalStatRepresentation(col);
                string canon = statType.Item1 + "," + statType.Item2 + "," + statType.Item3;
                DateTime gameDate = minDate;

                foreach (Game game in Games) {
                    gameDate = game.GetDate();

                    // only consider games in our date range
                    if (gameDate < minDate || gameDate > maxDate)
                        continue;

                    gameDate = TruncateDate(gameDate, granularity);

                    Tuple<int, int> subStats = game.GetSuccessRate(statType.Item1, statType.Item2, statType.Item3);

                    if (!result.ContainsKey(gameDate))
                        result.Add(gameDate, new Dictionary<string, Tuple<int, int>>());
                    if (!result[gameDate].ContainsKey(canon))
                        result[gameDate].Add(canon, new Tuple<int, int>(0, 0));

                    result[gameDate][canon] = new Tuple<int, int>(result[gameDate][canon].Item1 + subStats.Item1, result[gameDate][canon].Item2 + subStats.Item2);
                }

            }

            // do all the built-int calculations
            foreach (string col in builtInColumns) {
                DateTime gameDate = minDate;

                foreach (Game game in Games) {
                    gameDate = game.GetDate();
                    // only consider games in our date range
                    if (gameDate < minDate || gameDate > maxDate)
                        continue;

                    gameDate = TruncateDate(gameDate, granularity);

                    if (!result.ContainsKey(gameDate))
                        result.Add(gameDate, new Dictionary<string, Tuple<int, int>>());
                    if (!result[gameDate].ContainsKey(col))
                        result[gameDate].Add(col, new Tuple<int, int>(0, 0));

                    if (col == "maxs") {
                        if (game.GetScore() != null && (int)game.GetScore() > result[gameDate][col].Item1)
                            result[gameDate][col] = new Tuple<int, int>((int)game.GetScore(), 1);
                    }
                    else if (col == "mins") {
                        if (game.GetScore() != null && (int)game.GetScore() < (result[gameDate][col].Item1 == 0 ? int.MaxValue : result[gameDate][col].Item1))
                            result[gameDate][col] = new Tuple<int, int>((int)game.GetScore(), 1);
                    }
                    else if (col == "avgs") {
                        if (game.GetScore() != null)
                            result[gameDate][col] = new Tuple<int, int>(result[gameDate][col].Item1 + (int)game.GetScore(), result[gameDate][col].Item2 + 1);
                    }


                }
            }

            return (result);
        }

        private static DateTime TruncateDate(DateTime dt, string granularity) {
            if (granularity == "DAY")
                return new DateTime(dt.Year, dt.Month, dt.Day);
            else if (granularity == "WEEK")
                return FirstDateInWeek(dt);
            else if (granularity == "MONTH")
                return new DateTime(dt.Year, dt.Month, 1);
            else if (granularity == "YEAR")
                return new DateTime(dt.Year, 1, 1);
            else if (granularity == "CENTURY")
                return new DateTime(2000, 1, 1);
            else
                return (dt);
        }

        private static DateTime FirstDateInWeek(DateTime dt) {
            while (dt.DayOfWeek != DayOfWeek.Monday)
                dt = dt.AddDays(-1);
            return new DateTime(dt.Year, dt.Month, dt.Day); ;
        }

        private Tuple<int, int, char> GetCanonicalStatRepresentation(string col) {
            char bType = 'A';
            int minLevel = 1;
            int maxLevel = 22;

            if (!(col[0] >= '0' && col[0] <= '9')) {
                bType = col[0];
                col = col.Substring(1, col.Length - 1);
            }

            if (col.Length > 0) {
                List<string> lvls = col.Split('-').ToList();

                minLevel = int.Parse(lvls[0]);

                if (lvls.Count() > 1)
                    maxLevel = int.Parse(lvls[1]);
            }

            return (new Tuple<int, int, char>(minLevel, maxLevel, bType));
        }


        public void AddGame(string gameString) {
            Games.Add(new Game(gameString));
        }

        public void AddGame(Game inGame) {
            Games.Add(inGame);
        }

    }
}
