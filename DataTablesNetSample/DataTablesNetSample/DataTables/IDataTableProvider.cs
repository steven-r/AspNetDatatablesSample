using System.Collections.Generic;

namespace DataTablesNetSample.DataTables
{
    /// <summary>
    /// Defines an interface to provide data to drive a DataTable. 
    /// </summary>
    /// <remarks>This class should not be used as a basis for a detailed implementation. Consider using <see cref="IDataTableProvider{TKey,TData}"/>. 
    /// Anyhow, for a simple data provider without usage of displayed data IDataTableProvider is sufficient</remarks>
    public interface IDataTableProvider
    {
        /// <summary>
        /// The name of the provider. This name needs to be unique.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Initializes this instance. The initialization might be done more than one, the function should savely reset. 
        /// </summary>
        void Initialize();

        /// <summary>
        /// Initiate data load.
        /// </summary>
        /// <param name="parameters">a list of parameters dependant on the provider implementation.</param>
        /// <returns><c>true</c> if data is available to be displayed, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// <para>No calls to <see cref="GetData"/> should be performed before loading data.</para>
        /// </remarks>
        bool LoadData(params object[] parameters);

        /// <summary>
        /// Retrieve data towards the frontend. 
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.String.</returns>
        /// <remarks>For a detailed description of the target data format please refer to http://datatables.net/manual/server-side (setion "Returned Data")</remarks>
        string GetData(DataTableParameterModel model);

        /// <summary>
        /// Return the defined columns
        /// </summary>
        /// <returns>The columns.</returns>
        IEnumerable<DataTableColumn> Columns { get; }

        /// <summary>
        /// retrieve a single column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The column definition.</returns>
        DataTableColumn GetColumn(string columnName);
    }

    /// <summary>
    /// Define a interface towards a data table which allows generic access to loaded data.
    /// </summary>
    /// <typeparam name="TKey">The primary key information about the data stored.</typeparam>
    /// <typeparam name="TData">The data store used.</typeparam>
    /// <seealso cref="IDataTableProvider"/>
    public interface IDataTableProvider<in TKey, out TData> : IDataTableProvider
        where TData : class
    {
        /// <summary>
        /// Get a specific entry out of the list of displayed data.
        /// </summary>
        /// <param name="key">the primary key of the data element to be retrieved</param>
        /// <returns>The retrieved data element</returns>
        TData Get(TKey key);
    }
}
