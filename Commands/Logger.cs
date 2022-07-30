// Decompiled with JetBrains decompiler
// Type: IsoDataManager.Commands.Logger
// Assembly: IsoDataManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C7DAEF67-3FCE-4F74-9EA7-9C78771F8F42
// Assembly location: C:\Users\E1219903\Program\AutoCad\Use\Pentair\IsoDataManager\IsoDataManager.dll

using System;
using System.IO;

namespace IsoDataManager.Commands
{
  public class Logger
  {
    private readonly string _logFileName = "log.txt";

    public Logger(string currentDirectory) => this._logFileName = currentDirectory + Path.DirectorySeparatorChar.ToString() + this._logFileName;

    public void Error(string s)
    {
      using (StreamWriter streamWriter = new StreamWriter(this._logFileName, true))
      {
        DateTime now = DateTime.Now;
        streamWriter.WriteLine(string.Format("{0} ERROR: {1}", (object) now, (object) s));
      }
    }

    public void Info(string s)
    {
      using (StreamWriter streamWriter = new StreamWriter(this._logFileName, true))
      {
        DateTime now = DateTime.Now;
        streamWriter.WriteLine(string.Format("{0} INFO: {1}", (object) now, (object) s));
      }
    }

    public void Warn(string s)
    {
      using (StreamWriter streamWriter = new StreamWriter(this._logFileName, true))
      {
        DateTime now = DateTime.Now;
        streamWriter.WriteLine(string.Format("{0} WARN: {1}", (object) now, (object) s));
      }
    }
  }
}
