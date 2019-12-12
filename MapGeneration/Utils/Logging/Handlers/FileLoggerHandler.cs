using System.IO;

namespace MapGeneration.Utils.Logging.Handlers
{
    public class FileLoggerHandler : ILoggerHandler
    {
        private readonly string filename;

        public FileLoggerHandler(string filename)
        {
            this.filename = filename;
        }

        public void Write(string text)
        {
            using (var writer = new StreamWriter(File.Open(filename, FileMode.Append)))
            {
                writer.Write(text);
            }
        }
    }
}