using Autodesk.AutoCAD.DatabaseServices;
using IsoDataManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;


namespace IsoDataManager
{
    public class ReadService
    {    
        public static readonly string[] refatt = new string[24];
        public static Dictionary<string, List<Block>> ReadBlocks(Database db,
                                                                IEnumerable<string> blockNames,
                                                                Logger logger)
        {
            var source = new Dictionary<string, List<Block>>();

            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                int count = 0;

                BlockTable blockTable = transaction.GetObject(db.BlockTableId, 0) as BlockTable;

                foreach (ObjectId objectId in transaction.GetObject(((SymbolTable) blockTable)[BlockTableRecord.ModelSpace], 0) as BlockTableRecord)
                {
                    if ((objectId).ObjectClass.DxfName.EqualsOrdinalIgnoreCase("Insert") && transaction.GetObject(objectId, 0) is BlockReference blRef)
                    {
                        string acadBlockName = AcadHelper.GetAcadBlockName(blRef);
                        bool flag = false;
                        string key = string.Empty;

                        foreach (string blockName in blockNames)
                        {
                            if (acadBlockName.EqualsOrdinalIgnoreCase(blockName) || 
                                blockName.EndsWith("*") && acadBlockName.Length > blockName.Length && 
                                CheckAsterisk(blockName, acadBlockName))
                            {
                                flag = true;
                                key = blockName;

                                if (!source.ContainsKey(blockName))
                                {
                                    source.Add(blockName, new List<Block>());
                                    break;
                                }

                                break;
                            }
                        }

                        if (flag)
                        {
                            var dictionary = new Dictionary<string, string>();
                            string str = string.Empty;

                            try
                            {
                                foreach (ObjectId attribute in blRef.AttributeCollection)
                                {
                                    AttributeReference attributeReference = (AttributeReference) transaction.GetObject(attribute, 0);

                                    if (attributeReference.Tag.Contains("_REF") && attributeReference.Tag.Contains("0"))
                                    {
                                        attributeReference.UpgradeOpen();

                                        attributeReference.Tag = attributeReference.Tag.Remove(attributeReference.Tag.Length - 4, 4);
                                        attributeReference.Tag = attributeReference.Tag.Insert(attributeReference.Tag.Length - 2, "_REF");
                                        refatt[count] = attributeReference.Tag;
                                        count++;
                                        
                                        attributeReference.DowngradeOpen();
                                    }

                                    if (attributeReference.TextString == "")
                                        continue;
                                    else
                                        dictionary.Add(attributeReference.Tag, (attributeReference).TextString);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error("Reading blocks error. Block: " + acadBlockName + ", attribute: " + str + Environment.NewLine + ex.Message);
                                throw;
                            }

                            if (dictionary
                                .Any())source[key]
                                .Add(new Block(acadBlockName, dictionary));
                        }
                    }
                }
            }
            return source
                   .Where(x => x.Value.Any())
                   .ToDictionary(x => x.Key, (x => x.Value));
        }
        public static bool CheckAsterisk(string withAsterisk, string blockName)
        {
            int length = withAsterisk.IndexOf("*");

            return withAsterisk.Substring(0, length).EqualsOrdinalIgnoreCase(blockName.Substring(0, length));
        }
    }
}