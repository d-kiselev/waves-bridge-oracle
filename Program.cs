using System;
using System.Collections.Generic;
using System.Threading;
using WavesCS;

namespace wavesbridgeoracle
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var node = new Node(Node.TestNetChainId);
            var nodeInOtherChain = new Node("https://nodes-stagenet.wavesnodes.com/", 'S');

            var oracleSeed = "void entire theme slam explain seed take purity craft away cake month layer napkin nasty";
            var oracle = PrivateKeyAccount.CreateFromSeed(oracleSeed, node.ChainId);
            Console.WriteLine($"oracle address: {oracle.Address}");

            while (true)
            {
                try
                {
                    var height = nodeInOtherChain.GetHeight();
                    var merkleRoot = nodeInOtherChain.GetMerkleRootAtHeight(height - 15);
                    var key = height.ToString() + "_merkleRoot";

                    if (!node.GetAddressData(oracle.Address).ContainsKey(key))
                    {
                        node.PutData(oracle, new Dictionary<string, object> { { key, merkleRoot } });
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
        public static string GetMerkleRootAtHeight(this Node node, int height)
        {
            return node.GetObject($"blocks/headers/at/{height}").GetString("generator"); // .GetString("txMerkleRoot");
        }
    }
}
