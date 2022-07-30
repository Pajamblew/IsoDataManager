// Decompiled with JetBrains decompiler
// Type: IsoDataManager.WriteService
// Assembly: IsoDataManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C7DAEF67-3FCE-4F74-9EA7-9C78771F8F42
// Assembly location: C:\Users\E1219903\Program\AutoCad\Use\Pentair\IsoDataManager\IsoDataManager.dll

using Autodesk.AutoCAD.DatabaseServices;
using IsoDataManager.Commands;
using System;
using System.Collections.Generic;

namespace IsoDataManager
{
  public class WriteService
  {
    public static bool WriteBlocks(Database db, List<Block> blocks, Logger logger)
    {
      bool flag1 = false;
      using (Transaction transaction = db.TransactionManager.StartTransaction())
      {
        BlockTable blockTable = transaction.GetObject(db.BlockTableId, 0) as BlockTable;
                
        foreach (ObjectId objectId in transaction.GetObject( blockTable [BlockTableRecord.ModelSpace], 0) as BlockTableRecord)
        {
            if (objectId.ObjectClass.DxfName.EqualsOrdinalIgnoreCase("Insert") && transaction.GetObject(objectId, 0) is BlockReference blRef)
            {
                string acadBlockName = AcadHelper.GetAcadBlockName(blRef);
                bool flag2 = false;
                Block block1 = null;
                foreach (Block block2 in blocks)
                {
                    if (acadBlockName.EqualsOrdinalIgnoreCase(block2.Name))
                    {
                        flag2 = true;
                        block1 = block2;
                        break;
                    }
                }
                if (flag2)
                {
                    string empty1 = string.Empty;
                    string empty2 = string.Empty;
                    try
                    {
                        foreach (ObjectId attribute in blRef.AttributeCollection)
                        {
                            AttributeReference attributeReference = (AttributeReference)transaction.GetObject(attribute, (OpenMode)1);
                            if (block1.Attributes.TryGetValue(attributeReference.Tag, out empty2))
                            {
                                ((DBObject)attributeReference).UpgradeOpen();
                                ((DBText)attributeReference).TextString = empty2 ?? string.Empty;
                                ((DBText)attributeReference).AdjustAlignment(db);
                                ((DBObject)attributeReference).DowngradeOpen();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error during Setting attribute value. Block: " + acadBlockName + ", Attribute: " + empty1 + ", Value: " + empty2 + Environment.NewLine + ex.Message);
                        flag1 = true;
                    }
                }
            }
        }
        transaction.Commit();
      }
      return flag1;
    }
  }
}
