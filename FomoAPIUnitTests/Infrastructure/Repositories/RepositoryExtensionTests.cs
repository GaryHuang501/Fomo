using FomoAPI.Infrastructure.Repositories;
using System.Collections.Generic;
using Xunit;

namespace FomoAPIUnitTests.Infrastructure.Repositories
{
    public class RepositoryExtensionTests
    {
        public RepositoryExtensionTests()
        {
        }

        [Fact]
        public void ToDataTable_ShouldFlattenDictionaryKvpToDataTable_WhenEmpty()
        {
            var dict = new Dictionary<int, int>();
            var dataTable = dict.ToDataTable("A", "B");

            Assert.Empty(dataTable.Rows);
            Assert.Equal(2, dataTable.Columns.Count);

            Assert.True(dataTable.Columns.Contains("A"));
            Assert.True(dataTable.Columns.Contains("B"));

            Assert.Equal(typeof(int), dataTable.Columns["A"].DataType); 
            Assert.Equal(typeof(int), dataTable.Columns["B"].DataType);
        }

        [Fact]
        public void ToDataTable_ShouldFlattenDictionaryKvpToDataTable_WhenMany()
        {
            var dict = new Dictionary<int, int>
            {
                {1, 100 },
                {2, 200 },
                {3,300 }
            };

            var dataTable = dict.ToDataTable("A", "B");

            Assert.Equal(3, dataTable.Rows.Count);
            Assert.Equal(2, dataTable.Columns.Count);
            Assert.True(dataTable.Columns.Contains("A"));
            Assert.True(dataTable.Columns.Contains("B"));

            var i = 0;

            foreach(var kvp in dict)
            {
                Assert.Equal(kvp.Key, dataTable.Rows[i]["A"]);
                Assert.Equal(kvp.Value, dataTable.Rows[i]["B"]);
                i++;
            }
        }

        [Fact]
        public void ToDataTable_ShouldFlattenDictionaryKvpToDataTable_WhenManyAndDifferentType()
        {
            var dict = new Dictionary<string, int>
            {
                {"A", 100 },
                {"B", 200 },
                {"C",300 }
            };

            var dataTable = dict.ToDataTable("C1", "C2");

            Assert.Equal(3, dataTable.Rows.Count);
            Assert.Equal(2, dataTable.Columns.Count);
            Assert.True(dataTable.Columns.Contains("C1"));
            Assert.True(dataTable.Columns.Contains("C2"));

            var i = 0;

            foreach (var kvp in dict)
            {
                Assert.Equal(kvp.Key, dataTable.Rows[i]["C1"]);
                Assert.Equal(kvp.Value, dataTable.Rows[i]["C2"]);
                i++;
            }
        }

    }
}
