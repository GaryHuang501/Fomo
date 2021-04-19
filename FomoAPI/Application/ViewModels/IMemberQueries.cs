using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ViewModels
{
    /// <summary>
    /// Query data to display on the member page UI componets
    /// </summary>
    public interface IMemberQueries
    {
        /// <summary>
        /// Gets page of members to display
        /// </summary>
        /// <param name="limit">The limit</param>
        /// <param name="offset">The offset</param>
        /// <returns>The <see cref="MembersViewModel"/></returns>
        Task<MembersViewModel> GetPaginatedMembers(int limit, int offset);
    }
}
