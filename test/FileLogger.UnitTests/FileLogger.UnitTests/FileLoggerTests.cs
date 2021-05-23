using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileLogger.UnitTests
{
    public class FileLoggerTests
    {
        public FileLoggerTests()
        {
            var files = Directory.GetFiles(GetPath(), "log*.txt");
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        [Fact]
        public void LogSimple()
        {
            var now = DateTime.Now;
            var sut = new FileLoggerKata.FileLogger();
            sut.Log("simple log");

            var file = GetFile(now);
            var text = GetLastLine(file);

            text.Should().Be($"{now:yyyy-MM-dd HH:mm:ss} simple log");
        }

        private string GetFile(DateTime now)
        {
            return $"log{now:yyyyMMdd}.txt";
        }

        private string GetPath()
        {
            return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
        }

        private string GetPath(string file)
        {
            return Path.Combine(GetPath(), file);
        }

        private string GetLastLine(string file)
        {
            try
            {
                var lines = File.ReadAllLines(GetPath(file));
                return lines[lines.Length - 1];
            }
            catch
            {
                return "";
            }
        }


    }
}
