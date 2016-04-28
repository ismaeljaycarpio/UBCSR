<%@ Page Title="Edit Reservation Form" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="editreserve.aspx.cs" Inherits="UBCSR.reserve.editreserve" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <h5>Edit Reservation Form</h5>
                </div>
                <div class="panel-body">
                    <div role="form">
                        <div class="col-md-4">
                            <label for="ddlSubject">Subject</label>
                            <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control" Enabled="false"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3"
                                runat="server"
                                Display="Dynamic"
                                ValidationGroup="vgPrimaryAdd"
                                ControlToValidate="ddlSubject"
                                CssClass="label label-danger"
                                ErrorMessage="Subject is required"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-md-4">
                            <label for="txtExpNo">Experiment No</label>
                            <asp:TextBox ID="txtExpNo" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4"
                                runat="server"
                                Display="Dynamic"
                                ValidationGroup="vgPrimaryAdd"
                                ControlToValidate="txtExpNo"
                                CssClass="label label-danger"
                                ErrorMessage="Exp No is required"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-md-4">
                            <label for="txtLabRoom">Lab Room</label>
                            <asp:TextBox ID="txtLabRoom" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5"
                                runat="server"
                                Display="Dynamic"
                                ValidationGroup="vgPrimaryAdd"
                                ControlToValidate="txtLabRoom"
                                CssClass="label label-danger"
                                ErrorMessage="Lab Room is required"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>

                <div class="panel-body">
                    <div role="form">
                        <div class="col-md-4">
                            <label for="txtDateNeeded">Date/Time From</label>
                            <asp:TextBox ID="txtDateNeeded"
                                runat="server"
                                Enabled="false"
                                CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6"
                                runat="server"
                                Display="Dynamic"
                                ValidationGroup="vgPrimaryAdd"
                                ControlToValidate="txtDateNeeded"
                                CssClass="label label-danger"
                                ErrorMessage="Date Needed is required"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-md-4">
                            <label for="txtDateNeededTo">Date/Time To</label>
                            <asp:TextBox ID="txtDateNeededTo"
                                runat="server"
                                Enabled="false"
                                CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                                runat="server"
                                Display="Dynamic"
                                ControlToValidate="txtDateNeededTo"
                                CssClass="label label-danger"
                                ValidationGroup="vgPrimaryAdd"
                                ErrorMessage="Date Needed to is required"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="CustomValidator1"
                                runat="server"
                                CssClass="label label-danger"
                                Display="Dynamic"
                                ValidationGroup="vgPrimaryAdd"
                                OnServerValidate="CustomValidator1_ServerValidate"
                                ErrorMessage="Date/Time To must always be greater than Date/Time From"></asp:CustomValidator>
                        </div>
                        <div class="col-md-4">
                            <label for="txtIsReleased">Status</label>
                            <asp:TextBox ID="txtIsReleased"
                                runat="server"
                                Enabled="false"
                                CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="panel-body">
                    <div role="form">
                        <div class="col-md-4">
                            <label for="txtRemarks">Disapproved Remarks</label>
                            <asp:TextBox ID="txtDisapprovedRemarks"
                                runat="server"
                                Enabled="false"
                                TextMode="MultiLine"
                                CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="panel-body">
                    <div class="table-responsive">
                        <asp:UpdatePanel ID="upReserveItems" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gvReservaItems"
                                    runat="server"
                                    CssClass="table table-striped table-hover dataTable"
                                    GridLines="None"
                                    Enabled="false"
                                    AutoGenerateColumns="false"
                                    EmptyDataText="No Record(s) found"
                                    ShowHeaderWhenEmpty="true"
                                    DataKeyNames="Id">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkRow" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Reservation Item Id" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Inventory Id" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblInventoryId" runat="server" Text='<%# Eval("InventoryId") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="Name" HeaderText="Item" />
                                        <asp:BoundField DataField="Stocks" HeaderText="Remaining Quantity" />

                                        <asp:TemplateField HeaderText="Reserved Quantity">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQuantityToBorrow"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    Width="50"
                                                    Text='<%# Eval("Quantity") %>'
                                                    Enabled="false"></asp:TextBox>
                                                <asp:RangeValidator ID="RangeValidator1"
                                                    runat="server"
                                                    ForeColor="Red"
                                                    ControlToValidate="txtQuantityToBorrow"
                                                    Display="Dynamic"
                                                    MinimumValue="0"
                                                    ValidationGroup="vgPrimaryAdd"
                                                    MaximumValue='<%# Eval("Stocks") %>'
                                                    Type="Integer"
                                                    ErrorMessage="Invalid Input"></asp:RangeValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Quantity to Borrow by Group">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQuantityToBorrowByGroup"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    Width="50"
                                                    Text='<%# Eval("QuantityByGroup") %>'
                                                    Enabled="false"></asp:TextBox>
                                                <asp:RangeValidator ID="RangeValidator2"
                                                    runat="server"
                                                    ForeColor="Red"
                                                    ControlToValidate="txtQuantityToBorrowByGroup"
                                                    Display="Dynamic"
                                                    MinimumValue="1"
                                                    ValidationGroup="vgPrimaryAdd"
                                                    MaximumValue='<%# Eval("Stocks") %>'
                                                    Type="Integer"
                                                    ErrorMessage="Invalid Input"></asp:RangeValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                    </Columns>
                                    <PagerStyle CssClass="pagination-ys" />
                                </asp:GridView>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <div class="panel-footer">
                        <asp:Button ID="btnSave" runat="server"
                            Text="Save" OnClick="btnSave_Click" Visible="false"
                            CssClass="btn btn-primary" CausesValidation="true" ValidationGroup="vgPrimaryAdd" />
                        <asp:Button ID="btnApprove" runat="server"
                            Text="Approve" OnClick="btnApprove_Click" Visible="false"
                            CssClass="btn btn-success" CausesValidation="false" />
                        <asp:Button ID="btnDisapprove" runat="server" Visible="false"
                            Text="Disapprove" OnClick="btnDisapprove_Click"
                            CssClass="btn btn-danger" CausesValidation="false" />
                        <asp:Button ID="btnTagGroup" runat="server"
                            Text="Tag my Group" OnClick="btnTagGroup_Click"
                            CssClass="btn btn-success" CausesValidation="false" Visible="false" />
                        <asp:Button ID="btnCancel" runat="server"
                            Text="Close" OnClick="btnCancel_Click"
                            CssClass="btn btn-default" />
                    </div>

                    <asp:Panel ID="pnlSuccessfullJoin" CssClass="alert alert-success" runat="server" Visible="false">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <strong>Notice!</strong> Your group is  <strong>tagged !</strong>
                    </asp:Panel>

                    <asp:Panel ID="pnlDoublejoin" CssClass="alert alert-info" runat="server" Visible="false">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <strong>Notice!</strong> Your group is already <strong>tagged !</strong>
                    </asp:Panel>

                    <!-- Borrowers List -->
                    <div class="panel-body">
                        <div class="table-responsive">
                            <h4>Groups that are tagged: </h4>
                            <asp:UpdatePanel ID="upBorrowers" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="gvBorrowers"
                                        runat="server"
                                        CssClass="table table-striped table-hover dataTable"
                                        GridLines="None"
                                        AutoGenerateColumns="false"
                                        EmptyDataText="No Record(s) found"
                                        ShowHeaderWhenEmpty="true"
                                        DataKeyNames="Id"
                                        OnRowCommand="gvBorrowers_RowCommand">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Group Id" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="GroupName" HeaderText="Group Name" />
                                            <asp:BoundField DataField="GroupLeader" HeaderText="Group Leader" />
                                            <asp:BoundField DataField="Status" HeaderText="Status" />

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Button ID="btnShowBorrow"
                                                        runat="server"
                                                        Text="Release"
                                                        CommandName="showBorrow"
                                                        CssClass="btn btn-info"
                                                        CommandArgument='<%#((GridViewRow) Container).RowIndex %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%--<asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Button ID="btnShowReturn"
                                                        runat="server"
                                                        Text="Return"
                                                        CommandName="showReturn"
                                                        CssClass="btn btn-success"
                                                        CommandArgument='<%#((GridViewRow) Container).RowIndex %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" />
                                    </asp:GridView>
                                </ContentTemplate>
                                <Triggers>
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <!-- Release List -->
                    <div class="panel-body">
                        <div class="table-responsive">
                            <h4>Groups that are currently using the items: </h4>
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="gvRelease"
                                        runat="server"
                                        CssClass="table table-striped table-hover dataTable"
                                        GridLines="None"
                                        AutoGenerateColumns="false"
                                        EmptyDataText="No Record(s) found"
                                        ShowHeaderWhenEmpty="true"
                                        DataKeyNames="Id"
                                        OnRowCommand="gvRelease_RowCommand">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Group Id" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="GroupName" HeaderText="Group Name" />
                                            <asp:BoundField DataField="GroupLeader" HeaderText="Group Leader" />
                                            <asp:BoundField DataField="Status" HeaderText="Status" />

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Button ID="btnShowReturn"
                                                        runat="server"
                                                        Text="Return"
                                                        CommandName="showReturn"
                                                        CssClass="btn btn-success"
                                                        CommandArgument='<%#((GridViewRow) Container).RowIndex %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" />
                                    </asp:GridView>
                                </ContentTemplate>
                                <Triggers>
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <!-- Returned List-->
                    <div class="panel-body">
                        <div class="table-responsive">
                            <h4>Groups that returned the items: </h4>
                            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="gvReturned"
                                        runat="server"
                                        CssClass="table table-striped table-hover dataTable"
                                        GridLines="None"
                                        AutoGenerateColumns="false"
                                        EmptyDataText="No Record(s) found"
                                        ShowHeaderWhenEmpty="true"
                                        DataKeyNames="Id"
                                        OnRowCommand="gvReturned_RowCommand">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Group Id" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="GroupName" HeaderText="Group Name" />
                                            <asp:BoundField DataField="GroupLeader" HeaderText="Group Leader" />
                                            <asp:BoundField DataField="Status" HeaderText="Status" />

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Button ID="btnShowReturn"
                                                        runat="server"
                                                        Text="Return"
                                                        CommandName="showReturn"
                                                        CssClass="btn btn-success"
                                                        CommandArgument='<%#((GridViewRow) Container).RowIndex %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" />
                                    </asp:GridView>
                                </ContentTemplate>
                                <Triggers>
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>



    <!-- Borrow Modal -->
    <div id="showBorrowModal" class="modal fade" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true" role="dialog">
        <div class="modal-dialog">
            <!-- Return Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Borrow Items by Group</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form">
                                <div class="form-group">
                                    <asp:Label ID="lblBorrowId" runat="server" Visible="false"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="txtGroupNameBorrow">Group Name: </label>
                                    <asp:TextBox ID="txtGroupNameBorrow" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label for="txtGroupLeaderBorrow">Group Leader: </label>
                                    <asp:TextBox ID="txtGroupLeaderBorrow" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label for="gvBorrow"></label>
                                    <div class="table table-responsive">
                                        <asp:GridView ID="gvBorrow"
                                            runat="server"
                                            CssClass="table table-striped table-hover dataTable"
                                            GridLines="None"
                                            AutoGenerateColumns="false"
                                            EmptyDataText="No Record(s) found"
                                            ShowHeaderWhenEmpty="true"
                                            DataKeyNames="Id">
                                            <Columns>
                                                <asp:TemplateField HeaderText="ReservationItemId" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Inventory Id" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInventoryId" runat="server" Text='<%# Eval("InventoryId") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="ItemName" HeaderText="Item" />
                                                <asp:BoundField DataField="Stocks" HeaderText="Quantity Remaining" />
                                                <asp:BoundField DataField="ReservedQuantity" HeaderText="Reserved Quantity" />

                                                <asp:TemplateField HeaderText="Quantity to borrow">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtQuantity"
                                                            runat="server"
                                                            Text='<%# Eval("QuantityByGroup") %>'
                                                            CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                                                            runat="server"
                                                            ControlToValidate="txtQuantity"
                                                            Display="Dynamic"
                                                            ForeColor="Red"
                                                            ValidationGroup="vgConfirmBorrow"
                                                            ErrorMessage="">*</asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2"
                                                            runat="server"
                                                            ControlToValidate="txtQuantity"
                                                            Display="Dynamic"
                                                            ForeColor="Red"
                                                            ValidationGroup="vgConfirmBorrow"
                                                            ValidationExpression="(^([0-9]*\d*\d{1}\d*)$)"
                                                            ErrorMessage="">*</asp:RegularExpressionValidator>
                                                        <asp:RangeValidator ID="RangeValidator1"
                                                            runat="server"
                                                            ForeColor="Red"
                                                            ControlToValidate="txtQuantity"
                                                            Display="Dynamic"
                                                            MinimumValue="1"
                                                            ValidationGroup="vgConfirmBorrow"
                                                            MaximumValue='<%# Eval("ReservedQuantity") %>'
                                                            Type="Integer"
                                                            ErrorMessage="RangeValidator">*</asp:RangeValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle CssClass="pagination-ys" />
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnConfirmBorrow"
                                runat="server"
                                CssClass="btn btn-primary"
                                Text="Save"
                                CausesValidation="true"
                                ValidationGroup="vgConfirmBorrow"
                                OnClick="btnConfirmBorrow_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-hidden="true">Cancel</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvBreakage" EventName="RowCommand" />
                        <asp:AsyncPostBackTrigger ControlID="btnConfirmReturn" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- Return Modal -->
    <div id="showReturnModal" class="modal fade" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true" role="dialog">
        <div class="modal-dialog">
            <!-- Return Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel ID="upEdit" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Return Items by Group</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form">
                                <div class="form-group">
                                    <asp:Label ID="lblRowId" runat="server" Visible="false"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="txtGroupName">Group Name: </label>
                                    <asp:TextBox ID="txtGroupName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label for="txtGroupLeader">Group Leader: </label>
                                    <asp:TextBox ID="txtGroupLeader" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label for="gvBreakage"></label>
                                    <div class="table table-responsive">
                                        <asp:GridView ID="gvBreakage"
                                            runat="server"
                                            CssClass="table table-striped table-hover dataTable"
                                            GridLines="None"
                                            AutoGenerateColumns="false"
                                            EmptyDataText="No Record(s) found"
                                            ShowHeaderWhenEmpty="true"
                                            DataKeyNames="Id">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Row Id" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Inventory Id" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInventoryId" runat="server" Text='<%# Eval("InventoryId") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="Name" HeaderText="Item" />
                                                <asp:BoundField DataField="Stocks" HeaderText="Quantity Remaining" />

                                                <asp:TemplateField HeaderText="Borrowed Quantity">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBorrowedQuantity" 
                                                            runat="server" 
                                                            Text='<%# Eval("BorrowedQuantity") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Breakage">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtBreakage"
                                                            runat="server"
                                                            CssClass="form-control"
                                                            Text='<%# Eval("Breakage") %>'></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2"
                                                            runat="server"
                                                            ControlToValidate="txtBreakage"
                                                            Display="Dynamic"
                                                            ForeColor="Red"
                                                            ValidationGroup="vgConfirmReturn"
                                                            ValidationExpression="(^([0-9]*\d*\d{1}\d*)$)"
                                                            ErrorMessage="">*</asp:RegularExpressionValidator>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Remarks">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtRemarks"
                                                            runat="server"
                                                            CssClass="form-control"
                                                            Text='<%# Eval("Remarks") %>'
                                                            TextMode="MultiLine"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                            <PagerStyle CssClass="pagination-ys" />
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnConfirmReturn"
                                runat="server"
                                CssClass="btn btn-primary"
                                Text="Save"
                                CausesValidation="true"
                                ValidationGroup="vgConfirmReturn"
                                OnClick="btnConfirmReturn_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-hidden="true">Cancel</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvBreakage" EventName="RowCommand" />
                        <asp:AsyncPostBackTrigger ControlID="btnConfirmReturn" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- Disapprove Confirmation Modal-->
    <div id="disapproveModal" class="modal fade" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true" role="dialog">
        <div class="modal-dialog">
            <!-- Return Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Disapprove Confirm</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form">
                                <div class="form-group">
                                    <label for="txtDisapproveRemarks">Remarks: </label>
                                    <asp:TextBox ID="txtDisapproveRemarks"
                                        runat="server"
                                        TextMode="MultiLine"
                                        Height="200"
                                        CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnConfirmDisapprove"
                                runat="server"
                                CssClass="btn btn-danger"
                                Text="Save"
                                CausesValidation="true"
                                ValidationGroup="vgConfirmDisapprove"
                                OnClick="btnConfirmDisapprove_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-hidden="true">Cancel</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnDisapprove" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $(function () {
            //Enable Disable TextBoxes in a Row when the Row CheckBox is checked.
            $("[id*=chkRow]").bind("click", function () {

                //Find and reference the GridView.
                var grid = $(this).closest("table");

                //Find and reference the Header CheckBox.
                var chkHeader = $("[id*=chkHeader]", grid);

                //If the CheckBox is Checked then enable the TextBoxes in thr Row.
                if (!$(this).is(":checked")) {
                    var td = $("td", $(this).closest("tr"));
                    td.css({ "background-color": "#FFF" });
                    $("input[type=text]", td).attr("disabled", "disabled");
                } else {
                    var td = $("td", $(this).closest("tr"));
                    td.css({ "background-color": "#D8EBF2" });
                    $("input[type=text]", td).removeAttr("disabled");
                }
            });
        });
    </script>

    <script type="text/javascript">
        $(function () {
            $('#<%= txtDateNeeded.ClientID%>').datetimepicker();
            $('#<%= txtDateNeededTo.ClientID%>').datetimepicker();
        });
    </script>

    <asp:HiddenField ID="hfResId" runat="server" Value="" />
</asp:Content>
