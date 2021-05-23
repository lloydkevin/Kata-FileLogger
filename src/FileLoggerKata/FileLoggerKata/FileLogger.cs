using FileLoggerKata.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var filename = GetFileName();
            var prepend = $"{_currentTime.Now:yyyy-MM-dd HH:mm:ss}";

            if (!FileExists(filename))
            {
                CreateFile(filename);
            }

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

        private string GetFileName()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var now = _currentTime.Now;
            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
                return Path.Combine(dir, $"weekend.txt");

            var append = $"{now:yyyyMMdd}";
            return Path.Combine(dir, $"log{append}.txt");
        }
    }
}
