using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCHCWebAPI
{
    public class DateTimeConverter: IsoDateTimeConverter
    {
        public DateTimeConverter(string format)
        {
            base.DateTimeFormat = format;
        }
    }
}
