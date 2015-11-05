using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCore
{
    public static class Utils
    {
        public static string ToDisplay(this CardRank rank)
        {
            if (rank == CardRank.Ace)
                return "A";
            if (rank == CardRank.King)
                return "K";
            if (rank == CardRank.Queen)
                return "Q";
            if (rank == CardRank.Jack)
                return "J";

            return (((int)rank) + 1).ToString();
        }

        public static byte BitCount(this ulong value)
        {
            ulong result = value - ((value >> 1) & 0x5555555555555555UL);
            result = (result & 0x3333333333333333UL) + ((result >> 2) & 0x3333333333333333UL);
            return (byte)(unchecked(((result + (result >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }

        public static byte BitCount(this uint value)
        {
            return BitCount((ulong)value);
        }
    }
}
