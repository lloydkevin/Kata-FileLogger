using FileLoggerKata.Interfaces;
using System;
using System.IO;

namespace FileLoggerKata
{
    public class FileLogger
    {
        private ICurrentTime _currentTime;

        public FileLogger(ICurrentTime currentTime = null)
        {
            _currentTime = currentTime ?? new CurrentTime();
        }

        public void Log(string message)
        {
            var filename = CreateLogFile();

            var prepend = $"{_currentTime.Now:yyyy-MM-dd HH:mm:ss}";
            WriteMessage(filename, $"{prepend} {message}\r\n");
        }

        private static void WriteMessage(string filename, string message)
        {
            File.AppendAllText(filename, message);
        }

        private void CreateFile(string filename)
        {
            using var stream = File.Create(filename);
        }

        private bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        private string CreateLogFile()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var now = _currentTime.Now;
            var append = $"{now:yyyyMMdd}";
            var filename = Path.Combine(dir, $"log{append}.txt");

            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
            {
                filename = Path.Combine(dir, $"weekend.txt");
            }

            if (!FileExists(filename))
            {
                CreateFile(filename);
            }
            return filename;
        }
    }
}