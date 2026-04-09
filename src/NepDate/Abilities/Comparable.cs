using System;

namespace NepDate
{
    public readonly partial struct NepaliDate : IComparable<NepaliDate>
    {
        public int CompareTo(NepaliDate obj)
        {
            return AsInteger.CompareTo(obj.AsInteger);
        }
    }
}
