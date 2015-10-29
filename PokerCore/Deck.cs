using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public class Deck
    {
        private Random _rnd;
        private Card[] _cards = new Card[52];
        private int _card = 0;

        public Deck(Random rnd)
        {
            _rnd = rnd;
            Initialise();
        }

        private void Initialise()
        {
            var c = 0;
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 13; j++)
                {
                    _cards[c++] = new Card((CardRank)j, (CardSuit)i);
                }
            }
        }

        public void Shuffle()
        {
            for (int i = 51; i > 0; i--)
            {
                int j = _rnd.Next(i + 1);
                Card temp = _cards[i]; // Notice the change on this line
                _cards[i] = _cards[j];
                _cards[j] = temp;
            }
        }

        public void Reset()
        {
            _card = 0;
        }

        public void ResetAndShuffle()
        {
            Reset();
            Shuffle();
        }

        public Card DealCard()
        {
            return _cards[_card++];
        }
    }
}
