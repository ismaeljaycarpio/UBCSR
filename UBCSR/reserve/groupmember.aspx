<%@ Page Title="Group Members" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="groupmember.aspx.cs" Inherits="UBCSR.reserve.groupmember" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSearch">
        <div class="row">
            <div class="col-md-12">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h5>
                            <asp:Label ID="lblTitle" runat="server">Group Management</asp:Label></h5>
                    </div>

                    <div class="panel-body">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="col-sm-10">
                                    <div class="input-group">
                                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search..."></asp:TextBox>
                                        <span class="input-group-btn" style="width: 0;">
                                            <asp:Button ID="btnSearch"
                                                runat="server"
                                                CssClass="btn btn-primary"
                                                Text="Go"
                                                OnClick="btnSearch_Click" />
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <asp:UpdatePanel ID="upTeam" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="gvTeam"
                                        runat="server"
                                        CssClass="table table-striped table-hover dataTable"
                                        GridLines="None"
                                        AllowPaging="true"
                                        AllowSorting="true"
                                        AutoGenerateColumns="false"
                                        EmptyDataText="No Record(s) found"
                                        ShowHeaderWhenEmpty="true"
                                        DataKeyNames="Id"
                                        OnRowCommand="gvTeam_RowCommand"
                                        DataSourceID="GroupDataSource">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Row Id" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Group Name" SortExpression="GroupName">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblName" runat="server" Text='<%# Eval("GroupName") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Subject" SortExpression="Subject">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSubject" runat="server" Text='<%# Eval("Subject") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Section" SortExpression="Section">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSection" runat="server" Text='<%# Eval("Section") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="School Year" SortExpression="YearFrom">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblYearFrom" runat="server" Text='<%# Eval("YearFrom") %>'></asp:Label>
                                                    -
                                                <asp:Label ID="lblYearTo" runat="server" Text='<%# Eval("YearTo") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Semester" SortExpression="Sem">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSemester" runat="server" Text='<%# Eval("Sem") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%--<asp:ButtonField HeaderText="" ButtonType="Link" Text="Add Members" CommandName="addMembers" />--%>
                                            <asp:ButtonField HeaderText="" ButtonType="Link" Text="Edit Members" CommandName="editMembers" />
                                            <asp:ButtonField HeaderText="" ButtonType="Link" Text="Delete" CommandName="deleteRecord" />
                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" />
                                    </asp:GridView>
                                    <!-- Trigger the modal with a button -->
                                    <div class="pull-right">
                                        <asp:Button ID="btnOpenModal"
                                            runat="server"
                                            CssClass="btn btn-info btn-sm"
                                            Text="Create Group"
                                            OnClick="btnOpenModal_Click"
                                            CausesValidation="false" />
                                    </div>
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


    <!-- Create Group Modal -->
    <div id="createModal" class="modal fade" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Group Details</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form">

                                <div class="form-group">
                                    <label for="txtCreateGroupName">Group Name: </label>
                                    <asp:TextBox ID="txtCreateGroupName" runat="server" CssClass="form-control" placeholder="Group Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                                        runat="server"
                                        Display="Dynamic"
                                        ControlToValidate="txtCreateGroupName"
                                        CssClass="label label-danger"
                                        ValidationGroup="vgCreateGroup"
                                        ErrorMessage="Group Name is required"></asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label for="ddlCreateSubject">Subject: </label>
                                    <asp:DropDownList ID="ddlCreateSubject" 
                                        runat="server"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlCreateSubject_SelectedIndexChanged"
                                        CssClass="form-control"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                                        runat="server"
                                        Display="Dynamic"
                                        ControlToValidate="ddlCreateSubject"
                                        CssClass="label label-danger"
                                        ValidationGroup="vgCreateGroup"
                                        InitialValue="0"
                                        ErrorMessage="Subject is required"></asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label for="lblCreateYearFrom">Year From:</label>
                                    <asp:Label ID="lblCreateYearFrom" runat="server"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="lblCreateYearTo">Year To:</label>
                                    <asp:Label ID="lblCreateYearTo" runat="server"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="lblCreateSem">Semester:</label>
                                    <asp:Label ID="lblCreateSem" runat="server"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="lblCreateSection">Section:</label>
                                    <asp:Label ID="lblCreateSection" runat="server"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="gvCreateMembers"></label>
                                    <div class="table table-responsive">
                                        <asp:GridView ID="gvCreateMembers"
                                            runat="server"
                                            CssClass="table table-striped table-hover dataTable"
                                            GridLines="None"
                                            AutoGenerateColumns="false"
                                            EmptyDataText="No Record(s) found"
                                            ShowHeaderWhenEmpty="true"
                                            DataKeyNames="UserId">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkRow" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Row Id" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblUserId" runat="server" Text='<%# Eval("UserId") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="FullName" HeaderText="Name" />
                                            </Columns>
                                            <PagerStyle CssClass="pagination-ys" />
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnCreateGroup"
                                runat="server"
                                CssClass="btn btn-primary"
                                Text="Save"
                                CausesValidation="true"
                                ValidationGroup="vgCreateGroup"
                                OnClick="btnCreateGroup_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-hidden="true">Cancel</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCreateGroup" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- Add Members Modal -->
    <div id="addMembersModal" class="modal fade" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true" role="dialog">
        <div class="modal-dialog">
            <!-- Return Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Group Members</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form">
                                <div class="form-group">
                                    <asp:Label ID="lblGroupId" runat="server" Visible="false"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="txtGroupName">Group Name: </label>
                                    <asp:TextBox ID="txtGroupName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label for="gvMembers"></label>
                                    <div class="table table-responsive">
                                        <asp:GridView ID="gvMembers"
                                            runat="server"
                                            CssClass="table table-striped table-hover dataTable"
                                            GridLines="None"
                                            AutoGenerateColumns="false"
                                            EmptyDataText="No Record(s) found"
                                            ShowHeaderWhenEmpty="true"
                                            DataKeyNames="UserId">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkRow" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Row Id" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblGroupId" runat="server" Text='<%# Eval("UserId") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="FullName" HeaderText="Name" />
                                            </Columns>
                                            <PagerStyle CssClass="pagination-ys" />
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnAddGroup"
                                runat="server"
                                CssClass="btn btn-primary"
                                Text="Save"
                                CausesValidation="true"
                                ValidationGroup="vgAddGroup"
                                OnClick="btnAddGroup_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-hidden="true">Cancel</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvMembers" EventName="RowCommand" />
                        <asp:AsyncPostBackTrigger ControlID="btnAddGroup" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- Edit Members Modal -->
    <div id="editMembersModal" class="modal fade" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true" role="dialog">
        <div class="modal-dialog">
            <!-- Return Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Group Members</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form">
                                <div class="form-group">
                                    <asp:Label ID="lblEditGroupId" runat="server" Visible="false"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="txtGroupName">Group Name: </label>
                                    <asp:TextBox ID="txtEditGroupName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label for="gvEditMembers">Check user you want to remove:</label>
                                    <div class="table table-responsive">
                                        <asp:GridView ID="gvEditMembers"
                                            runat="server"
                                            CssClass="table table-striped table-hover dataTable"
                                            GridLines="None"
                                            AutoGenerateColumns="false"
                                            EmptyDataText="No Record(s) found"
                                            ShowHeaderWhenEmpty="true"
                                            DataKeyNames="UserId">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkRow" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Row Id" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEditGroupId" runat="server" Text='<%# Eval("UserId") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="FullName" HeaderText="Name" />
                                            </Columns>
                                            <PagerStyle CssClass="pagination-ys" />
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnEditGroup"
                                runat="server"
                                CssClass="btn btn-primary"
                                Text="Save"
                                CausesValidation="true"
                                ValidationGroup="gvEditGroup"
                                OnClick="btnEditGroup_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-hidden="true">Cancel</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvEditMembers" EventName="RowCommand" />
                        <asp:AsyncPostBackTrigger ControlID="btnEditGroup" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

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

    <asp:LinqDataSource ID="GroupDataSource" runat="server" OnSelecting="GroupDataSource_Selecting"></asp:LinqDataSource>
    <asp:HiddenField ID="hfUserId" runat="server" />

</asp:Content>
