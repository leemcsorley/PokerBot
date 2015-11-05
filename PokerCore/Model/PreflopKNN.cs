using PokerCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore.Model
{
    [Serializable]
    public class PreflopKNN
    {
        public PreflopKNN(IEnumerable<IGameHistory> history)
        {
            CreateModel(history);
        }

        private struct ActionDetail
        {
            public ActionType Type;
            public uint Amount;
            public uint RaiseTo;
        }

        private class Summary
        {
            public double CallRatio { get; set; }

            public double RaiseRatio { get; set; }

            public double FoldRatio { get; set; }
        }

        private void CreateModel(IEnumerable<IGameHistory> history)
        {
            Dictionary<PreflopPair, List<ActionDetail>> actions = new Dictionary<PreflopPair, List<ActionDetail>>();
            Dictionary<PreflopPair, Summary> summary = new Dictionary<PreflopPair, Summary>();
            foreach (var game in history)
            {
                var players = (PlayerHistorical[])game.Players;

                if (players.Any(p => p.HoleCards.HasValue))
                {
                    uint currentBet = ((GameHistory)game).BigBlind;
                    foreach (var a in game.PreFlopActions)
                    {
                        PlayerHistorical p;
                        if ((p = players[a.PlayerIndex]).HoleCards.HasValue)
                        {
                            if (!actions.ContainsKey(p.HoleCards.Value.PreflopPair))
                                actions[p.HoleCards.Value.PreflopPair] = new List<ActionDetail>();

                            actions[p.HoleCards.Value.PreflopPair].Add(new ActionDetail() { Type = a.Action.Type, Amount = a.Action.Type == ActionType.Fold ? 0 : currentBet, RaiseTo = a.Action.Type == ActionType.Raise ? currentBet + a.Action.Amount : 0 });
                        }

                        if (a.Action.Type == ActionType.Raise)
                            currentBet += a.Action.Amount;
                    }
                }
            }
            foreach (var pair in actions.Keys)
            {
                int call = 0, raise = 0, fold = 0, total = 0;
                foreach (var a in actions[pair])
                {
                    if (a.Type == ActionType.Call)
                        call++;
                    if (a.Type == ActionType.Raise)
                        raise++;
                    if (a.Type == ActionType.Fold)
                        fold++;
                    total++;
                }
                summary[pair] = new Summary()
                {
                    CallRatio = (double)call / (double)total,
                    RaiseRatio = (double)raise / (double)total,
                    FoldRatio = (double)fold / (double)total
                };
            }
        }
    }
}
