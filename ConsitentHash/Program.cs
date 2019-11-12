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
            return ("svr_" + ID).GetHashCode();
        }
    }

    class Program
    {
        private static void Test()
        {
            List<Server> servers = new List<Server>();
            for (int i = 0; i < 1000; i++)
            {
                servers.Add(new Server(i));
            }

            ConsistentHash<Server> ch = new ConsistentHash<Server>();
            ch.Init(servers);

            int search = 100000;

            DateTime start = DateTime.Now;
            SortedList<int, int> ay1 = new SortedList<int, int>();
            for (int i = 0; i < search; i++)
            {
                int temp = ch.GetNode(i.ToString()).ID;

                ay1[i] = temp;
                Console.WriteLine(temp);
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
            List<Server> servers = new List<Server>();
            for (int i = 0; i < numServers; i++)
            {
                servers.Add(new Server(i));
            }

            ConsistentHash<Server> ch = new ConsistentHash<Server>();
            ch.Init(servers);

            for (int k = 0; k < numRuns; k++)
            {
                Console.Write("Test {0}: -------- \n[", k);
                for (int i = 0; i < numTests; i++)
                {
                    int temp = ch.GetNode(i.ToString()).ID;
                    
                    Console.Write(" {0}",temp);
                }
                Console.WriteLine(" ]\n");
            }
        }

        static void Main(string[] args)
        {
//            Test();
            ConsistencyTest(3, 10, 5);
        }
    }
}