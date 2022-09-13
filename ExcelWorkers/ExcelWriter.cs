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
			bool result = false;

			char directorySeparatorChar = Path.DirectorySeparatorChar;

			FileInfo val = new FileInfo(SFileNameService.
							   GetFileName(currentDirectory 
							   + directorySeparatorChar 
							   + Path.GetFileNameWithoutExtension(Settings.FileName), 
							   Path.GetExtension(Settings.FileName)));

			var list = Enumerable
					   .ToList(Enumerable
							  .Select(Enumerable
									 .GroupBy(Enumerable
											 .SelectMany(drawings, (Drawing x) => Enumerable
																				  .Select(x.Blocks, (Func<KeyValuePair<string, List<Block>>, (string, string, List<Block>)>)
																				  ((KeyValuePair<string, List<Block>> y) => (x.Path, y.Key, y.Value))
																				  )
														),
														((string Path, string Key, List<Block> Value) x) => x.Key),
														(IGrouping<string, (string Path, string Key, List<Block> Value)> x) => Enumerable
																															   .ToList((IEnumerable<(string, string, List<Block>)>)x)
									)
							  );

			try
			{
				ExcelPackage val2 = new ExcelPackage();

				try
				{
					foreach (var item in list)
					{
						(string, string, List<Block>) tuple = item[0];
						bool hasAsterisk = tuple.Item2.Contains("*");
						ExcelWorksheet val3 = val2.Workbook.Worksheets.Add(tuple.Item2.Replace("*", ""));

						Dictionary<string, int> headers = GetHeaders(Enumerable
														 .ToList(Enumerable
														 .Distinct(Enumerable
														 .SelectMany(item, ((string Path, string Key, List<Block> Value) x) => Enumerable
														 .SelectMany(x.Value, (Block y) => Enumerable
														 .Select(y.Attributes,
														 (KeyValuePair<string, string> z)
														 => (!hasAsterisk) ? z.Key : z.Key.Substring(0, z.Key.Length - 2)))))));
						
						foreach (var item2 in headers)
						{
							val3.SetValue(1, item2.Value, item2.Key);
						}

						int num = 2;

						foreach (var item3 in item)
						{
							foreach (var item4 in item3.Item3)
							{
								val3.SetValue(num, 1, item3.Item1);
								val3.SetValue(num, 2, item4.Name);

								foreach (var attribute in item4.Attributes)
								{
									try
									{
										string key;

										key = (hasAsterisk ? attribute.Key.Substring(0, attribute.Key.Length - 2) : attribute.Key);

										val3.SetValue(num, headers[key], attribute.Value);

									}

									catch (Exception ex)
									{
										logger.Error("Writing block attribute data to Excel document. Drawing: " + item3.Item1 
													+ ", Block: " + item4.Name + ", attribute " + attribute.Key + "\n" + ex.Message);

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
			var list = new List<string>();
			list.Add("FileName");
			list.Add("BlockName");
			list.AddRange(uniqueAttributeNames);

			var dictionary = new Dictionary<string, int>();
			int num = 1;

			foreach (string item in list)
			{
				dictionary.Add(item, num++);
			}

			return dictionary;
		}
	}
}
