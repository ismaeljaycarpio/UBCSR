<%@ Page Title="Borrow List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="UBCSR.borrow._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSearch">
        <div class="row">
            <div class="col-md-12">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h5>
                            <asp:Label ID="lblTitle" runat="server"></asp:Label></h5>
                    </div>

                    <div class="panel-body">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="col-sm-10">
                                    <div class="input-group">
                                        <span class="input-group-btn">
                                            <asp:Button ID="btnSearch"
                                                runat="server"
                                                CssClass="btn btn-primary"
                                                Text="Go"
                                                OnClick="btnSearch_Click" />
                                        </span>
                                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search..."></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <asp:UpdatePanel ID="upBorrow" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="gvBorrow"
                                        runat="server"
                                        CssClass="table table-striped table-hover dataTable"
                                        GridLines="None"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true"
                                        AllowSorting="true"
                                        EmptyDataText="No Record(s) found"
                                        ShowHeaderWhenEmpty="true"
                                        DataKeyNames="Id"
                                        OnRowDataBound="gvBorrow_RowDataBound"
                                        OnPageIndexChanging="gvBorrow_PageIndexChanging"
                                        OnRowCommand="gvBorrow_RowCommand"
                                        OnSorting="gvBorrow_Sorting"
                                        PageSize="10">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Row Id" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbEdit" runat="server" Text="Edit" CommandName="editRecord" CommandArgument='<%#((GridViewRow)Container).RowIndex %>'></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" />
                                            <asp:BoundField DataField="ExperimentNo" HeaderText="Experiment No" SortExpression="ExperimentNo" />
                                            <asp:BoundField DataField="DateRequested" HeaderText="Date Requested" SortExpression="DateRequested" />
                                            <asp:BoundField DataField="DateFrom" HeaderText="Date Needed From" SortExpression="DateFrom" />
                                            <asp:BoundField DataField="DateTo" HeaderText="Date Needed To" SortExpression="DateTo" />
                                            <asp:BoundField DataField="LabRoom" HeaderText="Lab Room" SortExpression="LabRoom" />
                                            <asp:BoundField DataField="ApprovalStatus" HeaderText="Approval" SortExpression="ApprovalStatus" />

                                            <asp:TemplateField HeaderText="Released">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReleased" runat="server" Text='<%# Boolean.Parse(Eval("IsReleased").ToString()) ? "Yes" : "No" %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Returned">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReturned" runat="server" Text='<%# Boolean.Parse(Eval("IsReturned").ToString()) ? "Yes" : "No" %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Button ID="btnShowDelete" runat="server" Text="Delete" CommandName="deleteRecord" CssClass="btn btn-danger" CommandArgument='<%#((GridViewRow) Container).RowIndex %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" />
                                    </asp:GridView>
                                    <asp:Button ID="btnOpenModal"
                                        runat="server"
                                        CssClass="btn btn-default btn-sm"
                                        Text="Borrow"
                                        OnClick="btnOpenModal_Click"
                                        CausesValidation="false" />
                                </ContentTemplate>
                                <Triggers>
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Delete Modal -->
    <div id="deleteModal" class="modal fade" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true" role="dialog">
        <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Delete Confirmation</h4>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this record ?
                            <asp:HiddenField ID="hfDeleteId" runat="server" />
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-danger" Text="Delete" OnClick="btnDelete_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnDelete" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfUserId" runat="server" />
</asp:Content>
