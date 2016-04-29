using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using OfficeOpenXml;
using System.IO;

namespace UBCSR.borrow
{
    public partial class _default : System.Web.UI.Page
    {
        CSRContextDataContext db = new CSRContextDataContext();
        DAL.Report rep = new DAL.Report();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                bindGridview();

                //only instr can create reservation
                if (User.IsInRole("Instructor") || User.IsInRole("Admin"))
                {
                    btnCreateReservation.Visible = true;
                }
            }
        }

        protected void bindGridview()
        {
            if(User.IsInRole("Student"))
            {
                lblTitle.Text = "Reserved List by your Instructor - Only Users that belongs to a group can view the list";

                //chk if user belongs to a group
                Guid myUserId = Guid.Parse(Membership.GetUser().ProviderUserKey.ToString());
                var user = (from a in db.AccountLINQs
                            where a.UserId == myUserId
                            select a).FirstOrDefault();

                if(user !=  null)
                {
                    lblTitle.Text = "Reserved List by your Instructor - Only Group Leaders can view the list";
                    var q = from r in db.Reservations
                            join acc in db.AccountLINQs
                            on r.UserId equals acc.UserId
                            join s in db.SubjectLINQs
                            on r.SubjectId equals s.Id
                            where
                            r.ApprovalStatus == "Approved"
                            select new
                            {
                                Id = r.Id,
                                Name = acc.LastName + " , " + acc.FirstName + " " + acc.MiddleName,
                                Subject = s.Name,
                                ExperimentNo = r.ExperimentNo,
                                DateRequested = r.DateRequested,
                                DateFrom = r.DateFrom,
                                LabRoom = r.LabRoom,
                                ApprovalStatus = r.ApprovalStatus,
                                DateTo = r.DateTo,
                                DisapproveRemarks = r.DisapproveRemarks
                            };

                    gvBorrow.DataSource = q.ToList();
                    gvBorrow.DataBind();

                    //hide delete button
                    gvBorrow.Columns[9].Visible = false;
                    gvBorrow.Columns[10].Visible = false;
                    gvBorrow.Columns[11].Visible = false;
                }
                else
                {
                    gvBorrow.DataSource = null;
                    gvBorrow.DataBind();
                }
            }
            else if(User.IsInRole("Instructor"))
            {
                lblTitle.Text = "Instructor - My Reserved List";
                var q = from r in db.Reservations
                        join acc in db.AccountLINQs
                        on r.UserId equals acc.UserId
                        join s in db.SubjectLINQs
                        on r.SubjectId equals s.Id
                        where r.UserId == Guid.Parse(Membership.GetUser().ProviderUserKey.ToString())
                        select new
                        {
                            Id = r.Id,
                            Name = acc.LastName + " , " + acc.FirstName + " " + acc.MiddleName,
                            Subject = s.Name,
                            ExperimentNo = r.ExperimentNo,
                            DateRequested = r.DateRequested,
                            DateFrom = r.DateFrom,
                            LabRoom = r.LabRoom,
                            ApprovalStatus = r.ApprovalStatus,
                            DateTo = r.DateTo,
                            DisapproveRemarks = r.DisapproveRemarks
                        };

                gvBorrow.DataSource = q.ToList();
                gvBorrow.DataBind();
            }
            else if(User.IsInRole("Student Assistant"))
            {
                lblTitle.Text = "Approved List";
                var q = from r in db.Reservations
                        join acc in db.AccountLINQs
                        on r.UserId equals acc.UserId
                        join s in db.SubjectLINQs
                        on r.SubjectId equals s.Id
                        where
                        (r.ApprovalStatus == "Approved") &&
                        (r.IsReturned == false)
                        select new
                        {
                            Id = r.Id,
                            Name = acc.LastName + " , " + acc.FirstName + " " + acc.MiddleName,
                            Subject = s.Name,
                            ExperimentNo = r.ExperimentNo,
                            DateRequested = r.DateRequested,
                            DateFrom = r.DateFrom,
                            LabRoom = r.LabRoom,
                            ApprovalStatus = r.ApprovalStatus,
                            DateTo = r.DateTo,
                            DisapproveRemarks = r.DisapproveRemarks
                        };

                gvBorrow.DataSource = q.ToList();
                gvBorrow.DataBind();

                //hide delete button
                gvBorrow.Columns[9].Visible = false;
                gvBorrow.Columns[10].Visible = false;
                gvBorrow.Columns[11].Visible = false;
            }
            else if(User.IsInRole("CSR Head"))
            {
                lblTitle.Text = "Pending and Disapproved Reservation List";
                var q = from r in db.Reservations
                        join acc in db.AccountLINQs
                        on r.UserId equals acc.UserId
                        join s in db.SubjectLINQs
                        on r.SubjectId equals s.Id
                        where
                        (r.ApprovalStatus == "Pending")
                        select new
                        {
                            Id = r.Id,
                            Name = acc.LastName + " , " + acc.FirstName + " " + acc.MiddleName,
                            Subject = s.Name,
                            ExperimentNo = r.ExperimentNo,
                            DateRequested = r.DateRequested,
                            DateFrom = r.DateFrom,
                            LabRoom = r.LabRoom,
                            ApprovalStatus = r.ApprovalStatus,
                            DateTo = r.DateTo,
                            DisapproveRemarks = r.DisapproveRemarks
                        };

                gvBorrow.DataSource = q.ToList();
                gvBorrow.DataBind();

                //hide delete btn
                gvBorrow.Columns[10].Visible = false;
                gvBorrow.Columns[11].Visible = false;
            }
            else
            {
                //admin
                lblTitle.Text = "Admin - Reserved List";
                var q = from r in db.Reservations
                        join acc in db.AccountLINQs
                        on r.UserId equals acc.UserId
                        join s in db.SubjectLINQs
                        on r.SubjectId equals s.Id
                        select new
                        {
                            Id = r.Id,
                            Name = acc.LastName + " , " + acc.FirstName + " " + acc.MiddleName,
                            Subject = s.Name,
                            ExperimentNo = r.ExperimentNo,
                            DateRequested = r.DateRequested,
                            DateFrom = r.DateFrom,
                            LabRoom = r.LabRoom,
                            ApprovalStatus = r.ApprovalStatus,
                            DateTo = r.DateTo,
                            DisapproveRemarks = r.DisapproveRemarks
                        };

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

        protected void btnCreateReservation_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/reserve/reserve.aspx");
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var products = rep.searchReservation();
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Reservations");
            var totalCols = products.Columns.Count;
            var totalRows = products.Rows.Count;

            for (var col = 1; col <= totalCols; col++)
            {
                workSheet.Cells[1, col].Value = products.Columns[col - 1].ColumnName;
            }
            for (var row = 1; row <= totalRows; row++)
            {
                for (var col = 0; col < totalCols; col++)
                {
                    workSheet.Cells[row + 1, col + 1].Value = products.Rows[row - 1][col];
                }
            }
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Reservations.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

    }
}