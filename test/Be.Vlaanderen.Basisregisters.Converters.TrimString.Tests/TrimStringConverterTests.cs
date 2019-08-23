namespace Be.Vlaanderen.Basisregisters.Converters.TrimString.Tests
{
    using System;
    using System.IO;
    using FluentAssertions;
    using Newtonsoft.Json;
    using Xunit;

    public class TrimStringConverterTests
    {
        private readonly TrimStringConverter _trimStringConverter = new TrimStringConverter();

        [Fact]
        public void CanRead()
        {
            _trimStringConverter.CanRead.Should().BeTrue();
        }

        [Fact]
        public void CanConvertString()
        {
            var text = "test";

            _trimStringConverter.CanConvert(text.GetType()).Should().BeTrue();
        }

        [Fact]
        public void ReadsJson()
        {
            var jsonObject = "{ name:\" foo bar \"}";
            var jsonTextReader = new JsonTextReader(new StringReader(jsonObject));

            jsonTextReader.Read(); //start object
            jsonTextReader.Read(); //name
            jsonTextReader.Read(); //foo bar

            _trimStringConverter.ReadJson(jsonTextReader, jsonObject.GetType(), jsonObject, JsonSerializer.CreateDefault())
                .Should().Be("foo bar");
        }

        [Theory]
        [InlineData("    ", "")]
        [InlineData("a", "a")]
        [InlineData("a ", "a")]
        [InlineData(" a ", "a")]
        [InlineData("foo bar", "foo bar")]
        [InlineData("   foo bar   ", "foo bar")]
        public void TrimsInputAsExpected(string input, string expected)
        {
            _trimStringConverter.TrimInputField(input).Should().Be(expected);
        }

        [Fact]
        public void CannotConvertOtherTypes()
        {
            _trimStringConverter.CanConvert(1.GetType()).Should().BeFalse();
            _trimStringConverter.CanConvert(1L.GetType()).Should().BeFalse();
            _trimStringConverter.CanConvert(1.0.GetType()).Should().BeFalse();
            _trimStringConverter.CanConvert(DateTime.Now.GetType()).Should().BeFalse();
            _trimStringConverter.CanConvert('a'.GetType()).Should().BeFalse();
        }

        [Fact]
        public void CannotWrite()
        {
            _trimStringConverter.CanWrite.Should().BeFalse();
            Assert.Throws<NotImplementedException>(() =>
                _trimStringConverter.WriteJson(new JsonTextWriter(new StringWriter()), "abc", JsonSerializer.CreateDefault()));
        }
    }
}
