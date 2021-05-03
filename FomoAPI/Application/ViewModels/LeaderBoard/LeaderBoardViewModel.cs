using Newtonsoft.Json;
using System;

namespace FomoAPI.Application.ViewModels.LeaderBoard
{
    public class LeaderBoardViewModel
    {
        public Board MostBullish { get; private set; }

        public Board MostBearish { get; private set; }

        public Board BestPerformers { get; private set; }

        public Board WorstPerformers { get; private set; }

        [JsonConstructor]
        public LeaderBoardViewModel(Board mostBullish, Board mostBearish, Board bestPerformers, Board worstPerformers)
        {
            MostBullish = mostBullish;
            MostBearish = mostBearish;
            BestPerformers = bestPerformers;
            WorstPerformers = worstPerformers;
        }
    }
}
