using Entities;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper
{
    public class ValidationHelper
    {
        internal static void ValidationModel (object obj)
        {
            ValidationContext validationContext = new ValidationContext (obj);
            List<ValidationResult> results = new List<ValidationResult>();
            bool isvalid = Validator.TryValidateObject(obj, validationContext,results,true);
            if (!isvalid)
            {
                throw new ArgumentException(results.FirstOrDefault()?.ErrorMessage);
            }
        }
    }
}
