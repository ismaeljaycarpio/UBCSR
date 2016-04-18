using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UBCSR.FileMaintenance
{
    public partial class Group : System.Web.UI.Page
    {
        CSRContextDataContext db = new CSRContextDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                bindSubject();
                bindGridview();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bindGridview();
        }

        protected void gvGroups_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvGroups.PageIndex = e.NewPageIndex;
            bindGridview();
        }

        protected void gvGroups_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName.Equals("editRecord"))
            {
                var q = (from g in db.GroupLINQs
                         where g.Id == (int)gvGroups.DataKeys[index].Value
                         select g).FirstOrDefault();

                lblRowId.Text = q.Id.ToString();
                txtEditGroup.Text = q.Name;

                //load accnts
                Guid myUserid = Guid.Parse(q.LeaderUserId.ToString());
                bindEditDropdown(myUserid);

                ddlEditGroupLeader.SelectedValue = q.LeaderUserId.ToString();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#updateModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditShowModalScript", sb.ToString(), false);
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                string rowId = ((Label)gvGroups.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#deleteModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            //bindDropdown();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddShowModalScript", sb.ToString(), false);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var q = (from g in db.GroupLINQs
                     where g.Id == Convert.ToInt32(hfDeleteId.Value)
                     select g).FirstOrDefault();

            db.GroupLINQs.DeleteOnSubmit(q);

            db.SubmitChanges();

            bindGridview();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteHideModalScript", sb.ToString(), false);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var q = (from g in db.GroupLINQs
                     where g.Id == Convert.ToInt32(lblRowId.Text)
                     select g).FirstOrDefault();

            q.Name = txtEditGroup.Text;
            q.SubjectId = Convert.ToInt32(ddlEditSubject.SelectedValue);
            db.SubmitChanges();

            //chk if new leadership
            Guid previousLeaderId = Guid.Parse(q.LeaderUserId.ToString());
            q.LeaderUserId = Guid.Parse(ddlEditGroupLeader.SelectedValue.ToString());
            if(q.LeaderUserId != previousLeaderId)
            {
                //reset previous leader to groupid = 0
                var a = (from ac in db.AccountLINQs
                         where ac.UserId == previousLeaderId
                         select ac).FirstOrDefault();

                //a.GroupId = 0;
                db.SubmitChanges();
            }

            bindGridview();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#updateModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditHideModalScript", sb.ToString(), false);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            GroupLINQ g = new GroupLINQ();
            g.Name = txtAddGroup.Text;
            g.LeaderUserId = Guid.Parse(ddlGroupLeader.SelectedValue.ToString());
            g.SubjectId = Convert.ToInt32(ddlAddSubject.SelectedValue);

            db.GroupLINQs.InsertOnSubmit(g);

            db.SubmitChanges();

            bindGridview();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideShowModalScript", sb.ToString(), false);
        }

        protected void ddlAddSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAddSubject.SelectedValue == "0")
            {
                lblAddYearFrom.Text = String.Empty;
                lblAddYearTo.Text = String.Empty;
                lblAddSem.Text = String.Empty;
            }
            else
            {
                var q = (from s in db.SubjectLINQs
                         where s.Id == Convert.ToInt32(ddlAddSubject.SelectedValue)
                         select s).FirstOrDefault();

                lblAddYearFrom.Text = q.YearFrom.ToString();
                lblAddYearTo.Text = q.YearTo.ToString();
                lblAddSem.Text = q.Sem.ToString();

                //fill leader drop down - load users that dont belong on this subject
                var user = (from a in db.AccountLINQs
                           join gm in db.GroupMembers
                           on a.UserId equals gm.UserId
                           into b
                           from gm in b.DefaultIfEmpty()
                           select new
                           {
                               UserId = a.UserId,
                               FullName = a.LastName + " , " + a.FirstName + " " + a.MiddleName
                           }).ToList();

                ddlGroupLeader.DataSource = user;
                ddlGroupLeader.DataTextField = "FullName";
                ddlGroupLeader.DataValueField = "UserId";
                ddlGroupLeader.DataBind();
            }
        }

        protected void ddlEditSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEditSubject.SelectedValue == "0")
            {
                lblEditYearFrom.Text = String.Empty;
                lblEditYearTo.Text = String.Empty;
                lblEditSem.Text = String.Empty;
            }
            else
            {
                var q = (from s in db.SubjectLINQs
                         where s.Id == Convert.ToInt32(ddlEditSubject.SelectedValue)
                         select s).FirstOrDefault();

                lblEditYearFrom.Text = q.YearFrom.ToString();
                lblEditYearTo.Text = q.YearTo.ToString();
                lblEditSem.Text = q.Sem.ToString();
            }
        }

        protected void bindSubject()
        {
            var q = (from s in db.SubjectLINQs
                     select s).ToList();

            ddlAddSubject.DataSource = q;
            ddlAddSubject.DataTextField = "Name";
            ddlAddSubject.DataValueField = "Id";
            ddlAddSubject.DataBind();
            ddlAddSubject.Items.Insert(0, new ListItem("Select One", "0"));

            ddlEditSubject.DataSource = q;
            ddlEditSubject.DataTextField = "Name";
            ddlEditSubject.DataValueField = "Id";
            ddlEditSubject.DataBind();
            ddlEditSubject.Items.Insert(0, new ListItem("Select One", "0"));
        }

        protected void bindGridview()
        {
            var q = (from g in db.GroupLINQs
                     join a in db.AccountLINQs
                     on g.LeaderUserId equals a.UserId
                     join s in db.SubjectLINQs
                     on g.SubjectId equals s.Id
                     where
                     (g.Name.Contains(txtSearch.Text.Trim()))
                     select new
                     {
                         Id = g.Id,
                         Name = g.Name,
                         FullName = a.LastName + " ," + a.FirstName + " " + a.MiddleName,
                         Subject = s.Name,
                         YearFrom = s.YearFrom,
                         YearTo = s.YearTo,
                         Sem = s.Sem
                     }).ToList();

            gvGroups.DataSource = q;
            gvGroups.DataBind();

            txtSearch.Focus();
        }

        protected void bindDropdown()
        {
            //get users who dont have a group
            var q = (from a in db.AccountLINQs
                     join m in db.MembershipLINQs
                     on a.UserId equals m.UserId
                     join u in db.Users
                     on m.UserId equals u.UserId
                     join usr in db.UsersInRoles
                     on u.UserId equals usr.UserId
                     join r in db.Roles
                     on usr.RoleId equals r.RoleId
                     where
                     (r.RoleName == "Student")
                     //(a.GroupId == 0)
                     select new
                     {
                         Id = a.UserId,
                         FullName = a.LastName + ", " + a.FirstName + " " + a.MiddleName
                     }).ToList();

            ddlGroupLeader.DataSource = q;
            ddlGroupLeader.DataTextField = "FullName";
            ddlGroupLeader.DataValueField = "Id";
            ddlGroupLeader.DataBind();
            ddlGroupLeader.Items.Insert(0, new ListItem("Select One", "0"));
        }

        protected void bindEditDropdown(Guid myUserId)
        {
            var q = (from a in db.AccountLINQs
                     join m in db.MembershipLINQs
                     on a.UserId equals m.UserId
                     join u in db.Users
                     on m.UserId equals u.UserId
                     join usr in db.UsersInRoles
                     on u.UserId equals usr.UserId
                     join r in db.Roles
                     on usr.RoleId equals r.RoleId
                     where
                     (
                     //a.GroupId == 0 ||
                     a.UserId == myUserId) &&
                     r.RoleName == "Student"
                     select new
                     {
                         Id = a.UserId,
                         FullName = a.LastName + ", " + a.FirstName + " " + a.MiddleName
                     }).ToList();

            ddlEditGroupLeader.DataSource = q;
            ddlEditGroupLeader.DataTextField = "FullName";
            ddlEditGroupLeader.DataValueField = "Id";
            ddlEditGroupLeader.DataBind();
            ddlEditGroupLeader.Items.Insert(0, new ListItem("Select One", "0"));
        }
   
    }
}