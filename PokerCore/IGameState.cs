using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public interface IGameState
    {
        IPlayer[] Players { get; }

        PlayerState[] PlayerStates { get; }

        IPlayer Dealer { get; }

        int DealerPosition { get; }

        Round CurrentRound { get; }

        PlayerAction LastAction { get; }

        Card[] Flop { get; }

        Card Turn { get; }

        Card River { get; }

        uint CurrentBet { get; }

        uint Pot { get; }

        bool IsFinished { get; }

        IPlayer Winner { get; }

        List<PlayerAction> AllActions { get; }
    }
}
