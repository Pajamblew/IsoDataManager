using System;
using System.Collections.Generic;
using System.Linq;

namespace IsoDataManager
{
  internal static class StringExtension
  {
    public static bool ContainsOrdinalIgnoreCase(this string source, string toCheck) => source != null && source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;

    public static bool ContainsOrdinalIgnoreCase(this IEnumerable<string> source, string toCheck) => source != null && source.Any(x => x.EqualsOrdinalIgnoreCase(toCheck));

    public static bool EqualsOrdinalIgnoreCase(this string source, string toCheck) => source != null && source.Equals(toCheck, StringComparison.OrdinalIgnoreCase);
  }
}
