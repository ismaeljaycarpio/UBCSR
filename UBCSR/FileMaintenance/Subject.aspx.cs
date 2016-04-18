using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace UBCSR.FileMaintenance
{
    public partial class Subject : System.Web.UI.Page
    {
        CSRContextDataContext db = new CSRContextDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                bindGridview();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bindGridview();
        }

        protected void gvSubjects_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSubjects.PageIndex = e.NewPageIndex;
            bindGridview();
        }

        protected void gvSubjects_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            if(e.CommandName.Equals("editRecord"))
            {
                int Id = Convert.ToInt32(gvSubjects.DataKeys[index].Value.ToString());
                var q = (from s in db.SubjectLINQs
                         where s.Id == Id
                         select s).FirstOrDefault();

                lblRowId.Text = q.Id.ToString();
                txtEditCode.Text = q.Code;
                txtEditSubject.Text = q.Name;
                txtEditYearFrom.Text = q.YearFrom.ToString();
                txtEditYearTo.Text = q.YearTo.ToString();
                ddlEditSemester.SelectedValue = q.Sem;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#updateModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditShowModalScript", sb.ToString(), false);
            }
            else if(e.CommandName.Equals("deleteRecord"))
            {
                hfDeleteId.Value = gvSubjects.DataKeys[index].Value.ToString();

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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SubjectLINQ s = new SubjectLINQ();
            s.Code = txtAddCode.Text;
            s.Name = txtAddSubject.Text;
            s.YearFrom = Convert.ToInt32(txtAddYearFrom.Text);
            s.YearTo = Convert.ToInt32(txtAddYearTo.Text);
            s.Sem = ddlAddSemester.SelectedValue;

            db.SubjectLINQs.InsertOnSubmit(s);
            db.SubmitChanges();

            bindGridview();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#addModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideShowModalScript", sb.ToString(), false);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var q = (from s in db.SubjectLINQs
                     where s.Id == Convert.ToInt32(lblRowId.Text)
                     select s).FirstOrDefault();

            q.Code = txtEditCode.Text;
            q.Name = txtEditSubject.Text;
            q.YearFrom = Convert.ToInt32(txtEditYearFrom.Text);
            q.YearTo = Convert.ToInt32(txtEditYearTo.Text);
            q.Sem = ddlEditSemester.SelectedValue;

            db.SubmitChanges();

            bindGridview();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#updateModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditHideModalScript", sb.ToString(), false);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var q = (from s in db.SubjectLINQs
                     where s.Id == Convert.ToInt32(hfDeleteId.Value)
                     select s).FirstOrDefault();
            
            db.SubjectLINQs.DeleteOnSubmit(q);
            db.SubmitChanges();

            bindGridview();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteHideModalScript", sb.ToString(), false);
        }

        private void bindGridview()
        {
            var q = (from s in db.SubjectLINQs
                     where
                     (s.Code.Contains(txtSearch.Text) || 
                     s.Name.Contains(txtSearch.Text) ||
                     s.Sem.Contains(txtSearch.Text))
                     select s).ToList();

            gvSubjects.DataSource = q;
            gvSubjects.DataBind();

            txtSearch.Focus();
        }
    }
}