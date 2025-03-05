using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class GreetingRL:IGreetingRL
    {
        private readonly GreetingDBContext _context;

        public GreetingRL(GreetingDBContext context)
        {
            _context = context;
        }

        public bool UpdateGreeting(int id, string newValue)
        {
            var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id);
            if (greeting == null) return false;

            greeting.Value = newValue;
            return _context.SaveChanges() > 0;
        }

        public List<GreetingDTO> GetAllGreetings()
        {
            return _context.Greetings
                .Select(g => new GreetingDTO { Key = g.Key, Value = g.Value })
                .ToList();
        }

        public GreetingDTO GetGreetingById(int id)
        {
            var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id);
            return greeting == null ? null : new GreetingDTO { Key = greeting.Key, Value = greeting.Value };
        }

        public bool AddGreeting(GreetingDTO greetingDTO)
        {
            if (_context.Greetings.Any(g => g.Key == greetingDTO.Key))
                return false;

            var greeting = new GreetingEntity
            {
                Key = greetingDTO.Key,
                Value = greetingDTO.Value
            };

            _context.Greetings.Add(greeting);
            return _context.SaveChanges() > 0;

        }

        public string GetGreeting()
        {
            return "Hello World";
        }
    }
}
