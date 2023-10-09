using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sentinel.Validator.Validate.ValidationReaders;
using Xunit;

namespace Sentinel.Validator.Validate.Tests
{
    public class JsonValidationReaderTests
    {
        [Fact]
        public void Read_ShouldReturnValidations_WhenJsonValidationLocationsIsNotNull()
        {
            // Arrange
            var options = Options.Create(new JsonValidationOptions
            {
                Locations = new List<string> { "./jsonreaders/validation.json" }
            });
            var reader = new JsonValidationReader(options);

            // Act
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Read_ShouldReturnEmptyDictionary_WhenJsonValidationLocationsIsNull()
        {
            // Arrange
            var options = Options.Create<JsonValidationOptions>(null);
            var reader = new JsonValidationReader(options);

            // Act
            var result = reader.Read();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Read_ShouldReturnValidations_WhenLocationIsValid()
        {
            // Arrange
            var options = Options.Create(new JsonValidationOptions
            {
                Locations = new List<string> { "./jsonreaders/validation.json" }
            });
            var reader = new JsonValidationReader(options);

            // Act
            var result = reader.Read("./jsonreaders/validation.json");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Read_ShouldReturnEmptyDictionary_WhenLocationIsInvalid()
        {
            // Arrange
            var options = Options.Create(new JsonValidationOptions
            {
                Locations = new List<string> { "path/to/invalid/file.json" }
            });
            var reader = new JsonValidationReader(options);

            // Act
            var result = reader.Read("path/to/invalid/file.json");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }



        [Fact]
        public void Read_ShouldReturnValidations_WhenJsonFileContainsValidations()
        {
            // Arrange
            var options = Options.Create(new JsonValidationOptions
            {
                Locations = new List<string> { "./jsonreaders/validation.json" }
            });
            var reader = new JsonValidationReader(options);

            // Act
            var result = reader.Read("./jsonreaders/validation.json");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.ContainsKey("V1Namespace"));
            Assert.True(result.ContainsKey("V1Deployment"));
            Assert.True(result.ContainsKey("V1Pod"));
            Assert.True(result.ContainsKey("V1Service"));
            Assert.True(result.ContainsKey("VirtualService"));
            Assert.True(result.ContainsKey("EnvoyFilter"));
        }

        [Fact]
        public void Read_ShouldReturnEmptyDictionary_WhenJsonFileDoesNotContainValidations()
        {
            // Arrange
            var options = Options.Create(new JsonValidationOptions
            {
                Locations = new List<string> { "./jsonreaders/empty.json" }
            });
            var reader = new JsonValidationReader(options);

            // Act
            var result = reader.Read("./jsonreaders/empty.json");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Read_ShouldReturnEmptyDictionary_WhenJsonFileIsInvalid()
        {
            // Arrange
            var options = Options.Create(new JsonValidationOptions
            {
                Locations = new List<string> { "./jsonreaders/invalid.json" }
            });
            var reader = new JsonValidationReader(options);

            // Act
            var result = reader.Read("./jsonreaders/invalid.json");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}