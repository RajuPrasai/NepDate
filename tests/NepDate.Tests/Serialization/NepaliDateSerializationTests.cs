using NepDate.Serialization;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;
using STJ = System.Text.Json;

namespace NepDate.Tests.Serialization;

public class SerializationTests
{
    private readonly NepaliDate _testDate = new(2080, 4, 15);
    private readonly Person _testPerson;

    public SerializationTests()
    {
        _testPerson = new Person
        {
            Name = "Test User",
            BirthDate = new NepaliDate(2040, 2, 15),
            JoinDate = _testDate
        };
    }

    #region System.Text.Json Tests

    [Fact]
    public void SystemTextJson_String_SerializeDeserialize_SingleDate()
    {
        var options = new STJ.JsonSerializerOptions().ConfigureForNepaliDate();

        string json = STJ.JsonSerializer.Serialize(_testDate, options);
        var deserializedDate = STJ.JsonSerializer.Deserialize<NepaliDate>(json, options);

        Assert.Equal("\"2080-04-15\"", json);
        Assert.Equal(_testDate, deserializedDate);
    }

    [Fact]
    public void SystemTextJson_Object_SerializeDeserialize_SingleDate()
    {
        var options = new STJ.JsonSerializerOptions().ConfigureForNepaliDate(useObjectFormat: true);

        string json = STJ.JsonSerializer.Serialize(_testDate, options);
        var deserializedDate = STJ.JsonSerializer.Deserialize<NepaliDate>(json, options);

        Assert.Contains("\"Year\":2080", json);
        Assert.Contains("\"Month\":4", json);
        Assert.Contains("\"Day\":15", json);
        Assert.Equal(_testDate, deserializedDate);
    }

    [Fact]
    public void SystemTextJson_String_SerializeDeserialize_DateList()
    {
        var options = new STJ.JsonSerializerOptions().ConfigureForNepaliDate();
        var dateList = new List<NepaliDate>
        {
            new(2080, 1, 1),
            _testDate,
            new(2080, 12, 30)
        };

        string json = STJ.JsonSerializer.Serialize(dateList, options);
        var deserializedList = STJ.JsonSerializer.Deserialize<List<NepaliDate>>(json, options);

        Assert.Equal(3, deserializedList!.Count);
        Assert.Equal(dateList[0], deserializedList[0]);
        Assert.Equal(dateList[1], deserializedList[1]);
        Assert.Equal(dateList[2], deserializedList[2]);
    }

    [Fact]
    public void SystemTextJson_String_SerializeDeserialize_ComplexObject()
    {
        var options = new STJ.JsonSerializerOptions
        {
            PropertyNamingPolicy = STJ.JsonNamingPolicy.CamelCase
        }.ConfigureForNepaliDate();

        string json = STJ.JsonSerializer.Serialize(_testPerson, options);
        var deserializedPerson = STJ.JsonSerializer.Deserialize<Person>(json, options);

        Assert.Equal(_testPerson.Name, deserializedPerson!.Name);
        Assert.Equal(_testPerson.BirthDate, deserializedPerson.BirthDate);
        Assert.Equal(_testPerson.JoinDate, deserializedPerson.JoinDate);
    }

    #endregion

    #region Newtonsoft.Json Tests

    [Fact]
    public void NewtonsoftJson_String_SerializeDeserialize_SingleDate()
    {
        var settings = new JsonSerializerSettings().ConfigureForNepaliDate();

        string json = JsonConvert.SerializeObject(_testDate, settings);
        var deserializedDate = JsonConvert.DeserializeObject<NepaliDate>(json, settings);

        Assert.Equal("\"2080-04-15\"", json);
        Assert.Equal(_testDate, deserializedDate);
    }

    [Fact]
    public void NewtonsoftJson_Object_SerializeDeserialize_SingleDate()
    {
        var settings = new JsonSerializerSettings().ConfigureForNepaliDate(useObjectFormat: true);

        string json = JsonConvert.SerializeObject(_testDate, settings);
        var deserializedDate = JsonConvert.DeserializeObject<NepaliDate>(json, settings);

        Assert.Contains("\"Year\":2080", json);
        Assert.Contains("\"Month\":4", json);
        Assert.Contains("\"Day\":15", json);
        Assert.Equal(_testDate, deserializedDate);
    }

    [Fact]
    public void NewtonsoftJson_String_SerializeDeserialize_DateList()
    {
        var settings = new JsonSerializerSettings().ConfigureForNepaliDate();
        var dateList = new List<NepaliDate>
        {
            new(2080, 1, 1),
            _testDate,
            new(2080, 12, 30)
        };

        string json = JsonConvert.SerializeObject(dateList, settings);
        var deserializedList = JsonConvert.DeserializeObject<List<NepaliDate>>(json, settings);

        Assert.Equal(3, deserializedList!.Count);
        Assert.Equal(dateList[0], deserializedList[0]);
        Assert.Equal(dateList[1], deserializedList[1]);
        Assert.Equal(dateList[2], deserializedList[2]);
    }

    [Fact]
    public void NewtonsoftJson_String_SerializeDeserialize_ComplexObject()
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        }.ConfigureForNepaliDate();

        string json = JsonConvert.SerializeObject(_testPerson, settings);
        var deserializedPerson = JsonConvert.DeserializeObject<Person>(json, settings);

        Assert.Equal(_testPerson.Name, deserializedPerson!.Name);
        Assert.Equal(_testPerson.BirthDate, deserializedPerson.BirthDate);
        Assert.Equal(_testPerson.JoinDate, deserializedPerson.JoinDate);
    }

    #endregion

    #region XML Serialization Tests

    [Fact]
    public void Xml_SerializeDeserialize_WithXmlWrapper()
    {
        var serializer = new XmlSerializer(typeof(PersonXml));
        var personXml = new PersonXml(_testPerson);

        var stringWriter = new StringWriter();
        using (var xmlWriter = XmlWriter.Create(stringWriter))
        {
            serializer.Serialize(xmlWriter, personXml);
        }
        string xml = stringWriter.ToString();

        var stringReader = new StringReader(xml);
        var deserializedPersonXml = (PersonXml)serializer.Deserialize(stringReader)!;
        var deserializedPerson = deserializedPersonXml.ToPerson();

        Assert.Equal(_testPerson.Name, deserializedPerson.Name);
        Assert.Equal(_testPerson.BirthDate, deserializedPerson.BirthDate);
        Assert.Equal(_testPerson.JoinDate, deserializedPerson.JoinDate);
    }

    #endregion

    #region Helper Classes

    public class Person
    {
        public required string Name { get; set; }
        public NepaliDate BirthDate { get; set; }
        public NepaliDate JoinDate { get; set; }
    }

    public class PersonXml
    {
        public PersonXml()
        {
        }

        public PersonXml(Person person)
        {
            Name = person.Name;
            BirthDate = new NepaliDateXmlSerializer(person.BirthDate);
            JoinDate = new NepaliDateXmlSerializer(person.JoinDate);
        }

        public string Name { get; set; } = null!;

        public NepaliDateXmlSerializer BirthDate { get; set; } = null!;

        public NepaliDateXmlSerializer JoinDate { get; set; } = null!;

        public Person ToPerson()
        {
            return new Person
            {
                Name = Name,
                BirthDate = BirthDate.Value,
                JoinDate = JoinDate.Value
            };
        }
    }

    #endregion
}