using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Security;
using OfficeOpenXml;
using System.IO;

namespace UBCSR.Inventory
{
    public partial class Default : System.Web.UI.Page
    {
        DAL.Transaction tr = new DAL.Transaction();
        DAL.FileMaintenance fm = new DAL.FileMaintenance();
        DataTable dt;
        CSRContextDataContext db = new CSRContextDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                populateDropdowns();
                bindData(); 
            }
        }

        protected void bindData()
        {
            gvInventory.DataSource = tr.searchInventory(txtSearch.Text);
            gvInventory.DataBind();

            txtSearch.Focus();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bindData();
        }

        protected void gvInventory_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortingDirection = string.Empty;
            if (direction == SortDirection.Ascending)
            {
                direction = SortDirection.Descending;
                sortingDirection = "Desc";
            }
            else
            {
                direction = SortDirection.Ascending;
                sortingDirection = "Asc";
            }

            DataView sortedView = new DataView(tr.searchInventory(txtSearch.Text));
            sortedView.Sort = e.SortExpression + " " + sortingDirection;
            Session["SortedView_inv"] = sortedView;
            gvInventory.DataSource = sortedView;
            gvInventory.DataBind();
        }

        protected void gvInventory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvInventory.PageIndex = e.NewPageIndex;
            if (Session["SortedView_inv"] != null)
            {
                gvInventory.DataSource = Session["SortedView_inv"];
                gvInventory.DataBind();
            }
            else
            {
                bindData();
            }
        }

        public SortDirection direction
        {
            get
            {
                if (ViewState["directionState"] == null)
                {
                    ViewState["directionState"] = SortDirection.Ascending;
                }
                return (SortDirection)ViewState["directionState"];
            }

            set
            {
                ViewState["directionState"] = value;
            }
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            Javascript.ShowModal(this, this, "addModal");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var inv = (from i in db.InventoryLINQs
                      where 
                      (i.ItemId == Convert.ToInt32(ddlAddItem.SelectedValue)) &&
                      (i.Serial == txtAddSerial.Text)
                      select i).ToList();

            if(inv.Count > 0)
            {
                lblDuplicateRecord.Text = "Duplicate record exists";
            }
            else
            {
                tr.addToInventory(ddlAddItem.SelectedValue,
                    txtAddStocks.Text,
                    txtAddSerial.Text, 
                    txtAddRemarks.Text);

                bindData();
                Javascript.HideModal(this, this, "addModal");
            }
        }

        protected void populateDropdowns()
        {
            ddlAddItem.DataSource = fm.searchItem("");
            ddlAddItem.DataTextField = "ItemName";
            ddlAddItem.DataValueField = "Id";
            ddlAddItem.DataBind();
            ddlAddItem.Items.Insert(0, new ListItem("Select One", "0"));

            ddlEditItem.DataSource = fm.searchItem("");
            ddlEditItem.DataTextField = "ItemName";
            ddlEditItem.DataValueField = "Id";
            ddlEditItem.DataBind();
            ddlEditItem.Items.Insert(0, new ListItem("Select One", "0"));

            ddlUpdateItem.DataSource = fm.searchItem("");
            ddlUpdateItem.DataTextField = "ItemName";
            ddlUpdateItem.DataValueField = "Id";
            ddlUpdateItem.DataBind();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            tr.editInventory(ddlEditItem.SelectedValue,
                txtEditStocks.Text,
                txtEditSerial.Text,
                txtEditRemarks.Text,
                lblRowId.Text);

            bindData();
            Javascript.HideModal(this, this, "editModal");
        }

        protected void gvInventory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            dt = new DataTable();
            if (e.CommandName.Equals("editRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);

                dt = tr.getInventory((int)(gvInventory.DataKeys[index].Value));
                lblRowId.Text = dt.Rows[0]["Id"].ToString();
                ddlEditItem.SelectedValue = dt.Rows[0]["ItemId"].ToString();
                txtEditStocks.Text = dt.Rows[0]["Stocks"].ToString();                
                txtEditSerial.Text = dt.Rows[0]["Serial"].ToString();
                txtEditRemarks.Text = dt.Rows[0]["Remarks"].ToString();
                Javascript.ShowModal(this, this, "editModal");
            }
            else if (e.CommandName.Equals("updateRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);

                dt = tr.getInventory((int)(gvInventory.DataKeys[index].Value));
                lblId.Text = dt.Rows[0]["Id"].ToString();
                ddlUpdateItem.SelectedValue = dt.Rows[0]["ItemId"].ToString();
                lblCurrentStocks.Text = dt.Rows[0]["Stocks"].ToString();
                Javascript.ShowModal(this, this, "updateModal");
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);

                hfDeleteId.Value = ((Label)gvInventory.Rows[index].FindControl("lblId")).Text;
                Javascript.ShowModal(this, this, "deleteModal");
            }
        }

        protected void btnUpdateStocks_Click(object sender, EventArgs e)
        {
            tr.updateStocks(txtUpdateStocks.Text, lblId.Text);
            bindData();
            Javascript.HideModal(this, this, "updateModal");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            //delete account info
            tr.deleteInventory(hfDeleteId.Value);

            bindData();
            Javascript.HideModal(this, this, "deleteModal");
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var products = tr.searchInventory(txtSearch.Text);
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Inventory");
            var totalCols = products.Columns.Count;
            var totalRows = products.Rows.Count;

            for (var col = 1; col <= totalCols; col++)
            {
                workSheet.Cells[1, col].Value = products.Columns[col - 1].ColumnName;
            }
            for (var row = 1; row <= totalRows; row++)
            {
                for (var col = 0; col < totalCols; col++)
                {
                    workSheet.Cells[row + 1, col + 1].Value = products.Rows[row - 1][col];
                }
            }
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Inventory.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
    }
}