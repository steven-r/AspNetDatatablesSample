namespace DataTablesNetSample.DataTables
{
    /// <summary>
    /// For details please refer to http://datatables.net/manual/server-side
    /// </summary>
    public class DataTableParameterModel
    {
        /// <summary>
        /// Create default class
        /// </summary>
        public DataTableParameterModel()
        {
            DisplayStart = 0;
            DisplayLength = 20;
            SearchString = "";
        }

        /// <summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        /// </summary>       
        public string Draw { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string SearchString { get; set; }

        public bool IsSearchStringRegex { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int DisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int DisplayStart { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int SortingCols { get; set; }

        /// <summary>
        /// Array of column ids to sort, one per sort column. The size of this array is iSortingCols
        /// </summary>
        public int[] SortColumns { get; set; }

        /// <summary>
        /// The direction of each sort, one per sort column. The size of this array is iSortingCols
        /// </summary>
        public string[] SortDirs { get; set; }


        public DataTableColumn[] Columns { get; set; }
        public int ColumnCount { get; set; }


        public class DataTableColumn
        {
            public object Data { get; set; }

            public string Name { get; set; }

            public bool Searchable { get; set; }

            public bool Orderable { get; set; }

            public string SearchValue { get; set; }

            public bool RegexSearch { get; set; }
        }
    }

}
