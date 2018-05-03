using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKSuccess {

    class Game {

        int? Score;
        List<Death> Deaths;
        DateTime Date;

        public DateTime GetDate() {
            return Date;
        }

        public List<Death> GetDeaths() {
            return Deaths;
        }

        public int? GetScore() {
            return Score;
        }

        public string GetDeathString() {
            string result = string.Empty;
            foreach (Death death in Deaths) {
                result += death.ToString() + ",";
            }

            return (result.Substring(0, result.Length - 1));
        }

        public Game(string parameters) {
            Score = null;
            Deaths = new List<Death>();
            Date = DateTime.Now;

            string deaths = parameters.Split(' ')[0];
            if (parameters.Contains(" "))
                Score = int.Parse(parameters.Split(' ')[1]);

            foreach (string deathString in deaths.Split(','))
                Deaths.Add(new Death(deathString));

            Deaths.Sort();
        }

        public Game(string parameters, DateTime date) {
            Score = null;
            Deaths = new List<Death>();
            Date = DateTime.Now;

            string deaths = parameters.Split(' ')[0];
            if (parameters.Contains(" "))
                Score = int.Parse(parameters.Split(' ')[1]);

            foreach (string deathString in deaths.Split(','))
                Deaths.Add(new Death(deathString));

            Deaths.Sort();

            Date = date;
        }

    }
}
