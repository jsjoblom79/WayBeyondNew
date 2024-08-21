using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
                text = text.Replace("$", "").Replace("(","").Replace(")","");
            }
            double.TryParse(text, out var result);
            if(result<0) { result = result * -1; }
            return result;
        }
        public static double ToDoubleNoDecimal(this string text)
        {
            double.TryParse(text, out double number);
            return number / 100;
        }
        public static DateTime? ToDateTime(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            } else
            {
                DateTime.TryParse(text, out var result);
                return result;
            }
            
        }
        public static DateTime? ToDateTimeYMD(this string text)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(text))
                if(text.Length>6)
                {
                    int.TryParse(text.Substring(0, 4), out int year);
                    int.TryParse(text.Substring(4, 2), out int month);
                    int.TryParse(text.Substring(6, 2), out int day);
                    return new DateTime(year, month, day);
                } else if (text.Length < 7)
                {
                    int.TryParse(text.Substring(0, 2), out int month);
                    int.TryParse(text.Substring(2, 2), out int day);
                    int.TryParse(text.Substring(4, 2), out int year);

                    var currentyear = DateTime.Now.AddYears(-2000).Year;
                    if(year >= 0 && year <= currentyear)
                    {
                        result = 2000 + year;
                    }
                    else
                    {
                        result = 1900 + year;
                    }
                    return new DateTime(result, month, day);
                }
            return null;
           

           
        }
        public static DateTime? ToDateTimeMDY(this string text)
        {
            if(string.IsNullOrEmpty(text)) 
                return null;

            if(text.Length > 6)
            {
                int.TryParse(text.Substring(0, 2), out var month);
                int.TryParse(text.Substring(2, 2), out var day);
                int.TryParse(text.Substring(4, 4), out var year);
                DateTime.TryParse($"{month}/{day}/{year}", out DateTime result);
                return result;
            }
            else if (text.Length < 7)
            {
                int.TryParse(text.Substring(0, 2), out var month);
                int.TryParse(text.Substring(2, 2), out var day);
                int.TryParse(text.Substring(4, 2), out var year);

                if(year >=0 && year <= DateTime.Now.Year - 2000)
                {
                    year += 2000;
                }
                else
                {
                    year += 1900;
                }
                return new DateTime(year, month, day);
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
                switch (data.Count())
                {
                    case 1:
                    case 2:
                        return data[1];
                    case 3:
                        return $"{data[1]} {data[2]}";
                    default:
                        break;
                }
                return null;
            }
            else
            {
                var data = text.Split(' ');
                switch (data.Count())
                {
                    case 1:
                    case 2:
                        return data[1];
                    case 3:
                        return $"{data[1]} {data[2]}";
                    default:
                        break;
                }
                return null;
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

        public static string? RemoveTabs(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return text.Replace("\t", "");
            }
            return null;
        }
        public static string? ToCleanString(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var dash = text.Contains('-') ? text.Replace("-", "").Trim() : text.Trim();
                var lParen = dash.Contains('(') ? dash.Replace("(", "").Trim() : dash.Trim();
                var rParen = lParen.Contains(')') ? lParen.Replace(")", "").Trim() : lParen.Trim();
                var tabs = rParen.Replace("\t", "").Trim();
                var result = tabs.Replace(" ", "").Trim();
                return result;
            }

            return null;
        }

        #region TexasTech Methods
        public static DateTime? ToDate(this string? date)
        {
            if (date == null || date.Equals(string.Empty))
            {
                return null;
            }
            else
            {
                DateTime.TryParse(date, out DateTime result);
                return result;
            }
        }

        public static string? ToFirstName(this string? name)
        {
            string[] names = name.Split(",");
            return names[1];
        }

        public static string? ToLastName(this string? name)
        {
            string[] names = name.Split(",");
            return names[0];
        }

        public static string ToMrn(this string MrnInv)
        {
            string[] ids;
            if (MrnInv.Contains("\\"))
            {
                ids = MrnInv.Split("\\");
            }
            else
            {
                ids = MrnInv.Split("/");
            }
            return ids[0];
        }

        public static string ToInv(this string MrnInv)
        {
            string[] ids;
            if (MrnInv.Contains("\\"))
            {
                ids = MrnInv.Split("\\");
            }
            else
            {
                ids = MrnInv.Split("/");
            }

            return ids[1];
        }
        public static IQueryable<ToPIF> FromSqlRawPif<ToPIF>(this DbContext db, string sql) where ToPIF : class
        {
            var item = db.Set<ToPIF>().FromSqlRaw(sql);
            return item;
        }
        public static IQueryable<ToTransunion> FromSqlRaw<ToTransunion>(this DbContext db, string sql) where ToTransunion : class
        {
            var item = db.Set<ToTransunion>().FromSqlRaw(sql);
            return item;
        }
        #endregion
    }
}
