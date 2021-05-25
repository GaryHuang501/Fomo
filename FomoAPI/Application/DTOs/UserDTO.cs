using System;

namespace FomoAPI.Application.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public UserDTO(Guid id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
