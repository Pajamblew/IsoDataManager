// Decompiled with JetBrains decompiler
// Type: IsoDataManager.ReadCommand
// Assembly: IsoDataManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C7DAEF67-3FCE-4F74-9EA7-9C78771F8F42
// Assembly location: C:\Users\E1219903\Program\AutoCad\Use\Pentair\IsoDataManager\IsoDataManager.dll

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using IsoDataManager.Commands;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace IsoDataManager
{
  public class ReadCommand
  {
        [CommandMethod("VTReadFromIsoRN", CommandFlags.Session)]
        public static void Read()
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      DocumentCollection documentManager = Application.DocumentManager;
      List<Drawing> source = new List<Drawing>();
      string directoryName = Path.GetDirectoryName(documentManager.MdiActiveDocument.Name);
      Logger logger = new Logger(directoryName);
      List<string> list = Directory.EnumerateFiles(directoryName, "*.dwg").ToList<string>();
      DocumentExtension.CloseAndDiscard(documentManager.MdiActiveDocument);
      bool flag1 = false;
      foreach (string path in list)
      {
        using (Database db = new Database(false, false))
        {
          try
          {
            db.ReadDwgFile(path, FileShare.Read, true, string.Empty);
            Dictionary<string, List<Block>> dictionary = ReadService.ReadBlocks(db, (IEnumerable<string>) Settings.Names, logger);
            if (dictionary.Any<KeyValuePair<string, List<Block>>>())
              source.Add(new Drawing(Path.GetFileName(path), dictionary));
          }
          catch (System.Exception ex)
          {
            logger.Error("Error during reading Drawing: " + Path.GetFileName(path) + Environment.NewLine + ((System.Exception) ex).Message);
            flag1 = true;
          }
        }
      }
      bool flag2 = ExcelWriter.WriteData(directoryName, (IEnumerable<Drawing>) source, logger) | flag1;
      stopwatch.Stop();
      DocumentCollectionExtension.Open(documentManager, directoryName + Path.DirectorySeparatorChar.ToString() + source.First<Drawing>().Path);
      if (flag2)
        Application.ShowAlertDialog("Errors occured. Check log file in the directory: " + AppDomain.CurrentDomain.BaseDirectory);
      else
        Application.ShowAlertDialog("No errors");
      logger.Info(string.Format("Read Command Performance: processed {0} drawings in {1:0.###}s", (object) source.Count, (object) ((double) stopwatch.ElapsedMilliseconds / 1000.0)));
    }
  }
}
