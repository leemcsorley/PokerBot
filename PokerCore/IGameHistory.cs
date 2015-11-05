using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public interface IGameHistory
    {
        IPlayer[] Players { get; }

        PlayerState[] PlayerStates { get; }

        IPlayer Dealer { get; }

        int DealerPosition { get; }

        Card[] Flop { get; }

        Card? Turn { get; }

        Card? River { get; }

        IPlayer Winner { get; }

        uint Winnings { get; set; }

        List<PlayerAction> PreFlopActions { get; }

        List<PlayerAction> FlopActions { get; }

        List<PlayerAction> TurnActions { get; }

        List<PlayerAction> RiverActions { get; }
    }
}
