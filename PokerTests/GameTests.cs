using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokerCore;
using System.Linq;

namespace PokerTests
{
    [TestClass]
    public class GameTests
    {
        private static readonly Card[] _HighestCard1 = new[] { Card.AceDiamonds, Card.TenDiamonds, Card.TwoDiamonds, Card.SevenSpades, Card.SixClubs, Card.FiveClubs, Card.ThreeHearts };
        private static readonly Card[] _HighestCard2 = new[] { Card.AceDiamonds, Card.TenDiamonds, Card.TwoDiamonds, Card.SevenSpades, Card.EightClubs, Card.FiveClubs, Card.ThreeHearts };
        private static readonly Card[] _Pair1 = new[] { Card.AceDiamonds, Card.TenClubs, Card.TwoDiamonds, Card.SevenClubs, Card.TenHearts, Card.ThreeClubs, Card.JackDiamonds };
        private static readonly Card[] _Pair2 = new[] { Card.AceDiamonds, Card.QueenClubs, Card.TwoDiamonds, Card.SevenClubs, Card.QueenHearts, Card.ThreeClubs, Card.JackDiamonds };
        private static readonly Card[] _Pair3 = new[] { Card.AceDiamonds, Card.QueenClubs, Card.TwoDiamonds, Card.FiveClubs, Card.QueenHearts, Card.ThreeClubs, Card.JackDiamonds };
        private static readonly Card[] _Pair4 = new[] { Card.AceDiamonds, Card.QueenClubs, Card.TwoDiamonds, Card.EightClubs, Card.QueenHearts, Card.ThreeClubs, Card.JackDiamonds };
        private static readonly Card[] _TwoPair1 = new[] { Card.KingClubs, Card.JackClubs, Card.KingDiamonds, Card.AceSpades, Card.TwoHearts, Card.JackDiamonds, Card.ThreeSpades };
        private static readonly Card[] _TwoPair2 = new[] { Card.KingClubs, Card.JackClubs, Card.KingDiamonds, Card.AceSpades, Card.TwoHearts, Card.JackDiamonds, Card.TwoClubs };
        private static readonly Card[] _Three1 = new[] { Card.KingClubs, Card.JackClubs, Card.JackSpades, Card.AceSpades, Card.TwoHearts, Card.JackDiamonds, Card.ThreeSpades };
        private static readonly Card[] _Three2 = new[] { Card.KingClubs, Card.ThreeClubs, Card.ThreeDiamonds, Card.AceSpades, Card.TwoHearts, Card.JackDiamonds, Card.ThreeSpades };
        private static readonly Card[] _Straight1 = new[] { Card.FiveDiamonds, Card.SevenClubs, Card.SixHearts, Card.EightClubs, Card.AceSpades, Card.NineClubs, Card.TenSpades };
        private static readonly Card[] _Straight2 = new[] { Card.AceDiamonds, Card.SevenClubs, Card.SixHearts, Card.EightClubs, Card.AceSpades, Card.NineClubs, Card.TenSpades };
        private static readonly Card[] _Straight3 = new[] { Card.AceDiamonds, Card.TwoClubs, Card.FourClubs, Card.ThreeDiamonds, Card.FiveHearts, Card.FiveSpades, Card.TenSpades };
        private static readonly Card[] _Straight4 = new[] { Card.AceDiamonds, Card.QueenClubs, Card.KingClubs, Card.JackClubs, Card.ThreeDiamonds, Card.FourClubs, Card.TenSpades };
        private static readonly Card[] _Flush1 = new[] { Card.AceDiamonds, Card.QueenDiamonds, Card.KingClubs, Card.JackDiamonds, Card.ThreeDiamonds, Card.FourDiamonds, Card.TenSpades };
        private static readonly Card[] _Flush2 = new[] { Card.AceDiamonds, Card.KingDiamonds, Card.QueenDiamonds, Card.ThreeDiamonds, Card.FourDiamonds, Card.KingClubs, Card.TenSpades };
        private static readonly Card[] _FullHouse1 = new[] { Card.TenSpades, Card.TenHearts, Card.AceClubs, Card.ThreeHearts, Card.JackClubs, Card.TenDiamonds, Card.ThreeDiamonds };
        private static readonly Card[] _FullHouse2 = new[] { Card.TenSpades, Card.TenHearts, Card.AceClubs, Card.ThreeHearts, Card.JackClubs, Card.ThreeClubs, Card.ThreeDiamonds };
        private static readonly Card[] _Four1 = new[] { Card.EightClubs, Card.AceClubs, Card.EightDiamonds, Card.EightHearts, Card.TwoClubs, Card.EightSpades, Card.QueenHearts };
        private static readonly Card[] _Four2 = new[] { Card.EightClubs, Card.KingClubs, Card.EightDiamonds, Card.EightHearts, Card.TwoClubs, Card.EightSpades, Card.QueenHearts };
        private static readonly Card[] _StraightFlush1 = new[] { Card.FiveClubs, Card.SevenClubs, Card.SixClubs, Card.EightClubs, Card.AceSpades, Card.NineClubs, Card.TenSpades };
        private static readonly Card[] _StraightFlush2 = new[] { Card.AceDiamonds, Card.QueenDiamonds, Card.KingDiamonds, Card.JackDiamonds, Card.ThreeDiamonds, Card.FourClubs, Card.TenDiamonds };

        [TestMethod]
        public void Hands()
        {
            var best = Hand.GetBestHand(_HighestCard1);
            Assert.IsInstanceOfType(best, typeof(HighestCard));
            Assert.IsTrue(new [] { CardRank.Ace, CardRank.Ten, CardRank.Seven, CardRank.Six, CardRank.Five }.SequenceEqual(((HighestCard)best).Ranks));

            best = Hand.GetBestHand(_Pair1);
            Assert.IsInstanceOfType(best, typeof(Pair));
            Assert.AreEqual(CardRank.Ten, ((Pair)best).PairRank);
            Assert.AreEqual(CardRank.Ace, ((Pair)best).Rank1);
            Assert.AreEqual(CardRank.Jack, ((Pair)best).Rank2);
            Assert.AreEqual(CardRank.Seven, ((Pair)best).Rank3);

            best = Hand.GetBestHand(_TwoPair1);
            Assert.IsInstanceOfType(best, typeof(TwoPair));
            Assert.AreEqual(CardRank.King, ((TwoPair)best).PairRank1);
            Assert.AreEqual(CardRank.Jack, ((TwoPair)best).PairRank2);
            Assert.AreEqual(CardRank.Ace, ((TwoPair)best).Kicker);

            best = Hand.GetBestHand(_Three1);
            Assert.IsInstanceOfType(best, typeof(ThreeOfaKind));
            Assert.AreEqual(CardRank.Jack, ((ThreeOfaKind)best).ThreeRank);
            Assert.AreEqual(CardRank.Ace, ((ThreeOfaKind)best).Rank1);
            Assert.AreEqual(CardRank.King, ((ThreeOfaKind)best).Rank2);

            best = Hand.GetBestHand(_Three2);
            Assert.IsInstanceOfType(best, typeof(ThreeOfaKind));
            Assert.AreEqual(CardRank.Three, ((ThreeOfaKind)best).ThreeRank);
            Assert.AreEqual(CardRank.Ace, ((ThreeOfaKind)best).Rank1);
            Assert.AreEqual(CardRank.King, ((ThreeOfaKind)best).Rank2);

            best = Hand.GetBestHand(_Straight1);
            Assert.IsInstanceOfType(best, typeof(Straight));
            Assert.AreEqual(CardRank.Ten, ((Straight)best).Rank);

            best = Hand.GetBestHand(_Straight2);
            Assert.IsInstanceOfType(best, typeof(Straight));
            Assert.AreEqual(CardRank.Ten, ((Straight)best).Rank);

            best = Hand.GetBestHand(_Straight3);
            Assert.IsInstanceOfType(best, typeof(Straight));
            Assert.AreEqual(CardRank.Five, ((Straight)best).Rank);

            best = Hand.GetBestHand(_Straight4);
            Assert.IsInstanceOfType(best, typeof(Straight));
            Assert.AreEqual(CardRank.Ace, ((Straight)best).Rank);

            best = Hand.GetBestHand(_Flush1);
            Assert.IsInstanceOfType(best, typeof(Flush));
            Assert.IsTrue(new[] { CardRank.Ace, CardRank.Queen, CardRank.Jack, CardRank.Four, CardRank.Three }.SequenceEqual(((Flush)best).Ranks));

            best = Hand.GetBestHand(_Flush2);
            Assert.IsInstanceOfType(best, typeof(Flush));
            Assert.IsTrue(new[] { CardRank.Ace, CardRank.King, CardRank.Queen, CardRank.Four, CardRank.Three }.SequenceEqual(((Flush)best).Ranks));

            best = Hand.GetBestHand(_FullHouse1);
            Assert.IsInstanceOfType(best, typeof(FullHouse));
            Assert.AreEqual(CardRank.Ten, ((FullHouse)best).ThreeRank);
            Assert.AreEqual(CardRank.Three, ((FullHouse)best).PairRank);

            best = Hand.GetBestHand(_FullHouse2);
            Assert.IsInstanceOfType(best, typeof(FullHouse));
            Assert.AreEqual(CardRank.Three, ((FullHouse)best).ThreeRank);
            Assert.AreEqual(CardRank.Ten, ((FullHouse)best).PairRank);

            best = Hand.GetBestHand(_Four1);
            Assert.IsInstanceOfType(best, typeof(FourOfaKind));
            Assert.AreEqual(CardRank.Eight, ((FourOfaKind)best).Rank);
            Assert.AreEqual(CardRank.Ace, ((FourOfaKind)best).Kicker);

            best = Hand.GetBestHand(_Four2);
            Assert.IsInstanceOfType(best, typeof(FourOfaKind));
            Assert.AreEqual(CardRank.Eight, ((FourOfaKind)best).Rank);
            Assert.AreEqual(CardRank.King, ((FourOfaKind)best).Kicker);

            best = Hand.GetBestHand(_StraightFlush1);
            Assert.IsInstanceOfType(best, typeof(StraightFlush));
            Assert.AreEqual(CardRank.Nine, ((StraightFlush)best).Rank);

            best = Hand.GetBestHand(_StraightFlush2);
            Assert.IsInstanceOfType(best, typeof(StraightFlush));
            Assert.AreEqual(CardRank.Ace, ((StraightFlush)best).Rank);

            var hands = new[] 
            {
                _Flush2,
                _Flush1,
                _Four1,
                _Four2,
                _FullHouse1,
                _FullHouse2,
                _HighestCard2,
                _HighestCard1,
                _Pair4,
                _Pair3,
                _Pair1,
                _Pair2,
                _Straight4,
                _Straight2,
                _Straight1,
                _Straight3,
                _Three2,
                _StraightFlush2,
                _Three1,
                _StraightFlush1,
                _TwoPair2,
                _TwoPair1
            }.Select(h => Hand.GetBestHand(h)).ToArray();

            var handse = new[]
            {
                _HighestCard1,
                _HighestCard2,
                _Pair1,
                _Pair3,
                _Pair2,
                _Pair4,
                _TwoPair1,
                _TwoPair2,
                _Three2,
                _Three1,
                _Straight3,
                _Straight1,
                _Straight2,
                _Straight4,
                _Flush1,
                _Flush2,
                _FullHouse2,
                _FullHouse1,
                _Four2,
                _Four1,
                _StraightFlush1,
                _StraightFlush2
            }.Select(h => Hand.GetBestHand(h)).ToArray();

            Array.Sort(hands);

            Assert.IsTrue(hands.SequenceEqual(handse));
        }
    }
}
