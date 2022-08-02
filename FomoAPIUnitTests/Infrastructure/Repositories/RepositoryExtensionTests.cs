using FomoAPI.Controllers;
using FomoAPI.Infrastructure.Repositories;
using MoreLinq;
using System;
using System.Collections.Generic;
using Xunit;

namespace FomoAPIUnitTests.Infrastructure.Repositories
{
    public class RepositoryExtensionTests
    {
        private class TestObject
        {
            public int Integer1 { get; set; }

            public string String1 { get; set; }

            public DateTime DateTime1 { get; set; }
        }

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

            foreach (var kvp in dict)
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

        [Fact]
        public void ToDataTable_ShouldConvertEnumerableToDataTableWhenEmpty()
        {
            var testObjects = new List<TestObject>();

            var dataTable = testObjects.ToDataTable(
                new ColumnSchema<TestObject>("DateTime1", typeof(DateTime), (o => o.DateTime1)),
                new ColumnSchema<TestObject>("Integer1", typeof(int), (o => o.Integer1))
             );

            Assert.Equal(2, dataTable.Columns.Count);
            Assert.Empty(dataTable.Rows);

            Assert.Equal("DateTime1", dataTable.Columns[0].ColumnName);
            Assert.Equal("Integer1", dataTable.Columns[1].ColumnName);

            Assert.Equal(typeof(DateTime), dataTable.Columns[0].DataType);
            Assert.Equal(typeof(int), dataTable.Columns[1].DataType);

        }

        [Fact]
        public void ToDataTable_ShouldConvertEnumerableToDataTableForColumns()
        {
            var testObjects = new List<TestObject> {
                new TestObject{ DateTime1 = DateTime.Now, Integer1 = 100, String1 = "ABC"},
                new TestObject{ DateTime1 = DateTime.Now.AddDays(1), Integer1 = 200, String1 = "DEF"},
                new TestObject{ DateTime1 = DateTime.Now.AddDays(2), Integer1 = 300, String1 = "GHI" }
            };

            var dataTable = testObjects.ToDataTable(
                new ColumnSchema<TestObject>("DateTime1", typeof(DateTime), (o => o.DateTime1)),
                new ColumnSchema<TestObject>("Integer1", typeof(int), (o => o.Integer1)),
                new ColumnSchema<TestObject>("String1", typeof(string), (o => o.String1))
             );

            Assert.Equal(3, dataTable.Columns.Count);
            Assert.Equal(3, dataTable.Rows.Count);

            Assert.Equal("DateTime1", dataTable.Columns[0].ColumnName);
            Assert.Equal("Integer1", dataTable.Columns[1].ColumnName);
            Assert.Equal("String1", dataTable.Columns[2].ColumnName);

            Assert.Equal(typeof(DateTime), dataTable.Columns[0].DataType);
            Assert.Equal(typeof(int), dataTable.Columns[1].DataType);
            Assert.Equal(typeof(string), dataTable.Columns[2].DataType);

            for(var i = 0; i < testObjects.Count; i++)
            {
                Assert.Equal(testObjects[i].DateTime1, dataTable.Rows[i][0]);
                Assert.Equal(testObjects[i].Integer1, dataTable.Rows[i][1]);
                Assert.Equal(testObjects[i].String1, dataTable.Rows[i][2]);
            }
        }
    }
}
