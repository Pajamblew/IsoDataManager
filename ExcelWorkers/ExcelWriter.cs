
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IsoDataManager.Commands;
using OfficeOpenXml;

namespace IsoDataManager
{
	public class ExcelWriter
	{

		public static bool WriteData(string currentDirectory, IEnumerable<Drawing> drawings, Logger logger)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected O, but got Unknown
			bool result = false;
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			FileInfo val = new FileInfo(SFileNameService.GetFileName(currentDirectory + directorySeparatorChar + Path.GetFileNameWithoutExtension(Settings.FileName), Path.GetExtension(Settings.FileName)));
			List<List<(string, string, List<Block>)>> list = Enumerable.ToList<List<(string, string, List<Block>)>>(Enumerable.Select<IGrouping<string, (string, string, List<Block>)>, List<(string, string, List<Block>)>>(Enumerable.GroupBy<(string, string, List<Block>), string>(Enumerable.SelectMany<Drawing, (string, string, List<Block>)>(drawings, (Func<Drawing, IEnumerable<(string, string, List<Block>)>>)((Drawing x) => Enumerable.Select<KeyValuePair<string, List<Block>>, (string, string, List<Block>)>((IEnumerable<KeyValuePair<string, List<Block>>>)x.Blocks, (Func<KeyValuePair<string, List<Block>>, (string, string, List<Block>)>)((KeyValuePair<string, List<Block>> y) => (x.Path, y.Key, y.Value))))), (Func<(string, string, List<Block>), string>)(((string Path, string Key, List<Block> Value) x) => x.Key)), (Func<IGrouping<string, (string, string, List<Block>)>, List<(string, string, List<Block>)>>)((IGrouping<string, (string Path, string Key, List<Block> Value)> x) => Enumerable.ToList<(string, string, List<Block>)>((IEnumerable<(string, string, List<Block>)>)x))));
			try
			{
				ExcelPackage val2 = new ExcelPackage();
				try
				{
					foreach (List<(string, string, List<Block>)> item in list)
					{
						(string, string, List<Block>) tuple = item[0];
						bool hasAsterisk = tuple.Item2.Contains("*");
						ExcelWorksheet val3 = val2.Workbook.Worksheets.Add(tuple.Item2.Replace("*", ""));
						Dictionary<string, int> headers = GetHeaders(Enumerable.ToList<string>(Enumerable.Distinct<string>(Enumerable.SelectMany<(string, string, List<Block>), string>((IEnumerable<(string, string, List<Block>)>)item, (Func<(string, string, List<Block>), IEnumerable<string>>)(((string Path, string Key, List<Block> Value) x) => Enumerable.SelectMany<Block, string>((IEnumerable<Block>)x.Value, (Func<Block, IEnumerable<string>>)((Block y) => Enumerable.Select<KeyValuePair<string, string>, string>((IEnumerable<KeyValuePair<string, string>>)y.Attributes, (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> z) => (!hasAsterisk) ? z.Key : z.Key.Substring(0, z.Key.Length - 2))))))))));
						foreach (KeyValuePair<string, int> item2 in headers)
						{
							val3.SetValue(1, item2.Value, (object)item2.Key);
						}
						int num = 2;
						foreach (var item3 in item)
						{
							foreach (Block item4 in item3.Item3)
							{
								val3.SetValue(num, 1, (object)item3.Item1);
								val3.SetValue(num, 2, (object)item4.Name);
								foreach (KeyValuePair<string, string> attribute in item4.Attributes)
								{
									try
									{
										string key;
										key = (hasAsterisk ? attribute.Key.Substring(0, attribute.Key.Length - 2) : attribute.Key);
										val3.SetValue(num, headers[key], (object)attribute.Value);

									}
									catch (Exception ex)
									{
										logger.Error("Writing block attribute data to Excel document. Drawing: " + item3.Item1 + ", Block: " + item4.Name + ", attribute " + attribute.Key + "\n" + ex.Message);
										result = true;
									}
								}
								num++;
							}
						}
					}
					val2.SaveAs(val);
					return result;
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex2)
			{
				logger.Error("Writing Excel document outer error" + Environment.NewLine + ex2.Message);
				return true;
			}
		}

		private static Dictionary<string, int> GetHeaders(List<string> uniqueAttributeNames)
		{
			List<string> list = new List<string>();
			list.Add("FileName");
			list.Add("BlockName");
			list.AddRange(uniqueAttributeNames);
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int num = 1;
			foreach (string item in list)
			{
				dictionary.Add(item, num++);
			}
			return dictionary;
		}
	}
}
