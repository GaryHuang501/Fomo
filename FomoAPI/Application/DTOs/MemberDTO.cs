using System;

namespace FomoAPI.Application.DTOs
{
    public class MemberDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public MemberDTO(Guid id, string name, string email)
        {
            Name = name;
            Id = id;
            Email = email;
        }
    }
}
