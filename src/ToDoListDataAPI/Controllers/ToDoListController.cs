﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens;
using System.Diagnostics;
using ToDoListDataAPI.Models;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;

namespace ToDoListDataAPI.Controllers
{
    public class ToDoListController : ApiController
    {
        // Uncomment following lines for service principal authentication
        //private static string trustedCallerClientId = ConfigurationManager.AppSettings["todo:TrustedCallerClientId"];
        //private static string trustedCallerServicePrincipalId = ConfigurationManager.AppSettings["todo:TrustedCallerServicePrincipalId"];

        private static Dictionary<int, ToDoItem> mockData = new Dictionary<int, ToDoItem>();

        static ToDoListController()
        {
            mockData.Add(0, new ToDoItem { ID = 0, Owner = "*", Description = "feed the dog" });
            mockData.Add(1, new ToDoItem { ID = 1, Owner = "*", Description = "take the dog on a walk" });
        }

        private static void CheckCallerId()
        {
            // Uncomment following lines for service principal authentication
            //string currentCallerClientId = ClaimsPrincipal.Current.FindFirst("appid").Value;
            //string currentCallerServicePrincipalId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            //if (currentCallerClientId != trustedCallerClientId || currentCallerServicePrincipalId != trustedCallerServicePrincipalId)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized, ReasonPhrase = "The appID or service principal ID is not the expected value." });
            //}
        }

        // GET: api/ToDoItemList
        public IEnumerable<ToDoItem> Get(string owner)
        {
            CheckCallerId();

            System.Diagnostics.Trace.TraceInformation("This is a specific trace message at the method level ToDoListController::Get(string owner)");

            string accountName = "bobjacsilkroad2";
            string keyValue = "7JSPQio+N6pSYGU2JKVcfqNbVMrmRVBSMPY14h36Vvb4IjG8Tvzw1vx1RkzYxOCjhubvFIx5volG/TJjfiGA3A==";
            bool useHttps = true;
            bool exportSecrets = true;

            var storageCredentials = new StorageCredentials(accountName, keyValue);
            var storageAccount = new CloudStorageAccount(storageCredentials, useHttps);
            var connectionString = storageAccount.ToString(exportSecrets);

            var tableClient = storageAccount.CreateCloudTableClient();

            var cloudTable = tableClient.GetTableReference("insightsTest");
            cloudTable.CreateIfNotExists();
            System.Diagnostics.Trace.TraceInformation("Created reference to cloud storage table in ToDoListController::Get(string owner)");

            var testEntity = new TestEntity("This is a test entity with value of " + Guid.NewGuid().ToString(), "bobjac");
            var tableOperation = TableOperation.Insert(testEntity);

            cloudTable.Execute(tableOperation);
            System.Diagnostics.Trace.TraceInformation("Successfully executed insert operation in ToDoListController::Get(string owner)");


            return mockData.Values.Where(m => m.Owner == owner || owner == "*");
        }

        // GET: api/ToDoItemList/5
        public ToDoItem GetById(string owner, int id)
        {
            CheckCallerId();

            return mockData.Values.Where(m => (m.Owner == owner || owner == "*" ) && m.ID == id).First();
        }

        // POST: api/ToDoItemList
        public void Post(ToDoItem todo)
        {
            CheckCallerId();

            todo.ID = mockData.Count > 0 ? mockData.Keys.Max() + 1 : 1;
            mockData.Add(todo.ID, todo);
        }

        public void Put(ToDoItem todo)
        {
            CheckCallerId();

            ToDoItem xtodo = mockData.Values.First(a => (a.Owner == todo.Owner || todo.Owner == "*") && a.ID == todo.ID);
            if (todo != null && xtodo != null)
            {
                xtodo.Description = todo.Description;
            }
        }

        // DELETE: api/ToDoItemList/5
        public void Delete(string owner, int id)
        {
            CheckCallerId();

            ToDoItem todo = mockData.Values.First(a => (a.Owner == owner || owner == "*") && a.ID == id);
            if (todo != null)
            {
                mockData.Remove(todo.ID);
            }
        }
    }
}

