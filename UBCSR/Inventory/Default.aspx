<%@ Page Title="Inventory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="UBCSR.Inventory.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h5>Inventory</h5>
                </div>
                <div class="panel-body">
                    <asp:UpdatePanel ID="upInv" runat="server">
                        <ContentTemplate>
                            <div class="form-inline">
                                <div class="input-group">
                                    <asp:TextBox ID="txtSearch"
                                        runat="server"
                                        CssClass="form-control"
                                        placeholder="Search Inventory"></asp:TextBox>
                                    <div class="input-group-btn">
                                        <asp:Button ID="btnSearch"
                                            runat="server"
                                            CssClass="btn btn-primary"
                                            Text="Go"
                                            CausesValidation="false"
                                            OnClick="btnSearch_Click" />
                                    </div>
                                </div>
                                <div class="pull-right">
                                    <asp:Button ID="btnExport"
                                        runat="server"
                                        CausesValidation="false"
                                        Text="Export to Excel"
                                        CssClass="btn btn-default btn-sm"
                                        OnClick="btnExport_Click" />
                                </div>
                            </div>

                            <br />

                            <div class="table-responsive">
                                <asp:GridView ID="gvInventory"
                                    runat="server"
                                    class="table table-striped table-hover dataTable"
                                    GridLines="None"
                                    ShowHeader="true"
                                    ShowHeaderWhenEmpty="true"
                                    AutoGenerateColumns="false"
                                    AllowPaging="true"
                                    AllowSorting="true"
                                    DataKeyNames="Id"
                                    ShowFooter="true"
                                    EmptyDataText="No Record(s) found"
                                    OnSorting="gvInventory_Sorting"
                                    OnPageIndexChanging="gvInventory_PageIndexChanging"
                                    OnRowCommand="gvInventory_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Id" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Item" SortExpression="ItemName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Brand" SortExpression="BrandName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBrandName" runat="server" Text='<%# Eval("BrandName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Category" SortExpression="CategoryName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCategoryName" runat="server" Text='<%# Eval("CategoryName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Stocks" SortExpression="Stocks">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStocks" runat="server" Text='<%# Eval("Stocks") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Serial" SortExpression="Serial">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSerial" runat="server" Text='<%# Eval("Serial") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Remarks">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRemakrs" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:ButtonField HeaderText="Update Stocks" ButtonType="Link" Text="Update Stocks" CommandName="updateRecord" />
                                        <asp:ButtonField HeaderText="" ButtonType="Link" Text="Edit" CommandName="editRecord" />
                                        <asp:ButtonField HeaderText="" ButtonType="Link" Text="Delete" CommandName="deleteRecord" />

                                    </Columns>
                                    <PagerStyle CssClass="pagination-ys" />
                                </asp:GridView>

                                <div class="pull-right">
                                    <asp:Button ID="btnOpenModal"
                                        runat="server"
                                        CssClass="btn btn-info btn-sm"
                                        Text="Add Item to Inventory"
                                        OnClick="btnOpenModal_Click"
                                        CausesValidation="false" />
                                </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="gvInventory" />
                            <asp:PostBackTrigger ControlID="btnExport" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>

    <!-- Add Modal -->
    <div id="addModal" class="modal fade" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel ID="upAdd" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Add Item to Inventory</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form">
                                <div class="form-group">
                                    <label for="ddlAddItem">Item</label>
                                    <asp:DropDownList ID="ddlAddItem" runat="server" CssClass="form-control"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                                        runat="server"
                                        Display="Dynamic"
                                        ControlToValidate="ddlAddItem"
                                        CssClass="label label-danger"
                                        ValidationGroup="vgAdd"
                                        InitialValue="0"
                                        ErrorMessage="Item is required"></asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label for="txtAddStocks">Stocks</label>
                                    <asp:TextBox ID="txtAddStocks" runat="server" CssClass="form-control" placeholder="Stocks"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5"
                                        runat="server"
                                        Display="Dynamic"
                                        ControlToValidate="txtAddStocks"
                                        CssClass="label label-danger"
                                        ValidationGroup="vgAdd"
                                        ErrorMessage="Stocks is required"></asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label for="txtAddSerial">Serial</label>
                                    <asp:TextBox ID="txtAddSerial" runat="server" CssClass="form-control" placeholder="Serial"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label for="txtAddRemarks">Remarks</label>
                                    <asp:TextBox ID="txtAddRemarks" runat="server" CssClass="form-control" placeholder="Remarks" TextMode="MultiLine"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <asp:Label ID="lblDuplicateRecord" runat="server" CssClass="label label-danger"></asp:Label>
                                </div>
                            </div>

                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" ValidationGroup="vgAdd" OnClick="btnSave_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- Edit Modal -->
    <div id="editModal" class="modal fade" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true" role="dialog">
        <div class="modal-dialog">
            <!-- Edit Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel ID="upEdit" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Edit Item Inventory</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form">
                                <div class="form-group">
                                    <asp:Label ID="lblRowId" runat="server" Visible="false"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="ddlEditItem">Item</label>
                                    <asp:DropDownList ID="ddlEditItem" runat="server" CssClass="form-control"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4"
                                        runat="server"
                                        Display="Dynamic"
                                        ControlToValidate="ddlEditItem"
                                        CssClass="label label-danger"
                                        ValidationGroup="vgEdit"
                                        InitialValue="0"
                                        ErrorMessage="Item is required"></asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label for="txtEditStocks">Stocks</label>
                                    <asp:TextBox ID="txtEditStocks" runat="server" CssClass="form-control" placeholder="Stocks"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6"
                                        runat="server"
                                        Display="Dynamic"
                                        ControlToValidate="txtEditStocks"
                                        CssClass="label label-danger"
                                        ValidationGroup="vgEdit"
                                        ErrorMessage="Stocks is required"></asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label for="txtEditSerial">Serial</label>
                                    <asp:TextBox ID="txtEditSerial" runat="server" CssClass="form-control" placeholder="Serial"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label for="txtEditRemarks">Remarks</label>
                                    <asp:TextBox ID="txtEditRemarks" runat="server" CssClass="form-control" placeholder="Remarks" TextMode="MultiLine"></asp:TextBox>
                                </div>

                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnEdit" runat="server" CssClass="btn btn-primary" Text="Update" ValidationGroup="vgEdit" OnClick="btnEdit_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-hidden="true">Cancel</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvInventory" EventName="RowCommand" />
                        <asp:AsyncPostBackTrigger ControlID="btnEdit" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>


    <!-- Update Stock Modal -->
    <div id="updateModal" class="modal fade" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true" role="dialog">
        <div class="modal-dialog">
            <!-- Update Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Update Stock of Item Inventory</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form">
                                <div class="form-group">
                                    <asp:Label ID="lblId" runat="server" Visible="false"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="ddlUpdateItem">Item</label>
                                    <asp:DropDownList ID="ddlUpdateItem" runat="server" CssClass="form-control" Enabled="false"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9"
                                        runat="server"
                                        Display="Dynamic"
                                        ControlToValidate="ddlUpdateItem"
                                        CssClass="label label-danger"
                                        ValidationGroup="vgUpdate"
                                        InitialValue="0"
                                        ErrorMessage="Item is required"></asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group">
                                    <label for="lblCurrentStocks">Current Stocks: </label>
                                    <asp:Label ID="lblCurrentStocks" runat="server" CssClass="label label-primary"></asp:Label>
                                </div>

                                <div class="form-group">
                                    <label for="txtUpdateStocks">Stocks to add</label>
                                    <asp:TextBox ID="txtUpdateStocks" runat="server" CssClass="form-control" placeholder="Stocks to add"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator10"
                                        runat="server"
                                        Display="Dynamic"
                                        ControlToValidate="txtUpdateStocks"
                                        CssClass="label label-danger"
                                        ValidationGroup="vgUpdate"
                                        ForeColor="Red"
                                        ErrorMessage="Stocks is required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2"
                                        runat="server"
                                        ControlToValidate="txtUpdateStocks"
                                        Display="Dynamic"
                                        ForeColor="Red"
                                        ValidationGroup="vgUpdate"
                                        ValidationExpression="(^([0-9]*\d*\d{1}\d*)$)"
                                        ErrorMessage="">*</asp:RegularExpressionValidator>
                                </div>

                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnUpdateStocks" runat="server" CssClass="btn btn-primary" Text="Update Stocks" ValidationGroup="vgUpdate" OnClick="btnUpdateStocks_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-hidden="true">Cancel</button>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvInventory" EventName="RowCommand" />
                        <asp:AsyncPostBackTrigger ControlID="btnUpdateStocks" EventName="Click" />
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
                            <h4 class="modal-title">Delete Inventory Confirmation</h4>
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

</asp:Content>
