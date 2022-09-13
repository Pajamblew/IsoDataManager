// Decompiled with JetBrains decompiler
// Type: IsoDataManager.SFileNameService
// Assembly: IsoDataManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C7DAEF67-3FCE-4F74-9EA7-9C78771F8F42
// Assembly location: C:\Users\E1219903\Program\AutoCad\Use\Pentair\IsoDataManager\IsoDataManager.dll

using System.IO;

namespace IsoDataManager
{
  public static class SFileNameService
  {
    public static string GetFileName(string fileNameFull, string extension)
    {
      string withoutExtension = Path.GetFileNameWithoutExtension(fileNameFull);

      string directoryName = Path.GetDirectoryName(fileNameFull);

      string str = string.Empty;

      int num = 1;

      string fileName;

      while (true)
      {
        string[] directoryArray = new string[5]
        {
          directoryName,
          Path.DirectorySeparatorChar.ToString(),
          withoutExtension,
          str,
          extension
        };

        if (File.Exists(fileName = string.Concat(directoryArray)))
          str = string.Format("_({0})", num++);
        else
          break;
      }

      return fileName;
    }
  }
}
