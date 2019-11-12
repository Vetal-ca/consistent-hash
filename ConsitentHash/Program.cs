using System;
using System.Collections.Generic;

namespace ConsitentHash
{
    class Server
    {
        public int ID { get; set; }

        public Server(int _id)
        {
            ID = _id;
        }

        public override int GetHashCode()
        {
            // https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
            String str = "svr_" + ID;
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }

    class Program
    {
        private static List<Server> CreateHash(int numServers, out ConsistentHash<Server> ch)
        {
            List<Server> servers = new List<Server>();
            for (int i = 0; i < numServers; i++)
            {
                servers.Add(new Server(i));
            }

            ch = new ConsistentHash<Server>();
            ch.Init(servers);

            return servers;
        }

        private static void Test()
        {
            var servers = CreateHash(1000, out var ch);

            int search = 100000;

            DateTime start = DateTime.Now;
            SortedList<int, int> ay1 = new SortedList<int, int>();
            for (int i = 0; i < search; i++)
            {
                int temp = ch.GetNode(i.ToString()).ID;

                ay1[i] = temp;
            }

            TimeSpan ts = DateTime.Now - start;
            Console.WriteLine(search + " each use macro seconds: " + (ts.TotalMilliseconds / search) * 1000);

            //ch.Add(new Server(1000));
            ch.Remove(servers[1]);
            SortedList<int, int> ay2 = new SortedList<int, int>();
            for (int i = 0; i < search; i++)
            {
                int temp = ch.GetNode(i.ToString()).ID;

                ay2[i] = temp;
            }

            int diff = 0;
            for (int i = 0; i < search; i++)
            {
                if (ay1[i] != ay2[i])
                {
                    diff++;
                }
            }

            Console.WriteLine("diff: " + diff);
        }

        private static void ConsistencyTest(int numServers, int numTests, int numRuns)
        {
            CreateHash(numServers, out var ch);

            for (int k = 0; k < numRuns; k++)
            {
                Console.Write("Test {0}: -------- \n[", k);
                for (int i = 0; i < numTests; i++)
                {
                    int temp = ch.GetNode(i.ToString()).ID;

                    Console.Write(" {0}", temp);
                }

                Console.WriteLine(" ]\n");
            }
        }

        private static void ServerAddTest(int numServersFrom, int numServersTo, int numTests)
        {
            for (int ns = numServersFrom; ns <= numServersTo; ns++)
            {
                CreateHash(ns, out var ch);
                Console.Write("Num servers {0}: [", ns);
                for (int i = 0; i < numTests; i++)
                {
                    int temp = ch.GetNode(i.ToString()).ID;

                    Console.Write(" {0}", temp);
                }

                Console.WriteLine(" ]\n");
            }
        }

        // https://medium.com/@dgryski/consistent-hashing-algorithmic-tradeoffs-ef6b8e2fcae8
        static void Main(string[] args)
        {
//            Test();
//            ConsistencyTest(4, 10, 5);
            ServerAddTest(3, 8, 50);
        }
    }
}