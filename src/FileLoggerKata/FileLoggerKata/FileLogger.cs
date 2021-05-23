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
        public void Log(string message)
        {
            var filename = GetFileName();
            var prepend = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";

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
            var append = $"{DateTime.Now:yyyyMMdd}";
            return Path.Combine(dir, $"log{append}.txt");
        }
    }
}
