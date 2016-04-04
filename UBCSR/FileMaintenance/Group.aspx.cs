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
                bindGridview();
            }
        }

        protected void bindGridview()
        {
            var q = (from g in db.GroupLINQs
                     join a in db.AccountLINQs
                     on g.LeaderUserId equals a.UserId
                     where g.Name.Contains(txtSearch.Text.Trim()) ||
                     g.YearFrom.Contains(txtSearch.Text.Trim()) ||
                     g.YearTo.Contains(txtSearch.Text.Trim()) ||
                     g.Sem.Contains(txtSearch.Text.Trim())
                     select new
                     {
                         Id = g.Id,
                         Name = g.Name,
                         FullName = a.LastName + " ," + a.FirstName + " " + a.MiddleName,
                         YearFrom = g.YearFrom,
                         YearTo = g.YearTo,
                         Sem = g.Sem
                     }).ToList();

            gvGroups.DataSource = q;
            gvGroups.DataBind();

            txtSearch.Focus();
        }

        protected void bindDropdonw()
        {
            //doesnt belong to any group yet
            var q = (from a in db.AccountLINQs
                     where a.GroupId == 0
                     select new{
                         Id = a.UserId,
                         FullName = a.LastName + ", " + a.FirstName + " " + a.MiddleName
                     }).ToList();
            
            ddlGroupLeader.DataSource = q;
            ddlGroupLeader.DataTextField = "FullName";
            ddlGroupLeader.DataValueField = "Id";
            ddlGroupLeader.DataBind();         
        }

        protected void bindEditDropdown(Guid myUserId)
        {
            //doesnt belong to any group yet
            var q = (from a in db.AccountLINQs
                     where a.GroupId == 0 ||
                     a.UserId == myUserId
                     select new
                     {
                         Id = a.UserId,
                         FullName = a.LastName + ", " + a.FirstName + " " + a.MiddleName
                     }).ToList();

            ddlEditGroupLeader.DataSource = q;
            ddlEditGroupLeader.DataTextField = "FullName";
            ddlEditGroupLeader.DataValueField = "Id";
            ddlEditGroupLeader.DataBind();
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
                txtEditYearFrom.Text = q.YearFrom;
                txtEditYearTo.Text = q.YearTo;
                ddlEditSemester.SelectedItem.Text = q.Sem;

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
            bindDropdonw();

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
            q.YearFrom = txtEditYearFrom.Text;
            q.YearTo = txtEditYearTo.Text;
            q.Sem = ddlEditSemester.SelectedItem.Text;

            Guid previousLeaderId = Guid.Parse(q.LeaderUserId.ToString());

            q.LeaderUserId = Guid.Parse(ddlEditGroupLeader.SelectedValue.ToString());

            db.SubmitChanges();

            //chk if new leadership
            if(q.LeaderUserId != previousLeaderId)
            {
                //reset previous leader to groupid = 0
                var a = (from ac in db.AccountLINQs
                         where ac.UserId == previousLeaderId
                         select ac).FirstOrDefault();

                a.GroupId = 0;
                db.SubmitChanges();
            }

            //update Account table also
            var acc = (from a in db.AccountLINQs
                     where a.UserId == Guid.Parse(ddlEditGroupLeader.SelectedValue.ToString())
                     select a).FirstOrDefault();

            acc.GroupId = q.Id;
            db.SubmitChanges();

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
            g.YearFrom = txtAddYearFrom.Text;
            g.YearTo = txtAddYearTo.Text;
            g.Sem = ddlAddSemester.SelectedItem.Text;
            g.LeaderUserId = Guid.Parse(ddlGroupLeader.SelectedValue.ToString());

            db.GroupLINQs.InsertOnSubmit(g);

            db.SubmitChanges();

            //update Account table also
            var q = (from a in db.AccountLINQs
                     where a.UserId == Guid.Parse(ddlGroupLeader.SelectedValue.ToString())
                     select a).FirstOrDefault();

            q.GroupId = g.Id;
            db.SubmitChanges();

            bindGridview();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideShowModalScript", sb.ToString(), false);
        }
    }
}