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

        public Tuple<int, int> GetSuccessRate(int minLevel, int maxLevel, char boardType = 'A') {
            int success = GetSuccesses(minLevel, maxLevel, boardType);
            int failure = GetFails(minLevel, maxLevel, boardType);
            return new Tuple<int, int>(success, success + failure);
        }

        // sum up the fails for the given criteria
        private int GetFails(int minLevel, int maxLevel, char boardType = 'A') {
            int result = 0;

            foreach (Death death in Deaths) {
                if (death.Fulfills(minLevel, maxLevel, boardType))
                    ++result;
            }

            return (result);
        }

        // sum up the successes for the given criteria
        private int GetSuccesses(int minLevel, int maxLevel, char boardType = 'A') {
            int lvl = Deaths[Deaths.Count() - 1].GetLevel();
            int brd = Deaths[Deaths.Count() - 1].GetBoard();

            if (lvl > maxLevel) {
                lvl = maxLevel + 1;
                brd = 1;
            }

            int result = LevelsBelow[new Tuple<int, int, char>(lvl, brd, boardType)];
            int substract = (minLevel == 0 ? 0 : LevelsBelow[new Tuple<int, int, char>(minLevel, 1, boardType)]);

            return (result - substract);
        }

        // pre-computed list of levels of each type below thresholds
        private static readonly Dictionary<Tuple<int, int, char>, int> LevelsBelow = new Dictionary<Tuple<int, int, char>, int>() {
                { new Tuple<int, int, char>(1,1,'B'), 0},   { new Tuple<int, int, char>(1,1,'C'), 0},   { new Tuple<int, int, char>(1,1,'S'), 0},   { new Tuple<int, int, char>(1,1,'R'), 0},   { new Tuple<int, int, char>(1,1,'A'), 0},
                { new Tuple<int, int, char>(1,2,'B'), 1},   { new Tuple<int, int, char>(1,2,'C'), 0},   { new Tuple<int, int, char>(1,2,'S'), 0},   { new Tuple<int, int, char>(1,2,'R'), 0},   { new Tuple<int, int, char>(1,2,'A'), 1},
                { new Tuple<int, int, char>(2,1,'B'), 1},   { new Tuple<int, int, char>(2,1,'C'), 0},   { new Tuple<int, int, char>(2,1,'S'), 0},   { new Tuple<int, int, char>(2,1,'R'), 1},   { new Tuple<int, int, char>(2,1,'A'), 2},
                { new Tuple<int, int, char>(2,2,'B'), 2},   { new Tuple<int, int, char>(2,2,'C'), 0},   { new Tuple<int, int, char>(2,2,'S'), 0},   { new Tuple<int, int, char>(2,2,'R'), 1},   { new Tuple<int, int, char>(2,2,'A'), 3},
                { new Tuple<int, int, char>(2,3,'B'), 2},   { new Tuple<int, int, char>(2,3,'C'), 0},   { new Tuple<int, int, char>(2,3,'S'), 1},   { new Tuple<int, int, char>(2,3,'R'), 1},   { new Tuple<int, int, char>(2,3,'A'), 4},
                { new Tuple<int, int, char>(3,1,'B'), 2},   { new Tuple<int, int, char>(3,1,'C'), 0},   { new Tuple<int, int, char>(3,1,'S'), 1},   { new Tuple<int, int, char>(3,1,'R'), 2},   { new Tuple<int, int, char>(3,1,'A'), 5},
                { new Tuple<int, int, char>(3,2,'B'), 3},   { new Tuple<int, int, char>(3,2,'C'), 0},   { new Tuple<int, int, char>(3,2,'S'), 1},   { new Tuple<int, int, char>(3,2,'R'), 2},   { new Tuple<int, int, char>(3,2,'A'), 6},
                { new Tuple<int, int, char>(3,3,'B'), 3},   { new Tuple<int, int, char>(3,3,'C'), 1},   { new Tuple<int, int, char>(3,3,'S'), 1},   { new Tuple<int, int, char>(3,3,'R'), 2},   { new Tuple<int, int, char>(3,3,'A'), 7},
                { new Tuple<int, int, char>(3,4,'B'), 3},   { new Tuple<int, int, char>(3,4,'C'), 1},   { new Tuple<int, int, char>(3,4,'S'), 2},   { new Tuple<int, int, char>(3,4,'R'), 2},   { new Tuple<int, int, char>(3,4,'A'), 8},
                { new Tuple<int, int, char>(4,1,'B'), 3},   { new Tuple<int, int, char>(4,1,'C'), 1},   { new Tuple<int, int, char>(4,1,'S'), 2},   { new Tuple<int, int, char>(4,1,'R'), 3},   { new Tuple<int, int, char>(4,1,'A'), 9},
                { new Tuple<int, int, char>(4,2,'B'), 4},   { new Tuple<int, int, char>(4,2,'C'), 1},   { new Tuple<int, int, char>(4,2,'S'), 2},   { new Tuple<int, int, char>(4,2,'R'), 3},   { new Tuple<int, int, char>(4,2,'A'), 10},
                { new Tuple<int, int, char>(4,3,'B'), 4},   { new Tuple<int, int, char>(4,3,'C'), 2},   { new Tuple<int, int, char>(4,3,'S'), 2},   { new Tuple<int, int, char>(4,3,'R'), 3},   { new Tuple<int, int, char>(4,3,'A'), 11},
                { new Tuple<int, int, char>(4,4,'B'), 5},   { new Tuple<int, int, char>(4,4,'C'), 2},   { new Tuple<int, int, char>(4,4,'S'), 2},   { new Tuple<int, int, char>(4,4,'R'), 3},   { new Tuple<int, int, char>(4,4,'A'), 12},
                { new Tuple<int, int, char>(4,5,'B'), 5},   { new Tuple<int, int, char>(4,5,'C'), 2},   { new Tuple<int, int, char>(4,5,'S'), 3},   { new Tuple<int, int, char>(4,5,'R'), 3},   { new Tuple<int, int, char>(4,5,'A'), 13},
                { new Tuple<int, int, char>(5,1,'B'), 5},   { new Tuple<int, int, char>(5,1,'C'), 2},   { new Tuple<int, int, char>(5,1,'S'), 3},   { new Tuple<int, int, char>(5,1,'R'), 4},   { new Tuple<int, int, char>(5,1,'A'), 14},
                { new Tuple<int, int, char>(5,2,'B'), 6},   { new Tuple<int, int, char>(5,2,'C'), 2},   { new Tuple<int, int, char>(5,2,'S'), 3},   { new Tuple<int, int, char>(5,2,'R'), 4},   { new Tuple<int, int, char>(5,2,'A'), 15},
                { new Tuple<int, int, char>(5,3,'B'), 6},   { new Tuple<int, int, char>(5,3,'C'), 3},   { new Tuple<int, int, char>(5,3,'S'), 3},   { new Tuple<int, int, char>(5,3,'R'), 4},   { new Tuple<int, int, char>(5,3,'A'), 16},
                { new Tuple<int, int, char>(5,4,'B'), 7},   { new Tuple<int, int, char>(5,4,'C'), 3},   { new Tuple<int, int, char>(5,4,'S'), 3},   { new Tuple<int, int, char>(5,4,'R'), 4},   { new Tuple<int, int, char>(5,4,'A'), 17},
                { new Tuple<int, int, char>(5,5,'B'), 7},   { new Tuple<int, int, char>(5,5,'C'), 3},   { new Tuple<int, int, char>(5,5,'S'), 4},   { new Tuple<int, int, char>(5,5,'R'), 4},   { new Tuple<int, int, char>(5,5,'A'), 18},
                { new Tuple<int, int, char>(5,6,'B'), 8},   { new Tuple<int, int, char>(5,6,'C'), 3},   { new Tuple<int, int, char>(5,6,'S'), 4},   { new Tuple<int, int, char>(5,6,'R'), 4},   { new Tuple<int, int, char>(5,6,'A'), 19},
                { new Tuple<int, int, char>(6,1,'B'), 8},   { new Tuple<int, int, char>(6,1,'C'), 3},   { new Tuple<int, int, char>(6,1,'S'), 4},   { new Tuple<int, int, char>(6,1,'R'), 5},   { new Tuple<int, int, char>(6,1,'A'), 20},
                { new Tuple<int, int, char>(6,2,'B'), 9},   { new Tuple<int, int, char>(6,2,'C'), 3},   { new Tuple<int, int, char>(6,2,'S'), 4},   { new Tuple<int, int, char>(6,2,'R'), 5},   { new Tuple<int, int, char>(6,2,'A'), 21},
                { new Tuple<int, int, char>(6,3,'B'), 9},   { new Tuple<int, int, char>(6,3,'C'), 4},   { new Tuple<int, int, char>(6,3,'S'), 4},   { new Tuple<int, int, char>(6,3,'R'), 5},   { new Tuple<int, int, char>(6,3,'A'), 22},
                { new Tuple<int, int, char>(6,4,'B'), 10},  { new Tuple<int, int, char>(6,4,'C'), 4},   { new Tuple<int, int, char>(6,4,'S'), 4},   { new Tuple<int, int, char>(6,4,'R'), 5},   { new Tuple<int, int, char>(6,4,'A'), 23},
                { new Tuple<int, int, char>(6,5,'B'), 10},  { new Tuple<int, int, char>(6,5,'C'), 4},   { new Tuple<int, int, char>(6,5,'S'), 5},   { new Tuple<int, int, char>(6,5,'R'), 5},   { new Tuple<int, int, char>(6,5,'A'), 24},
                { new Tuple<int, int, char>(6,6,'B'), 11},  { new Tuple<int, int, char>(6,6,'C'), 4},   { new Tuple<int, int, char>(6,6,'S'), 5},   { new Tuple<int, int, char>(6,6,'R'), 5},   { new Tuple<int, int, char>(6,6,'A'), 25},
                { new Tuple<int, int, char>(7,1,'B'), 11},  { new Tuple<int, int, char>(7,1,'C'), 4},   { new Tuple<int, int, char>(7,1,'S'), 5},   { new Tuple<int, int, char>(7,1,'R'), 6},   { new Tuple<int, int, char>(7,1,'A'), 26},
                { new Tuple<int, int, char>(7,2,'B'), 12},  { new Tuple<int, int, char>(7,2,'C'), 4},   { new Tuple<int, int, char>(7,2,'S'), 5},   { new Tuple<int, int, char>(7,2,'R'), 6},   { new Tuple<int, int, char>(7,2,'A'), 27},
                { new Tuple<int, int, char>(7,3,'B'), 12},  { new Tuple<int, int, char>(7,3,'C'), 5},   { new Tuple<int, int, char>(7,3,'S'), 5},   { new Tuple<int, int, char>(7,3,'R'), 6},   { new Tuple<int, int, char>(7,3,'A'), 28},
                { new Tuple<int, int, char>(7,4,'B'), 13},  { new Tuple<int, int, char>(7,4,'C'), 5},   { new Tuple<int, int, char>(7,4,'S'), 5},   { new Tuple<int, int, char>(7,4,'R'), 6},   { new Tuple<int, int, char>(7,4,'A'), 29},
                { new Tuple<int, int, char>(7,5,'B'), 13},  { new Tuple<int, int, char>(7,5,'C'), 5},   { new Tuple<int, int, char>(7,5,'S'), 6},   { new Tuple<int, int, char>(7,5,'R'), 6},   { new Tuple<int, int, char>(7,5,'A'), 30},
                { new Tuple<int, int, char>(7,6,'B'), 14},  { new Tuple<int, int, char>(7,6,'C'), 5},   { new Tuple<int, int, char>(7,6,'S'), 6},   { new Tuple<int, int, char>(7,6,'R'), 6},   { new Tuple<int, int, char>(7,6,'A'), 31},
                { new Tuple<int, int, char>(8,1,'B'), 14},  { new Tuple<int, int, char>(8,1,'C'), 5},   { new Tuple<int, int, char>(8,1,'S'), 6},   { new Tuple<int, int, char>(8,1,'R'), 7},   { new Tuple<int, int, char>(8,1,'A'), 32},
                { new Tuple<int, int, char>(8,2,'B'), 15},  { new Tuple<int, int, char>(8,2,'C'), 5},   { new Tuple<int, int, char>(8,2,'S'), 6},   { new Tuple<int, int, char>(8,2,'R'), 7},   { new Tuple<int, int, char>(8,2,'A'), 33},
                { new Tuple<int, int, char>(8,3,'B'), 15},  { new Tuple<int, int, char>(8,3,'C'), 6},   { new Tuple<int, int, char>(8,3,'S'), 6},   { new Tuple<int, int, char>(8,3,'R'), 7},   { new Tuple<int, int, char>(8,3,'A'), 34},
                { new Tuple<int, int, char>(8,4,'B'), 16},  { new Tuple<int, int, char>(8,4,'C'), 6},   { new Tuple<int, int, char>(8,4,'S'), 6},   { new Tuple<int, int, char>(8,4,'R'), 7},   { new Tuple<int, int, char>(8,4,'A'), 35},
                { new Tuple<int, int, char>(8,5,'B'), 16},  { new Tuple<int, int, char>(8,5,'C'), 6},   { new Tuple<int, int, char>(8,5,'S'), 7},   { new Tuple<int, int, char>(8,5,'R'), 7},   { new Tuple<int, int, char>(8,5,'A'), 36},
                { new Tuple<int, int, char>(8,6,'B'), 17},  { new Tuple<int, int, char>(8,6,'C'), 6},   { new Tuple<int, int, char>(8,6,'S'), 7},   { new Tuple<int, int, char>(8,6,'R'), 7},   { new Tuple<int, int, char>(8,6,'A'), 37},
                { new Tuple<int, int, char>(9,1,'B'), 17},  { new Tuple<int, int, char>(9,1,'C'), 6},   { new Tuple<int, int, char>(9,1,'S'), 7},   { new Tuple<int, int, char>(9,1,'R'), 8},   { new Tuple<int, int, char>(9,1,'A'), 38},
                { new Tuple<int, int, char>(9,2,'B'), 18},  { new Tuple<int, int, char>(9,2,'C'), 6},   { new Tuple<int, int, char>(9,2,'S'), 7},   { new Tuple<int, int, char>(9,2,'R'), 8},   { new Tuple<int, int, char>(9,2,'A'), 39},
                { new Tuple<int, int, char>(9,3,'B'), 18},  { new Tuple<int, int, char>(9,3,'C'), 7},   { new Tuple<int, int, char>(9,3,'S'), 7},   { new Tuple<int, int, char>(9,3,'R'), 8},   { new Tuple<int, int, char>(9,3,'A'), 40},
                { new Tuple<int, int, char>(9,4,'B'), 19},  { new Tuple<int, int, char>(9,4,'C'), 7},   { new Tuple<int, int, char>(9,4,'S'), 7},   { new Tuple<int, int, char>(9,4,'R'), 8},   { new Tuple<int, int, char>(9,4,'A'), 41},
                { new Tuple<int, int, char>(9,5,'B'), 19},  { new Tuple<int, int, char>(9,5,'C'), 7},   { new Tuple<int, int, char>(9,5,'S'), 8},   { new Tuple<int, int, char>(9,5,'R'), 8},   { new Tuple<int, int, char>(9,5,'A'), 42},
                { new Tuple<int, int, char>(9,6,'B'), 20},  { new Tuple<int, int, char>(9,6,'C'), 7},   { new Tuple<int, int, char>(9,6,'S'), 8},   { new Tuple<int, int, char>(9,6,'R'), 8},   { new Tuple<int, int, char>(9,6,'A'), 43},
                { new Tuple<int, int, char>(10,1,'B'), 20}, { new Tuple<int, int, char>(10,1,'C'), 7},  { new Tuple<int, int, char>(10,1,'S'), 8},  { new Tuple<int, int, char>(10,1,'R'), 9},  { new Tuple<int, int, char>(10,1,'A'), 44},
                { new Tuple<int, int, char>(10,2,'B'), 21}, { new Tuple<int, int, char>(10,2,'C'), 7},  { new Tuple<int, int, char>(10,2,'S'), 8},  { new Tuple<int, int, char>(10,2,'R'), 9},  { new Tuple<int, int, char>(10,2,'A'), 45},
                { new Tuple<int, int, char>(10,3,'B'), 21}, { new Tuple<int, int, char>(10,3,'C'), 8},  { new Tuple<int, int, char>(10,3,'S'), 8},  { new Tuple<int, int, char>(10,3,'R'), 9},  { new Tuple<int, int, char>(10,3,'A'), 46},
                { new Tuple<int, int, char>(10,4,'B'), 22}, { new Tuple<int, int, char>(10,4,'C'), 8},  { new Tuple<int, int, char>(10,4,'S'), 8},  { new Tuple<int, int, char>(10,4,'R'), 9},  { new Tuple<int, int, char>(10,4,'A'), 47},
                { new Tuple<int, int, char>(10,5,'B'), 22}, { new Tuple<int, int, char>(10,5,'C'), 8},  { new Tuple<int, int, char>(10,5,'S'), 9},  { new Tuple<int, int, char>(10,5,'R'), 9},  { new Tuple<int, int, char>(10,5,'A'), 48},
                { new Tuple<int, int, char>(10,6,'B'), 23}, { new Tuple<int, int, char>(10,6,'C'), 8},  { new Tuple<int, int, char>(10,6,'S'), 9},  { new Tuple<int, int, char>(10,6,'R'), 9},  { new Tuple<int, int, char>(10,6,'A'), 49},
                { new Tuple<int, int, char>(11,1,'B'), 23}, { new Tuple<int, int, char>(11,1,'C'), 8},  { new Tuple<int, int, char>(11,1,'S'), 9},  { new Tuple<int, int, char>(11,1,'R'), 10}, { new Tuple<int, int, char>(11,1,'A'), 50},
                { new Tuple<int, int, char>(11,2,'B'), 24}, { new Tuple<int, int, char>(11,2,'C'), 8},  { new Tuple<int, int, char>(11,2,'S'), 9},  { new Tuple<int, int, char>(11,2,'R'), 10}, { new Tuple<int, int, char>(11,2,'A'), 51},
                { new Tuple<int, int, char>(11,3,'B'), 24}, { new Tuple<int, int, char>(11,3,'C'), 9},  { new Tuple<int, int, char>(11,3,'S'), 9},  { new Tuple<int, int, char>(11,3,'R'), 10}, { new Tuple<int, int, char>(11,3,'A'), 52},
                { new Tuple<int, int, char>(11,4,'B'), 25}, { new Tuple<int, int, char>(11,4,'C'), 9},  { new Tuple<int, int, char>(11,4,'S'), 9},  { new Tuple<int, int, char>(11,4,'R'), 10}, { new Tuple<int, int, char>(11,4,'A'), 53},
                { new Tuple<int, int, char>(11,5,'B'), 25}, { new Tuple<int, int, char>(11,5,'C'), 9},  { new Tuple<int, int, char>(11,5,'S'), 10}, { new Tuple<int, int, char>(11,5,'R'), 10}, { new Tuple<int, int, char>(11,5,'A'), 54},
                { new Tuple<int, int, char>(11,6,'B'), 26}, { new Tuple<int, int, char>(11,6,'C'), 9},  { new Tuple<int, int, char>(11,6,'S'), 10}, { new Tuple<int, int, char>(11,6,'R'), 10}, { new Tuple<int, int, char>(11,6,'A'), 55},
                { new Tuple<int, int, char>(12,1,'B'), 26}, { new Tuple<int, int, char>(12,1,'C'), 9},  { new Tuple<int, int, char>(12,1,'S'), 10}, { new Tuple<int, int, char>(12,1,'R'), 11}, { new Tuple<int, int, char>(12,1,'A'), 56},
                { new Tuple<int, int, char>(12,2,'B'), 27}, { new Tuple<int, int, char>(12,2,'C'), 9},  { new Tuple<int, int, char>(12,2,'S'), 10}, { new Tuple<int, int, char>(12,2,'R'), 11}, { new Tuple<int, int, char>(12,2,'A'), 57},
                { new Tuple<int, int, char>(12,3,'B'), 27}, { new Tuple<int, int, char>(12,3,'C'), 10}, { new Tuple<int, int, char>(12,3,'S'), 10}, { new Tuple<int, int, char>(12,3,'R'), 11}, { new Tuple<int, int, char>(12,3,'A'), 58},
                { new Tuple<int, int, char>(12,4,'B'), 28}, { new Tuple<int, int, char>(12,4,'C'), 10}, { new Tuple<int, int, char>(12,4,'S'), 10}, { new Tuple<int, int, char>(12,4,'R'), 11}, { new Tuple<int, int, char>(12,4,'A'), 59},
                { new Tuple<int, int, char>(12,5,'B'), 28}, { new Tuple<int, int, char>(12,5,'C'), 10}, { new Tuple<int, int, char>(12,5,'S'), 11}, { new Tuple<int, int, char>(12,5,'R'), 11}, { new Tuple<int, int, char>(12,5,'A'), 60},
                { new Tuple<int, int, char>(12,6,'B'), 29}, { new Tuple<int, int, char>(12,6,'C'), 10}, { new Tuple<int, int, char>(12,6,'S'), 11}, { new Tuple<int, int, char>(12,6,'R'), 11}, { new Tuple<int, int, char>(12,6,'A'), 61},
                { new Tuple<int, int, char>(13,1,'B'), 29}, { new Tuple<int, int, char>(13,1,'C'), 10}, { new Tuple<int, int, char>(13,1,'S'), 11}, { new Tuple<int, int, char>(13,1,'R'), 12}, { new Tuple<int, int, char>(13,1,'A'), 62},
                { new Tuple<int, int, char>(13,2,'B'), 30}, { new Tuple<int, int, char>(13,2,'C'), 10}, { new Tuple<int, int, char>(13,2,'S'), 11}, { new Tuple<int, int, char>(13,2,'R'), 12}, { new Tuple<int, int, char>(13,2,'A'), 63},
                { new Tuple<int, int, char>(13,3,'B'), 30}, { new Tuple<int, int, char>(13,3,'C'), 11}, { new Tuple<int, int, char>(13,3,'S'), 11}, { new Tuple<int, int, char>(13,3,'R'), 12}, { new Tuple<int, int, char>(13,3,'A'), 64},
                { new Tuple<int, int, char>(13,4,'B'), 31}, { new Tuple<int, int, char>(13,4,'C'), 11}, { new Tuple<int, int, char>(13,4,'S'), 11}, { new Tuple<int, int, char>(13,4,'R'), 12}, { new Tuple<int, int, char>(13,4,'A'), 65},
                { new Tuple<int, int, char>(13,5,'B'), 31}, { new Tuple<int, int, char>(13,5,'C'), 11}, { new Tuple<int, int, char>(13,5,'S'), 12}, { new Tuple<int, int, char>(13,5,'R'), 12}, { new Tuple<int, int, char>(13,5,'A'), 66},
                { new Tuple<int, int, char>(13,6,'B'), 32}, { new Tuple<int, int, char>(13,6,'C'), 11}, { new Tuple<int, int, char>(13,6,'S'), 12}, { new Tuple<int, int, char>(13,6,'R'), 12}, { new Tuple<int, int, char>(13,6,'A'), 67},
                { new Tuple<int, int, char>(14,1,'B'), 32}, { new Tuple<int, int, char>(14,1,'C'), 11}, { new Tuple<int, int, char>(14,1,'S'), 12}, { new Tuple<int, int, char>(14,1,'R'), 13}, { new Tuple<int, int, char>(14,1,'A'), 68},
                { new Tuple<int, int, char>(14,2,'B'), 33}, { new Tuple<int, int, char>(14,2,'C'), 11}, { new Tuple<int, int, char>(14,2,'S'), 12}, { new Tuple<int, int, char>(14,2,'R'), 13}, { new Tuple<int, int, char>(14,2,'A'), 69},
                { new Tuple<int, int, char>(14,3,'B'), 33}, { new Tuple<int, int, char>(14,3,'C'), 12}, { new Tuple<int, int, char>(14,3,'S'), 12}, { new Tuple<int, int, char>(14,3,'R'), 13}, { new Tuple<int, int, char>(14,3,'A'), 70},
                { new Tuple<int, int, char>(14,4,'B'), 34}, { new Tuple<int, int, char>(14,4,'C'), 12}, { new Tuple<int, int, char>(14,4,'S'), 12}, { new Tuple<int, int, char>(14,4,'R'), 13}, { new Tuple<int, int, char>(14,4,'A'), 71},
                { new Tuple<int, int, char>(14,5,'B'), 34}, { new Tuple<int, int, char>(14,5,'C'), 12}, { new Tuple<int, int, char>(14,5,'S'), 13}, { new Tuple<int, int, char>(14,5,'R'), 13}, { new Tuple<int, int, char>(14,5,'A'), 72},
                { new Tuple<int, int, char>(14,6,'B'), 35}, { new Tuple<int, int, char>(14,6,'C'), 12}, { new Tuple<int, int, char>(14,6,'S'), 13}, { new Tuple<int, int, char>(14,6,'R'), 13}, { new Tuple<int, int, char>(14,6,'A'), 73},
                { new Tuple<int, int, char>(15,1,'B'), 35}, { new Tuple<int, int, char>(15,1,'C'), 12}, { new Tuple<int, int, char>(15,1,'S'), 13}, { new Tuple<int, int, char>(15,1,'R'), 14}, { new Tuple<int, int, char>(15,1,'A'), 74},
                { new Tuple<int, int, char>(15,2,'B'), 36}, { new Tuple<int, int, char>(15,2,'C'), 12}, { new Tuple<int, int, char>(15,2,'S'), 13}, { new Tuple<int, int, char>(15,2,'R'), 14}, { new Tuple<int, int, char>(15,2,'A'), 75},
                { new Tuple<int, int, char>(15,3,'B'), 36}, { new Tuple<int, int, char>(15,3,'C'), 13}, { new Tuple<int, int, char>(15,3,'S'), 13}, { new Tuple<int, int, char>(15,3,'R'), 14}, { new Tuple<int, int, char>(15,3,'A'), 76},
                { new Tuple<int, int, char>(15,4,'B'), 37}, { new Tuple<int, int, char>(15,4,'C'), 13}, { new Tuple<int, int, char>(15,4,'S'), 13}, { new Tuple<int, int, char>(15,4,'R'), 14}, { new Tuple<int, int, char>(15,4,'A'), 77},
                { new Tuple<int, int, char>(15,5,'B'), 37}, { new Tuple<int, int, char>(15,5,'C'), 13}, { new Tuple<int, int, char>(15,5,'S'), 14}, { new Tuple<int, int, char>(15,5,'R'), 14}, { new Tuple<int, int, char>(15,5,'A'), 78},
                { new Tuple<int, int, char>(15,6,'B'), 38}, { new Tuple<int, int, char>(15,6,'C'), 13}, { new Tuple<int, int, char>(15,6,'S'), 14}, { new Tuple<int, int, char>(15,6,'R'), 14}, { new Tuple<int, int, char>(15,6,'A'), 79},
                { new Tuple<int, int, char>(16,1,'B'), 38}, { new Tuple<int, int, char>(16,1,'C'), 13}, { new Tuple<int, int, char>(16,1,'S'), 14}, { new Tuple<int, int, char>(16,1,'R'), 15}, { new Tuple<int, int, char>(16,1,'A'), 80},
                { new Tuple<int, int, char>(16,2,'B'), 39}, { new Tuple<int, int, char>(16,2,'C'), 13}, { new Tuple<int, int, char>(16,2,'S'), 14}, { new Tuple<int, int, char>(16,2,'R'), 15}, { new Tuple<int, int, char>(16,2,'A'), 81},
                { new Tuple<int, int, char>(16,3,'B'), 39}, { new Tuple<int, int, char>(16,3,'C'), 14}, { new Tuple<int, int, char>(16,3,'S'), 14}, { new Tuple<int, int, char>(16,3,'R'), 15}, { new Tuple<int, int, char>(16,3,'A'), 82},
                { new Tuple<int, int, char>(16,4,'B'), 40}, { new Tuple<int, int, char>(16,4,'C'), 14}, { new Tuple<int, int, char>(16,4,'S'), 14}, { new Tuple<int, int, char>(16,4,'R'), 15}, { new Tuple<int, int, char>(16,4,'A'), 83},
                { new Tuple<int, int, char>(16,5,'B'), 40}, { new Tuple<int, int, char>(16,5,'C'), 14}, { new Tuple<int, int, char>(16,5,'S'), 15}, { new Tuple<int, int, char>(16,5,'R'), 15}, { new Tuple<int, int, char>(16,5,'A'), 84},
                { new Tuple<int, int, char>(16,6,'B'), 41}, { new Tuple<int, int, char>(16,6,'C'), 14}, { new Tuple<int, int, char>(16,6,'S'), 15}, { new Tuple<int, int, char>(16,6,'R'), 15}, { new Tuple<int, int, char>(16,6,'A'), 85},
                { new Tuple<int, int, char>(17,1,'B'), 41}, { new Tuple<int, int, char>(17,1,'C'), 14}, { new Tuple<int, int, char>(17,1,'S'), 15}, { new Tuple<int, int, char>(17,1,'R'), 16}, { new Tuple<int, int, char>(17,1,'A'), 86},
                { new Tuple<int, int, char>(17,2,'B'), 42}, { new Tuple<int, int, char>(17,2,'C'), 14}, { new Tuple<int, int, char>(17,2,'S'), 15}, { new Tuple<int, int, char>(17,2,'R'), 16}, { new Tuple<int, int, char>(17,2,'A'), 87},
                { new Tuple<int, int, char>(17,3,'B'), 42}, { new Tuple<int, int, char>(17,3,'C'), 15}, { new Tuple<int, int, char>(17,3,'S'), 15}, { new Tuple<int, int, char>(17,3,'R'), 16}, { new Tuple<int, int, char>(17,3,'A'), 88},
                { new Tuple<int, int, char>(17,4,'B'), 43}, { new Tuple<int, int, char>(17,4,'C'), 15}, { new Tuple<int, int, char>(17,4,'S'), 15}, { new Tuple<int, int, char>(17,4,'R'), 16}, { new Tuple<int, int, char>(17,4,'A'), 89},
                { new Tuple<int, int, char>(17,5,'B'), 43}, { new Tuple<int, int, char>(17,5,'C'), 15}, { new Tuple<int, int, char>(17,5,'S'), 16}, { new Tuple<int, int, char>(17,5,'R'), 16}, { new Tuple<int, int, char>(17,5,'A'), 90},
                { new Tuple<int, int, char>(17,6,'B'), 44}, { new Tuple<int, int, char>(17,6,'C'), 15}, { new Tuple<int, int, char>(17,6,'S'), 16}, { new Tuple<int, int, char>(17,6,'R'), 16}, { new Tuple<int, int, char>(17,6,'A'), 91},
                { new Tuple<int, int, char>(18,1,'B'), 44}, { new Tuple<int, int, char>(18,1,'C'), 15}, { new Tuple<int, int, char>(18,1,'S'), 16}, { new Tuple<int, int, char>(18,1,'R'), 17}, { new Tuple<int, int, char>(18,1,'A'), 92},
                { new Tuple<int, int, char>(18,2,'B'), 45}, { new Tuple<int, int, char>(18,2,'C'), 15}, { new Tuple<int, int, char>(18,2,'S'), 16}, { new Tuple<int, int, char>(18,2,'R'), 17}, { new Tuple<int, int, char>(18,2,'A'), 93},
                { new Tuple<int, int, char>(18,3,'B'), 45}, { new Tuple<int, int, char>(18,3,'C'), 16}, { new Tuple<int, int, char>(18,3,'S'), 16}, { new Tuple<int, int, char>(18,3,'R'), 17}, { new Tuple<int, int, char>(18,3,'A'), 94},
                { new Tuple<int, int, char>(18,4,'B'), 46}, { new Tuple<int, int, char>(18,4,'C'), 16}, { new Tuple<int, int, char>(18,4,'S'), 16}, { new Tuple<int, int, char>(18,4,'R'), 17}, { new Tuple<int, int, char>(18,4,'A'), 95},
                { new Tuple<int, int, char>(18,5,'B'), 46}, { new Tuple<int, int, char>(18,5,'C'), 16}, { new Tuple<int, int, char>(18,5,'S'), 17}, { new Tuple<int, int, char>(18,5,'R'), 17}, { new Tuple<int, int, char>(18,5,'A'), 96},
                { new Tuple<int, int, char>(18,6,'B'), 47}, { new Tuple<int, int, char>(18,6,'C'), 16}, { new Tuple<int, int, char>(18,6,'S'), 17}, { new Tuple<int, int, char>(18,6,'R'), 17}, { new Tuple<int, int, char>(18,6,'A'), 97},
                { new Tuple<int, int, char>(19,1,'B'), 47}, { new Tuple<int, int, char>(19,1,'C'), 16}, { new Tuple<int, int, char>(19,1,'S'), 17}, { new Tuple<int, int, char>(19,1,'R'), 18}, { new Tuple<int, int, char>(19,1,'A'), 98},
                { new Tuple<int, int, char>(19,2,'B'), 48}, { new Tuple<int, int, char>(19,2,'C'), 16}, { new Tuple<int, int, char>(19,2,'S'), 17}, { new Tuple<int, int, char>(19,2,'R'), 18}, { new Tuple<int, int, char>(19,2,'A'), 99},
                { new Tuple<int, int, char>(19,3,'B'), 48}, { new Tuple<int, int, char>(19,3,'C'), 17}, { new Tuple<int, int, char>(19,3,'S'), 17}, { new Tuple<int, int, char>(19,3,'R'), 18}, { new Tuple<int, int, char>(19,3,'A'), 100},
                { new Tuple<int, int, char>(19,4,'B'), 49}, { new Tuple<int, int, char>(19,4,'C'), 17}, { new Tuple<int, int, char>(19,4,'S'), 17}, { new Tuple<int, int, char>(19,4,'R'), 18}, { new Tuple<int, int, char>(19,4,'A'), 101},
                { new Tuple<int, int, char>(19,5,'B'), 49}, { new Tuple<int, int, char>(19,5,'C'), 17}, { new Tuple<int, int, char>(19,5,'S'), 18}, { new Tuple<int, int, char>(19,5,'R'), 18}, { new Tuple<int, int, char>(19,5,'A'), 102},
                { new Tuple<int, int, char>(19,6,'B'), 50}, { new Tuple<int, int, char>(19,6,'C'), 17}, { new Tuple<int, int, char>(19,6,'S'), 18}, { new Tuple<int, int, char>(19,6,'R'), 18}, { new Tuple<int, int, char>(19,6,'A'), 103},
                { new Tuple<int, int, char>(20,1,'B'), 50}, { new Tuple<int, int, char>(20,1,'C'), 17}, { new Tuple<int, int, char>(20,1,'S'), 18}, { new Tuple<int, int, char>(20,1,'R'), 19}, { new Tuple<int, int, char>(20,1,'A'), 104},
                { new Tuple<int, int, char>(20,2,'B'), 51}, { new Tuple<int, int, char>(20,2,'C'), 17}, { new Tuple<int, int, char>(20,2,'S'), 18}, { new Tuple<int, int, char>(20,2,'R'), 19}, { new Tuple<int, int, char>(20,2,'A'), 105},
                { new Tuple<int, int, char>(20,3,'B'), 51}, { new Tuple<int, int, char>(20,3,'C'), 18}, { new Tuple<int, int, char>(20,3,'S'), 18}, { new Tuple<int, int, char>(20,3,'R'), 19}, { new Tuple<int, int, char>(20,3,'A'), 106},
                { new Tuple<int, int, char>(20,4,'B'), 52}, { new Tuple<int, int, char>(20,4,'C'), 18}, { new Tuple<int, int, char>(20,4,'S'), 18}, { new Tuple<int, int, char>(20,4,'R'), 19}, { new Tuple<int, int, char>(20,4,'A'), 107},
                { new Tuple<int, int, char>(20,5,'B'), 52}, { new Tuple<int, int, char>(20,5,'C'), 18}, { new Tuple<int, int, char>(20,5,'S'), 19}, { new Tuple<int, int, char>(20,5,'R'), 19}, { new Tuple<int, int, char>(20,5,'A'), 108},
                { new Tuple<int, int, char>(20,6,'B'), 53}, { new Tuple<int, int, char>(20,6,'C'), 18}, { new Tuple<int, int, char>(20,6,'S'), 19}, { new Tuple<int, int, char>(20,6,'R'), 19}, { new Tuple<int, int, char>(20,6,'A'), 109},
                { new Tuple<int, int, char>(21,1,'B'), 53}, { new Tuple<int, int, char>(21,1,'C'), 18}, { new Tuple<int, int, char>(21,1,'S'), 19}, { new Tuple<int, int, char>(21,1,'R'), 20}, { new Tuple<int, int, char>(21,1,'A'), 110},
                { new Tuple<int, int, char>(21,2,'B'), 54}, { new Tuple<int, int, char>(21,2,'C'), 18}, { new Tuple<int, int, char>(21,2,'S'), 19}, { new Tuple<int, int, char>(21,2,'R'), 20}, { new Tuple<int, int, char>(21,2,'A'), 111},
                { new Tuple<int, int, char>(21,3,'B'), 54}, { new Tuple<int, int, char>(21,3,'C'), 19}, { new Tuple<int, int, char>(21,3,'S'), 19}, { new Tuple<int, int, char>(21,3,'R'), 20}, { new Tuple<int, int, char>(21,3,'A'), 112},
                { new Tuple<int, int, char>(21,4,'B'), 55}, { new Tuple<int, int, char>(21,4,'C'), 19}, { new Tuple<int, int, char>(21,4,'S'), 19}, { new Tuple<int, int, char>(21,4,'R'), 20}, { new Tuple<int, int, char>(21,4,'A'), 113},
                { new Tuple<int, int, char>(21,5,'B'), 55}, { new Tuple<int, int, char>(21,5,'C'), 19}, { new Tuple<int, int, char>(21,5,'S'), 20}, { new Tuple<int, int, char>(21,5,'R'), 20}, { new Tuple<int, int, char>(21,5,'A'), 114},
                { new Tuple<int, int, char>(21,6,'B'), 56}, { new Tuple<int, int, char>(21,6,'C'), 19}, { new Tuple<int, int, char>(21,6,'S'), 20}, { new Tuple<int, int, char>(21,6,'R'), 20}, { new Tuple<int, int, char>(21,6,'A'), 115},
                { new Tuple<int, int, char>(22,1,'B'), 56}, { new Tuple<int, int, char>(22,1,'C'), 19}, { new Tuple<int, int, char>(22,1,'S'), 20}, { new Tuple<int, int, char>(22,1,'R'), 21}, { new Tuple<int, int, char>(22,1,'A'), 116}
        };
    }
}
