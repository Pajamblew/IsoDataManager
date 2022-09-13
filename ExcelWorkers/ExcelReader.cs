using IsoDataManager.Commands;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace IsoDataManager
{
  public class ExcelReader
  {
        
      public static List<Drawing> ReadData(string currentDirectory,
                                           Logger logger,
                                           out bool hasError)
      {
        var source = new Dictionary<string, Dictionary<string, List<Block>>>();

        var newFile = new FileInfo(currentDirectory + Path.DirectorySeparatorChar.ToString() + Settings.FileName);

        hasError = false;

        using (ExcelPackage excelPackage = new ExcelPackage(newFile))
        {
            foreach (ExcelWorksheet worksheet in excelPackage.Workbook.Worksheets)
            {
                try
                {
                    string blockName = GetBlockName(worksheet);

                    if (blockName != null)
                    {  
                        ExcelRange cells = worksheet.Cells;
                        Dictionary<string, int> headers = GetHeaders(worksheet);

                        if (headers.Count >= 1)
                        {            
                            for (int Row = 2; Row <= worksheet.Dimension.Rows; ++Row)
                            {
                                string key1 = cells[Row, 1].GetValue<string>();
                                string Name = cells[Row, 2].GetValue<string>();
                                string str = blockName.Contains("*") ? Name.Substring(Name.Length - 2) : "";

                                var Attributes = new Dictionary<string, string>();

                                string key2 = string.Empty;
                                string empty = string.Empty;

                                try
                                {
                                    foreach (var keyValuePair in headers)
                                    {
                                        if (keyValuePair.Value >= 3)
                                        {
                                            key2 = keyValuePair.Key + str;

                                            for (int i=0; i<24; i++)
                                            {
                                                if (ReadService.refatt[i] == key2)
                                                {
                                                    key2 = key2.Remove(key2.Length - 2, 2);
                                                    key2 = key2.Insert(key2.Length - 4, str);
                                                }
                                            }

                                            empty = cells[Row, keyValuePair.Value].GetValue<string>(); 
                                            Attributes.Add(key2, empty);
                                        }                 
                                    }

                                    Block block = new Block(Name, Attributes);

                                    Dictionary<string, List<Block>> dictionary;

                                    if (!source.TryGetValue(key1, out dictionary))
                                    {
                                        source.Add(key1, new Dictionary<string, List<Block>>()
                                        {
                                            {
                                                blockName,
                                                new List<Block>() { block }
                                            }
                                        });
                                    }

                                    else
                                    {
                                        List<Block> blockList;

                                        if (!dictionary.TryGetValue(blockName, out blockList))

                                            dictionary.Add(blockName, new List<Block>()
                                            {
                                                block
                                            });

                                        else
                                            blockList.Add(block);
                                    }
                                }

                                catch (System.Exception ex)
                                {
                                    logger.Error("Reading data from Excel file. Block: " + Name 
                                                + ", Attribute: " + key2 + ", Value: " + empty 
                                                + Environment.NewLine + ex.Message);
                                    hasError = true;
                                }
                            }
                        }
                    }
                }

                catch (System.Exception ex)
                {
                    logger.Error("Reading data from Excel file. Worksheet: " + worksheet.Name 
                                + Environment.NewLine + ex.Message);
                    hasError = true;
                }
            }
        }
        return source.Select(
                     x => new Drawing(x.Key, x.Value)).
                     ToList<Drawing>();
      }

    private static string GetBlockName(ExcelWorksheet worksheet) => Settings.BlockNames
                                                                    .FirstOrDefault(x => x
                                                                    .EqualsOrdinalIgnoreCase(worksheet.Name)) ?? Settings.BlockNames
                                                                    .Where(x => x.Contains("*"))
                                                                    .FirstOrDefault(x => x.Replace("*", "")
                                                                    .Equals(worksheet.Name));

    private static Dictionary<string, int> GetHeaders(ExcelWorksheet ws)
    {
      Dictionary<string, int> headers = new Dictionary<string, int>();

      for (int Col = 3; Col <= ws.Dimension.Columns; ++Col)
        headers.Add(ws.Cells[1, Col].GetValue<string>(), Col);

      return headers;
    }
  }
}
