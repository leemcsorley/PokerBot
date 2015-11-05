using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore.Data
{
    public class FullHistory
    {
        private static Card ParseCard(string card)
        {
            CardRank rank = CardRank.Ace;
            CardSuit suit = CardSuit.Clubs;
            switch (card[0])
            {
                case 'A':
                    rank = CardRank.Ace;
                    break;
                case 'K':
                    rank = CardRank.King;
                    break;
                case 'Q':
                    rank = CardRank.Queen;
                    break;
                case 'J':
                    rank = CardRank.Jack;
                    break;
                case 'T':
                    rank = CardRank.Ten;
                    break;
                default:
                    rank = (CardRank)(Int32.Parse(card[0].ToString()) - 2);
                    break;
            }
            switch (card[1])
            {
                case 's':
                    suit = CardSuit.Spades;
                    break;
                case 'c':
                    suit = CardSuit.Clubs;
                    break;
                case 'd':
                    suit = CardSuit.Diamonds;
                    break;
                case 'h':
                    suit = CardSuit.Hearts;
                    break;
            }
            return new Card(rank, suit);
        }

        private static IGameHistory LoadHistoryHHFromText(string text)
        {
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Any(l => l.Trim() == ""))
                return null;
            var haspos = lines[0].IndexOf('#');
            var colonpos = lines[0].IndexOf(':');
            var num = lines[0].Substring(haspos + 1, colonpos - haspos - 1);
            haspos = lines[1].IndexOf('#');
            var dealerSeat = Int32.Parse(lines[1].Substring(haspos + 1, 1));
            int dealerPos = 0;
            int bracketpos = lines[0].IndexOf('(');
            int slashpos = lines[0].IndexOf('/');
       
            uint smallBlind = (uint)(100 * Decimal.Parse(lines[0].Substring(bracketpos + 2, slashpos - bracketpos - 2)));
            uint bigBlind = (uint)(100 * Decimal.Parse(lines[0].Substring(slashpos + 2, lines[0].IndexOf(' ', slashpos) - 2 - slashpos)));

            // players
            int i = 2, pc = 0;
            List<PlayerHistorical> players = new List<PlayerHistorical>();
            Dictionary<int, PlayerHistorical> seatIndex = new Dictionary<int, PlayerHistorical>();
            List<int> seats = new List<int>();
            while (lines[i].Contains("Seat"))
            {
                colonpos = lines[i].IndexOf(':');
                var n = Int32.Parse(lines[i].Substring(5, colonpos - 5));
                if (n == dealerSeat)
                    dealerPos = pc;
                seats.Add(n);

                var id = lines[i].Substring(colonpos + 2, lines[i].LastIndexOf('(') - colonpos - 3);
                var dollarpos = lines[i].LastIndexOf('$');
                var chips = (uint)(100 * Decimal.Parse(lines[i].Substring(dollarpos + 1, lines[i].IndexOf(' ', dollarpos) - dollarpos - 1)));
                PlayerHistorical p;
                players.Add(p = new PlayerHistorical() { Id = id, Balance = chips });
                seatIndex.Add(n, p);
                pc++;
                i++;
            }

            bool[] folded = new bool[players.Count];
            bool[] allin = new bool[players.Count];
            while (!lines[i++].Contains("*** HOLE")) ;

            uint currentBet = bigBlind;
            uint pot = smallBlind + bigBlind;
            int playerPos = (dealerPos + 3) % players.Count;
            if (players.Count == 2)
                playerPos = dealerPos;
            int canbetcount = players.Count;
            // preflop
            int lastPos = 0;

            bool noRaise = true;
            Action<List<PlayerAction>> addActions = list =>
                {
                    while (!(lines[i].StartsWith("*** ") || lines[i].StartsWith("Uncalled bet (") || lines[i].Contains(" collected ")))
                    {
                        if (lines[i].Contains(" folds "))
                        {
                            list.Add(new PlayerAction(playerPos, new Action(ActionType.Fold, 0)));
                            folded[playerPos] = true;
                            canbetcount--;
                        }
                        else if (lines[i].Contains(" calls ") || lines[i].Contains(" checks "))
                        {
                            list.Add(new PlayerAction(playerPos, new Action(ActionType.Call, 0)));
                            pot += currentBet;
                            if (lines[i].Contains(" all-in"))
                            {
                                allin[playerPos] = true;
                                canbetcount--;
                            }
                        }
                        else if (lines[i].Contains(" raises ") || lines[i].Contains(" bets "))
                        {
                             var dpos = lines[i].LastIndexOf("$");
                            var spos = lines[i].IndexOf(' ', dpos);
                            uint raise = (uint)(100 * Decimal.Parse(lines[i].Substring(dpos + 1, (spos > -1 ? spos : lines[i].Length) - dpos - 1)));
                            list.Add(new PlayerAction(playerPos, new Action(ActionType.Raise, (ushort)(raise - currentBet))));
                            currentBet = raise;
                            pot += currentBet;
                            if (lines[i].Contains(" all-in"))
                            {
                                allin[playerPos] = true;
                                canbetcount--;
                            }
                            lastPos = playerPos;
                            noRaise = false;
                        } else
                        {
                            i++;
                            continue;
                        }
                        if (!lines[i].Contains(players[playerPos].Id)) //&& players.Count != 2)
                        {

                        }
                        if (canbetcount != 0)
                            do
                            {
                                playerPos = (playerPos + 1) % players.Count;
                            }
                            while (folded[playerPos] || allin[playerPos]);
                        i++;
                    }
                };

            var history = new GameHistory();
            history.Players = players.ToArray();
            history.SmallBlind = smallBlind;
            history.BigBlind = bigBlind;
            history.DealerPosition = dealerPos;
            history.Dealer = players[dealerPos];
            history.TableId = num;
            history.SiteName = "Poker Stars";

            noRaise = true;
            addActions(history.PreFlopActions);

            if (!(lines[i].Contains("Uncalled bet (") || lines[i].Contains(" collected ")))
            {
                // flop
                var flop = lines[i].Substring(lines[i++].IndexOf('[') + 1, 8).Split(' ').Select(c => ParseCard(c)).ToArray();
                history.Flop = flop;
                playerPos = dealerPos;
                if (canbetcount != 0)
                    do { playerPos = (playerPos + 1) % players.Count; } while (folded[playerPos] || allin[playerPos]);
                currentBet = 0;
                noRaise = true;
                addActions(history.FlopActions);
                if (!(lines[i].Contains("Uncalled bet (") || lines[i].Contains(" collected ")))
                {
                    var turn = ParseCard(lines[i].Substring(lines[i++].LastIndexOf('[') + 1, 2));
                    history.Turn = turn;
                    playerPos = dealerPos;
                    if (canbetcount != 0)
                        do { playerPos = (playerPos + 1) % players.Count; } while (folded[playerPos] || allin[playerPos]);
                    currentBet = 0;
                    noRaise = true;
                    addActions(history.TurnActions);
                    if (!(lines[i].Contains("Uncalled bet (") || lines[i].Contains(" collected ")))
                    {
                        var river = ParseCard(lines[i].Substring(lines[i++].LastIndexOf('[') + 1, 2));
                        history.River = river;
                        playerPos = dealerPos;
                        if (canbetcount != 0)
                            do { playerPos = (playerPos + 1) % players.Count; } while (folded[playerPos] || allin[playerPos]);
                        currentBet = 0;
                        noRaise = true;
                        addActions(history.RiverActions);
                    }
                }
            }

            if (lines[i].Contains("Uncalled bet (") || lines[i].Contains(" collected "))
            {
                history.Winner = players[folded.Select((f, id) => new { f = f, i = id }).Where(f => !f.f).First().i];
                history.Winnings = pot;
            }
            else
            {
                int p = 0;
                foreach (var s in seats)
                {
                    if (folded[p++])
                        continue;
                    while (i < lines.Length && !lines[i].StartsWith("Seat " + s + ":")) { i++; }
                    if (i < lines.Length && lines[i].Contains("showed ["))
                    {
                        var hcstr = lines[i].Substring(lines[i].LastIndexOf("showed [") + 8, 5);
                        var cp = new CardPair(ParseCard(hcstr.Substring(0, 2)), ParseCard(hcstr.Substring(3, 2)));
                        seatIndex[s].HoleCards = cp;
                    }
                }
            }

            return history;
        }

        private static IEnumerable<IGameHistory> LoadHistoryHHFromZip(string zipFile)
        {
            using (ZipFile zip = ZipFile.Read(zipFile))    
            {
                foreach (ZipEntry entry in zip)
                {
                    using (var stream = entry.OpenReader())
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        var text = sr.ReadToEnd();
                        var split = text.Split(new[] { "PokerStars Hand #" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var game in split)
                            if (!game.Contains("Hand was run twice") && !game.Contains("deniPokerStars") && game.Contains("*** SUMMARY ***"))
                            {
                                var tgame = game.Trim();
                                var h = LoadHistoryHHFromText("PokerStars Hand #" + tgame);
                                if (h != null)
                                    yield return h;
                            }
                    }
                }
            }
        }

        public static IEnumerable<IGameHistory> LoadHistoryHH(string folderPath)
        {
            foreach (var file in Directory.GetFiles(folderPath, "*.zip"))
            {
                foreach (var history in LoadHistoryHHFromZip(file))
                    yield return history;
            }
        }

        public static IEnumerable<IGameHistory> LoadHistoryBin(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            using (System.IO.Compression.GZipStream zs = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Decompress))
            {
                while (fs.Position != fs.Length)
                    yield return (IGameHistory)bf.Deserialize(zs);
            }
        }
    }
}
