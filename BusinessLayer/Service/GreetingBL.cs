using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.DTO;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class GreetingBL:IGreetingBL
    {
        private readonly IGreetingRL _greetingRL;

        public GreetingBL(IGreetingRL greetingRL)
        {
            _greetingRL = greetingRL;
        }

        public bool DeleteGreeting(int id)
        {
            return _greetingRL.DeleteGreeting(id);
        }

        public bool UpdateGreeting(int id, string newValue)
        {
            return _greetingRL.UpdateGreeting(id, newValue);
        }

        public List<GreetingDTO> GetAllGreetings(){
		return _greetingRL.GetAllGreetings();
        }

	public GreetingDTO GetGreetingById(int id)
        {
            return _greetingRL.GetGreetingById(id);
        }

        public GreetingDTO GetGreetingById(int id)
        {
            return _greetingRL.GetGreetingById(id);
        }

        public bool AddGreeting(GreetingDTO greetingDTO)
        {
            return _greetingRL.AddGreeting(greetingDTO);
        }

        public string GetGreetingMessage(string firstName, string lastName)
        {
            return GenerateGreeting(firstName, lastName);
        }

        public string GetPersonalizedGreeting(GreetingRequestModel request)
        {
            return GenerateGreeting(request.FirstName, request.LastName);
        }

        private string GenerateGreeting(string firstName, string lastName)
        {
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                return $"Hello, {firstName} {lastName}!";
            }
            else if (!string.IsNullOrWhiteSpace(firstName))
            {
                return $"Hello, {firstName}!";
            }
            else if (!string.IsNullOrWhiteSpace(lastName))
            {
                return $"Hello, {lastName}!";
            }

            return _greetingRL.GetGreeting();
        }


    }
}
