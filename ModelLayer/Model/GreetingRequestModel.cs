using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Model
{
    public class GreetingRequestModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

    }
}
