using System.ComponentModel;

namespace NepDate
{
    /// <summary>
    /// Specifies the character used to separate year, month, and day components
    /// when formatting a <see cref="NepaliDate"/> with
    /// <see cref="NepaliDate.ToString(DateFormats, Separators, bool)"/>.
    /// </summary>
    public enum Separators
    {
        /// <summary>Forward slash separator. Example: <c>2081/01/01</c></summary>
        [Description("2081/01/01")]
        ForwardSlash = 0,

        /// <summary>Backward slash separator. Example: <c>2081\01\01</c></summary>
        [Description("2081\\01\\01")]
        BackwardSlash = 1,

        /// <summary>Full stop separator. Example: <c>2081.01.01</c></summary>
        [Description("2081.01.01")]
        Dot = 2,

        /// <summary>Underscore separator. Example: <c>2081_01_01</c></summary>
        [Description("2081_01_01")]
        Underscore = 3,

        /// <summary>Hyphen separator. Example: <c>2081-01-01</c></summary>
        [Description("2081-01-01")]
        Dash = 4,

        /// <summary>Space separator. Example: <c>2081 01 01</c></summary>
        [Description("2081 01 01")]
        Space = 5
    }
}
