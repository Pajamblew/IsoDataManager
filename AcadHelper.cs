using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsoDataManager
{
  public class AcadHelper
  {
    public static void ChangeLayer(ObjectId[] children,
                                   string layerName,
                                   bool freeze = false,
                                   byte transparent = 0)
    {
      ObjectId layerId = AcadHelper.CreateFindLayer(layerName, freeze, transparent);

      using (Transaction transaction = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
      {
        Parallel
        .ForEach(children, x => ((transaction
                                .GetObject(x, (OpenMode) 1) as BlockReference)).LayerId = layerId);

        transaction.Commit();
      }
    }

    public static string GetAcadBlockName(BlockReference blRef)
    {
      if (!blRef.IsDynamicBlock)
      {

        ObjectId blockTableRecord1 = blRef.BlockTableRecord;

        return ((blockTableRecord1).GetObject(0) 
                            is BlockTableRecord blockTableRecord2 ? (blockTableRecord2).Name : null) ?? "";
      }

      ObjectId blockTableRecord3 = blRef.DynamicBlockTableRecord;

      return ((blockTableRecord3).GetObject(0) 
                          is BlockTableRecord blockTableRecord4 ? (blockTableRecord4).Name : null) ?? "";
    }

    public static string GetAttributeValue(ObjectId id, string tag)
    {
      using (Transaction transaction = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
      {

        foreach (ObjectId attribute in (transaction.GetObject(id, 0) as BlockReference).AttributeCollection)
        {
          AttributeReference attributeReference = (AttributeReference) transaction.GetObject(attribute, 0);

          if (attributeReference.Tag.EqualsOrdinalIgnoreCase(tag))
            return (attributeReference).TextString;
        }
      }

      return string.Empty;
    }

    public static string GetBoxName(ObjectId id) => AcadHelper.GetAttributeValue(id, "NAME");

    public static PromptResult SimplePrompt(string message,
                                            string defaultOption,
                                            params string[] keys)
    {
      string[] strArray = new string[keys.Length + 1];

      strArray[0] = defaultOption;

      Array.Copy(keys, 0, strArray, 1, keys.Length);

      if (!(strArray).Any())
        return null;

      Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

      var promptKeywordOptions1 = new PromptKeywordOptions("");
      promptKeywordOptions1.Message = "\n" + message + " :";

      PromptKeywordOptions promptKeywordOptions2 = promptKeywordOptions1;

      foreach (string key in keys)
        promptKeywordOptions2.Keywords.Add(key);

      promptKeywordOptions2.Keywords.Default = keys[0];
      promptKeywordOptions2.AllowNone = false;

      return editor.GetKeywords(promptKeywordOptions2);
    }

    internal static void PrintConnectionResult(IEnumerable<double> currents)
    {
      Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
      editor.WriteMessage(string.Format("\nTotal Startup on MB: {0}", currents.Sum(x => x)));

      int num = 1;

      foreach (double current in currents)
        editor.WriteMessage(string.Format("\nPhase {0}: {1:0.##}", num++, current));
    }

    private static ObjectId CreateFindLayer(string layerName,
                                            bool freeze,
                                            byte transparent)
    {
      Database database = Application.DocumentManager.MdiActiveDocument.Database;

      layerName = string.IsNullOrWhiteSpace(layerName) ? "0" : layerName;

      ObjectId findLayer;

      using (Transaction transaction = database.TransactionManager.StartTransaction())
      {
        LayerTable layerTable = transaction.GetObject(database.LayerTableId, 0) as LayerTable;

        if ((layerTable).Has(layerName))
          return ((SymbolTable) layerTable)[layerName];

        var layerTableRecord1 = new LayerTableRecord();
        layerTableRecord1.Name = layerName;
        layerTableRecord1.Color = Color.FromColorIndex((ColorMethod) 195, 0);
        layerTableRecord1.IsFrozen = freeze;
        layerTableRecord1.Transparency = new Transparency(transparent);

        LayerTableRecord layerTableRecord2 = layerTableRecord1;

        layerTable.UpgradeOpen();
        findLayer = layerTable.Add(layerTableRecord2);

        transaction.AddNewlyCreatedDBObject(layerTableRecord2, true);
        transaction.Commit();
      }

      return findLayer;
    }
  }
}
