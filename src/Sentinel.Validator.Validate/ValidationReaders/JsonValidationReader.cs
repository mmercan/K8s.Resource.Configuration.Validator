using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Sentinel.Validator.Validate.Models;

namespace Sentinel.Validator.Validate.ValidationReaders;

public class JsonValidationReader : IValidationReader
{

    // write a test for this class in tests\Sentinel.Validator.Validate.Tests\JsonValidationReaderTests.cs
    // add the test to the project.json in tests\Sentinel.Validator.Validate.Tests\project.json
    // run the test using the dotnet test command from tests\Sentinel.Validator.Validate.Tests\

    JsonValidationOptions _jsonValidationLocations { get; set; }
    public JsonValidationReader(IOptions<JsonValidationOptions> options)
    {
        if (options != null && options.Value != null)
            _jsonValidationLocations = options.Value;
    }

    public IDictionary<string, ValidationModel> Read()
    {

        IDictionary<string, ValidationModel> validations = new Dictionary<string, ValidationModel>();
        if (_jsonValidationLocations == null) return validations;

        _jsonValidationLocations.Locations.ForEach(x =>
        {
            validations = validations.Concat(this.Read(x)).ToDictionary(x => x.Key, x => x.Value);

        });

        return validations;
    }



    public IDictionary<string, ValidationModel> Read(string Location)
    {

        IDictionary<string, ValidationModel> validations = new Dictionary<string, ValidationModel>();

        var Jsons = ExtractJtokenFromFile(Location);

        var headers = Jsons?.Children();
        if (headers != null)
        {
            foreach (var header in headers)
            {
                if (header is JProperty item)
                {
                    var val = item.Value.ToObject<ValidationModel>();
                    val.Name = item.Name;
                    validations.Add(item.Name, val);
                }
            }
        }

        return validations;

    }


    private JToken? ExtractJtokenFromFile(string fileLocation)
    {
        if (!System.IO.File.Exists(fileLocation))
            return JToken.FromObject(new object()); //throw new FileNotFoundException("File not found");
        var fileContent = System.IO.File.ReadAllText(fileLocation);
        var o1 = JObject.Parse(fileContent);
        var valition = o1.SelectToken("$.Validate");
        return valition;
    }
}
