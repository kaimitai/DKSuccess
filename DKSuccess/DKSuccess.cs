using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKSuccess {
    class DKSuccess {
        static void Main(string[] args) {
            Database db = new Database();
            //db.AddGame("5-3,5-4,5-5,8-5 251100");
            db.Save();
        }
    }
}
