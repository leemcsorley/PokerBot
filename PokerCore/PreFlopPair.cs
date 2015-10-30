using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public struct PreFlopPair
    {
        public PreFlopPair(CardRank card1, CardRank card2, bool suited)
        {
            Card1 = card1;
            Card2 = card2;
            Suited = suited;
        }

        public CardRank Card1;

        public CardRank Card2;

        public bool Suited;

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Card1, Card2, Suited ? "s" : "o");
        }
    }
}
