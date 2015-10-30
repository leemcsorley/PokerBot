using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public enum CardSuit : ushort
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum CardRank : ushort
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public enum Round
    {
        PreFlop,
        Flop,
        Turn,
        River,
        End
    }

    public enum ActionType : ushort
    {
        Fold,
        Call,
        Raise
    }

    public enum PlayerState
    {
        In,
        Out
    }

    public enum HandType
    {
        /// <summary>
        /// Only a high card
        /// </summary>
        HighCard = 0,
        /// <summary>
        /// One Pair
        /// </summary>
        Pair = 1,
        /// <summary>
        /// Two Pair
        /// </summary>
        TwoPair = 2,
        /// <summary>
        /// Three of a kind (Trips)
        /// </summary>
        Trips = 3,
        /// <summary>
        /// Straight
        /// </summary>
        Straight = 4,
        /// <summary>
        /// Flush
        /// </summary>
        Flush = 5,
        /// <summary>
        /// FullHouse
        /// </summary>
        FullHouse = 6,
        /// <summary>
        /// Four of a kind
        /// </summary>
        FourOfAKind = 7,
        /// <summary>
        /// Straight Flush
        /// </summary>
        StraightFlush = 8
    }
}
