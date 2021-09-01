using System;

namespace FomoAPI.Application.DTOs
{
    /// <summary>
    /// A user
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// Unique GUID ID of user
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Custom User Name
        /// </summary>
        public string Name { get; private set; }

        public UserDTO(Guid id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
