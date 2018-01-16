namespace Dawg.Compact.Build
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    public class WordListFileSource : IDisposable, IEnumerable<string>
    {
        private FileStream _stream;

        public WordListFileSource(string filePath)
        {
            _stream = File.OpenRead(filePath);
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
