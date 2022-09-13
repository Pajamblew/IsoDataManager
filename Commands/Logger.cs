using System;
using System.IO;

namespace IsoDataManager.Commands
{
  public class Logger
  {
    private readonly string _logFileName = "log.txt";

    public Logger(string currentDirectory) => _logFileName = currentDirectory + Path.DirectorySeparatorChar.ToString() + _logFileName;

    public void Error(string s)
    {
      using (var streamWriter = new StreamWriter(_logFileName, true))
      {
        DateTime now = DateTime.Now;
        streamWriter.WriteLine(string.Format("{0} ERROR: {1}", now, s));
      }
    }

    public void Info(string s)
    {
      using (var streamWriter = new StreamWriter(_logFileName, true))
      {
        DateTime now = DateTime.Now;
        streamWriter.WriteLine(string.Format("{0} INFO: {1}", now, s));
      }
    }

    public void Warn(string s)
    {
      using (var streamWriter = new StreamWriter(_logFileName, true))
      {
        DateTime now = DateTime.Now;
        streamWriter.WriteLine(string.Format("{0} WARN: {1}", now, s));
      }
    }
  }
}
