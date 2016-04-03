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
                     where g.Name.Contains(txtSearch.Text.Trim()) ||
                     g.YearFrom.Contains(txtSearch.Text.Trim()) ||
                     g.YearTo.Contains(txtSearch.Text.Trim()) ||
                     g.Sem.Contains(txtSearch.Text.Trim())
                     select g).ToList();

            gvGroups.DataSource = q;
            gvGroups.DataBind();

            txtSearch.Focus();
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

        protected void gvGroups_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

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

            db.GroupLINQs.InsertOnSubmit(g);

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