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
            double.TryParse(text, out var result);
            return result;
        }
        
        public static DateTime? ToDateTime(this string text)
        {
            DateTime.TryParse(text, out var result);
            return result;
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
            var dash = text.Contains("-") ? text.Replace("-", "") : text;
            var lParen = dash.Contains("(") ? dash.Replace("(", "") : dash;
            var rParen = dash.Contains(")") ? dash.Replace(")", "") : lParen;

            return rParen;
        }
    }
}
