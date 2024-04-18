<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="frmSaleInvoice.aspx.cs" Inherits="AxPOSWebReport.frmSaleInvoice" %>

<link href="ReportCss/SaleInvoiceCss.css" rel="stylesheet" />

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    
        <div class="header">
        <asp:Label ID="PosHeader1" runat="server"></asp:Label>
            <div class="clearfix"></div>
         <asp:Label ID="PosHeader2" runat="server"></asp:Label>
            <div class="clearfix"></div>
         <asp:Label ID="PosHeader3" runat="server"></asp:Label>
         </div>

        <div class="Storeinfo">

             <div class="row" >
                <asp:Label ID="Label1" runat="server" Text="GSTIN"></asp:Label>
                <span class="required GSTIn"></span>
                 <asp:Label ID="lblGSTIN" runat="server"></asp:Label>
            </div>
           
             <div class="row">
              <asp:Label ID="Label2" runat="server" Text="Name"></asp:Label>
              <span class="required Name"></span>
              <asp:Label ID="lblName" runat="server"></asp:Label>
          </div>
            
             <div class="row">
           <asp:Label ID="Label3" runat="server" Text="Address"></asp:Label>
                <span class="required Address" ></span>
          <asp:Label ID="lbladdress1"  runat="server"></asp:Label>
           <div class="clearfix"></div>
          <asp:Label ID="lbladdress2" CssClass="Address2"  runat="server"></asp:Label>
            </div>

             <div class="row">
           <asp:Label ID="Label4" runat="server" Text="SI No of Invoice"></asp:Label>
                <span class="required invoiceno" ></span>
          <asp:Label ID="lblInvoiceNo"  runat="server"></asp:Label>
            </div>

             <div class="row">
           <asp:Label ID="Label5" runat="server" Text="Date of Invoice"></asp:Label>
                <span class="required invoicedate" ></span>
          <asp:Label ID="lblInvoicedate"  runat="server"></asp:Label>
            </div>

             <div class="row">
           <asp:Label ID="Label22" runat="server" Text="Sale Person Code "></asp:Label>
                <span class="required saleperson" ></span>
          <asp:Label ID="lblsaleperson"  runat="server"></asp:Label>
            </div>

        </div>

        <table class="Details">
            <tr>
                <td>
                    <div class="ReceiverDetails">
        <asp:Panel ID="ReceiverDetails" runat="server" CssClass="Reciverpanel" GroupingText="Details of Receiver(Billed to)" Width="590px">
      
               <div class="row">
              <asp:Label ID="Label6" runat="server" Text="Name"></asp:Label>
              <span class="required Name"></span>
              <asp:Label ID="lblRecName" runat="server"></asp:Label>
          </div>
     
               <div class="row">
           <asp:Label ID="Label7" runat="server" Text="Address"></asp:Label>
                <span class="required Address" ></span>
          <asp:Label ID="lblRecAddress"  runat="server"></asp:Label>
            </div>

               <div class="row">
           <asp:Label ID="Label8" runat="server" Text="Mob"></asp:Label>
                <span class="required Mob" ></span>
          <asp:Label ID="lblRecMobile"  runat="server"></asp:Label>
            </div>

               <div class="row">
           <asp:Label ID="Label9" runat="server" Text="State"></asp:Label>
                <span class="required State" ></span>
          <asp:Label ID="lblRecState"  runat="server"></asp:Label>
            </div>
              
               <div class="row">
           <asp:Label ID="Label12" runat="server" Text="State Code"></asp:Label>
                <span class="required StateCode" ></span>
          <asp:Label ID="lblRecStateCode"  runat="server"></asp:Label>
            </div>
  
               <div class="row">
           <asp:Label ID="Label10" runat="server" Text="GSTIN/UIN"></asp:Label>
                <span class="required RecGSTIn" ></span>
          <asp:Label ID="lblRecGSTIN"  runat="server"></asp:Label>
            </div>
              
               <div class="row">
           <asp:Label ID="Label11" runat="server" Text="PAN/ID"></asp:Label>
                <span class="required PAN" ></span>
          <asp:Label ID="lblRecPAN"  runat="server"></asp:Label>
            </div>
            
              </asp:Panel>
                   </div>
                 </td>
                <td>
                    <div class="ShippedDetails">
            <asp:Panel ID="ShippedDetails" runat="server" GroupingText="Details of Consignee(Shipped to)" Width="590px">
          
                <div class="row">
              <asp:Label ID="Label13" runat="server" Text="Name"></asp:Label>
              <span class="required Name"></span>
              <asp:Label ID="lblShipName" runat="server"></asp:Label>
          </div>
     
               <div class="row">
           <asp:Label ID="Label15" runat="server" Text="Address"></asp:Label>
                <span class="required Address" ></span>
          <asp:Label ID="lblShipAddress"  runat="server"></asp:Label>
                   <asp:Label ID="lblShipAddres2"  runat="server"></asp:Label>
            </div>

               <div class="row">
           <asp:Label ID="Label17" runat="server" Text="Mob"></asp:Label>
                <span class="required Mob" ></span>
          <asp:Label ID="lblShipMob"  runat="server"></asp:Label>
            </div>

               <div class="row">
           <asp:Label ID="Label19" runat="server" Text="State"></asp:Label>
                <span class="required State" ></span>
          <asp:Label ID="lblShipState"  runat="server"></asp:Label>
            </div>
              
               <div class="row">
           <asp:Label ID="Label21" runat="server" Text="State Code"></asp:Label>
                <span class="required StateCode" ></span>
          <asp:Label ID="lblShipStateCode"  runat="server"></asp:Label>
            </div>
  
               <div class="row">
           <asp:Label ID="Label23" runat="server" Text="GSTIN/UIN"></asp:Label>
                <span class="required RecGSTIn" ></span>
          <asp:Label ID="lblShipGst"  runat="server"></asp:Label>
            </div>

                 </asp:Panel>
                    </div>
                </td>
                </tr>
      </table>

        <div class="invoice Line">
        <asp:GridView ID="gdinvoiceline" runat="server" AutoGenerateColumns="False" ShowFooter="True" ShowHeaderWhenEmpty="True">
            <Columns>
                 <asp:BoundField DataField="SLNO" HeaderText="SL No.">
                    <HeaderStyle HorizontalAlign="Left" Width="50px" />
                    <ItemStyle HorizontalAlign="Left" Width="50px" VerticalAlign="Middle" />
                </asp:BoundField>
              <asp:BoundField DataField="NAMEALIAS" HeaderText="Particulars">
                    <HeaderStyle HorizontalAlign="Left" Width="180px" />
                    <ItemStyle HorizontalAlign="Left" Width="180px" VerticalAlign="Middle" />
                </asp:BoundField>
                
              <asp:BoundField HeaderText="HSN" DataField="CODE">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    <ItemStyle HorizontalAlign="Left" Width="100px" VerticalAlign="Middle" />
                </asp:BoundField>
                 <asp:BoundField DataField="UNITID" HeaderText="UOM">
                    <HeaderStyle HorizontalAlign="Left" Width="50px" />
                    <ItemStyle HorizontalAlign="Left" Width="50px" VerticalAlign="Middle" />
                </asp:BoundField>
               
                 <asp:BoundField DataField="PCS" HeaderText="PCS">
                    <HeaderStyle HorizontalAlign="Left" Width="50px" />
                    <ItemStyle HorizontalAlign="Left" Width="50px" VerticalAlign="Middle" />
                </asp:BoundField>
                   <asp:BoundField HeaderText="Gross Wt." DataField="GrossWeight">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    <ItemStyle HorizontalAlign="Left" Width="100px" VerticalAlign="Middle" />
                </asp:BoundField>
                  <asp:BoundField HeaderText="Dia/Stone Wt." DataField="NetWeight">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    <ItemStyle HorizontalAlign="Left" Width="100px" VerticalAlign="Middle" />
                </asp:BoundField>
                  <asp:BoundField DataField="GoldValue" HeaderText="Gold Value">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    <ItemStyle HorizontalAlign="Right" Width="100px" VerticalAlign="Middle" />
                </asp:BoundField>
               
                   <asp:BoundField DataField="VA" HeaderText="VA">
                    <HeaderStyle HorizontalAlign="Right" Width="100px" />
                    <ItemStyle HorizontalAlign="Right" Width="100px" VerticalAlign="Middle" />
                </asp:BoundField>
               
                   <asp:BoundField HeaderText="Stone Value" DataField="StoneValue">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    <ItemStyle HorizontalAlign="Right" Width="100px" VerticalAlign="Middle" />
                </asp:BoundField>
                
                <asp:BoundField HeaderText="Total value" DataField="TotalValue">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    <ItemStyle HorizontalAlign="Right" Width="100px" VerticalAlign="Middle" />
                </asp:BoundField>
                  <asp:BoundField DataField="Disc" HeaderText="Disc Amt">
                    <HeaderStyle HorizontalAlign="Left" Width="80px" />
                    <ItemStyle HorizontalAlign="Right" Width="80px" VerticalAlign="Middle" />
                </asp:BoundField>

                 <asp:BoundField DataField="TaxableValue" HeaderText="Taxable Value">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    <ItemStyle HorizontalAlign="Right" Width="100px" VerticalAlign="Middle" />
                </asp:BoundField>
              </Columns>
        </asp:GridView>
            </div>

        <table class="gridfooter">
            <tr><td>
            <div class="row amtinword">
            <asp:Label ID="lblamtinWord" runat="server" ></asp:Label>
                <div class="clearfix"></div>
           <asp:Label ID="lblUsername" runat="server" ></asp:Label>
           </div>
            </td>
            <td class="taxgdTd">
            <div class="tendergrid">
                 <asp:GridView ID="gdtaxDec" runat="server" AutoGenerateColumns="False"  ShowHeader="false">
            <Columns>
                   <asp:BoundField DataField="Description">
                    <HeaderStyle HorizontalAlign="Left" Width="150px" />
                    <ItemStyle HorizontalAlign="Left" Width="150px" VerticalAlign="Middle" />
                </asp:BoundField>
                 <asp:BoundField DataField="TaxAmount">
                    <HeaderStyle HorizontalAlign="Left" Width="100px" />
                    <ItemStyle HorizontalAlign="Right" Width="100px" VerticalAlign="Middle" />
                </asp:BoundField>
                </Columns>
               </asp:GridView>
             </div>

         
                </td>
            </tr>
        </table>


    <table  class="footer">
        <tr>
            <td>
    <div class="row condition">  
     <div class="row"><asp:Label ID="lblCond" class="font" runat="server" Text="Condition : This below conditions are applicable only Gold,Diamond and Precious ornaments."></asp:Label></div>
    <div class="clearfix"></div>
       <div class="row"><asp:Label ID="lblCond1" runat="server" Text="1. The Above invoice value includes the Hallmarking charge of Rs 35 per Piece   "></asp:Label></div>
    <div class="clearfix"></div>
    <asp:Label ID="lblCond2" runat="server" Text="2. In addition to the indication of separate description of each article, net weight of precious metal, purity in carat and fineness,"></asp:Label>
   <div class="clearfix"></div>
     <asp:Label ID="Label14" runat="server" Text="and Hallmarking charges in the bill or invoice of sale of hallmarked precious metal articles as specified in BIS (Hallmarking)"></asp:Label>
        <div class="clearfix"></div>
     <asp:Label ID="Label16" runat="server" Text="Regulations,2018 the bill or invoice of sale of hallmarked precious metal articles shall also mention that the consumer can get the"></asp:Label>
    <div class="clearfix"></div>
     <asp:Label ID="Label18" runat="server" Text="purity of the hallmarked jewellery/artefacts verified from any of the BIS recognized A & H centre. The list of BIS recognized A&H "></asp:Label>     
    <div class="clearfix"></div>
     <asp:Label ID="Label20" runat="server" Text="centre along with address and contact details are available on the website www.bis.gov.in"></asp:Label>   
    </div>
</td>
            <td class="signtd">
    <div class="sign">
       <div class="row"> <asp:Label ID="Signature" runat="server"  Text="Signature"></asp:Label>
        <span class="required Sign"></span>
           </div>
        <div class="clearfix"></div>
       <div class="row">   <asp:Label ID="NameofSignatory"   runat="server" Text="Name of Signatory  :"></asp:Label></div>
    <div class="clearfix"></div>
        <div class="row">  <asp:Label ID="Designationstatus"  runat="server" Text="Designation/Status  :"></asp:Label></div>
    </div>
                </td>
            </tr>
        </table>

    </form>
</body>
</html>
