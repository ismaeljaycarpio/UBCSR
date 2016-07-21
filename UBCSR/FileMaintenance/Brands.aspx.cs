using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace UBCSR.FileMaintenance
{
    public partial class Brand : System.Web.UI.Page
    {
        DAL.FileMaintenance fm = new DAL.FileMaintenance();
        DataTable dt;
        CSRContextDataContext db = new CSRContextDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                bindData();
            }
        }

        protected void bindData()
        {
            gvBrand.DataSource = fm.searchBrand(txtSearch.Text);
            gvBrand.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var brands = (from b in db.ItemBrands
                          where b.BrandName == txtAddBrand.Text.Trim()
                          select b).ToList();

            if(brands.Count > 0)
            {
                lblDuplicateRecords.Text = "Brand name already exists!";
            }
            else
            {
                fm.addBrand(txtAddBrand.Text);

                bindData();

                Javascript.HideModal(this, this, "addModal");
            }   
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            fm.editBrand(txtEditBrand.Text, lblRowId.Text);
            bindData();
            Javascript.HideModal(this, this, "updateModal");
        }

        protected void gvBrand_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBrand.PageIndex = e.NewPageIndex;
            bindData();
        }

        protected void gvBrand_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string rowId = ((Label)gvBrand.Rows[e.RowIndex].FindControl("lblRowId")).Text;
            fm.deleteBrand(rowId);

            bindData();
        }

        protected void gvBrand_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            dt = new DataTable();
            int index = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName.Equals("editRecord"))
            {
                dt = fm.getBrand((int)(gvBrand.DataKeys[index].Value));
                lblRowId.Text = dt.Rows[0]["Id"].ToString();
                txtEditBrand.Text = dt.Rows[0]["BrandName"].ToString();
                Javascript.ShowModal(this, this, "updateModal");
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                string rowId = ((Label)gvBrand.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;
                Javascript.ShowModal(this, this, "deleteModal");
            }
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            Javascript.ShowModal(this, this, "addModal");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bindData();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            fm.deleteBrand(hfDeleteId.Value);
            bindData();
            Javascript.HideModal(this, this, "deleteModal");
        }
    }
}