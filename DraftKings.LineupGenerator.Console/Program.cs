using System;
using System.Threading.Tasks;

namespace draftkings_lineup_generator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Yield();

            Console.WriteLine("Hello, World!");
        }
    }
}