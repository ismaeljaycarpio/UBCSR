using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UBCSR.reserve
{
    public partial class groupmember : System.Web.UI.Page
    {
        CSRContextDataContext db = new CSRContextDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.gvTeam.DataBind();
        }

        protected void gvTeam_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName.Equals("addMembers"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string rowId = ((Label)gvTeam.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;

                var query = (from acc in db.AccountLINQs
                             join gm in db.GroupMembers
                             on acc.UserId equals gm.UserId
                             join g in db.GroupLINQs
                             on gm.GroupId equals g.Id
                             where
                             (g.Id == Convert.ToInt32(rowId))
                             select new
                             {
                                 GroupId = g.Id,
                                 GroupName = g.Name,
                                 FullName = acc.LastName + " , " + acc.FirstName + " " + acc.MiddleName
                             }).FirstOrDefault();

                lblGroupId.Text = query.GroupId.ToString();
                txtGroupName.Text = query.GroupName;
                txtGroupLeader.Text = query.FullName;

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
                        (
                            //(gm.GroupId != query.GroupId) &&
                        (a.UserId != Guid.Parse(Membership.GetUser().ProviderUserKey.ToString())) &&
                        (r.RoleName == "Student")
                        )
                        select new
                        {
                            UserId = a.UserId,
                            FullName = a.LastName + " , " + a.FirstName + " " + a.MiddleName
                        };

                gvMembers.DataSource = q.ToList();
                gvMembers.DataBind();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#addMembersModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
            else if(e.CommandName.Equals("editMembers"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string rowId = ((Label)gvTeam.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;

                var query = (from acc in db.AccountLINQs
                             join gm in db.GroupMembers
                             on acc.UserId equals gm.UserId
                             join g in db.GroupLINQs
                             on gm.GroupId equals g.Id
                             where
                             (g.Id == Convert.ToInt32(rowId))
                             select new
                             {
                                 GroupId = g.Id,
                                 GroupName = g.Name,
                                 FullName = acc.LastName + " , " + acc.FirstName + " " + acc.MiddleName
                             }).FirstOrDefault();

                lblEditGroupId.Text = query.GroupId.ToString();
                txtEditGroupName.Text = query.GroupName;
                txtEditGroupLeader.Text = query.FullName;

                //load members
                var q = from a in db.AccountLINQs
                        join gm in db.GroupMembers
                        on a.UserId equals gm.UserId
                        where
                        (
                        gm.GroupId == query.GroupId &&
                        a.UserId != Guid.Parse(Membership.GetUser().ProviderUserKey.ToString())
                        )
                        select new
                        {
                            UserId = a.UserId,
                            FullName = a.LastName + " , " + a.FirstName + " " + a.MiddleName
                        };

                gvEditMembers.DataSource = q.ToList();
                gvEditMembers.DataBind();

                //set chkbox
                foreach (GridViewRow row in gvEditMembers.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox chk = (CheckBox)row.FindControl("chkRow");
                        chk.Checked = true;
                    }
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#editMembersModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {

        }

        protected void btnAddGroup_Click(object sender, EventArgs e)
        {
            int groupId = Convert.ToInt32(lblGroupId.Text);

            foreach(GridViewRow row in gvMembers.Rows)
            {
                if(row.RowType == DataControlRowType.DataRow)
                {
                    //chk if checked
                    if (((CheckBox)row.FindControl("chkRow")).Checked == true)
                    {
                        string rowId = ((Label)row.FindControl("lblGroupId")).Text;
                        Guid userId = Guid.Parse(rowId);

                        var user = (from a in db.AccountLINQs
                                    where a.UserId == userId
                                    select a).FirstOrDefault();

                        
                        //add to GroupMember
                        GroupMember gm = new GroupMember();
                        gm.GroupId = groupId;
                        gm.UserId = user.UserId;

                        db.GroupMembers.InsertOnSubmit(gm);
                        db.SubmitChanges();
                    }
                }
            }

            this.gvTeam.DataBind();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addMembersModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }

        protected void btnEditGroup_Click(object sender, EventArgs e)
        {
            int groupId = Convert.ToInt32(lblEditGroupId.Text);

            //clear group in GroupMember
            var q = (from gm in db.GroupMembers
                     where gm.GroupId == groupId
                     select gm).ToList();
            db.GroupMembers.DeleteAllOnSubmit(q);
            db.SubmitChanges();

            foreach (GridViewRow row in gvEditMembers.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    //chk if checked
                    if (((CheckBox)row.FindControl("chkRow")).Checked == false)
                    {
                        string rowId = ((Label)row.FindControl("lblEditGroupId")).Text;
                        Guid userId = Guid.Parse(rowId);

                        var user = (from a in db.AccountLINQs
                                    where a.UserId == userId
                                    select a).FirstOrDefault();

                        //re-add to group member
                        GroupMember gm = new GroupMember();
                        gm.GroupId = groupId;
                        gm.UserId = user.UserId;

                        db.GroupMembers.InsertOnSubmit(gm);
                        db.SubmitChanges();
                    }
                }
            }

            this.gvTeam.DataBind();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#editMembersModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }

        protected void GroupDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            string strSearch = txtSearch.Text;

            var q = (from g in db.GroupLINQs
                     join s in db.SubjectLINQs
                     on g.SubjectId equals s.Id
                     join sec in db.Sections
                     on s.SectionId equals sec.Id
                     where
                     (g.Name.Contains(strSearch) || s.Code.Contains(strSearch) || s.Name.Contains(strSearch) || s.Sem.Contains(strSearch) || sec.Section1.Contains(strSearch))
                     select new
                     {
                         Id = g.Id,
                         GroupName = g.Name,
                         Subject = s.Name,
                         Section = sec.Section1,
                         YearFrom = s.YearFrom,
                         YearTo = s.YearTo,
                         Sem = s.Sem
                     }).ToList();

            e.Result = q;
            txtSearch.Focus();
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#createModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddShowModalScript", sb.ToString(), false);
        }

        protected void btnCreateGroup_Click(object sender, EventArgs e)
        {

        }

        protected void ddlCreateSubject_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}