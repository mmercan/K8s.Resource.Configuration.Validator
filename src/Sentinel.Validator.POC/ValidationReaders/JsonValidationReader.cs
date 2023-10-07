using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sentinel.Validator.POC.Models;

namespace Sentinel.Validator.POC.ValidationReaders;

public class JsonValidationReader : IValidationReader
{

    // public IDictionary<string, ValidationModel> Validations { get; set; }


    public IDictionary<string, ValidationModel> Read()
    {
        IDictionary<string, ValidationModel> validations = new Dictionary<string, ValidationModel>();

        var Jsons = ExtractJtokenFromFile("./Validation.json");
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
        // var Jsons = ExtractJtokenFromFile("./Validation.json");
        // var headers = Jsons?.Children();
        // if (headers != null)
        // {
        //     foreach (var header in headers)
        //     {
        //         if (header is JProperty item)
        //         {

        //             Console.WriteLine(item.Name);
        //             // item.Value.ToObject<ValidationModel>();
        //             //var content = header.SelectToken("$." + item.Name);
        //             var val = item.Value.ToObject<ValidationModel>();
        //             val.Name = item.Name;
        //             Console.WriteLine(val.K8sResource.KubeKind);
        //         }
        //     }
        // }

    }

    private JToken? ExtractJtokenFromFile(string fileLocation)
    {
        var fileContent = System.IO.File.ReadAllText(fileLocation);
        var o1 = JObject.Parse(fileContent);
        var valition = o1.SelectToken("$.Validate");
        return valition;
    }
}
