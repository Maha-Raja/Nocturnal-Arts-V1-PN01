using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nat.Core.Storage.Extension
{
    public static class CloudTableExtension
    {
        /// <summary>
        /// Extension method for executing query asynchronously
        /// </summary>
        /// <typeparam name="T">TElement</typeparam>
        /// <param name="table"></param>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <param name="onProgress"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query, CancellationToken ct = default(CancellationToken), Action<IList<T>> onProgress = null) where T : ITableEntity, new()
        {
            var items = new List<T>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<T> seg = await table.ExecuteQuerySegmentedAsync<T>(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                onProgress?.Invoke(items);
            } while (token != null && !ct.IsCancellationRequested);

            return items;
        }
    }
}
