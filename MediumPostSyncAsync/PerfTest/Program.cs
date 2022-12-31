// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Running single-thread sync code ...");
            for (int i = 0; i < 100; i++)
            {
                string text = (new WebClient()).DownloadString("https://google.com/");
                Console.WriteLine($"Operation number : {i} executed on thread : {Thread.CurrentThread.ManagedThreadId}");
            }
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"The single-thread sync way took : {stopwatch.ElapsedMilliseconds} miliseconds");
            
            Console.WriteLine("Running multi-threaded sync code ...");
            stopwatch.Restart();
            var threads = new List<Thread>();
            for (int i = 0; i < 100; i++)
            {
                Thread thread = new Thread(() => ReadFromThread(i));
                thread.Start();
                threads.Add(thread);
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            stopwatch.Stop();
            Console.WriteLine($"The multi-thread sync way took : {stopwatch.ElapsedMilliseconds} miliseconds");
            
            stopwatch.Restart();
            Console.WriteLine("Running single-thread async code ...");
            var tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(ReadAsync(i));
            }
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"The single-thread async way took : {stopwatch.ElapsedMilliseconds} miliseconds");
            
            Console.WriteLine("Running multi-thread async code ...");
            stopwatch.Restart();
            var threads2 = new List<Thread>();
            for (int i = 0; i < 10; i++)
            {
                Thread thread = new Thread(() => ReadFromThreadAsync(i));
                thread.Start();
                threads2.Add(thread);
            }
            foreach (var thread in threads2)
            {
                thread.Join();
            }
            stopwatch.Stop();
            Console.WriteLine($"The multi-thread async way took : {stopwatch.ElapsedMilliseconds} miliseconds");
        }

        private static async Task<string> ReadAsync(int i)
        {
            var text = await (new WebClient()).DownloadStringTaskAsync("https://google.com/");
            Console.WriteLine($"Operation number : {i} executed on thread : {Thread.CurrentThread.ManagedThreadId}");
            return text;
        }


        static void ReadFromThread(int i)
        {
            string text = (new WebClient()).DownloadString("https://google.com/");
            Console.WriteLine($"Operation number : {i} executed on thread : {Thread.CurrentThread.ManagedThreadId}");
        }

        static void ReadFromThreadAsync(int nr)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add((ReadAsync(nr * 10 + i)));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}