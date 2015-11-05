using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    [Serializable]
    public struct Action
    {
        public Action(ActionType type, ushort amount)
        {
            Type = type;
            Amount = amount;
        }

        public ActionType Type;

        public ushort Amount;
    }
}
