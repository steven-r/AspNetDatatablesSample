using System;
using System.Reflection;

namespace DataTablesNetSample.DataTables
{
    public class DataTableColumn
    {
        private static readonly Type DefaultDataType = typeof(string);

        public DataTableColumn()
        {
            IsInitialSort = PrimaryKey = false;
            ToolTip = string.Empty;
            Searchable = Sortable = IsMapped = Visible = true;
            DataType = DefaultDataType;
        }

        public bool IsInitialSort { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        protected Type DataType { get; set; }
        public bool PrimaryKey { get; set; }

        public string ToolTip { get; set; }

        public bool Searchable { get; set; }

        public bool Sortable { get; set; }

        public bool IsMapped { get; set; }

        public Func<DataTableColumn, string> Render;

        public bool Visible { get; set; }
        public bool RowAttribute { get; set; }
        public string RowAttributeName { get; set; }

        public bool CanSort => Visible && Sortable;

        public bool CanFilter => Visible && Searchable;

        public string Width { get; set; }

        public object GetValue(object data)
        {
            PropertyInfo info = data.GetType().GetProperty(Name);
            return info.GetValue(data);
        }
    }
}
