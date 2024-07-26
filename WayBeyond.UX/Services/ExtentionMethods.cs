using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public static class ExtentionMethods
    {
        public static double ToDouble(this string text)
        {
            if (text.Contains("$"))
            {
                text = text.Replace("$", "");
            }
            double.TryParse(text, out var result);
            return result;
        }
        
        public static DateTime? ToDateTime(this string text)
        {
            DateTime.TryParse(text, out var result);
            return result;
        }
        public static DateTime? ToDateTimeYMD(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                int.TryParse(text.Substring(0, 4), out int year);
                int.TryParse(text.Substring(4, 2), out int month);
                int.TryParse(text.Substring(6, 2), out int day);
                return new DateTime(year, month, day);
            }
            return null;
           

           
        }
        public static DateTime? ToDateTimeMDY(this string text)
        {
            if(text.Length > 6)
            {
                int.TryParse(text.Substring(0, 2), out var month);
                int.TryParse(text.Substring(2, 2), out var day);
                int.TryParse(text.Substring(4, 4), out var year);
                DateTime.TryParse($"{month}/{day}/{year}", out DateTime result);
                return result;
            }
            return null;
        }

        public static DateTime? ToSingleDateFromMulti(this string text)
        {
            var dates = text.Split(',');
            List<DateTime?> comparableDates = new List<DateTime?>();
            foreach (var date in dates)
            {
                comparableDates.Add(date.ToDateTime());
            }
            return comparableDates.Max();
        }

        public static DateTime? ToMaxDateFromList(this List<string> dates)
        {
            List<DateTime?> myDates = new List<DateTime?>();
            foreach (var date in dates)
            {
                if (!string.IsNullOrEmpty(date))
                {
                    var actualDate = date.Split('-');
                    myDates.Add(actualDate[0].ToDateTime());
                }
               
            }
            return myDates.Max();
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

        public static string? ToLastNameFirst(this string text)
        {
            if (text.Contains(','))
            {
                var data = text.Split(',');
                return data[0];
            } else
            {
                var data = text.Split(' ');
                return data[0];
            }
        }

        public static string? ToFirstMiddleLast(this string text)
        {
            if (text.Contains(','))
            {
                var data = text.Split(',');
                return data[1];
            }
            else
            {
                var data = text.Split(' ');
                return data[1];
            }
        }
        public static PayType? ToPayType(this string text)
        {
            switch (text.ToLower())
            {
                case "medicare":
                    return PayType.MEDICARE;
                default:
                    return PayType.NON_MEDICARE;
            }
        }

        public static string? ToCleanString(this string text)
        {
            var dash = text.Contains('-') ? text.Replace("-", "") : text;
            var lParen = dash.Contains('(') ? dash.Replace("(", "") : dash;
            var rParen = lParen.Contains(')') ? lParen.Replace(")", "") : lParen;

            return rParen.Replace("\t","");
        }
    }
}
