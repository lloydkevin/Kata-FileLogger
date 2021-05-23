using FileLoggerKata.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace FileLogger.UnitTests
{
    public class FileLoggerTests
    {
        public FileLoggerTests()
        {
            DeleteFiles("log*.txt");
            DeleteFiles("weekend*.txt");
        }

        private void DeleteFiles(string search)
        {
            var files = Directory.GetFiles(GetPath(), search);
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

            var file = GetLastTextFile();
            var text = GetLastLine(file);

            text.Should().Be($"{now:yyyy-MM-dd HH:mm:ss} simple log");
        }

        [Fact]
        public void WithAMTime_ShouldLogAM()
        {
            var now = new DateTime(2021, 1, 1, 8, 10, 30);
            var currentTime = new Mock<ICurrentTime>();
            currentTime.Setup(x => x.Now).Returns(now);
            var sut = new FileLoggerKata.FileLogger(currentTime.Object);
            sut.Log("simple log");

            var file = GetLastTextFile();
            file.Should().EndWith("log20210101.txt");
            var text = GetLastLine(file);

            text.Should().Be($"2021-01-01 08:10:30 simple log");
        }

        [Fact]
        public void WithPMTime_ShouldLog24HourPM()
        {
            var now = new DateTime(2021, 1, 1, 16, 10, 30);
            var currentTime = new Mock<ICurrentTime>();
            currentTime.Setup(x => x.Now).Returns(now);
            var sut = new FileLoggerKata.FileLogger(currentTime.Object);
            sut.Log("simple log");

            var file = GetLastTextFile();
            file.Should().EndWith("log20210101.txt");
            var text = GetLastLine(file);

            text.Should().Be($"2021-01-01 16:10:30 simple log");
        }

        [Fact]
        public void OnSaturday_ShouldLogToWeekendFile()
        {
            var now = new DateTime(2021, 1, 2, 16, 10, 30);
            var currentTime = new Mock<ICurrentTime>();
            currentTime.Setup(x => x.Now).Returns(now);
            var sut = new FileLoggerKata.FileLogger(currentTime.Object);
            sut.Log("simple log");

            var file = GetLastTextFile();
            file.Should().EndWith("weekend.txt");
        }

        

        [Fact]
        public void OnSunday_ShouldLogToWeekendFile()
        {
            var now = new DateTime(2021, 1, 3, 16, 10, 30);
            var currentTime = new Mock<ICurrentTime>();
            currentTime.Setup(x => x.Now).Returns(now);
            var sut = new FileLoggerKata.FileLogger(currentTime.Object);
            sut.Log("simple log");

            var file = GetLastTextFile();
            file.Should().EndWith("weekend.txt");
        }

        [Fact]
        public void OnBothWeekendDays_ShouldRenameSaturdayFileAddSundayFile()
        {
            var saturday = new DateTime(2020, 2, 1, 16, 10, 30);
            var sunday = new DateTime(2020, 2, 2, 16, 10, 30);
            var currentTime = new Mock<ICurrentTime>();
            currentTime.Setup(x => x.Now).Returns(saturday);

            var fileInfo = new Mock<ILogFileInfo>();
            
            var sut = new FileLoggerKata.FileLogger(currentTime.Object, fileInfo.Object);
            sut.Log("saturday1 msg");

            var files = GetTextFiles();
            files.Should().HaveCount(1);
            files.Last().Should().EndWith("weekend.txt");

            currentTime.Setup(x => x.Now).Returns(sunday.AddDays(7));
            fileInfo.Setup(x => x.GetLastModifiedDate(It.IsAny<string>()))
                .Returns(saturday);
            sut.Log("sunday2 msg");

            files = GetTextFiles();
            files.Should().HaveCount(2);
            files[0].Should().EndWith("weekend-20200201.txt");
            files[1].Should().EndWith("weekend.txt");

            currentTime.Setup(x => x.Now).Returns(sunday.AddDays(14));
            fileInfo.Setup(x => x.GetLastModifiedDate(It.IsAny<string>()))
                .Returns(sunday.AddDays(7));
            sut.Log("sunday3 msg");

            files = GetTextFiles();
            files.Should().HaveCount(3);
            files[0].Should().EndWith("weekend-20200201.txt");
            files[1].Should().EndWith("weekend-20200208.txt"); // correspond to saturday date, even though not written on saturday
            files[2].Should().EndWith("weekend.txt");

        }

        private string GetLastTextFile()
        {
            List<string> logFiles = GetTextFiles();
            return logFiles.Last();
        }

        private List<string> GetTextFiles()
        {
            var dir = new DirectoryInfo(GetPath());

            var logFiles = dir.GetFiles().Where(x => x.Extension == ".txt")
                .OrderBy(x => x.LastWriteTime).Select(x => x.FullName)
                .ToList();
            return logFiles;
        }

        private string GetPath()
        {
            return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
        }

        private string GetLastLine(string file)
        {
            try
            {
                var lines = File.ReadAllLines(file);
                return lines[lines.Length - 1];
            }
            catch
            {
                return "";
            }
        }
    }
}