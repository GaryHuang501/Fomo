using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Commands.User
{
    /// <summary>
    /// Command to register a new user
    /// </summary>
    public class NewUserCommand
    {
        public string Name { get; set; }
    }
}
