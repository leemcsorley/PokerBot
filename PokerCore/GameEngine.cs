using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public class GameEngine
    {
        private Random _rnd;
        private Deck _deck;
        private IPlayer[] _players;
        private ushort _small;
        private ushort _big;
        private int _dealerPosition;
        private int _smallPosition;
        private int _bigPosition;
        private CardPair[] _playerCards;
        private int _numPlayers;
        private GameState _state;

        private class GameState : IGameState
        {
            public GameState()
            {
                AllActions = new List<PlayerAction>();
                IsFinished = false;
            }

            public IPlayer[] Players { get; set; }

            public IPlayer Dealer { get; set; }

            public int DealerPosition { get; set; }

            public Round CurrentRound { get; set; }

            public PlayerAction LastAction { get; set; }

            public Card[] Flop { get; set; }

            public Card Turn { get; set; }

            public Card River { get; set; }

            public PlayerState[] PlayerStates { get; set; }

            public uint CurrentBet { get; set; }

            public uint Pot { get; set; }

            public bool IsFinished { get; set; }

            public IPlayer Winner { get; set; }

            public List<PlayerAction> AllActions { get; set; }
        }

        public GameEngine(IPlayer[] players, ushort small, ushort big, Random rnd)
        {
            _rnd = rnd;
            _deck = new Deck(_rnd);
            _players = players;
            _small = small;
            _big = big;
            _numPlayers = players.Length;
        }

        public void Initialise()
        {
            _dealerPosition = _rnd.Next(_players.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private int GetNextPosition(int position)
        {
            var t = position++;
            if (t >= _numPlayers)
                return 0;
            return t;
        }

        public void Next()
        {
            // next dealer
            _dealerPosition++;
            if (_dealerPosition >= _numPlayers)
                _dealerPosition = 0;

            // post blinds
            _players[_smallPosition = GetNextPosition(_dealerPosition)].Balance -= _small;
            _players[_bigPosition = GetNextPosition(_smallPosition)].Balance -= _big;

            // deal cards
            _playerCards = new CardPair[_numPlayers];
            for (int i = 0; i < _numPlayers; i++)
            {
                _playerCards[i] = new CardPair(_deck.DealCard(), _deck.DealCard());
            }
        }

        private bool RunRound(GameState state, bool preFlop, ref IPlayer winner)
        {
            var currentBet = preFlop ? _big : (ushort)0;
            var prevBet = -1;
            var pbets = new ushort[_numPlayers];
            pbets[_smallPosition] = _small;
            pbets[_bigPosition] = _big;
            int foldCount = 0;
            state.CurrentBet = currentBet;
            while (currentBet != prevBet)
            {
                prevBet = currentBet;
                var pi = _bigPosition;
                for (int i = 0; i < _numPlayers; i++)
                {
                    pi = GetNextPosition(pi);
                    if (state.PlayerStates[pi] == PlayerState.Out)
                        continue;

                    var p = _players[pi];
                    var action = p.GetAction(state);
                    state.AllActions.Add(state.LastAction = new PlayerAction(pi, action));
                    switch (action.Type)
                    {
                        case ActionType.Fold:
                            foldCount++;
                            state.PlayerStates[pi] = PlayerState.Out;
                            if (foldCount == _numPlayers - 1)
                            {
                                // winner - everyone else folded
                                winner = _players[GetNextPosition(pi)];
                                return true;
                            }
                            break;
                        case ActionType.Call:
                            p.Balance -= (ushort)(currentBet - pbets[pi]);
                            pbets[pi] = currentBet;
                            break;
                        case ActionType.Raise:
                            p.Balance -= (ushort)((currentBet - pbets[pi]) + action.Amount);
                            state.CurrentBet = currentBet = pbets[pi] = (ushort)(currentBet + action.Amount);
                            break;
                    }
                }
            }
            return false;
        }

        public void Run()
        {
            var state = new GameState()
            {
                Players = _players,
                PlayerStates = new PlayerState[_numPlayers],
                CurrentRound = Round.PreFlop,
                DealerPosition = _dealerPosition,
                Dealer = _players[_dealerPosition]
            };

            IPlayer winner = null;
            // pre flop
            if (!RunRound(state, true, ref winner))
            {
                // deal flop
                state.Flop = new Card[] { _deck.DealCard(), _deck.DealCard(), _deck.DealCard() };
                state.CurrentRound = Round.Flop;

                if (!RunRound(state, false, ref winner))
                {
                    // deal turn
                    state.Turn = _deck.DealCard();
                    state.CurrentRound = Round.Turn;

                    if (!RunRound(state, false, ref winner))
                    {
                        // deal river
                        state.River = _deck.DealCard();
                        state.CurrentRound = Round.River;

                        RunRound(state, false, ref winner);
                    }
                }
            }
            
            if (winner == null)
            {
                // showdown
                Hand best = null;
                List<IPlayer> winners = new List<IPlayer>(12);

                for (int i = 0; i < _numPlayers; i++)
                {
                    if (state.PlayerStates[i] == PlayerState.Out)
                        continue;

                    var hand = Hand.GetBestHand(new[] { _playerCards[i].Card1, 
                                                         _playerCards[i].Card2,
                                                         state.Flop[0],
                                                         state.Flop[1],
                                                         state.Flop[2],
                                                         state.Turn,
                                                         state.River });

                    if (best == null)
                    {
                        best = hand;
                        winners.Add(_players[i]);
                    }
                    else
                    {
                        int c = hand.CompareTo(best);
                        if (c == 0)
                            winners.Add(_players[i]);
                        else
                            if (c == 1)
                            {
                                winners.Clear();
                                winners.Add(_players[i]);
                                best = hand;
                            }
                    }
                }

                // distribute winnings

            }
            else
            {

            }
        }
    }
}
