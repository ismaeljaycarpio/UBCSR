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

                Javascript.HideModal(this, this, "addModal");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
           fm.editCategory(txtEditCategory.Text,lblRowId.Text);
           bindData();

           Javascript.HideModal(this, this, "updateModal");
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            Javascript.ShowModal(this, this, "addModal");
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
                dt = fm.getCategory((int)(gvCategory.DataKeys[index].Value));
                lblRowId.Text = dt.Rows[0]["Id"].ToString();
                txtEditCategory.Text = dt.Rows[0]["CategoryName"].ToString();

                Javascript.ShowModal(this, this, "updateModal");
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                string rowId = ((Label)gvCategory.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;

                Javascript.ShowModal(this, this, "deleteModal");
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
            Javascript.HideModal(this, this, "deleteModal");
        }
    }
}