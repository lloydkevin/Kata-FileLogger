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
            File.AppendAllText(filename, $"\r\n{message}");
        }

        private string GetFileName()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(dir, "log.txt");
        }
    }
}
