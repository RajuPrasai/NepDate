using System.ComponentModel;

namespace NepDate.Tests.Abilities;

public class NepaliDateTypeConverterTests
{
    private readonly TypeConverter _converter = TypeDescriptor.GetConverter(typeof(NepaliDate));
    private readonly NepaliDate _date = new(2081, 4, 15);

    [Fact]
    public void TypeDescriptor_ReturnsNepaliDateTypeConverter()
    {
        Assert.IsType<NepDate.TypeConversion.NepaliDateTypeConverter>(_converter);
    }

    [Fact]
    public void CanConvertFrom_String_True()
    {
        Assert.True(_converter.CanConvertFrom(typeof(string)));
    }

    [Fact]
    public void CanConvertFrom_Int_True()
    {
        Assert.True(_converter.CanConvertFrom(typeof(int)));
    }

    [Fact]
    public void CanConvertFrom_DateTime_True()
    {
        Assert.True(_converter.CanConvertFrom(typeof(DateTime)));
    }

    [Fact]
    public void CanConvertTo_String_True()
    {
        Assert.True(_converter.CanConvertTo(typeof(string)));
    }

    [Fact]
    public void CanConvertTo_Int_True()
    {
        Assert.True(_converter.CanConvertTo(typeof(int)));
    }

    [Fact]
    public void CanConvertTo_DateTime_True()
    {
        Assert.True(_converter.CanConvertTo(typeof(DateTime)));
    }

    [Fact]
    public void ConvertFrom_String_IsoFormat_ReturnsCorrectDate()
    {
        var result = (NepaliDate)_converter.ConvertFrom("2081-04-15")!;
        Assert.Equal(_date, result);
    }

    [Fact]
    public void ConvertFrom_String_SlashFormat_ReturnsCorrectDate()
    {
        var result = (NepaliDate)_converter.ConvertFrom("2081/04/15")!;
        Assert.Equal(_date, result);
    }

    [Fact]
    public void ConvertFrom_Int_ReturnsCorrectDate()
    {
        var result = (NepaliDate)_converter.ConvertFrom(20810415)!;
        Assert.Equal(_date, result);
    }

    [Fact]
    public void ConvertFrom_EmptyString_ReturnsDefault()
    {
        var result = (NepaliDate)_converter.ConvertFrom(string.Empty)!;
        Assert.Equal(default, result);
    }

    [Fact]
    public void ConvertTo_String_ReturnsIsoFormat()
    {
        var result = _converter.ConvertTo(_date, typeof(string));
        Assert.Equal("2081-04-15", result);
    }

    [Fact]
    public void ConvertTo_Int_ReturnsYyyyMmDdInteger()
    {
        var result = _converter.ConvertTo(_date, typeof(int));
        Assert.Equal(20810415, result);
    }

    [Fact]
    public void ConvertTo_DateTime_ReturnsEnglishDate()
    {
        var result = (DateTime)_converter.ConvertTo(_date, typeof(DateTime))!;
        Assert.Equal(_date.EnglishDate.Date, result.Date);
    }

    [Fact]
    public void IsValid_ValidString_True()
    {
        Assert.True(_converter.IsValid("2081-04-15"));
    }

    [Fact]
    public void IsValid_InvalidString_False()
    {
        Assert.False(_converter.IsValid("not-a-date"));
    }

    [Fact]
    public void RoundTrip_StringThroughConverter_PreservesValue()
    {
        var asString = _converter.ConvertTo(_date, typeof(string)) as string;
        var back = (NepaliDate)_converter.ConvertFrom(asString!)!;
        Assert.Equal(_date, back);
    }

    [Fact]
    public void RoundTrip_IntThroughConverter_PreservesValue()
    {
        var asInt = _converter.ConvertTo(_date, typeof(int));
        var back = (NepaliDate)_converter.ConvertFrom(asInt!)!;
        Assert.Equal(_date, back);
    }
}
