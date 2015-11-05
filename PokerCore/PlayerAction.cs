using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    [Serializable]
    public struct PlayerAction
    {
        public PlayerAction(int playerIndex, Action action)
        {
            PlayerIndex = playerIndex;
            Action = action;
        }

        public int PlayerIndex;

        public Action Action;
    }
}
