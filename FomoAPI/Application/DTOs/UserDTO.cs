using System;

namespace FomoAPI.Application.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public UserDTO(Guid id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
