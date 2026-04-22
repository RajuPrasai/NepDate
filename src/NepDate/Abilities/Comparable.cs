using System;

namespace NepDate
{
    public readonly partial struct NepaliDate : IComparable<NepaliDate>, IComparable
    {
        /// <summary>
        /// Compares this <see cref="NepaliDate"/> to another <see cref="NepaliDate"/>,
        /// implementing <see cref="IComparable{T}"/> for sorting and ordering operations.
        /// </summary>
        /// <param name="obj">The other <see cref="NepaliDate"/> to compare against.</param>
        /// <returns>
        /// A negative integer if this instance is earlier than <paramref name="obj"/>,
        /// zero if they represent the same date, or a positive integer if this instance is later.
        /// </returns>
        public int CompareTo(NepaliDate obj)
        {
            return AsInteger.CompareTo(obj.AsInteger);
        }

        /// <summary>
        /// Compares this <see cref="NepaliDate"/> to an untyped object,
        /// implementing the non-generic <see cref="IComparable"/> interface.
        /// </summary>
        /// <param name="obj">
        /// The object to compare. Must be a <see cref="NepaliDate"/> instance or <see langword="null"/>.
        /// </param>
        /// <returns>
        /// A negative integer if this instance is earlier, zero if equal, or a positive integer if later.
        /// A <see langword="null"/> <paramref name="obj"/> is treated as less than any date, returning 1.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="obj"/> is not a <see cref="NepaliDate"/>.
        /// </exception>
        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            if (obj is NepaliDate date) return CompareTo(date);
            throw new ArgumentException($"Object must be of type {nameof(NepaliDate)}.", nameof(obj));
        }
    }
}
