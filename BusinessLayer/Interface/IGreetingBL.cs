using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
    public interface IGreetingBL
    {
        //string GetGreetingMessage();

        string GetGreetingMessage(string firstName, string lastName);
        string GetPersonalizedGreeting(GreetingRequestModel request);
    }
}
