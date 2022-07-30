// Decompiled with JetBrains decompiler
// Type: IsoDataManager.ExtensionMethods
// Assembly: IsoDataManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C7DAEF67-3FCE-4F74-9EA7-9C78771F8F42
// Assembly location: C:\Users\E1219903\Program\AutoCad\Use\Pentair\IsoDataManager\IsoDataManager.dll

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;

namespace IsoDataManager
{
  public static class ExtensionMethods
  {
    private static readonly RXClass attDefClass = RXObject.GetClass(typeof (AttributeDefinition));

    public static void SynchronizeAttributes(this BlockTableRecord target, Transaction tr)
    {
      if (target == null || tr == null)
        return;
      List<AttributeDefinition> attributes1 = target.GetAttributes(tr);
      foreach (ObjectId blockReferenceId in target.GetBlockReferenceIds(true, false))
        ((BlockReference) tr.GetObject(blockReferenceId, (OpenMode) 1)).ResetAttributes(attributes1, tr);
      if (!target.IsDynamicBlock)
        return;
      target.UpdateAnonymousBlocks();
      foreach (ObjectId anonymousBlockId in target.GetAnonymousBlockIds())
      {
        BlockTableRecord target1 = (BlockTableRecord) tr.GetObject(anonymousBlockId, (OpenMode) 0);
        List<AttributeDefinition> attributes2 = target1.GetAttributes(tr);
        foreach (ObjectId blockReferenceId in target1.GetBlockReferenceIds(true, false))
          ((BlockReference) tr.GetObject(blockReferenceId, (OpenMode) 1)).ResetAttributes(attributes2, tr);
      }
    }

    private static List<AttributeDefinition> GetAttributes(
      this BlockTableRecord target,
      Transaction tr)
    {
      List<AttributeDefinition> attributes = new List<AttributeDefinition>();
      foreach (ObjectId objectId in target)
      {
        if (objectId.ObjectClass == ExtensionMethods.attDefClass)
        {
          AttributeDefinition attributeDefinition = (AttributeDefinition) tr.GetObject(objectId, (OpenMode) 0);
          attributes.Add(attributeDefinition);
        }
      }
      return attributes;
    }

    private static void ResetAttributes(
      this BlockReference br,
      List<AttributeDefinition> attDefs,
      Transaction tr)
    {
      Dictionary<string, (string, AttachmentPoint)> dictionary = new Dictionary<string, (string, AttachmentPoint)>();
      foreach (ObjectId attribute in br.AttributeCollection)
      {
        if (!((ObjectId) attribute).IsErased)
        {
          AttributeReference attributeReference = (AttributeReference) tr.GetObject(attribute, (OpenMode) 1);
          string str = attributeReference.IsMTextAttribute ? attributeReference.MTextAttribute.Contents : ((DBText) attributeReference).TextString;
          dictionary.Add(attributeReference.Tag, (str, ((DBText) attributeReference).Justify));
          ((DBObject) attributeReference).Erase();
        }
      }
      foreach (AttributeDefinition attDef in attDefs)
      {
        AttributeReference attributeReference = new AttributeReference();
        attributeReference.SetAttributeFromBlock(attDef, br.BlockTransform);
        if (attDef.Constant)
          ((DBText) attributeReference).TextString = attDef.IsMTextAttributeDefinition ? attDef.MTextAttributeDefinition.Contents : ((DBText) attDef).TextString;
        else if (dictionary.ContainsKey(attributeReference.Tag))
        {
          ((DBText) attributeReference).TextString = dictionary[attributeReference.Tag].Item1;
          ((DBText) attributeReference).Justify = dictionary[attributeReference.Tag].Item2;
          ((DBText) attributeReference).Height = 2.5;
        }
        br.AttributeCollection.AppendAttribute(attributeReference);
        tr.AddNewlyCreatedDBObject((DBObject) attributeReference, true);
      }
    }
  }
}
