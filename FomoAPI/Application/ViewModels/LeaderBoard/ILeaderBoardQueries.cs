using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ViewModels.LeaderBoard
{
    public interface ILeaderBoardQueries
    {
        Task<LeaderBoardViewModel> GetLeaderBoardData(int top);
    }
}
