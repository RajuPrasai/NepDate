using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NepDate
{
    public readonly partial struct NepaliDate
    {
        /// <summary>
        /// Provides methods for bulk conversion between English (Gregorian) and Nepali (Bikram Sambat) dates.
        /// Includes optimized methods for handling large collections efficiently with parallel processing options.
        /// </summary>
        /// <remarks>
        /// This class offers methods to convert collections of dates between calendar systems.
        /// For large collections, it automatically uses parallel processing to improve performance,
        /// and provides batch processing options to handle very large datasets efficiently.
        /// </remarks>
        public class BulkConvert
        {
            /// <summary>
            /// The minimum number of items in a collection before parallel processing is automatically used.
            /// Collections smaller than this threshold are processed sequentially for better performance.
            /// </summary>
            private const int ParallelThreshold = 500;

            /// <summary>
            /// Converts a collection of English (Gregorian) dates to Nepali dates.
            /// </summary>
            /// <param name="engDates">The collection of English DateTime objects to convert.</param>
            /// <param name="useParallel">
            /// Whether to use parallel processing. If true, collections larger than 
            /// the ParallelThreshold will be processed in parallel. Default is true.
            /// </param>
            /// <returns>
            /// An IEnumerable of NepaliDate objects corresponding to the input English dates.
            /// </returns>
            /// <remarks>
            /// For small collections (fewer than 500 items), sequential processing is used even if useParallel is true.
            /// For large collections, parallel processing can significantly improve performance on multi-core systems.
            /// 
            /// This method materializes the input collection to determine its size for optimizing the processing approach.
            /// </remarks>
            /// <example>
            /// <code>
            /// var englishDates = new List&lt;DateTime&gt; { DateTime.Today, DateTime.Today.AddDays(1) };
            /// var nepaliDates = NepaliDate.BulkConvert.ToNepaliDates(englishDates);
            /// </code>
            /// </example>
            public static IEnumerable<NepaliDate> ToNepaliDates(IEnumerable<DateTime> engDates, bool useParallel = true)
            {
                if (engDates == null) throw new ArgumentNullException(nameof(engDates));

                var datesList = engDates.ToList();
                
                if (useParallel && datesList.Count > ParallelThreshold)
                {
                    return datesList.AsParallel().Select(item => new NepaliDate(item)).ToList();
                }
                
                return datesList.Select(item => new NepaliDate(item));
            }

            /// <summary>
            /// Converts a collection of Nepali dates represented as strings to English (Gregorian) dates.
            /// </summary>
            /// <param name="nepDates">The collection of Nepali date strings to convert.</param>
            /// <param name="useParallel">
            /// Whether to use parallel processing. If true, collections larger than 
            /// the ParallelThreshold will be processed in parallel. Default is true.
            /// </param>
            /// <returns>
            /// An IEnumerable of DateTime objects representing the English dates 
            /// corresponding to the input Nepali dates.
            /// </returns>
            /// <remarks>
            /// The input strings must be in a format that can be parsed by the NepaliDate constructor.
            /// Valid formats include: "YYYY/MM/DD", "YYYY-MM-DD", and other formats with various separators.
            /// 
            /// For large collections, parallel processing is used to improve performance.
            /// This method materializes the input collection to determine its size for optimizing the processing approach.
            /// </remarks>
            /// <example>
            /// <code>
            /// var nepaliDateStrings = new List&lt;string&gt; { "2080/01/15", "2080-02-20" };
            /// var englishDates = NepaliDate.BulkConvert.ToEnglishDates(nepaliDateStrings);
            /// </code>
            /// </example>
            public static IEnumerable<DateTime> ToEnglishDates(IEnumerable<string> nepDates, bool useParallel = true)
            {
                if (nepDates == null) throw new ArgumentNullException(nameof(nepDates));

                var datesList = nepDates.ToList();
                
                if (useParallel && datesList.Count > ParallelThreshold)
                {
                    return datesList.AsParallel().Select(item => Parse(item).EnglishDate).ToList();
                }
                
                return datesList.Select(item => Parse(item).EnglishDate);
            }

            /// <summary>
            /// Converts a collection of NepaliDate objects to English (Gregorian) dates.
            /// </summary>
            /// <param name="nepDates">The collection of NepaliDate objects to convert.</param>
            /// <param name="useParallel">
            /// Whether to use parallel processing. If true, collections larger than 
            /// the ParallelThreshold will be processed in parallel. Default is true.
            /// </param>
            /// <returns>
            /// An IEnumerable of DateTime objects representing the English dates 
            /// corresponding to the input Nepali dates.
            /// </returns>
            /// <remarks>
            /// This method performs conversions from Nepali dates to English dates in bulk.
            /// It's more efficient than converting dates individually when dealing with large collections.
            /// 
            /// For large collections, parallel processing is used to improve performance.
            /// This method materializes the input collection to determine its size for optimizing the processing approach.
            /// </remarks>
            /// <example>
            /// <code>
            /// var nepaliDates = new List&lt;NepaliDate&gt; { NepaliDate.Now, NepaliDate.Now.AddDays(5) };
            /// var englishDates = NepaliDate.BulkConvert.ToEnglishDates(nepaliDates);
            /// </code>
            /// </example>
            public static IEnumerable<DateTime> ToEnglishDates(IEnumerable<NepaliDate> nepDates, bool useParallel = true)
            {
                if (nepDates == null) throw new ArgumentNullException(nameof(nepDates));

                var datesList = nepDates.ToList();
                
                if (useParallel && datesList.Count > ParallelThreshold)
                {
                    return datesList.AsParallel().Select(item => item.EnglishDate).ToList();
                }
                
                return datesList.Select(item => item.EnglishDate);
            }
            
            /// <summary>
            /// Batch processes a large collection of English dates for conversion to Nepali dates,
            /// handling memory more efficiently by processing in smaller batches.
            /// </summary>
            /// <param name="engDates">The collection of English dates to convert.</param>
            /// <param name="batchSize">The size of each batch to process. Default is 1000.</param>
            /// <returns>
            /// An IEnumerable of NepaliDate objects corresponding to the input English dates.
            /// </returns>
            /// <remarks>
            /// This method is designed for very large collections that might cause memory pressure
            /// if processed all at once. It processes the dates in batches of the specified size,
            /// using parallel processing within each batch for optimal performance.
            /// 
            /// Unlike the ToNepaliDates method, this method does not materialize the entire
            /// input collection at once, making it suitable for streaming scenarios or very large datasets.
            /// </remarks>
            /// <example>
            /// <code>
            /// // Generate a large number of dates
            /// var largeDateCollection = Enumerable.Range(0, 100000)
            ///     .Select(i => DateTime.Today.AddDays(i));
            /// 
            /// // Process in batches of 2000
            /// var nepaliDates = NepaliDate.BulkConvert.BatchProcessToNepaliDates(largeDateCollection, 2000);
            /// </code>
            /// </example>
            public static IEnumerable<NepaliDate> BatchProcessToNepaliDates(IEnumerable<DateTime> engDates, int batchSize = 1000)
            {
                if (engDates == null) throw new ArgumentNullException(nameof(engDates));
                if (batchSize < 1) throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be at least 1.");

                var result = new List<NepaliDate>();
                var batch = new List<DateTime>(batchSize);
                
                foreach (var date in engDates)
                {
                    batch.Add(date);
                    
                    if (batch.Count >= batchSize)
                    {
                        result.AddRange(ToNepaliDates(batch, true));
                        batch.Clear();
                    }
                }
                
                if (batch.Count > 0)
                {
                    result.AddRange(ToNepaliDates(batch, true));
                }
                
                return result;
            }
            
            /// <summary>
            /// Batch processes a large collection of Nepali dates for conversion to English dates,
            /// handling memory more efficiently by processing in smaller batches.
            /// </summary>
            /// <param name="nepDates">The collection of Nepali dates to convert.</param>
            /// <param name="batchSize">The size of each batch to process. Default is 1000.</param>
            /// <returns>
            /// An IEnumerable of DateTime objects corresponding to the input Nepali dates.
            /// </returns>
            /// <remarks>
            /// This method is designed for very large collections that might cause memory pressure
            /// if processed all at once. It processes the dates in batches of the specified size,
            /// using parallel processing within each batch for optimal performance.
            /// 
            /// Unlike the ToEnglishDates method, this method does not materialize the entire
            /// input collection at once, making it suitable for streaming scenarios or very large datasets.
            /// 
            /// It's particularly useful for applications that need to convert large historical datasets
            /// or generate reports spanning long time periods.
            /// </remarks>
            /// <example>
            /// <code>
            /// // Generate a large number of Nepali dates
            /// var largeNepaliDateCollection = Enumerable.Range(0, 50000)
            ///     .Select(i => NepaliDate.Now.AddDays(i));
            /// 
            /// // Process in batches of 1500
            /// var englishDates = NepaliDate.BulkConvert.BatchProcessToEnglishDates(largeNepaliDateCollection, 1500);
            /// </code>
            /// </example>
            public static IEnumerable<DateTime> BatchProcessToEnglishDates(IEnumerable<NepaliDate> nepDates, int batchSize = 1000)
            {
                if (nepDates == null) throw new ArgumentNullException(nameof(nepDates));
                if (batchSize < 1) throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be at least 1.");

                var result = new List<DateTime>();
                var batch = new List<NepaliDate>(batchSize);
                
                foreach (var date in nepDates)
                {
                    batch.Add(date);
                    
                    if (batch.Count >= batchSize)
                    {
                        result.AddRange(ToEnglishDates(batch, true));
                        batch.Clear();
                    }
                }
                
                if (batch.Count > 0)
                {
                    result.AddRange(ToEnglishDates(batch, true));
                }
                
                return result;
            }
        }
    }
}
