using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Security;

namespace UBCSR.Inventory
{
    public partial class Default : System.Web.UI.Page
    {
        DAL.Transaction tr = new DAL.Transaction();
        DAL.FileMaintenance fm = new DAL.FileMaintenance();
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                bindData();
                populateDropdowns();
            }
        }

        protected void bindData()
        {
            gvInventory.DataSource = tr.searchInventory(txtSearch.Text);
            gvInventory.DataBind();
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

        protected void gvInventory_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gvInventory_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {

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
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddShowModalScript", sb.ToString(), false);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            tr.addToInventory(ddlAddItem.SelectedValue,
                txtAddStocks.Text,
                txtAddExpiration.Text,
                txtAddSerial.Text, txtAddRemarks.Text);

            bindData();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideShowModalScript", sb.ToString(), false);
        }

        protected void populateDropdowns()
        {
            ddlAddItem.DataSource = fm.searchItem("");
            ddlAddItem.DataTextField = "ItemName";
            ddlAddItem.DataValueField = "Id";
            ddlAddItem.DataBind();

            ddlEditItem.DataSource = fm.searchItem("");
            ddlEditItem.DataTextField = "ItemName";
            ddlEditItem.DataValueField = "Id";
            ddlEditItem.DataBind();

            ddlUpdateItem.DataSource = fm.searchItem("");
            ddlUpdateItem.DataTextField = "ItemName";
            ddlUpdateItem.DataValueField = "Id";
            ddlUpdateItem.DataBind();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            tr.editInventory(ddlEditItem.SelectedValue,
                txtEditStocks.Text,
                txtEditExpiration.Text,
                txtEditSerial.Text,
                txtEditRemarks.Text,
                lblRowId.Text);

            bindData();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#editModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditHideModalScript", sb.ToString(), false);
        }

        protected void gvInventory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            dt = new DataTable();
            if (e.CommandName.Equals("editRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                dt = tr.getInventory((int)(gvInventory.DataKeys[index].Value));
                lblRowId.Text = dt.Rows[0]["Id"].ToString();
                ddlEditItem.SelectedValue = dt.Rows[0]["ItemId"].ToString();
                txtEditStocks.Text = dt.Rows[0]["Stocks"].ToString();
                txtEditExpiration.Text = Convert.ToDateTime(dt.Rows[0]["Expiration"].ToString()).ToShortDateString();
                txtEditSerial.Text = dt.Rows[0]["Serial"].ToString();
                txtEditRemarks.Text = dt.Rows[0]["Remarks"].ToString();

                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#editModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditShowModalScript", sb.ToString(), false);
            }
            else if (e.CommandName.Equals("updateRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                dt = tr.getInventory((int)(gvInventory.DataKeys[index].Value));
                lblId.Text = dt.Rows[0]["Id"].ToString();
                ddlUpdateItem.SelectedValue = dt.Rows[0]["ItemId"].ToString();
                lblCurrentStocks.Text = dt.Rows[0]["Stocks"].ToString();

                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#updateModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditShowModalScript", sb.ToString(), false);
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);

                hfDeleteId.Value = ((Label)gvInventory.Rows[index].FindControl("lblId")).Text;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#deleteModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
        }

        protected void btnUpdateStocks_Click(object sender, EventArgs e)
        {
            tr.updateStocks(txtUpdateStocks.Text, lblId.Text);

            bindData();
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#updateModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "UpdateHideModalScript", sb.ToString(), false);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            //delete account info
            tr.deleteInventory(hfDeleteId.Value);

            bindData();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteHideModalScript", sb.ToString(), false);
        }
    }
}