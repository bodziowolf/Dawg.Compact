namespace Dawg.Compact.Demo
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            Console.WriteLine("Building dictionary...");

            var matcher = new DawgBuilder()
                .WithOrderedWordsFromFile("scrabble-polish-words.txt")
                .BuildCompactDawg();

            var query = "";

            while (true)
            {
                Console.CursorVisible = false;
                Console.Clear();

                var prompt = "Type to see completions> ";
                Console.WriteLine(prompt + query + "_");
                var indent = new string(' ', prompt.Length);
                if (query.Length > 0)
                {
                    var matches = matcher.GetWordsByPrefix(query).Take(10);
                    if (!matches.Any())
                    {
                        Console.WriteLine(indent + "<No matches>");
                    }
                    else
                    {
                        foreach (var match in matches)
                        {
                            Console.WriteLine(indent + match);
                        }
                    }
                }

                Console.WriteLine("\nPress esc to exit.");

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Backspace && query.Length > 0)
                {
                    query = query.Substring(0, query.Length - 1);
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return;
                }
                else
                {
                    query += key.KeyChar;
                }
            }
        }
    }
}
