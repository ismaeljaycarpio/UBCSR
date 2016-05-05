using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace UBCSR.FileMaintenance
{
    public partial class Categories : System.Web.UI.Page
    {
        DAL.FileMaintenance fm = new DAL.FileMaintenance();
        DataTable dt;
        CSRContextDataContext db = new CSRContextDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                bindData();
            }
        }

        protected void bindData()
        {
            gvCategory.DataSource = fm.searchCategory(txtSearch.Text);
            gvCategory.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string strCategory = txtAddCategory.Text.Trim();

            var cat = (from c in db.ItemCategories
                       where c.CategoryName == strCategory
                       select c).ToList();

            if(cat.Count > 0)
            {
                lblDuplicateRecords.Text = "Category already exist!";
            }
            else
            {
                fm.addCategory(strCategory);
                bindData();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#addModal').modal('hide');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideShowModalScript", sb.ToString(), false);
            }   
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
           fm.editCategory(txtEditCategory.Text,lblRowId.Text);

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

        protected void gvCategory_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string rowId = ((Label)gvCategory.Rows[e.RowIndex].FindControl("lblRowId")).Text;
            fm.deleteCategory(rowId);

            bindData();
        }

        protected void gvCategory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            dt = new DataTable();
            int index = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName.Equals("editRecord"))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                dt = fm.getCategory((int)(gvCategory.DataKeys[index].Value));
                lblRowId.Text = dt.Rows[0]["Id"].ToString();
                txtEditCategory.Text = dt.Rows[0]["CategoryName"].ToString();

                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#updateModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditShowModalScript", sb.ToString(), false);
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                string rowId = ((Label)gvCategory.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#deleteModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
        }

        protected void gvCategory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategory.PageIndex = e.NewPageIndex;
            bindData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bindData();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            fm.deleteCategory(hfDeleteId.Value);
            bindData();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteHideModalScript", sb.ToString(), false);
        }
    }
}