using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OfficeOpenXml;
using System.IO;

namespace UBCSR.FileMaintenance
{
    public partial class Items : System.Web.UI.Page
    {
        DAL.FileMaintenance fm = new DAL.FileMaintenance();
        DataTable dt;

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
            gvItem.DataSource = fm.searchItem(txtSearch.Text);
            gvItem.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            fm.addItem(ddlAddCategory.SelectedValue, ddlAddBrand.SelectedValue, txtAddItem.Text);

            bindData();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideShowModalScript", sb.ToString(), false);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            fm.editItem(ddlEditCategory.SelectedValue, ddlEditBrand.SelectedValue, txtEditItem.Text, lblRowId.Text);

            bindData();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#updateModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditHideModalScript", sb.ToString(), false);
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddShowModalScript", sb.ToString(), false);
        }

        protected void gvItem_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string rowId = ((Label)gvItem.Rows[e.RowIndex].FindControl("lblRowId")).Text;
            fm.deleteItem(rowId);

            bindData();
        }

        protected void gvItem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            dt = new DataTable();
            int index = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName.Equals("editRecord"))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                dt = fm.getItem((int)(gvItem.DataKeys[index].Value));
                lblRowId.Text = dt.Rows[0]["Id"].ToString();
                ddlEditCategory.SelectedValue = dt.Rows[0]["ItemCategoryId"].ToString();
                ddlEditBrand.SelectedValue = dt.Rows[0]["ItemBrandId"].ToString();
                txtEditItem.Text = dt.Rows[0]["ItemName"].ToString();

                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#updateModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditShowModalScript", sb.ToString(), false);
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                string rowId = ((Label)gvItem.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#deleteModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
        }

        protected void gvItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvItem.PageIndex = e.NewPageIndex;
            bindData();
        }

        protected void populateDropdowns()
        {
            ddlAddCategory.DataSource = fm.searchCategory("");
            ddlAddCategory.DataTextField = "CategoryName";
            ddlAddCategory.DataValueField = "Id";            
            ddlAddCategory.DataBind();
            ddlAddCategory.Items.Insert(0, new ListItem("Select One", "0"));

            ddlAddBrand.DataSource = fm.searchBrand("");
            ddlAddBrand.DataTextField = "BrandName";
            ddlAddBrand.DataValueField = "Id";          
            ddlAddBrand.DataBind();
            ddlAddBrand.Items.Insert(0, new ListItem("Select One", "0"));

            ddlEditCategory.DataSource = fm.searchCategory("");
            ddlEditCategory.DataTextField = "CategoryName";
            ddlEditCategory.DataValueField = "Id";
            ddlEditCategory.DataBind();
            ddlEditCategory.Items.Insert(0, new ListItem("Select One", "0"));

            ddlEditBrand.DataSource = fm.searchBrand("");
            ddlEditBrand.DataTextField = "BrandName";
            ddlEditBrand.DataValueField = "Id";
            ddlEditBrand.DataBind();
            ddlEditBrand.Items.Insert(0, new ListItem("Select One", "0"));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bindData();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            fm.deleteItem(hfDeleteId.Value);
            bindData();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteHideModalScript", sb.ToString(), false);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var products = fm.searchItem(txtSearch.Text);
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Items");
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
                Response.AddHeader("content-disposition", "attachment;  filename=items.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
    }
}