using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Fixtures
{
    /// <summary>
    /// Fixture to connect to database
    /// </summary>
    public class DBFixture : IAsyncLifetime
    {
        public SqlConnection Connection { get; }

        public DBFixture()
        {
            Connection = new SqlConnection(AppTestSettings.Instance.TestDBConnectionString);
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask; 
        }

        public virtual async Task DisposeAsync()
        {
            await Connection.DisposeAsync();
        }
    }
}
