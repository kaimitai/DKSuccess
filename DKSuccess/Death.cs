using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKSuccess
{
    class Death : IComparable<Death>
    {

        int Level;
        int Board;

        public Death(string death) {
            try {
                Level = int.Parse(death.Substring(0, 1));
                Board = int.Parse(death.Substring(2, 1));
            } catch (Exception ex) {
                throw new Exception("Could not instantiate Death from string " + death + ". Expected form; \"L-B\"" + Environment.NewLine + "Exception message: " + ex.Message);
            }
        }

        public Death(int lvl, int board) {
            Level = lvl;
            Board = board;
        }

        public int CompareTo(Death rhs) {
            if (this > rhs)
                return (1);
            else if (this < rhs)
                return (-1);
            else
                return (0);
        }

        public override string ToString() {
            return Level.ToString() + "-" + Board.ToString();
        }

        public static bool operator <(Death d1, Death d2) {
            return (d1.Level < d2.Level || (d1.Level == d2.Level && d1.Board < d2.Board));
        }

        public static bool operator >(Death d1, Death d2) {
            return (d1.Level > d2.Level || (d1.Level == d2.Level && d1.Board > d2.Board));
        }

        // Check if a death fulfils the criteria given
        public bool IsType(int minLevel, int maxLevel, char boardType = 'A') {
            if (Level < minLevel || Level > maxLevel)
                return (false);

            if (boardType == 'A' || GetBoardType() == boardType)
                return (true);
            else
                return (false);
        }


        // is this death on a [B]arrel, [S]pring, [C]onveyor or [R]ivet board?
        public char GetBoardType() {
            // from level five on, the pattern repeats
            int lvl = Math.Min(5, Level);

            if (Board == lvl + 1)
                return 'R';
            else if (Board == 2)
                return (Level == 2 ? 'S' : 'C');
            else if (Board == 3)
                return (Level == 3 ? 'S' : 'B');
            else if (Board == 4)
                return ('S');
            else
                return ('B');
        }
    }
}
