using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using QuantLib;
using Calendar = QuantLib.Calendar;

namespace Utils
{
    public static class QuantLibUtils_
    {
        public static DateTime GetDateTimeFromQuantLibDate(QuantLib.Date date)
        {
            return new DateTime(date.year(), (int)date.month(), date.dayOfMonth());
        }

        public static QuantLib.Date GetQuantLibDateFromDateTime(DateTime datetime)
        {
            return new QuantLib.Date(datetime.Day, (Month)datetime.Month, datetime.Year);
        }
    }
}
