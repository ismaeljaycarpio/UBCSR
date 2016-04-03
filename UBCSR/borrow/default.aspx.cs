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
                lblTitle.Text = "My Borrowed List";
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
                //approvals
            }
            else if(User.IsInRole("CSR Head"))
            {
                //approvals
            }
            else
            {
                //admin
                lblTitle.Text = "Admin - Borrowed List";
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

        }

        protected void gvBorrow_Sorting(object sender, GridViewSortEventArgs e)
        {

        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/borrow/reserve.aspx");
        }
    }
}