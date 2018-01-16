namespace Dawg.Compact.Benchmark
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    public class WordDictionary : IDisposable, IEnumerable<string>
    {
        private readonly FileStream _stream;

        public WordDictionary(string fileName)
        {
            var path = fileName;
            try
            {
                _stream = File.OpenRead(path);
            }
            catch
            {
                Console.WriteLine();
                Console.WriteLine("Error!");
                Console.WriteLine($"Word list not fount at {path}!");
                Console.ReadLine();
                throw;
            }
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                try
                {
                    _stream.Close();
                }
                catch
                {
                    //already closed
                }             
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            using (var reader = new StreamReader(_stream))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return GetEnumerator();
        }
    }
}
