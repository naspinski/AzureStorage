using Naspinski.AzureStorage.Interfaces;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;

namespace Naspinski.AzureStorage.Default
{
    public class AzureStorage : IStorage
    {
        public CloudStorageAccount Account { get; }
        public Dictionary<string, ITable> Tables { get; } = new Dictionary<string, ITable>();

        public AzureStorage(string connectionString)
        {
            Account = CloudStorageAccount.Parse(connectionString);
        }

        public ITable GetTable(string tableName)
        {
            if (!Tables.ContainsKey(tableName))
                Tables.Add(tableName, new AzureTable(Account, tableName));
            return Tables[tableName];
        }
    }
}
