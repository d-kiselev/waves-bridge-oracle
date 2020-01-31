using System;
using System.Collections.Generic;
using System.Threading;
using WavesCS;

namespace wavesbridgeoracle
{
    class MainClass
    {
        public static string chainCollectorPSeed = "take purity craft away cake month layer napkin theme slam explain seed take purity craft";
        public static string chainCollectorQSeed = "void entire theme slam explain seed take purity craft away cake month layer napkin nasty";
        public static string tokenPortPSeed = "theme slam explain seed take purity craft away cake month layer napkin nasty void entire";
        public static string tokenPortQSeed = "seed take purity craft away cake month layer napkin nasty void entire theme slam explain";
        public static string AlicePSeed = "whip disagree egg melt satisfy repeat engine envelope federal toward shoulder cattle rare much lava";
        public static string BobQSeed = "whip disagree egg satisfy repeat engine envelope federal toward shoulder cattle rare much lava melt";

        public static PrivateKeyAccount chainCollectorP = PrivateKeyAccount.CreateFromSeed(chainCollectorPSeed, 'P');
        public static PrivateKeyAccount chainCollectorQ = PrivateKeyAccount.CreateFromSeed(chainCollectorQSeed, 'Q');
        public static PrivateKeyAccount tokenPortP = PrivateKeyAccount.CreateFromSeed(tokenPortPSeed, 'P');
        public static PrivateKeyAccount tokenPortQ = PrivateKeyAccount.CreateFromSeed(tokenPortQSeed, 'Q');
        public static PrivateKeyAccount AliceP = PrivateKeyAccount.CreateFromSeed(AlicePSeed, 'P');
        public static PrivateKeyAccount BobQ = PrivateKeyAccount.CreateFromSeed(BobQSeed, 'Q');
        public static PrivateKeyAccount faucetP = PrivateKeyAccount.CreateFromSeed("seed", 'P');
        public static PrivateKeyAccount faucetQ = PrivateKeyAccount.CreateFromSeed("seed", 'Q');

        public static Node nodeP;
        public static Node nodeQ;

        public static void Init()
        {
            chainCollectorP = PrivateKeyAccount.CreateFromSeed(chainCollectorPSeed, 'P');
            chainCollectorQ = PrivateKeyAccount.CreateFromSeed(chainCollectorQSeed, 'Q');
            tokenPortP = PrivateKeyAccount.CreateFromSeed(tokenPortPSeed, 'P');
            tokenPortQ = PrivateKeyAccount.CreateFromSeed(tokenPortQSeed, 'Q');
            AliceP = PrivateKeyAccount.CreateFromSeed(AlicePSeed, 'P');
            BobQ = PrivateKeyAccount.CreateFromSeed(BobQSeed, 'Q');
            faucetP = PrivateKeyAccount.CreateFromSeed("seed", 'P');
            faucetQ = PrivateKeyAccount.CreateFromSeed("seed", 'Q');

            nodeP = new Node("http://127.0.0.1:6870", 'P');
            nodeQ = new Node("http://127.0.0.1:6869", 'Q');
        }

        public static void Main(string[] args)
        {
            Init();

            // publish Merkle roots from network P to network Q

            Console.WriteLine($"Chain collector address: {chainCollectorQ.Address}");

            while (true)
            {
                try
                {
                    var height = nodeP.GetHeight() - 1;
                    var merkleRoot = nodeP.GetMerkleRootAtHeight(height);
                    var key = height.ToString() + "_transactionsRoot";

                    if (!nodeQ.GetAddressData(chainCollectorQ.Address).ContainsKey(key))
                    {
                        nodeQ.PutData(chainCollectorQ, new Dictionary<string, object> { { key, merkleRoot } });
                        Console.WriteLine($"height {height} added");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Thread.Sleep(10000);
                }
            }

        }
    }

    static class NodeExtentions
    {
        public static byte[] GetMerkleRootAtHeight(this Node node, int height)
        {
            return node.GetObject($"blocks/headers/at/{height}").GetString("transactionsRoot").FromBase58();
        }
    }
}
