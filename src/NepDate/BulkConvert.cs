using System;
using System.Collections.Generic;

namespace NepDate
{
    public readonly partial struct NepaliDate
    {
        /// <summary>
        /// Converts whole collections between English (Gregorian) and Nepali (Bikram Sambat) dates.
        /// </summary>
        /// <remarks>
        /// Every method converts the input in one sequential pass and returns a filled collection.
        /// One conversion takes a few nanoseconds, so splitting the work across threads costs more
        /// than it saves, whatever the size of the collection. The <c>useParallel</c> and
        /// <c>batchSize</c> parameters remain for source compatibility and do not change the result.
        /// </remarks>
        public class BulkConvert
        {
            /// <summary>
            /// Converts a collection of English (Gregorian) dates to Nepali dates.
            /// </summary>
            /// <param name="engDates">The collection of English DateTime objects to convert.</param>
            /// <param name="useParallel">Kept for source compatibility. The conversion is always sequential.</param>
            /// <returns>
            /// The Nepali dates, in the order of the input.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="engDates"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when a date is outside the supported range.</exception>
            /// <example>
            /// <code>
            /// var englishDates = new List&lt;DateTime&gt; { DateTime.Today, DateTime.Today.AddDays(1) };
            /// var nepaliDates = NepaliDate.BulkConvert.ToNepaliDates(englishDates);
            /// </code>
            /// </example>
            public static IEnumerable<NepaliDate> ToNepaliDates(IEnumerable<DateTime> engDates, bool useParallel = true)
            {
                if (engDates == null) throw new ArgumentNullException(nameof(engDates));

                return ConvertAll(engDates, item => new NepaliDate(item));
            }

            /// <summary>
            /// Converts a collection of Nepali dates represented as strings to English (Gregorian) dates.
            /// </summary>
            /// <param name="nepDates">The collection of Nepali date strings to convert.</param>
            /// <param name="useParallel">Kept for source compatibility. The conversion is always sequential.</param>
            /// <returns>
            /// The English dates, in the order of the input.
            /// </returns>
            /// <remarks>
            /// The input strings must be in a format that can be parsed by the NepaliDate constructor.
            /// Valid formats include: "YYYY/MM/DD", "YYYY-MM-DD", and other formats with various separators.
            /// </remarks>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="nepDates"/> is <see langword="null"/>.</exception>
            /// <example>
            /// <code>
            /// var nepaliDateStrings = new List&lt;string&gt; { "2080/01/15", "2080-02-20" };
            /// var englishDates = NepaliDate.BulkConvert.ToEnglishDates(nepaliDateStrings);
            /// </code>
            /// </example>
            public static IEnumerable<DateTime> ToEnglishDates(IEnumerable<string> nepDates, bool useParallel = true)
            {
                if (nepDates == null) throw new ArgumentNullException(nameof(nepDates));

                return ConvertAll(nepDates, item => Parse(item).EnglishDate);
            }

            /// <summary>
            /// Converts a collection of NepaliDate objects to English (Gregorian) dates.
            /// </summary>
            /// <param name="nepDates">The collection of NepaliDate objects to convert.</param>
            /// <param name="useParallel">Kept for source compatibility. The conversion is always sequential.</param>
            /// <returns>
            /// The English dates, in the order of the input.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="nepDates"/> is <see langword="null"/>.</exception>
            /// <example>
            /// <code>
            /// var nepaliDates = new List&lt;NepaliDate&gt; { NepaliDate.Today, NepaliDate.Today.AddDays(5) };
            /// var englishDates = NepaliDate.BulkConvert.ToEnglishDates(nepaliDates);
            /// </code>
            /// </example>
            public static IEnumerable<DateTime> ToEnglishDates(IEnumerable<NepaliDate> nepDates, bool useParallel = true)
            {
                if (nepDates == null) throw new ArgumentNullException(nameof(nepDates));

                return ConvertAll(nepDates, item => item.EnglishDate);
            }

            /// <summary>
            /// Converts a collection of English dates to Nepali dates.
            /// Equivalent to <see cref="ToNepaliDates(IEnumerable{DateTime}, bool)"/>.
            /// </summary>
            /// <param name="engDates">The collection of English dates to convert.</param>
            /// <param name="batchSize">Kept for source compatibility. Must be at least 1.</param>
            /// <returns>
            /// The Nepali dates, in the order of the input.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="engDates"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="batchSize"/> is less than 1.</exception>
            public static IEnumerable<NepaliDate> BatchProcessToNepaliDates(IEnumerable<DateTime> engDates, int batchSize = 1000)
            {
                if (engDates == null) throw new ArgumentNullException(nameof(engDates));
                if (batchSize < 1) throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be at least 1.");

                return ConvertAll(engDates, item => new NepaliDate(item));
            }

            /// <summary>
            /// Converts a collection of Nepali dates to English dates.
            /// Equivalent to <see cref="ToEnglishDates(IEnumerable{NepaliDate}, bool)"/>.
            /// </summary>
            /// <param name="nepDates">The collection of Nepali dates to convert.</param>
            /// <param name="batchSize">Kept for source compatibility. Must be at least 1.</param>
            /// <returns>
            /// The English dates, in the order of the input.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="nepDates"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="batchSize"/> is less than 1.</exception>
            public static IEnumerable<DateTime> BatchProcessToEnglishDates(IEnumerable<NepaliDate> nepDates, int batchSize = 1000)
            {
                if (nepDates == null) throw new ArgumentNullException(nameof(nepDates));
                if (batchSize < 1) throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be at least 1.");

                return ConvertAll(nepDates, item => item.EnglishDate);
            }

            /// <summary>
            /// Converts every item in one pass. When the source knows its size the result is an
            /// array of exactly that size; otherwise it is a list that grows as needed.
            /// </summary>
            private static IEnumerable<TResult> ConvertAll<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> convert)
            {
                if (source is ICollection<TSource> collection)
                {
                    var converted = new TResult[collection.Count];
                    var index = 0;

                    foreach (var item in collection)
                    {
                        converted[index++] = convert(item);
                    }

                    return converted;
                }

                var result = new List<TResult>();

                foreach (var item in source)
                {
                    result.Add(convert(item));
                }

                return result;
            }
        }
    }
}
