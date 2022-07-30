// Decompiled with JetBrains decompiler
// Type: IsoDataManager.AcadHelper
// Assembly: IsoDataManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C7DAEF67-3FCE-4F74-9EA7-9C78771F8F42
// Assembly location: C:\Users\E1219903\Program\AutoCad\Use\Pentair\IsoDataManager\IsoDataManager.dll

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
    public static void ChangeLayer(
      ObjectId[] children,
      string layerName,
      bool freeze = false,
      byte transparent = 0)
    {
      ObjectId layerId = AcadHelper.CreateFindLayer(layerName, freeze, transparent);
      using (Transaction transaction = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
      {
        Parallel.ForEach<ObjectId>((IEnumerable<ObjectId>) children, (Action<ObjectId>) (x => ((Entity) (transaction.GetObject(x, (OpenMode) 1) as BlockReference)).LayerId = layerId));
        transaction.Commit();
      }
    }

    public static string GetAcadBlockName(BlockReference blRef)
    {
      if (!blRef.IsDynamicBlock)
      {
        ObjectId blockTableRecord1 = blRef.BlockTableRecord;
        return (((ObjectId) blockTableRecord1).GetObject((OpenMode) 0) is BlockTableRecord blockTableRecord2 ? ((SymbolTableRecord) blockTableRecord2).Name : (string) null) ?? "";
      }
      ObjectId blockTableRecord3 = blRef.DynamicBlockTableRecord;
      return (((ObjectId) blockTableRecord3).GetObject((OpenMode) 0) is BlockTableRecord blockTableRecord4 ? ((SymbolTableRecord) blockTableRecord4).Name : (string) null) ?? "";
    }

    public static string GetAttributeValue(ObjectId id, string tag)
    {
      using (Transaction transaction = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
      {
        foreach (ObjectId attribute in (transaction.GetObject(id, (OpenMode) 0) as BlockReference).AttributeCollection)
        {
          AttributeReference attributeReference = (AttributeReference) transaction.GetObject(attribute, (OpenMode) 0);
          if (attributeReference.Tag.EqualsOrdinalIgnoreCase(tag))
            return ((DBText) attributeReference).TextString;
        }
      }
      return string.Empty;
    }

    public static string GetBoxName(ObjectId id) => AcadHelper.GetAttributeValue(id, "NAME");

    public static PromptResult SimplePrompt(
      string message,
      string defaultOption,
      params string[] keys)
    {
      string[] strArray = new string[keys.Length + 1];
      strArray[0] = defaultOption;
      Array.Copy((Array) keys, 0, (Array) strArray, 1, keys.Length);
      if (!((IEnumerable<string>) strArray).Any<string>())
        return (PromptResult) null;
      Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
      PromptKeywordOptions promptKeywordOptions1 = new PromptKeywordOptions("");
      ((PromptOptions) promptKeywordOptions1).Message = "\n" + message + " :";
      PromptKeywordOptions promptKeywordOptions2 = promptKeywordOptions1;
      foreach (string key in keys)
        ((PromptOptions) promptKeywordOptions2).Keywords.Add(key);
      ((PromptOptions) promptKeywordOptions2).Keywords.Default = keys[0];
      promptKeywordOptions2.AllowNone = false;
      return editor.GetKeywords(promptKeywordOptions2);
    }

    internal static void PrintConnectionResult(IEnumerable<double> currents)
    {
      Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
      editor.WriteMessage(string.Format("\nTotal Startup on MB: {0}", (object) currents.Sum<double>((Func<double, double>) (x => x))));
      int num = 1;
      foreach (double current in currents)
        editor.WriteMessage(string.Format("\nPhase {0}: {1:0.##}", (object) num++, (object) current));
    }

    private static ObjectId CreateFindLayer(
      string layerName,
      bool freeze,
      byte transparent)
    {
      Database database = Application.DocumentManager.MdiActiveDocument.Database;
      layerName = string.IsNullOrWhiteSpace(layerName) ? "0" : layerName;
      ObjectId findLayer;
      using (Transaction transaction = database.TransactionManager.StartTransaction())
      {
        LayerTable layerTable = transaction.GetObject(database.LayerTableId, (OpenMode) 0) as LayerTable;
        if (((SymbolTable) layerTable).Has(layerName))
          return ((SymbolTable) layerTable)[layerName];
        LayerTableRecord layerTableRecord1 = new LayerTableRecord();
        ((SymbolTableRecord) layerTableRecord1).Name = layerName;
        layerTableRecord1.Color = Color.FromColorIndex((ColorMethod) 195, (short) 0);
        layerTableRecord1.IsFrozen = freeze;
        layerTableRecord1.Transparency = new Transparency(transparent);
        LayerTableRecord layerTableRecord2 = layerTableRecord1;
        ((DBObject) layerTable).UpgradeOpen();
        findLayer = ((SymbolTable) layerTable).Add((SymbolTableRecord) layerTableRecord2);
        transaction.AddNewlyCreatedDBObject((DBObject) layerTableRecord2, true);
        transaction.Commit();
      }
      return findLayer;
    }
  }
}
