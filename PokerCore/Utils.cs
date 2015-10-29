using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public static class Utils
    {
        public static string ToDisplay(this CardRank rank)
        {
            if (rank == CardRank.Ace)
                return "A";
            if (rank == CardRank.King)
                return "K";
            if (rank == CardRank.Queen)
                return "Q";
            if (rank == CardRank.Jack)
                return "J";

            return (((int)rank) + 1).ToString();
        }
    }
}
