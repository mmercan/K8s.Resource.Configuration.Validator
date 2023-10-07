using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sentinel.Validator.POC.Models;

namespace Sentinel.Validator.POC.ValidationReaders;
public interface IValidationReader
{
    IDictionary<string, ValidationModel> Read();

}
