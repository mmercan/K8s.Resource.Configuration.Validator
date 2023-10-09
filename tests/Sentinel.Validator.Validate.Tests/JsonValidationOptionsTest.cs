using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sentinel.Validator.Validate.ValidationReaders;
using Xunit;

namespace Sentinel.Validator.Validate.Tests
{
    public class JsonValidationOptionsTest
    {
        [Fact]
        public void JsonValidationLocation_ShouldHaveDefaultValue()
        {
            // Assert
            Assert.Equal("JsonValidationLocation", JsonValidationOptions.JsonValidationLocation);
        }
    }
}