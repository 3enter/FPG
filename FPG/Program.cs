using FPG.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPG
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Transaction<char>> database = new List<Transaction<char>>();
            database.Add(new Transaction<char>('f', 'a', 'c', 'd', 'g', 'i', 'm', 'p'));
            database.Add(new Transaction<char>('a', 'b', 'c', 'f', 'l', 'm', 'o'));
            database.Add(new Transaction<char>('b', 'f', 'h', 'j', 'o', 'w'));
            database.Add(new Transaction<char>('b', 'c', 'k', 's', 'p'));
            database.Add(new Transaction<char>('a', 'f', 'c', 'e', 'l', 'p', 'm', 'n'));
            database.Add(new Transaction<char>('a', 'f', 'c', 'e', 'l', 'p', 'm', 'n'));
            database.Add(new Transaction<char>('a', 'f', 'c', 'e', 'l', 'p', 'm', 'n'));
            database.Add(new Transaction<char>('a', 'f', 'c', 'e', 'l', 'p', 'm', 'n','z'));
            database.Add(new Transaction<char>('a', 'f', 'c', 'e', 'l', 'p', 'm', 'n'));
            database.Add(new Transaction<char>('a', 'h', 'c', 'e', 'o', 'p', 'm', 'n'));
            database.Add(new Transaction<char>('a', 'f', 'c', 'e', 'l', 'o', 'm', 'n'));
            database.Add(new Transaction<char>('a', 'f', 'c', 'e', 'l', 'p', 'm', 'n'));
            database.Add(new Transaction<char>('a', 'j', 'c', 'e', 'b', 'p', 'm', 'n'));


            DateTime start_time = DateTime.UtcNow;
            FPGrowth<char> method = new FPGrowth<char>();
            var ftDomain = Transaction<char>.ExtractDomain(database);
            ItemSets<char> fis = method.MinePatterns(database, ftDomain, 0.4);
            DateTime end_time = DateTime.UtcNow;
            Show(fis);
            Console.WriteLine("Time Span: {0} ms", (end_time - start_time).TotalMilliseconds);

            Console.WriteLine("Finding Closed Pattern");
            Show(method.FindMaxPatterns(database, Transaction<char>.ExtractDomain(database), 0.4));

            Console.ReadKey();
        }
        private static void Show(ItemSets<char> fis)
        {
            for (int i = 0; i < fis.Count; ++i)
            {
                Console.WriteLine("{0} (Support: {1})", fis[i], fis[i].Support);
            }
        }

    }
}
