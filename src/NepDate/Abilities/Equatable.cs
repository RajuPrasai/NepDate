using System;

namespace NepDate
{
    public readonly partial struct NepaliDate : IEquatable<NepaliDate>
    {
        /// <summary>
        /// Determines whether this NepaliDate instance is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if the specified object is a NepaliDate and has the same value as the current instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is NepaliDate date && Equals(date);
        }

        /// <summary>
        /// Determines whether this NepaliDate instance is equal to another NepaliDate.
        /// </summary>
        /// <param name="other">The NepaliDate to compare with the current instance.</param>
        /// <returns>true if the specified NepaliDate has the same value as the current instance; otherwise, false.</returns>
        public bool Equals(NepaliDate other)
        {
            return AsInteger == other.AsInteger;
        }

        /// <summary>
        /// Returns the hash code for this NepaliDate instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code based on the value of this NepaliDate instance.</returns>
        public override int GetHashCode()
        {
            return AsInteger;
        }
    }
}
