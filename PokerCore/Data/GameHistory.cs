using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore.Data
{
    [Serializable]
    public class GameHistory : IGameHistory
    {
        public GameHistory()
        {
            PreFlopActions = new List<PlayerAction>();
            FlopActions = new List<PlayerAction>();
            TurnActions = new List<PlayerAction>();
            RiverActions = new List<PlayerAction>();
        }

        public string SiteName { get; set; }

        public string TableId { get; set; }

        public uint SmallBlind { get; set; }

        public uint BigBlind { get; set; }

        public IPlayer[] Players { get; set; }

        public PlayerState[] PlayerStates { get; set; }

        public IPlayer Dealer { get; set; }

        public int DealerPosition { get; set; }

        public Card[] Flop { get; set; }

        public Card? Turn { get; set; }

        public Card? River { get; set; }

        public IPlayer Winner { get; set; }

        public uint Winnings { get; set; }

        public List<PlayerAction> PreFlopActions { get; private set; }

        public List<PlayerAction> FlopActions { get; private set; }

        public List<PlayerAction> TurnActions { get; private set; }

        public List<PlayerAction> RiverActions { get; private set; }
    }
}
