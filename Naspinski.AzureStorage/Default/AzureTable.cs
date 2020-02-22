using Naspinski.AzureStorage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naspinski.AzureStorage.Default
{
    public class AzureTable : ITable
    {
        public CloudTableClient Client { get; }
        public CloudTable Table { get; }

        public AzureTable(CloudStorageAccount account, string tableName)
        {
            Client = account.CreateCloudTableClient();
            Table = Client.GetTableReference(tableName);
            Table.CreateIfNotExistsAsync();
        }
        
        async public void Add<T>(T entity) where T : ITableEntity, new()
        {
            TableOperation insert = TableOperation.Insert(entity);
            await Table.ExecuteAsync(insert);
        }

        async public void Add<T>(IEnumerable<T> entities) where T : ITableEntity, new()
        {
            var batch = new TableBatchOperation();
            entities.ToList().ForEach(entity => batch.Insert(entity));
            await Table.ExecuteBatchAsync(batch);
        }
        
        async public void Save<T>(T entity) where T : ITableEntity, new()
        {
            var upsert = TableOperation.InsertOrReplace(entity);
            var result = await Table.ExecuteAsync(upsert);
        }

        async public void Save<T>(IEnumerable<T> entities) where T : ITableEntity, new()
        {
            var batch = new TableBatchOperation();
            entities.ToList().ForEach(entity => batch.InsertOrReplace(entity));
            await Table.ExecuteBatchAsync(batch);
        }

        public async Task<T> Get<T>(string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            var retrieve = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await Table.ExecuteAsync(retrieve);
            return (T)result.Result;
        }

        public async Task<IList<T>> Get<T>(string partitionKey) where T : ITableEntity, new()
        {
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            var results = new List<T>();
            TableContinuationToken token = null;
            do
            {
                var resultSegment = await Table.ExecuteQuerySegmentedAsync(query, token);
                results.AddRange(resultSegment);
                token = resultSegment.ContinuationToken;
            } while (token != null);
            return results;
        }

        public async Task Delete<T>(T entity) where T : ITableEntity, new()
        {
            if(!EqualityComparer<T>.Default.Equals(entity, default(T)))
            {
                var delete = TableOperation.Delete(entity);
                await Table.ExecuteAsync(delete).ConfigureAwait(false);
            }
        }

        public async Task Delete<T>(string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            await Delete(await Get<T>(partitionKey, rowKey).ConfigureAwait(false));
        }
    }
}
