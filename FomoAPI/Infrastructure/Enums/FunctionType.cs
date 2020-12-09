using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Enums
{
    /// <summary>
    /// Enum class for data interval for the query.
    /// Based of AlphaVantage url parameters but should be reusable.
    /// </summary>
    public class QueryFunctionType
    {
        private const string SingleQuoteName = "GLOBAL_QUOTE";
        private const string IntraDayName = "TIME_SERIES_INTRADAY";
        private const string DailyName = "TIME_SERIES_DAILY";
        private const string WeeklyName = "TIME_SERIES_WEEKLY";
        private const string MonthlyName = "TIME_SERIES_MONTHLY";

        /// <summary>
        /// Enum concrete types. Daily, weekly, and monthly renew every day because
        /// the resulting data set will change at the end of the day.
        /// </summary>
        public readonly static QueryFunctionType SingleQuote = new QueryFunctionType(SingleQuoteName, TimeSpan.FromMinutes(5));
        public readonly static QueryFunctionType IntraDay = new QueryFunctionType(IntraDayName, TimeSpan.FromMinutes(5));
        public readonly static QueryFunctionType Daily = new QueryFunctionType(DailyName, TimeSpan.FromMinutes(15));
        public readonly static QueryFunctionType Weekly = new QueryFunctionType(WeeklyName, TimeSpan.FromMinutes(30));
        public readonly static QueryFunctionType Monthly = new QueryFunctionType(MonthlyName, TimeSpan.FromMinutes(30));

        public string Name { get; private set; }

        public TimeSpan RenewalTime { get; private set; }

        private QueryFunctionType(string functionName, TimeSpan renewalTime)
        {
            Name = functionName;
            RenewalTime = renewalTime;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool IsExpired(DateTime oldDateTime, DateTime newDateTime)
        {
            return (newDateTime - oldDateTime) > RenewalTime;
        }
    }
}
