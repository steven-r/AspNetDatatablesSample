using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTablesNetSample.DataProviders;
using DataTablesNetSample.DataTables;

namespace DataTablesNetSample
{
    public partial class _Default : Page
    {
        private IDataTableProvider _provider;

        protected void Page_Load(object sender, EventArgs e)
        {
            _provider = DataTableHelper.GetProvider(typeof(DemoDataProvider));
            _provider.Initialize();
            _provider.LoadData(Server.MapPath("~/App_Data/MOCK_DATA.zip"));
        }
    }
}
