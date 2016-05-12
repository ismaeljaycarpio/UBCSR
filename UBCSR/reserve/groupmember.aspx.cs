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
                fillReservation();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.gvTeam.DataBind();
        }

        protected void gvTeam_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName.Equals("editRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string rowId = ((Label)gvTeam.Rows[index].FindControl("lblRowId")).Text;
            }
            else if(e.CommandName.Equals("addMembers"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string rowId = ((Label)gvTeam.Rows[index].FindControl("lblRowId")).Text;

                var query = (from g in db.GroupLINQs
                             where
                             (g.Id == Convert.ToInt32(rowId))
                             select new
                             {
                                 GroupId = g.Id,
                                 GroupName = g.Name,
                                 ReservationId = g.ReservationId
                             }).FirstOrDefault();

                lblGroupId.Text = query.GroupId.ToString();
                txtGroupName.Text = query.GroupName;
                ddlAddReservation.SelectedValue = query.ReservationId.ToString();

                //load accnts that dont belong to this group
                //dont include self
                var q = from a in db.AccountLINQs
                        join gm in db.GroupMembers
                        on a.UserId equals gm.UserId
                        into JoinedAccountGroupMember
                        from agm in JoinedAccountGroupMember.DefaultIfEmpty()
                        join m in db.MembershipLINQs
                        on agm.UserId equals m.UserId
                        into JoinedAccountMembership
                        from mem in JoinedAccountMembership.DefaultIfEmpty()
                        join u in db.Users
                        on mem.UserId equals u.UserId
                        into JoinedMembershipUser
                        from ussr in JoinedMembershipUser.DefaultIfEmpty()
                        join usr in db.UsersInRoles
                        on ussr.UserId equals usr.UserId
                        join r in db.Roles
                        on usr.RoleId equals r.RoleId
                        into JoinedUserRoles
                        from uirr in JoinedUserRoles.DefaultIfEmpty()
                        where
                        (a.UserId != Guid.Parse(Membership.GetUser().ProviderUserKey.ToString())) &&
                        (uirr.RoleName == "Student") &&
                        (agm.GroupId != query.GroupId)
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

                var query = (from g in db.GroupLINQs
                             where
                             (g.Id == Convert.ToInt32(rowId))
                             select new
                             {
                                 GroupId = g.Id,
                                 GroupName = g.Name,
                                 ReservationId = g.ReservationId
                             }).FirstOrDefault();

                lblEditGroupId.Text = query.GroupId.ToString();
                txtEditGroupName.Text = query.GroupName;
                ddlEditReservation.SelectedValue = query.ReservationId.ToString();

                //load members
                var q = from a in db.AccountLINQs
                        join gm in db.GroupMembers
                        on a.UserId equals gm.UserId
                        where
                        (
                        gm.GroupId == query.GroupId
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
                        chk.Checked = false;
                    }
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#editMembersModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
            else if(e.CommandName.Equals("deleteRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string rowId = ((Label)gvTeam.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#deleteModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var q = (from g in db.GroupLINQs
                     where g.Id == Convert.ToInt32(hfDeleteId.Value)
                     select g).FirstOrDefault();

            db.GroupLINQs.DeleteOnSubmit(q);
            db.SubmitChanges();

            this.gvTeam.DataBind();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
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

            if(User.IsInRole("Admin"))
            {
                var q = (from g in db.GroupLINQs
                         join r in db.Reservations
                         on g.ReservationId equals r.Id
                         join sub in db.SubjectLINQs
                         on r.SubjectId equals sub.Id
                         join sec in db.Sections
                         on sub.SectionId equals sec.Id
                         where
                         (g.Name.Contains(strSearch) ||
                         sub.Code.Contains(strSearch) ||
                         sub.Name.Contains(strSearch) ||
                         sub.Sem.Contains(strSearch) ||
                         sec.Section1.Contains(strSearch))
                         select new
                         {
                             Id = g.Id,
                             GroupName = g.Name,
                             Subject = sub.Name,
                             Section = sec.Section1,
                             YearFrom = sub.YearFrom,
                             YearTo = sub.YearTo,
                             Sem = sub.Sem
                         }).ToList();

                e.Result = q;
            }
            else
            {
                var q = (from g in db.GroupLINQs
                         join r in db.Reservations
                         on g.ReservationId equals r.Id
                         join sub in db.SubjectLINQs
                         on r.SubjectId equals sub.Id
                         join sec in db.Sections
                         on sub.SectionId equals sec.Id
                         where
                         (g.Name.Contains(strSearch) ||
                         sub.Code.Contains(strSearch) ||
                         sub.Name.Contains(strSearch) ||
                         sub.Sem.Contains(strSearch) ||
                         sec.Section1.Contains(strSearch)) &&
                         (g.CreatedBy == Guid.Parse(Membership.GetUser().ProviderUserKey.ToString()))
                         select new
                         {
                             Id = g.Id,
                             GroupName = g.Name,
                             Subject = sub.Name,
                             Section = sec.Section1,
                             YearFrom = sub.YearFrom,
                             YearTo = sub.YearTo,
                             Sem = sub.Sem
                         }).ToList();

                e.Result = q;
            }
            
            txtSearch.Focus();
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            //fill gv
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
                    (r.RoleName == "Student")
                    select new
                    {
                        UserId = a.UserId,
                        FullName = a.LastName + " , " + a.FirstName + " " + a.MiddleName
                    };

            gvCreateMembers.DataSource = q.ToList();
            gvCreateMembers.DataBind();

            //fill dropdown
            fillCreateDropdown();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#createModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddShowModalScript", sb.ToString(), false);
        }

        protected void btnCreateGroup_Click(object sender, EventArgs e)
        {
            //add to Group table
            GroupLINQ g = new GroupLINQ();
            g.Name = txtCreateGroupName.Text;
            g.ReservationId = Convert.ToInt32(ddlCreateReservation.SelectedValue);
            g.Status = "Joined";
            g.JoinedDate = DateTime.Now;
            g.HasBreakage = false;
            g.CreatedBy = Guid.Parse(Membership.GetUser().ProviderUserKey.ToString());
            g.IsReturned = false;

            db.GroupLINQs.InsertOnSubmit(g);
            db.SubmitChanges();

            int groupId = g.Id;
            int reservationId = Convert.ToInt32(g.ReservationId);

            foreach (GridViewRow row in gvCreateMembers.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    //chk if checked
                    if (((CheckBox)row.FindControl("chkRow")).Checked == true)
                    {
                        string rowId = ((Label)row.FindControl("lblUserId")).Text;
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
           
            var reservationItems = (from ri in db.ReservationItems
                                    where ri.ReservationId == reservationId
                                    select ri).ToList();

            foreach(var resItem in reservationItems)
            {
                GroupItem gi = new GroupItem();
                gi.GroupId = groupId;
                gi.InventoryId = resItem.InventoryId;
                gi.BorrowedQuantity = resItem.QuantityByGroup;
                gi.Breakage = 0;
                gi.HasBreakage = false;
                gi.ReturnedQuantity = 0;
                
                db.GroupItems.InsertOnSubmit(gi);
                db.SubmitChanges();
            }

            this.gvTeam.DataBind();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#createModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }

        protected void fillReservation()
        {
            var reservations = (from r in db.Reservations
                               join sub in db.SubjectLINQs
                               on r.SubjectId equals sub.Id
                               where
                               (r.ApprovalStatus == "Approved")
                               select new
                               {
                                   Id = r.Id,
                                   Name = "Subject: " + sub.Name + " / Expirement No: " + r.ExperimentNo + " / Labroom: " + r.LabRoom 
                               }).ToList();

            ddlAddReservation.DataSource = reservations;
            ddlAddReservation.DataTextField = "Name";
            ddlAddReservation.DataValueField = "Id";
            ddlAddReservation.DataBind();
            ddlAddReservation.Items.Insert(0, new ListItem("-- Select Reservation --", "0"));

            ddlEditReservation.DataSource = reservations;
            ddlEditReservation.DataTextField = "Name";
            ddlEditReservation.DataValueField = "Id";
            ddlEditReservation.DataBind();
            ddlEditReservation.Items.Insert(0, new ListItem("-- Select Reservation --", "0"));
        }

        protected void fillCreateDropdown()
        {
            var reservations = (from r in db.Reservations
                                join sub in db.SubjectLINQs
                                on r.SubjectId equals sub.Id
                                where
                                (r.ApprovalStatus == "Approved")
                                select new
                                {
                                    Id = r.Id,
                                    Name = "Subject: " + sub.Name + " / Expirement No: " + r.ExperimentNo + " / Labroom: " + r.LabRoom
                                }).ToList();

            ddlCreateReservation.DataSource = reservations;
            ddlCreateReservation.DataTextField = "Name";
            ddlCreateReservation.DataValueField = "Id";
            ddlCreateReservation.DataBind();
            ddlCreateReservation.Items.Insert(0, new ListItem("-- Select Reservation --", "0"));
        }
    }
}