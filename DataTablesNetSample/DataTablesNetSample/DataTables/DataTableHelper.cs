using System;
using System.Collections.Concurrent;

namespace DataTablesNetSample.DataTables
{
    /// <exclude />
    public static class DataTableHelper
    {
        private static readonly ConcurrentDictionary<string, IDataTableProvider> DataTableProviders =
            new ConcurrentDictionary<string, IDataTableProvider>();

        /// <summary>
        /// Retrieve a provider out of the list of providers registered
        /// </summary>
        /// <param name="providerType">Type of the provider.</param>
        /// <returns>A provider registered for this type</returns>
        /// <exception cref="ArgumentException">Type needs to be inherited from IDataTableProvider</exception>
        public static IDataTableProvider GetProvider(Type providerType)
        {
            if (!typeof(IDataTableProvider).IsAssignableFrom(providerType))
            {
                throw new ArgumentException("type needs to be inherited from IDataTableProvider", nameof(providerType));
            }
            IDataTableProvider provider = (IDataTableProvider)Activator.CreateInstance(providerType);
            provider = DataTableProviders.AddOrUpdate(provider.Name, provider,
                (s, tableProvider) => tableProvider); // no update
            return provider;
        }

        /// <summary>
        /// Retrieve a provider out of the list of providers registered
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TData">The type of the t data.</typeparam>
        /// <param name="providerType">Type of the provider.</param>
        /// <returns>A typed provider.</returns>
        /// <exception cref="System.ArgumentException">type needs to be inherited from IDataTableProvider</exception>
        public static IDataTableProvider<TKey, TData> GetProvider<TKey, TData>(Type providerType) where TData : class
        {
            if (!typeof(IDataTableProvider<TKey, TData>).IsAssignableFrom(providerType))
            {
                throw new ArgumentException("type needs to be inherited from IDataTableProvider", nameof(providerType));
            }
            IDataTableProvider provider = (IDataTableProvider)Activator.CreateInstance(providerType);
            provider = DataTableProviders.AddOrUpdate(provider.Name, provider,
                (s, tableProvider) => tableProvider); // no update
            return provider as IDataTableProvider<TKey, TData>;
        }

        /// <summary>
        /// Gets a provider by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IDataTableProvider.</returns>
        public static IDataTableProvider GetProvider(string name)
        {
            IDataTableProvider provider;
            if (DataTableProviders.TryGetValue(name, out provider))
            {
                return provider;
            }
            return null;
        }
    }
}
