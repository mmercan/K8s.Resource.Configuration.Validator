using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sentinel.Validator.POC.Models;
using Sentinel.Validator.POC.ValidationReaders;

namespace Sentinel.Validator.POC.Repo
{
    public class ValidationRepo
    {
        public IDictionaryRepo<ValidationModel> ValidationModels { get; set; }


        public ValidationRepo(IEnumerable<IValidationReader> readers, ILogger<IDictionaryRepo<ValidationModel>> logger)
        {

            ValidationModels = new DictionaryMemoryRepo<ValidationModel>(logger);
            foreach (var reader in readers)
            {
                reader.Read().ToList().ForEach(p => ValidationModels.Add(p));
            }

            //var q = new JsonValidationReader();
            // ValidationModels = validationModels;
        }
    }
}