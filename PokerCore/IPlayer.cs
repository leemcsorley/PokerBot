using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public interface IPlayer
    {
        string Id { get; set; }

        uint Balance { get; set; }

        Action GetAction(IGameState state);
    }
}
