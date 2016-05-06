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

namespace UBCSR.FileMaintenance
{
    public partial class Accounts : System.Web.UI.Page
    {
        DAL.FileMaintenance fm = new DAL.FileMaintenance();
        DataTable dt;
        CSRContextDataContext db = new CSRContextDataContext();
        Guid myUserId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                hfUserId.Value = Membership.GetUser().ProviderUserKey.ToString();
                
                bindData();
                populateDropdowns();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            //change username
            fm.changeUsername(Guid.Parse(lblRowId.Text), txtEditUserId.Text);

            //remove to existing role
            System.Web.Security.Roles.RemoveUserFromRoles(txtEditUserId.Text,
                System.Web.Security.Roles.GetRolesForUser(txtEditUserId.Text));
        
            //add to role
            System.Web.Security.Roles.AddUserToRole(txtEditUserId.Text, ddlEditRole.SelectedItem.Text);

            //update user's info
            fm.editUser(Guid.Parse(lblRowId.Text),
                txtEditFirstName.Text,
                txtEditMiddleName.Text,
                txtEditLastName.Text,
                txtEditUserId.Text);

            bindData();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#updateModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditHideModalScript", sb.ToString(), false);
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

                //hide delete btn for logged-in accnt
                LinkButton lbtnDelete = e.Row.FindControl("lbtnDelete") as LinkButton;
                Guid userId = Guid.Parse(gvAccount.DataKeys[e.Row.RowIndex].Value.ToString());

                if(userId == Guid.Parse(hfUserId.Value))
                {
                    lbtnDelete.Visible = false;
                }
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
                
                dt = fm.SelectUserAccounts((Guid)(gvAccount.DataKeys[index].Value));
                lblRowId.Text = dt.Rows[0]["UserId"].ToString();
                txtEditFirstName.Text = dt.Rows[0]["FirstName"].ToString();
                txtEditMiddleName.Text = dt.Rows[0]["MiddleName"].ToString();
                txtEditLastName.Text = dt.Rows[0]["LastName"].ToString();
                txtEditUserId.Text = dt.Rows[0]["StudentId"].ToString();
                ddlEditRole.SelectedValue = dt.Rows[0]["RoleId"].ToString();

                //[load groups]


                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#updateModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditShowModalScript", sb.ToString(), false);
            }
            else if(e.CommandName.Equals("deleteRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);

                string userName = ((Label)gvAccount.Rows[index].FindControl("lblStudentId")).Text;
                lblStudentId.Text = userName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#deleteModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
                //dont implement
            else if(e.CommandName.Equals("updateGroups"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string userName = ((Label)gvAccount.Rows[index].FindControl("lblStudentId")).Text;
                lblUpdateGroupId.Text = userName;

                var q = (from a in db.AccountLINQs
                         where a.StudentId == userName
                         select a).FirstOrDefault();

                //ddlGroupNo.SelectedValue = q.GroupId.ToString();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#updateGroupModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //same username and pass for newly created accnts
            MembershipUser newUser = Membership.CreateUser(txtAddUserId.Text, 
                txtAddUserId.Text);

            System.Web.Security.Roles.AddUserToRole(newUser.UserName, 
                ddlAddRole.SelectedItem.Text);

            fm.addUser(Guid.Parse(newUser.ProviderUserKey.ToString()),
                txtAddFirstName.Text,
                txtAddMiddleName.Text,
                txtAddLastName.Text,
                txtAddUserId.Text);

            bindData();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideShowModalScript", sb.ToString(), false);
        }

        protected void populateDropdowns()
        {
            ddlAddRole.DataSource = fm.getRoles("");
            ddlAddRole.DataTextField = "RoleName";
            ddlAddRole.DataValueField = "RoleId";
            ddlAddRole.DataBind();
            ddlAddRole.Items.Insert(0, new ListItem("-- Select One --", "0"));

            ddlEditRole.DataSource = fm.getRoles("");
            ddlEditRole.DataTextField = "RoleName";
            ddlEditRole.DataValueField = "RoleId";
            ddlEditRole.DataBind();
            ddlEditRole.Items.Insert(0, new ListItem("-- Select One --", "0"));

            var q = (from g in db.GroupLINQs
                     select g);

            ddlGroupNo.DataSource = q.ToList();
            ddlGroupNo.DataTextField = "Name";
            ddlGroupNo.DataValueField = "Id";
            ddlGroupNo.DataBind();
        }

        protected void gvAccount_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string userName = ((Label)gvAccount.Rows[e.RowIndex].FindControl("lblStudentId")).Text;
            Membership.DeleteUser(userName, true);

            //delete account info
            fm.deleteUser(userName);

            bindData();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Membership.DeleteUser(lblStudentId.Text, true);

            //delete account info
            fm.deleteUser(lblStudentId.Text);

            bindData();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteHideModalScript", sb.ToString(), false);
        }

        protected void btnUpdateGroupNo_Click(object sender, EventArgs e)
        {
            var q = (from a in db.AccountLINQs
                     where a.StudentId == lblUpdateGroupId.Text
                     select a).FirstOrDefault();

            //q.GroupId = Convert.ToInt32(ddlGroupNo.SelectedValue);
            db.SubmitChanges();

            bindData();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#updateGroupModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteHideModalScript", sb.ToString(), false);
        }

        protected void ddlGroupNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var q = (from g in db.GroupLINQs
                     where g.Id == Convert.ToInt32(ddlGroupNo.SelectedValue)
                     select g).FirstOrDefault();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var products = fm.searchUser(txtSearch.Text);
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Accounts");
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
                Response.AddHeader("content-disposition", "attachment;  filename=Accounts.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        protected void bindData()
        {
            gvAccount.DataSource = fm.searchUser(txtSearch.Text);
            gvAccount.DataBind();

            txtSearch.Focus();
        }
    }
}