﻿using FileLoggerKata.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.IO;
using System.Linq;
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

        private string GetLastTextFile()
        {
            var dir = new DirectoryInfo(GetPath());

            var logFile = dir.GetFiles().Where(x => x.Extension == ".txt")
                .OrderBy(x => x.LastWriteTime).Last();

            return logFile.FullName;
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