﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.DTO;
using RepositoryLayer.Service;

namespace RepositoryLayer.Interface
{
    public interface IGreetingRL
    {
        string GetGreeting();
        bool AddGreeting(GreetingDTO greetingDTO, int userId);
        GreetingDTO GetGreetingById(int id);

        List<GreetingDTO> GetAllGreetings(int userId);

        bool UpdateGreeting(int id, string newValue);

        bool DeleteGreeting(int id);
    }
}
