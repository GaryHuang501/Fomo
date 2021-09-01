using System;

namespace FomoAPI.Application.DTOs
{
    /// <summary>
    /// A user other than the current the user.
    /// </summary>
    public class MemberDTO
    {
        /// <summary>
        /// Unique GUID Id of user
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; private set; }

        public MemberDTO(Guid id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
