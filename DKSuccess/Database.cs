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

        string Filename;
        List<Game> db;

        public Database(string filename = DbFilename) {
            XmlDocument doc = new XmlDocument();
            Filename = filename;

            db = new List<Game>();

            if (File.Exists(Filename)) {
                doc.Load(Filename);

                XmlNode rootNode = doc.SelectSingleNode("/DKSuccess");

                foreach (XmlElement gameNode in rootNode.ChildNodes) {
                    string gameString = gameNode.GetAttribute("Deaths");
                    if (gameNode.HasAttribute("Score"))
                        gameString += " " + gameNode.GetAttribute("Score");
                    DateTime gameTime = DateTime.ParseExact(gameNode.GetAttribute("Date"), DateFormat, CultureInfo.InvariantCulture);

                    db.Add(new Game(gameString, gameTime));
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

            foreach (Game game in db) {
                XmlElement gameNode = doc.CreateElement("Game");
                gameNode.SetAttribute("Date", game.GetDate().ToString(DateFormat, CultureInfo.InvariantCulture));
                gameNode.SetAttribute("Deaths", game.GetDeathString());

                if (game.GetScore() != null)
                    gameNode.SetAttribute("Score", game.GetScore().ToString());

                rootNode.AppendChild(gameNode);
            }

            doc.Save(Filename);
        }

        public void AddGame(string gameString) {
            db.Add(new Game(gameString));
        }

    }
}
