using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naspinski.AzureStorage.Interfaces
{
    public interface ITable
    {
        CloudTableClient Client { get; }
        CloudTable Table { get; }

        void Add<T>(T entity) where T : ITableEntity, new();
        void Add<T>(IEnumerable<T> entities) where T : ITableEntity, new();

        void Save<T>(T entity) where T : ITableEntity, new();
        void Save<T>(IEnumerable<T> entities) where T : ITableEntity, new();

        Task<T> Get<T>(string partitionKey, string rowKey) where T : ITableEntity, new();
        Task<IList<T>> Get<T>(string partitionKey) where T : ITableEntity, new();
        
        Task Delete<T>(T entity) where T : ITableEntity, new();
        Task Delete<T>(string partitionKey, string rowKey) where T : ITableEntity, new();
    }
}
