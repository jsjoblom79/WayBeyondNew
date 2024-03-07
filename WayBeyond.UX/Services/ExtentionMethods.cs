using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.UX.Services
{
    public static class ExtentionMethods
    {
        public static double ToDouble(this string text)
        {
            double.TryParse(text, out var result);
            return result;
        }
        
        public static DateTime? ToDateTime(this string text)
        {
            DateTime.TryParse(text, out var result);
            return result;
        }

        public static long? ToLong(this string text)
        {
            long.TryParse(text, out var result);
            return result;
        }

        public static int? ToInt(this string text)
        {
            int.TryParse(text, out var result);
            return result;
        }

        public static string? ToZip(this string text)
        {
            if(text.Trim().Length > 5)
            {
                return text.Substring(0, 5);
            }
            else
            {
                return text.Trim();
            }
        }
    }
}
