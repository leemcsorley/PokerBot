using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;
using PokerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerConsole
{
    class Program
    {
        static void SwapEndianness(ulong[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Array.Reverse(b);
                values[i] = BitConverter.ToUInt64(b, 0);
            }
        }

        static void SwapEndianness(uint[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Array.Reverse(b);
                values[i] = BitConverter.ToUInt32(b, 0);
            }
        }

        static void Main(string[] args)
        {
            CudafyModes.Target = eGPUType.OpenCL; // To use OpenCL, change this enum
            CudafyModes.DeviceId = 0;
            CudafyTranslator.Language = CudafyModes.Target == eGPUType.OpenCL || CudafyModes.Target == eGPUType.Emulator ? eLanguage.OpenCL : eLanguage.Cuda;

            var hands = PokerCore.Gpu.Hand.Hands(7).Take(10).ToArray();

            var o = PokerCore.Gpu.Hand.Evaluate(hands, 7);

            var s = PokerCore.Gpu.Hand.DescriptionFromMask(hands[0], o[0]);

            var rnd = new MathNet.Numerics.Random.MersenneTwister();
            var deck = new Deck(rnd);

            while (true)
            {
                deck.ResetAndShuffle();
                Card[] c;
                Hand.Sort7Rank(c = new Card[] { deck.DealCard(), deck.DealCard(), deck.DealCard(), deck.DealCard(), deck.DealCard(), deck.DealCard(), deck.DealCard() });

                var p = CardRank.Two;
                for (var i = 0; i < c.Length; i++)
                {
                    if (c[i].Rank < p)
                        throw new Exception();
                    p = c[i].Rank;
                }
            }
        }
    }
}
