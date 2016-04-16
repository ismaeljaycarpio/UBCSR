<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UBCSR.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-lg-4 col-lg-offset-4 text-center">
            <div class="panel panel-default">
                <div class="panel-body">
                    <asp:Login runat="server"
                        ViewStateMode="Disabled"
                        RenderOuterTable="false"
                        DestinationPageUrl="~/Default.aspx">
                        <LayoutTemplate>
                            <p class="validation-summary-errors text-danger">
                                <asp:Literal runat="server" ID="FailureText" />
                                <div class="col-sm-10"></div>
                            </p>
                            <br />
                            <div class="form">
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="UserName">Username</asp:Label>
                                    <asp:TextBox runat="server" ID="UserName" CssClass="form-control center-block" />
                                    <asp:RequiredFieldValidator
                                        runat="server"
                                        ControlToValidate="UserName"
                                        CssClass="label label-danger" ErrorMessage="The user name field is required." />
                                </div>

                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="Password">Password</asp:Label>
                                    <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control center-block" />
                                    <asp:RequiredFieldValidator runat="server"
                                        ControlToValidate="Password"
                                        CssClass="label label-danger"
                                        ErrorMessage="The password field is required." />
                                </div>
                                <div class="form-group">
                                    <asp:CheckBox runat="server" ID="RememberMe" />
                                    <asp:Label runat="server" AssociatedControlID="RememberMe" CssClass="checkbox">Remember me?</asp:Label>
                                </div>
                                <asp:Button runat="server" CommandName="Login" Text="Log in" CssClass="btn btn-primary" />
                            </div>
                        </LayoutTemplate>
                    </asp:Login>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
