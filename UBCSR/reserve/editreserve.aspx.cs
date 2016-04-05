using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UBCSR.reserve
{
    public partial class editreserve : System.Web.UI.Page
    {
        CSRContextDataContext db = new CSRContextDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                if(Request.QueryString["resId"] == null)
                {
                    Response.Redirect("~/reserve/default.aspx");
                }
                else
                {
                    int resId = Convert.ToInt32(Request.QueryString["resId"].ToString());
                    var q = (from r in db.Reservations
                             where r.Id == resId
                             select r).ToList();

                    if(q.Count < 1)
                    {
                        //no record found
                        Response.Redirect("~/reserve/default.aspx");
                    }
                    else
                    {
                        var r = q.FirstOrDefault();

                        txtSubject.Text = r.Subject;
                        txtExpNo.Text = r.ExperimentNo;
                        txtDateNeeded.Text = r.DateFrom.ToString();
                        txtDateNeededTo.Text = r.DateTo.ToString();
                        txtLabRoom.Text = r.LabRoom;
                        hfResId.Value = r.Id.ToString();

                        if(r.IsReleased == true)
                        {
                            txtIsReleased.Text = "Released";
                            txtIsReleased.ForeColor = System.Drawing.Color.DarkOrange;
                            txtIsReleased.Font.Bold = true;
                        }
                        else if(r.IsReturned == true)
                        {
                            txtIsReleased.Text = "Returned";
                            txtIsReleased.ForeColor = System.Drawing.Color.DarkGreen;
                            txtIsReleased.Font.Bold = true;
                        }

                        bindGridview();
                        bindBorrowers();

                        //approval to CSR Head
                        if(!User.IsInRole("CSR Head") && !User.IsInRole("Admin"))
                        {
                            btnApprove.Visible = false;
                            btnDisapprove.Visible = false;
                        }

                        //chk if student
                        if(User.IsInRole("Student"))
                        {
                            disableFields();
                            btnSave.Text = "Ok";
                            btnBorrow.Visible = true;

                            //hide borrow/return button
                            gvBorrowers.Columns[4].Visible = false;
                            gvBorrowers.Columns[5].Visible = false;
                        }


                        //cant edit released reservations
                        if(r.IsReleased == true)
                        {
                            disableFields();
                        }
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate();
            
            if(btnSave.Text != "Ok")
            {
                var q = (from r in db.Reservations
                         where r.Id == Convert.ToInt32(hfResId.Value)
                         select r).FirstOrDefault();

                q.Subject = txtSubject.Text;
                q.ExperimentNo = txtExpNo.Text;
                q.DateFrom = Convert.ToDateTime(txtDateNeeded.Text);
                q.DateTo = Convert.ToDateTime(txtDateNeededTo.Text);
                q.LabRoom = txtLabRoom.Text;

                db.SubmitChanges();

                int resId = q.Id;

                foreach (GridViewRow row in gvInv.Rows)
                {
                    //chk if checkbox is checked
                    if (((CheckBox)row.FindControl("chkRow")).Checked == true)
                    {
                        string quantity = "";
                        int riId = Convert.ToInt32(((Label)row.FindControl("lblRowId")).Text);

                        if ((quantity = ((TextBox)row.FindControl("txtQuantityToBorrow")).Text) != String.Empty)
                        {
                            var r = (from ri in db.ReservationItems
                                     where ri.Id == riId
                                     select ri).FirstOrDefault();

                            r.Quantity = Convert.ToInt32(quantity);

                            db.SubmitChanges();
                        }
                    }
                }
            }
            else
            {
                Response.Redirect("~/reserve/default.aspx");
            }

            Response.Redirect("~/reserve/default.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/reserve/default.aspx");
        }

        protected void gvInv_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void bindGridview()
        {
            var q = from i in db.Items
                    join inv in db.InventoryLINQs
                    on i.Id equals inv.ItemId
                    join ri in db.ReservationItems
                    on inv.Id equals ri.InventoryId
                    join r in db.Reservations
                    on ri.ReservationId equals r.Id
                    where r.Id == Convert.ToInt32(hfResId.Value)
                    select new
                    {
                        Id = ri.Id,
                        Name = i.ItemName,
                        Stocks = inv.Stocks,
                        Quantity = ri.Quantity
                    };

            gvInv.DataSource = q.ToList();
            gvInv.DataBind();
        }

        protected void bindBorrowers()
        {
            var q = from b in db.Borrows
                    join g in db.GroupLINQs
                    on b.GroupId equals g.Id
                    join acc in db.AccountLINQs
                    on g.LeaderUserId equals acc.UserId
                    where b.ReservationId == Convert.ToInt32(Request.QueryString["resId"])
                    select new
                    {
                        Id = b.Id,
                        GroupName = g.Name,
                        GroupLeader = acc.LastName + ", " + acc.FirstName + " " + acc.MiddleName,
                        Status = b.Status
                    };
            gvBorrowers.DataSource = q.ToList();
            gvBorrowers.DataBind();
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            int resId = Convert.ToInt32(Request.QueryString["resId"].ToString());
            var q = (from r in db.Reservations
                     where r.Id == resId
                     select r).FirstOrDefault();

            q.ApprovalStatus = "Approved";

            db.SubmitChanges();

            Response.Redirect("~/reserve/default.aspx");
        }

        protected void btnDisapprove_Click(object sender, EventArgs e)
        {
            int resId = Convert.ToInt32(Request.QueryString["resId"].ToString());
            var q = (from r in db.Reservations
                     where r.Id == resId
                     select r).FirstOrDefault();

            q.ApprovalStatus = "Disapproved";

            db.SubmitChanges();

            Response.Redirect("~/reserve/default.aspx");
        }

        protected void disableFields()
        {
            txtDateNeeded.Enabled = false;
            txtDateNeededTo.Enabled = false;
            txtExpNo.Enabled = false;
            txtLabRoom.Enabled = false;
            txtSubject.Enabled = false;
            gvInv.Enabled = false;
        }

        protected void btnBorrow_Click(object sender, EventArgs e)
        {
            //hook to reservation
            var q = (from b in db.Borrows
                     join g in db.GroupLINQs
                     on b.GroupId equals g.Id
                     where (g.LeaderUserId == Guid.Parse(Membership.GetUser().ProviderUserKey.ToString())) &&
                     (b.ReservationId == Convert.ToInt32(Request.QueryString["resId"]))
                     select new
                     {
                         Id = b.Id
                     }).ToList();

            var group = (from g in db.GroupLINQs
                        where g.LeaderUserId == Guid.Parse(Membership.GetUser().ProviderUserKey.ToString())
                        select g).FirstOrDefault();

            if(q.Count < 1)
            {
                Borrow b = new Borrow();
                b.GroupId = group.Id;
                b.ReservationId = Convert.ToInt32(Request.QueryString["resId"]);
                b.Status = "Joined";
                b.JoinedDate = DateTime.Now;

                db.Borrows.InsertOnSubmit(b);
                db.SubmitChanges();

                Response.Redirect("~/reserve/default.aspx");
            }
            else
            {
                //show alert
                pnlDoublejoin.Visible = true;
            }
        }

        protected void gvBorrowers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("showReturn"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int borId = (int)gvBorrowers.DataKeys[index].Value;

                //load group info
                var q = (from b in db.Borrows
                        join g in db.GroupLINQs
                        on b.GroupId equals g.Id
                        join acc in db.AccountLINQs
                        on g.LeaderUserId equals acc.UserId
                        where (b.ReservationId == Convert.ToInt32(Request.QueryString["resId"])) &&
                        (b.Id == borId)
                        select new
                        {
                            Id = b.Id,
                            GroupName = g.Name,
                            GroupLeader = acc.LastName + ", " + acc.FirstName + " " + acc.MiddleName,
                            Status = b.Status,
                            GroupId = g.Id
                        }).FirstOrDefault();


                lblRowId.Text = q.Id.ToString();
                txtGroupName.Text = q.GroupName;
                txtGroupLeader.Text = q.GroupLeader;

                //load related items and chk if it has breakage/missing
                var items = from i in db.Items
                        join inv in db.InventoryLINQs
                        on i.Id equals inv.ItemId
                        join bi in db.BorrowItems
                        on inv.Id equals bi.InventoryId
                        join b in db.Borrows
                        on bi.BorrowId equals b.Id
                        where bi.BorrowId == q.Id
                        select new
                        {
                            Id = bi.Id,
                            Name = i.ItemName,
                            Stocks = inv.Stocks,
                            BorrowedQuantity = bi.BorrowedQuantity,
                            Breakage = bi.Breakage,
                            Remarks = bi.Remarks
                        };

                gvBreakage.DataSource = items.ToList();
                gvBreakage.DataBind();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#showReturnModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
            else if(e.CommandName.Equals("showBorrow"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int borId = (int)gvBorrowers.DataKeys[index].Value;

                //load group info
                var q = (from b in db.Borrows
                         join g in db.GroupLINQs
                         on b.GroupId equals g.Id
                         join acc in db.AccountLINQs
                         on g.LeaderUserId equals acc.UserId
                         where (b.ReservationId == Convert.ToInt32(Request.QueryString["resId"])) &&
                         (b.Id == borId)
                         select new
                         {
                             Id = b.Id,
                             GroupName = g.Name,
                             GroupLeader = acc.LastName + ", " + acc.FirstName + " " + acc.MiddleName,
                             Status = b.Status,
                             GroupId = g.Id
                         }).FirstOrDefault();


                lblBorrowId.Text = q.Id.ToString();
                txtGroupNameBorrow.Text = q.GroupName;
                txtGroupLeaderBorrow.Text = q.GroupLeader;

                //load related items and chk if it has breakage/missing
                var items = from i in db.Items
                            join inv in db.InventoryLINQs
                            on i.Id equals inv.ItemId
                            join ri in db.ReservationItems
                            on inv.Id equals ri.InventoryId
                            join r in db.Reservations
                            on ri.ReservationId equals r.Id
                            join b in db.Borrows
                            on r.Id equals b.ReservationId
                            join g in db.GroupLINQs
                            on b.GroupId equals g.Id
                            where (r.Id == Convert.ToInt32(hfResId.Value)) &&
                            (b.GroupId == q.GroupId)
                            select new
                            {
                                Id = ri.InventoryId,
                                Name = i.ItemName,
                                Stocks = inv.Stocks,
                                Quantity = ri.Quantity
                            };

                gvBorrow.DataSource = items.ToList();
                gvBorrow.DataBind();


                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#showBorrowModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);

            }
        }

        protected void btnRelease_Click(object sender, EventArgs e)
        {
            int resId = Convert.ToInt32(Request.QueryString["resId"]);

            //set released status - reservation
            foreach(GridViewRow row in gvBorrowers.Rows)
            {
                if(row.RowType == DataControlRowType.DataRow)
                {
                    var q = (from r in db.Reservations
                             where r.Id == resId
                             select r).FirstOrDefault();

                    q.IsReleased = true;
                    q.ReleasedDate = DateTime.Now;
                    db.SubmitChanges();
                }
            }

            //deduct borrowed quantity to inv
            var resItems = (from ri in db.ReservationItems
                     where ri.ReservationId == Convert.ToInt32(Request.QueryString["resId"])
                     select ri).ToList();

                               
            foreach(var item in resItems)
            {
                var updateStocks = (from inv in db.InventoryLINQs
                                    where inv.Id == item.InventoryId
                                    select inv).FirstOrDefault();

                updateStocks.Stocks = (updateStocks.Stocks - item.Quantity);
                db.SubmitChanges();
            }

            Response.Redirect("~/reserve/default.aspx");
        }

        protected void btnConfirmReturn_Click(object sender, EventArgs e)
        {


            bindBorrowers();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#showReturnModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }

        protected void btnConfirmBorrow_Click(object sender, EventArgs e)
        {
            //clear
            var clearList = (from bi in db.BorrowItems
                             where bi.BorrowId == Convert.ToInt32(lblBorrowId.Text)
                             select bi).ToList();
            db.BorrowItems.DeleteAllOnSubmit(clearList);
            db.SubmitChanges();

            //add to borrowitem
            foreach(GridViewRow row in gvBorrow.Rows)
            {
                if(row.RowType == DataControlRowType.DataRow)
                {
                    int inventoryId = Convert.ToInt32(((Label)row.FindControl("lblRowId")).Text);
                    int borrowedQuantity = Convert.ToInt32(((TextBox)row.FindControl("txtQuantity")).Text);

                    BorrowItem bi = new BorrowItem();
                    bi.BorrowId = Convert.ToInt32(lblBorrowId.Text);
                    bi.InventoryId = inventoryId;
                    bi.BorrowedQuantity = borrowedQuantity;
                    bi.Breakage = 0;

                    db.BorrowItems.InsertOnSubmit(bi);
                    db.SubmitChanges();

                    //deduct in Inv
                    var q = (from i in db.InventoryLINQs
                             where i.Id == bi.InventoryId
                             select i).FirstOrDefault();
                    q.Stocks = (q.Stocks - borrowedQuantity);
                    db.SubmitChanges();
                }
            }

            bindGridview();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#showBorrowModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }
    }
}