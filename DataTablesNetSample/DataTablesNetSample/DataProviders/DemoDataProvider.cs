using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using DataTablesNetSample.DataTables;
using DataTablesNetSample.Helper;
using Ionic.Zip;
using Newtonsoft.Json;

namespace DataTablesNetSample.DataProviders
{
    /// <summary>
    /// Class DemoDataProvider.
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class DemoDataProvider: IDataTableProvider<int, DemoDataEntry>
    {
        private List<DemoDataEntry> _data;
        private static IEnumerable<DataTableColumn> _columns; 

        /// <summary>
        /// Get a specific entry out of the list of displayed data.
        /// </summary>
        /// <param name="key">the primary key of the data element to be retrieved</param>
        /// <returns>The retrieved data element</returns>
        public DemoDataEntry Get(int key)
        {
            return _data.SingleOrDefault(x => x.Id == key);
        }

        public string Name => "Demo";

        public void Initialize()
        {
            CreateColumns();
        }

        private void CreateColumns()
        {
            if (_columns != null)
            {
                return; // done
            }
            _columns = new List<DataTableColumn>
            {
                new DataTableColumn { Name = "Id", },
                new DataTableColumn { Name = "FirstName", },
                new DataTableColumn { Name = "LastName", },
                new DataTableColumn { Name = "Gender", },
                new DataTableColumn { Name = "Email", },
            };
        }

        /// <summary>
        /// Initiate data load.
        /// </summary>
        /// <param name="parameters">a list of parameters dependant on the provider implementation.</param>
        /// <returns><c>true</c> if data is available to be displayed, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// <para>No calls to <see cref="IDataTableProvider.GetData"/> should be performed before loading data.</para>
        /// </remarks>
        public bool LoadData(params object[] parameters)
        {
            string filename = parameters[0] as string;
            if (HttpContext.Current.Session["Data"] != null &&
                (string) HttpContext.Current.Session["FileName"] == filename)
            {
                // data hasn't changed
                return true;
            }
            using (ZipFile zipFile = new ZipFile(filename, Encoding.UTF8))
            {
                ZipEntry entry = zipFile["MOCK_DATA.json"];
                using (TextReader reader = new StreamReader(entry.OpenReader()))
                {
                    _data = JsonConvert.DeserializeObject<List<DemoDataEntry>>(reader.ReadToEnd());
                }
                HttpContext.Current.Session["Data"] = _data;
                HttpContext.Current.Session["FileName"] = filename;
            }
            return true; // false might help to identify errors during loading
        }

        /// <summary>
        /// Retrieve data towards the frontend. 
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.String.</returns>
        /// <remarks>For a detailed description of the target data format please refer to http://datatables.net/manual/server-side (setion "Returned Data")</remarks>
        public string GetData(DataTableParameterModel model)
        {
            List<DemoDataEntry> data = (List<DemoDataEntry>)HttpContext.Current.Session["Data"];
            var predicate = BuildFilter<DemoDataEntry>(model);

            int totalCount = data.Count;
            data = data.AsQueryable().Where(predicate).ToList();
            int filterCount = data.Count;
            IOrderedQueryable<DemoDataEntry> sortQuery = CreateSortedQuery(model, data.AsQueryable());

            data = sortQuery.ToList();
            if (model.DisplayLength > 0)
            {
                data = data.Skip(model.DisplayStart).Take(model.DisplayLength).ToList();
            }
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
#if DEBUG
                writer.Formatting = Formatting.Indented;
#endif
                writer.WriteStartObject();
                writer.WritePropertyName("draw");
                writer.WriteValue(model.Draw);
                writer.WritePropertyName("recordsTotal");
                writer.WriteValue(totalCount);
                writer.WritePropertyName("recordsFiltered");
                writer.WriteValue(filterCount);
                writer.WritePropertyName("data");
                writer.WriteStartArray();
                foreach (DemoDataEntry entity in data)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("DT_RowId");
                    writer.WriteValue(entity.Id);
                    foreach (DataTableColumn column in Columns.Where(x => x.IsMapped && !x.RowAttribute))
                    {
                        writer.WritePropertyName(column.Name);
                        writer.WriteValue(column.GetValue(entity));
                    }
                    writer.WritePropertyName("DT_RowAttr");
                    writer.WriteStartObject();
                    foreach (DataTableColumn column in Columns.Where(x => x.RowAttribute))
                    {
                        writer.WritePropertyName(column.RowAttributeName ?? column.Name);
                        writer.WriteValue(column.GetValue(entity));
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
                writer.WriteEnd();
                writer.WriteEndObject();
            }
            return sb.ToString();

        }

        protected virtual Expression<Func<T, bool>> BuildFilter<T>(DataTableParameterModel model)
        {
            var predicate = PredicateBuilder.True<T>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (
                DataTableParameterModel.DataTableColumn column in
                    model.Columns.Where(x => x.Searchable && !string.IsNullOrWhiteSpace(x.SearchValue)))
            {
                // x =>
                var parameterExp = Expression.Parameter(typeof(T), "type");

                // x => x.<PropertyName>
                var propertyExp = Expression.Property(parameterExp, column.Name);
                Expression condition;
                MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { });
                MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                MethodInfo toStringMethod = typeof(int).GetMethod("ToString", new Type[] { });

                if (propertyExp.Type == typeof (int))
                {
                    // x.<PropertyName>.ToString()
                    var propertyStringExp = Expression.Call(propertyExp, toStringMethod);
                    // x.<PropertyName>.ToString().Contains(<searchValue>)
                    condition = Expression.Call(propertyStringExp, containsMethod, Expression.Constant(column.SearchValue));
                }
                else
                {
                    // <searchValue>.ToLower()
                    var someValue = Expression.Call(Expression.Constant(column.SearchValue, typeof(string)), toLowerMethod);

                    // x.<PropertyName>.ToLower()
                    var toLowerMethodExp = Expression.Call(propertyExp, toLowerMethod);

                    // x.<PropertyName>.ToLower().Contains(<searchValue>.ToLower())
                    var containsMethodExp = Expression.Call(toLowerMethodExp, containsMethod, someValue);

                    // x.<PropertyName> != null
                    var nullCheck = Expression.NotEqual(propertyExp, Expression.Constant(null, typeof(object)));

                    // x => x.<PropertyName> != null && x.<PropertyName>.ToLower().Contains(<SearchString>.ToLower()) 
                    condition = Expression.AndAlso(nullCheck, containsMethodExp);
                }

                // Build a expression that is a function that gets a element of <T> as input and returns true or false if t.propertyName contains searchstring
                Expression<Func<T, bool>> lambda =
                    Expression.Lambda<Func<T, bool>>(
                        condition, parameterExp);
                predicate = predicate.And(lambda);
            }
            return predicate;
        }

        protected virtual IOrderedQueryable<T> CreateSortedQuery<T>(DataTableParameterModel parameterModel, IQueryable<T> baseQuery)
        {
            var orderedQuery = (IOrderedQueryable<T>)baseQuery;

            for (int i = 0; i < parameterModel.SortingCols; ++i)
            {
                var ascending = string.Equals("asc", parameterModel.SortDirs[i], StringComparison.OrdinalIgnoreCase);
                int sortCol = parameterModel.SortColumns[i];

                Expression<Func<T, IComparable>> orderByExpression = GetOrderByExpression<T>(sortCol, parameterModel);

                if (ascending)
                {
                    orderedQuery = (i == 0)
                        ? orderedQuery.OrderBy(orderByExpression)
                        : orderedQuery.ThenBy(orderByExpression);
                }
                else
                {
                    orderedQuery = (i == 0)
                        ? orderedQuery.OrderByDescending(orderByExpression)
                        : orderedQuery.ThenByDescending(orderByExpression);
                }
            }
            return orderedQuery;
        }
        protected virtual Expression<Func<T, IComparable>> GetOrderByExpression<T>(int sortCol, DataTableParameterModel model)
        {
            string propertyName = model.Columns[sortCol].Name;
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                propertyName = model.Columns.First(x => x.Name != null).Name;
            }
            Type t = typeof(T);
            PropertyInfo info = t.GetProperty(propertyName);
            var parameter = Expression.Parameter(t);
            var property = Expression.Property(parameter, info);
            var funcType = typeof(Func<,>).MakeGenericType(t, typeof(IComparable));
            var lambda = Expression.Lambda(funcType, Expression.Convert(property, typeof(IComparable)), parameter);
            return (Expression<Func<T, IComparable>>)lambda;
        }

        public IEnumerable<DataTableColumn> Columns
        {
            get
            {
                CreateColumns();
                return _columns;
            }
        }

        /// <summary>
        /// retrieve a single column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The column definition.</returns>
        public DataTableColumn GetColumn(string columnName)
        {
            return Columns.Single(x => x.Name == columnName);
        }
    }
}