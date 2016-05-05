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

                    //load dropdown
                    bindDropdown();
                    ddlSubject.Items.Insert(0, new ListItem("Select Subject", "0"));

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

                        ddlSubject.SelectedValue = r.SubjectId.ToString();
                        txtExpNo.Text = r.ExperimentNo;
                        txtDateNeeded.Text = r.DateFrom.ToString();
                        txtDateNeededTo.Text = r.DateTo.ToString();
                        txtLabRoom.Text = r.LabRoom;
                        
                        if(r.DisapproveRemarks != String.Empty)
                        {
                            txtDisapprovedRemarks.Text = r.DisapproveRemarks;
                        }
                        else
                        {
                            pnlDisapprove.Visible = false;
                        }

                        //assign Id to control
                        hfResId.Value = r.Id.ToString();

                        bindReserveItems();
                        bindTaggedGroups();
                        bindReleaseGroups();
                        bindReturnedGroups();

                        //show approval buttons
                        if(User.IsInRole("CSR Head") ||
                            User.IsInRole("Admin"))
                        {
                            //chk if approved
                            //to avoid duplication of deduction
                            if(r.ApprovalStatus != "Approved")
                            {
                                btnApprove.Visible = true;
                                btnDisapprove.Visible = true;
                            }
                        }

                        //show update button
                        if(User.IsInRole("Admin") || 
                            User.IsInRole("Instructor"))
                        {
                            btnSave.Visible = true;
                            enableFields();
                        }

                        //can edit -> admin,csr head,student assistant,instructor
                        if(User.IsInRole("Student"))
                        {
                            //btnTagGroup.Visible = true;

                            //hide borrow/return button
                            gvTaggedGroups.Columns[3].Visible = false;
                            gvReleaseGroup.Columns[4].Visible = false;
                        }
                        else
                        {
                            if(!User.IsInRole("Admin") && 
                                !User.IsInRole("Student Assistant"))
                            {
                                //hide borrow/return button
                                gvTaggedGroups.Columns[3].Visible = false;
                                //gvBorrowers.Columns[5].Visible = false;
                            }
                        }
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate("vgPrimaryAdd");
            
            if(Page.IsValid)
            {
                if (btnSave.Text != "Ok")
                {
                    var q = (from r in db.Reservations
                             where r.Id == Convert.ToInt32(hfResId.Value)
                             select r).FirstOrDefault();

                    q.SubjectId = Convert.ToInt32(ddlSubject.SelectedValue);
                    q.ExperimentNo = txtExpNo.Text;
                    q.DateFrom = Convert.ToDateTime(txtDateNeeded.Text);
                    q.DateTo = Convert.ToDateTime(txtDateNeededTo.Text);
                    q.LabRoom = txtLabRoom.Text;

                    db.SubmitChanges();

                    int resId = q.Id;

                    foreach (GridViewRow row in gvReservaItems.Rows)
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
                    Response.Redirect("~/reserve/default.aspx");
                }
                else
                {
                    Response.Redirect("~/reserve/default.aspx");
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/reserve/default.aspx");
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            int resId = Convert.ToInt32(Request.QueryString["resId"].ToString());
            var q = (from r in db.Reservations
                     where r.Id == resId
                     select r).FirstOrDefault();

            q.ApprovalStatus = "Approved";
            q.DisapproveRemarks = String.Empty;
            db.SubmitChanges();

            //deduct in stocks
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

        protected void btnDisapprove_Click(object sender, EventArgs e)
        {
            //open remarks modal
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#disapproveModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }

        //Returning Confirmation
        protected void btnConfirmReturn_Click(object sender, EventArgs e)
        {
            var group = (from g in db.GroupLINQs
                         where g.Id == Convert.ToInt32(lblRowId.Text)
                         select g).FirstOrDefault();

            bool isComplete = true;

            //first time return
            if(group.IsReturned == false)
            {
                //update here:
                foreach (GridViewRow row in gvBreakage.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        int giId = Convert.ToInt32(((Label)row.FindControl("lblRowId")).Text);
                        int inventoryId = Convert.ToInt32(((Label)row.FindControl("lblInventoryId")).Text);
                        int breakageQuantity = Convert.ToInt32(((TextBox)row.FindControl("txtBreakage")).Text);
                        int quantityToBorrow = Convert.ToInt32(((Label)row.FindControl("lblBorrowedQuantity")).Text);
                        int returnedQuantity = Convert.ToInt32(((Label)row.FindControl("lblReturnedQuantity")).Text);
                        string remarks = ((TextBox)row.FindControl("txtRemarks")).Text;

                        var groupItem = (from gi in db.GroupItems
                                         where gi.Id == giId
                                         select gi).FirstOrDefault();

                        groupItem.Breakage = breakageQuantity;
                        groupItem.Remarks = remarks;
                        db.SubmitChanges();

                        //return quantity to Inventory Stocks
                        var inv = (from i in db.InventoryLINQs
                                   where i.Id == groupItem.InventoryId
                                   select i).FirstOrDefault();

                        //chk if it has breakage
                        if (breakageQuantity > 0)
                        {
                            inv.Stocks = (inv.Stocks + (groupItem.BorrowedQuantity - breakageQuantity));
                            groupItem.HasBreakage = true;
                        }
                        else
                        {
                            inv.Stocks = (inv.Stocks + groupItem.BorrowedQuantity);
                            groupItem.HasBreakage = false;
                        }
                        groupItem.ReturnedQuantity = (groupItem.BorrowedQuantity - breakageQuantity);
                        db.SubmitChanges();

                        //chk if it has breakage - change status
                        if (breakageQuantity > 0)
                        {
                            group.HasBreakage = true;
                            isComplete = false;
                        }
                    }
                }
                group.IsReturned = true;
            }
            else
            {
                foreach (GridViewRow row in gvBreakage.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        int giId = Convert.ToInt32(((Label)row.FindControl("lblRowId")).Text);
                        int inventoryId = Convert.ToInt32(((Label)row.FindControl("lblInventoryId")).Text);
                        int breakageQuantity = Convert.ToInt32(((TextBox)row.FindControl("txtBreakage")).Text);
                        int quantityToBorrow = Convert.ToInt32(((Label)row.FindControl("lblBorrowedQuantity")).Text);
                        int returnedQuantity = Convert.ToInt32(((Label)row.FindControl("lblReturnedQuantity")).Text);
                        string remarks = ((TextBox)row.FindControl("txtRemarks")).Text;

                        var groupItem = (from gi in db.GroupItems
                                         where gi.Id == giId
                                         select gi).FirstOrDefault();

                        groupItem.Breakage = breakageQuantity;
                        groupItem.Remarks = remarks;
                        db.SubmitChanges();

                        //return quantity to Inventory Stocks
                        var inv = (from i in db.InventoryLINQs
                                   where i.Id == groupItem.InventoryId
                                   select i).FirstOrDefault();

                        //deduct returned quantity before subtracting again
                        inv.Stocks = (inv.Stocks - returnedQuantity);

                        //chk if it has breakage
                        if (breakageQuantity > 0)
                        {
                            inv.Stocks = (inv.Stocks + (groupItem.BorrowedQuantity - breakageQuantity));
                            groupItem.HasBreakage = true;                           
                        }
                        else
                        {
                            inv.Stocks = (inv.Stocks + groupItem.BorrowedQuantity);
                            groupItem.HasBreakage = false;
                            groupItem.Remarks = String.Empty;
                        }
                        groupItem.ReturnedQuantity = (groupItem.BorrowedQuantity - breakageQuantity);
                        db.SubmitChanges();

                        group.HasBreakage = false;

                        //chk if it has breakage - change status
                        if (breakageQuantity > 0)
                        {
                            group.HasBreakage = true;
                            isComplete = false;
                        }
                    }
                }
                group.IsReturned = true;
            }

            if(isComplete == true)
            {
                group.Status = "Complete";
            }

            db.SubmitChanges();

            bindReserveItems();
            bindTaggedGroups();
            bindReleaseGroups();
            bindReturnedGroups();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#showReturnModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }

        //Viewing Completed Returns - in-case of edits
        protected void btnConfirmCompletedReturn_Click(object sender, EventArgs e)
        {
            var group = (from g in db.GroupLINQs
                         where g.Id == Convert.ToInt32(lblRowId.Text)
                         select g).FirstOrDefault();

            bool isComplete = true;

            //first time return
            if (group.IsReturned == false)
            {
                //update here:
                foreach (GridViewRow row in gvCompleteReturned.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        int giId = Convert.ToInt32(((Label)row.FindControl("lblRowId")).Text);
                        int inventoryId = Convert.ToInt32(((Label)row.FindControl("lblInventoryId")).Text);
                        int breakageQuantity = Convert.ToInt32(((TextBox)row.FindControl("txtBreakage")).Text);
                        int quantityToBorrow = Convert.ToInt32(((Label)row.FindControl("lblBorrowedQuantity")).Text);
                        int returnedQuantity = Convert.ToInt32(((Label)row.FindControl("lblReturnedQuantity")).Text);
                        string remarks = ((TextBox)row.FindControl("txtRemarks")).Text;

                        var groupItem = (from gi in db.GroupItems
                                         where gi.Id == giId
                                         select gi).FirstOrDefault();

                        groupItem.Breakage = breakageQuantity;
                        groupItem.Remarks = remarks;
                        db.SubmitChanges();

                        //return quantity to Inventory Stocks
                        var inv = (from i in db.InventoryLINQs
                                   where i.Id == groupItem.InventoryId
                                   select i).FirstOrDefault();

                        //chk if it has breakage
                        if (breakageQuantity > 0)
                        {
                            inv.Stocks = (inv.Stocks + (groupItem.BorrowedQuantity - breakageQuantity));
                            groupItem.HasBreakage = true;
                        }
                        else
                        {
                            inv.Stocks = (inv.Stocks + groupItem.BorrowedQuantity);
                            groupItem.HasBreakage = false;
                        }
                        groupItem.ReturnedQuantity = (groupItem.BorrowedQuantity - breakageQuantity);
                        db.SubmitChanges();

                        //chk if it has breakage - change status
                        if (breakageQuantity > 0)
                        {
                            group.HasBreakage = true;
                            isComplete = false;
                        }
                    }
                }
                group.IsReturned = true;
            }
            else
            {
                foreach (GridViewRow row in gvCompleteReturned.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        int giId = Convert.ToInt32(((Label)row.FindControl("lblRowId")).Text);
                        int inventoryId = Convert.ToInt32(((Label)row.FindControl("lblInventoryId")).Text);
                        int breakageQuantity = Convert.ToInt32(((TextBox)row.FindControl("txtBreakage")).Text);
                        int quantityToBorrow = Convert.ToInt32(((Label)row.FindControl("lblBorrowedQuantity")).Text);
                        int returnedQuantity = Convert.ToInt32(((Label)row.FindControl("lblReturnedQuantity")).Text);
                        string remarks = ((TextBox)row.FindControl("txtRemarks")).Text;

                        var groupItem = (from gi in db.GroupItems
                                         where gi.Id == giId
                                         select gi).FirstOrDefault();

                        groupItem.Breakage = breakageQuantity;
                        groupItem.Remarks = remarks;
                        db.SubmitChanges();

                        //return quantity to Inventory Stocks
                        var inv = (from i in db.InventoryLINQs
                                   where i.Id == groupItem.InventoryId
                                   select i).FirstOrDefault();

                        //deduct returned quantity before subtracting again
                        inv.Stocks = (inv.Stocks - returnedQuantity);

                        //chk if it has breakage
                        if (breakageQuantity > 0)
                        {
                            inv.Stocks = (inv.Stocks + (groupItem.BorrowedQuantity - breakageQuantity));
                            groupItem.HasBreakage = true;
                        }
                        else
                        {
                            inv.Stocks = (inv.Stocks + groupItem.BorrowedQuantity);
                            groupItem.HasBreakage = false;
                            groupItem.Remarks = String.Empty;
                        }
                        groupItem.ReturnedQuantity = (groupItem.BorrowedQuantity - breakageQuantity);
                        db.SubmitChanges();

                        group.HasBreakage = false;

                        //chk if it has breakage - change status
                        if (breakageQuantity > 0)
                        {
                            group.HasBreakage = true;
                            isComplete = false;
                        }
                    }
                }
                group.IsReturned = true;
            }

            if (isComplete == true)
            {
                group.Status = "Complete";
            }

            db.SubmitChanges();

            bindReserveItems();
            bindTaggedGroups();
            bindReleaseGroups();
            bindReturnedGroups();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#showCompleteReturnedModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }

        //Releasing Confirmation
        protected void btnConfirmBorrow_Click(object sender, EventArgs e)
        {
            //set group to In-Progress
            var group = (from g in db.GroupLINQs
                       where
                       g.Id == Convert.ToInt32(lblGroupId.Text)
                       select g).FirstOrDefault();

            group.Status = "In-Progress";
            db.SubmitChanges();

            //bindReserveItems();
            bindTaggedGroups();
            bindReleaseGroups();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#showBorrowModal').modal('hide');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
        }

        //discontinued - no more function of tagging of groups
        protected void btnTagGroup_Click(object sender, EventArgs e)
        {
            Guid myUserId = Guid.Parse(Membership.GetUser().ProviderUserKey.ToString());

            //chk if user group is tagged in the reservation
            var q = (from g in db.GroupLINQs
                     join gm in db.GroupMembers
                     on g.Id equals gm.GroupId
                     where
                     (g.ReservationId == Convert.ToInt32(Request.QueryString["resId"])) &&
                     (gm.UserId == myUserId)
                     select new
                     {
                         Id = g.Id
                     }).ToList();

            //get user group
            var group = (from g in db.GroupLINQs
                         join gm in db.GroupMembers
                         on g.Id equals gm.GroupId
                         where
                         (gm.UserId == myUserId)
                         select new
                         {
                             GroupId = g.Id
                         }).FirstOrDefault();

            if (q.Count < 1)
            {
                //Borrow b = new Borrow();
                //b.GroupId = group.GroupId;
                //b.ReservationId = Convert.ToInt32(Request.QueryString["resId"]);
                //b.Status = "Joined";
                //b.JoinedDate = DateTime.Now;

                //db.Borrows.InsertOnSubmit(b);
                //db.SubmitChanges();

                ////insert to borrowitem
                //foreach(GridViewRow row in gvReservaItems.Rows)
                //{
                //    if(row.RowType == DataControlRowType.DataRow)
                //    {
                //        int invId = Convert.ToInt32(((Label)row.FindControl("lblInventoryId")).Text);

                //        BorrowItem bi = new BorrowItem();
                //        bi.BorrowId = b.Id;
                //        bi.InventoryId = invId;
                //        bi.BorrowedQuantity = 0;
                //        bi.Breakage = 0;

                //        db.BorrowItems.InsertOnSubmit(bi);
                //        db.SubmitChanges();
                //    }
                //}
                bindTaggedGroups();
                pnlSuccessfullJoin.Visible = true;
            }
            else
            {
                //show alert
                pnlDoublejoin.Visible = true;
                pnlSuccessfullJoin.Visible = false;
            }
        }

        protected void btnConfirmDisapprove_Click(object sender, EventArgs e)
        {
            int resId = Convert.ToInt32(Request.QueryString["resId"].ToString());
            var reservation = (from r in db.Reservations
                     where r.Id == resId
                     select r).FirstOrDefault();

            reservation.ApprovalStatus = "Disapproved";
            reservation.DisapproveRemarks = txtDisapproveRemarks.Text;

            db.SubmitChanges();

            Response.Redirect("~/reserve/default.aspx");
        }

        //Tagged Groups Gridview
        protected void gvBorrowers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("showBorrow"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int groupId = (int)gvTaggedGroups.DataKeys[index].Value;

                //load group info
                var group = (from g in db.GroupLINQs
                         where 
                         (g.ReservationId == Convert.ToInt32(Request.QueryString["resId"])) &&
                         (g.Id == groupId)
                         select new
                         {
                             Id = g.Id,
                             GroupName = g.Name,
                             Status = g.Status,
                         }).FirstOrDefault();

                lblGroupId.Text = group.Id.ToString();
                txtGroupNameBorrow.Text = group.GroupName;

                //load related items - from BorrowItems
                var items = from i in db.Items
                            join inv in db.InventoryLINQs
                            on i.Id equals inv.ItemId
                            join ri in db.ReservationItems
                            on inv.Id equals ri.InventoryId
                            where
                            (ri.ReservationId == Convert.ToInt32(Request.QueryString["resId"]))
                            select new
                            {
                                Id = ri.Id,
                                InventoryId = ri.InventoryId,
                                ItemName = i.ItemName,
                                Stocks = inv.Stocks,
                                ReservedQuantity = ri.Quantity,
                                QuantityByGroup = ri.QuantityByGroup
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

        //Groups currently using the items Gridview
        protected void gvRelease_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("showReturn"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int groupId = (int)gvReleaseGroup.DataKeys[index].Value;

                //load group info
                var q = (from g in db.GroupLINQs
                         where 
                         (g.ReservationId == Convert.ToInt32(Request.QueryString["resId"])) &&
                         (g.Id == groupId)
                         select new
                         {
                             Id = g.Id,
                             GroupName = g.Name,
                             Status = g.Status
                         }).FirstOrDefault();

                lblRowId.Text = q.Id.ToString();
                txtGroupName.Text = q.GroupName;

                //load related items and chk if it has breakage/missing
                var items = from i in db.Items
                            join inv in db.InventoryLINQs
                            on i.Id equals inv.ItemId
                            join gi in db.GroupItems
                            on inv.Id equals gi.InventoryId
                            join g in db.GroupLINQs
                            on gi.GroupId equals g.Id
                            where gi.GroupId == q.Id
                            select new
                            {
                                Id = gi.Id,
                                InventoryId = gi.InventoryId,
                                Name = i.ItemName,
                                Stocks = inv.Stocks,
                                BorrowedQuantity = gi.BorrowedQuantity,
                                ReturnedQuantity = gi.ReturnedQuantity,
                                Breakage = gi.Breakage,
                                Remarks = gi.Remarks,
                                HasBreakage = gi.HasBreakage
                            };

                gvBreakage.DataSource = items.ToList();
                gvBreakage.DataBind();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#showReturnModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
        }

        protected void gvReturned_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("showReturned"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int groupId = (int)gvReturnedGroup.DataKeys[index].Value;

                //load group info
                var q = (from g in db.GroupLINQs
                         where
                         (g.ReservationId == Convert.ToInt32(Request.QueryString["resId"])) &&
                         (g.Id == groupId)
                         select new
                         {
                             Id = g.Id,
                             GroupName = g.Name,
                             Status = g.Status
                         }).FirstOrDefault();

                lblCompleteReturnedGroupId.Text = q.Id.ToString();
                txtCompleteReturnedGroupName.Text = q.GroupName;

                //load related items and chk if it has breakage/missing
                var items = from i in db.Items
                            join inv in db.InventoryLINQs
                            on i.Id equals inv.ItemId
                            join gi in db.GroupItems
                            on inv.Id equals gi.InventoryId
                            join g in db.GroupLINQs
                            on gi.GroupId equals g.Id
                            where gi.GroupId == q.Id
                            select new
                            {
                                Id = gi.Id,
                                InventoryId = gi.InventoryId,
                                Name = i.ItemName,
                                Stocks = inv.Stocks,
                                BorrowedQuantity = gi.BorrowedQuantity,
                                ReturnedQuantity = gi.ReturnedQuantity,
                                Breakage = gi.Breakage,
                                Remarks = gi.Remarks,
                                HasBreakage = gi.HasBreakage
                            };

                gvCompleteReturned.DataSource= items.ToList();
                gvCompleteReturned.DataBind();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(@"<script type='text/javascript'>");
                sb.Append("$('#showCompleteReturnedModal').modal('show');");
                sb.Append(@"</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteShowModalScript", sb.ToString(), false);
            }
        }

        protected void bindReserveItems()
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
                        InventoryId = ri.InventoryId,
                        Name = i.ItemName,
                        Stocks = inv.Stocks,
                        Quantity = ri.Quantity,
                        QuantityByGroup = ri.QuantityByGroup
                    };

            gvReservaItems.DataSource = q.ToList();
            gvReservaItems.DataBind();
        }

        protected void bindTaggedGroups()
        {
            var q = from g in db.GroupLINQs
                    where
                    (g.ReservationId == Convert.ToInt32(Request.QueryString["resId"])) &&
                    (g.Status == "Joined")
                    select new
                    {
                        Id = g.Id,
                        GroupName = g.Name,
                        Status = g.Status
                    };
            gvTaggedGroups.DataSource = q.ToList();
            gvTaggedGroups.DataBind();
        }

        protected void bindReleaseGroups()
        {
            var q = from g in db.GroupLINQs
                    where
                    (g.ReservationId == Convert.ToInt32(Request.QueryString["resId"])) &&
                    (g.Status == "In-Progress")
                    select new
                    {
                        Id = g.Id,
                        GroupName = g.Name,
                        Status = g.Status,
                        HasBreakage = g.HasBreakage
                    };
            gvReleaseGroup.DataSource = q.ToList();
            gvReleaseGroup.DataBind();
        }

        protected void bindReturnedGroups()
        {
            var q = from g in db.GroupLINQs
                    where
                    (g.ReservationId == Convert.ToInt32(Request.QueryString["resId"])) &&
                    (g.Status == "Complete") &&
                    (g.HasBreakage == false) &&
                    (g.IsReturned == true)
                    select new
                    {
                        Id = g.Id,
                        GroupName = g.Name,
                        Status = g.Status
                    };

            gvReturnedGroup.DataSource = q.ToList();
            gvReturnedGroup.DataBind();
        }

        protected void disableFields()
        {
            txtDateNeeded.Enabled = false;
            txtDateNeededTo.Enabled = false;
            txtExpNo.Enabled = false;
            txtLabRoom.Enabled = false;
            ddlSubject.Enabled = false;
            gvReservaItems.Enabled = false;
        }

        protected void enableFields()
        {
            txtDateNeeded.Enabled = true;
            txtDateNeededTo.Enabled = true;
            txtExpNo.Enabled = true;
            txtLabRoom.Enabled = true;
            ddlSubject.Enabled = true;
            gvReservaItems.Enabled = true;
        }

        private void bindDropdown()
        {
            var q = (from s in db.SubjectLINQs
                     select s).ToList();

            ddlSubject.DataSource = q;
            ddlSubject.DataTextField = "Name";
            ddlSubject.DataValueField = "Id";
            ddlSubject.DataBind();
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (Convert.ToDateTime(txtDateNeeded.Text) >= Convert.ToDateTime(txtDateNeededTo.Text))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        
    }
}