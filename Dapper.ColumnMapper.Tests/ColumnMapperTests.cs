using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Dapper.ColumnMapper.Tests
{
    [TestFixture]
    public class ColumnMapperTests
    {
        private SqlConnection connection;

        public static readonly string connectionString = "Data Source=localhost;Initial Catalog=tempdb;Integrated Security=True";

        public class ColumnMappingObject
        {
            public string DefaultColumn { get; set; }

            public string NonMatchingColumn { get; set; }

            [ColumnMapping("MappedCol")]
            public string MappedColumn { get; set; }

            public string MiscasedColumn { get; set; }
        }

        [SetUp]
        public void Setup()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();

            CreateTestTable();

            SqlMapper.SetTypeMap(typeof(ColumnMappingObject), new ColumnTypeMapper(typeof(ColumnMappingObject)));
        }

        private void CreateTestTable()
        {
            const string createSql = @"
                create table #Test (Id int, DefaultColumn varchar(20), BadCol varchar(20), MappedCol varchar(20), miscasedColumn varchar(20))

                insert #Test values(1, 'DefaultColumn1', 'BadColumn', 'MappedColumn1', 'MiscasedColumn1')";

            connection.Execute(createSql);
        }

        [TearDown]
        public void TearDown()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                const string dropSql = "drop table #Test";
                connection.Execute(dropSql);

                connection.Close();
            }

            connection = null;
        }

        [Test]
        public void Property_Matching_Column_Name_Exactly_Is_Mapped_Correctly()
        {
            var selectedObject = SelectObjects();

            selectedObject.Should().NotBeNull();
            selectedObject.DefaultColumn.Should().NotBeNullOrEmpty();
            selectedObject.DefaultColumn.Should().Be("DefaultColumn1");
        }

        [Test]
        public void Property_Not_Matching_Coumn_Name_Exactly_And_No_Attribute_Is_Not_Mapped()
        {
            var selectedObject = SelectObjects();

            selectedObject.Should().NotBeNull();
            selectedObject.NonMatchingColumn.Should().BeNullOrEmpty();
        }

        [Test]
        public void Property_Using_Column_Mapping_Attribute_Is_Mapped_Correctly()
        {
            var selectedObject = SelectObjects();

            selectedObject.Should().NotBeNull();
            selectedObject.MappedColumn.Should().NotBeNullOrEmpty();
            selectedObject.MappedColumn.Should().Be("MappedColumn1");
        }

        [Test]
        public void Miscased_Property_Is_Mapped_Correctly()
        {
            var selectedObject = SelectObjects();

            selectedObject.Should().NotBeNull();
            selectedObject.MiscasedColumn.Should().NotBeNullOrEmpty();
            selectedObject.MiscasedColumn.Should().Be("MiscasedColumn1");
        }

        private ColumnMappingObject SelectObjects()
        {
            const string selectSql = "select * from #Test";

            return connection.Query<ColumnMappingObject>(selectSql).FirstOrDefault();
        }

        [Test]
        public void Can_Register_Single_Type()
        {
            ColumnTypeMapper.RegisterForTypes(typeof(DateTime));

            var map = SqlMapper.GetTypeMap(typeof(DateTime));

            map.Should().NotBeNull();
            map.Should().BeOfType<ColumnTypeMapper>();
        }

        [Test]
        public void Can_Register_Multiple_Types()
        {
            ColumnTypeMapper.RegisterForTypes(typeof(DateTime), typeof(TimeSpan));

            var dateTimeMap = SqlMapper.GetTypeMap(typeof(DateTime));

            dateTimeMap.Should().NotBeNull();
            dateTimeMap.Should().BeOfType<ColumnTypeMapper>();

            var timeSpanMap = SqlMapper.GetTypeMap(typeof(TimeSpan));

            timeSpanMap.Should().NotBeNull();
            timeSpanMap.Should().BeOfType<ColumnTypeMapper>();
        }
    }
}
