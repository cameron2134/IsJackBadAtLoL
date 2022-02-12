using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class MatchDataComment
    {
        public int ID { get; set; }
        public int MatchDataID { get; set; }
        public string Comment { get; set; }

        public virtual MatchData MatchData { get; set; }

    }
}
