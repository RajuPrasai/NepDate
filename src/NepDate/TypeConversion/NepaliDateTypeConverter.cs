using System;
using System.ComponentModel;
using System.Globalization;

namespace NepDate.TypeConversion
{
    /// <summary>
    /// Provides type conversion for <see cref="NepaliDate"/> to and from <see cref="string"/>,
    /// <see cref="int"/> (YYYYMMDD integer), and <see cref="DateTime"/>.
    /// Registering this converter via <c>[TypeConverter]</c> enables WPF/WinForms data binding,
    /// ASP.NET MVC model binding (pre-Core), property grids, and any ComponentModel-based system.
    /// </summary>
    public sealed class NepaliDateTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string)
            || sourceType == typeof(int)
            || sourceType == typeof(DateTime)
            || base.CanConvertFrom(context, sourceType);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string)
            || destinationType == typeof(int)
            || destinationType == typeof(DateTime)
            || base.CanConvertTo(context, destinationType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value)
            {
                case string s:
                    if (string.IsNullOrWhiteSpace(s)) return default(NepaliDate);
                    return NepaliDate.Parse(s);

                case int intVal:
                    int y = intVal / 10000;
                    int m = (intVal / 100) % 100;
                    int d = intVal % 100;
                    return new NepaliDate(y, m, d);

                case DateTime dt:
                    return NepaliDate.Now.EnglishDate == dt.Date
                        ? NepaliDate.Now
                        : new NepaliDate(dt);

                default:
                    return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is NepaliDate date)
            {
                if (destinationType == typeof(string))
                    return date.ToString("s", null);   // canonical: YYYY-MM-DD

                if (destinationType == typeof(int))
                    return date.Year * 10000 + date.Month * 100 + date.Day;

                if (destinationType == typeof(DateTime))
                    return date.EnglishDate.Date;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            if (value is string s)
                return NepaliDate.TryParse(s, out _);
            if (value is int i)
            {
                int y = i / 10000, m = (i / 100) % 100, d = i % 100;
                return NepaliDate.TryParse($"{y}/{m}/{d}", out _);
            }
            return value is DateTime || value is NepaliDate || base.IsValid(context, value);
        }
    }
}
