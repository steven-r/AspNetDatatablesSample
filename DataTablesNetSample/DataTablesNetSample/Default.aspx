<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DataTablesNetSample._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>DataTables example</h1>
        <p class="lead">See http://www.datatables.net for more details</p>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
<table id="entityOverview" class="filtered-data-table table">
    <thead>
    <tr class="heading">
        <th>Id</th>
        <th>First Name</th>
        <th>Last Name</th>
        <th>Gender</th>
        <th>Email</th>
    </tr>
    <tr class="filter">
<th data-column-id='Id'>
	<i class='fa fa-filter filter-btn' data-target-column='Id' data-target='0' ></i>
	<input type='text' class='filterstring' placeholder='Id' data-target='0' name='filter-Id' style='display: none;'/>
    <span class='filter-display' style='display: none;' data-target-column='Id' data-target='0'></span>
</th>
<th data-column-FirstName='FirstName'>
	<i class='fa fa-filter filter-btn' data-target-column='FirstName' data-target='1' ></i>
	<input type='text' class='filterstring' placeholder='FirstName' data-target='1' name='filter-FirstName' style='display: none;'/>
    <span class='filter-display' style='display: none;' data-target-column='FirstName' data-target='1'></span>
</th>
<th data-column-id='LastName'>
	<i class='fa fa-filter filter-btn' data-target-column='LastName' data-target='2' ></i>
	<input type='text' class='filterstring' placeholder='LastName' data-target='2' name='filter-Id' style='display: none;'/>
    <span class='filter-display' style='display: none;' data-target-column='LastName' data-target='2'></span>
</th>
<th data-column-id='Gender'>
	<i class='fa fa-filter filter-btn' data-target-column='Gender' data-target='3' ></i>
	<input type='text' class='filterstring' placeholder='Gender' data-target='3' name='filter-Id' style='display: none;'/>
    <span class='filter-display' style='display: none;' data-target-column='Gender' data-target='3'></span>
</th>
<th data-column-id='Email'>
	<i class='fa fa-filter filter-btn' data-target-column='Email' data-target='4' ></i>
	<input type='text' class='filterstring' placeholder='Email' data-target='4' name='filter-Id' style='display: none;'/>
    <span class='filter-display' style='display: none;' data-target-column='Email' data-target='4'></span>
</th>
    </tr>
    </thead>
    <tbody>
    </tbody>
</table>
    <script type="text/javascript">
        if (! $.fn.DataTable.isDataTable('#entityOverview')) {
            // DataTable
            var table = $('#entityOverview').DataTable({
                "serverSide": true,
                "orderCellsTop": true,
                "width": "100%",
                "ajax": {
                    "url": "/DataTableProvider.ashx",
                    "type": "POST",
                    cache: false,
                    "data": function(d) {
                        return $.extend({}, d, {
                            "provider": "Demo"
                        });
                    }
                },
                "order": [[3, "asc"]],
                "columns": [{ data: 'Id', name: 'Id' },
                { data: 'FirstName', name: 'FirstName' },
                { data: 'LastName', name: 'LastName' },
                { data: 'Gender', name: 'Gender' },
                { data: 'Email', name: 'Email' },
                ],
            });
            $('#entityOverview').on('keyup', 'input.filterstring', function (e) {
                var code = e.which; // recommended to use e.which, it's normalized across browsers
                var col;
                if (code === 13) {
                    e.preventDefault();
                    col = $(this).attr("data-target");
                    $(this).siblings("span").html("&nbsp;" + this.value).show();
                    $(this).siblings("i").show();
                    $(this).hide();
                    table.columns(col).search(this.value).draw();
                    if (this.value.length > 0) {
                        $(this).parent().addClass("active");
                    } else {
                        $(this).parent().removeClass("active");
                    }
                } else if (code === 27) { // esc
                    e.preventDefault();
                    this.value = "";
                    col = $(this).attr("data-target");
                    $(this).siblings("span").html(this.value).show();
                    $(this).siblings("i").show();
                    $(this).hide();
                    $(this).parent().removeClass("active");
                    table.columns(col).search(this.value).draw();
                }
            });
            $('#entityOverview').on('blur', 'input.filterstring', function (e) {
                e.preventDefault();
                var col = $(this).attr("data-target");
                table.columns(col).search(this.value).draw();
                $(this).siblings("span").html("&nbsp;" + this.value).show();
                $(this).siblings("i").show();
                $(this).hide();
            });
            $('#entityOverview').on('click', 'tr.filter th:not(.active)', function (e) {
                e.preventDefault();
                $("span, i", this).hide();
                $("input", this).show().focus();
            });
            $('#entityOverview').on('dblclick', 'tr.filter th.active', function (e) {
                e.preventDefault();
                $("span, i", this).hide();
                $("input", this).show().focus();
            });
            $('#entityOverview').on('click', 'tr.filter th.active i', function (e) {
                e.preventDefault();
                var col = $(this).attr("data-target");
                table.columns(col).search("").draw();
                $(this).parent().removeClass("active");
                $(this).addClass("fa-filter").removeClass("fa-times").attr("title", $(this).attr("data-old-title"));
                $("#entityOverview thead tr.filter .filterstring[data-target='" + col + "']").val("");
                $("#entityOverview thead tr.filter .filter-display[data-target='" + col + "']").html("");
            });
            $('#entityOverview').on('mouseenter', 'tr.filter th.active i.fa', function (e) {
                $(this).addClass("fa-times").removeClass("fa-filter").attr("data-old-title", $(this).attr("title")).attr("title", "Clear filter");
            });
            $('#entityOverview').on('mouseleave', 'tr.filter th.active i.fa', function (e) {
                $(this).addClass("fa-filter").removeClass("fa-times").attr("title", $(this).attr("data-old-title"));
            });
            table.page(0); // move to first page on display 
        }
        function Callback_(arg) {
            if (console.log) {
                console.log("Call action" + arg);
            }
            __doPostBack($("[overview-action-button='']")[0].name, arg);
        }
    </script>
</div>
</asp:Content>
