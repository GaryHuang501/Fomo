using System;

namespace FomoAPI.Application.DTOs
{
    public class MemberDTO
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Email { get; private set; }

        public MemberDTO(Guid id, string name, string email)
        {
            Name = name;
            Id = id;
            Email = email;
        }
    }
}
