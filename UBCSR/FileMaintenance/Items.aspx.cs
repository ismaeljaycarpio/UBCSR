using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

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
            gvItem.DataSource = fm.searchItem("");
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

            ddlAddBrand.DataSource = fm.searchBrand("");
            ddlAddBrand.DataTextField = "BrandName";
            ddlAddBrand.DataValueField = "Id";
            ddlAddBrand.DataBind();

            ddlEditCategory.DataSource = fm.searchCategory("");
            ddlEditCategory.DataTextField = "CategoryName";
            ddlEditCategory.DataValueField = "Id";
            ddlEditCategory.DataBind();

            ddlEditBrand.DataSource = fm.searchBrand("");
            ddlEditBrand.DataTextField = "BrandName";
            ddlEditBrand.DataValueField = "Id";
            ddlEditBrand.DataBind();
        }
    }
}