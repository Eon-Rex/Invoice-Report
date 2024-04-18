using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;

namespace AxPOSWebReport
{
    public partial class PrintDocuments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          //// postXML();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
           
            Response.Redirect("frmReportViewer.aspx?ReportType=0&ReceiptID=" + txtInvoice.Text + "&Pdfflag="+0);
        }

        public void postXML()
        {
            System.Net.WebRequest req = null;
            System.Net.WebResponse rsp = null;
            try
            {
        //        string xmlMessage = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
        //"construct your xml request message as required by that method along with parameters";
        //        string url = "http://localhost:2605/frmReportViewer.aspx";
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        //        byte[] requestInFormOfBytes = System.Text.Encoding.ASCII.GetBytes(xmlMessage);
        //        request.Method = "POST";
        //        request.ContentType = "text/xml;charset=utf-8";
        //        request.ContentLength = requestInFormOfBytes.Length;
        //        Stream requestStream = request.GetRequestStream();
        //        requestStream.Write(requestInFormOfBytes, 0, requestInFormOfBytes.Length);
        //        requestStream.Close();
                try
                {
                    string uri ="http://localhost:2605/PostData.aspx";
                    string xmlData = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n";
                    xmlData=xmlData+
                    @"<TAXINVOICE>
                      <dtRTT>
                        <RECEIPTID>IVANG31006413</RECEIPTID>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <CHANNEL>5637148326</CHANNEL>
                        <CUSTACCOUNT>JALK-000001</CUSTACCOUNT>
                        <TRANSDATE>2/27/2021 12:00:00 AM</TRANSDATE>
                        <STAFF>000132</STAFF>
                        <STORE>ANG</STORE>
                        <GROSSAMOUNT>-28435.0500000000000000</GROSSAMOUNT>
                        <ISDELIVERYATSTORE>1</ISDELIVERYATSTORE>
                      </dtRTT>
                      <dtRTST>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <LINENUM>1.0000000000000000</LINENUM>
                        <ITEMID>14MIS22K</ITEMID>
                        <ITEMNAME>14MISCELLANEOUS_22K</ITEMNAME>
                        <HSNCODE>71131910</HSNCODE>
                        <UNITID>g</UNITID>
                        <GROSSWEIGHT>5.0000000000000000</GROSSWEIGHT>
                        <DIAMONDWEIGHT>0.00</DIAMONDWEIGHT>
                        <NETWEIGHT>5.0000000000000000</NETWEIGHT>
                        <METALVALUE>27000.0000000000000000</METALVALUE>
                        <MAKINGVALUE>540.0000000000000000</MAKINGVALUE>
                        <STONECVALUE>0.0000000000000000</STONECVALUE>
                        <STONEGVALUE>0.0000000000000000</STONEGVALUE>
                        <DIAMONDVALUE>0.0000000000000000</DIAMONDVALUE>
                        <DISCAMOUNT>0.0000000000000000</DISCAMOUNT>
                        <NETAMOUNT>-27540.0000000000000000</NETAMOUNT>
                        <TAGNO>MIS2204</TAGNO>
                        <RECEIPTID>PANG51270221000074</RECEIPTID>
                        <PURITY>0.00000</PURITY>
                        <PCS>1</PCS>
                        <VA>540.0000000000000000</VA>
                        <BRANDDESC></BRANDDESC>
                        <PRICE>5400.0000000000000000</PRICE>
                        <SALESPERSONCODE>JAT20S00021</SALESPERSONCODE>
                      </dtRTST>
                      <dtCust>
                        <CUSTACCOUNT>JALK-000001</CUSTACCOUNT>
                        <NAME>Raj Bahadur k</NAME>
                        <PHONE>9740980123</PHONE>
                        <ADDRESS>unity road THRISSUR Amity sky city BISRAKH -  UTTAR PRAD IND 201301 GAUTAM BUDDHA NAGAR,</ADDRESS>
                        <CITY>BISRAKH</CITY>
                        <COUNTRYREGIONID>IND</COUNTRYREGIONID>
                        <DISTRICTNAME>GAUTAM BUDDHA NAGAR</DISTRICTNAME>
                        <STATECODE>UTTAR PRAD</STATECODE>
                        <STATENAME>UTTAR PRADESH</STATENAME>
                        <STREET>unity road THRISSUR</STREET>
                        <ZIPCODE>201301</ZIPCODE>
                        <PANNUMBER>AISDF1111E</PANNUMBER>
                        <GSTIN></GSTIN>
                      </dtCust>
                      <dtGTE>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <TAXAMOUNT>413.1000000000000000</TAXAMOUNT>
                        <TAXPERCENTAGE>1.5000000000000000</TAXPERCENTAGE>
                        <TAXCOMPONENT>CGST</TAXCOMPONENT>
                      </dtGTE>
                      <dtGTE>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <TAXAMOUNT>68.8500000000000000</TAXAMOUNT>
                        <TAXPERCENTAGE>0.2500000000000000</TAXPERCENTAGE>
                        <TAXCOMPONENT>KFC</TAXCOMPONENT>
                      </dtGTE>
                      <dtGTE>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <TAXAMOUNT>413.1000000000000000</TAXAMOUNT>
                        <TAXPERCENTAGE>1.5000000000000000</TAXPERCENTAGE>
                        <TAXCOMPONENT>SGST</TAXCOMPONENT>
                      </dtGTE>
                      <dtTotal>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <DESCRIPTION>Taxable Value</DESCRIPTION>
                        <AMOUNT>0</AMOUNT>
                      </dtTotal>
                      <dtTotal>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <DESCRIPTION>CGST - 1.50%</DESCRIPTION>
                        <AMOUNT>413.10</AMOUNT>
                      </dtTotal>
                      <dtTotal>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <DESCRIPTION>KFC - 0.25%</DESCRIPTION>
                        <AMOUNT>68.85</AMOUNT>
                      </dtTotal>
                      <dtTotal>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <DESCRIPTION>SGST - 1.50%</DESCRIPTION>
                        <AMOUNT>413.10</AMOUNT>
                      </dtTotal>
                      <dtTotal>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <DESCRIPTION>Credit Cards/Debit Card </DESCRIPTION>
                        <AMOUNT>0.00</AMOUNT>
                      </dtTotal>
                      <dtTotal>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <DESCRIPTION>Accor</DESCRIPTION>
                        <AMOUNT>0.00</AMOUNT>
                      </dtTotal>
                      <dtTotal>
                        <TRANSACTIONID>ANG-000151-292</TRANSACTIONID>
                        <DESCRIPTION>Advance Adjustment</DESCRIPTION>
                        <AMOUNT>28435.05</AMOUNT>
                      </dtTotal>
                      <dtTotal>
                        <TRANSACTIONID>ANG-000231-1172</TRANSACTIONID>
                        <DESCRIPTION>Cash Received </DESCRIPTION>
                        <AMOUNT>0.00</AMOUNT>
                      </dtTotal>
                      <dtHeader>
                        <STORENUMBER>ANG</STORENUMBER>
                        <GSTIN>32AABCJ1087G1ZW</GSTIN>
                        <COMPANYNAME>JOYALUKKAS INDIA PRIVATE LIMITED</COMPANYNAME>
                        <ADDRESS1>MAIN ROAD, ANGAMALY ,</ADDRESS1>
                        <ADDRESS2></ADDRESS2>
                        <ADDRESS3>0484 2452999</ADDRESS3>
                        <POSINVOICEHEADER1>Form GST INV-1</POSINVOICEHEADER1>
                        <POSINVOICEHEADER2>Form GST INV-1</POSINVOICEHEADER2>
                        <POSINVOICEHEADER3>(SEE RULE 46 OF CGST RULES 2017)</POSINVOICEHEADER3>
                        <SHIPPEDADDRESS></SHIPPEDADDRESS>
                        <PAN>AABCJ1087G</PAN>
                        <CIN>U51398KL2002PTC015372</CIN>
                        <DATAAREAID>JALK</DATAAREAID>
                        <STATECODE>KL</STATECODE>
                        <STATENAME>KERALA</STATENAME>
                      </dtHeader>
                      <dtInventLocation>
                        <STORE>ANG</STORE>
                        <STATECODE>KERALA</STATECODE>
                        <STATENAME>KERALA</STATENAME>
                        <COUNTRYREGIONID>IND</COUNTRYREGIONID>
                        <GSTIN>32AABCJ1087G1ZW</GSTIN>
                      </dtInventLocation>
                    </TAXINVOICE>";


                                

                    req = System.Net.WebRequest.Create(uri);
                    req.Method = "POST";
                    req.ContentType = "text/xml";
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(req.GetRequestStream());
                    writer.WriteLine(xmlData);
                    writer.Close();
                    rsp = req.GetResponse();



                  

                }
                catch(Exception ex)
                {

                }
               

                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //StreamReader respStream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
                //string receivedResponse = respStream.ReadToEnd();
                //Console.WriteLine(receivedResponse);
                //respStream.Close();
                //response.Close();
            }
            catch (Exception ex)
            {

            }
        }

    }
}