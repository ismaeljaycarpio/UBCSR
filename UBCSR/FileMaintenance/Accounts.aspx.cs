using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace UBCSR.FileMaintenance
{
    public partial class Accounts : System.Web.UI.Page
    {
        DAL.FileMaintenance fm = new DAL.FileMaintenance();
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                bindData();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            fm.ChangeRole(Guid.Parse(lblRowId.Text), DDLRole.SelectedItem.Text);

            bindData();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#updateModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditHideModalScript", sb.ToString(), false);
        }

        protected void bindData()
        {
            gvAccount.DataSource = fm.searchUser(txtSearch.Text);
            gvAccount.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bindData();
        }

        protected void gvAccount_Sorting(object sender, GridViewSortEventArgs e)
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

            DataView sortedView = new DataView(fm.searchUser(txtSearch.Text));
            sortedView.Sort = e.SortExpression + " " + sortingDirection;
            Session["SortedView_accnt"] = sortedView;
            gvAccount.DataSource = sortedView;
            gvAccount.DataBind();
        }

        protected void gvAccount_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnkStatus = (LinkButton)e.Row.FindControl("lblStatus");
                LinkButton lnkReset = (LinkButton)e.Row.FindControl("lblReset");

                if (lnkStatus.Text == "Active")
                {
                    lnkStatus.Attributes.Add("onclick", "return confirm('Do you want to deactivate this user ? ');");
                }
                else
                {
                    lnkStatus.Attributes.Add("onclick", "return confirm('Do you want to activate this user ? ');");
                }

                lnkReset.Attributes.Add("onclick", "return confirm('Do you want to reset the password of this user ? ');");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                int _TotalRecs = fm.searchUser(txtSearch.Text).Rows.Count;
                int _CurrentRecStart = gvAccount.PageIndex * gvAccount.PageSize + 1;
                int _CurrentRecEnd = gvAccount.PageIndex * gvAccount.PageSize + gvAccount.Rows.Count;

                e.Row.Cells[0].ColumnSpan = 2;
                e.Row.Cells[0].Text = string.Format("Displaying <b style=color:red>{0}</b> to <b style=color:red>{1}</b> of {2} records found", _CurrentRecStart, _CurrentRecEnd, _TotalRecs);
            }
        }

        protected void gvAccount_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvAccount.PageIndex = e.NewPageIndex;
            if (Session["SortedView_accnt"] != null)
            {
                gvAccount.DataSource = Session["SortedView_accnt"];
                gvAccount.DataBind();
            }
            else
            {
                bindData();
            }
        }

        protected void gvAccount_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {

        }

        protected void gvAccount_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            dt = new DataTable();
            if (e.CommandName.Equals("editRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                dt = fm.SelectUserAccounts((Guid)(gvAccount.DataKeys[index].Value));
                lblRowId.Text = dt.Rows[0]["UserId"].ToString();
                txtEditName.Text = dt.Rows[0]["FullName"].ToString();
                DDLRole.SelectedValue = dt.Rows[0]["RoleId"].ToString();

                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#updateModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditShowModalScript", sb.ToString(), false);
            }
        }

        protected void lblStatus_Click(object sender, EventArgs e)
        {
            LinkButton lnkStatus = sender as LinkButton;
            GridViewRow gvrow = lnkStatus.NamingContainer as GridViewRow;
            Guid UserId = Guid.Parse(gvAccount.DataKeys[gvrow.RowIndex].Value.ToString());

            if (lnkStatus.Text == "Active")
            {
                fm.DeactivateUser(UserId);
            }
            else
            {
                fm.ActivateUser(UserId);
            }

            bindData();
        }

        protected void lblReset_Click(object sender, EventArgs e)
        {
            LinkButton lnkReset = sender as LinkButton;
            GridViewRow gvrow = lnkReset.NamingContainer as GridViewRow;
            Guid UserId = Guid.Parse(gvAccount.DataKeys[gvrow.RowIndex].Value.ToString());

            fm.ResetPassword(UserId);

            bindData();
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
    }
}