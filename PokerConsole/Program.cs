using PokerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var rnd = new MathNet.Numerics.Random.MersenneTwister();
            var deck = new Deck(rnd);

            while (true)
            {
                deck.ResetAndShuffle();
                Card[] c;
                Hand.Sort7Rank(c = new Card[] { deck.DealCard(), deck.DealCard(), deck.DealCard(), deck.DealCard(), deck.DealCard(), deck.DealCard(), deck.DealCard() });

                var p = CardRank.Two;
                for (var i = 0; i < c.Length; i++)
                {
                    if (c[i].Rank < p)
                        throw new Exception();
                    p = c[i].Rank;
                }
            }
        }
    }
}
