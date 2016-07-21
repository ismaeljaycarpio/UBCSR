using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
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
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            Javascript.ShowModal(this, this, "addModal");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var q = (from g in db.GroupLINQs
                     where g.Id == Convert.ToInt32(hfDeleteId.Value)
                     select g).FirstOrDefault();

            db.GroupLINQs.DeleteOnSubmit(q);

            db.SubmitChanges();

            bindGridview();
            Javascript.HideModal(this, this, "deleteModal");
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            bindGridview();
            Javascript.HideModal(this, this, "updateModal");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            GroupLINQ g = new GroupLINQ();
            g.Name = txtAddGroup.Text;
            //g.LeaderUserId = Guid.Parse(ddlGroupLeader.SelectedValue.ToString());
            //g.SubjectId = Convert.ToInt32(ddlAddSubject.SelectedValue);

            db.GroupLINQs.InsertOnSubmit(g);

            db.SubmitChanges();

            bindGridview();
            Javascript.HideModal(this, this, "addModal");
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

                //fill leader drop down
                //load users that dont belong on this subject
                var user = (from a in db.AccountLINQs
                           join gm in db.GroupMembers
                           on a.UserId equals gm.UserId
                           into JoinedAccountGroupMember
                           from gm in JoinedAccountGroupMember.DefaultIfEmpty()
                           join g in db.GroupLINQs
                           on gm.GroupId equals g.Id
                           into JoinedGroupMemberGroup
                           from g in JoinedGroupMemberGroup.DefaultIfEmpty()
                           join uir in db.UsersInRoles
                           on a.UserId equals uir.UserId
                           into JoinedGroupUserInRoles
                           from c in JoinedGroupUserInRoles.DefaultIfEmpty()
                           join r in db.Roles
                           on c.RoleId equals r.RoleId
                           into JoinedUserInRolesToRoles
                           from d in JoinedUserInRolesToRoles.DefaultIfEmpty()
                           where
                            (
                            d.RoleName == "Student"
                            )
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

        protected void ddlGroupLeader_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindAddMembers(Guid.Parse(ddlGroupLeader.SelectedValue));
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

        protected void bindAddMembers(Guid selectedLeaderUserId)
        {
            //load accnts that dont belong to this group
            //dont include self
            var q = from a in db.AccountLINQs
                    join gm in db.GroupMembers
                    on a.UserId equals gm.UserId
                    into JoinedAccountGroupMember
                    from gm in JoinedAccountGroupMember.DefaultIfEmpty()
                    join m in db.MembershipLINQs
                    on a.UserId equals m.UserId
                    join u in db.Users
                    on m.UserId equals u.UserId
                    join usr in db.UsersInRoles
                    on u.UserId equals usr.UserId
                    join r in db.Roles
                    on usr.RoleId equals r.RoleId
                    where
                    (a.UserId != Guid.Parse(Membership.GetUser().ProviderUserKey.ToString())) &&
                    (a.UserId != selectedLeaderUserId) &&
                    (r.RoleName == "Student")
                    select new
                    {
                        UserId = a.UserId,
                        FullName = a.LastName + " , " + a.FirstName + " " + a.MiddleName
                    };

            gvMembers.DataSource = q.ToList();
            gvMembers.DataBind();
        }
   
    }
}