#define OPENCL

using Cudafy;
using Cudafy.Host;
using Cudafy.IntegerIntrinsics;
using Cudafy.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore.Gpu
{
    public static partial class Hand
    {
        private const int Hearts = (int)CardSuit.Hearts;
        private const int Diamonds = (int)CardSuit.Diamonds;
        private const int Clubs = (int)CardSuit.Clubs;
        private const int Spades = (int)CardSuit.Spades;

        private const int Rank2 = (int)CardRank.Two;
        private const int Rank3 = (int)CardRank.Three;
        private const int Rank4 = (int)CardRank.Four;
        private const int Rank5 = (int)CardRank.Five;
        private const int Rank6 = (int)CardRank.Six;
        private const int Rank7 = (int)CardRank.Seven;
        private const int Rank8 = (int)CardRank.Eight;
        private const int Rank9 = (int)CardRank.Nine;
        private const int RankTen = (int)CardRank.Ten;
        private const int RankJack = (int)CardRank.Jack;
        private const int RankQueen = (int)CardRank.Queen;
        private const int RankKing = (int)CardRank.King;
        private const int RankAce = (int)CardRank.Ace;

        private const int CardJoker = 52;
        private const int NumberOfCards = 52;
        private const int NCardsWJoker = 53;
        private const int HANDTYPE_SHIFT = 24;
        private const int TOP_CARD_SHIFT = 16;
        private const System.UInt32 TOP_CARD_MASK = 0x000F0000;
        private const int SECOND_CARD_SHIFT = 12;
        private const System.UInt32 SECOND_CARD_MASK = 0x0000F000;
        private const int THIRD_CARD_SHIFT = 8;
        private const int FOURTH_CARD_SHIFT = 4;
        private const int FIFTH_CARD_SHIFT = 0;
        private const System.UInt32 FIFTH_CARD_MASK = 0x0000000F;
        private const int CARD_WIDTH = 4;
        private const System.UInt32 CARD_MASK = 0x0F;

        private const uint HANDTYPE_VALUE_STRAIGHTFLUSH = (((uint)HandType.StraightFlush) << HANDTYPE_SHIFT);
        private const uint HANDTYPE_VALUE_STRAIGHT = (((uint)HandType.Straight) << HANDTYPE_SHIFT);
        private const uint HANDTYPE_VALUE_FLUSH = (((uint)HandType.Flush) << HANDTYPE_SHIFT);
        private const uint HANDTYPE_VALUE_FULLHOUSE = (((uint)HandType.FullHouse) << HANDTYPE_SHIFT);
        private const uint HANDTYPE_VALUE_FOUR_OF_A_KIND = (((uint)HandType.FourOfAKind) << HANDTYPE_SHIFT);
        private const uint HANDTYPE_VALUE_TRIPS = (((uint)HandType.Trips) << HANDTYPE_SHIFT);
        private const uint HANDTYPE_VALUE_TWOPAIR = (((uint)HandType.TwoPair) << HANDTYPE_SHIFT);
        private const uint HANDTYPE_VALUE_PAIR = (((uint)HandType.Pair) << HANDTYPE_SHIFT);
        private const uint HANDTYPE_VALUE_HIGHCARD = (((uint)HandType.HighCard) << HANDTYPE_SHIFT);
        private const int SPADE_OFFSET = 13 * Spades;
        private const int CLUB_OFFSET = 13 * Clubs;
        private const int DIAMOND_OFFSET = 13 * Diamonds;
        private const int HEART_OFFSET = 13 * Hearts;

        private const uint SA2345 = 0x100F;
        private const uint S23456 = 0x1F;
        private const uint S34567 = 0x3E;
        private const uint S45678 = 0x7C;
        private const uint S56789 = 0xF8;
        private const uint S678910 = 0x1F0;
        private const uint S78910J = 0x3E0;
        private const uint S8910JQ = 0x7C0;
        private const uint S910JQK = 0xF80;
        private const uint S10JQKA = 0x1F00;

        [Cudafy]
        private static uint getTop5Cards(GThread thread, int ranks)
        {
            int c = 31 - thread.clz(ranks);
            int top5cards = c << TOP_CARD_SHIFT;
            ranks ^= 1 << c;
            c = 31 - thread.clz(ranks);
            top5cards += c << SECOND_CARD_SHIFT;
            ranks ^= 1 << c;
            c = 31 - thread.clz(ranks);
            top5cards += c << THIRD_CARD_SHIFT;
            ranks ^= 1 << c;
            c = 31 - thread.clz(ranks);
            top5cards += c << FOURTH_CARD_SHIFT;
            ranks ^= 1 << c;
            c = 31 - thread.clz(ranks);
            top5cards += c << FIFTH_CARD_SHIFT;
            return (uint)top5cards;
        }

        [Cudafy]
        private static void evaluate(GThread thread, ulong[] hands, int numCards, int numHands, uint[] oranks)
        {
            int tid = thread.threadIdx.x + thread.blockIdx.x * thread.blockDim.x +
                      (thread.threadIdx.y + thread.blockIdx.y * thread.blockDim.y) * thread.gridDim.x * thread.blockDim.x;

            uint retval = 0, four_mask, three_mask, two_mask;
            if (tid < numHands)
            {
                ulong cards = hands[tid];

                // Seperate out by suit
                uint sc = (uint)(cards & 8191ul);//(uint)((cards >> (CLUB_OFFSET)) & 0x1fffUL);
                ulong c_ = cards / 8192ul;
                uint sd = (uint)(c_ & 8191ul); //(uint)((cards >> (DIAMOND_OFFSET)) & 0x1fffUL);
                c_ /= 8192ul;
                uint sh = (uint)(c_ & 8191ul); //((cards >> (HEART_OFFSET)) & 0x1fffUL);
                c_ /= 8192ul;
                uint ss = (uint)(c_ & 8191ul); // ((cards >> (SPADE_OFFSET)) & 0x1fffUL); 

                uint ranks = sc | sd | sh | ss;
                int n_ranks = thread.popcount(ranks);
                int n_dups = numCards - n_ranks;

                if (thread.popcount(sc) >= 5)
                {
                    if ((sc & S10JQKA) == S10JQKA)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 10;
                    else if ((sc & S910JQK) == S910JQK)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 9;
                    else if ((sc & S8910JQ) == S8910JQ)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 8;
                    else if ((sc & S78910J) == S78910J)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 7;
                    else if ((sc & S678910) == S678910)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 6;
                    else if ((sc & S56789) == S56789)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 5;
                    else if ((sc & S45678) == S45678)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 4;
                    else if ((sc & S34567) == S34567)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 3;
                    else if ((sc & S23456) == S23456)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 2;
                    else if ((sc & SA2345) == SA2345)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 1;
                    else
                        retval = HANDTYPE_VALUE_FLUSH + (uint)getTop5Cards(thread, (int)sc);
                }
                else if (thread.popcount(sd) >= 5)
                {
                    if ((sd & S10JQKA) == S10JQKA)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 10;
                    else if ((sd & S910JQK) == S910JQK)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 9;
                    else if ((sd & S8910JQ) == S8910JQ)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 8;
                    else if ((sd & S78910J) == S78910J)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 7;
                    else if ((sd & S678910) == S678910)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 6;
                    else if ((sd & S56789) == S56789)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 5;
                    else if ((sd & S45678) == S45678)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 4;
                    else if ((sd & S34567) == S34567)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 3;
                    else if ((sd & S23456) == S23456)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 2;
                    else if ((sd & SA2345) == SA2345)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 1;
                    else
                        retval = HANDTYPE_VALUE_FLUSH + (uint)getTop5Cards(thread, (int)sc);
                } 
                else if (thread.popcount(sh) >= 5)
                {
                    if ((sh & S10JQKA) == S10JQKA)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 10;
                    else if ((sh & S910JQK) == S910JQK)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 9;
                    else if ((sh & S8910JQ) == S8910JQ)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 8;
                    else if ((sh & S78910J) == S78910J)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 7;
                    else if ((sh & S678910) == S678910)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 6;
                    else if ((sh & S56789) == S56789)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 5;
                    else if ((sh & S45678) == S45678)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 4;
                    else if ((sh & S34567) == S34567)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 3;
                    else if ((sh & S23456) == S23456)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 2;
                    else if ((sh & SA2345) == SA2345)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 1;
                    else
                        retval = HANDTYPE_VALUE_FLUSH + (uint)getTop5Cards(thread, (int)sc);
                }
                else if (thread.popcount(ss) >= 5)
                {
                    if ((ss & S10JQKA) == S10JQKA)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 10;
                    else if ((ss & S910JQK) == S910JQK)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 9;
                    else if ((ss & S8910JQ) == S8910JQ)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 8;
                    else if ((ss & S78910J) == S78910J)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 7;
                    else if ((ss & S678910) == S678910)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 6;
                    else if ((ss & S56789) == S56789)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 5;
                    else if ((ss & S45678) == S45678)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 4;
                    else if ((ss & S34567) == S34567)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 3;
                    else if ((ss & S23456) == S23456)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 2;
                    else if ((ss & SA2345) == SA2345)
                        retval = HANDTYPE_VALUE_STRAIGHTFLUSH + 1;
                    else
                        retval = HANDTYPE_VALUE_FLUSH + (uint)getTop5Cards(thread, (int)sc);
                }
                else
                {
                    if ((ranks & S10JQKA) == S10JQKA)
                        retval = HANDTYPE_VALUE_STRAIGHT + 10;
                    else if ((ranks & S910JQK) == S910JQK)
                        retval = HANDTYPE_VALUE_STRAIGHT + 9;
                    else if ((ranks & S8910JQ) == S8910JQ)
                        retval = HANDTYPE_VALUE_STRAIGHT + 8;
                    else if ((ranks & S78910J) == S78910J)
                        retval = HANDTYPE_VALUE_STRAIGHT + 7;
                    else if ((ranks & S678910) == S678910)
                        retval = HANDTYPE_VALUE_STRAIGHT + 6;
                    else if ((ranks & S56789) == S56789)
                        retval = HANDTYPE_VALUE_STRAIGHT + 5;
                    else if ((ranks & S45678) == S45678)
                        retval = HANDTYPE_VALUE_STRAIGHT + 4;
                    else if ((ranks & S34567) == S34567)
                        retval = HANDTYPE_VALUE_STRAIGHT + 3;
                    else if ((ranks & S23456) == S23456)
                        retval = HANDTYPE_VALUE_STRAIGHT + 2;
                    else if ((ranks & SA2345) == SA2345)
                        retval = HANDTYPE_VALUE_STRAIGHT + 1;
                }
                
                if (retval == 0)
                    switch (n_dups)
                    {
                        case 0:
                            retval = HANDTYPE_VALUE_HIGHCARD + (uint)getTop5Cards(thread, (int)ranks);
                            break;
                        case 1:
                            {
                                /* It's a one-pair hand */
                                uint t, kickers;

                                two_mask = ranks ^ (sc ^ sd ^ sh ^ ss);

                                retval = (uint)(HANDTYPE_VALUE_PAIR + ((31 - thread.clz((int)two_mask)) << TOP_CARD_SHIFT));
                                t = ranks ^ two_mask;      /* Only one bit set in two_mask */
                                /* Get the top five cards in what is left, drop all but the top three 
                                    * cards, and shift them by one to get the three desired kickers */
                                kickers = ((uint)getTop5Cards(thread, (int)t) >> CARD_WIDTH) & ~FIFTH_CARD_MASK;
                                retval += kickers;
                                break;
                            }
                        case 2:
                            /* Either two pair or trips */
                            two_mask = ranks ^ (sc ^ sd ^ sh ^ ss);
                            if (two_mask != 0)
                            {
                                uint t = ranks ^ two_mask; /* Exactly two bits set in two_mask */
                                retval = (uint)(HANDTYPE_VALUE_TWOPAIR
                                    + (getTop5Cards(thread, (int)two_mask)
                                    & (TOP_CARD_MASK | SECOND_CARD_MASK))
                                    + ((31 - thread.clz((int)t)) << THIRD_CARD_SHIFT));

                                break;
                            }
                            else
                            {
                                uint t, second;
                                three_mask = ((sc & sd) | (sh & ss)) & ((sc & sh) | (sd & ss));
                                retval = (uint)(HANDTYPE_VALUE_TRIPS + ((31 - thread.clz((int)three_mask)) << TOP_CARD_SHIFT));
                                t = ranks ^ three_mask; /* Only one bit set in three_mask */
                                second = (uint)(31 - thread.clz((int)t));
                                retval += (second << SECOND_CARD_SHIFT);
                                t ^= (1U << (int)second);
                                retval += (uint)((31 - thread.clz((int)t)) << THIRD_CARD_SHIFT);
                                break;
                            }
                        default:
                            /* Possible quads, fullhouse, straight or flush, or two pair */
                            four_mask = sh & sd & sc & ss;
                            if (four_mask != 0)
                            {
                                uint tc = (uint)(31 - thread.clz((int)four_mask));
                                retval = (uint)(HANDTYPE_VALUE_FOUR_OF_A_KIND
                                    + (tc << TOP_CARD_SHIFT)
                                    + ((31 - thread.clz((int)(ranks ^ (1U << (int)tc)))) << SECOND_CARD_SHIFT));
                                break;
                            };

                            /* Technically, three_mask as defined below is really the set of
                                bits which are set in three or four of the suits, but since
                                we've already eliminated quads, this is OK */
                            /* Similarly, two_mask is really two_or_four_mask, but since we've
                                already eliminated quads, we can use this shortcut */

                            two_mask = ranks ^ (sc ^ sd ^ sh ^ ss);
                            if (thread.popcount(two_mask) != n_dups)
                            {
                                /* Must be some trips then, which really means there is a 
                                    full house since n_dups >= 3 */
                                uint tc, t;
                                three_mask = ((sc & sd) | (sh & ss)) & ((sc & sh) | (sd & ss));
                                retval = HANDTYPE_VALUE_FULLHOUSE;
                                tc = (uint)(31 - thread.clz((int)three_mask));
                                retval += (tc << TOP_CARD_SHIFT);
                                t = (two_mask | three_mask) ^ (1U << (int)tc);
                                retval += (uint)(31 - thread.clz((int)t) << SECOND_CARD_SHIFT);
                                break;
                            };

                            if (retval == 0)
                            {
                                /* Must be two pair */
                                uint top, second;

                                retval = HANDTYPE_VALUE_TWOPAIR;
                                top = (uint)(31 - thread.clz((int)two_mask));
                                retval += (top << TOP_CARD_SHIFT);
                                second = (uint)(31 - thread.clz((int)(two_mask ^ (1 << (int)top))));
                                retval += (second << SECOND_CARD_SHIFT);
                                retval += (uint)((31 - thread.clz((int)(ranks ^ (1U << (int)top) ^ (1 << (int)second)))) << THIRD_CARD_SHIFT);

                            }
                            break;
                    }
                    /*
                     * By the time we're here, either: 
                       1) there's no five-card hand possible (flush or straight), or
                       2) there's a flush or straight, but we know that there are enough
                          duplicates to make a full house / quads possible.  
                     */

                oranks[tid] = retval;
            }
        }

        public static uint[] Evaluate(ulong[] hands, int numCards)
        {
            // Translates this class to CUDA C and then compliles
            CudafyModule km = CudafyTranslator.Cudafy();//eArchitecture.sm_20);

            // Get the first GPU and load the module
            GPGPU gpu = CudafyHost.GetDevice(CudafyModes.Target, CudafyModes.DeviceId);
            gpu.LoadModule(km);

            int blockSize = 256;
            int blockx = hands.Length / blockSize;
            if (hands.Length % blockSize != 0)
                blockx++;

            ulong[] dev_hands = gpu.Allocate<ulong>(hands.Length);
            uint[] dev_ranks = gpu.Allocate<uint>(hands.Length);

            gpu.CopyToDevice(hands, dev_hands);

            gpu.StartTimer();
            gpu.Launch(blockx, blockSize).evaluate(dev_hands, numCards, hands.Length, dev_ranks);
            var ts = gpu.StopTimer();

            uint[] toReturn = new uint[hands.Length];
            gpu.CopyFromDevice(dev_ranks, toReturn);

            return toReturn;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ConvertHand(this Card[] hand)
        {
            ulong handmask = 0UL;

            // Parse the hand
            for (int i = 0; i < hand.Length; i++)
            {
                var card = GetCardBit(hand[i]);
                handmask |= (1UL << card);
            }
            return handmask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ConvertCard(this Card card)
        {
            ulong handmask = 0UL;
            handmask |= (1UL << GetCardBit(card));
            return handmask;
        }

        private static int GetCardBit(Card card)
        {
            int rank = (int)card.Rank, suit = (int)card.Suit;
            
            return rank + (suit * 13);
        }

        public static uint GetHandType(uint handValue)
        {
            return (handValue >> HANDTYPE_SHIFT);
        }

        public static string DescriptionFromMask(ulong cards, uint handvalue)
        {
            int numberOfCards = BitCount(cards);

#if DEBUG
            // This functions supports 1-7 cards
            if (numberOfCards < 1 || numberOfCards > 7)
                throw new ArgumentOutOfRangeException("numberOfCards");
#endif
            // Seperate out by suit
            uint sc = (uint)((cards >> (CLUB_OFFSET)) & 0x1fffUL);
            uint sd = (uint)((cards >> (DIAMOND_OFFSET)) & 0x1fffUL);
            uint sh = (uint)((cards >> (HEART_OFFSET)) & 0x1fffUL);
            uint ss = (uint)((cards >> (SPADE_OFFSET)) & 0x1fffUL);

            switch ((HandType)GetHandType(handvalue))
            {
                case HandType.HighCard:
                case HandType.Pair:
                case HandType.TwoPair:
                case HandType.Trips:
                case HandType.Straight:
                case HandType.FullHouse:
                case HandType.FourOfAKind:
                    return DescriptionFromHandValueInternal(handvalue);
                case HandType.Flush:
                    if (ss.BitCount() >= 5)
                    {
                        return "Flush (Spades) with " + ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (sc.BitCount() >= 5)
                    {
                        return "Flush (Clubs) with " + ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (sd.BitCount() >= 5)
                    {
                        return "Flush (Diamonds) with " + ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (sh.BitCount() >= 5)
                    {
                        return "Flush (Hearts) with " + ranktbl[TopCard(handvalue)] + " high";
                    }
                    break;
                case HandType.StraightFlush:
                    if (ss.BitCount() >= 5)
                    {
                        return "Straight Flush (Spades) with " + ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (sc.BitCount() >= 5)
                    {
                        return "Straight (Clubs) with " + ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (sd.BitCount() >= 5)
                    {
                        return "Straight (Diamonds) with " + ranktbl[TopCard(handvalue)] + " high";
                    }
                    else if (sh.BitCount() >= 5)
                    {
                        return "Straight  (Hearts) with " + ranktbl[TopCard(handvalue)] + " high";
                    }
                    break;
            }
            return "";
        }

        public static uint TopCard(System.UInt32 hv)
        {
            return ((hv >> TOP_CARD_SHIFT) & CARD_MASK);
        }

        /// <exclude/>
        private static uint SECOND_CARD(System.UInt32 hv)
        {
            return (((hv) >> SECOND_CARD_SHIFT) & CARD_MASK);
        }

        /// <exclude/>
        private static uint THIRD_CARD(System.UInt32 hv)
        {
            return (((hv) >> THIRD_CARD_SHIFT) & CARD_MASK);
        }

        /// <exclude/>
        private static uint FOURTH_CARD(System.UInt32 hv)
        {
            return (((hv) >> FOURTH_CARD_SHIFT) & CARD_MASK);
        }

        /// <exclude/>
        private static uint FIFTH_CARD(System.UInt32 hv)
        {
            return (((hv) >> FIFTH_CARD_SHIFT) & CARD_MASK);
        }

        /// <exclude/>
        private static uint HANDTYPE_VALUE(HandType ht)
        {
            return (((uint)ht) << HANDTYPE_SHIFT);
        }

        private static string DescriptionFromHandValueInternal(uint handValue)
        {
            StringBuilder b = new StringBuilder();

            switch ((HandType)GetHandType(handValue))
            {
                case HandType.HighCard:
                    b.Append("High card: ");
                    b.Append(ranktbl[TopCard(handValue)]);
                    return b.ToString();
                case HandType.Pair:
                    b.Append("One pair, ");
                    b.Append(ranktbl[TopCard(handValue)]);
                    return b.ToString();
                case HandType.TwoPair:
                    b.Append("Two pair, ");
                    b.Append(ranktbl[TopCard(handValue)]);
                    b.Append("'s and ");
                    b.Append(ranktbl[SECOND_CARD(handValue)]);
                    b.Append("'s with a ");
                    b.Append(ranktbl[THIRD_CARD(handValue)]);
                    b.Append(" for a kicker");
                    return b.ToString();
                case HandType.Trips:
                    b.Append("Three of a kind, ");
                    b.Append(ranktbl[TopCard(handValue)]);
                    b.Append("'s");
                    return b.ToString();
                case HandType.Straight:
                    b.Append("A straight, ");
                    b.Append(ranktbl[TopCard(handValue)]);
                    b.Append(" high");
                    return b.ToString();
                case HandType.Flush:
                    b.Append("A flush");
                    return b.ToString();
                case HandType.FullHouse:
                    b.Append("A fullhouse, ");
                    b.Append(ranktbl[TopCard(handValue)]);
                    b.Append("'s and ");
                    b.Append(ranktbl[SECOND_CARD(handValue)]);
                    b.Append("'s");
                    return b.ToString();
                case HandType.FourOfAKind:
                    b.Append("Four of a kind, ");
                    b.Append(ranktbl[TopCard(handValue)]);
                    b.Append("'s");
                    return b.ToString();
                case HandType.StraightFlush:
                    b.Append("A straight flush");
                    return b.ToString();
            }
            return "";
        }

        public static string MaskToString(ulong mask)
        {
            StringBuilder builder = new StringBuilder();
            int count = 0;
            for (int i = 51; i >= 0; i--)
            {
                if (((1UL << i) & mask) != 0)
                {
                    if (count != 0)
                    {
                        builder.Append(" ");
                    }
                    builder.Append(Hand.CardTable[i]);
                    count++;
                }
            }
            return builder.ToString();
        }
    }
}
