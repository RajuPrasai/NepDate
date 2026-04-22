using System;

namespace NepDate
{
    public readonly partial struct NepaliDate
    {
        /// <summary>
        /// Returns the elapsed time between <paramref name="d1"/> and <paramref name="d2"/>.
        /// The result is positive when <paramref name="d1"/> is later, negative when earlier.
        /// </summary>
        public static TimeSpan operator -(NepaliDate d1, NepaliDate d2)
        {
            return d1.EnglishDate.Date.Subtract(d2.EnglishDate.Date);
        }

        /// <summary>Returns <see langword="true"/> when <paramref name="d1"/> and <paramref name="d2"/> represent the same Nepali date.</summary>
        public static bool operator ==(NepaliDate d1, NepaliDate d2)
        {
            return d1.AsInteger == d2.AsInteger;
        }

        /// <summary>Returns <see langword="true"/> when <paramref name="d1"/> and <paramref name="d2"/> represent different Nepali dates.</summary>
        public static bool operator !=(NepaliDate d1, NepaliDate d2)
        {
            return d1.AsInteger != d2.AsInteger;
        }

        /// <summary>Returns <see langword="true"/> when <paramref name="t1"/> is earlier than <paramref name="t2"/>.</summary>
        public static bool operator <(NepaliDate t1, NepaliDate t2)
        {
            return t1.AsInteger < t2.AsInteger;
        }

        /// <summary>Returns <see langword="true"/> when <paramref name="t1"/> is earlier than or the same as <paramref name="t2"/>.</summary>
        public static bool operator <=(NepaliDate t1, NepaliDate t2)
        {
            return t1.AsInteger <= t2.AsInteger;
        }

        /// <summary>Returns <see langword="true"/> when <paramref name="t1"/> is later than <paramref name="t2"/>.</summary>
        public static bool operator >(NepaliDate t1, NepaliDate t2)
        {
            return t1.AsInteger > t2.AsInteger;
        }

        /// <summary>Returns <see langword="true"/> when <paramref name="t1"/> is later than or the same as <paramref name="t2"/>.</summary>
        public static bool operator >=(NepaliDate t1, NepaliDate t2)
        {
            return t1.AsInteger >= t2.AsInteger;
        }
    }
}
