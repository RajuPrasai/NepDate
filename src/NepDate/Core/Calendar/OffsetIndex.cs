namespace NepDate.Core.Calendar
{
    /// <summary>
    /// Constant-time membership and position lookup over a sorted list of day offsets.
    /// </summary>
    /// <remarks>
    /// One bit per day says whether the day is in the list. A running count per 64-bit word turns a
    /// set bit into the position of that day in the sorted list (its rank), so no search is needed.
    /// For the 32,509 days of bundled calendar data this costs about 5 KB per list.
    /// </remarks>
    internal sealed class OffsetIndex
    {
        private readonly ulong[] _bits;
        private readonly ushort[] _rankBefore;

        /// <param name="sortedOffsets">The day offsets, strictly increasing.</param>
        /// <param name="totalDays">One more than the largest offset that may be queried.</param>
        internal OffsetIndex(ushort[] sortedOffsets, int totalDays)
        {
            var wordCount = (totalDays + 63) >> 6;
            _bits = new ulong[wordCount];
            _rankBefore = new ushort[wordCount];

            foreach (var offset in sortedOffsets)
            {
                _bits[offset >> 6] |= 1UL << (offset & 63);
            }

            var rank = 0;
            for (int word = 0; word < wordCount; word++)
            {
                _rankBefore[word] = (ushort)rank;
                rank += PopCount(_bits[word]);
            }
        }

        /// <summary>
        /// Returns whether the list contains <paramref name="offset"/>, which must be in
        /// <c>[0, totalDays)</c>.
        /// </summary>
        internal bool Contains(int offset)
        {
            return (_bits[offset >> 6] & (1UL << (offset & 63))) != 0;
        }

        /// <summary>
        /// Returns the position of <paramref name="offset"/> in the sorted list, or -1 when the list
        /// does not contain it. The offset must be in <c>[0, totalDays)</c>.
        /// </summary>
        internal int IndexOf(int offset)
        {
            var word = offset >> 6;
            var bits = _bits[word];
            var bit = 1UL << (offset & 63);

            if ((bits & bit) == 0)
            {
                return -1;
            }

            return _rankBefore[word] + PopCount(bits & (bit - 1));
        }

        private static int PopCount(ulong value)
        {
#if NETCOREAPP3_0_OR_GREATER
            return System.Numerics.BitOperations.PopCount(value);
#else
            value -= (value >> 1) & 0x5555555555555555UL;
            value = (value & 0x3333333333333333UL) + ((value >> 2) & 0x3333333333333333UL);
            value = (value + (value >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
            return (int)((value * 0x0101010101010101UL) >> 56);
#endif
        }
    }
}
