// Decompiled with JetBrains decompiler
// Type: IsoDataManager.StringExtension
// Assembly: IsoDataManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C7DAEF67-3FCE-4F74-9EA7-9C78771F8F42
// Assembly location: C:\Users\E1219903\Program\AutoCad\Use\Pentair\IsoDataManager\IsoDataManager.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace IsoDataManager
{
  internal static class StringExtension
  {
    public static bool ContainsOrdinalIgnoreCase(this string source, string toCheck) => source != null && source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;

    public static bool ContainsOrdinalIgnoreCase(this IEnumerable<string> source, string toCheck) => source != null && source.Any<string>((Func<string, bool>) (x => x.EqualsOrdinalIgnoreCase(toCheck)));

    public static bool EqualsOrdinalIgnoreCase(this string source, string toCheck) => source != null && source.Equals(toCheck, StringComparison.OrdinalIgnoreCase);
  }
}
