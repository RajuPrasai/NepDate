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
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to a <see cref="NepaliDate"/>.
        /// </summary>
        /// <param name="context">Context information. Can be <see langword="null"/>.</param>
        /// <param name="sourceType">The type to convert from.</param>
        /// <returns>
        /// <see langword="true"/> when <paramref name="sourceType"/> is <see cref="string"/>,
        /// <see cref="int"/>, or <see cref="DateTime"/>; otherwise delegates to the base implementation.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string)
            || sourceType == typeof(int)
            || sourceType == typeof(DateTime)
            || base.CanConvertFrom(context, sourceType);

        /// <summary>
        /// Returns whether this converter can convert a <see cref="NepaliDate"/> to the given destination type.
        /// </summary>
        /// <param name="context">Context information. Can be <see langword="null"/>.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <returns>
        /// <see langword="true"/> when <paramref name="destinationType"/> is <see cref="string"/>,
        /// <see cref="int"/>, or <see cref="DateTime"/>; otherwise delegates to the base implementation.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string)
            || destinationType == typeof(int)
            || destinationType == typeof(DateTime)
            || base.CanConvertTo(context, destinationType);

        /// <summary>
        /// Converts the given value to a <see cref="NepaliDate"/>.
        /// </summary>
        /// <param name="context">Context information. Can be <see langword="null"/>.</param>
        /// <param name="culture">Culture for parsing. Currently ignored.</param>
        /// <param name="value">
        /// The value to convert. Supported types:
        /// <list type="bullet">
        ///   <item><description><see cref="string"/> — parsed as a Nepali date string (<c>"YYYY/MM/DD"</c> or any supported separator); a null or whitespace string returns <c>default(NepaliDate)</c>.</description></item>
        ///   <item><description><see cref="int"/> — treated as a <c>YYYYMMDD</c> integer.</description></item>
        ///   <item><description><see cref="DateTime"/> — converted from Gregorian to Bikram Sambat.</description></item>
        /// </list>
        /// </param>
        /// <returns>A <see cref="NepaliDate"/> equivalent to the converted value.</returns>
        /// <exception cref="InvalidNepaliDateFormatException">Thrown when the string or integer value cannot be parsed.</exception>
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
                    return NepaliDate.Today.EnglishDate == dt.Date
                        ? NepaliDate.Today
                        : new NepaliDate(dt);

                default:
                    return base.ConvertFrom(context, culture, value);
            }
        }

        /// <summary>
        /// Converts a <see cref="NepaliDate"/> to the specified destination type.
        /// </summary>
        /// <param name="context">Context information. Can be <see langword="null"/>.</param>
        /// <param name="culture">Culture for formatting. Currently ignored.</param>
        /// <param name="value">The <see cref="NepaliDate"/> value to convert.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="string"/> — sortable ISO format <c>"YYYY-MM-DD"</c>.</description></item>
        ///   <item><description><see cref="int"/> — integer in <c>YYYYMMDD</c> form.</description></item>
        ///   <item><description><see cref="DateTime"/> — midnight on the equivalent Gregorian date.</description></item>
        /// </list>
        /// </returns>
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

        /// <summary>
        /// Returns whether the given value is a valid input for conversion to <see cref="NepaliDate"/>.
        /// </summary>
        /// <param name="context">Context information. Can be <see langword="null"/>.</param>
        /// <param name="value">
        /// The candidate value. Accepts <see cref="string"/> (parsed with <see cref="NepaliDate.TryParse(string, out NepaliDate)"/>),
        /// <see cref="int"/> (<c>YYYYMMDD</c> form), <see cref="DateTime"/>, and <see cref="NepaliDate"/>.
        /// </param>
        /// <returns><see langword="true"/> when the value can be successfully converted; otherwise <see langword="false"/>.</returns>
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
