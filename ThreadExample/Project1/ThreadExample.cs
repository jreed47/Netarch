using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;

namespace Project1
{
    class ThreadExample
    {
        private static readonly int WAIT_TIME = 40;
        private static readonly int BUF_LEN = 20;
        private static Queue que;
        private static Semaphore que_lock;
        private static Semaphore alert_sem;

        public static int Main()
        {
            que = new Queue();
            que_lock = new Semaphore(1, 1);
            alert_sem = new Semaphore(0, BUF_LEN);

            Thread cons = new Thread(new ThreadStart(Consumer));
            cons.Start();

            Producer();

            cons.Join();

            return 0;
        }

        public static void Producer()
        {
            Console.WriteLine("Prod");

            for (int i = 0; i < BUF_LEN; i++)
            {
                que_lock.WaitOne();
                que.Enqueue(i);
                Console.WriteLine("P: {0}", i);
                que_lock.Release();
                
                alert_sem.Release(1);
                Thread.Sleep(WAIT_TIME);
            }
        }

        public static void Consumer()
        {
            Console.WriteLine("Consumer");
            for (int i = 0; i < BUF_LEN; i++)
            {
                alert_sem.WaitOne();

                que_lock.WaitOne();
                if (que.Count > 0)
                {
                    int obj = (int)que.Dequeue();
                    Console.WriteLine("C: {0}", obj);
                }
                else
                {
                    Console.WriteLine("C: queue empty");
                }
                que_lock.Release();
            }
        }
    }
}
