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
                Javascript.ShowModal(this, this, "updateModal");
            }
            else if (e.CommandName.Equals("deleteRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string rowId = ((Label)gvSection.Rows[index].FindControl("lblRowId")).Text;
                hfDeleteId.Value = rowId;
                Javascript.ShowModal(this, this, "deleteModal");
            }
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            Javascript.ShowModal(this, this, "addModal");
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
                Javascript.HideModal(this, this, "addModal");
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
            Javascript.HideModal(this, this, "updateModal");
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
            Javascript.HideModal(this, this, "deleteModal");
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