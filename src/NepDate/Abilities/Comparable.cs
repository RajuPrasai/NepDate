using System;

namespace NepDate
{
    public readonly partial struct NepaliDate : IComparable<NepaliDate>, IComparable
    {
        public int CompareTo(NepaliDate obj)
        {
            return AsInteger.CompareTo(obj.AsInteger);
        }

        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            if (obj is NepaliDate date) return CompareTo(date);
            throw new ArgumentException($"Object must be of type {nameof(NepaliDate)}.", nameof(obj));
        }
    }
}
