<%@ Page Title="Reserve Form" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="reserve.aspx.cs" Inherits="UBCSR.borrow.reserve" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <h5>Reservation Form</h5>
                </div>
                <div class="panel-body">
                    <div role="form">
                        <div class="col-md-4">
                            <label for="ddlSubject">Subject</label>
                            <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3"
                                runat="server"
                                Display="Dynamic"
                                ValidationGroup="vgPrimaryAdd"
                                ControlToValidate="ddlSubject"
                                CssClass="label label-danger"
                                InitialValue="0"
                                ErrorMessage="Subject is required"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-md-4">
                            <label for="txtExpNo">Experiment No</label>
                            <asp:TextBox ID="txtExpNo" runat="server" CssClass="form-control"></asp:TextBox>
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
                            <asp:TextBox ID="txtLabRoom" runat="server" CssClass="form-control"></asp:TextBox>
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
                                CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                                runat="server"
                                Display="Dynamic"
                                ControlToValidate="txtDateNeededTo"
                                CssClass="label label-danger"
                                ValidationGroup="vgPrimaryAdd"
                                ErrorMessage="Date Needed to is required"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
                <div class="panel-body">
                    <div class="table-responsive">
                        <asp:UpdatePanel ID="upInv" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gvInv"
                                    runat="server"
                                    CssClass="table table-striped table-hover dataTable"
                                    GridLines="None"
                                    AutoGenerateColumns="false"
                                    EmptyDataText="No Record(s) found"
                                    ShowHeaderWhenEmpty="true"
                                    DataKeyNames="Id"
                                    OnRowCommand="gvInv_RowCommand">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkRow" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Row Id" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="Name" HeaderText="Item" />
                                        <asp:BoundField DataField="Stocks" HeaderText="Remaining Quantity" />

                                        <asp:TemplateField HeaderText="Quantity to Borrow">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQuantityToBorrow"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    Width="50"
                                                    Enabled="false"></asp:TextBox>
                                                <asp:RangeValidator ID="RangeValidator1"
                                                    runat="server"
                                                    ForeColor="Red"
                                                    ControlToValidate="txtQuantityToBorrow"
                                                    Display="Dynamic"
                                                    MinimumValue="1"
                                                    ValidationGroup="vgPrimaryAdd"
                                                    MaximumValue='<%# Eval("Stocks") %>'
                                                    Type="Integer"
                                                    ErrorMessage="RangeValidator">*</asp:RangeValidator>
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
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-primary" CausesValidation="true" ValidationGroup="vgPrimaryAdd" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-default" />
                    </div>
                </div>
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
</asp:Content>
