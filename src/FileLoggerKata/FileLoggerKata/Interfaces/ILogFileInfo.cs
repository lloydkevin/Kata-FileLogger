using System;

namespace FileLoggerKata.Interfaces
{
    public interface ILogFileInfo
    {
        public DateTime GetLastModifiedDate(string filePath);
    }
}