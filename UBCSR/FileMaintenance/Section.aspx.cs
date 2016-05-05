using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UBCSR.FileMaintenance
{
    public partial class Section : System.Web.UI.Page
    {
        CSRContextDataContext db = new CSRContextDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.gvSection.DataBind();
        }

        protected void gvSection_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string rowId = ((Label)gvSection.Rows[index].FindControl("lblRowId")).Text;
                lblEditId.Text = rowId;

                //load info
                var q = (from s in db.Sections
                         where s.Id == Convert.ToInt32(rowId)
                         select s).FirstOrDefault();

                txtEditSection.Text = q.Section1;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#updateModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditShowModalScript", sb.ToString(), false);
            }

            else if (e.CommandName.Equals("deleteRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string rowId = ((Label)gvSection.Rows[index].FindControl("lblRowId")).Text;
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
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideShowModalScript", sb.ToString(), false);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var secs = (from s in db.Sections
                        where s.Section1 == txtAddSection.Text.Trim()
                        select s).ToList();

            if(secs.Count > 0)
            {
                lblDuplicateRecords.Text = "Section already exists";
            }
            else
            {
                UBCSR.Section sec = new UBCSR.Section();
                sec.Section1 = txtAddSection.Text;
                db.Sections.InsertOnSubmit(sec);
                db.SubmitChanges();

                this.gvSection.DataBind();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#addModal').modal('hide');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideShowModalScript", sb.ToString(), false);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            int sectionId = Convert.ToInt32(lblEditId.Text);
            var q = (from s in db.Sections
                     where s.Id == sectionId
                     select s).FirstOrDefault();

            q.Section1 = txtEditSection.Text;
            db.SubmitChanges();

            this.gvSection.DataBind();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#updateModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideShowModalScript", sb.ToString(), false);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int sectionId = Convert.ToInt32(hfDeleteId.Value);
            var q = (from s in db.Sections
                     where s.Id == sectionId
                     select s).FirstOrDefault();

            db.Sections.DeleteOnSubmit(q);
            db.SubmitChanges();

            this.gvSection.DataBind();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteHideModalScript", sb.ToString(), false);
        }

        protected void SectionDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            string strSearch = txtSearch.Text;

            var q = (from s in db.Sections
                     where
                     s.Section1.Contains(strSearch)
                     select s).ToList();

            e.Result = q;
            txtSearch.Focus();
        }
    }
}