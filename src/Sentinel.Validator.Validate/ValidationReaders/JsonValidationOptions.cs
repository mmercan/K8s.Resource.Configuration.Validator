using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sentinel.Validator.Validate.ValidationReaders
{
    public class JsonValidationOptions
    {
        public static string JsonValidationLocation = "JsonValidationLocation";

        public List<string> Locations { get; set; } = new List<string>();
    }
}