using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Konsole;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // partitions starting from 00 -> 99
            var partitions = Enumerable.Range(0, 100).Select(x => x.ToString("D2"));

            foreach (var partition in partitions)
            {
                var unitOfWork = new UnitOfWork(partition);
                var progressBar = new ProgressBar(100);
                unitOfWork.ReportProgress = progress => progressBar.Refresh(Convert.ToInt32(progress), String.Empty);
                PerformWork(unitOfWork);
            }
        }

        public static void PerformWork(UnitOfWork uow)
        {
            var random = new Random();
            var work = Enumerable.Range(0, random.Next(1000)).ToList();

            Console.WriteLine($"Begin Partition {uow.Partition}");
            Console.WriteLine("Some other logging");
            Console.WriteLine("Some other logging");
            Console.WriteLine("Some other logging");

            var completed = 0;
            Parallel.ForEach(work, i =>
            {
                Task.Delay(i);
                Interlocked.Increment(ref completed);
                uow.ReportProgress(completed * 100.0 / work.Count);
            });
            uow.ReportProgress(100.0);

            Console.WriteLine("Some other logging");
            Console.WriteLine("Some other logging");
            Console.WriteLine("Some other logging");
            Console.WriteLine($"End Partition {uow.Partition}");
        }
    }

    public class UnitOfWork
    {
        public String Partition { get; set; }
        public Action<Double> ReportProgress { get; set; }

        public UnitOfWork(String partition)
        {
            Partition = partition;
        }
    }
}
