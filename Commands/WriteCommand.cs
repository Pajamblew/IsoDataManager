using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using IsoDataManager.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.Runtime;

namespace IsoDataManager
{
  public class WriteCommand
  {
        [CommandMethod("VTWriteToIsoRN", CommandFlags.Session)]

        public static void Read()
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();

      DocumentCollection documentManager = Application.DocumentManager;
      string directoryName = Path.GetDirectoryName(documentManager.MdiActiveDocument.Name);

      Logger logger = new Logger(directoryName);

      if (!File.Exists(directoryName + Path.DirectorySeparatorChar.ToString() + Settings.FileName))
      {
        Application.ShowAlertDialog(Settings.FileName + " is not found in directory: " + directoryName);
        logger.Warn(Settings.FileName + " is not found in directory: " + directoryName);
      }
      else
      {
        bool hasError;
        List<Drawing> source = ExcelReader.ReadData(directoryName, logger, out hasError);

        if (hasError)
        {
          string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
          Application.ShowAlertDialog("Errors during read " + Settings.FileName + Environment.NewLine + "Check log file in the directory: " + baseDirectory);
        }
        else
        {
          DocumentExtension.CloseAndDiscard(documentManager.MdiActiveDocument);

          foreach (Drawing drawing in source)
          {
            string path = directoryName + Path.DirectorySeparatorChar.ToString() + drawing.Path;

            if (File.Exists(path))
            {
              try
              {
                using (Database db = new Database(false, false))
                {
                  db.ReadDwgFile(path, FileShare.ReadWrite, true, string.Empty);
                  hasError = WriteService.WriteBlocks(db, drawing.Blocks.SelectMany((x => x.Value)).ToList(), logger);

                  if (hasError)
                    logger.Error("Error ABOVE occured in Drawing: " + drawing.Path);

                  db.SaveAs(path, (DwgVersion) 33);
                }
              }
              catch (System.Exception ex)
              {
                logger.Error("Error during writing Drawing: " + drawing.Path + Environment.NewLine + ex.Message);
                hasError = true;
              }
            }
          }

          stopwatch.Stop();

          DocumentCollectionExtension.Open(documentManager, directoryName + Path.DirectorySeparatorChar.ToString() + source.First().Path);

          if (hasError)
            Application.ShowAlertDialog("Errors occured. Check log file in the directory: " + AppDomain.CurrentDomain.BaseDirectory);
          else
            Application.ShowAlertDialog("No errors");

          logger.Info(string.Format("Write Command Performance: processed {0} drawings in {1:0.###}s", source.Count, stopwatch.ElapsedMilliseconds / 1000.0));
        }
      }
    }
  }
}
