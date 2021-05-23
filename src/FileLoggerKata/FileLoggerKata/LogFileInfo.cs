using FileLoggerKata.Interfaces;
using System;
using System.IO;

public class LogFileInfo : ILogFileInfo
{
    public DateTime GetLastModifiedDate(string filePath)
    {
        return new FileInfo(filePath).LastWriteTime;
    }
}