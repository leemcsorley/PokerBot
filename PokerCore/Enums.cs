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
}
