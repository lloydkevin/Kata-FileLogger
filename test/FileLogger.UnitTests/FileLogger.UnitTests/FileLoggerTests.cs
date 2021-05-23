using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileLogger.UnitTests
{
    public class FileLoggerTests
    {
        [Fact]
        public void LogSimple()
        {
            var sut = new FileLoggerKata.FileLogger();

            sut.Log("test");
        }
    }
}
