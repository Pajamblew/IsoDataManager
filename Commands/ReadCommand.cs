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

      var source = new List<Drawing>();
      bool flag1 = false;
          
      DocumentCollection documentManager = Application.DocumentManager;
      DocumentExtension.CloseAndDiscard(documentManager.MdiActiveDocument);
      
      string directoryName = Path.GetDirectoryName(documentManager.MdiActiveDocument.Name);
      List<string> list = Directory.EnumerateFiles(directoryName, "*.dwg").ToList();
      
      Logger logger = new Logger(directoryName);

      foreach (string path in list)
      {
        using (Database db = new Database(false, false))
        {
          try
          {
            db.ReadDwgFile(path, FileShare.Read, true, string.Empty);

            Dictionary<string, List<Block>> dictionary = ReadService.ReadBlocks(db, (IEnumerable<string>) Settings.BlockNames, logger);

            if (dictionary.Any())
              source.Add(new Drawing(Path.GetFileName(path), dictionary));
          }

          catch (System.Exception ex)
          {
            logger.Error("Error during reading Drawing: " + Path.GetFileName(path) + Environment.NewLine + (ex).Message);
            flag1 = true;
          }
        }
      }

      bool flag2 = ExcelWriter.WriteData(directoryName, source, logger) | flag1;

      stopwatch.Stop();

      DocumentCollectionExtension.Open(documentManager, directoryName + Path.DirectorySeparatorChar.ToString() + source.First<Drawing>().Path);

      if (flag2)
        Application.ShowAlertDialog("Errors occured. Check log file in the directory: " + AppDomain.CurrentDomain.BaseDirectory);
      else
        Application.ShowAlertDialog("No errors");

      logger.Info(string.Format("Read Command Performance: processed {0} drawings in {1:0.###}s", source.Count, stopwatch.ElapsedMilliseconds / 1000.0));
    }
  }
}
