using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.Core.Storage.Extension;

namespace Nat.Core.Storage
{
    public class TableStorage
    {
        public async Task<String> InsertTableStorage(String tableName, TableEntity entity)
        {
            //Azure Connection Creation
            CloudStorageAccount _StorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = _StorageAccount.CreateCloudTableClient();

            //Get Table Access
            CloudTable _Table = tableClient.GetTableReference(tableName);
            await _Table.CreateIfNotExistsAsync();

            // Create the TableOperation that inserts the customer entity. 
            TableOperation insertOperation = TableOperation.Insert(entity);

            // Execute the insert operation. 
            await _Table.ExecuteAsync(insertOperation);

            return "Inserted Successfully";
        }
        public async Task<String> UpdateTableStorage(String tableName, TableEntity entity)
        {
            //Azure Connection Creation
            CloudStorageAccount _StorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = _StorageAccount.CreateCloudTableClient();

            //Get Table Access
            CloudTable _Table = tableClient.GetTableReference(tableName);
            await _Table.CreateIfNotExistsAsync();

            // Create the TableOperation that inserts the customer entity. 
            TableOperation insertOperation = TableOperation.Replace(entity);

            // Execute the insert operation. 
            await _Table.ExecuteAsync(insertOperation);

            return "Replaced Successfully";
        }


        public async Task<TElement> DeleteTableStorage<TElement>(String tableName, String partitionKey, String rowKey) where TElement : ITableEntity, new()
        {
            //Azure Connection Creation
            CloudStorageAccount _StorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = _StorageAccount.CreateCloudTableClient();

            //Get Table Access
            CloudTable _Table = tableClient.GetTableReference(tableName);

            //Retrieve 
            TableOperation retrieveOperation = TableOperation.Retrieve<TElement>(partitionKey, rowKey);

            // Execute the operation.
            TableResult retrievedResult = await _Table.ExecuteAsync(retrieveOperation);
            // Create the TableOperation that deletes the customer entity. 
            var result = (TElement)retrievedResult.Result;
            TableOperation deleteOperation = TableOperation.Delete(result);

            // Execute the delete operation. 
            await _Table.ExecuteAsync(deleteOperation);

            return result;
        }

        public async Task<IEnumerable<TElement>> RetrieveTableStorage<TElement>(String tableName) where TElement : ITableEntity, new()
        {
            return await RetrieveTableStorage<TElement>(tableName, null);
        }

        public async Task<IEnumerable<TElement>> RetrieveTableStorage<TElement> (String tableName, String filter) where TElement : ITableEntity, new()
        {
            //Azure Connection Creation
            CloudStorageAccount _StorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = _StorageAccount.CreateCloudTableClient();

            //Get Table Access
            CloudTable _Table = tableClient.GetTableReference(tableName);

            TableQuery<TElement> query = new TableQuery<TElement>();
            if (filter != null) query = query.Where(filter);

            IEnumerable<TElement> requests = await _Table.ExecuteQueryAsync(query);

            return requests;
        }
    }
}
