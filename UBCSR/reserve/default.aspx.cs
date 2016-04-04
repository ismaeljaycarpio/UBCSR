using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace UBCSR.borrow
{
    public partial class _default : System.Web.UI.Page
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
            if(User.IsInRole("Student"))
            {
                lblTitle.Text = "Reserved List by your Instructor - Only Group Leaders can view the list";
                var q = from r in db.Reservations
                        where r.ApprovalStatus == "Approved"
                        select r;

                gvBorrow.DataSource = q.ToList();
                gvBorrow.DataBind();

                //hide delete button
                gvBorrow.Columns[10].Visible = false;
                gvBorrow.Columns[9].Visible = false;
                btnOpenModal.Visible = false;
            }
            else if(User.IsInRole("Instructor"))
            {
                lblTitle.Text = "My Reserved List";
                var q = from r in db.Reservations
                        select r;

                gvBorrow.DataSource = q.ToList();
                gvBorrow.DataBind();
            }
            else if(User.IsInRole("Student Assistant"))
            {
                lblTitle.Text = "Approved List";
                var q = from r in db.Reservations
                        where r.ApprovalStatus == "Approved"
                        select r;

                gvBorrow.DataSource = q.ToList();
                gvBorrow.DataBind();
            }
            else if(User.IsInRole("CSR Head"))
            {
                lblTitle.Text = "Approval List";
                var q = from r in db.Reservations
                        where (r.ApprovalStatus == "Pending" ||
                        r.ApprovalStatus == "Disapproved")
                        select r;

                gvBorrow.DataSource = q.ToList();
                gvBorrow.DataBind();
            }
            else
            {
                //admin
                lblTitle.Text = "Admin - Reserved List";
                var q = from r in db.Reservations
                        select r;

                gvBorrow.DataSource = q.ToList();
                gvBorrow.DataBind();
            }

            txtSearch.Focus();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bindGridview();
        }

        protected void gvBorrow_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected void gvBorrow_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBorrow.PageIndex = e.NewPageIndex;
            bindGridview();
        }

        protected void gvBorrow_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName.Equals("editRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int resId = (int)gvBorrow.DataKeys[index].Value;

                Response.Redirect("~/reserve/editreserve.aspx?resId=" + resId.ToString());
            }
            else if(e.CommandName.Equals("deleteRecord"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                hfDeleteId.Value = ((Label)gvBorrow.Rows[index].FindControl("lblRowId")).Text;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#deleteModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
        }

        protected void gvBorrow_Sorting(object sender, GridViewSortEventArgs e)
        {

        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/reserve/reserve.aspx");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var q = (from r in db.Reservations
                     where r.Id == Convert.ToInt32(hfDeleteId.Value)
                     select r).FirstOrDefault();

            db.Reservations.DeleteOnSubmit(q);
            db.SubmitChanges();

            bindGridview();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#deleteModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }
    }
}