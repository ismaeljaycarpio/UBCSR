<%@ Page Title="Borrow List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="UBCSR.borrow._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSearch">
        <div class="row">
            <div class="col-md-12">
                <div class="panel panel-danger">
                    <div class="panel-heading">
                        <h5><asp:Label ID="lblTitle" runat="server"></asp:Label></h5>
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
                                            <asp:BoundField DataField="LeaveName" HeaderText="Leave" SortExpression="LeaveName" />
                                            <asp:BoundField DataField="NumberOfDays" HeaderText="No Of Days" SortExpression="NumberOfDays" />
                                            <asp:BoundField DataField="FiledDate" HeaderText="Filed Date" SortExpression="FiledDate" />

                                            <asp:TemplateField HeaderText="Duration" SortExpression="FromDate">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFromDate" runat="server" Text='<%# Eval("FromDate") %>'></asp:Label>
                                                    <asp:Label ID="Label1" runat="server" Text=" - "></asp:Label>
                                                    <asp:Label ID="lblToDate" runat="server" Text='<%# Eval("ToDate") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Department Head Approval" SortExpression="DepartmentHeadApproval">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDepartmentHeadApproval" runat="server" Text='<%# Eval("DepartmentHeadApproval") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="HR Approval" SortExpression="HRApproval">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblHRApproval" runat="server" Text='<%# Eval("HRApproval") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Status" SortExpression="Status">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStatus" runat="server"></asp:Label>
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
                                        CssClass="btn btn-info btn-sm"
                                        Text="Borrow"
                                        OnClick="btnOpenModal_Click"
                                        CausesValidation="false" />
                                </ContentTemplate>
                                <Triggers></Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:HiddenField ID="hfUserId" runat="server" />

</asp:Content>
