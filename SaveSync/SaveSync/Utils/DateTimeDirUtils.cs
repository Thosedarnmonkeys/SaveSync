﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveSync.Utils
{
  public static class DateTimeDirUtils
  {
    public static string DateTimeDirFormat = "yyyy-MM-dd HH;mm;ss";

    public static DateTime GetDirDateTime(string dirname)
    {
      if (string.IsNullOrWhiteSpace(dirname))
        return DateTime.MinValue;

      DateTime dirTime;
      if (DateTime.TryParseExact(dirname, DateTimeDirFormat, null, DateTimeStyles.AllowWhiteSpaces, out dirTime))
        return dirTime;

      return DateTime.MinValue;
    }

		public static string GetDirDateTimeString(DateTime currentTime)
		{
			return currentTime.ToString(DateTimeDirFormat);
		}
  }
}
