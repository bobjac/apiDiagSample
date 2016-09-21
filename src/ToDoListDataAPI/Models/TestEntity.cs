using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoListDataAPI.Models
{
    public class TestEntity : TableEntity
    {
        public TestEntity(string description, string owner)
        {
            this.PartitionKey = owner;
            this.RowKey = description;
        }

        public TestEntity() { }

        public string AdditionalInformation { get; set; }
    }
}