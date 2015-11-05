using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;
using PokerCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
            var games = PokerCore.Data.FullHistory.LoadHistoryBin(@"C:\Data\Poker\2015-11-03_STA_NL10_SH_QLVWN231\binary.dat");

            var preflopModel = new PokerCore.Model.PreflopKNN(games);

            BinaryFormatter bf = new BinaryFormatter();
            int count = 0;
            using (FileStream fs = new FileStream(@"C:\Data\Poker\2015-11-03_STA_NL10_SH_QLVWN231\binary.dat", FileMode.OpenOrCreate))
            using (GZipStream zs = new GZipStream(fs, CompressionLevel.Fastest))
            {
                foreach (var game in PokerCore.Data.FullHistory.LoadHistoryHH(@"C:\Data\Poker\2015-11-03_STA_NL10_SH_QLVWN231").Take(100000))
                {
                    bf.Serialize(zs, game);
                    count++; 
                    if (count % 100 == 0)
                        Console.WriteLine(count);

                    
                }
            }
            

            CudafyModes.Target = eGPUType.OpenCL; // To use OpenCL, change this enum
            CudafyModes.DeviceId = 0;
            CudafyTranslator.Language = CudafyModes.Target == eGPUType.OpenCL || CudafyModes.Target == eGPUType.Emulator ? eLanguage.OpenCL : eLanguage.Cuda;
            CudafyTranslator.GenerateDebug = true;

            var hands = PokerCore.Gpu.Hand.RandomHands(7, 256 * 256 * 256 * 2).ToArray();

            //var hands = new Card[][] { new[] { Card.KingClubs, Card.KingDiamonds, Card.EightDiamonds, Card.SixHearts, Card.EightClubs, Card.SevenClubs, Card.TwoClubs } }
            //    .Select(c => PokerCore.Gpu.Hand.ConvertHand(c)).ToArray();

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
