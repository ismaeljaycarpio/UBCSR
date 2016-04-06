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
                bindGridview();
            }
        }

        protected void bindGridview()
        {
            var q = (from g in db.GroupLINQs
                     where g.LeaderUserId == Guid.Parse(Membership.GetUser().ProviderUserKey.ToString())
                     select g).ToList();

            gvTeam.DataSource = q;
            gvTeam.DataBind();

            txtSearch.Focus();

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bindGridview();
        }

        protected void gvTeam_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName.Equals("editRecord"))
            {
                string rowId = ((Label)gvTeam.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;

                var query = (from acc in db.AccountLINQs
                            join g in db.GroupLINQs
                            on acc.GroupId equals g.Id
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

                //load accnts to grid
                var q = from acc in db.AccountLINQs
                        where (acc.GroupId == 0) &&
                        (acc.UserId != Guid.Parse(Membership.GetUser().ProviderUserKey.ToString()))
                        select new
                        {
                            UserId = acc.UserId,
                            FullName = acc.LastName + " , " + acc.FirstName + " " + acc.MiddleName
                        };

                gvMembers.DataSource = q.ToList();
                gvMembers.DataBind();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#updateModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
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

        }

        protected void btnAddGroup_Click(object sender, EventArgs e)
        {
            foreach(GridViewRow row in gvMembers.Rows)
            {
                if(row.RowType == DataControlRowType.DataRow)
                {
                    //chk if checked
                    if (((CheckBox)row.FindControl("chkRow")).Checked == true)
                    {
                        string rowId = ((Label)row.FindControl("lblRowId")).Text;
                        Guid userId = Guid.Parse(rowId);

                        var user = (from a in db.AccountLINQs
                                    where a.UserId == userId
                                    select a).FirstOrDefault();

                        user.GroupId = Convert.ToInt32(lblGroupId.Text);
                        db.SubmitChanges();
                    }
                }
            }

            bindGridview();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#updateModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }
    }
}