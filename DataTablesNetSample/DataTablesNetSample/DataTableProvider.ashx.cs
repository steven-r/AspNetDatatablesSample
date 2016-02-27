using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using DataTablesNetSample.DataTables;

namespace DataTablesNetSample
{
    /// <summary>
    /// Summary description for DataTableProvider
    /// </summary>
    public class DataTableProvider : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request["Provider"] == null)
            {
                context.Response.Write("Provider is missing");
            }
            try
            {
                IDataTableProvider provider = DataTableHelper.GetProvider(context.Request["Provider"]);

                context.Response.Write(provider.GetData(GetModel(context)));
            }
            catch (Exception e)
            {
                context.Response.Write("Cannot process due to '" + e.Message + "'");
            }
        }

        private DataTableParameterModel GetModel(HttpContext context)
        {
            string map = string.Join("[]", context.Request.Form.AllKeys);
            int colCount = 0;
            while (map.Contains("columns[" + colCount + "]"))
            {
                colCount++;
            }
            int sortCount = 0;
            while (map.Contains("order[" + sortCount + "]"))
            {
                sortCount++;
            }
            var model = new DataTableParameterModel
            {
                ColumnCount = colCount,
                DisplayLength = Convert.ToInt32(context.Request["length"]),
                DisplayStart = Convert.ToInt32(context.Request["start"]),
                SortingCols = sortCount,
                Draw = context.Request["draw"],
                SearchString = context.Request["search[value]"],
                IsSearchStringRegex =
                    bool.Parse(context.Request["search[regex]"])
            };
            model.SortColumns = new int[model.SortingCols];
            model.SortDirs = new string[model.SortingCols];
            model.Columns = new DataTableParameterModel.DataTableColumn[model.ColumnCount];
            for (int i = 0; i < model.ColumnCount; ++i)
            {
                string prefix = $"columns[{i}]";
                DataTableParameterModel.DataTableColumn column = new DataTableParameterModel.DataTableColumn
                {
                    Name = context.Request[prefix + "[name]"],
                    Orderable =
                        bool.Parse(context.Request[prefix + "[orderable]"]),
                    Searchable =
                        bool.Parse(context.Request[prefix + "[searchable]"]),
                    SearchValue =
                        context.Request[prefix + "[search][value]"],
                    RegexSearch =
                        bool.Parse(context.Request[prefix + "[search][regex]"]),
                };
                model.Columns[i] = column;
            }

            for (int i = 0; i < model.SortingCols; ++i)
            {
                string prefix = $"order[{i}]";
                model.SortColumns[i] =
                    Convert.ToInt32(context.Request[prefix + "[column]"]);
                model.SortDirs[i] = context.Request[prefix + "[dir]"];
            }
            return model;
        }


        public bool IsReusable => false;
    }
}