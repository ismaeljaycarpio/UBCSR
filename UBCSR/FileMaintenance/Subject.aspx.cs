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
                bindDropdown();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.gvSubjects.DataBind();
        }

        protected void gvSubjects_RowCommand(object sender, GridViewCommandEventArgs e)
        {         
            if(e.CommandName.Equals("editRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
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
                ddlEditSection.SelectedValue = q.SectionId.ToString();

                Javascript.ShowModal(this, this, "updateModal");
            }
            else if(e.CommandName.Equals("deleteRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                hfDeleteId.Value = gvSubjects.DataKeys[index].Value.ToString();
                Javascript.ShowModal(this, this, "deleteModal");
            }        
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            Javascript.ShowModal(this, this, "addModal");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var dup = (from sub in db.SubjectLINQs
                       where 
                       (sub.Code == txtAddCode.Text) &&
                       (sub.SectionId == Convert.ToInt32(ddlAddSection.SelectedValue))
                       select sub).ToList();

            if(dup.Count > 0)
            {
                lblDuplicateRecords.Text = "Duplicate entry exist";
            }
            else
            {
                SubjectLINQ s = new SubjectLINQ();
                s.Code = txtAddCode.Text;
                s.Name = txtAddSubject.Text;
                s.YearFrom = Convert.ToInt32(txtAddYearFrom.Text);
                s.YearTo = Convert.ToInt32(txtAddYearTo.Text);
                s.Sem = ddlAddSemester.SelectedValue;
                s.SectionId = Convert.ToInt32(ddlAddSection.SelectedValue);

                db.SubjectLINQs.InsertOnSubmit(s);
                db.SubmitChanges();

                this.gvSubjects.DataBind();
                Javascript.HideModal(this, this, "addModal");
            }
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
            q.SectionId = Convert.ToInt32(ddlEditSection.SelectedValue);

            db.SubmitChanges();

            this.gvSubjects.DataBind();
            Javascript.HideModal(this, this, "updateModal");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var q = (from s in db.SubjectLINQs
                     where s.Id == Convert.ToInt32(hfDeleteId.Value)
                     select s).FirstOrDefault();
            
            db.SubjectLINQs.DeleteOnSubmit(q);
            db.SubmitChanges();

            this.gvSubjects.DataBind();
            Javascript.HideModal(this, this, "deleteModal");
        }

        protected void bindDropdown()
        {
            var q = (from s in db.Sections
                     select s).ToList();

            ddlAddSection.DataSource = q;
            ddlAddSection.DataTextField = "Section1";
            ddlAddSection.DataValueField = "Id";
            ddlAddSection.DataBind();
            ddlAddSection.Items.Insert(0, new ListItem("-- Select Section --", "0"));

            ddlEditSection.DataSource = q;
            ddlEditSection.DataTextField = "Section1";
            ddlEditSection.DataValueField = "Id";
            ddlEditSection.DataBind();
            ddlEditSection.Items.Insert(0, new ListItem("-- Select Section --", "0"));

        }

        protected void SubjectDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var q = (from sub in db.SubjectLINQs
                     join sec in db.Sections
                     on sub.SectionId equals sec.Id
                     where
                     (sub.Code.Contains(txtSearch.Text) ||
                     sub.Name.Contains(txtSearch.Text) ||
                     sub.Sem.Contains(txtSearch.Text) ||
                     sec.Section1.Contains(txtSearch.Text)
                     )
                     select new
                     {
                         Id = sub.Id,
                         SubjectCode = sub.Code,
                         Subject = sub.Name,
                         YearFrom = sub.YearFrom,
                         YearTo = sub.YearTo,
                         Sem = sub.Sem,
                         Section = sec.Section1
                     }).ToList();

            e.Result = q;
            txtSearch.Focus();
        }
    }
}