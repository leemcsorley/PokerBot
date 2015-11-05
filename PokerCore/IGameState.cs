using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public interface IGameState : IGameHistory
    {
        Round CurrentRound { get; }

        PlayerAction LastAction { get; }

        uint CurrentBet { get; }

        uint Pot { get; }

        bool IsFinished { get; }
    }
}
