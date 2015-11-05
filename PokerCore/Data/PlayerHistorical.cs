using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore.Data
{
    [Serializable]
    public class PlayerHistorical : IPlayer
    {
        public string Id { get; set; }

        public uint Balance { get; set; }

        public CardPair? HoleCards { get; set; }

        public Action GetAction(IGameState state)
        {
            throw new System.Exception("Historical players cannot create actions");
        }
    }
}
