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
            gvItem.DataSource = fm.searchItem(txtSearch.Text);
            gvItem.DataBind();
            txtSearch.Focus();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var items = (from i in db.Items
                         where i.ItemName == txtAddItem.Text.Trim()
                         select i).ToList();

            if(items.Count > 0)
            {
                lblDuplicateRecords.Text = "Item already exists";
            }
            else
            {
                fm.addItem(ddlAddCategory.SelectedValue, ddlAddBrand.SelectedValue, txtAddItem.Text);

                bindData();
                Javascript.HideModal(this, this, "addModal");
            } 
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            fm.editItem(ddlEditCategory.SelectedValue, ddlEditBrand.SelectedValue, txtEditItem.Text, lblRowId.Text);
            bindData();
            Javascript.HideModal(this, this, "updateModal");
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            Javascript.ShowModal(this, this, "addModal");
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
                dt = fm.getItem((int)(gvItem.DataKeys[index].Value));
                lblRowId.Text = dt.Rows[0]["Id"].ToString();
                ddlEditCategory.SelectedValue = dt.Rows[0]["ItemCategoryId"].ToString();
                ddlEditBrand.SelectedValue = dt.Rows[0]["ItemBrandId"].ToString();
                txtEditItem.Text = dt.Rows[0]["ItemName"].ToString();

                Javascript.ShowModal(this, this, "updateModal");
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                string rowId = ((Label)gvItem.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;
                Javascript.ShowModal(this, this, "deleteModal");
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
            Javascript.HideModal(this, this, "deleteModal");
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