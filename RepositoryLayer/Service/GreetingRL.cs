using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using StackExchange.Redis;

namespace RepositoryLayer.Service
{
    public class GreetingRL:IGreetingRL
    {
        private readonly GreetingDBContext _context;
        private readonly IDatabase _cache;

        public GreetingRL(GreetingDBContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _cache = redis.GetDatabase();
        }

        

        public bool DeleteGreeting(int id)
        {
            var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id);
            if (greeting == null) return false;

            _context.Greetings.Remove(greeting);
            bool result = _context.SaveChanges() > 0;

            if (result) _cache.KeyDelete("AllGreetings"); 
            return result;
        }

        public bool UpdateGreeting(int id, string newValue)
        {
            var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id);
            if (greeting == null) return false;

            greeting.Value = newValue;
            bool result = _context.SaveChanges() > 0;

            if (result) _cache.KeyDelete("AllGreetings");
            return result;
        }

        public List<GreetingDTO> GetAllGreetings()
        {
            string cacheKey = "AllGreetings";
            string cachedGreetings = _cache.StringGet(cacheKey);

            if (!string.IsNullOrEmpty(cachedGreetings))
                return JsonSerializer.Deserialize<List<GreetingDTO>>(cachedGreetings);

            var greetings = _context.Greetings
                .Select(g => new GreetingDTO { Key = g.Key, Value = g.Value })
                .ToList();

            _cache.StringSet(cacheKey, JsonSerializer.Serialize(greetings));
            _cache.KeyExpire(cacheKey, TimeSpan.FromMinutes(10));

            return greetings;
        }

        public GreetingDTO GetGreetingById(int id)
        {
            string cacheKey = $"Greeting:{id}";
            string cachedGreeting = _cache.StringGet(cacheKey);

            if (!string.IsNullOrEmpty(cachedGreeting))
                return JsonSerializer.Deserialize<GreetingDTO>(cachedGreeting);

            var greeting = _context.Greetings.FirstOrDefault(g => g.Id == id);
            if (greeting == null) return null;

            var greetingDTO = new GreetingDTO { Key = greeting.Key, Value = greeting.Value };

            // Store in Redis
            _cache.StringSet(cacheKey, JsonSerializer.Serialize(greetingDTO));
            _cache.KeyExpire(cacheKey, TimeSpan.FromMinutes(10));  // Cache expires in 10 minutes

            return greetingDTO;
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
            bool result = _context.SaveChanges() > 0;

            if (result) _cache.KeyDelete("AllGreetings");
            return result;

        }

        public string GetGreeting()
        {
            return "Hello World";
        }
    }
}
