using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public struct Card
    {
        public Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public CardSuit Suit;

        public CardRank Rank;

        public override string ToString()
        {
            return string.Format("{0} {1}", Rank, Suit);
        }

        #region Cards
        public static readonly Card AceHearts = new Card(CardRank.Ace, CardSuit.Hearts);
        public static readonly Card TwoHearts = new Card(CardRank.Two, CardSuit.Hearts);
        public static readonly Card ThreeHearts = new Card(CardRank.Three, CardSuit.Hearts);
        public static readonly Card FourHearts = new Card(CardRank.Four, CardSuit.Hearts);
        public static readonly Card FiveHearts = new Card(CardRank.Five, CardSuit.Hearts);
        public static readonly Card SixHearts = new Card(CardRank.Six, CardSuit.Hearts);
        public static readonly Card SevenHearts = new Card(CardRank.Seven, CardSuit.Hearts);
        public static readonly Card EightHearts = new Card(CardRank.Eight, CardSuit.Hearts);
        public static readonly Card NineHearts = new Card(CardRank.Nine, CardSuit.Hearts);
        public static readonly Card TenHearts = new Card(CardRank.Ten, CardSuit.Hearts);
        public static readonly Card JackHearts = new Card(CardRank.Jack, CardSuit.Hearts);
        public static readonly Card QueenHearts = new Card(CardRank.Queen, CardSuit.Hearts);
        public static readonly Card KingHearts = new Card(CardRank.King, CardSuit.Hearts);

        public static readonly Card AceDiamonds = new Card(CardRank.Ace, CardSuit.Diamonds);
        public static readonly Card TwoDiamonds = new Card(CardRank.Two, CardSuit.Diamonds);
        public static readonly Card ThreeDiamonds = new Card(CardRank.Three, CardSuit.Diamonds);
        public static readonly Card FourDiamonds = new Card(CardRank.Four, CardSuit.Diamonds);
        public static readonly Card FiveDiamonds = new Card(CardRank.Five, CardSuit.Diamonds);
        public static readonly Card SixDiamonds = new Card(CardRank.Six, CardSuit.Diamonds);
        public static readonly Card SevenDiamonds = new Card(CardRank.Seven, CardSuit.Diamonds);
        public static readonly Card EightDiamonds = new Card(CardRank.Eight, CardSuit.Diamonds);
        public static readonly Card NineDiamonds = new Card(CardRank.Nine, CardSuit.Diamonds);
        public static readonly Card TenDiamonds = new Card(CardRank.Ten, CardSuit.Diamonds);
        public static readonly Card JackDiamonds = new Card(CardRank.Jack, CardSuit.Diamonds);
        public static readonly Card QueenDiamonds = new Card(CardRank.Queen, CardSuit.Diamonds);
        public static readonly Card KingDiamonds = new Card(CardRank.King, CardSuit.Diamonds);

        public static readonly Card AceClubs = new Card(CardRank.Ace, CardSuit.Clubs);
        public static readonly Card TwoClubs = new Card(CardRank.Two, CardSuit.Clubs);
        public static readonly Card ThreeClubs = new Card(CardRank.Three, CardSuit.Clubs);
        public static readonly Card FourClubs = new Card(CardRank.Four, CardSuit.Clubs);
        public static readonly Card FiveClubs = new Card(CardRank.Five, CardSuit.Clubs);
        public static readonly Card SixClubs = new Card(CardRank.Six, CardSuit.Clubs);
        public static readonly Card SevenClubs = new Card(CardRank.Seven, CardSuit.Clubs);
        public static readonly Card EightClubs = new Card(CardRank.Eight, CardSuit.Clubs);
        public static readonly Card NineClubs = new Card(CardRank.Nine, CardSuit.Clubs);
        public static readonly Card TenClubs = new Card(CardRank.Ten, CardSuit.Clubs);
        public static readonly Card JackClubs = new Card(CardRank.Jack, CardSuit.Clubs);
        public static readonly Card QueenClubs = new Card(CardRank.Queen, CardSuit.Clubs);
        public static readonly Card KingClubs = new Card(CardRank.King, CardSuit.Clubs);

        public static readonly Card AceSpades = new Card(CardRank.Ace, CardSuit.Spades);
        public static readonly Card TwoSpades = new Card(CardRank.Two, CardSuit.Spades);
        public static readonly Card ThreeSpades = new Card(CardRank.Three, CardSuit.Spades);
        public static readonly Card FourSpades = new Card(CardRank.Four, CardSuit.Spades);
        public static readonly Card FiveSpades = new Card(CardRank.Five, CardSuit.Spades);
        public static readonly Card SixSpades = new Card(CardRank.Six, CardSuit.Spades);
        public static readonly Card SevenSpades = new Card(CardRank.Seven, CardSuit.Spades);
        public static readonly Card EightSpades = new Card(CardRank.Eight, CardSuit.Spades);
        public static readonly Card NineSpades = new Card(CardRank.Nine, CardSuit.Spades);
        public static readonly Card TenSpades = new Card(CardRank.Ten, CardSuit.Spades);
        public static readonly Card JackSpades = new Card(CardRank.Jack, CardSuit.Spades);
        public static readonly Card QueenSpades = new Card(CardRank.Queen, CardSuit.Spades);
        public static readonly Card KingSpades = new Card(CardRank.King, CardSuit.Spades);
        #endregion
    }

    public struct CardPair
    {
        public CardPair(Card card1, Card card2)
        {
            Card1 = card1;
            Card2 = card2;
        }

        public Card Card1;
        public Card Card2;
    }
}
