using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public abstract class Hand : IComparable<Hand>
    {
        internal abstract int HandRank { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapRank(Card[] cards, int i, int j)
        {
            if (cards[i].Rank > cards[j].Rank)
            {
                var t = cards[j];
                cards[j] = cards[i];
                cards[i] = t;
            }
        }

        public static void Sort7Rank(Card[] cards)
        {
            SwapRank(cards, 1, 2);
            SwapRank(cards, 3, 4);
            SwapRank(cards, 5, 6);
            SwapRank(cards, 0, 2);
            SwapRank(cards, 4, 6);
            SwapRank(cards, 3, 5);
            SwapRank(cards, 2, 6);
            SwapRank(cards, 1, 5);
            SwapRank(cards, 0, 4);
            SwapRank(cards, 2, 5);
            SwapRank(cards, 0, 3);
            SwapRank(cards, 2, 4);
            SwapRank(cards, 1, 3);
            SwapRank(cards, 0, 1);
            SwapRank(cards, 2, 3);
            SwapRank(cards, 4, 5);
        }

        public static bool IsFlush(Card[] cards, ref CardRank[] ranks)
        {
            var cc = (cards[0].Suit == CardSuit.Clubs ? 1 : 0) +
                     (cards[1].Suit == CardSuit.Clubs ? 1 : 0) +
                     (cards[2].Suit == CardSuit.Clubs ? 1 : 0) +
                     (cards[3].Suit == CardSuit.Clubs ? 1 : 0) +
                     (cards[4].Suit == CardSuit.Clubs ? 1 : 0) +
                     (cards[5].Suit == CardSuit.Clubs ? 1 : 0) +
                     (cards[6].Suit == CardSuit.Clubs ? 1 : 0);

            var dc = (cards[0].Suit == CardSuit.Diamonds ? 1 : 0) +
                     (cards[1].Suit == CardSuit.Diamonds ? 1 : 0) +
                     (cards[2].Suit == CardSuit.Diamonds ? 1 : 0) +
                     (cards[3].Suit == CardSuit.Diamonds ? 1 : 0) +
                     (cards[4].Suit == CardSuit.Diamonds ? 1 : 0) +
                     (cards[5].Suit == CardSuit.Diamonds ? 1 : 0) +
                     (cards[6].Suit == CardSuit.Diamonds ? 1 : 0);

            var hc = (cards[0].Suit == CardSuit.Hearts ? 1 : 0) +
                     (cards[1].Suit == CardSuit.Hearts ? 1 : 0) +
                     (cards[2].Suit == CardSuit.Hearts ? 1 : 0) +
                     (cards[3].Suit == CardSuit.Hearts ? 1 : 0) +
                     (cards[4].Suit == CardSuit.Hearts ? 1 : 0) +
                     (cards[5].Suit == CardSuit.Hearts ? 1 : 0) +
                     (cards[6].Suit == CardSuit.Hearts ? 1 : 0);

            var sc = (cards[0].Suit == CardSuit.Spades ? 1 : 0) +
                     (cards[1].Suit == CardSuit.Spades ? 1 : 0) +
                     (cards[2].Suit == CardSuit.Spades ? 1 : 0) +
                     (cards[3].Suit == CardSuit.Spades ? 1 : 0) +
                     (cards[4].Suit == CardSuit.Spades ? 1 : 0) +
                     (cards[5].Suit == CardSuit.Spades ? 1 : 0) +
                     (cards[6].Suit == CardSuit.Spades ? 1 : 0);

            CardSuit fsuit = CardSuit.Spades;
            bool isFlush = false;
            if (cc >= 5) { isFlush = true; fsuit = CardSuit.Clubs; }
            if (dc >= 5) { isFlush = true; fsuit = CardSuit.Diamonds; }
            if (hc >= 5) { isFlush = true; fsuit = CardSuit.Hearts; }
            if (sc >= 5) { isFlush = true; fsuit = CardSuit.Spades; }

            if (isFlush)
            {
                ranks = new CardRank[5];
                for (int i = 6, j = 0; j < 5; i--)
                {
                    if (cards[i].Suit == fsuit)
                        ranks[j++] = cards[i].Rank;
                }
            }
            return isFlush;
        }

        public static bool IsStraight(Card[] cards, ref CardRank rank)
        {
            // remove dups
            var h5 = new CardRank[7];
            var prev = h5[6] = cards[6].Rank;
            int j = 5;
            for (int i = 5; i >= 0 && j >= 0; i--)
            {
                if (cards[i].Rank != prev)
                    h5[j--] = prev = cards[i].Rank;
            }
            if (j > 1)
                return false;

            j++;
            if (h5[6] == CardRank.Ace)
            {
                if (h5[5] == CardRank.King && h5[4] == CardRank.Queen && h5[3] == CardRank.Jack && h5[2] == CardRank.Ten)
                {
                    rank = CardRank.Ace;
                    return true;
                }
                if (h5[j] == CardRank.Two && h5[j + 1] == CardRank.Three && h5[j + 2] == CardRank.Four && h5[j + 3] == CardRank.Five)
                {
                    rank = CardRank.Five;
                    return true;
                }
            }
            if ((int)h5[6] - (int)h5[5] == 1 &&
                (int)h5[5] - (int)h5[4] == 1 &&
                (int)h5[4] - (int)h5[3] == 1 &&
                (int)h5[3] - (int)h5[2] == 1)
            {
                rank = h5[6];
                return true;
            }
            if (j <= 1 &&
                (int)h5[5] - (int)h5[4] == 1 &&
                (int)h5[4] - (int)h5[3] == 1 &&
                (int)h5[3] - (int)h5[2] == 1 &&
                (int)h5[2] - (int)h5[1] == 1)
            {
                rank = h5[5];
                return true;
            }
            if (j == 0 &&
                (int)h5[4] - (int)h5[3] == 1 &&
                (int)h5[3] - (int)h5[2] == 1 &&
                (int)h5[2] - (int)h5[1] == 1 &&
                (int)h5[1] - (int)h5[0] == 1)
            {
                rank = h5[4];
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFourOfaKind(Card[] cards, ref CardRank rank, ref CardRank kicker)
        {
            if (cards[6].Rank == cards[5].Rank &&
                cards[6].Rank == cards[4].Rank &&
                cards[6].Rank == cards[3].Rank)
            {
                rank = cards[6].Rank;
                kicker = cards[1].Rank;
                return true;
            }
            if (cards[5].Rank == cards[4].Rank &&
                cards[5].Rank == cards[3].Rank &&
                cards[5].Rank == cards[2].Rank) 
            {
                rank = cards[5].Rank;
                kicker = cards[6].Rank;
                return true;
            }
            if (cards[4].Rank == cards[3].Rank &&
                cards[4].Rank == cards[2].Rank &&
                cards[4].Rank == cards[1].Rank)
            {
                rank = cards[4].Rank;
                kicker = cards[6].Rank;
                return true;
            }
            if (cards[3].Rank == cards[2].Rank &&
                cards[3].Rank == cards[1].Rank &&
                cards[3].Rank == cards[0].Rank)
            {
                rank = cards[3].Rank;
                kicker = cards[6].Rank;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountPairs(Card[] cards, ref CardRank pairRank1, ref CardRank pairRank2)
        {
            if (cards[6].Rank == cards[5].Rank)
            {
                pairRank1 = cards[6].Rank;
                if (ContainsPair(cards[0], cards[1], cards[2], cards[3], cards[4], ref pairRank2))
                    return 2;
                return 1;
            }
            if (cards[5].Rank == cards[4].Rank)
            {
                pairRank1 = cards[5].Rank;
                if (ContainsPair(cards[0], cards[1], cards[2], cards[3], ref pairRank2))
                    return 2;
                return 1;
            }
            if (cards[4].Rank == cards[3].Rank)
            {
                pairRank1 = cards[4].Rank;
                if (ContainsPair(cards[0], cards[1], cards[2], ref pairRank2))
                    return 2;
                return 1;
            }
            if (cards[3].Rank == cards[2].Rank)
            {
                pairRank1 = cards[3].Rank;
                if (ContainsPair(cards[0], cards[1], ref pairRank2))
                    return 2;
                return 1;
            }
            if (cards[2].Rank == cards[1].Rank)
            {
                pairRank1 = cards[2].Rank;
                return 1;
            }
            if (cards[1].Rank == cards[0].Rank)
            {
                pairRank1 = cards[1].Rank;
                return 1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ContainsPair(Card c1, Card c2, Card c3, Card c4, Card c5, ref CardRank pairRank)
        {
            if (c5.Rank == c4.Rank)
            {
                pairRank = c5.Rank;
                return true;
            }
            if (c4.Rank == c3.Rank)
            {
                pairRank = c4.Rank;
                return true;
            }
            if (c3.Rank == c2.Rank)
            {
                pairRank = c3.Rank;
                return true;
            }
            if (c2.Rank == c1.Rank)
            {
                pairRank = c2.Rank;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ContainsPair(Card c1, Card c2, Card c3, Card c4, ref CardRank pairRank)
        {
            if (c4.Rank == c3.Rank)
            {
                pairRank = c4.Rank;
                return true;
            }
            if (c3.Rank == c2.Rank)
            {
                pairRank = c3.Rank;
                return true;
            }
            if (c2.Rank == c1.Rank)
            {
                pairRank = c2.Rank;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ContainsPair(Card c1, Card c2, Card c3, ref CardRank pairRank)
        {
            if (c3.Rank == c2.Rank)
            {
                pairRank = c3.Rank;
                return true;
            }
            if (c2.Rank == c1.Rank)
            {
                pairRank = c2.Rank;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ContainsPair(Card c1, Card c2, ref CardRank pairRank)
        {
            if (c2.Rank == c1.Rank)
            {
                pairRank = c2.Rank;
                return true;
            }
            return false;
        }

        public static Hand GetBestHand(Card[] cards)
        {
            // sort
            Sort7Rank(cards);

            // test flush
            CardRank[] franks = null;
            if (IsFlush(cards, ref franks))
            {
                // straight flush
                if ((int)franks[0] - (int)franks[1] == 1 &&
                    (int)franks[1] - (int)franks[2] == 1 &&
                    (int)franks[2] - (int)franks[3] == 1 &&
                    (int)franks[3] - (int)franks[4] == 1)
                {
                    return new StraightFlush(franks[0]);
                }
                return new Flush(franks);
            }
            // test four of a kind
            CardRank fr = default(CardRank), fk = default(CardRank);
            if (IsFourOfaKind(cards, ref fr, ref fk))
            {
                return new FourOfaKind(fr, fk);
            }

            // test full house
            bool containsThree = false;
            CardRank threeRank = default(CardRank), trank1 = default(CardRank), trank2 = default(CardRank);
            if (containsThree = (cards[6].Rank == cards[5].Rank &&
                                 cards[6].Rank == cards[4].Rank))
            {
                threeRank = cards[6].Rank;
                CardRank fhpr = default(CardRank);
                if (ContainsPair(cards[0], cards[1], cards[2], cards[3], ref fhpr))
                    return new FullHouse(threeRank, fhpr);
                trank1 = cards[3].Rank;
                trank2 = cards[2].Rank;
            }
            else if (containsThree = (cards[5].Rank == cards[4].Rank &&
                                      cards[5].Rank == cards[3].Rank))
            {
                threeRank = cards[5].Rank;
                CardRank fhpr = default(CardRank);
                if (ContainsPair(cards[0], cards[1], cards[2], ref fhpr))
                    return new FullHouse(threeRank, fhpr);
                trank1 = cards[6].Rank;
                trank2 = cards[2].Rank;
            }
            else if (containsThree = (cards[4].Rank == cards[3].Rank &&
                                      cards[4].Rank == cards[2].Rank))
            {
                threeRank = cards[4].Rank;
                CardRank fhpr = default(CardRank);
                if (ContainsPair(cards[5], cards[6], ref fhpr))
                    return new FullHouse(threeRank, fhpr);
                if (ContainsPair(cards[0], cards[1], ref fhpr))
                    return new FullHouse(threeRank, fhpr);
                trank1 = cards[6].Rank;
                trank2 = cards[5].Rank;
            }
            else if (containsThree = (cards[3].Rank == cards[2].Rank &&
                                      cards[3].Rank == cards[1].Rank))
            {
                threeRank = cards[3].Rank;
                CardRank fhpr = default(CardRank);
                if (ContainsPair(cards[4], cards[5], cards[6], ref fhpr))
                    return new FullHouse(threeRank, fhpr);
                trank1 = cards[6].Rank;
                trank2 = cards[5].Rank;
            }
            else if (containsThree = (cards[2].Rank == cards[1].Rank &&
                                      cards[2].Rank == cards[0].Rank))
            {
                threeRank = cards[2].Rank;
                CardRank fhpr = default(CardRank);
                if (ContainsPair(cards[3], cards[4], cards[5], cards[6], ref fhpr))
                    return new FullHouse(threeRank, fhpr);
                trank1 = cards[6].Rank;
                trank2 = cards[5].Rank;
            }
            
            // test straight
            CardRank srank = default(CardRank);
            if (IsStraight(cards, ref srank))
                return new Straight(srank);

            // three of a kind
            if (containsThree)
            {
                return new ThreeOfaKind(threeRank, trank1, trank2);
            }

            // pair
            CardRank prank1 = default(CardRank), prank2 = default(CardRank);
            var p = CountPairs(cards, ref prank1, ref prank2);
            if (p == 2)
            {
                if (cards[6].Rank != prank1 && cards[6].Rank != prank2)
                    return new TwoPair(prank1, prank2, cards[6].Rank);
                if (cards[5].Rank != prank1 && cards[5].Rank != prank2)
                    return new TwoPair(prank1, prank2, cards[5].Rank);
                if (cards[4].Rank != prank1 && cards[4].Rank != prank2)
                    return new TwoPair(prank1, prank2, cards[4].Rank);
                if (cards[3].Rank != prank1 && cards[3].Rank != prank2)
                    return new TwoPair(prank1, prank2, cards[3].Rank);
                if (cards[2].Rank != prank1 && cards[2].Rank != prank2)
                    return new TwoPair(prank1, prank2, cards[2].Rank);
                if (cards[1].Rank != prank1 && cards[1].Rank != prank2)
                    return new TwoPair(prank1, prank2, cards[1].Rank);
                if (cards[0].Rank != prank1 && cards[0].Rank != prank2)
                    return new TwoPair(prank1, prank2, cards[0].Rank);
            }
            else if (p == 1)
            {
                if (cards[6].Rank == prank1)
                    return new Pair(prank1, cards[4].Rank, cards[3].Rank, cards[2].Rank);
                else if (cards[5].Rank == prank1)
                    return new Pair(prank1, cards[6].Rank, cards[3].Rank, cards[2].Rank);
                else if (cards[4].Rank == prank1)
                    return new Pair(prank1, cards[6].Rank, cards[5].Rank, cards[2].Rank);
                else
                    return new Pair(prank1, cards[6].Rank, cards[5].Rank, cards[4].Rank);
            }
            return new HighestCard(cards[6].Rank, cards[5].Rank, cards[4].Rank, cards[3].Rank, cards[2].Rank);
        }

        public virtual int CompareTo(Hand other)
        {
            return HandRank > other.HandRank ? 1 : -1;
        }
    }

    public class HighestCard : Hand, IComparable<HighestCard>
    {
        public HighestCard(params CardRank[] ranks)
        {
            Ranks = ranks;
        }

        public CardRank[] Ranks { get; private set; }

        public int CompareTo(HighestCard other)
        {
            if (other.Ranks[0] == Ranks[0]) 
            {
                if (other.Ranks[1] == Ranks[1])
                {
                    if (other.Ranks[2] == Ranks[2])
                    {
                        if (other.Ranks[3] == Ranks[3])
                        {
                            if (other.Ranks[4] == Ranks[4])
                                return 0;
                        }
                        return Ranks[3] > other.Ranks[3] ? 1 : -1;
                    }
                    return Ranks[2] > other.Ranks[2] ? 1 : -1;
                }
                return Ranks[1] > other.Ranks[1] ? 1 : -1;
            }
            return Ranks[0] > other.Ranks[0] ? 1 : -1;
        }

        public override int CompareTo(Hand other)
        {
            if (other is HighestCard)
                return CompareTo((HighestCard)other);
            return base.CompareTo(other);
        }

        internal override int HandRank
        {
            get { return 0; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HighestCard) || obj == null)
                return false;

            var h = (HighestCard)obj;

            return Ranks[0] == h.Ranks[0] &&
                   Ranks[1] == h.Ranks[1] &&
                   Ranks[2] == h.Ranks[2] &&
                   Ranks[3] == h.Ranks[3] &&
                   Ranks[4] == h.Ranks[4];
        }

        public override int GetHashCode()
        {
            return HandRank ^ (int)Ranks[0] ^ (int)Ranks[1] ^ (int)Ranks[2] ^ (int)Ranks[3] ^ (int)Ranks[4];
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}", Ranks[0].ToDisplay(), Ranks[1].ToDisplay(), Ranks[2].ToDisplay(), Ranks[3].ToDisplay(), Ranks[4].ToDisplay());
        }
    }

    public class Pair : Hand, IComparable<Pair>
    {
        public Pair(CardRank pairRank, CardRank rank1, CardRank rank2, CardRank rank3)
        {
            PairRank = pairRank;
            Rank1 = rank1;
            Rank2 = rank2;
            Rank3 = rank3;
        }

        public CardRank PairRank { get; private set; }

        public CardRank Rank1 { get; private set; }

        public CardRank Rank2 { get; private set; }

        public CardRank Rank3 { get; private set; }

        public int CompareTo(Pair other)
        {
            if (PairRank == other.PairRank)
            {
                if (Rank1 == other.Rank1)
                {
                    if (Rank2 == other.Rank2)
                    {
                        if (Rank3 == other.Rank3)
                        {
                            return 0;
                        }
                        return Rank3 > other.Rank3 ? 1 : -1;
                    }
                    return Rank2 > other.Rank2 ? 1 : -1;
                }
                return Rank1 > other.Rank1 ? 1 : -1;
            }
            return PairRank > other.PairRank ? 1 : -1;
        }

        public override int CompareTo(Hand other)
        {
            if (other is Pair)
                return CompareTo((Pair)other);
            return base.CompareTo(other);
        }

        internal override int HandRank
        {
            get { return 1; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Pair) || obj == null)
                return false;

            var h = (Pair)obj;

            return PairRank == h.PairRank &&
                   Rank1 == h.Rank1 &&
                   Rank2 == h.Rank2 &&
                   Rank3 == h.Rank3;
        }

        public override int GetHashCode()
        {
            return HandRank ^ (int)PairRank ^ (int)Rank1 ^ (int)Rank2 ^ (int)Rank3;
        }

        public override string ToString()
        {
            return string.Format("{0}{0} {1} {2} {3}", PairRank.ToDisplay(), Rank1.ToDisplay(), Rank2.ToDisplay(), Rank3.ToDisplay());
        }
    }

    public class TwoPair : Hand, IComparable<TwoPair>
    {
        public TwoPair(CardRank pairRank1, CardRank pairRank2, CardRank kicker)
        {
            PairRank1 = pairRank1;
            PairRank2 = pairRank2;
            Kicker = kicker;
        }

        public CardRank PairRank1 { get; private set; }

        public CardRank PairRank2 { get; private set; }

        public CardRank Kicker { get; private set; }

        internal override int HandRank
        {
            get { return 2; }
        }

        public override int CompareTo(Hand other)
        {
            if (other is TwoPair)
                return CompareTo((TwoPair)other);
            return base.CompareTo(other);
        }

        public int CompareTo(TwoPair other)
        {
            if (PairRank1 == other.PairRank1)
            {
                if (PairRank2 == other.PairRank2)
                {
                    if (Kicker == other.Kicker)
                        return 0;
                    return Kicker > other.Kicker ? 1 : -1;
                }
                return PairRank2 > other.PairRank2 ? 1 : -1; 
            }
            return PairRank1 > other.PairRank1 ? 1 : -1;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TwoPair) || obj == null)
                return false;

            var h = (TwoPair)obj;

            return PairRank1 == h.PairRank1 &&
                   PairRank2 == h.PairRank2 &&
                   Kicker == h.Kicker;
        }

        public override int GetHashCode()
        {
            return HandRank ^ (int)PairRank1 ^ (int)PairRank2 ^ (int)Kicker;
        }

        public override string ToString()
        {
            return string.Format("{0}{0} {1}{1} {2}", PairRank1.ToDisplay(), PairRank2.ToDisplay(), Kicker.ToDisplay());
        }
    }

    public class ThreeOfaKind : Hand, IComparable<ThreeOfaKind>
    {
        public ThreeOfaKind(CardRank threeRank, CardRank rank1, CardRank rank2)
        {
            ThreeRank = threeRank;
            Rank1 = rank1;
            Rank2 = rank2;
        }

        public CardRank ThreeRank { get; private set; }

        public CardRank Rank1 { get; private set; }

        public CardRank Rank2 { get; private set; }

        internal override int HandRank
        {
            get { return 3; }
        }

        public override int CompareTo(Hand other)
        {
            if (other is ThreeOfaKind)
                return CompareTo((ThreeOfaKind)other);
            return base.CompareTo(other);
        }

        public int CompareTo(ThreeOfaKind other)
        {
            if (ThreeRank == other.ThreeRank)
            {
                if (Rank1 == other.Rank1)
                {
                    if (Rank2 == other.Rank2)
                        return 0;
                    return Rank2 > other.Rank2 ? 1 : -1;
                }
                return Rank1 > other.Rank1 ? 1 : -1;
            }
            return ThreeRank > other.ThreeRank ? 1 : -1;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ThreeOfaKind) || obj == null)
                return false;

            var h = (ThreeOfaKind)obj;

            return ThreeRank == h.ThreeRank &&
                   Rank1 == h.Rank1 &&
                   Rank2 == h.Rank2;
        }

        public override int GetHashCode()
        {
            return HandRank ^ (int)ThreeRank ^ (int)Rank1 ^ (int)Rank2;
        }

        public override string ToString()
        {
            return string.Format("{0}{0}{0} {1} {2}", ThreeRank.ToDisplay(), Rank1.ToDisplay(), Rank2.ToDisplay());
        }
    }

    public class Straight : Hand, IComparable<Straight>
    {
        public Straight(CardRank rank)
        {
            Rank = rank;
        }

        public CardRank Rank { get; private set; }

        internal override int HandRank
        {
            get { return 4; }
        }

        public override int CompareTo(Hand other)
        {
            if (other is Straight)
                return CompareTo((Straight)other);
            return base.CompareTo(other);
        }

        public int CompareTo(Straight other)
        {
            if (Rank == other.Rank)
                return 0;
            return Rank > other.Rank ? 1 : -1;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Straight) || obj == null)
                return false;

            var h = (Straight)obj;

            return Rank == h.Rank;
        }

        public override int GetHashCode()
        {
            return HandRank ^ (int)Rank;
        }

        public override string ToString()
        {
            return string.Format("S {0}", Rank.ToDisplay());
        }
    }

    public class Flush : Hand, IComparable<Flush>
    {
        public Flush(CardRank[] ranks)
        {
            Ranks = ranks;
        }

        public CardRank[] Ranks { get; private set; }

        internal override int HandRank
        {
            get { return 5; }
        }

        public override int CompareTo(Hand other)
        {
            if (other is Flush)
                return CompareTo((Flush)other);
            return base.CompareTo(other);
        }

        public int CompareTo(Flush other)
        {
            if (other.Ranks[0] == Ranks[0])
            {
                if (other.Ranks[1] == Ranks[1])
                {
                    if (other.Ranks[2] == Ranks[2])
                    {
                        if (other.Ranks[3] == Ranks[3])
                        {
                            if (other.Ranks[4] == Ranks[4])
                                return 0;
                        }
                        return Ranks[3] > other.Ranks[3] ? 1 : -1;
                    }
                    return Ranks[2] > other.Ranks[2] ? 1 : -1;
                }
                return Ranks[1] > other.Ranks[1] ? 1 : -1;
            }
            return Ranks[0] > other.Ranks[0] ? 1 : -1;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Flush) || obj == null)
                return false;

            var h = (Flush)obj;

            return Ranks[0] == h.Ranks[0] &&
                   Ranks[1] == h.Ranks[1] &&
                   Ranks[2] == h.Ranks[2] &&
                   Ranks[3] == h.Ranks[3] &&
                   Ranks[4] == h.Ranks[4];
        }

        public override int GetHashCode()
        {
            return HandRank ^ (int)Ranks[0] ^ (int)Ranks[1] ^ (int)Ranks[2] ^ (int)Ranks[3] ^ (int)Ranks[4];
        }

        public override string ToString()
        {
            return string.Format("F {0} {1} {2} {3} {4}", Ranks[0].ToDisplay(), Ranks[1].ToDisplay(), Ranks[2].ToDisplay(), Ranks[3].ToDisplay(), Ranks[4].ToDisplay());
        }
    }

    public class FullHouse : Hand, IComparable<FullHouse>
    {
        public FullHouse(CardRank threeRank, CardRank pairRank)
        {
            ThreeRank = threeRank;
            PairRank = pairRank;
        }

        public CardRank ThreeRank { get; private set; }

        public CardRank PairRank { get; private set; }

        internal override int HandRank
        {
            get { return 6; }
        }

        public override int CompareTo(Hand other)
        {
            if (other is FullHouse)
                return CompareTo((FullHouse)other);
            return base.CompareTo(other);
        }

        public int CompareTo(FullHouse other)
        {
            if (ThreeRank == other.ThreeRank)
            {
                if (PairRank == other.PairRank)
                    return 0;
                return PairRank > other.PairRank ? 1 : -1;
            }
            return ThreeRank > other.ThreeRank ? 1 : -1;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FullHouse) || obj == null)
                return false;

            var h = (FullHouse)obj;

            return ThreeRank == h.ThreeRank &&
                   PairRank == h.PairRank;
        }

        public override int GetHashCode()
        {
            return HandRank ^ (int)ThreeRank ^ (int)PairRank;
        }

        public override string ToString()
        {
            return string.Format("{0}{0}{0} {1}{1}", ThreeRank.ToDisplay(), PairRank.ToDisplay());
        }
    }

    public class FourOfaKind : Hand, IComparable<FourOfaKind>
    {
        public FourOfaKind(CardRank rank, CardRank kicker)
        {
            Rank = rank;
            Kicker = kicker;
        }

        public CardRank Rank { get; private set; }

        public CardRank Kicker { get; private set; }

        internal override int HandRank
        {
            get { return 7; }
        }

        public override int CompareTo(Hand other)
        {
            if (other is FourOfaKind)
                return CompareTo((FourOfaKind)other);
            return base.CompareTo(other);
        }

        public int CompareTo(FourOfaKind other)
        {
            if (Rank == other.Rank)
            {
                if (Kicker == other.Kicker)
                    return 0;
                return Kicker > other.Kicker ? 1 : -1;
            }
            return Rank > other.Rank ? 1 : -1;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FourOfaKind) || obj == null)
                return false;

            var h = (FourOfaKind)obj;

            return Rank == h.Rank &&
                   Kicker == h.Kicker;
        }

        public override int GetHashCode()
        {
            return HandRank ^ (int)Rank ^ (int)Kicker;
        }

        public override string ToString()
        {
            return string.Format("{0}{0}{0}{0} {1}", Rank.ToDisplay(), Kicker.ToDisplay());
        }
    }

    public class StraightFlush : Hand, IComparable<StraightFlush>
    {
        public StraightFlush(CardRank rank)
        {
            Rank = rank;
        }

        public CardRank Rank { get; private set; }

        internal override int HandRank
        {
            get { return 8; }
        }

        public override int CompareTo(Hand other)
        {
            if (other is StraightFlush)
                return CompareTo((StraightFlush)other);
            return base.CompareTo(other);
        }

        public int CompareTo(StraightFlush other)
        {
            if (Rank == other.Rank)
                return 0;
            return Rank > other.Rank ? 1 : -1;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StraightFlush) || obj == null)
                return false;

            var h = (StraightFlush)obj;

            return Rank == h.Rank;
        }

        public override int GetHashCode()
        {
            return HandRank ^ (int)Rank;
        }

        public override string ToString()
        {
            return string.Format("SF {0}", Rank.ToDisplay());
        }
    }
}
