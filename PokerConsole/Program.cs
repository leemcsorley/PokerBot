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
            CudafyModes.Target = eGPUType.Cuda; // To use OpenCL, change this enum
            CudafyModes.DeviceId = 0;
            CudafyTranslator.Language = CudafyModes.Target == eGPUType.OpenCL || CudafyModes.Target == eGPUType.Emulator ? eLanguage.OpenCL : eLanguage.Cuda;
            CudafyTranslator.GenerateDebug = true;

            var hands = PokerCore.Gpu.Hand.RandomHands(7, 10000000).ToArray();

            var o = PokerCore.Gpu.Hand.Evaluate(hands, 7);

            for (int i = 0; i < hands.Length; i++)
            {
                var s = PokerCore.Gpu.Hand.DescriptionFromMask(hands[i], o[i]);
                var hs = PokerCore.Gpu.Hand.MaskToString(hands[i]);
            }

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
