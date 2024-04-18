using Microsoft.Reporting.WebForms;
using System;
using System.Net;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Web.Configuration;
using System.Threading;
using System.Security.Principal;
using System.Security.Authentication;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Web.Services.Description;
using Newtonsoft.Json.Serialization;
using System.Runtime.InteropServices.ComTypes;
using System.Reflection;
using System.Text;

namespace AxPOSWebReport
{
    public partial class frmReportViewer : System.Web.UI.Page
    {
        #region" Sql Connection"
        public SqlConnection con = null;
        public SqlCommand cmd = null;
        public SqlTransaction trans;
        public DataTable dt = null;
        public static string OccuredOn = string.Empty;

        private static List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        private static List<char> characters = new List<char>() {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
                                                        'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
                                                        'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S',
                                                        'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

        public static string GetRandomKey()
        {
            string RandomKey = "";
            Random rand = new Random();
            // run the loop till I get a string of 10 characters
            for (int i = 0; i < 11; i++)
            {
                // Get random numbers, to get either a character or a number...
                int random = rand.Next(0, 3);
                if (random == 1)
                {
                    // use a number
                    random = rand.Next(0, numbers.Count);
                    RandomKey += numbers[random].ToString();
                }
                else
                {
                    random = rand.Next(0, characters.Count);
                    RandomKey += characters[random].ToString();
                }
            }

            return RandomKey;
        }
        public string GetConnectionString()
        {
            // return System.Configuration.ConfigurationManager.AppSettings["DBCON"].ToString();
            return System.Configuration.ConfigurationManager.AppSettings["POSDBCON"].ToString();
        }

        public SqlConnection GetConnection()
        {
            con = new SqlConnection(GetConnectionString());
            con.Open();
            return con;
        }

        public void CloseSqlConnection()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
                con.Dispose();
            }
        }



        public DataTable GetData(string query)
        {
            GetConnection();
            try
            {
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string ReportType = Request["ReportType"];
                string ReceiptID = Request["ReceiptID"];
                string pdfflag = Request["Pdfflag"];
                string Storeid = Request["StoreCode"];

                string EstimateGroup = Request["EstimateGroup"];
                string CustAccount = Request["CustAccount"];
                string TransDate = Request["TransDate"];


                if (ReportType == "0")
                {
                    OccuredOn = "On Load";
                    ShowInvoice(ReceiptID, pdfflag, Storeid, ReportType);
                }
                else if (ReportType == "1")
                {
                    SaleReturn(ReceiptID, Storeid, pdfflag, ReportType);
                }
                else if (ReportType == "2")
                {
                    PurchaseInvoice(ReceiptID, Storeid, pdfflag, ReportType);
                }
                else if (ReportType == "3")
                {
                    ShowAdvanceReceipt(ReceiptID, Storeid, pdfflag, ReportType);
                }
                else if (ReportType == "4")
                {
                    ShowSchemeReceipt(ReceiptID, Storeid, pdfflag, ReportType);
                }
                else if (ReportType == "6")
                {
                    ShowInsurance(ReceiptID, pdfflag, ReportType);
                }
                else if (ReportType == "7")
                {
                    ShowForm60(ReceiptID, pdfflag, ReportType);
                }
                else if (ReportType == "8")
                {
                    string fromdate = Request["fromdate"];
                    string Todate = Request["Todate"];
                    ShowAnalysisReport(fromdate, Todate, ReportType);
                }
                else if (ReportType == "9")
                {
                    string Sectionid = Request["Sectionid"];
                    string Brand = Request["Brand"];
                    string StoreCode = Request["StoreCode"];
                    string OrnamentCode = Request["OrnamentCode"];
                    BrandWiseClosingStock(Sectionid, Brand, OrnamentCode, StoreCode, ReportType);
                }
                else if (ReportType == "10")
                {
                    string TerminalID = Request["TerminalID"];
                    string StoreCode = Request["StoreCode"];

                    CashDailySummaryReport(TerminalID, StoreCode, "0", ReportType);
                }

                else if (ReportType == "11")
                {

                    OfferCreditMemo(ReceiptID, Storeid, pdfflag, ReportType);
                }
                else if (ReportType == "12")
                {

                    ShowSalesOrder(ReceiptID, Storeid, pdfflag, ReportType);
                }
                else if (ReportType == "22")
                {
                    FinalEstimate(CustAccount, EstimateGroup, TransDate, Storeid, pdfflag, ReportType);
                }

                else if (ReportType == "21")
                {
                    RoughEstimate(CustAccount, EstimateGroup, TransDate, Storeid, pdfflag, ReportType);
                }
            }
        }

        public void ShowInvoice(string strReceiptId, string pdfflag, string Storeid, string ReportType)
        {
            try
            {
                Int32 totalAmount = 0;

                string strInvoiceCaption = string.Empty;
                string strInvoiceHeader = string.Empty;
                decimal decTaxableValue = 0;
                string mAmountWords;
                string transid = string.Empty; 
                /////// dtRTT.Rows[0]["TRANSACTIONID"].ToString();
                // string storeno = ""; //////// dtRTST.Rows[0]["STORE"].ToString();
                string strSalesPersonCode = string.Empty;
                string stqry = "";
                double roundoff = 0;
                string strTransDate = "";
                //                            string[] Tender  = new string[5];                        
                DataTable dtTotal;
                DataRow drRow;

                dtTotal = new DataTable();
                dtTotal.Columns.Add("Description");
                dtTotal.Columns.Add("TaxAmount");
                string ConnectionString = ConfigurationManager.AppSettings["POSDBCON"].ToString();/// ConfigurationManager.AppSettings["DBCON"].ToString();
                SqlConnection conn = new SqlConnection(ConnectionString);

                SqlCommand Cmd;
                SqlDataAdapter da;


                if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = "select  rtt.RECEIPTID,rtt.salespaymentdifference ,rtt.TRANSACTIONID, rtt.CHANNEL, rtt.CUSTACCOUNT, rtt.TRANSDATE, STAFF, rtt.Store, rtt.GrossAmount," + Environment.NewLine;
                //stqry += "at.               
                //ISDELIVERYATSTORE from ax.RetailTransactionTable rtt LEFT JOIN ext.ACXRETAILTRANSACTIONTABLE at " + Environment.NewLine;
                //stqry += " on at.TRANSACTIONID = rtt.TRANSACTIONID and at.STORE = rtt.STORE where rtt.RECEIPTID='" + strReceiptId + "' AND ENTRYSTATUS=0";
                OccuredOn = "Get Invoice";
                stqry = "SELECT RECEIPTID,ROUNDOFF AS salespaymentdifference,TRANSACTIONID,CHANNEL,EMAILID,MOBILENUMBER,CUSTACCOUNT,TRANSDATE,STAFF,Store,GrossAmount,ISDELIVERYATSTORE" + Environment.NewLine;
                stqry += "FROM ACXINVOICETABLE" + Environment.NewLine;
                stqry += "WHERE RECEIPTID='" + strReceiptId + "' ";


                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtRTT = new DataTable();
                da.Fill(dtRTT);
                if (dtRTT.Rows.Count > 0)
                {
                    OccuredOn = "Detail Found";
                    /// storeno = dtRTT.Rows[0]["STORE"].ToString();
                    ///totalAmount = Convert.ToInt32(Convert.ToDecimal(dtRTT.Rows[0]["GrossAmount"].ToString()) * -1);
                    strTransDate = Convert.ToDateTime(dtRTT.Rows[0]["TRANSDATE"].ToString()).ToString("dd-MMM-yyyy");
                    transid = dtRTT.Rows[0]["TRANSACTIONID"].ToString();
                    roundoff = Convert.ToDouble(dtRTT.Rows[0]["salespaymentdifference"]);
                }
                else
                {
                    Response.Redirect("ErrorPage.aspx?Error=" + "No Invoice Details Found" + "", false);
                    conn.Close();
                    return;
                }

                if (conn.State != ConnectionState.Open) { conn.Open(); }
                OccuredOn = "Get Invoice Line";
                stqry = "select NAME AS NAMEALIAS,HSNCODE AS CODE,UNITID,COALESCE(GROSSWEIGHT,0.000) as GrossWeight , " + Environment.NewLine;
                stqry += "CASE WHEN UNITID='CT' THEN ROUND(cast(COALESCE(DiamondWeight/5,0.000) as decimal(10,3)),3) ELSE cast(COALESCE(DiamondWeight,0.000) as decimal(10,3) ) END  as DiamondWeight  ,COALESCE(NETWEIGHT,0.000) as NetWeight," + Environment.NewLine;
                stqry += " COALESCE(METALVALUE,0.00) as GoldValue ,COALESCE(VA,0.00) as VA,PCS, 														  " + Environment.NewLine;
                stqry += " COALESCE((STONECVALUE + STONEGVALUE + DIAMONDVALUE),0.00) as StoneValue , 															  " + Environment.NewLine;
                stqry += " COALESCE((METALVALUE+ISNULL(VA,0)+STONECVALUE+STONEGVALUE+DIAMONDVALUE),0.00) as TotalValue 											  " + Environment.NewLine;
                stqry += ",COALESCE(DISCAMOUNT,0.00) as Disc , COALESCE((NetAmount *-1),0.00) as TaxableValue, COALESCE(TagNo,'') as TagNo,						  " + Environment.NewLine;
                stqry += "ReceiptId, Coalesce(Purity,0) as Purity ,COALESCE(BRANDDESC,'') as BrandDesc , COALESCE(PRICE,0) as Price, CASE WHEN COALESCE(NETWEIGHT,0.000)>0 THEN ROUND((COALESCE(METALVALUE,0.00)/COALESCE(NETWEIGHT,0.000)),2) ELSE 0 END  RATE	,					  " + Environment.NewLine;
                stqry += "SALESPERSONCODE from ACXINVOICELINES a  " + Environment.NewLine;
                stqry += "where TRANSACTIONID='" + transid + "'  AND ISPRINT=1 AND ComplimentItem = 0";


                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtRTST = new DataTable();
                da.Fill(dtRTST);
                decTaxableValue = 0;
                OccuredOn = "Get Invoice Line Found";
                if (dtRTST != null)
                {
                    for (int ii = 0; ii < dtRTST.Rows.Count; ii++)
                    {
                        decTaxableValue += Convert.ToDecimal(dtRTST.Rows[ii]["TaxableValue"].ToString());
                        if (strSalesPersonCode == "")
                        {
                            strSalesPersonCode = dtRTST.Rows[ii]["SALESPERSONCODE"].ToString();
                        }
                    }
                }
                drRow = dtTotal.NewRow();
                drRow["Description"] = "Taxable Value";
                drRow["TaxAmount"] = decTaxableValue.ToString("########0.00");
                dtTotal.Rows.Add(drRow);


                SqlDataReader dr;
                conn.Close();
                string Invoicegstdetails = "";
                OccuredOn = "Get Tax Info";
                stqry = "select TAXAMOUNT   as TaxAmount,TAXPERCENTAGE,TAXCOMPONENT from ACXINVOICETAX a      " + Environment.NewLine;
                stqry += " where TRANSACTIONID='" + transid + "'  AND TAXAMOUNT>0 ORDER BY TAXPERCENTAGE DESC  ";
                Cmd = new SqlCommand(stqry);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtGTE = new DataTable();
                da.Fill(dtGTE);
                OccuredOn = "Tax Info Found";
                for (int intGTE = 0; intGTE < dtGTE.Rows.Count; intGTE++)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = dtGTE.Rows[intGTE]["TAXCOMPONENT"].ToString() + " - " + Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXPERCENTAGE"].ToString()).ToString("0.000") + "%";
                    drRow["TaxAmount"] = Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXAMOUNT"].ToString()).ToString("####0.00");
                    decTaxableValue += Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXAMOUNT"].ToString());
                    dtTotal.Rows.Add(drRow);
                    Invoicegstdetails += dtGTE.Rows[intGTE]["TAXCOMPONENT"].ToString() + " - " + Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXPERCENTAGE"].ToString()).ToString("0.000") + "%" + Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXAMOUNT"].ToString()).ToString("####0.00") + "";
                }
                totalAmount = Convert.ToInt32(Convert.ToDecimal(decTaxableValue));
                drRow = dtTotal.NewRow();
                drRow["Description"] = "Total Invoice Value (in Figure) ";
                drRow["TaxAmount"] = decTaxableValue.ToString("####0.00");
                dtTotal.Rows.Add(drRow);
                OccuredOn = "Add Total Tax Detail";
                
                OccuredOn = "Get Payment Info";
                stqry = "select 'Credit Cards/Debit Cards/UPI/Credit Customer' AS Description, COALESCE(SUM(TENDERAMOUNT),0) AS TENDERAMOUNT  " + Environment.NewLine;
                stqry += " from dbo.acxINVOICEPAYMENT  " + Environment.NewLine;
                stqry += "  WHERE  TRANSACTIONID='" + transid + "'  AND ((Convert(int,TENDERTYPE)>=20) AND (Convert(int,TENDERTYPE)<=40 )) " + Environment.NewLine;
                stqry += " HAVING COALESCE(SUM(TENDERAMOUNT),0)>0  " + Environment.NewLine;
                stqry += " UNION ALL  " + Environment.NewLine;
                stqry += "  select TENDERNAME AS Description,  Sum(TENDERAMOUNT)  TENDERAMOUNT   " + Environment.NewLine;
                stqry += " from dbo.acxINVOICEPAYMENT  " + Environment.NewLine;
                stqry += " WHERE    TRANSACTIONID='" + transid + "'  AND  ((Convert(int,TENDERTYPE)<20) OR (Convert(int,TENDERTYPE)>40))  group by TENDERTYPE,TENDERNAME  having COALESCE(SUM(TENDERAMOUNT),0)>0  ";

                Cmd = new SqlCommand(stqry);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtTender = new DataTable();
                da.Fill(dtTender);
                OccuredOn = "Payment Info Found";
                foreach (DataRow row in dtTender.Rows)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = row["Description"].ToString();
                    drRow["TaxAmount"] = Convert.ToDecimal(row["TENDERAMOUNT"].ToString()).ToString("####0.00");
                    dtTotal.Rows.Add(drRow);

                }
                OccuredOn = "Set Payment Info";
                #region"Adv Gen"

                if (conn.State != ConnectionState.Open) { conn.Open(); }
                
                stqry = "select  ca.AMOUNT as Amount from ACXINVOICELINES st join ACXCUSTOMERADVANCE ca "
                      + "on ca.TRANSACTIONID=st.TRANSACTIONID and ca.LINENUM=st.LINENUM  "
                      + "where ca.TRANSACTIONID='" + transid + "' AND st.ISPRINT=1 ";
                OccuredOn = "Get Advance";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;

                DataTable dtAdvTend = new DataTable();
                int intTenderCount = 0;
                da.Fill(dtAdvTend);
                decTaxableValue = 0;
                OccuredOn = "Fetch Advance";
                for (intTenderCount = 0; intTenderCount < dtAdvTend.Rows.Count; intTenderCount++)
                {
                    if (Convert.ToDecimal(dtAdvTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                    {
                        decTaxableValue += Convert.ToDecimal(dtAdvTend.Rows[intTenderCount]["Amount"].ToString());
                    }
                }
                if (decTaxableValue != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Adv Gen";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                OccuredOn = "Set Advance";

                conn.Close();
                #endregion

                #region"RoundOFF"
                if (roundoff != 0.00)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Round Off";
                    drRow["TaxAmount"] = roundoff.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }

                #endregion


                string custqry1 = "Select  CUSTACCOUNT AS ACCOUNTNUM, NAME,replace(replace(COALEsCE(ADDRESS,''),char(10),' '),char(13),' ') as       " + Environment.NewLine;
                custqry1 += "CustAddress, COALEsCE(CITY,'') as CustCity , COALESCE(COUNTRYREGIONID, '') as CustCountRegion,            " + Environment.NewLine;
                custqry1 += "COALESCE(DISTRICTNAME, '') as CustDistrictName , COALESCE(STATECODE, '') as CustStateCode, COALESCE(STREET, '') as CustStreet            " + Environment.NewLine;
                custqry1 += ", COALESCE(ZIPCODE, '') as CustZipCode , COALESCE(PANNUMBER, '') as PANNumber  " + Environment.NewLine;
                custqry1 += ", COALESCE(GSTIN, '') as RegistrationNumber   " + Environment.NewLine;
                custqry1 += ", COALESCE(STATENAME, '') as StateName ,PHONE, COALESCE(EMAILID, '') EMAILID" + Environment.NewLine;
                custqry1 += " From ACXCUSTDETAILS a " + Environment.NewLine;
                custqry1 += "where CUSTACCOUNT  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
                DataTable dtcust;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (Cmd = new SqlCommand(custqry1, conn))
                {
                    using (SqlDataReader reader1 = Cmd.ExecuteReader())
                    {
                        using (DataTable transtable1 = new DataTable())
                        {
                            transtable1.Load(reader1);
                            dtcust = transtable1.Copy();
                            if (dtcust.Rows.Count <= 0)
                            {
                                Response.Redirect("ErrorPage.aspx?Error=" + "Customer Details Not Found" + "", false);
                                conn.Close();
                                return;
                            }
                        }
                    }
                }

                // for customer Phone

                string strPhone = string.Empty;
                strPhone = dtcust.Rows[0]["PHONE"].ToString();
                //strPhone = dtcust.Rows[0]["EMAILID"].ToString();

                string InventLocationqry1 = "Select Coalesce(STATENAME, '') as StateName, COALESCE(GSTIN,'') as RegistrationNumber "
                               + ",Coalesce(STATECODE,'') as StateCode, Coalesce(COUNTRYREGIONID,'') as CountryRegion "
                               + " From ACXINVENTLOCATION a "
                               + "where STORE = '" + Storeid + "' ";

                DataTable dtInventLocation;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (Cmd = new SqlCommand(InventLocationqry1, conn))
                {
                    using (SqlDataReader reader1 = Cmd.ExecuteReader())
                    {
                        using (DataTable transtable1 = new DataTable())
                        {
                            transtable1.Load(reader1);
                            dtInventLocation = transtable1.Copy();
                        }
                    }
                }
                ////dr.Close();
                strInvoiceCaption = "TAX INVOICE";
                // Old Gold Weight
                conn.Close();
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string strUserId = "";
                /*
              string qEmplName = "select a.STORENUMBER,b.ADDRESSBOOK,c.PARTY, d.PERSONNELNUMBER,e.NAME, '' as ACX_POSINVOICEHEADER1 from ax.RETAILSTORETABLE as a "
                                + "inner join ax.retailStoreAddressBook as b  on b.STORERECID = a.RECID and b.ADDRESSBOOKTYPE = 1 "
                                + "inner join ax.DIRADDRESSBOOKPARTY as c on c.ADDRESSBOOK = b.ADDRESSBOOK "
                                + "inner join ax.HCMWORKER as d on d.PERSON = c.PARTY "
                                + "inner join ax.DIRPARTYTABLE as e on e.RECID = c.PARTY "
                                + "where d.PERSONNELNUMBER = '" + strSalesPersonCode + "' ";


                using (SqlCommand command6 = new SqlCommand(qEmplName, conn))
                {
                    using (SqlDataReader readerEmpl = command6.ExecuteReader())
                    {
                        using (DataTable dtEmpl = new DataTable())
                        {
                            dtEmpl.Load(readerEmpl);
                            if (dtEmpl.Rows.Count > 0)
                            {
                                strUserId = dtEmpl.Rows[0]["PERSONNELNUMBER"].ToString() + " - " + dtEmpl.Rows[0]["Name"].ToString();
                                strInvoiceHeader = dtEmpl.Rows[0]["ACX_POSINVOICEHEADER1"].ToString();
                            }
                        }
                    }
                }
                conn.Close();

                 * */


                string mStoreName = "";
                
                stqry = " select GSTIN,COMPANYNAME,ADDRESS1,ADDRESS2,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3 "
                                + ", PAN, CIN, STATECODE, STATENAME as NAME,(ADDRESS1+ +ADDRESS2+ +ADDRESS3) AS  StoreAddress, "
                                + "BankName As BANKACCOUNTNAME,BANKACCOUNTNO ,IFSC  AS IFSCCODE, UPIID  from ACXSTOREDETAILS a "
                                + " where a.STORENUMBER='" + Storeid + "' ";
                Cmd = new SqlCommand(stqry);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtHeader = new DataTable();
                da.Fill(dtHeader);

                string Shippingaddress = "";
                if (dtRTT.Rows.Count > 0)
                {
                    dtHeader.Columns.Add(new DataColumn("ShippedAddress", typeof(string)));
                    dtHeader.Columns.Add(new DataColumn("StateCode", typeof(string)));
                    dtHeader.Columns.Add(new DataColumn("StateName", typeof(string)));
                    dtHeader.Columns["ShippedAddress"].ReadOnly = false;
                    dtInventLocation.Columns["StateCode"].ReadOnly = false;
                    dtInventLocation.Columns["StateName"].ReadOnly = false;
                    dtInventLocation.Columns["RegistrationNumber"].ReadOnly = false;
                    DataRow rwinvlocation = dtInventLocation.NewRow();
                    dtInventLocation.Rows.Add(rwinvlocation);
                    if (Convert.ToInt32(dtRTT.Rows[0]["ISDELIVERYATSTORE"]) == 0)
                    {
                        if (dtcust.Rows.Count > 0)
                            Shippingaddress = dtcust.Rows[0]["CustAddress"].ToString() + ' ' + dtcust.Rows[0]["CustCity"].ToString();
                        dtInventLocation.Rows[0]["StateCode"] = dtcust.Rows[0]["CustStateCode"];
                        dtInventLocation.Rows[0]["StateName"] = dtcust.Rows[0]["StateName"];
                        dtInventLocation.Rows[0]["RegistrationNumber"] = dtcust.Rows[0]["RegistrationNumber"];
                    }
                    else if (Convert.ToInt32(dtRTT.Rows[0]["ISDELIVERYATSTORE"]) == 1)
                    {
                        //Remove this work on 25-12-2020 when new point issue  given by  joy team " Delivery at shop -We need only shop details  in "details of consignee" side"
                        //Add this two new field  of shop address and mobile no
                        Shippingaddress = "Supply at shop";
                        dtInventLocation.Rows[0]["RegistrationNumber"] = "";
                        //Remove this work on 12-02-2021 when new point issue  given by  joy team " Delivery at shop -We need only supply at shop"
                        //Shippingaddress = dtHeader.Rows[0]["ADDRESS1"].ToString()+' '+dtHeader.Rows[0]["ADDRESS2"].ToString();
                        //strPhone = dtHeader.Rows[0]["ADDRESS3"].ToString();
                        //end
                        //dtHeader.Rows[0]["StateCode"] = dtcust.Rows[0]["CustStateCode"];
                        //dtHeader.Rows[0]["StateName"] = dtcust.Rows[0]["StateName"];
                    }
                }

                #region"Show The Complimentry item"

                stqry = "select NAME AS ItemName,case UNITID when 'g' then ABS(PCS) else QTY end As PCS" + Environment.NewLine;
                stqry += "from ACXINVOICELINES a  " + Environment.NewLine;
                //stqry += "where TRANSACTIONID='" + transid + "'  AND ISPRINT=0 ";
                //Print Complimentry item
                stqry += "where TRANSACTIONID='" + transid + "'  AND ISPRINT = 1 AND ComplimentItem = 1";

                DataTable dtComp = new DataTable();
                using (Cmd = new SqlCommand(stqry, conn))
                {
                    using (SqlDataReader reader1 = Cmd.ExecuteReader())
                    {
                        using (DataTable transtable1 = new DataTable())
                        {
                            transtable1.Load(reader1);
                            dtComp = transtable1.Copy();
                        }
                    }
                }

                #endregion

                #region "Data need to generate code"

                string gstinno = "";
                string supplierupiid = "";
                string Bankdetails = "";
                string IFSCCode = "";
                string Bankaccountname = "";
                if (dtHeader.Rows.Count > 0)
                {
                    gstinno = dtHeader.Rows[0]["GSTIN"].ToString();
                    supplierupiid = dtHeader.Rows[0]["UPIID"].ToString();
                    Bankdetails = dtHeader.Rows[0]["BANKACCOUNTNO"].ToString();
                    IFSCCode = dtHeader.Rows[0]["IFSCCODE"].ToString();
                    Bankaccountname = dtHeader.Rows[0]["BANKACCOUNTNAME"].ToString();
                }


                string Qrcode = GenrateQrCode(strReceiptId, strTransDate, gstinno, supplierupiid, Bankdetails, IFSCCode, Invoicegstdetails, totalAmount, Bankaccountname);
                #endregion

                mAmountWords = words(totalAmount);
                ReportDataSource RDS = new ReportDataSource("Dset", dtRTT);                

                rptViewr.LocalReport.DataSources.Add(RDS);
                //if (dtcust.Rows.Count > 0 &&( dtComp.Rows.Count >0 || dtComp != null))
                if (dtcust.Rows.Count > 0 && dtComp.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtcust.Rows[0]["RegistrationNumber"].ToString()))
                    {
                        rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SalesInvoiceQR.rdl");
                    }
                    else
                    {
                        rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SalesInvoice.rdl");
                    }
                    
                }
                else if (dtcust.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtcust.Rows[0]["RegistrationNumber"].ToString()))
                    {
                        rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SalesInvoiceQRWithoutCompliments.rdl");
                    }
                    else
                    {
                        rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SalesInvoiceWithoutCompliments.rdl");

                    }
                   
                }
                OccuredOn = "Set Data Set Header";
                rptViewr.LocalReport.DataSources.Clear();
                var Rds = new ReportDataSource();
                Rds.Name = "Header";
                Rds.Value = dtHeader;
                rptViewr.LocalReport.DataSources.Add(Rds);
                OccuredOn = "Set Data Set Line";
                var RdsHeader = new ReportDataSource();
                RdsHeader.Name = "ldata";
                RdsHeader.Value = dtRTST;
                rptViewr.LocalReport.DataSources.Add(RdsHeader);
                OccuredOn = "Set Data Set Cust";
                var RdsCustDetail = new ReportDataSource();
                RdsCustDetail.Name = "CustomerDetail";
                RdsCustDetail.Value = dtcust;
                rptViewr.LocalReport.DataSources.Add(RdsCustDetail);
                OccuredOn = "Set Data Set Location";
                var RdsInventLocation = new ReportDataSource();
                RdsInventLocation.Name = "InventLocationDetail";
                RdsInventLocation.Value = dtInventLocation;
                rptViewr.LocalReport.DataSources.Add(RdsInventLocation);
                OccuredOn = "Set Data Set Total";
                var RdsTotal = new ReportDataSource();
                RdsTotal.Name = "ITotal";
                RdsTotal.Value = dtTotal;
                rptViewr.LocalReport.DataSources.Add(RdsTotal);
                OccuredOn = "Set Data Set Complimentary";
                var RdsComp = new ReportDataSource();
                RdsComp.Name = "ICOMP";
                RdsComp.Value = dtComp;
                rptViewr.LocalReport.DataSources.Add(RdsComp);
                OccuredOn = "Set Param Set Amt in Words";

                ReportParameter parameter = new ReportParameter();
                parameter.Name = "AmtInWords";
                parameter.Values.Add(mAmountWords);
                rptViewr.LocalReport.SetParameters(parameter);
                OccuredOn = "Set Param Set User";

                ReportParameter uname = new ReportParameter();
                uname.Name = "UserName";
                uname.Values.Add(strUserId);
                rptViewr.LocalReport.SetParameters(uname);
                OccuredOn = "Set Param Invoice date";

                ReportParameter InvoiceDate = new ReportParameter();
                InvoiceDate.Name = "InvoiceDate";
                InvoiceDate.Values.Add(strTransDate);
                rptViewr.LocalReport.SetParameters(InvoiceDate);
                OccuredOn = "Set Param Set Receipt Id";

                ReportParameter ReceiptId = new ReportParameter();
                ReceiptId.Name = "ReceiptId";
                ReceiptId.Values.Add(strReceiptId);
                rptViewr.LocalReport.SetParameters(ReceiptId);
                OccuredOn = "Set Param Set Store Name";

                ReportParameter SName1 = new ReportParameter();
                SName1.Name = "StoreName";
                SName1.Values.Add(mStoreName);
                rptViewr.LocalReport.SetParameters(SName1);
                OccuredOn = "Set Param Set Cust Phone";

                ReportParameter custPhone = new ReportParameter();
                custPhone.Name = "CustPhone";
                custPhone.Values.Add(strPhone);
                rptViewr.LocalReport.SetParameters(custPhone);
                OccuredOn = "Set Param Set Shipping Address";

                ReportParameter shipping = new ReportParameter();
                shipping.Name = "Shippingaddress";
                shipping.Values.Add(Shippingaddress);
                rptViewr.LocalReport.SetParameters(shipping);
                OccuredOn = "Set Param Set QR";

                ReportParameter QRCODE = new ReportParameter();
                QRCODE.Name = "QRCODE";
                QRCODE.Values.Add(Qrcode);
                rptViewr.LocalReport.SetParameters(QRCODE);
                OccuredOn = "Set File Name";

                string pdfFileName = String.Concat(strReceiptId, "_", transid, String.Concat("_", ReportType));
                OccuredOn = "File Name is  " + pdfFileName + "::" + pdfflag;
                string reportUrl = ConfigurationManager.AppSettings["DOCUMENTURL"].ToString();
                OccuredOn = "Set Report Url " + reportUrl;

                rptViewr.LocalReport.DisplayName = strReceiptId + "_Sale Invoice";

                if (pdfflag == "0")
                {
                    OccuredOn = "Loading Report";
                    rptViewr.LocalReport.Refresh();
                    var reportPath = showPDF(reportUrl, pdfFileName);
                    OccuredOn = "Call Digital";
                    string strcustAccount = dtRTT.Rows[0]["CUSTACCOUNT"].ToString();
                    string strReportType = ReportType;
                    string strStoreId = dtRTT.Rows[0]["STORE"].ToString();
                    DateTime dtExpiryDate = Convert.ToDateTime("2050-12-31");
                    SendDigitalInvoice(strReceiptId, pdfflag, Storeid, ReportType, dtTotal,
                        dtRTT, dtRTST, dtGTE, dtTender, dtAdvTend, dtcust, dtInventLocation,
                        dtHeader, dtComp, Qrcode,strcustAccount,strReportType,strStoreId,dtExpiryDate);
                    

                  
               }
               else if (pdfflag == "1")
               {
                   DownloadPdf(strReceiptId + "_" + ReportType);
               }
               else if (pdfflag == "2")
               {
                   string savePath = Server.MapPath("DownloadInvoice\\Invoice.pdf");
                   SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
               }
               conn.Close();
           }
           catch (Exception ex)
           {
               Response.Redirect("ErrorPage.aspx?Error=" + OccuredOn + "::" + ex.Message.ToString());
               /////MessageBox.Show(ex.Message.ToString());
           }

       }



       public void SendDigitalInvoice(string strReceiptId, string pdfflag, string Storeid, string ReportType, DataTable dtTotal,
           DataTable dtRTT, DataTable dtRTST, DataTable dtGTE, DataTable dtTender, DataTable dtAdvTend, DataTable dtcust, DataTable dtInventLocation,
           DataTable dtHeader, DataTable dtComp, string Qrcode,string strcustAccount,string strReportType,string strStoreId,DateTime dtExpiryDate)
       {
           try
           {
                Int32 totalAmount = 0;

                string strInvoiceCaption = string.Empty;
                string strInvoiceHeader = string.Empty;
                decimal decTaxableValue = 0;
                string mAmountWords;
                string transid = string.Empty;
                string strSalesPersonCode = string.Empty;
                
                double roundoff = 0;
                string strTransDate = "";
                
                if (dtRTT.Rows.Count > 0)
                {
                    strTransDate = Convert.ToDateTime(dtRTT.Rows[0]["TRANSDATE"].ToString()).ToString("dd-MMM-yyyy");
                    transid = dtRTT.Rows[0]["TRANSACTIONID"].ToString();
                    roundoff = Convert.ToDouble(dtRTT.Rows[0]["salespaymentdifference"]);
                }
                else
                {
                    Response.Redirect("ErrorPage.aspx?Error=" + "No Invoice Details Found" + "", false);
                    
                    return;
                }

               
                decTaxableValue = 0;
                for (int ii = 0; ii < dtRTST.Rows.Count; ii++)
                {
                    decTaxableValue += Convert.ToDecimal(dtRTST.Rows[ii]["TaxableValue"].ToString());
                    if (strSalesPersonCode == "")
                    {
                        strSalesPersonCode = dtRTST.Rows[ii]["SALESPERSONCODE"].ToString();
                    }
                }
                

                string Invoicegstdetails = "";

                for (int intGTE = 0; intGTE < dtGTE.Rows.Count; intGTE++)
                {
                    decTaxableValue += Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXAMOUNT"].ToString());
                   
                    Invoicegstdetails += dtGTE.Rows[intGTE]["TAXCOMPONENT"].ToString() + " - " + Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXPERCENTAGE"].ToString()).ToString("0.000") + "%" + Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXAMOUNT"].ToString()).ToString("####0.00") + "";
                }
                totalAmount = Convert.ToInt32(Convert.ToDecimal(decTaxableValue));
                

                #region"Adv Gen"

                int intTenderCount = 0;
                decTaxableValue = 0;
                for (intTenderCount = 0; intTenderCount < dtAdvTend.Rows.Count; intTenderCount++)
                {
                    if (Convert.ToDecimal(dtAdvTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                    {
                        decTaxableValue += Convert.ToDecimal(dtAdvTend.Rows[intTenderCount]["Amount"].ToString());
                    }
                }

                #endregion
                // for customer Phone
                string strPhone, strEmail, custName, strEmailContent = string.Empty;
                strPhone = dtRTT.Rows[0]["MOBILENUMBER"].ToString();
                strEmail = dtRTT.Rows[0]["EMAILID"].ToString();

                custName = dtcust.Rows[0]["NAME"].ToString();
                strEmailContent = "Please find your digital invoice copy. Click on the link ";
                strInvoiceCaption = "TAX INVOICE";                
                string strUserId = "";
                string mStoreName = "";
                

                string Shippingaddress = "";
                if (dtRTT.Rows.Count > 0)
                {
                    
                    
                    if (Convert.ToInt32(dtRTT.Rows[0]["ISDELIVERYATSTORE"]) == 0)
                    {
                        if (dtcust.Rows.Count > 0)
                            Shippingaddress = dtcust.Rows[0]["CustAddress"].ToString() + ' ' + dtcust.Rows[0]["CustCity"].ToString();
                        
                    }
                    else if (Convert.ToInt32(dtRTT.Rows[0]["ISDELIVERYATSTORE"]) == 1)
                    {
                        //Remove this work on 25-12-2020 when new point issue  given by  joy team " Delivery at shop -We need only shop details  in "details of consignee" side"
                        //Add this two new field  of shop address and mobile no
                        Shippingaddress = "Supply at shop";
                        dtInventLocation.Rows[0]["RegistrationNumber"] = "";
                    }
                }


                mAmountWords = words(totalAmount);
                ReportDataSource RDS = new ReportDataSource("Dset", dtRTT);

                rptViewr.LocalReport.DataSources.Add(RDS);
                //if (dtcust.Rows.Count > 0 &&( dtComp.Rows.Count >0 || dtComp != null))
                if (dtcust.Rows.Count > 0 && dtComp.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtcust.Rows[0]["RegistrationNumber"].ToString()))
                    {
                        rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SalesInvoiceQRDigital.rdl");
                    }
                    else
                    {
                        rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SalesInvoiceDigital.rdl");
                    }
                    
                }
                else if (dtcust.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtcust.Rows[0]["RegistrationNumber"].ToString()))
                    {
                        rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SalesInvoiceQRWithoutComplimentsDigital.rdl");
                    }
                    else
                    {
                        rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SalesInvoiceWithoutComplimentsDigital.rdl");

                    }
                   
                }

                rptViewr.LocalReport.DataSources.Clear();
                var Rds = new ReportDataSource();
                Rds.Name = "Header";
                Rds.Value = dtHeader;
                rptViewr.LocalReport.DataSources.Add(Rds);

                var RdsHeader = new ReportDataSource();
                RdsHeader.Name = "ldata";
                RdsHeader.Value = dtRTST;
                rptViewr.LocalReport.DataSources.Add(RdsHeader);

                var RdsCustDetail = new ReportDataSource();
                RdsCustDetail.Name = "CustomerDetail";
                RdsCustDetail.Value = dtcust;
                rptViewr.LocalReport.DataSources.Add(RdsCustDetail);

                var RdsInventLocation = new ReportDataSource();
                RdsInventLocation.Name = "InventLocationDetail";
                RdsInventLocation.Value = dtInventLocation;
                rptViewr.LocalReport.DataSources.Add(RdsInventLocation);

                var RdsTotal = new ReportDataSource();
                RdsTotal.Name = "ITotal";
                RdsTotal.Value = dtTotal;
                rptViewr.LocalReport.DataSources.Add(RdsTotal);

                var RdsComp = new ReportDataSource();
                RdsComp.Name = "ICOMP";
                RdsComp.Value = dtComp;
                rptViewr.LocalReport.DataSources.Add(RdsComp);

                ReportParameter parameter = new ReportParameter();
                parameter.Name = "AmtInWords";
                parameter.Values.Add(mAmountWords);
                rptViewr.LocalReport.SetParameters(parameter);

                ReportParameter uname = new ReportParameter();
                uname.Name = "UserName";
                uname.Values.Add(strUserId);
                rptViewr.LocalReport.SetParameters(uname);

                ReportParameter InvoiceDate = new ReportParameter();
                InvoiceDate.Name = "InvoiceDate";
                InvoiceDate.Values.Add(strTransDate);
                rptViewr.LocalReport.SetParameters(InvoiceDate);

                ReportParameter ReceiptId = new ReportParameter();
                ReceiptId.Name = "ReceiptId";
                ReceiptId.Values.Add(strReceiptId);
                rptViewr.LocalReport.SetParameters(ReceiptId);

                ReportParameter SName1 = new ReportParameter();
                SName1.Name = "StoreName";
                SName1.Values.Add(mStoreName);
                rptViewr.LocalReport.SetParameters(SName1);

                ReportParameter custPhone = new ReportParameter();
                custPhone.Name = "CustPhone";
                custPhone.Values.Add(strPhone);
                rptViewr.LocalReport.SetParameters(custPhone);

                ReportParameter shipping = new ReportParameter();
                shipping.Name = "Shippingaddress";
                shipping.Values.Add(Shippingaddress);
                rptViewr.LocalReport.SetParameters(shipping);

                ReportParameter QRCODE = new ReportParameter();
                QRCODE.Name = "QRCODE";
                QRCODE.Values.Add(Qrcode);
                rptViewr.LocalReport.SetParameters(QRCODE);

                string pdfFileName = String.Concat(strReceiptId, "_", transid, String.Concat("_", ReportType));
                string reportUrl = ConfigurationManager.AppSettings["DOCUMENTURL"].ToString();

                rptViewr.LocalReport.DisplayName = strReceiptId + "_Digital Sale Invoice";

                rptViewr.LocalReport.Refresh();

                new Thread(async () =>
                {
                    Thread.CurrentThread.IsBackground = true;
                     
                    var reportPath = await createPDF(reportUrl, pdfFileName).ConfigureAwait(false);
                    String stqry = "SELECT * FROM ACXDIGITALINVOICE WHERE RECEIPTID='" + strReceiptId + "'";
                    string ConnectionString = ConfigurationManager.AppSettings["POSDBCON"].ToString();

                    SqlConnection conn = new SqlConnection(ConnectionString);
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    SqlCommand Cmd = new SqlCommand(stqry);
                    Cmd.Connection = conn;
                    SqlDataAdapter da = new SqlDataAdapter(Cmd);
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.CommandTimeout = 120;
                    DataTable dtDigital = new DataTable();
                    da.Fill(dtDigital);
                    bool isFileExits = false;
                    string strKey = string.Empty;

                    if (dtDigital!=null)
                    {
                        if (dtDigital.Rows.Count > 0)
                        {
                            isFileExits = true;
                            strKey = dtDigital.Rows[0]["SECRETKEY"].ToString();
                        }
                    }

                    //await WriteLog("PDF Created" +"," + transid + "," + strEmail + "," + custName + "," + pdfFileName + "," + reportPath).ConfigureAwait(false);
                    string uploaded_path = string.Empty;
                    pdfFileName = Convert.ToDateTime(strTransDate).ToString("yyyy") + "\\" + Convert.ToDateTime(strTransDate).ToString("MMM") + "\\" + pdfFileName;
                    if (!isFileExits)
                        uploaded_path = await UploadFiles(reportPath, pdfFileName).ConfigureAwait(false);
                    else
                        uploaded_path = Convert.ToString(dtDigital.Rows[0]["BLOBPATH"]);
                    int inCount = 0;
                    if (!string.IsNullOrWhiteSpace(uploaded_path) && isFileExits==false)
                    {
                    NextRetry:
                        try
                        {
                            inCount += 1;
                            strKey = GetRandomKey();
                            string strQuery = "INSERT INTO ACXDIGITALINVOICE (DATAAREAID,CUSTACCOUNT,REPORTTYPE," + Environment.NewLine;
                            strQuery += "TRANSACTIONID,RECEIPTID,STORE,SECRETKEY,BLOBPATH,EXPIRYDATE,CONTAINERNAME) " + Environment.NewLine;
                            strQuery += "VALUES ('','" + strcustAccount + "'," + Environment.NewLine;//strReportType
                            strQuery += "'" + strReportType +"',"  + Environment.NewLine;
                            strQuery += "'" + dtRTT.Rows[0]["TRANSACTIONID"].ToString() + "'," + Environment.NewLine;
                            strQuery += "'" + strReceiptId + "'," + Environment.NewLine;
                            strQuery += "'" + strStoreId +"'," + Environment.NewLine;
                            strQuery += "'" + strKey +"'," + Environment.NewLine;
                            strQuery += "'" + uploaded_path +"'," + Environment.NewLine;
                            strQuery += "'" + dtExpiryDate.ToString("yyyy-MM-dd") +"'," + Environment.NewLine;
                            strQuery += "'')";
                            cmd = new SqlCommand(strQuery, conn);
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            if (inCount <= 3)
                                goto NextRetry;    
                        }
                    }
                    //else
                    //{
                    //    strKey = "";
                    //}
                    //await WriteLog("Uploaded path" + "," + transid + "," + strEmail + "," + custName + "," + pdfFileName + "," + uploaded_path).ConfigureAwait(false);

                    if (strPhone.Length >= 10 && string.IsNullOrWhiteSpace(uploaded_path)==false)
                    {
                        string strresp = await SendWhatsapp(uploaded_path, transid, strPhone,strKey).ConfigureAwait(false);
                        if (strresp == null)
                            await WriteLog("Whatsapp" + "," + transid + "," + strPhone + "," + custName + "," + "whats app not send" + "," + uploaded_path).ConfigureAwait(false);
                        else
                            await WriteLog("Whatsapp" + "," + transid + "," + strPhone + "," + custName + "," + "whats send" + "," + strresp).ConfigureAwait(false);

                    }
                    else
                    {
                        await WriteLog("Whatsapp" + "," + transid + "," + strEmail + "," + custName + "," + "Phone no not exists" + "," + uploaded_path).ConfigureAwait(false);
                    }
                    //SendWhatsapp(uploaded_path, pdfFileName, "9795131800");

                    if (strEmail != string.Empty && string.IsNullOrWhiteSpace(uploaded_path) == false)
                    {
                        await SendMail(uploaded_path, transid, strEmail,
                            strTransDate,
                            custName,
                            pdfFileName,
                            strEmailContent).ConfigureAwait(false);
                    }
                    else
                    {
                        await WriteLog("Email," +transid + "," + strEmail + "," + "Not Send" + "," + pdfFileName + "," + uploaded_path).ConfigureAwait(false);

                    }
                    //SendMail(uploaded_path, transid, "abhishek@triserv360.com",
                    //    "13-04-2023",
                    //    "Abhishek Dheeman",
                    //    pdfFileName,
                    //    "Test");

                }).Start();

               
            }
            catch (Exception ex)
            {
                WriteLog("SendDigitalInvoice,,,Not Send,,"+ex.Message);

                //Response.Redirect("ErrorPage.aspx?Error=" + ex.Message.ToString() );
                /////MessageBox.Show(ex.Message.ToString());
            }

        }

        public void ShowInsurance(string strReceiptId, string pdfflag, string ReportType)
        {
            try
            {
                Int32 totalAmount = 0;

                string strInvoiceCaption = string.Empty;
                string strInvoiceHeader = string.Empty;
                string mAmountWords;
                string transid = string.Empty;
                string storeno = "";
                string strSalesPersonCode = string.Empty;
                string stqry = "";
                string strCustomerName = string.Empty;

                string strTransDate = "";
                string ConnectionString = ConfigurationManager.AppSettings["POSDBCON"].ToString();
                SqlConnection conn = new SqlConnection(ConnectionString);

                SqlCommand Cmd;
                SqlDataAdapter da;


                if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " select  rtt.RECEIPTID, rtt.TRANSACTIONID, rtt.CHANNEL, rtt.CUSTACCOUNT, rtt.TRANSDATE, STAFF, rtt.Store, rtt.GrossAmount " +
                //            " from ax.RetailTransactionTable rtt " +
                //            " where rtt.ReceiptID='" + strReceiptId + "' AND ENTRYSTATUS=0 ";

                stqry = "SELECT RECEIPTID,TRANSACTIONID,CHANNEL,CUSTACCOUNT,TRANSDATE,STAFF,Store,GrossAmount" + Environment.NewLine;
                stqry += "FROM ACXINVOICETABLE" + Environment.NewLine;
                stqry += "WHERE RECEIPTID='" + strReceiptId + "' ";


                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtRTT = new DataTable();
                da.Fill(dtRTT);

                //InsuranceHeader

                if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " Select TRANSACTIONID,RECEIPTID,STOREID,INSURANCEAMOUNT,RETAILINSURANCENO,INSURANCEDATE,POLICYFROMDATE "
                //      + ",POLICYTODATE,MASTERPOLICYNO,GROUPPOLICYHOLDERNAME,AUTHORISEDSIGNATORY,PERIODINSURANCEDAYS "
                //      + "From ext.ACXRETAILINVOICETABLE "
                //      + "where RECEIPTID='" + strReceiptId + "' ";

                stqry = "Select TRANSACTIONID,RECEIPTID,STORE  AS STOREID,INSURANCEAMOUNT,RETAILINSURANCENO,convert(varchar,INSURANCEDATE,105) INSURANCEDATE,convert(varchar,convert(varchar,POLICYFROMDATE,105)+' 12:00 AM') POLICYFROMDATE, "
                      + "convert(varchar,convert(varchar,POLICYTODATE,105)+' 11:59 PM') POLICYTODATE ,MASTERPOLICYNO,GROUPPOLICYHOLDERNAME,AUTHORISEDSIGNATORY,PERIODINSURANCEDAYS "
                     + "From ACXINSURANCETABLE  "
                      + " where RECEIPTID='" + strReceiptId + "' ";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtHeader = new DataTable();
                da.Fill(dtHeader);


                ///Detail

                strTransDate = Convert.ToDateTime(dtRTT.Rows[0]["TRANSDATE"].ToString()).ToString("dd-MMM-yyyy");
                transid = dtRTT.Rows[0]["TRANSACTIONID"].ToString();
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " select b.ITEMID,b.NAMEALIAS "
                //      + ", COALESCE((a.NetAmount *-1) + (a.TaxAmount *-1) ,0.00) as InsAmount "
                //      + "from ax.RetailTransactionSalesTrans a "
                //      + "INNER JOIN ax.INVENTTABLE b on a.ITEMID=b.ITEMID "
                //      + "where a.RECEIPTID='" + strReceiptId + "' AND a.TRANSACTIONSTATUS=0 ";
                stqry = " SELECT ITEMID,NAME AS NAMEALIAS,COALESCE((NetAmount *-1) + (TaxAmount *-1) ,0.00) as InsAmount "
                       + " FROM ACXINVOICELINES A "
                       + " JOIN ACXINVOICETABLE B "
                       + " ON A.TRANSACTIONID=B.TRANSACTIONID "
                       + " WHERE B.RECEIPTID='" + strReceiptId + "' AND A.ISPRINT=1";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtRTST = new DataTable();
                da.Fill(dtRTST);


                conn.Close();
                
                stqry = "select  STATENAME from ACXSTOREDETAILS where STORENUMBER= '" + dtRTT.Rows[0]["Store"].ToString() + "'";
                   
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtstore = new DataTable();
                da.Fill(dtstore);

                // for customer detail
                //string custqry1 = "Select ACCOUNTNUM,b.NAME "
                //                + " From ax.CUSTTABLE a "
                //                + "INNER JOIN ax.DIRPARTYTABLE b on a.PARTY = b.RECID "
                //                + "where ACCOUNTNUM  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";

              //  string custqry1 = "SELECT CUSTACCOUNT AS  ACCOUNTNUM,NAME "
              //                   + "FROM ACXCUSTDETAILS "
              //                   + "WHERE CUSTACCOUNT='" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";

                string custqry1 = "SELECT CUSTACCOUNT AS ACCOUNTNUM,NAME, " + Environment.NewLine;
                custqry1 += "replace(replace(COALEsCE(ADDRESS,''),char(10),' '),char(13),' ') as CustAddress,  " + Environment.NewLine;
                custqry1 += "COALEsCE(CITY,'') as CustCity,COALESCE(COUNTRYREGIONID, '') as CustCountRegion,  " + Environment.NewLine;
                custqry1 += "COALESCE(DISTRICTNAME, '') as CustDistrictName,  " + Environment.NewLine;
                custqry1 += "COALESCE(STATECODE, '') as CustStateCode, COALESCE(STREET, '') as CustStreet,  " + Environment.NewLine;
                custqry1 += "COALESCE(ZIPCODE, '') as CustZipCode,COALESCE(PANNUMBER, '') as PANNumber, " + Environment.NewLine;
                custqry1 += "COALESCE(GSTIN ,'')  as RegistrationNumber,COALESCE(STATENAME, '') as StateName,PHONE  " + Environment.NewLine;
                custqry1 += "FROM DBO.ACXCUSTDETAILS" + Environment.NewLine;
                custqry1 += "WHERE CUSTACCOUNT= '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";

                DataTable dtcust;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (Cmd = new SqlCommand(custqry1, conn))
                {
                    using (SqlDataReader reader1 = Cmd.ExecuteReader())
                    {
                        using (DataTable transtable1 = new DataTable())
                        {
                            transtable1.Load(reader1);
                            dtcust = transtable1.Copy();
                            if (dtcust.Rows.Count > 0)
                            {
                                strCustomerName = dtcust.Rows[0]["NAME"].ToString();
                                dtHeader.Columns.Add(new DataColumn("CustAddress", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("CustCity", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("CustCountRegion", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("CustDistrictName", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("CustStateCode", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("CustStreet", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("CustZipCode", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("PANNumber", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("RegistrationNumber", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("StateName", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("PHONE", typeof(string)));
                                dtHeader.Columns.Add(new DataColumn("STORECITY", typeof(string)));
                                dtHeader.Columns["CustAddress"].ReadOnly = false;
                                dtHeader.Columns["CustCity"].ReadOnly = false;
                                dtHeader.Columns["CustCountRegion"].ReadOnly = false;
                                dtHeader.Columns["CustDistrictName"].ReadOnly = false;
                                dtHeader.Columns["CustStateCode"].ReadOnly = false;
                                dtHeader.Columns["CustStreet"].ReadOnly = false;
                                dtHeader.Columns["CustZipCode"].ReadOnly = false;
                                dtHeader.Columns["PANNumber"].ReadOnly = false;
                                dtHeader.Columns["RegistrationNumber"].ReadOnly = false;
                                dtHeader.Columns["StateName"].ReadOnly = false;
                                dtHeader.Columns["PHONE"].ReadOnly = false;
                                dtHeader.Columns["STORECITY"].ReadOnly = false;
                                dtHeader.Rows[0]["CustAddress"] = dtcust.Rows[0]["CustAddress"];
                                dtHeader.Rows[0]["CustCity"] = dtcust.Rows[0]["CustCity"];
                                dtHeader.Rows[0]["CustCountRegion"] = dtcust.Rows[0]["CustCountRegion"];
                                dtHeader.Rows[0]["CustDistrictName"] = dtcust.Rows[0]["CustDistrictName"];
                                dtHeader.Rows[0]["CustStateCode"] = dtcust.Rows[0]["CustStateCode"];
                                dtHeader.Rows[0]["CustStreet"] = dtcust.Rows[0]["CustStreet"];
                                dtHeader.Rows[0]["CustZipCode"] = dtcust.Rows[0]["CustZipCode"];
                                dtHeader.Rows[0]["PANNumber"] = dtcust.Rows[0]["PANNumber"];
                                dtHeader.Rows[0]["RegistrationNumber"] = dtcust.Rows[0]["RegistrationNumber"];
                                dtHeader.Rows[0]["StateName"] = dtcust.Rows[0]["StateName"];
                                dtHeader.Rows[0]["PHONE"] = dtcust.Rows[0]["PHONE"];
                                dtHeader.Rows[0]["STORECITY"] = dtstore.Rows[0]["STATENAME"];
                            }
                        }
                    }
                }

                conn.Close();

                mAmountWords = words(totalAmount);
                ReportDataSource RDS = new ReportDataSource("Dset", dtRTT);
                rptViewr.LocalReport.DataSources.Add(RDS);
                rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/Insurance.rdl");

                rptViewr.LocalReport.DataSources.Clear();
                var Rds = new ReportDataSource();
                Rds.Name = "IHeader";
                Rds.Value = dtHeader;
                rptViewr.LocalReport.DataSources.Add(Rds);

                var RdsHeader = new ReportDataSource();
                RdsHeader.Name = "IData";
                RdsHeader.Value = dtRTST;
                rptViewr.LocalReport.DataSources.Add(RdsHeader);


                ReportParameter paramCustomerName = new ReportParameter();
                paramCustomerName.Name = "CustomerName";
                paramCustomerName.Values.Add(strCustomerName);
                rptViewr.LocalReport.SetParameters(paramCustomerName);


                rptViewr.LocalReport.Refresh();
                if (pdfflag == "0")
                {
                    rptViewr.LocalReport.Refresh();
                }
                else if (pdfflag == "1")
                {
                    DownloadPdf(strReceiptId + "_" + ReportType);
                }
                else if (pdfflag == "2")
                {
                    string savePath = Server.MapPath("DownloadInvoice\\ShowInsurance.pdf");
                    SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
                }

                conn.Close();
               
            }
            catch (Exception ex)
            {
                Response.Redirect("ErrorPage.aspx?Error=" + ex.Message.ToString());
                /////MessageBox.Show(ex.Message.ToString());
            }

        }
        public void ShowAdvanceReceipt(string strReceiptId, string Storeid, string pdfflag, string ReportType)
        {
            try
            {
                Int32 totalAmount = 0;
                decimal decTaxableValue = 0;
                string strInvoiceCaption = string.Empty;
                string strInvoiceHeader = string.Empty;
                string mAmountWords;
                string transid = string.Empty;
                string storeno = "";
                string strSalesPersonCode = string.Empty;
                string strRateFixed = string.Empty;
                string stqry = "";
                string strExpiryDate = string.Empty;
                string strTransDate = "";
                string strGoldRate = string.Empty;
                string strOfferId = string.Empty;
                decimal amtword = 0;
                //                            string[] Tender  = new string[5];                        
                DataTable dtTotal;
                DataRow drRow;
                decimal scrapexchangeamt = 0;
                decimal salereturnamt = 0;
                string expirydays = "0";
                dtTotal = new DataTable();
                dtTotal.Columns.Add("Description");
                dtTotal.Columns.Add("TaxAmount");
                string ConnectionString = ConfigurationManager.AppSettings["POSDBCON"].ToString();
                SqlConnection conn = new SqlConnection(ConnectionString);

                SqlCommand Cmd;
                SqlDataAdapter da;


                if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " select  rtt.RECEIPTID, rtt.TRANSACTIONID, rtt.CHANNEL, rtt.CUSTACCOUNT, rtt.TRANSDATE, STAFF, rtt.Store, rtt.GrossAmount " +
                //            " from ax.RetailTransactionTable rtt " +
                //            " where rtt.ReceiptID='" + strReceiptId + "' AND ENTRYSTATUS=0 ";

                stqry = "SELECT RECEIPTID,TRANSACTIONID,CHANNEL,CUSTACCOUNT,TRANSDATE,STAFF,Store,GrossAmount,ISDELIVERYATSTORE" + Environment.NewLine;
                stqry += "FROM ACXINVOICETABLE" + Environment.NewLine;
                stqry += "WHERE RECEIPTID='" + strReceiptId + "' ";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtRTT = new DataTable();
                da.Fill(dtRTT);
                if (dtRTT.Rows.Count > 0)
                {
                    storeno = dtRTT.Rows[0]["STORE"].ToString();
                    totalAmount = Convert.ToInt32(Convert.ToDecimal(dtRTT.Rows[0]["GrossAmount"].ToString()) * -1);

                    ///Detail
                    strTransDate = Convert.ToDateTime(dtRTT.Rows[0]["TRANSDATE"].ToString()).ToString("dd-MMM-yyyy");
                    transid = dtRTT.Rows[0]["TRANSACTIONID"].ToString();

                }
                else
                {

                    Response.Redirect("ErrorPage.aspx?Error=" + "No Advance Details Found" + "", false);
                    conn.Close();
                    return;
                }
                if (conn.State != ConnectionState.Open) { conn.Open(); }

                //                stqry = @"Select a.ITEMID, 'Gold/Diamond/Precious/Platinum/Silver/Other Products ' as ItemDescription, a.RECEIPTID,
                //                    a.TRANSDATE, a.AMOUNT, RATEG92, CASE WHEN IsRateFixed=0 THEN 'No' ELSE 'Yes' END IsRateFixed, ADVANCEEXPIRYDATE, b.SALESGROUP as SALESPERSONCODE, a.OFFERID, OFFERLINERECID
                //                    ,COALESCE(c.NAME,'') as OFFERNAME, NoOfDays,B.TRANSACTIONID,
                //                    (SELECT TOP 1 ADVANCEEXPIRYDAYS FROM EXT.ACXJEWELLERYPARAMETER) ADVEXPIRY,a.ADVANCETYPE
                //                    From ext.ACXCUSTOMERADVANCE a
                //                    INNER JOIN ax.RETAILTRANSACTIONSALESTRANS b on a.RECEIPTID = b.RECEIPTID and a.ITEMID = b.ITEMID
                //                    AND A.LINENUM=B.LINENUM
                //                    LEFT JOIN ext.ACXRETAILPERIODICDISCOUNT c on a.OFFERID = c.OFFERID
                //                     where a.RECEIPTID='" + strReceiptId + "'";

                stqry = @"SELECT ITEMID,ITEMDESC AS ItemDescription,RECEIPTID,TRANSDATE,AMOUNT,RATEG92,CASE WHEN IsRateFixed=0 THEN 'No' ELSE 'Yes' END AS IsRateFixed,
		                    ADVANCEEXPIRYDATE,SALESPERSONCODE,OFFERID, OFFERLINERECID,OFFERNAME,NOOFDAYS AS NoOfDays,TRANSACTIONID,0 AS ADVEXPIRY,ADVANCETYPE
		                    FROM DBO.ACXCUSTOMERADVANCE
                            WHERE RECEIPTID='" + strReceiptId + "'";

                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtCustAdv = new DataTable();
                da.Fill(dtCustAdv);
                
                if (dtCustAdv.Rows.Count > 0)
                {
                    strSalesPersonCode = dtCustAdv.Rows[0]["SALESPERSONCODE"].ToString();
                    strSalesPersonCode = strSalesPersonCode == "" ? "-" : strSalesPersonCode;
                    strExpiryDate = Convert.ToDateTime(dtCustAdv.Rows[0]["ADVANCEEXPIRYDATE"].ToString()).ToString("dd-MMM-yyyy");

                    amtword = dtCustAdv.AsEnumerable().Sum(s => s.Field<decimal>("AMOUNT"));
                    scrapexchangeamt = dtCustAdv.AsEnumerable().Where(s => s.Field<int>("ADVANCETYPE") == 8).Sum(s => s.Field<decimal>("AMOUNT"));
                    salereturnamt = dtCustAdv.AsEnumerable().Where(s => s.Field<int>("ADVANCETYPE") == 7).Sum(s => s.Field<decimal>("AMOUNT"));
                    mAmountWords = words(Convert.ToInt32(amtword));
                    //if (dtCustAdv.Rows[0]["ISRATEFIXED"].ToString() == "0")
                    //{
                    //    strRateFixed = "No";
                    //}
                    //else
                    //{
                    //    strRateFixed = "Yes";
                    //}
                    /*Gold Rate Not showing (Rate Fixed Customer) added this on 25-12-2020*/
                    int i = 0;
                   
                    for (i = 0; i < dtCustAdv.Rows.Count; i++)
                    {
                       
                        if (dtCustAdv.Rows[i]["IsRateFixed"].ToString() == "Yes")
                        {
                            DateTime ADVANCEEXPIRYDATE = Convert.ToDateTime(dtCustAdv.Rows[i]["ADVANCEEXPIRYDATE"].ToString());
                             expirydays=(ADVANCEEXPIRYDATE - DateTime.Now).TotalDays.ToString("###0");
                             
                            strGoldRate = Convert.ToDecimal(dtCustAdv.Rows[i]["RATEG92"].ToString()).ToString("###0.00");
                            break;
                        }
                        else
                        {
                            strGoldRate = "0";
                        }
                    }
                    /*end */

                }
                else
                {

                    Response.Redirect("ErrorPage.aspx?Error=" + "Customer Advance Details Not Found" + "", false);
                    conn.Close();
                    return;
                }

                conn.Close();


                //string custqry1 = "Select A.ACCOUNTNUM,b.CUSTNAME NAME,replace(replace(COALEsCE(B.ADDRESS,''),char(10),' '),char(13),' ') as       " + Environment.NewLine;
                //custqry1 += "CustAddress, COALEsCE(B.CITY,'') as CustCity , COALESCE(B.COUNTRYREGIONID, '') as CustCountRegion,            " + Environment.NewLine;
                //custqry1 += "COALESCE(B.DISTRICTNAME, '') as CustDistrictName ,  COALESCE(LT.STATECODE_IN, '') as CustStateCode, COALESCE(B.STREET, '') as CustStreet            " + Environment.NewLine;
                //custqry1 += ", COALESCE(B.ZIPCODE, '') as CustZipCode , COALESCE(B.PANNUMBER, '') as PANNumber  " + Environment.NewLine;
                //custqry1 += ", COALESCE(TRI.REGISTRATIONNUMBER ,'')  as RegistrationNumber   " + Environment.NewLine;
                //custqry1 += "--, COALESCE(g.REGISTRATIONNUMBER,'') as RegistrationNumber   " + Environment.NewLine;
                //custqry1 += ", COALESCE(B.STATENAME, '') as StateName   " + Environment.NewLine;
                //custqry1 += " From ax.CUSTTABLE a INNER JOIN ext.ACXCUSTOMERDETAILSVIEW b on a.ACCOUNTNUM = b.CUSTACCOUNT   " + Environment.NewLine;
                ///*add the four join to getting the gst no on 25-12-2020*/
                //custqry1 += " LEFT JOIN AX.DIRPARTYTABLE DT ON DT.RECID=A.PARTY" + Environment.NewLine;
                //custqry1 += "LEFT JOIN CRT.CUSTOMERPOSTALADDRESSESVIEW CV ON CV.PARTYNUMBER=DT.PARTYNUMBER AND CV.ISPRIMARY=1" + Environment.NewLine;
                //custqry1 += "LEFT JOIN ax.TAXINFORMATION_IN  TI ON  TI.REGISTRATIONLOCATION=CV.LOGISTICSLOCATIONRECID AND TI.ISPRIMARY=1" + Environment.NewLine;
                //custqry1 += "LEFT JOIN ax.TAXREGISTRATIONNUMBERS_IN TRI ON TRI.RECID=TI.GSTIN " + Environment.NewLine;
                ///*end */
                ///*ADDING THIS JOIN FOR GETTING THE STATECODE 25-12-2020*/
                //custqry1 += "LEFT JOIN [ext].[ACXLOGISTICSADDRESSSTATE] LT ON LT.STATEID=B.STATE  AND LT.COUNTRYREGIONID =B.COUNTRYREGIONID " + Environment.NewLine;
                ///*END*/
                //custqry1 += "where ACCOUNTNUM  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
                string custqry1 = "SELECT CUSTACCOUNT AS ACCOUNTNUM,NAME, " + Environment.NewLine;
                custqry1 += "replace(replace(COALEsCE(ADDRESS,''),char(10),' '),char(13),' ') as CustAddress,  " + Environment.NewLine;
                custqry1 += "COALEsCE(CITY,'') as CustCity,COALESCE(COUNTRYREGIONID, '') as CustCountRegion,  " + Environment.NewLine;
                custqry1 += "COALESCE(DISTRICTNAME, '') as CustDistrictName,  " + Environment.NewLine;
                custqry1 += "COALESCE(STATECODE, '') as CustStateCode, COALESCE(STREET, '') as CustStreet,  " + Environment.NewLine;
                custqry1 += "COALESCE(ZIPCODE, '') as CustZipCode,COALESCE(PANNUMBER, '') as PANNumber, " + Environment.NewLine;
                custqry1 += "COALESCE(GSTIN ,'')  as RegistrationNumber,COALESCE(STATENAME, '') as StateName,PHONE  " + Environment.NewLine;
                custqry1 += "FROM DBO.ACXCUSTDETAILS" + Environment.NewLine;
                custqry1 += "WHERE CUSTACCOUNT= '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
                DataTable dtcust;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (Cmd = new SqlCommand(custqry1, conn))
                {
                    using (SqlDataReader reader1 = Cmd.ExecuteReader())
                    {
                        using (DataTable transtable1 = new DataTable())
                        {
                            transtable1.Load(reader1);
                            dtcust = transtable1.Copy();
                        }
                    }
                }

                // for customer Phone
                string strPhone = string.Empty;
                //custqry1 = "Select ACCOUNTNUM,b.NAME, d.Locator "
                //                + " From ax.CUSTTABLE a "
                //                + "INNER JOIN ax.DIRPARTYTABLE b on a.PARTY = b.RECID "
                //                + "INNER JOIN ax.DIRPARTYLOCATION c on a.PARTY = c.PARTY "
                //                + "INNER JOIN ax.LOGISTICSELECTRONICADDRESS d on c.Location = d.Location and d.Type=1 "
                //                + "where ACCOUNTNUM  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
                //DataTable dtPhone;
                //if (conn.State == ConnectionState.Closed)
                //    conn.Open();

                //using (Cmd = new SqlCommand(custqry1, conn))
                //{
                //    using (SqlDataReader reader1 = Cmd.ExecuteReader())
                //    {
                //        using (DataTable transtable1 = new DataTable())
                //        {
                //            transtable1.Load(reader1);
                //            dtPhone = transtable1.Copy();
                //        }
                //    }
                //}
                //if (dtPhone.Rows.Count > 0)
                //{
                //    strPhone = dtPhone.Rows[0]["Locator"].ToString();
                //}

                strPhone = dtcust.Rows.Count > 0 ? dtcust.Rows[0]["PHONE"].ToString() : "";



                stqry = "select 'Credit Cards/Debit Cards/UPI/Credit Customer' AS Description, COALESCE(SUM(TENDERAMOUNT),0) AS TENDERAMOUNT  " + Environment.NewLine;
                stqry += " from dbo.acxINVOICEPAYMENT  " + Environment.NewLine;
                stqry += "  WHERE  TRANSACTIONID='" + transid + "'  AND ((Convert(int,TENDERTYPE)>=20) AND (Convert(int,TENDERTYPE)<=40 )) " + Environment.NewLine;
                stqry += " HAVING COALESCE(SUM(TENDERAMOUNT),0)>0  " + Environment.NewLine;
                stqry += " UNION ALL  " + Environment.NewLine;
                stqry += "  select TENDERNAME AS Description,  TENDERAMOUNT  " + Environment.NewLine;
                stqry += " from dbo.acxINVOICEPAYMENT  " + Environment.NewLine;
                stqry += " WHERE    TRANSACTIONID='" + transid + "'  AND  ((Convert(int,TENDERTYPE)<20) OR (Convert(int,TENDERTYPE)>40)) ";

                Cmd = new SqlCommand(stqry);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtTender = new DataTable();
                da.Fill(dtTender);

                foreach (DataRow row in dtTender.Rows)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = row["Description"].ToString();
                    drRow["TaxAmount"] = Convert.ToDecimal(row["TENDERAMOUNT"].ToString()).ToString("####0.00");
                    dtTotal.Rows.Add(drRow);

                }



                if (scrapexchangeamt != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Scrap Exchange";
                    drRow["TaxAmount"] = scrapexchangeamt.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                if (salereturnamt != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Sale Return";
                    drRow["TaxAmount"] = salereturnamt.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }



                conn.Close();
                //stqry = " select GSTIN,COMPANYNAME,ADDRESS1,ADDRESS3,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3 "
                //                + ",PAN, CIN, STATECODE,b.NAME "
                //                + "from ext.ACXSTOREINFO a "
                //                + "LEFT JOIN ax.LOGISTICSADDRESSSTATE b on b.STATEID = a.STATECODE "

                //             + "where a.STORENUMBER='" + storeno + "' ";

                stqry = "select GSTIN,COMPANYNAME,ADDRESS1,ADDRESS2,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3 "
                                + ", PAN, CIN, STATECODE, STATENAME AS NAME "
                                + "from ACXSTOREDETAILS a "
                                + " where a.STORENUMBER ='" + storeno + "' ";
                Cmd = new SqlCommand(stqry);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtHeader = new DataTable();
                da.Fill(dtHeader);

                if (dtCustAdv.Rows[0]["OFFERLINERECID"].ToString() == "0" && dtCustAdv.Rows[0]["OFFERID"].ToString() != "")
                {
                    ReportDataSource RDS = new ReportDataSource("Dset", dtRTT);
                    rptViewr.LocalReport.DataSources.Add(RDS);
                    rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/AdvanceReceiptOffer.rdl");
                }
                else
                {
                    int NoOfDays = dtCustAdv.AsEnumerable().Max(s => s.Field<int>("NoOfDays"));
                    strRateFixed = "Ref - " + strReceiptId + Environment.NewLine;
                    strRateFixed += "This Sales Order is valid upto " + NoOfDays.ToString() + " days from the date of Purchase.  " + Environment.NewLine;
                    strRateFixed += "Note: Rate Fixed Benefit will be Applicable for " + expirydays+" days ";
                    ReportDataSource RDS = new ReportDataSource("Dset", dtRTT);
                    rptViewr.LocalReport.DataSources.Add(RDS);
                    rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/AdvanceReceipt.rdl");

                }


                rptViewr.LocalReport.DataSources.Clear();
                var Rds = new ReportDataSource();
                Rds.Name = "Header";
                Rds.Value = dtHeader;
                rptViewr.LocalReport.DataSources.Add(Rds);

                var RdsHeader = new ReportDataSource();
                RdsHeader.Name = "IData";
                RdsHeader.Value = dtCustAdv;
                rptViewr.LocalReport.DataSources.Add(RdsHeader);

                var RdsCustDetail = new ReportDataSource();
                RdsCustDetail.Name = "CustomerDetail";
                RdsCustDetail.Value = dtcust;
                rptViewr.LocalReport.DataSources.Add(RdsCustDetail);



                var RdsTotal = new ReportDataSource();
                RdsTotal.Name = "ITotal";
                RdsTotal.Value = dtTotal;
                rptViewr.LocalReport.DataSources.Add(RdsTotal);


                ReportParameter AmountInWords = new ReportParameter();
                AmountInWords.Name = "AmountInWords";
                AmountInWords.Values.Add(mAmountWords);
                rptViewr.LocalReport.SetParameters(AmountInWords);

                ReportParameter RateFixed = new ReportParameter();
                RateFixed.Name = "RateFixed";
                RateFixed.Values.Add(strRateFixed);
                rptViewr.LocalReport.SetParameters(RateFixed);


                ReportParameter GoldRate = new ReportParameter();
                GoldRate.Name = "GoldRate";
                GoldRate.Values.Add(strGoldRate);
                rptViewr.LocalReport.SetParameters(GoldRate);

                ReportParameter DueDate = new ReportParameter();
                DueDate.Name = "DueDate";
                DueDate.Values.Add(strExpiryDate);
                rptViewr.LocalReport.SetParameters(DueDate);

                ReportParameter SalesPersonCode = new ReportParameter();
                SalesPersonCode.Name = "SalesPersonCode";
                SalesPersonCode.Values.Add(strSalesPersonCode);
                rptViewr.LocalReport.SetParameters(SalesPersonCode);


                ReportParameter InvoiceDate = new ReportParameter();
                InvoiceDate.Name = "InvoiceDate";
                InvoiceDate.Values.Add(strTransDate);
                rptViewr.LocalReport.SetParameters(InvoiceDate);

                ReportParameter parameter = new ReportParameter();
                parameter.Name = "CustPhone";
                parameter.Values.Add(strPhone);
                rptViewr.LocalReport.SetParameters(parameter);

                rptViewr.LocalReport.DisplayName = "Advance Receipt";
                if (pdfflag == "0")
                {
                    rptViewr.LocalReport.Refresh();
                }
                else if (pdfflag == "1")
                {
                    DownloadPdf(strReceiptId + "_" + ReportType);
                }
                else if (pdfflag == "2")
                {
                    string savePath = Server.MapPath("DownloadInvoice\\AdvanceReceipt.pdf");
                    SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
                }

                conn.Close();

            }
            catch (Exception ex)
            {
                Response.Redirect("ErrorPage.aspx?Error=" + ex.Message.ToString());
                return;
            }

        }

        public void ShowSchemeReceipt(string strReceiptId, string storeid, string pdfflag, string ReportType)
        {
            try
            {
                Int32 totalAmount = 0;
                decimal decTaxableValue = 0;
                string strInvoiceCaption = string.Empty;
                string strInvoiceHeader = string.Empty;
                string mAmountWords;
                string transid = string.Empty;
                string storeno = "";
                string strSalesPersonCode = string.Empty;
                string strRateFixed = string.Empty;
                string stqry = "";
                string strSchemeOpeningDate = string.Empty;
                string strNextDueDate = string.Empty;
                string strTransDate = "";
                DateTime dtDate = DateTime.Now;
                string strSchemeEntryNo = string.Empty;

                //                            string[] Tender  = new string[5];                        
                DataTable dtTotal;
                DataRow drRow;

                dtTotal = new DataTable();
                dtTotal.Columns.Add("Description");
                dtTotal.Columns.Add("TaxAmount");
                string ConnectionString = ConfigurationManager.AppSettings["POSDBCON"].ToString();
                SqlConnection conn = new SqlConnection(ConnectionString);

                SqlCommand Cmd;
                SqlDataAdapter da;


                if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " select  rtt.RECEIPTID, rtt.TRANSACTIONID, rtt.CHANNEL, rtt.CUSTACCOUNT, rtt.TRANSDATE, STAFF, rtt.Store, rtt.GrossAmount " +
                //            " from ax.RetailTransactionTable rtt " +
                //            " where rtt.ReceiptID='" + strReceiptId + "' AND ENTRYSTATUS=0 ";

                stqry = "SELECT RECEIPTID,TRANSACTIONID,CHANNEL,CUSTACCOUNT,TRANSDATE,STAFF,Store,GrossAmount" + Environment.NewLine;
                stqry += "FROM ACXINVOICETABLE" + Environment.NewLine;
                stqry += "WHERE RECEIPTID='" + strReceiptId + "' ";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtRTT = new DataTable();
                da.Fill(dtRTT);
                if (dtRTT.Rows.Count > 0)
                {
                    storeno = dtRTT.Rows[0]["STORE"].ToString();
                    totalAmount = Convert.ToInt32(Convert.ToDecimal(dtRTT.Rows[0]["GrossAmount"].ToString()) * -1);
                }
                else
                {

                    Response.Redirect("ErrorPage.aspx?Error=" + "No Scheme Details Found" + "", false);
                    conn.Close();
                    return;
                }
                ///Detail
                strTransDate = Convert.ToDateTime(dtRTT.Rows[0]["TRANSDATE"].ToString()).ToString("dd-MMM-yyyy");
                transid = dtRTT.Rows[0]["TRANSACTIONID"].ToString();
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = "Select 'Easygold Advance Purchase Scheme' + b.SCHEMECODE as ItemDescription,a.INSTALLMENTNO "
                //      + ",a.INSTALLMENTMONTH, a.PAYMENTAMOUNT, a.PAYMENTDATE, b.SCHEMEOPENINGDATE, a.SALESPERSONCODE as SALESPERSONCODE, b.SCHEMEENTRYNO, a.RECEIPTID "
                //      + "From ext.ACXCUSTOMERSCHEMEPAYMENT a "
                //      + "INNER JOIN ext.ACXCUSTOMERSCHEMEENTRY b on a.SCHEMEENTRYNO=b.SCHEMEENTRYNO "
                //      + "INNER JOIN ax.RETAILTRANSACTIONSALESTRANS c on a.RECEIPTID = c.RECEIPTID AND a.LINENUM = c.LINENUM "
                //      + "where a.RECEIPTID='" + strReceiptId + "' ";

                stqry = "SELECT ITEMID,ITEMDESC as ItemDescription,INSTALLMENTNO, "
                    + "INSTALLMENTMONTH,PAYMENTAMOUNT,PAYMENTDATE,SCHEMEOPENINGDATE, "
                    + "SALESPERSONCODE,SCHEMEENTRYNO,RECEIPTID "
                    + "FROM dbo.ACXCUSTOMEREASYGOLDPAYMENT "
                    + "where RECEIPTID='" + strReceiptId + "' ";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtSchemePayment = new DataTable();
                da.Fill(dtSchemePayment);
                if (dtSchemePayment.Rows.Count > 0)
                {
                    strSchemeEntryNo = dtSchemePayment.Rows[0]["SCHEMEENTRYNO"].ToString();
                    strSchemeOpeningDate = Convert.ToDateTime(dtSchemePayment.Rows[0]["SCHEMEOPENINGDATE"].ToString()).ToString("dd-MMM-yyyy");
                    dtDate = Convert.ToDateTime(dtSchemePayment.Rows[0]["SCHEMEOPENINGDATE"].ToString());
                    strSalesPersonCode = dtSchemePayment.Rows[0]["SALESPERSONCODE"].ToString();
                }
                else
                {
                    //MessageBox.Show("No Payment Details Found");
                    Response.Redirect("ErrorPage.aspx?Error=" + "No Payment Details Found" + "", false);
                    conn.Close();
                    return;
                }

                if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " Select COUNT(PAYMENTMONTH) as SCHEMECOUNT "
                //      + "From ext.ACXCUSTOMERSCHEMEPAYMENT "
                //      + "where SCHEMEENTRYNO='" + strSchemeEntryNo + "' "
                //      + "and CANCELLED =0 ";
                stqry = " Select PAYMENTCOUNT as SCHEMECOUNT "
                    + "From ACXCUSTOMEREASYGOLDPAYMENT "
                    + "where SCHEMEENTRYNO='" + strSchemeEntryNo + "'  AND  RECEIPTID='" + strReceiptId + "'";



                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtSchemeCount = new DataTable();
                da.Fill(dtSchemeCount);
                if (dtSchemeCount.Rows.Count > 0)
                {
                    strNextDueDate = dtDate.AddMonths(Convert.ToInt16(dtSchemeCount.Rows[0]["SCHEMECOUNT"].ToString())).ToString("dd-MMM-yyyy");
                }
                /*Issue 25-12-2020 :--No need Next payment due date on Instalment Receipt  voucher -Scheme completed  customer  then added this */
                //stqry = " select NOOFINSTALLMENT from ext.ACXCUSTOMERSCHEMEENTRY  "
                //      + "where SCHEMEENTRYNO='" + strSchemeEntryNo + "' ";

                //Cmd = new SqlCommand(stqry);
                //Cmd.Connection = conn;
                //da = new SqlDataAdapter(Cmd);
                //da.SelectCommand.CommandType = CommandType.Text;
                //da.SelectCommand.CommandTimeout = 120;
                //DataTable dt = new DataTable();
                //da.Fill(dt);
                if (dtSchemeCount.Rows.Count > 0)
                {
                    if (Convert.ToInt16(dtSchemeCount.Rows[0]["SCHEMECOUNT"]) == 10)
                    {
                        strNextDueDate = "";
                    }
                }

                /*end here*/

                conn.Close();

                /*Customer Address two time showing resolved :-remove the field cust city,custstreet,custzipcode form the rdl*/
                // for customer detail
                //string custqry1 = "Select A.ACCOUNTNUM,b.CUSTNAME NAME,replace(replace(COALEsCE(B.ADDRESS,''),char(10),' '),char(13),' ') as       " + Environment.NewLine;
                //custqry1 += "CustAddress, COALEsCE(B.CITY,'') as CustCity , COALESCE(B.COUNTRYREGIONID, '') as CustCountRegion,            " + Environment.NewLine;
                //custqry1 += "COALESCE(B.DISTRICTNAME, '') as CustDistrictName , COALESCE(LT.STATECODE_IN,'') as CustStateCode, COALESCE(B.STREET, '') as CustStreet            " + Environment.NewLine;
                //custqry1 += ", COALESCE(B.ZIPCODE, '') as CustZipCode , COALESCE(B.PANNUMBER, '') as PANNumber  " + Environment.NewLine;
                //custqry1 += ", '' as RegistrationNumber   " + Environment.NewLine;
                //custqry1 += "--, COALESCE(g.REGISTRATIONNUMBER,'') as RegistrationNumber   " + Environment.NewLine;
                //custqry1 += ", COALESCE(B.STATENAME, '') as StateName   " + Environment.NewLine;
                //custqry1 += " From ax.CUSTTABLE a INNER JOIN ext.ACXCUSTOMERDETAILSVIEW b on a.ACCOUNTNUM = b.CUSTACCOUNT   " + Environment.NewLine;
                //custqry1 += "--LEFT JOIN ax.TAXINFORMATION_IN f on f.REGISTRATIONLOCATION = c.LOCATION   " + Environment.NewLine;
                //custqry1 += "--LEFT JOIN ax.TAXREGISTRATIONNUMBERS_IN g on g.RECID=f.GSTIN   " + Environment.NewLine;
                ///*ADDING THIS JOIN FOR GETTING THE STATECODE 25-12-2020*/
                //custqry1 += "LEFT JOIN [ext].[ACXLOGISTICSADDRESSSTATE] LT ON LT.STATEID=B.STATE  AND LT.COUNTRYREGIONID =B.COUNTRYREGIONID " + Environment.NewLine;
                ///*END*/
                //custqry1 += "where ACCOUNTNUM  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";

                string custqry1 = "SELECT CUSTACCOUNT AS ACCOUNTNUM,NAME, " + Environment.NewLine;
                custqry1 += "replace(replace(COALEsCE(ADDRESS,''),char(10),' '),char(13),' ') as CustAddress,  " + Environment.NewLine;
                custqry1 += "COALEsCE(CITY,'') as CustCity,COALESCE(COUNTRYREGIONID, '') as CustCountRegion,  " + Environment.NewLine;
                custqry1 += "COALESCE(DISTRICTNAME, '') as CustDistrictName,  " + Environment.NewLine;
                custqry1 += "COALESCE(STATECODE, '') as CustStateCode, COALESCE(STREET, '') as CustStreet,  " + Environment.NewLine;
                custqry1 += "COALESCE(ZIPCODE, '') as CustZipCode,COALESCE(PANNUMBER, '') as PANNumber, " + Environment.NewLine;
                custqry1 += "COALESCE(GSTIN ,'')  as RegistrationNumber,COALESCE(STATENAME, '') as StateName,PHONE  " + Environment.NewLine;
                custqry1 += "FROM DBO.ACXCUSTDETAILS" + Environment.NewLine;
                custqry1 += "WHERE CUSTACCOUNT= '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";

                DataTable dtcust;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (Cmd = new SqlCommand(custqry1, conn))
                {
                    using (SqlDataReader reader1 = Cmd.ExecuteReader())
                    {
                        using (DataTable transtable1 = new DataTable())
                        {
                            transtable1.Load(reader1);
                            dtcust = transtable1.Copy();
                        }
                    }
                }

                // for customer Phone
                string strPhone = string.Empty;
                //custqry1 = "Select ACCOUNTNUM,b.NAME, d.Locator "
                //                + " From ax.CUSTTABLE a "
                //                + "INNER JOIN ax.DIRPARTYTABLE b on a.PARTY = b.RECID "
                //                + "INNER JOIN ax.DIRPARTYLOCATION c on a.PARTY = c.PARTY "
                //                + "INNER JOIN ax.LOGISTICSELECTRONICADDRESS d on c.Location = d.Location and d.Type=1 "
                //                + "where ACCOUNTNUM  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
                //DataTable dtPhone;
                //if (conn.State == ConnectionState.Closed)
                //    conn.Open();

                //using (Cmd = new SqlCommand(custqry1, conn))
                //{
                //    using (SqlDataReader reader1 = Cmd.ExecuteReader())
                //    {
                //        using (DataTable transtable1 = new DataTable())
                //        {
                //            transtable1.Load(reader1);
                //            dtPhone = transtable1.Copy();
                //        }
                //    }
                //}
                if (dtcust.Rows.Count > 0)
                {
                    strPhone = dtcust.Rows[0]["PHONE"].ToString();
                }

                ////Credit Card Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }

                stqry = "select 'Credit Cards/Debit Cards/UPI/Credit Customer' AS Description, COALESCE(SUM(TENDERAMOUNT),0) AS TENDERAMOUNT  " + Environment.NewLine;
                stqry += " from dbo.acxINVOICEPAYMENT  " + Environment.NewLine;
                stqry += "  WHERE  TRANSACTIONID='" + transid + "'  AND ((Convert(int,TENDERTYPE)>=20) AND (Convert(int,TENDERTYPE)<=40 )) " + Environment.NewLine;
                stqry += " HAVING COALESCE(SUM(TENDERAMOUNT),0)>0  " + Environment.NewLine;
                stqry += " UNION ALL  " + Environment.NewLine;
                stqry += "  select TENDERNAME AS Description,  TENDERAMOUNT  " + Environment.NewLine;
                stqry += " from dbo.acxINVOICEPAYMENT  " + Environment.NewLine;
                stqry += " WHERE    TRANSACTIONID='" + transid + "'  AND  ((Convert(int,TENDERTYPE)<20) OR (Convert(int,TENDERTYPE)>40)) ";

                Cmd = new SqlCommand(stqry);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtTender = new DataTable();
                da.Fill(dtTender);
                string str7 = "";
                foreach (DataRow row in dtTender.Rows)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = row["Description"].ToString();
                    drRow["TaxAmount"] = Convert.ToDecimal(row["TENDERAMOUNT"].ToString()).ToString("####0.00");
                    dtTotal.Rows.Add(drRow);
                    str7 = str7 + row["Description"].ToString() + " , ";
                }



                /*
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) >=20 and Convert(int,TENDERTYPE) <= 40 AND VOIDSTATUS=0 ";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtTend = new DataTable();
                int intTenderCount = 0;
                da.Fill(dtTend);
                decTaxableValue = 0;
                for (intTenderCount = 0; intTenderCount < dtTend.Rows.Count; intTenderCount++)
                {
                    if (Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                    {
                        decTaxableValue += Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString());
                    }
                }
                if (decTaxableValue != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Credit Cards/Debit Cards/UPI/Credit Customer ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                ////Cheques Payment and DD
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =42 AND TRANSACTIONSTATUS =0 AND VOIDSTATUS=0";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                dtTend.Clear();
                dtTend = new DataTable();
                intTenderCount = 0;
                da.Fill(dtTend);
                decTaxableValue = 0;
                for (intTenderCount = 0; intTenderCount < dtTend.Rows.Count; intTenderCount++)
                {
                    if (Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                    {
                        decTaxableValue += Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString());
                    }
                }
                if (decTaxableValue != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Cheque/DD ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                ////Cheques RTGS and NEFT
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =43 AND TRANSACTIONSTATUS =0 AND VOIDSTATUS=0";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                dtTend.Clear();
                dtTend = new DataTable();
                intTenderCount = 0;
                da.Fill(dtTend);
                decTaxableValue = 0;
                for (intTenderCount = 0; intTenderCount < dtTend.Rows.Count; intTenderCount++)
                {
                    if (Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                    {
                        decTaxableValue += Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString());
                    }
                }
                if (decTaxableValue != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "RTGS/NEFT ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }

                ////Credit Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =50 AND VOIDSTATUS=0";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                dtTend.Clear();
                dtTend = new DataTable();
                intTenderCount = 0;
                da.Fill(dtTend);
                decTaxableValue = 0;
                for (intTenderCount = 0; intTenderCount < dtTend.Rows.Count; intTenderCount++)
                {
                    if (Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                    {
                        decTaxableValue += Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString());
                    }
                }
                if (decTaxableValue != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Credit Allowed ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }

                ////Cash Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =1 AND VOIDSTATUS=0";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                dtTend.Clear();
                dtTend = new DataTable();
                intTenderCount = 0;
                da.Fill(dtTend);
                decTaxableValue = 0;
                for (intTenderCount = 0; intTenderCount < dtTend.Rows.Count; intTenderCount++)
                {
                    if (Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                    {
                        decTaxableValue += Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString());
                    }
                }
                if (decTaxableValue != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Cash Received ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                 * 
                 * */






                conn.Close();
                //stqry = " select GSTIN,COMPANYNAME,ADDRESS1,ADDRESS3,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3 "
                //                + ",PAN, CIN, a.STATECODE, b.NAME "
                //                + "from ext.ACXSTOREINFO a "
                //                + "LEFT JOIN ax.LOGISTICSADDRESSSTATE b on b.STATEID = a.STATECODE "
                //                + " where a.STORENUMBER='" + storeno + "' ";

                stqry = "select GSTIN,COMPANYNAME,ADDRESS1,ADDRESS2,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3 "
                               + ", PAN, CIN, STATECODE, STATENAME AS NAME "
                               + "from ACXSTOREDETAILS a "
                               + " where a.STORENUMBER ='" + storeno + "' ";
                Cmd = new SqlCommand(stqry);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtHeader = new DataTable();
                da.Fill(dtHeader);

                mAmountWords = words(totalAmount);

                // ReportDataSource RDS = new ReportDataSource("Dset", dtRTT);
                // rptViewr.LocalReport.DataSources.Add(RDS);
                rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SchemeReceipt.rdl");

                rptViewr.LocalReport.DataSources.Clear();
                var Rds = new ReportDataSource();
                Rds.Name = "Header";
                Rds.Value = dtHeader;
                rptViewr.LocalReport.DataSources.Add(Rds);

                var RdsHeader = new ReportDataSource();
                RdsHeader.Name = "IData";
                RdsHeader.Value = dtSchemePayment;
                rptViewr.LocalReport.DataSources.Add(RdsHeader);

                var RdsCustDetail = new ReportDataSource();
                RdsCustDetail.Name = "CustomerDetail";
                RdsCustDetail.Value = dtcust;
                rptViewr.LocalReport.DataSources.Add(RdsCustDetail);

                var RdsTotal = new ReportDataSource();
                RdsTotal.Name = "ITotal";
                RdsTotal.Value = dtTotal;
                rptViewr.LocalReport.DataSources.Add(RdsTotal);

                ReportParameter parameter = new ReportParameter();
                parameter.Name = "CustPhone";
                parameter.Values.Add(strPhone);
                rptViewr.LocalReport.SetParameters(parameter);

                ReportParameter AmountInWords = new ReportParameter();
                AmountInWords.Name = "AmountInWords";
                AmountInWords.Values.Add(mAmountWords);
                rptViewr.LocalReport.SetParameters(AmountInWords);


                ReportParameter DueDate = new ReportParameter();
                DueDate.Name = "DueDate";
                DueDate.Values.Add(strNextDueDate);
                rptViewr.LocalReport.SetParameters(DueDate);

                ReportParameter SalesPersonCode = new ReportParameter();
                SalesPersonCode.Name = "SalesPersonCode";
                SalesPersonCode.Values.Add(strSalesPersonCode);
                rptViewr.LocalReport.SetParameters(SalesPersonCode);

                ReportParameter SchemeEntryNo = new ReportParameter();
                SchemeEntryNo.Name = "SchemeEntryNo";
                SchemeEntryNo.Values.Add(strSchemeEntryNo);
                rptViewr.LocalReport.SetParameters(SchemeEntryNo);

                ReportParameter strPaymentMode = new ReportParameter();
                strPaymentMode.Name = "PaymentMode";
                strPaymentMode.Values.Add(str7);
                rptViewr.LocalReport.SetParameters(strPaymentMode);

                rptViewr.LocalReport.Refresh();
                if (pdfflag == "0")
                {
                    rptViewr.LocalReport.Refresh();
                }
                else if (pdfflag == "1")
                {
                    DownloadPdf(strReceiptId + "_" + ReportType);
                }
                else if (pdfflag == "2")
                {
                    string savePath = Server.MapPath("DownloadInvoice\\SchemeReceipt.pdf");
                    SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
                }

                conn.Close();

            }
            catch (Exception ex)
            {
                Response.Redirect("ErrorPage.aspx?Error=" + ex.Message.ToString());
                return;
            }

        }


        public void ShowForm60(string strReceiptId, string pdfflag, string ReportType)
        {

            string strTransDate = string.Empty;
            string strSalesPersonCode = string.Empty;
            string stqry = "";
            Int32 totalAmount = 0;
            string mAmountWords = string.Empty;
            string transId = string.Empty;


            string ConnectionString = ConfigurationManager.AppSettings["POSDBCON"].ToString();
            SqlConnection conn = new SqlConnection(ConnectionString);

            SqlCommand Cmd;
            SqlDataAdapter da;
            string storeid = "";

            if (conn.State != ConnectionState.Open) { conn.Open(); }
            ////stqry = " select  rtt.RECEIPTID, rtt.TRANSACTIONID, rtt.CHANNEL, rtt.CUSTACCOUNT, rtt.TRANSDATE, STAFF, rtt.Store, rtt.GrossAmount " +
            ////            " from ax.RetailTransactionTable rtt " +
            ////            " where rtt.ReceiptID='" + strReceiptId + "' AND ENTRYSTATUS=0 ";

            stqry = "SELECT RECEIPTID,TRANSACTIONID,CHANNEL,CUSTACCOUNT,TRANSDATE,STAFF,Store,GrossAmount" + Environment.NewLine;
            stqry += "FROM ACXINVOICETABLE" + Environment.NewLine;
            stqry += "WHERE RECEIPTID='" + strReceiptId + "' ";

            Cmd = new SqlCommand(stqry);
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            DataTable dtRTT = new DataTable();
            da.Fill(dtRTT);

            strTransDate = Convert.ToDateTime(dtRTT.Rows[0]["TRANSDATE"].ToString()).ToString("dd-MMM-yyyy");
            transId = dtRTT.Rows[0]["TRANSACTIONID"].ToString();
            storeid = dtRTT.Rows[0]["Store"].ToString();
            totalAmount = Convert.ToInt32(Convert.ToDecimal(dtRTT.Rows[0]["GrossAmount"].ToString()) * -1);
            // for customer detail
            string strCustomerName = string.Empty;
            //string custqry1 = "Select ACCOUNTNUM,b.FIRSTNAME,b.MIDDLENAME,b.LASTNAME, COALEsCE(e.CITY,'') as CustCity "
            //                + ", COALESCE(e.DISTRICTNAME, '') as CustDistrictName, COALESCE(e.ZIPCODE, '') as CustZipCode "
            //                + ", COALESCE(f.Name, '') as StateName, e.STREET "
            //                + ", FATHERNAME, case g.AADHARNO when '' then g.VOTERID else g.AADHARNO end as CustDocNo, g.AADHARNO "
            //                + ", g.AGRICULTURALINCOME, case g.AADHARNO when '' then 'VOTERID' else 'AADHAR' end as CustDocCode, g.DATEOFBIRTH "
            //                + "From ax.CUSTTABLE a "
            //                + "LEFT JOIN ax.DIRPERSONNAME b on a.PARTY = b.PERSON "
            //                + "INNER JOIN ax.DIRPARTYTABLE c on a.PARTY = c.RECID "
            //                + "INNER JOIN ax.DIRPARTYLOCATION d on a.PARTY = d.PARTY "
            //                + "LEFT JOIN ax.LOGISTICSPOSTALADDRESS e on d.LOCATION = e.LOCATION "
            //                + "LEFT JOIN ax.LOGISTICSADDRESSSTATE f on f.STATEID=e.STATE and f.COUNTRYREGIONID=e.COUNTRYREGIONID "
            //                + "INNER JOIN ext.ACXCUSTTABLE g on a.ACCOUNTNUM = g.CUSTACCOUNT "
            //                + "where ACCOUNTNUM  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' and d.ISPRIMARY=1 "
            //                + "and ((b.VALIDFROM <= '" + strTransDate + " 23:59:59' and b.VALIDTO >='" + strTransDate + " 00:00:01') or b.RECID is Null) "
            //                + "and ((e.VALIDFROM <= '" + strTransDate + " 23:59:59' and e.VALIDTO >='" + strTransDate + " 00:00:01') or e.RECID is Null) ";

            //string custqry1 = "SELECT CUSTACCOUNT,NAME AS FIRSTNAME,'' MIDDLENAME,'' LASTNAME, COALEsCE(CITY,'') as CustCity, "
            //                    + " COALESCE(DISTRICTNAME, '') as CustDistrictName,COALESCE(ZIPCODE, '') as CustZipCode, "
            //                     + "COALESCE(STATENAME, '') as StateName,STREET,FATHERNAME, "
            //                     + "case AADHARNO when '' then VOTERID else AADHARNO end as CustDocNo,AADHARNO, "
            //                     + "AGRICULTUREINCOME,case AADHARNO when '' then 'VOTERID' else 'AADHAR' end as CustDocCode,DATEOFBIRTH,PHONE "
            //                     + "FROM DBO.ACXCUSTDETAILS "
            //                     + "WHERE CUSTACCOUNT= '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
            string custqry1 = @"SELECT CUSTACCOUNT,NAME AS FIRSTNAME,'' MIDDLENAME,'' LASTNAME,
COALEsCE(CITY, '') as CustCity, 
COALESCE(DISTRICTNAME, '') as CustDistrictName,
COALESCE(ZIPCODE, '') as CustZipCode, 
COALESCE(STATENAME, '') as StateName,STREET,FATHERNAME,
case AADHARNO when '' then(
case PASSPORT  when '' then(
case DRIVINGLICENSE when '' then
VOTERID else DRIVINGLICENSE end)else PASSPORT end) else AADHARNO end as CustDocNo, AGRICULTUREINCOME,
case AADHARNO when '' then(
case PASSPORT  when '' then(
case DRIVINGLICENSE when '' then(
case VOTERID when '' then 
'' else 'VOTERID' end ) else 'DRIVINGLICENSE'  end)else 'PASSPORT' end) else 'AADHARNO' end
as CustDocCode,DATEOFBIRTH,PHONE FROM DBO.ACXCUSTDETAILS WHERE CUSTACCOUNT = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
            DataTable dtcust;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (Cmd = new SqlCommand(custqry1, conn))
            {
                using (SqlDataReader reader1 = Cmd.ExecuteReader())
                {
                    using (DataTable transtable1 = new DataTable())
                    {
                        transtable1.Load(reader1);
                        dtcust = transtable1.Copy();
                        if (dtcust.Rows.Count > 0)
                        {
                            strCustomerName = dtcust.Rows[0]["FIRSTNAME"].ToString() + " " + dtcust.Rows[0]["LASTNAME"].ToString();
                        }
                    }
                }
            }

            // for customer Phone
            string strPhone = string.Empty;
            //custqry1 = "Select ACCOUNTNUM,b.NAME, d.Locator "
            //                + " From ax.CUSTTABLE a "
            //                + "INNER JOIN ax.DIRPARTYTABLE b on a.PARTY = b.RECID "
            //                + "INNER JOIN ax.DIRPARTYLOCATION c on a.PARTY = c.PARTY "
            //                + "INNER JOIN ax.LOGISTICSELECTRONICADDRESS d on c.Location = d.Location and d.Type=1 "
            //                + "where ACCOUNTNUM  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
            //DataTable dtPhone;
            //if (conn.State == ConnectionState.Closed)
            //    conn.Open();

            //using (Cmd = new SqlCommand(custqry1, conn))
            //{
            //    using (SqlDataReader reader1 = Cmd.ExecuteReader())
            //    {
            //        using (DataTable transtable1 = new DataTable())
            //        {
            //            transtable1.Load(reader1);
            //            dtPhone = transtable1.Copy();
            //        }
            //    }
            //}
            if (dtcust.Rows.Count > 0)
            {
                strPhone = dtcust.Rows[0]["PHONE"].ToString();
            }




            ////Credit Card Payment
            string strPaymentMethods = "";

            stqry = "select 'Credit Cards/Debit Cards/UPI/Credit Customer' AS Description, COALESCE(SUM(TENDERAMOUNT),0) AS TENDERAMOUNT  " + Environment.NewLine;
            stqry += " from dbo.acxINVOICEPAYMENT  " + Environment.NewLine;
            stqry += "  WHERE  TRANSACTIONID='" + transId + "'  AND ((Convert(int,TENDERTYPE)>=20) AND (Convert(int,TENDERTYPE)<=40 )) " + Environment.NewLine;
            stqry += " HAVING COALESCE(SUM(TENDERAMOUNT),0)>0  " + Environment.NewLine;
            stqry += " UNION ALL  " + Environment.NewLine;
            stqry += "  select TENDERNAME AS Description,  TENDERAMOUNT  " + Environment.NewLine;
            stqry += " from dbo.acxINVOICEPAYMENT  " + Environment.NewLine;
            stqry += " WHERE    TRANSACTIONID='" + transId + "'  AND  ((Convert(int,TENDERTYPE)<20) OR (Convert(int,TENDERTYPE)>40)) ";

            Cmd = new SqlCommand(stqry);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            DataTable dtTender = new DataTable();
            da.Fill(dtTender);

            foreach (DataRow row in dtTender.Rows)
            {
                if (string.IsNullOrEmpty(strPaymentMethods))
                {
                    strPaymentMethods = row["Description"].ToString();
                }
                else
                {
                    strPaymentMethods = strPaymentMethods + ", " + row["Description"].ToString();
                }

            }
            /*
            if (conn.State != ConnectionState.Open) { conn.Open(); }
            stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                  + "where TRANSACTIONID='" + transId + "' and Convert(int,TENDERTYPE) >=20 and Convert(int,TENDERTYPE) <= 40 ";
            Cmd = new SqlCommand(stqry);
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            DataTable dtTend = new DataTable();
            int intTenderCount = 0;
            da.Fill(dtTend);
            if (dtTend.Rows.Count > 0)
            {
                decimal credit = Convert.ToDecimal(dtTend.Rows[0]["Amount"]);
                if (credit > 0)
                {
                    strPaymentMethods = "Credit Card";
                }

            }
            ////Cheques Payment
            if (conn.State != ConnectionState.Open) { conn.Open(); }
            stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                  + "where TRANSACTIONID='" + transId + "' and Convert(int,TENDERTYPE) =42 ";
            Cmd = new SqlCommand(stqry);
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            dtTend.Clear();
            dtTend = new DataTable();
            intTenderCount = 0;
            da.Fill(dtTend);
            if (dtTend.Rows.Count > 0)
            {
                decimal credit = Convert.ToDecimal(dtTend.Rows[0]["Amount"]);
                if (credit > 0)
                {
                    if (strPaymentMethods != "")
                    {
                        strPaymentMethods = strPaymentMethods + ", CHEQUE";
                    }
                    else
                    {
                        strPaymentMethods = "CHEQUE";
                    }

                }


            }



            ////RTGS /NEFT Payment
            if (conn.State != ConnectionState.Open) { conn.Open(); }
            stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                  + "where TRANSACTIONID='" + transId + "' and Convert(int,TENDERTYPE) =43 ";
            Cmd = new SqlCommand(stqry);
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            dtTend.Clear();
            dtTend = new DataTable();
            intTenderCount = 0;
            da.Fill(dtTend);
            if (dtTend.Rows.Count > 0)
            {
                decimal credit = Convert.ToDecimal(dtTend.Rows[0]["Amount"]);
                if (credit > 0)
                {
                    if (strPaymentMethods != "")
                    {
                        strPaymentMethods = strPaymentMethods + ", RTGS/NEFT";
                    }
                    else
                    {
                        strPaymentMethods = "RTGS/NEFT";
                    }
                }
            }



            ////Credit Payment
            if (conn.State != ConnectionState.Open) { conn.Open(); }
            stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                  + "where TRANSACTIONID='" + transId + "' and Convert(int,TENDERTYPE) =53 ";
            Cmd = new SqlCommand(stqry);
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            dtTend.Clear();
            dtTend = new DataTable();
            intTenderCount = 0;
            da.Fill(dtTend);
            if (dtTend.Rows.Count > 0)
            {
                decimal credit = Convert.ToDecimal(dtTend.Rows[0]["Amount"]);
                if (credit > 0)
                {
                    if (strPaymentMethods != "")
                    {
                        strPaymentMethods = strPaymentMethods + ", CREDIT PAYMENTS";
                    }
                    else
                    {
                        strPaymentMethods = "CREDIT PAYMENTS";
                    }
                }
            }

            ////Cash Payment
            if (conn.State != ConnectionState.Open) { conn.Open(); }
            stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                  + "where TRANSACTIONID='" + transId + "' and Convert(int,TENDERTYPE) =1 ";
            Cmd = new SqlCommand(stqry);
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            dtTend.Clear();
            dtTend = new DataTable();
            intTenderCount = 0;
            da.Fill(dtTend);
            if (dtTend.Rows.Count > 0)
            {
                decimal credit = Convert.ToDecimal(dtTend.Rows[0]["Amount"]);
                if (credit > 0)
                {
                    if (strPaymentMethods != "")
                    {
                        strPaymentMethods = strPaymentMethods + ", CASH";
                    }
                    else
                    {
                        strPaymentMethods = "CASH";
                    }
                }
            }
            */

            string placecity = "";
            if (conn.State != ConnectionState.Open) { conn.Open(); }
            stqry = "select '' AS  CITY  --------- crt.STOREADDRESSESVIEW where STORENUMBER='" + storeid + "'";

            Cmd = new SqlCommand(stqry);
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            DataTable dtcity = new DataTable();
            da.Fill(dtcity);
            if (dtcity.Rows.Count > 0)
            {
                placecity = dtcity.Rows[0]["CITY"].ToString();
                dtcust.Columns.Add(new DataColumn("Placecity", typeof(string)));
                dtcust.Rows[0]["Placecity"] = placecity;

            }

            conn.Close();

            mAmountWords = words(totalAmount);
            ReportDataSource RDS = new ReportDataSource("Dset", dtRTT);
            rptViewr.LocalReport.DataSources.Add(RDS);
            rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/Form60.rdl");

            rptViewr.LocalReport.DataSources.Clear();
            var Rds = new ReportDataSource();
            Rds.Name = "CustomerDetail";
            Rds.Value = dtcust;
            rptViewr.LocalReport.DataSources.Add(Rds);


            ReportParameter parameter = new ReportParameter();
            parameter.Name = "CustomerName";
            parameter.Values.Add(strCustomerName);
            rptViewr.LocalReport.SetParameters(parameter);



            ReportParameter CustPhone = new ReportParameter();
            CustPhone.Name = "CustPhone";
            CustPhone.Values.Add(strPhone);
            rptViewr.LocalReport.SetParameters(CustPhone);

            ReportParameter InvoiceDate = new ReportParameter();
            InvoiceDate.Name = "InvoiceDate";
            InvoiceDate.Values.Add(strTransDate);
            rptViewr.LocalReport.SetParameters(InvoiceDate);

            ReportParameter AmountInWords = new ReportParameter();
            AmountInWords.Name = "AmountInWords";
            AmountInWords.Values.Add(mAmountWords);
            rptViewr.LocalReport.SetParameters(AmountInWords);

            ReportParameter PaymentMethod = new ReportParameter();
            PaymentMethod.Name = "PaymentMethod";
            PaymentMethod.Values.Add(strPaymentMethods);
            rptViewr.LocalReport.SetParameters(PaymentMethod);
            rptViewr.LocalReport.Refresh();
            if (pdfflag == "0")
            {
                rptViewr.LocalReport.Refresh();
            }
            else if (pdfflag == "1")
            {
                DownloadPdf(strReceiptId + "_" + ReportType);
            }
            else if (pdfflag == "2")
            {
                string savePath = Server.MapPath("DownloadInvoice\\Form60.pdf");
                SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
            }
        }

        public static string words(Int32 numbers)
        {
            int number = numbers;

            if (number == 0) return "Zero";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            if (sb.ToString().TrimStart().Substring(0, 3) == "and")
                sb.Remove(0, 3);

            return "Rs. " + sb.ToString().TrimEnd() + " Only";
        }

        public void ShowAnalysisReport(string fromdate, string todate, string ReportType)
        {
            try
            {
                string ConnectionString = ConfigurationManager.AppSettings["DBCON"].ToString();
                SqlConnection conn = new SqlConnection(ConnectionString);

                SqlCommand Cmd;
                SqlDataAdapter da;


                if (conn.State != ConnectionState.Open) { conn.Open(); }
                string stqry = "select IT.ITEMTYPECODE,ABS(CAST(SUM(CASE WHEN RS.QTY<0 THEN RS.QTY ELSE 0 END ) AS DECIMAL(18,3)))  SALEWEIGHT ,     " + Environment.NewLine;
                stqry += "ABS(CAST(SUM(CASE WHEN RS.QTY<0 THEN RS.NETAMOUNT ELSE 0 END ) AS DECIMAL(18,3))) SALEAMOUNT ,				" + Environment.NewLine;
                stqry += "CAST(SUM(CASE WHEN RS.QTY<0 THEN ES.MAKINGVALUE ELSE 0 END ) AS DECIMAL(18,3)) SALEMAKINGVALUE,			" + Environment.NewLine;
                stqry += "CAST(SUM(CASE WHEN RS.QTY<0 THEN TH.PURCHASECOST ELSE 0 END ) AS DECIMAL(18,3)) SALEPURCHASECOST,		" + Environment.NewLine;
                stqry += "ABS(CAST(SUM(CASE WHEN RS.QTY>0 THEN RS.QTY ELSE 0 END ) AS DECIMAL(18,3)))  SALERETURNWEIGHT ,			" + Environment.NewLine;
                stqry += "ABS(CAST(SUM(CASE WHEN RS.QTY>0 THEN RS.NETAMOUNT ELSE 0 END ) AS DECIMAL(18,3))) SALERETURNAMOUNT ,	" + Environment.NewLine;
                stqry += "CAST(SUM(CASE WHEN RS.QTY>0 THEN ES.MAKINGVALUE ELSE 0 END ) AS DECIMAL(18,3)) SALERETURNMAKINGVALUE,	" + Environment.NewLine;
                stqry += "CAST(SUM(CASE WHEN RS.QTY>0 THEN TH.PURCHASECOST ELSE 0 END ) AS DECIMAL(18,3)) SALERETURNPURCHASECOST,	" + Environment.NewLine;
                stqry += "ABS((ABS(CAST(SUM(CASE WHEN RS.QTY<0 THEN RS.QTY ELSE 0 END ) AS DECIMAL(18,3)))-(ABS(CAST(SUM(CASE WHEN RS.QTY>0 THEN RS.QTY ELSE 0 END ) AS DECIMAL(18,3)))))) SALENETWEIGHT,                       	" + Environment.NewLine;
                stqry += "ABS((ABS(CAST(SUM(CASE WHEN RS.QTY<0 THEN RS.NETAMOUNT ELSE 0 END ) AS DECIMAL(18,3)))-(ABS(CAST(SUM(CASE WHEN RS.QTY>0 THEN RS.NETAMOUNT ELSE 0 END ) AS DECIMAL(18,3)))))) SALENETAMOUNT ,		   	" + Environment.NewLine;
                stqry += "ABS(((CAST(SUM(CASE WHEN RS.QTY<0 THEN ES.MAKINGVALUE ELSE 0 END ) AS DECIMAL(18,3)))-(ABS(CAST(SUM(CASE WHEN RS.QTY>0 THEN ES.MAKINGVALUE ELSE 0 END ) AS DECIMAL(18,3)))))) SALENETMAKINGVALUE , 	" + Environment.NewLine;
                stqry += "ABS(((CAST(SUM(CASE WHEN RS.QTY<0 THEN TH.PURCHASECOST ELSE 0 END ) AS DECIMAL(18,3)))-(ABS(CAST(SUM(CASE WHEN RS.QTY>0 THEN TH.PURCHASECOST ELSE 0 END ) AS DECIMAL(18,3)))))) SALENETPURCHASECOST	" + Environment.NewLine;
                stqry += "from  ax.retailtransactiontable RT WITH(NOLOCK)   " + Environment.NewLine;
                stqry += "INNER JOIN ax.retailtransactionsalestrans RS WITH(NOLOCK) ON RS.RECEIPTID=RT.RECEIPTID AND RS.TRANSACTIONID=RT.TRANSACTIONID  " + Environment.NewLine;
                stqry += "INNER JOIN ext.acxretailtransactionsalestrans ES WITH(NOLOCK) ON  ES.TRANSACTIONID=RS.TRANSACTIONID AND ES.ITEMID=RS.ITEMID   " + Environment.NewLine;
                stqry += "INNER JOIN Ext.acxInventtable IT  WITH(NOLOCK) ON IT.ITEMID=RS.ITEMID  " + Environment.NewLine;
                stqry += "INNER JOIN ext.acxTagheader TH WITH(NOLOCK)  ON TH.TAGNO=RS.INVENTSERIALID  " + Environment.NewLine;
                stqry += "where rt.TRANSDATE>='" + fromdate + "' and RT.TRANSDATE<='" + todate + "' GROUP BY  IT.ITEMTYPECODE  ";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtRTT = new DataTable();
                da.Fill(dtRTT);
                rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/AnalysisReport.rdl");
                var RdsHeader = new ReportDataSource();
                RdsHeader.Name = "ldata";
                RdsHeader.Value = dtRTT;
                rptViewr.LocalReport.DataSources.Add(RdsHeader);
                rptViewr.LocalReport.Refresh();
                conn.Close();

            }
            catch (Exception ee)
            {

            }
        }


        public void BrandWiseClosingStock(string SECTIONID, string BRAND, string ORNAMENTCATEGORYCODE, string StoreCode, string ReportType)
        {
            try
            {
                //SECTIONID = "SCBR"; BRAND = "SBR"; ORNAMENTCATEGORYCODE = "SL17";

                string ConnectionString = ConfigurationManager.AppSettings["DBCON"].ToString();
                SqlConnection conn = new SqlConnection(ConnectionString);

                SqlCommand Cmd;
                SqlDataAdapter da;


                if (conn.State != ConnectionState.Open) { conn.Open(); }
                string stqry = "SELECT SECTIONID,BRAND,ORNAMENTCATEGORYCODE,CAST(GROSSWEIGHT AS DECIMAL(18,3))GROSSWEIGHT, " + Environment.NewLine;
                stqry += " CAST(NETWEIGHT    AS DECIMAL(18,3) )NETWEIGHT,CAST(PURCHASECOST AS DECIMAL(18,3) ) PURCHASECOST FROM ext.acxtagheader " + Environment.NewLine;
                stqry += " WHERE SECTIONID LIKE CASE WHEN   '" + SECTIONID.ToUpper() + "'='ALL' THEN '%' ELSE '" + SECTIONID + "'END  AND  " + Environment.NewLine;
                stqry += " BRAND LIKE CASE WHEN   '" + BRAND.ToUpper() + "'='ALL' THEN '%' ELSE '" + BRAND + "' END  AND  " + Environment.NewLine;
                stqry += " ORNAMENTCATEGORYCODE LIKE CASE WHEN   '" + ORNAMENTCATEGORYCODE.ToUpper() + "'='ALL' THEN '%' ELSE '" + ORNAMENTCATEGORYCODE + "' END  AND  " + Environment.NewLine;
                stqry += " TAGSTATUS=1 AND  INVENTLOCATIONID='" + StoreCode + "'";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtRTT = new DataTable();
                da.Fill(dtRTT);
                rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/BrandWiseClosingStock.rdl");
                var RdsHeader = new ReportDataSource();
                RdsHeader.Name = "ldata";
                RdsHeader.Value = dtRTT;
                rptViewr.LocalReport.DataSources.Add(RdsHeader);
                rptViewr.LocalReport.Refresh();
                conn.Close();

            }
            catch (Exception ee)
            {

            }
        }

        public void CashDailySummaryReport(string Terminalid, string StoreCode, string CashDeposittoBank, string ReportType)
        {
            //Terminalid="ALL";
            //StoreCode="%";

            string ConnectionString = ConfigurationManager.AppSettings["DBCON"].ToString();
            SqlConnection conn = new SqlConnection(ConnectionString);


            SqlCommand Cmd;
            SqlDataAdapter da;
            string stqry = "";
            int i;
            decimal amount = 0;
            int pcs = 0;
            decimal weight = 0;
            decimal ReductionAmount = 0;
            int ReductionPcs = 0;
            decimal ReductionWeight = 0;
            DataTable dtTend = new DataTable();
            DataTable dtTotal = new DataTable();
            DataRow drRow;
            #region "Total Sale"
            if (conn.State != ConnectionState.Open) { conn.Open(); }
            stqry = "SELECT'Opening Cash Balance'PARTICULARS,ISNULL(CAST(SUM(AMOUNTTENDERED)AS DECIMAL(18,3)),0) AMOUNT,0 PIECES, 0.0[WEIGHT]" + Environment.NewLine;
            stqry += "FROM ax.retailtransactionPaymentTrans where TransactionStatus=0 and TenderType= 1 and " + Environment.NewLine;
            stqry += "TERMINAL LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL' THEN '%' ELSE '" + Terminalid + "' END" + Environment.NewLine;
            stqry += "AND  TRANSDATE =CAST(GETDATE()-1 AS DATE) AND  VOIDSTATUS=0 AND STORE LIKE '" + StoreCode + "'" + Environment.NewLine;
            stqry += "UNION ALL" + Environment.NewLine;
            stqry += "SELECT'Sales - Diamond' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,ISNULL(SUM(EP.PCS),0) PIECES ," + Environment.NewLine;
            stqry += "ISNULL(CAST(SUM(ES.GROSSWEIGHT) AS decimal),0) [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS INNER JOIN   ext.acxestimatetablePosted EP ON   " + Environment.NewLine;
            stqry += " EP.POSTEDTRANSACTIONID=RS.TRANSACTIONID  AND EP.RECEIPTID=RS.RECEIPTID AND EP.TERMINALID = RS.TERMINALID									   " + Environment.NewLine;
            stqry += "INNER JOIN ext.ACXRETAILTRANSACTIONSALESTRANS ES ON ES.TRANSACTIONID=RS.TRANSACTIONID AND ES.TERMINALID=RS.TERMINALID						   " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID																					   " + Environment.NewLine;
            stqry += "WHERE RS.QTY <0  AND IT.LinkBarcode=1 AND RS.TERMINALID LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL' THEN '%' ELSE '" + Terminalid + "'  END" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "'AND RS.TRANSDATE=CAST(GETDATE() AS DATE)" + Environment.NewLine;
            stqry += " UNION ALL																																	   " + Environment.NewLine;
            stqry += "SELECT'Sales - Gold' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,ISNULL(SUM(EP.PCS),0) PIECES ,   " + Environment.NewLine;
            stqry += "ISNULL(CAST(SUM(ES.GROSSWEIGHT) AS decimal),0) [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS INNER JOIN   ext.acxestimatetablePosted EP ON   " + Environment.NewLine;
            stqry += " EP.POSTEDTRANSACTIONID=RS.TRANSACTIONID  AND EP.RECEIPTID=RS.RECEIPTID AND EP.TERMINALID = RS.TERMINALID									   " + Environment.NewLine;
            stqry += "INNER JOIN ext.ACXRETAILTRANSACTIONSALESTRANS ES ON ES.TRANSACTIONID=RS.TRANSACTIONID AND ES.TERMINALID=RS.TERMINALID						   " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID																					   " + Environment.NewLine;
            stqry += "WHERE RS.QTY <0  AND IT.LinkBarcode=2 AND RS.TERMINALID LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL' THEN '%' ELSE '" + Terminalid + "'  END" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "'AND RS.TRANSDATE=CAST(GETDATE() AS DATE)" + Environment.NewLine;
            stqry += "UNION ALL																																	   " + Environment.NewLine;
            stqry += "SELECT'Sales-MRP' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,ISNULL(SUM(EP.PCS),0) PIECES ,	   " + Environment.NewLine;
            stqry += "ISNULL(CAST(SUM(ES.GROSSWEIGHT) AS decimal),0) [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS INNER JOIN   ext.acxestimatetablePosted EP ON   " + Environment.NewLine;
            stqry += " EP.POSTEDTRANSACTIONID=RS.TRANSACTIONID  AND EP.RECEIPTID=RS.RECEIPTID AND EP.TERMINALID = RS.TERMINALID									   " + Environment.NewLine;
            stqry += "INNER JOIN ext.ACXRETAILTRANSACTIONSALESTRANS ES ON ES.TRANSACTIONID=RS.TRANSACTIONID AND ES.TERMINALID=RS.TERMINALID						   " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID																					   " + Environment.NewLine;
            stqry += "WHERE RS.QTY <0  AND IT.LinkBarcode=3 AND  RS.TERMINALID LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL' THEN '%' ELSE '" + Terminalid + "'  END" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "' AND RS.TRANSDATE=CAST(GETDATE() AS DATE)" + Environment.NewLine;
            stqry += "UNION ALL																																	   " + Environment.NewLine;
            stqry += "SELECT'Sales-MRP with Wt'PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,ISNULL(SUM(EP.PCS),0) PIECES," + Environment.NewLine;
            stqry += "ISNULL(CAST(SUM(ES.GROSSWEIGHT) AS decimal),0) [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS INNER JOIN   ext.acxestimatetablePosted EP ON   " + Environment.NewLine;
            stqry += " EP.POSTEDTRANSACTIONID=RS.TRANSACTIONID  AND EP.RECEIPTID=RS.RECEIPTID AND EP.TERMINALID = RS.TERMINALID									   " + Environment.NewLine;
            stqry += "INNER JOIN ext.ACXRETAILTRANSACTIONSALESTRANS ES ON ES.TRANSACTIONID=RS.TRANSACTIONID AND ES.TERMINALID=RS.TERMINALID						   " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID																					   " + Environment.NewLine;
            stqry += "WHERE RS.QTY <0  AND IT.LinkBarcode=4 AND  RS.TERMINALID LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL' THEN '%' ELSE '" + Terminalid + "'  END" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "' AND RS.TRANSDATE=CAST(GETDATE() AS DATE)" + Environment.NewLine;
            stqry += "UNION ALL																																	   " + Environment.NewLine;
            stqry += "SELECT'Sales-Silver' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,ISNULL(SUM(EP.PCS),0) PIECES ,   " + Environment.NewLine;
            stqry += "ISNULL(CAST(SUM(ES.GROSSWEIGHT) AS decimal),0) [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS INNER JOIN   ext.acxestimatetablePosted EP ON   " + Environment.NewLine;
            stqry += " EP.POSTEDTRANSACTIONID=RS.TRANSACTIONID  AND EP.RECEIPTID=RS.RECEIPTID AND EP.TERMINALID = RS.TERMINALID									   " + Environment.NewLine;
            stqry += "INNER JOIN ext.ACXRETAILTRANSACTIONSALESTRANS ES ON ES.TRANSACTIONID=RS.TRANSACTIONID AND ES.TERMINALID=RS.TERMINALID						   " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID																					   " + Environment.NewLine;
            stqry += "WHERE RS.QTY <0  AND IT.LinkBarcode=5 AND RS.TERMINALID LIKE CASE WHEN'" + Terminalid.ToUpper() + "'='ALL' THEN '%' ELSE '" + Terminalid + "'  END" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "' AND RS.TRANSDATE=CAST(GETDATE() AS DATE)" + Environment.NewLine;
            stqry += "UNION ALL																																	   " + Environment.NewLine;
            stqry += "SELECT'Sales-Platinum' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,ISNULL(SUM(EP.PCS),0) PIECES , " + Environment.NewLine;
            stqry += "ISNULL(CAST(SUM(ES.GROSSWEIGHT) AS decimal),0) [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS INNER JOIN   ext.acxestimatetablePosted EP ON   " + Environment.NewLine;
            stqry += " EP.POSTEDTRANSACTIONID=RS.TRANSACTIONID  AND EP.RECEIPTID=RS.RECEIPTID AND EP.TERMINALID = RS.TERMINALID									   " + Environment.NewLine;
            stqry += "INNER JOIN ext.ACXRETAILTRANSACTIONSALESTRANS ES ON ES.TRANSACTIONID=RS.TRANSACTIONID AND ES.TERMINALID=RS.TERMINALID						   " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID																					   " + Environment.NewLine;
            stqry += "WHERE RS.QTY <0  AND IT.LinkBarcode=6 AND RS.TERMINALID LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL' THEN '%' ELSE '" + Terminalid + "'  END" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "' AND RS.TRANSDATE=CAST(GETDATE() AS DATE)" + Environment.NewLine;
            stqry += "UNION ALL																																	   " + Environment.NewLine;
            stqry += "SELECT'Sales-Gossip/Bullion' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,ISNULL(SUM(EP.PCS),0) PIECES ," + Environment.NewLine;
            stqry += "ISNULL(CAST(SUM(ES.GROSSWEIGHT) AS decimal),0) [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS INNER JOIN   ext.acxestimatetablePosted EP ON " + Environment.NewLine;
            stqry += " EP.POSTEDTRANSACTIONID=RS.TRANSACTIONID  AND EP.RECEIPTID=RS.RECEIPTID AND EP.TERMINALID = RS.TERMINALID									 " + Environment.NewLine;
            stqry += "INNER JOIN ext.ACXRETAILTRANSACTIONSALESTRANS ES ON ES.TRANSACTIONID=RS.TRANSACTIONID AND ES.TERMINALID=RS.TERMINALID						 " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID																					 " + Environment.NewLine;
            stqry += "WHERE RS.QTY <0  AND IT.LinkBarcode=7 AND RS.TERMINALID LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL'  THEN '%' ELSE '" + Terminalid + "'  END" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "' AND RS.TRANSDATE=CAST(GETDATE() AS DATE)" + Environment.NewLine;
            stqry += "UNION ALL																																	 " + Environment.NewLine;
            stqry += "SELECT'Sales-Gift Card'PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,ISNULL(SUM(EP.PCS),0) PIECES," + Environment.NewLine;
            stqry += "ISNULL(CAST(SUM(ES.GROSSWEIGHT) AS decimal),0) [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS INNER JOIN   ext.acxestimatetablePosted EP ON " + Environment.NewLine;
            stqry += " EP.POSTEDTRANSACTIONID=RS.TRANSACTIONID  AND EP.RECEIPTID=RS.RECEIPTID AND EP.TERMINALID = RS.TERMINALID									 " + Environment.NewLine;
            stqry += "INNER JOIN ext.ACXRETAILTRANSACTIONSALESTRANS ES ON ES.TRANSACTIONID=RS.TRANSACTIONID AND ES.TERMINALID=RS.TERMINALID						 " + Environment.NewLine;
            stqry += "WHERE RS.QTY <0  AND RS.TERMINALID LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL' THEN '%' ELSE '" + Terminalid + "'  END" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "' AND RS.GIFTCARDNUMBER!='' AND  RS.TRANSDATE=CAST(GETDATE() AS DATE) ";


            Cmd = new SqlCommand(stqry);
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;

            da.Fill(dtTend);
            dtTotal = dtTend;



            for (i = 1; i < dtTend.Rows.Count; i++)
            {
                amount += Convert.ToDecimal(dtTend.Rows[i]["AMOUNT"]);
                pcs += Convert.ToInt16(dtTend.Rows[i]["PIECES"]);
                weight += Convert.ToDecimal(dtTend.Rows[i]["WEIGHT"]);

            }
            if (dtTotal.Rows.Count > 0)
            {
                drRow = dtTotal.NewRow();
                drRow["PARTICULARS"] = "TOTAL SALES";
                drRow["AMOUNT"] = amount;
                drRow["PIECES"] = pcs;
                drRow["WEIGHT"] = weight;
                dtTotal.Rows.Add(drRow);
            }
            #endregion

            #region "Total Receipt"
            stqry = "SELECT'Advance Received' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ," + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID" + Environment.NewLine;
            stqry += "WHERE IT.ADVANCEITEM=1 AND RS.TERMINALID LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL'  THEN '%' ELSE '" + Terminalid + "'  END" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "' AND RS.TRANSDATE=CAST(GETDATE() AS DATE)" + Environment.NewLine;
            stqry += "UNION ALL " + Environment.NewLine;
            stqry += "SELECT'Scheme Received (Emi)' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ," + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID " + Environment.NewLine;
            stqry += "WHERE IT.ADVANCEITEM=2 AND RS.TERMINALID LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL'  THEN '%' ELSE '" + Terminalid + "'  END" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "' AND RS.TRANSDATE=CAST(GETDATE() AS DATE)" + Environment.NewLine;
            stqry += "UNION ALL" + Environment.NewLine;
            stqry += "select'Cash Receipts' PARTICULARS ,isnull(CAST(SUM(IncomeExpenseAmount) AS DECIMAL(18,3)),0) AMOUNT, 0 PIECES ,0 [WEIGHT] from Ax.RetailTransactionTable RS " + Environment.NewLine;
            stqry += "where RS.TERMINAL LIKE CASE WHEN '" + Terminalid.ToUpper() + "'='ALL'  THEN '%' ELSE  '" + Terminalid + "' END " + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "' AND  IncomeExpenseAmount<=0 AND RS.TRANSDATE=CAST(GETDATE() AS DATE)";


            Cmd = new SqlCommand(stqry);
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            DataTable dttotalreceipt = new DataTable();
            da.Fill(dttotalreceipt);
            dtTotal.Merge(dttotalreceipt, true, MissingSchemaAction.Ignore);
            //for (i = 0; i < dttotalreceipt.Rows.Count; i++)
            //{
            //    amount += Convert.ToDecimal(dttotalreceipt.Rows[i]["AMOUNT"]);
            //    pcs += Convert.ToInt16(dttotalreceipt.Rows[i]["PIECES"]);
            //    weight += Convert.ToDecimal(dttotalreceipt.Rows[i]["WEIGHT"]);

            //}
            amount = Convert.ToDecimal(dttotalreceipt.Compute("SUM(AMOUNT)", string.Empty));
            pcs = Convert.ToInt16(dttotalreceipt.Compute("SUM(PIECES)", string.Empty));
            weight = Convert.ToDecimal(dttotalreceipt.Compute("SUM(WEIGHT)", string.Empty));

            if (dtTotal.Rows.Count > 0)
            {
                drRow = dtTotal.NewRow();
                drRow["PARTICULARS"] = "Total Receipt";
                drRow["AMOUNT"] = amount;
                drRow["PIECES"] = pcs;
                drRow["WEIGHT"] = weight;
                dtTotal.Rows.Add(drRow);
            }

            #endregion

            #region "Total Deduction"


            stqry = "SELECT'Credit Sales (Customer Account)' PARTICULARS,ISNULL(CAST(SUM(ABS(RS.AMOUNTTENDERED)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ," + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM ax.retailtransactionPaymentTrans RS WHERE RS.TRANSACTIONSTATUS=0 AND TENDERTYPE=4 AND	RS.TRANSDATE=CAST(GETDATE() AS DATE) AND 	" + Environment.NewLine;
            stqry += " 	RS.TERMINAL LIKE CASE WHEN  '" + Terminalid.ToUpper() + "'='ALL'  THEN '%' ELSE 'ALL'  END AND RS.VOIDSTATUS=0 AND RS.STORE = '" + StoreCode + "'																" + Environment.NewLine;
            stqry += "UNION ALL 																														" + Environment.NewLine;
            stqry += "SELECT'Credit Card' PARTICULARS,ISNULL(CAST(SUM(ABS(RS.AMOUNTTENDERED)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,					" + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM ax.retailtransactionPaymentTrans RS 																			" + Environment.NewLine;
            stqry += "WHERE RS.TRANSACTIONSTATUS=0 AND TENDERTYPE>=20 AND TENDERTYPE <=37 AND RS.TRANSDATE=CAST(GETDATE() AS DATE) AND" + Environment.NewLine;
            stqry += " RS.TERMINAL LIKE CASE WHEN  '" + Terminalid.ToUpper() + "'='ALL'  THEN '%' ELSE 'ALL'  END	AND RS.VOIDSTATUS=0															" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "'																									" + Environment.NewLine;
            stqry += "UNION ALL 																														" + Environment.NewLine;
            stqry += "SELECT'Cheque/NEFT/RTGS' PARTICULARS,ISNULL(CAST(SUM(ABS(RS.AMOUNTTENDERED)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,				" + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM ax.retailtransactionPaymentTrans RS 																			" + Environment.NewLine;
            stqry += "WHERE RS.TRANSACTIONSTATUS=0 AND TENDERTYPE IN (42,43) AND RS.TRANSDATE=CAST(GETDATE() AS DATE) AND																		" + Environment.NewLine;
            stqry += " RS.TERMINAL LIKE CASE WHEN  '" + Terminalid.ToUpper() + "'='ALL'  THEN '%' ELSE 'ALL'  END	AND RS.VOIDSTATUS=0															" + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "'																									" + Environment.NewLine;
            stqry += "UNION ALL																														" + Environment.NewLine;
            stqry += "SELECT 'Local Purchase  - Diamond' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ," + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=1" + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END " + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  AND IT.OLDBULLION=1 OR IT.OLDITEM=1 AND RS.TRANSDATE=CAST(GETDATE() AS DATE)  " + Environment.NewLine;
            stqry += "UNION ALL " + Environment.NewLine;
            stqry += "SELECT 'Local Purchase-Gold' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ," + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						  " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=2														  " + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												  " + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  AND IT.OLDBULLION=1 OR IT.OLDITEM=1	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)							  " + Environment.NewLine;
            stqry += "UNION ALL																																  " + Environment.NewLine;
            stqry += "SELECT 'Local Purchase-MRP' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,			  " + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						  " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=3														  " + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												  " + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  AND IT.OLDBULLION=1 OR IT.OLDITEM=1 AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																	  " + Environment.NewLine;
            stqry += "UNION ALL																																  " + Environment.NewLine;
            stqry += "SELECT 'Local Purchase-MRP with Wt' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,	  " + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						  " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=4														  " + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												  " + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  AND IT.OLDBULLION=1 OR IT.OLDITEM=1	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																  " + Environment.NewLine;
            stqry += "UNION ALL																																  " + Environment.NewLine;
            stqry += "SELECT 'Local Purchase-Silver' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,		  " + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						  " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=5														  " + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												  " + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  AND IT.OLDBULLION=1 OR IT.OLDITEM=1	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																  " + Environment.NewLine;
            stqry += "UNION ALL																																  " + Environment.NewLine;
            stqry += "SELECT 'Local Purchase-Platinum' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,	  " + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						  " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=6														  " + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												  " + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  AND IT.OLDBULLION=1 OR IT.OLDITEM=1	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																  " + Environment.NewLine;
            stqry += "UNION ALL																																  " + Environment.NewLine;
            stqry += "SELECT 'Local Purchase-Gossip/Bullion' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ," + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						" + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=7														" + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												" + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0 AND IT.OLDBULLION=1 OR IT.OLDITEM=1	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																" + Environment.NewLine;
            stqry += "UNION ALL																																" + Environment.NewLine;
            stqry += "SELECT 'Sales Return-Diamond' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,		" + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						" + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=1														" + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												" + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																								" + Environment.NewLine;
            stqry += "UNION ALL																																" + Environment.NewLine;
            stqry += "SELECT 'Sales Return-Gold' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,			" + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						" + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=2														" + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												" + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																									" + Environment.NewLine;
            stqry += "UNION ALL																																" + Environment.NewLine;
            stqry += "SELECT 'Sales Return-MRP' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,			" + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						" + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=3														" + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												" + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																									" + Environment.NewLine;
            stqry += "UNION ALL																																" + Environment.NewLine;
            stqry += "SELECT 'Sales Return-MRP with Wt' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,	" + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						" + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=4														" + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												" + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																									" + Environment.NewLine;
            stqry += "UNION ALL																																" + Environment.NewLine;
            stqry += "SELECT 'Sales Return-Silver' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,		" + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						" + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=5														" + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												" + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0 	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																									" + Environment.NewLine;
            stqry += "UNION ALL																																" + Environment.NewLine;
            stqry += "SELECT 'Sales Return-Platinum' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,		" + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																						" + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=6														" + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END												" + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0  	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																								" + Environment.NewLine;
            stqry += "UNION ALL																																" + Environment.NewLine;
            stqry += "SELECT 'Sales Return-Gossip/Bullion' PARTICULARS,ISNULL(CAST(SUM(ABS(NETAMOUNT)+ABS(RS.TAXAMOUNT)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ," + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM Ax.RetailTransactionSalesTrans RS 																		   " + Environment.NewLine;
            stqry += " INNER  JOIN ext.acxInventTable IT ON IT.ITEMID = RS.ITEMID AND IT.LINKBARCODE=7										   " + Environment.NewLine;
            stqry += "WHERE  RS.TERMINALID LIKE CASE WHEN RS.TERMINALID='ALL'  THEN '%' ELSE RS.TERMINALID END								   " + Environment.NewLine;
            stqry += "AND RS.STORE = '%' AND RS.QTY>0 	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																					   " + Environment.NewLine;
            stqry += "UNION ALL																												   " + Environment.NewLine;
            stqry += "SELECT'Loyalty Redeemed' PARTICULARS,ISNULL(CAST(SUM(ABS(RS.AMOUNTTENDERED)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,		   " + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM ax.retailtransactionPaymentTrans RS 																	   " + Environment.NewLine;
            stqry += "WHERE RS.TRANSACTIONSTATUS=0 AND TENDERTYPE=10 AND 																		   " + Environment.NewLine;
            stqry += " RS.TERMINAL LIKE CASE WHEN  '" + Terminalid.ToUpper() + "'='ALL'  THEN '%' ELSE 'ALL'  END														   " + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "'	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)	AND RS.VOIDSTATUS=0																					   " + Environment.NewLine;
            stqry += "UNION ALL																												   " + Environment.NewLine;
            stqry += "SELECT'DV,GV,GC- Redeemed' PARTICULARS,ISNULL(CAST(SUM(ABS(RS.AMOUNTTENDERED)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,	   " + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM ax.retailtransactionPaymentTrans RS 																	   " + Environment.NewLine;
            stqry += "WHERE RS.TRANSACTIONSTATUS=0 AND TENDERTYPE IN (38,39,40) AND 															   " + Environment.NewLine;
            stqry += " RS.TERMINAL LIKE CASE WHEN  '" + Terminalid.ToUpper() + "'='ALL'  THEN '%' ELSE 'ALL'  END														   " + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "' AND RS.TRANSDATE=CAST(GETDATE() AS DATE)	AND RS.VOIDSTATUS=0																						   " + Environment.NewLine;
            stqry += "UNION ALL																												   " + Environment.NewLine;
            stqry += "SELECT'Advance Adjusted' PARTICULARS,ISNULL(CAST(SUM(ABS(RS.AMOUNTTENDERED)) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,		   " + Environment.NewLine;
            stqry += "0 [WEIGHT] FROM ax.retailtransactionPaymentTrans RS 																	   " + Environment.NewLine;
            stqry += "WHERE RS.TRANSACTIONSTATUS=0 AND TENDERTYPE =44 AND 																	   " + Environment.NewLine;
            stqry += " RS.TERMINAL LIKE CASE WHEN  '" + Terminalid.ToUpper() + "'='ALL'  THEN '%' ELSE 'ALL'  END														   " + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "'	AND RS.TRANSDATE=CAST(GETDATE() AS DATE)																						   " + Environment.NewLine;
            stqry += "UNION ALL																												   " + Environment.NewLine;
            stqry += "SELECT 'Easy Gold Repayments' PARTICULARS, ISNULL(CAST(SUM(SchemeClosingValue) AS DECIMAL(18,3)),0)AMOUNT,0 PIECES ,	   " + Environment.NewLine;
            stqry += "0 [WEIGHT]  FROM ext.acxcustomerschemeentry WHERE MATURITYDATE= CAST(GETDATE()-1 AS DATE) AND 										   " + Environment.NewLine;
            stqry += "INVENTLOCATIONID LIKE '%' AND  SCHEMEENTRYSTATUS=2																		   " + Environment.NewLine;
            stqry += "UNION ALL																												   " + Environment.NewLine;
            stqry += "select'Other Cash payment (MIS expenses)' PARTICULARS ,isnull(CAST(SUM(IncomeExpenseAmount) AS DECIMAL(18,3)),0) AMOUNT, 0 PIECES    " + Environment.NewLine;
            stqry += " ,0 [WEIGHT] from Ax.RetailTransactionTable RS where RS.TERMINAL LIKE CASE WHEN  '" + Terminalid.ToUpper() + "'='ALL'   THEN '%' ELSE  '%' END        " + Environment.NewLine;
            stqry += "AND RS.STORE = '" + StoreCode + "'	 AND  IncomeExpenseAmount>=0 AND RS.TRANSDATE=CAST(GETDATE() AS DATE) ";

            Cmd = new SqlCommand(stqry);
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            DataTable dttotaldeduction = new DataTable();
            da.Fill(dttotaldeduction);
            dtTotal.Merge(dttotaldeduction, true, MissingSchemaAction.Ignore);

            // for (i = 0; i < dttotaldeduction.Rows.Count; i++)
            //{
            //    ReductionAmount += Convert.ToDecimal(dttotaldeduction.Rows[i]["AMOUNT"]);
            //    //ReductionPcs += Convert.ToInt16(dttotaldeduction.Rows[i]["PIECES"]);
            //    //ReductionWeight += Convert.ToDecimal(dttotaldeduction.Rows[i]["WEIGHT"]);

            //}
            ReductionAmount = Convert.ToDecimal(dttotaldeduction.Compute("SUM(AMOUNT)", string.Empty));
            if (dtTotal.Rows.Count > 0)
            {
                drRow = dtTotal.NewRow();
                drRow["PARTICULARS"] = "Total Deduction";
                drRow["AMOUNT"] = ReductionAmount;
                drRow["PIECES"] = ReductionPcs;
                drRow["WEIGHT"] = ReductionWeight;
                dtTotal.Rows.Add(drRow);
                drRow = dtTotal.NewRow();

            }

            #endregion

            #region "Balance Cash And  "Cash Deposit to Bank And Closing Cash "
            if (dtTotal.Rows.Count > 0)
            {
                drRow = dtTotal.NewRow();
                drRow["PARTICULARS"] = "Balance Cash";
                object Openingamount = dtTotal.Rows[0]["AMOUNT"];
                int Openingpiece = 0; // Convert.ToInt16(dtTotal.Rows[0]["PIECES"]);
                decimal OpeningWeight = 0; //Convert.ToDecimal(dtTotal.Rows[0]["WEIGHT"]);

                drRow["AMOUNT"] = (Convert.ToDecimal(Openingamount) + amount - ReductionAmount);  /////amount is total receipt which summ of 
                drRow["PIECES"] = 0;       //(Openingpiece + pcs - ReductionPcs);
                drRow["WEIGHT"] = 0;      //(OpeningWeight + weight - ReductionWeight);
                dtTotal.Rows.Add(drRow);

                drRow = dtTotal.NewRow();
                drRow["PARTICULARS"] = "Cash Deposit to Bank";
                drRow["AMOUNT"] = CashDeposittoBank;
                drRow["PIECES"] = 0;
                drRow["WEIGHT"] = 0;
                dtTotal.Rows.Add(drRow);

                drRow = dtTotal.NewRow();
                drRow["PARTICULARS"] = "Closing Cash as on Current Date";
                drRow["AMOUNT"] = Math.Abs(Convert.ToDecimal(Openingamount) + amount - ReductionAmount) - Convert.ToDecimal(CashDeposittoBank);
                drRow["PIECES"] = 0;
                drRow["WEIGHT"] = 0;
                dtTotal.Rows.Add(drRow);

            }


            #endregion

            rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/CashDailySummaryReport.rdl");
            var RdsHeader = new ReportDataSource();
            RdsHeader.Name = "ldata";
            RdsHeader.Value = dtTotal;
            rptViewr.LocalReport.DataSources.Add(RdsHeader);
            rptViewr.LocalReport.Refresh();
            conn.Close();

        }

        public void SaleReturn(string strReceiptId, string storecode, string pdfflag, string ReportType)
        {
            DataTable dtcompany = new DataTable();
            DataTable dtcust = new DataTable();
            DataTable dtsalereturn = new DataTable();
            DataTable dttax = new DataTable();

            dttax = new DataTable();
            dttax.Columns.Add("Description");
            dttax.Columns.Add("TaxAmount");
            DataTable dtstoreinfo = new DataTable();
            string strqry = "";
            string custaccount = "";
            string mAmountWords = "";
            string ConnectionString = ConfigurationManager.AppSettings["POSDBCON"].ToString();
            SqlConnection conn = new SqlConnection(ConnectionString);

            SqlCommand Cmd;
            SqlDataAdapter da;

            #region"companyDetails"
            //strqry = "select  SI.GSTIN,SI.COMPANYNAME,SI.ADDRESS1,SI.ADDRESS2,SI.ADDRESS3 ,SI.POSINVOICEHEADER1,SI.POSINVOICEHEADER2,SI.POSINVOICEHEADER3,RT.RECEIPTID,RT.TRANSDATE " + Environment.NewLine;
            //strqry += ",RT.CUSTACCOUNT,'' CITY,'' PINCODE,SI.CIN,SI.PAN,SI.STATECODE,ISNULL(B.NAME,'') STATENAME from ext.AcxStoreInfo SI WITH(NOLOCK) JOIN Ax.RETAILTRANSACTIONSALESTRANS   RT WITH(NOLOCK)  ON RT.STORE=SI.STORENUMBER" + Environment.NewLine;
            //strqry += "JOIN Ext.AcxRetailTransactionSalesTrans ET WITH(NOLOCK)  ON ET.TRANSACTIONID=RT.TRANSACTIONID AND ET.LINENUM=RT.LINENUM AND ET.STORE=RT.STORE" + Environment.NewLine;
            //strqry += " LEFT JOIN ax.LOGISTICSADDRESSSTATE B on b.STATEID = SI.STATECODE" + Environment.NewLine;
            //strqry += "where RT.RECEIPTID='" + strReceiptId + "' AND RT.STORE='" + storecode + "' and ET.SALESTYPE=1";

            strqry = "    SELECT GSTIN,COMPANYNAME,ADDRESS1,ADDRESS2,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3," + Environment.NewLine;
            strqry += "  RECEIPTID,TRANSDATE,CUSTACCOUNT,'' CITY,'' PINCODE,CIN,PAN,STATECODE,STATENAME  " + Environment.NewLine;
            strqry += "  FROM ACXSTOREDETAILS SI " + Environment.NewLine;
            strqry += "  JOIN ACXINVOICETABLE ET " + Environment.NewLine;
            strqry += "  ON SI.STORENUMBER=ET.STORE " + Environment.NewLine;
            strqry += "  WHERE ET.RECEIPTID='" + strReceiptId + "' AND ET.STORE='" + storecode + "' ";
            dtcompany = GetData(strqry);
            if (dtcompany.Rows.Count > 0)
            {
                custaccount = dtcompany.Rows[0]["CUSTACCOUNT"].ToString();
            }
            else
            {
                Response.Redirect("ErrorPage.aspx?Error=" + "No Company  Details Found" + "", false);

                return;
            }
            #endregion


            #region"Recipient Details"
            //string custqry1 = "Select A.ACCOUNTNUM,b.CUSTNAME NAME,replace(replace(COALEsCE(B.ADDRESS,''),char(10),' '),char(13),' ') as       " + Environment.NewLine;
            //custqry1 += "CustAddress, COALEsCE(B.CITY,'') as CustCity , COALESCE(B.COUNTRYREGIONID, '') as CustCountRegion,            " + Environment.NewLine;
            //custqry1 += "COALESCE(B.DISTRICTNAME, '') as CustDistrictName ,  COALESCE(LT.STATECODE_IN, '') as CustStateCode, COALESCE(B.STREET, '') as CustStreet            " + Environment.NewLine;
            //custqry1 += ", COALESCE(B.ZIPCODE, '') as CustZipCode , COALESCE(B.PANNUMBER, '') as PANNumber  " + Environment.NewLine;
            //custqry1 += ", COALESCE(TRI.REGISTRATIONNUMBER ,'') as RegistrationNumber,B.CITY,B.PHONE,B.CIN, B.ZIPCODE  " + Environment.NewLine;
            //custqry1 += ", COALESCE(B.STATENAME, '') as StateName   " + Environment.NewLine;
            //custqry1 += " From ax.CUSTTABLE a INNER JOIN ext.ACXCUSTOMERDETAILSVIEW b on a.ACCOUNTNUM = b.CUSTACCOUNT   " + Environment.NewLine;
            ///*add the four join to getting the gst no on 25-12-2020*/
            //custqry1 += " LEFT JOIN AX.DIRPARTYTABLE DT ON DT.RECID=A.PARTY" + Environment.NewLine;
            //custqry1 += "LEFT JOIN CRT.CUSTOMERPOSTALADDRESSESVIEW CV ON CV.PARTYNUMBER=DT.PARTYNUMBER AND CV.ISPRIMARY=1" + Environment.NewLine;
            //custqry1 += "LEFT JOIN ax.TAXINFORMATION_IN  TI ON  TI.REGISTRATIONLOCATION=CV.LOGISTICSLOCATIONRECID AND TI.ISPRIMARY=1" + Environment.NewLine;
            //custqry1 += "LEFT JOIN ax.TAXREGISTRATIONNUMBERS_IN TRI ON TRI.RECID=TI.GSTIN " + Environment.NewLine;
            ///*end */
            ///*ADDING THIS JOIN FOR GETTING THE STATECODE 25-12-2020*/
            //custqry1 += "LEFT JOIN [ext].[ACXLOGISTICSADDRESSSTATE] LT ON LT.STATEID=B.STATE  AND LT.COUNTRYREGIONID =B.COUNTRYREGIONID " + Environment.NewLine;
            ///*END*/
            //custqry1 += "where ACCOUNTNUM  = '" + custaccount + "' ";

            string custqry1 = "SELECT CUSTACCOUNT AS ACCOUNTNUM,NAME, " + Environment.NewLine;
            custqry1 += "replace(replace(COALEsCE(ADDRESS,''),char(10),' '),char(13),' ') as CustAddress,  " + Environment.NewLine;
            custqry1 += "COALEsCE(CITY,'') as CustCity,COALESCE(COUNTRYREGIONID, '') as CustCountRegion,  " + Environment.NewLine;
            custqry1 += "COALESCE(DISTRICTNAME, '') as CustDistrictName,  " + Environment.NewLine;
            custqry1 += "COALESCE(STATECODE, '') as CustStateCode, COALESCE(STREET, '') as CustStreet,  " + Environment.NewLine;
            custqry1 += "COALESCE(ZIPCODE, '') as CustZipCode,COALESCE(PANNUMBER, '') as PANNumber, " + Environment.NewLine;
            custqry1 += "COALESCE(GSTIN ,'')  as RegistrationNumber,COALESCE(STATENAME, '') as StateName,PHONE,'' CIN,ZIPCODE,CITY  " + Environment.NewLine;
            custqry1 += "FROM DBO.ACXCUSTDETAILS" + Environment.NewLine;
            custqry1 += "WHERE CUSTACCOUNT=  '" + custaccount + "' ";
            dtcust = GetData(custqry1);

            #endregion

            #region"SALE Return Line"
            /*Added the unit column 26-12-2020 ,
             * Making Rate Value remove from the rdl(case when ET.FINALMETALV>0 and ET.FINALMAKINGV >0 then CAST((ET.FINALMETALV/ET.FINALMAKINGV)/100 AS decimal(18,4)) else 0 end [MAKING RATE]  )
            // */
            //strqry = "select ROW_NUMBER() OVER(ORDER BY RT.LINENUM ) SONO,IT.ITEMID,TB.RECEIPTID [SALE INVOICE],TB.TRANSDATE [SALE DATE],IIT.NAMEALIAS,ET.SALESPERSONCODE," + Environment.NewLine;
            //strqry += "HSNCODE_IN HSNCODE,RT.UNIT,ET.PCS,CAST(ET.GROSSWEIGHT AS decimal(18,3)) GROSSWEIGHT,CAST(ET.FINALDIAMONDWT+ET.FINALSTONECWT+ et.FINALSTONEGWT AS decimal(18,3))" + Environment.NewLine;
            //strqry += "[DIAMOND CT],CAST(ET.NETWEIGHT AS decimal(18,3)) NETWEIGHT,CAST(ET.FINALMETALV AS decimal(18,3)) [METAL VAL],CAST(et.FINALMAKINGV AS DECIMAL(18,3)) MAKINGVALUE, " + Environment.NewLine;
            //strqry += "CAST(ET.FINALDIAMONDV+ET.FINALSTONECV+et.FINALSTONEGV AS decimal(18,3))[STONE VAL] " + Environment.NewLine;
            //strqry += ",CAST(RT.NETAMOUNT AS decimal(18,3)) [TAXABLE VALUE],CAST(RT.TAXAMOUNT  AS decimal(18,3)) TAXAMOUNT,							   " + Environment.NewLine;
            //strqry += "CAST(RT.NETAMOUNT+RT.TAXAMOUNT AS decimal(18,3)) [TOTAL] ,cast(et.FINALSTONEGWT as decimal(18,3)) [Stone Wt],cast(et.FINALSTONEGV as decimal(18,3)) [G_STONE VAL]  " + Environment.NewLine;
            //strqry += "from Ax.RetailTransactionSalesTrans RT WITH(NOLOCK) JOIN Ext.AcxRetailTransactionSalesTrans ET WITH(NOLOCK)  ON ET.TRANSACTIONID=RT.TRANSACTIONID AND ET.LINENUM=RT.LINENUM 		   " + Environment.NewLine;
            //strqry += " AND ET.STORE=RT.STORE																											   " + Environment.NewLine;
            //strqry += "JOIN AX.RETAILTRANSACTIONTABLE  TB  WITH(NOLOCK) ON TB.TRANSACTIONID=RT.RETURNTRANSACTIONID 									   " + Environment.NewLine;
            //strqry += "JOIN ext.acxInventTable IT WITH(NOLOCK) ON IT.ITEMID=RT.ITEMID  																   " + Environment.NewLine;
            //strqry += "JOIN AX.InventTable IIT WITH(NOLOCK) ON IIT.ITEMID=RT.ITEMID AND ET.STORE=RT.STORE												   " + Environment.NewLine;
            //strqry += " WHERE 1=1  AND RT.RECEIPTID='" + strReceiptId + "' AND RT.STORE='" + storecode + "' AND ET.SALESTYPE=1";

            strqry = "	SELECT ROW_NUMBER() OVER(ORDER BY LINENUM ) SONO,ITEMID,SALERECEIPTID AS  [SALE INVOICE],SALEDATE AS  [SALE DATE],  " + Environment.NewLine;
            strqry += "ITEMDESC AS NAMEALIAS,SALESPERSONCODE,  " + Environment.NewLine;
            strqry += "HSNCODE,UNITID UNIT,PCS,CAST(GROSSWEIGHT AS decimal(18,3)) GROSSWEIGHT,  " + Environment.NewLine;
            strqry += "cast(DIAMONDWT as decimal(18,3)) AS [DIAMOND CT],CAST(NETWEIGHT AS decimal(18,3)) NETWEIGHT, METALVALUE AS [METAL VAL],VA AS MAKINGVALUE,  " + Environment.NewLine;
            strqry += "STONEVALUE AS [STONE VAL],CAST(TAXABLEAMOUNT AS decimal(18,3)) [TAXABLE VALUE],CAST(TAXAMOUNT  AS decimal(18,3)) TAXAMOUNT,  " + Environment.NewLine;
            strqry += "CAST(TAXABLEAMOUNT+TAXAMOUNT AS decimal(18,3)) [TOTAL],  " + Environment.NewLine;
            strqry += "cast(STONEWT as decimal(18,3)) [Stone Wt],cast(STONEGVALUE as decimal(18,3)) [G_STONE VAL]	  " + Environment.NewLine;
            strqry += "FROM ACXINVOICERETURNLINES  " + Environment.NewLine;
            strqry += "WHERE RECEIPTID='" + strReceiptId + "'  " + Environment.NewLine;
            strqry += "AND SALESTYPE=1 ";
            dtsalereturn = GetData(strqry);
            if (dtsalereturn.Rows.Count > 0)
            {
                decimal amount = dtsalereturn.AsEnumerable().Sum(s => s.Field<decimal>("TOTAL"));
                mAmountWords = words(Convert.ToInt32(amount));
            }
            #endregion


            #region"tax table"


            decimal decTaxableValue = 0;
            DataRow drRow;
            for (int ii = 0; ii < dtsalereturn.Rows.Count; ii++)
            {
                decTaxableValue += Convert.ToDecimal(dtsalereturn.Rows[ii]["TAXABLE VALUE"].ToString());

            }
            drRow = dttax.NewRow();
            drRow["Description"] = "Taxable Value";
            drRow["TaxAmount"] = decTaxableValue.ToString("########0.00");
            dttax.Rows.Add(drRow);

            //strqry = "  select RG.TAXCOMPONENT Description,RG.TAXPERCENTAGE,SUM(ABS(CAST(RG.TAXAMOUNT AS decimal(18,3)))) TaxAmount from Ax.RetailTransactionSalesTrans RT WITH(NOLOCK)  " + Environment.NewLine;
            //strqry += "  JOIN Ax.RetailTransactionTaxTransGTE RG WITH(NOLOCK) ON  RG.TRANSACTIONID=RT.TRANSACTIONID AND RG.SALELINENUM=RT.LINENUM AND RG.STOREID=RT.STORE" + Environment.NewLine;
            //strqry += "   JOIN Ext.AcxRetailTransactionSalesTrans ET WITH(NOLOCK)  ON ET.TRANSACTIONID=RT.TRANSACTIONID AND ET.LINENUM=RT.LINENUM  AND ET.STORE=RT.STORE " + Environment.NewLine;
            //strqry += " WHERE 1=1   AND RT.RECEIPTID='" + strReceiptId + "' AND RT.STORE='" + storecode + "'  AND ET.SALESTYPE=1 	group by RG.TAXCOMPONENT,RG.TAXPERCENTAGE	 having SUM(ABS(CAST(RG.TAXAMOUNT AS decimal(18,3))))>0";

            strqry = "SELECT TAXCOMPONENT AS Description,TAXPERCENTAGE,SUM(ABS(CAST(A.TAXAMOUNT AS decimal(18,3)))) TaxAmount " + Environment.NewLine;
            strqry += " FROM ACXINVOICETAX A " + Environment.NewLine;
            strqry += " JOIN ACXINVOICETABLE B " + Environment.NewLine;
            strqry += "  ON A.TRANSACTIONID=B.TRANSACTIONID " + Environment.NewLine;
            strqry += "  WHERE B.RECEIPTID='" + strReceiptId + "' " + Environment.NewLine;
            strqry += " AND B.STORE='" + storecode + "' " + Environment.NewLine;
            strqry += " group by TAXCOMPONENT,TAXPERCENTAGE	 having SUM(ABS(CAST(A.TAXAMOUNT AS decimal(18,3))))>0";

            Cmd = new SqlCommand(strqry);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            Cmd.Connection = conn;
            da = new SqlDataAdapter(Cmd);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 120;
            DataTable dtGTE = new DataTable();
            da.Fill(dtGTE);

            for (int intGTE = 0; intGTE < dtGTE.Rows.Count; intGTE++)
            {
                drRow = dttax.NewRow();
                drRow["Description"] = dtGTE.Rows[intGTE]["Description"].ToString() + " - " + Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXPERCENTAGE"].ToString()).ToString("0.000") + "%";
                drRow["TaxAmount"] = Convert.ToDecimal(dtGTE.Rows[intGTE]["TaxAmount"].ToString()).ToString("####0.00");
                dttax.Rows.Add(drRow);
            }




            #endregion


            #region"Store info"

            //strqry = "SELECT DISTINCT   rprct.ORIGINID AS RECID,  rprst.STORENUMBER,  lpa.ADDRESS," + Environment.NewLine;
            //strqry += "lpa.STREETNUMBER, lpa.STREET,  lpa.DISTRICTNAME,  lpa.CITY,lpa.COUNTY," + Environment.NewLine;
            //strqry += "lpa.ZIPCODE,  lpa.STATE FROM [ax].RETAILPUBRETAILSTORETABLE rprst	 " + Environment.NewLine;
            //strqry += "INNER JOIN [ax].RETAILPUBRETAILCHANNELTABLE rprct ON rprct.ORIGINID = rprst.STOREORIGINID " + Environment.NewLine;
            //strqry += "INNER JOIN [ax].DIRPARTYLOCATION dpl ON dpl.PARTY = rprct.OMOPERATINGUNITID AND dpl.ISPOSTALADDRESS = 1 " + Environment.NewLine;
            //strqry += " INNER JOIN [ax].LOGISTICSPOSTALADDRESS lpa ON lpa.LOCATION = dpl.LOCATION where rprst.STORENUMBER ='" + storecode + "'";

            strqry = "SELECT DISTINCT 0  AS RECID,STORENUMBER,(ADDRESS1+ADDRESS2+ADDRESS3) AS ADDRESS, ''  STREETNUMBER,'' AS STREET,'' AS DISTRICTNAME,'' AS CITY,'' AS COUNTY," + Environment.NewLine;
            strqry += "'' AS ZIPCODE,  STATECODE " + Environment.NewLine;
            strqry += "FROM ACXSTOREDETAILS " + Environment.NewLine;
            strqry += "WHERE STORENUMBER='" + storecode + "'";
            dtstoreinfo = GetData(strqry);
            #endregion

            //if(dtsalereturn.Rows.Count>0)
            //{
            //    if (Convert.ToInt16(dtsalereturn.Rows[0]["LINKBARCODE"]) == 1 || Convert.ToInt16(dtsalereturn.Rows[0]["LINKBARCODE"]) == 5)
            //    { 
            //       rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SaleReturnDiamond.rdl");
            //    }
            //    if (Convert.ToInt16(dtsalereturn.Rows[0]["LINKBARCODE"]) == 2 )
            //    {
            //        rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SaleReturnGold.rdl");
            //    }
            //}

            rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SaleReturn.rdl");
            rptViewr.LocalReport.DataSources.Clear();
            var Rds = new ReportDataSource();
            Rds.Name = "Header";
            Rds.Value = dtcompany;
            rptViewr.LocalReport.DataSources.Add(Rds);

            var RdsHeader = new ReportDataSource();
            RdsHeader.Name = "ldata";
            RdsHeader.Value = dtsalereturn;
            rptViewr.LocalReport.DataSources.Add(RdsHeader);

            var RdsCustDetail = new ReportDataSource();
            RdsCustDetail.Name = "CustomerDetail";
            RdsCustDetail.Value = dtcust;
            rptViewr.LocalReport.DataSources.Add(RdsCustDetail);

            var Taxtotal = new ReportDataSource();
            Taxtotal.Name = "ITotal";
            Taxtotal.Value = dttax;
            rptViewr.LocalReport.DataSources.Add(Taxtotal);


            var RDsStoreInfo = new ReportDataSource();
            RDsStoreInfo.Name = "StoreInfo";
            RDsStoreInfo.Value = dtstoreinfo;
            rptViewr.LocalReport.DataSources.Add(RDsStoreInfo);

            ReportParameter amtpara = new ReportParameter();
            amtpara.Name = "AmtInWords";
            amtpara.Values.Add(mAmountWords);
            rptViewr.LocalReport.SetParameters(amtpara);


            rptViewr.LocalReport.DisplayName = "SaleReturn";
            if (pdfflag == "0")
            {
                rptViewr.LocalReport.Refresh();
            }
            else if (pdfflag == "1")
            {
                DownloadPdf(strReceiptId + "_" + ReportType);
            }
            else if (pdfflag == "2")
            {
                string savePath = Server.MapPath("DownloadInvoice\\SaleReturn.pdf");
                SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
            }

        }

        public void PurchaseInvoice(string strReceiptId, string storecode, string pdfflag, string ReportType)
        {
            DataTable dtcompany = new DataTable();
            DataTable dtcust = new DataTable();
            DataTable dtsalereturn = new DataTable();
            DataTable dsfooter = new DataTable();
            string strqry = "";
            string custaccount = "";
            string mAmountWords;
            DataTable dtstoreinfo = new DataTable();

            #region"companyDetails"
            //strqry = "select  SI.GSTIN,SI.COMPANYNAME,SI.ADDRESS1+'-'+SI.ADDRESS2+'-'+SI.ADDRESS3 ADDRESS,SI.ADDRESS1,SI.ADDRESS2,SI.ADDRESS3,SI.POSINVOICEHEADER1,SI.POSINVOICEHEADER2,SI.POSINVOICEHEADER3,RT.RECEIPTID,convert(varchar, RT.TRANSDATE , 103)  TRANSDATE " + Environment.NewLine;
            //strqry += ",si.CIN,si.PAN,RT.CUSTACCOUNT,rtt.ISDELIVERYATSTORE,SI.STATECODE,ISNULL(B.NAME ,'') STATENAME from ext.AcxStoreInfo SI WITH(NOLOCK) JOIN Ax.RETAILTRANSACTIONSALESTRANS   RT WITH(NOLOCK)  ON RT.STORE=SI.STORENUMBER" + Environment.NewLine;
            //strqry += "JOIN Ext.AcxRetailTransactionSalesTrans ET WITH(NOLOCK)  ON ET.TRANSACTIONID=RT.TRANSACTIONID AND ET.LINENUM=RT.LINENUM AND ET.STORE=RT.STORE" + Environment.NewLine;
            //strqry += "LEFT JOIN ext.ACXRETAILTRANSACTIONTABLE rtt  on rt.TRANSACTIONID = rtt.TRANSACTIONID and rt.STORE = rtt.STORE LEFT JOIN ax.LOGISTICSADDRESSSTATE B on b.STATEID = SI.STATECODE" + Environment.NewLine;
            //strqry += "where RT.RECEIPTID='" + strReceiptId + "' AND RT.STORE='" + storecode + "' and ET.SALESTYPE=2";

            strqry = "    SELECT GSTIN,COMPANYNAME,ADDRESS1+'-'+ADDRESS2+'-'+ADDRESS3 AS ADDRESS ,ADDRESS1,ADDRESS2,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3," + Environment.NewLine;
            strqry += "  RECEIPTID,convert(varchar, TRANSDATE , 103) AS TRANSDATE,SI.CIN AS CIN,SI.PAN, CUSTACCOUNT,ISDELIVERYATSTORE,SI.STATECODE,SI.STATENAME  " + Environment.NewLine;
            strqry += "  FROM ACXSTOREDETAILS SI " + Environment.NewLine;
            strqry += "  JOIN ACXINVOICETABLE ET " + Environment.NewLine;
            strqry += "  ON SI.STORENUMBER=ET.STORE " + Environment.NewLine;
            strqry += "  WHERE ET.RECEIPTID='" + strReceiptId + "' AND ET.STORE='" + storecode + "' ";


            dtcompany = GetData(strqry);
            if (dtcompany.Rows.Count > 0)
            {
                custaccount = dtcompany.Rows[0]["CUSTACCOUNT"].ToString();
            }
            else
            {
                Response.Redirect("ErrorPage.aspx?Error=" + "No Company  Details Found" + "", false);

                return;
            }
            #endregion


            #region"Recipient Details"
            //string custqry1 = "Select A.ACCOUNTNUM,b.CUSTNAME NAME,replace(replace(COALEsCE(B.ADDRESS,''),char(10),' '),char(13),' ') as       " + Environment.NewLine;
            //custqry1 += "CustAddress, COALEsCE(B.CITY,'') as CustCity , COALESCE(B.COUNTRYREGIONID, '') as CustCountRegion,            " + Environment.NewLine;
            //custqry1 += "COALESCE(B.DISTRICTNAME, '') as CustDistrictName ,  COALESCE(LT.STATECODE_IN, '') as CustStateCode, COALESCE(B.STREET, '') as CustStreet            " + Environment.NewLine;
            //custqry1 += ", COALESCE(B.ZIPCODE, '') as CustZipCode , COALESCE(B.PANNUMBER, '') as PANNumber  " + Environment.NewLine;
            //custqry1 += ", '' as RegistrationNumber  ,b.PHONE,b.ZIPCODE  " + Environment.NewLine;
            //custqry1 += "--, COALESCE(g.REGISTRATIONNUMBER,'') as RegistrationNumber   " + Environment.NewLine;
            //custqry1 += ", COALESCE(B.STATENAME, '') as StateName   " + Environment.NewLine;
            //custqry1 += " From ax.CUSTTABLE a INNER JOIN ext.ACXCUSTOMERDETAILSVIEW b on a.ACCOUNTNUM = b.CUSTACCOUNT   " + Environment.NewLine;
            //custqry1 += "--LEFT JOIN ax.TAXINFORMATION_IN f on f.REGISTRATIONLOCATION = c.LOCATION   " + Environment.NewLine;
            //custqry1 += "--LEFT JOIN ax.TAXREGISTRATIONNUMBERS_IN g on g.RECID=f.GSTIN   " + Environment.NewLine;
            ///*ADDING THIS JOIN FOR GETTING THE STATECODE 25-12-2020*/
            //custqry1 += "LEFT JOIN [ext].[ACXLOGISTICSADDRESSSTATE] LT ON LT.STATEID=B.STATE  AND LT.COUNTRYREGIONID =B.COUNTRYREGIONID " + Environment.NewLine;
            ///*END*/
            //custqry1 += "where ACCOUNTNUM  = '" + custaccount + "' ";

            string custqry1 = "SELECT CUSTACCOUNT AS ACCOUNTNUM,NAME, " + Environment.NewLine;
            custqry1 += "replace(replace(COALEsCE(ADDRESS,''),char(10),' '),char(13),' ') as CustAddress,  " + Environment.NewLine;
            custqry1 += "COALEsCE(CITY,'') as CustCity,COALESCE(COUNTRYREGIONID, '') as CustCountRegion,  " + Environment.NewLine;
            custqry1 += "COALESCE(DISTRICTNAME, '') as CustDistrictName,  " + Environment.NewLine;
            custqry1 += "COALESCE(STATECODE, '') as CustStateCode, COALESCE(STREET, '') as CustStreet,  " + Environment.NewLine;
            custqry1 += "COALESCE(ZIPCODE, '') as CustZipCode,COALESCE(PANNUMBER, '') as PANNumber, " + Environment.NewLine;
            custqry1 += "COALESCE(GSTIN ,'')  as RegistrationNumber,COALESCE(STATENAME, '') as StateName,PHONE,'' CIN,ZIPCODE,CITY  " + Environment.NewLine;
            custqry1 += "FROM DBO.ACXCUSTDETAILS" + Environment.NewLine;
            custqry1 += "WHERE CUSTACCOUNT=  '" + custaccount + "' ";
            dtcust = GetData(custqry1);


            if (dtcompany.Rows.Count <= 0)
            {

                Response.Redirect("ErrorPage.aspx?Error=" + "No Customer Details Found" + "", false);
                return;
            }

            #endregion

            #region"purchase invoice Line"

            //strqry = "select ROW_NUMBER() OVER(ORDER BY RT.LINENUM ) SONO,IT.ITEMID,RT.UNIT,	(CAST(ET.NETWEIGHT AS decimal(18,3))-CAST(ET.NETWTDEDUCTION AS decimal(18,3))) DEDUCTIONPERC,CAST(ET.NETWTDEDUCTION AS decimal(18,3)) [GOLD NET_WT]," + Environment.NewLine;
            //strqry += "TB.RECEIPTID [SALE INVOICE],TB.TRANSDATE [SALE DATE],IIT.NAMEALIAS,ET.SALESPERSONCODE,CAST(ET.FINALMAKINGV AS decimal(18,3)) [VA]," + Environment.NewLine;
            //strqry += "HSNCODE_IN HSNCODE,ET.PCS,CAST(ET.GROSSWEIGHT AS decimal(18,3)) GROSSWEIGHT,CAST(ET.FINALDIAMONDWT+ET.FINALSTONECWT+ et.FINALSTONEGWT AS decimal(18,3)) " + Environment.NewLine;
            //strqry += "[DIAMOND CT],CAST(ET.NETWEIGHT AS decimal(18,3)) NETWEIGHT,CAST(ET.FINALMETALV AS decimal(18,3)) [METAL VAL],CAST(et.FINALMAKINGV AS DECIMAL(18,3)) MAKINGVALUE, " + Environment.NewLine;
            //strqry += "CAST(ET.FINALDIAMONDV+ET.FINALSTONECV+et.FINALSTONEGV AS decimal(18,3))[STONE VAL] ,CAST(RT.NETAMOUNT AS decimal(18,3)) [TAXABLE VALUE],CAST(RT.TAXAMOUNT  AS decimal(18,3)) TAXAMOUNT,  " + Environment.NewLine;
            //strqry += "CAST(RT.NETAMOUNT+RT.TAXAMOUNT AS decimal(18,3)) [TOTAL] " + Environment.NewLine;
            //strqry += "from Ax.RetailTransactionSalesTrans RT WITH(NOLOCK) JOIN Ext.AcxRetailTransactionSalesTrans ET WITH(NOLOCK)  ON ET.TRANSACTIONID=RT.TRANSACTIONID AND ET.LINENUM=RT.LINENUM 		   " + Environment.NewLine;
            //strqry += " AND ET.STORE=RT.STORE																											   " + Environment.NewLine;
            //strqry += "JOIN AX.RETAILTRANSACTIONTABLE  TB  WITH(NOLOCK) ON TB.TRANSACTIONID=RT.TRANSACTIONID  								   " + Environment.NewLine;
            //strqry += "JOIN ext.acxInventTable IT WITH(NOLOCK) ON IT.ITEMID=RT.ITEMID  																   " + Environment.NewLine;
            //strqry += "JOIN AX.InventTable IIT WITH(NOLOCK) ON IIT.ITEMID=RT.ITEMID AND ET.STORE=RT.STORE												   " + Environment.NewLine;
            //strqry += " WHERE 1=1  AND RT.RECEIPTID='" + strReceiptId + "' AND RT.STORE='" + storecode + "' AND ET.SALESTYPE=2";

            strqry = "	SELECT ROW_NUMBER() OVER(ORDER BY LINENUM ) SONO,ITEMID,SALERECEIPTID AS  [SALE INVOICE],SALEDATE AS  [SALE DATE],  " + Environment.NewLine;
            strqry += "ITEMDESC AS NAMEALIAS,SALESPERSONCODE,  " + Environment.NewLine;
            strqry += "HSNCODE,UNITID UNIT, CAST(NETWTDEDUCTION AS decimal(18,3)) AS [GOLD NET_WT],PCS,CAST(GROSSWEIGHT AS decimal(18,3)) GROSSWEIGHT,(CAST(NETWEIGHT AS decimal(18,3))-CAST(NETWTDEDUCTION AS decimal(18,3))) DEDUCTIONPERC,  " + Environment.NewLine;
            strqry += "DIAMONDWT AS [DIAMOND CT],CAST(NETWEIGHT AS decimal(18,3)) NETWEIGHT, METALVALUE AS [METAL VAL],VA ,VA AS MAKINGVALUE ,  " + Environment.NewLine;
            strqry += "STONEVALUE AS [STONE VAL],CAST(TAXABLEAMOUNT AS decimal(18,3)) [TAXABLE VALUE],CAST(TAXAMOUNT  AS decimal(18,3)) TAXAMOUNT,  " + Environment.NewLine;
            strqry += "CAST(TAXABLEAMOUNT+TAXAMOUNT AS decimal(18,3)) [TOTAL],  " + Environment.NewLine;
            strqry += "cast(STONEWT as decimal(18,3)) [Stone Wt],cast(STONEGVALUE as decimal(18,3)) [G_STONE VAL]	  " + Environment.NewLine;
            strqry += "FROM ACXINVOICERETURNLINES  " + Environment.NewLine;
            strqry += "WHERE RECEIPTID='" + strReceiptId + "'  " + Environment.NewLine;
            strqry += "AND SALESTYPE=2 ";
            dtsalereturn = GetData(strqry);
            #endregion



            #region"Footor data"

            //strqry = "select CAST(RT.NETAMOUNT+RT.TAXAMOUNT AS decimal(18,3)) [TOTAL INVOICE],ET.SALESPERSONCODE," + Environment.NewLine;
            //strqry += "isnull(abs(cast(rtt.AMOUNTTENDERED as decimal(18,3))),0) [Cash Paid],CAST(RT.TAXAMOUNT  AS decimal(18,3)) [GST-COMPANY]" + Environment.NewLine;
            //strqry += "from  Ax.RETAILTRANSACTIONSALESTRANS  RT WITH(NOLOCK) JOIN Ext.AcxRetailTransactionSalesTrans ET WITH(NOLOCK)  ON " + Environment.NewLine;
            //strqry += "ET.TRANSACTIONID=RT.TRANSACTIONID AND ET.LINENUM=RT.LINENUM AND ET.STORE=RT.STORE					   " + Environment.NewLine;
            //strqry += "LEFT OUTER JOIN Ax.RetailtransactionPaymentTrans rtt  on rt.TRANSACTIONID = rtt.TRANSACTIONID and rt.STORE = rtt.STORE						   " + Environment.NewLine;
            //strqry += " WHERE 1=1  AND RT.RECEIPTID='" + strReceiptId + "' AND RT.STORE='" + storecode + "' AND ET.SALESTYPE=2";

            strqry = "SELECT TOTAL AS [TOTAL INVOICE],A.SALESPERSONCODE AS SALESPERSONCOE, " + Environment.NewLine;
            strqry += "CAST(A.TAXAMOUNT  AS decimal(18,3)) [GST-COMPANY] " + Environment.NewLine;
            strqry += "FROM ACXINVOICERETURNLINES A " + Environment.NewLine;
            strqry += "JOIN ACXINVOICETABLE B " + Environment.NewLine;
            strqry += "ON A.TRANSACTIONID=B.TRANSACTIONID  " + Environment.NewLine;
            strqry += "WHERE A.RECEIPTID='" + strReceiptId + "' AND B.STORE ='" + storecode + "' AND A.SALESTYPE=2";
            dsfooter = GetData(strqry);


            #endregion

            decimal amount = dsfooter.AsEnumerable().Sum(s => s.Field<decimal>("TOTAL INVOICE"));
            decimal goldrate = 0;
            if (dtsalereturn.Rows.Count > 0)
            {
                decimal tgoldvalue = dtsalereturn.AsEnumerable().Sum(s => s.Field<decimal>("METAL VAL"));
                decimal tNetWeight = dtsalereturn.AsEnumerable().Sum(s => s.Field<decimal>("GOLD NET_WT"));
                if (tNetWeight != 0 && tgoldvalue != 0)
                {
                    goldrate = (tgoldvalue / tNetWeight);

                }
            }
            mAmountWords = words(Convert.ToInt32(amount));
            rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/PurchaseInvoice.rdl");

            rptViewr.LocalReport.DataSources.Clear();
            var Rds = new ReportDataSource();
            Rds.Name = "Header";
            Rds.Value = dtcompany;
            rptViewr.LocalReport.DataSources.Add(Rds);

            var RdsHeader = new ReportDataSource();
            RdsHeader.Name = "ldata";
            RdsHeader.Value = dtsalereturn;
            rptViewr.LocalReport.DataSources.Add(RdsHeader);

            var RdsCustDetail = new ReportDataSource();
            RdsCustDetail.Name = "CustomerDetail";
            RdsCustDetail.Value = dtcust;
            rptViewr.LocalReport.DataSources.Add(RdsCustDetail);

            var Taxtotal = new ReportDataSource();
            Taxtotal.Name = "ITotal";
            Taxtotal.Value = dsfooter;
            rptViewr.LocalReport.DataSources.Add(Taxtotal);



            ReportParameter amountword = new ReportParameter();
            amountword.Name = "AmtInWords";
            amountword.Values.Add(mAmountWords);
            rptViewr.LocalReport.SetParameters(amountword);

            ReportParameter grate = new ReportParameter();
            grate.Name = "GoldRate";
            grate.Values.Add(goldrate.ToString("###0.00"));
            rptViewr.LocalReport.SetParameters(grate);

            rptViewr.LocalReport.DisplayName = "PurchaseInvoice";
            if (pdfflag == "0")
            {
                rptViewr.LocalReport.Refresh();
            }
            else if (pdfflag == "1")
            {
                DownloadPdf(strReceiptId + "_" + ReportType);
            }
            else if (pdfflag == "2")
            {
                string savePath = Server.MapPath("DownloadInvoice\\PurchaseInvoice.pdf");
                SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
            }
        }

        public void OfferCreditMemo(string strReceiptId, string storecode, string pdfflag, string ReportType)
        {
            DataTable dtcompany = new DataTable();
            DataTable dtcust = new DataTable();
            DataTable dtsalereturn = new DataTable();
            DataTable dttax = new DataTable();
            string strqry = "";
            string custaccount = "";
            string mAmountWords = "";

            #region"companyDetails"
            //strqry = "select  SI.GSTIN,SI.COMPANYNAME,SI.ADDRESS1+' '+SI.ADDRESS2+' '+SI.ADDRESS3 ADDRESS,SI.PAN,SI.CIN,       " + Environment.NewLine;
            //strqry += "SI.STATECODE,ISNULL(B.NAME ,'') STATENAME,ISNULL(CA.CUSTOMERCODE,'') CUSTOMERCODE,CA.RECEIPTID ,CA.STORE	" + Environment.NewLine;
            //strqry += ",CA.TRANSDATE from ext.AcxStoreInfo SI  																	" + Environment.NewLine;
            //strqry += "LEFT JOIN ax.LOGISTICSADDRESSSTATE B on b.STATEID = SI.STATECODE											" + Environment.NewLine;
            //strqry += "LEFT JOIN EXT.ACXCUSTOMERADVANCE  CA ON CA.STORE=SI.STORENUMBER											" + Environment.NewLine;
            //strqry += "WHERE  CA.ADVANCETYPE IN (1,9) AND CA.RECEIPTID='" + strReceiptId + "' AND CA.STORE='" + storecode + "'					" + Environment.NewLine;




            strqry = "    SELECT GSTIN,COMPANYNAME,ADDRESS1+'-'+ADDRESS2+'-'+ADDRESS3 AS ADDRESS ,ADDRESS1,ADDRESS2,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3," + Environment.NewLine;
            strqry += "  RECEIPTID,convert(varchar, TRANSDATE , 103) AS TRANSDATE,SI.CIN AS CIN,SI.PAN, ISNULL(ET.CUSTACCOUNT,'') CUSTOMERCODE,ISNULL(ET.STORE,'') STORE,SI.STATECODE,SI.STATENAME  " + Environment.NewLine;
            strqry += "  FROM ACXSTOREDETAILS SI " + Environment.NewLine;
            strqry += "  JOIN ACXCUSTOMERADVANCE ET " + Environment.NewLine;
            strqry += "  ON SI.STORENUMBER=ET.STORE " + Environment.NewLine;
            strqry += "  WHERE ET.RECEIPTID='" + strReceiptId + "' AND ET.STORE='" + storecode + "' ";

            dtcompany = GetData(strqry);
            if (dtcompany.Rows.Count > 0)
            {
                custaccount = dtcompany.Rows[0]["CUSTOMERCODE"].ToString();
            }
            else
            {
                Response.Redirect("ErrorPage.aspx?Error=" + "No Company  Details Found" + "", false);

                return;
            }
            #endregion


            #region"Recipient Details"
            //string custqry1 = "Select A.ACCOUNTNUM,b.CUSTNAME NAME,replace(replace(COALEsCE(B.ADDRESS,''),char(10),' '),char(13),' ') as       " + Environment.NewLine;
            //custqry1 += "CustAddress, COALEsCE(B.CITY,'') as CustCity , COALESCE(B.COUNTRYREGIONID, '') as CustCountRegion,            " + Environment.NewLine;
            //custqry1 += "COALESCE(B.DISTRICTNAME, '') as CustDistrictName ,  COALESCE(LT.STATECODE_IN, '') as CustStateCode, COALESCE(B.STREET, '') as CustStreet            " + Environment.NewLine;
            //custqry1 += ", COALESCE(B.ZIPCODE, '') as CustZipCode , COALESCE(B.PANNUMBER, '') as PANNumber  " + Environment.NewLine;
            //custqry1 += ", '' as RegistrationNumber ,b.PHONE  " + Environment.NewLine;
            //custqry1 += "--, COALESCE(g.REGISTRATIONNUMBER,'') as RegistrationNumber   " + Environment.NewLine;
            //custqry1 += ", COALESCE(B.STATENAME, '') as StateName   " + Environment.NewLine;
            //custqry1 += " From ax.CUSTTABLE a INNER JOIN ext.ACXCUSTOMERDETAILSVIEW b on a.ACCOUNTNUM = b.CUSTACCOUNT   " + Environment.NewLine;
            //custqry1 += "--LEFT JOIN ax.TAXINFORMATION_IN f on f.REGISTRATIONLOCATION = c.LOCATION   " + Environment.NewLine;
            //custqry1 += "--LEFT JOIN ax.TAXREGISTRATIONNUMBERS_IN g on g.RECID=f.GSTIN   " + Environment.NewLine;
            ///*ADDING THIS JOIN FOR GETTING THE STATECODE 25-12-2020*/
            //custqry1 += "LEFT JOIN [ext].[ACXLOGISTICSADDRESSSTATE] LT ON LT.STATEID=B.STATE  AND LT.COUNTRYREGIONID =B.COUNTRYREGIONID " + Environment.NewLine;
            ///*END*/

            //custqry1 += "where ACCOUNTNUM  = '" + custaccount + "' ";


            string custqry1 = "SELECT CUSTACCOUNT AS ACCOUNTNUM,NAME, " + Environment.NewLine;
            custqry1 += "replace(replace(COALEsCE(ADDRESS,''),char(10),' '),char(13),' ') as CustAddress,  " + Environment.NewLine;
            custqry1 += "COALEsCE(CITY,'') as CustCity,COALESCE(COUNTRYREGIONID, '') as CustCountRegion,  " + Environment.NewLine;
            custqry1 += "COALESCE(DISTRICTNAME, '') as CustDistrictName,  " + Environment.NewLine;
            custqry1 += "COALESCE(STATECODE, '') as CustStateCode, COALESCE(STREET, '') as CustStreet,  " + Environment.NewLine;
            custqry1 += "COALESCE(ZIPCODE, '') as CustZipCode,COALESCE(PANNUMBER, '') as PANNumber, " + Environment.NewLine;
            custqry1 += "COALESCE(GSTIN ,'')  as RegistrationNumber,COALESCE(STATENAME, '') as StateName,PHONE,'' CIN,ZIPCODE,CITY  " + Environment.NewLine;
            custqry1 += "FROM DBO.ACXCUSTDETAILS" + Environment.NewLine;
            custqry1 += "WHERE CUSTACCOUNT=  '" + custaccount + "' ";
            dtcust = GetData(custqry1);
            #endregion

            #region" Line"

            //strqry = "select ROW_NUMBER() OVER(ORDER BY RT.NAME) SONO ,RT.NAME,CAST(CA.AMOUNT AS DECIMAL(18,3)) AMOUNT from EXT.ACXCUSTOMERADVANCE CA" + Environment.NewLine;
            //strqry += "join ext.ACXRETAILPERIODICDISCOUNT RT ON RT.OFFERID=CA.CUSTOMERORDERNO " + Environment.NewLine;
            //strqry += "WHERE  CA.ADVANCETYPE IN (1,9) AND CA.RECEIPTID='" + strReceiptId + "' AND CA.STORE='" + storecode + "'	";

            strqry = "select ROW_NUMBER() OVER(ORDER BY OFFERNAME) SONO ,OFFERNAME AS NAME,CAST(CA.AMOUNT AS DECIMAL(18,3)) AMOUNT " + Environment.NewLine;
            strqry += "frOM ACXCUSTOMERADVANCE CA " + Environment.NewLine;
            strqry += "WHERE  CA.ADVANCETYPE IN (1,9) AND CA.RECEIPTID='" + strReceiptId + "' AND CA.STORE='" + storecode + "'	";

            dtsalereturn = GetData(strqry);
            if (dtsalereturn.Rows.Count > 0)
            {
                decimal amount = dtsalereturn.AsEnumerable().Sum(s => s.Field<decimal>("AMOUNT"));
                mAmountWords = words(Convert.ToInt32(amount));
            }
            #endregion

            #region"footer data"

            //strqry = " select CA.RECEIPTID,CA.NOOFDAYS,CA.ADVANCEEXPIRYDATE,ISNULL(ST.SALESPERSONCODE,'') SALESPERSONCODE from EXT.ACXCUSTOMERADVANCE CA  " + Environment.NewLine;
            //strqry += " LEFT JOIN EXT.ACXRETAILTRANSACTIONSALESTRANS ST ON ST.TRANSACTIONID=CA.TRANSACTIONID" + Environment.NewLine;
            //strqry += " WHERE  CA.ADVANCETYPE IN (1,9) AND CA.RECEIPTID='" + strReceiptId + "' AND CA.STORE='" + storecode + "'		";

            strqry = "select CA.RECEIPTID,CA.NOOFDAYS,CA.ADVANCEEXPIRYDATE,ISNULL(ST.SALESPERSONCODE,'') SALESPERSONCODE " + Environment.NewLine;
            strqry += "from ACXCUSTOMERADVANCE CA  " + Environment.NewLine;
            strqry += " LEFT JOIN  ACXINVOICELINES ST ON ST.TRANSACTIONID=CA.TRANSACTIONID AND ST.LINENUM=CA.LINENUM" + Environment.NewLine;
            strqry += " WHERE  CA.ADVANCETYPE IN (1,9) AND CA.RECEIPTID='" + strReceiptId + "' AND CA.STORE='" + storecode + "'	 AND ST.ISPRINT=1";
            dttax = GetData(strqry);
            #endregion

            rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/OfferCreditMemo.rdl");
            rptViewr.LocalReport.DataSources.Clear();
            var Rds = new ReportDataSource();
            Rds.Name = "Header";
            Rds.Value = dtcompany;
            rptViewr.LocalReport.DataSources.Add(Rds);

            var RdsHeader = new ReportDataSource();
            RdsHeader.Name = "ldata";
            RdsHeader.Value = dtsalereturn;
            rptViewr.LocalReport.DataSources.Add(RdsHeader);

            var RdsCustDetail = new ReportDataSource();
            RdsCustDetail.Name = "CustomerDetail";
            RdsCustDetail.Value = dtcust;
            rptViewr.LocalReport.DataSources.Add(RdsCustDetail);

            var Taxtotal = new ReportDataSource();
            Taxtotal.Name = "ITotal";
            Taxtotal.Value = dttax;
            rptViewr.LocalReport.DataSources.Add(Taxtotal);

            ReportParameter amtpara = new ReportParameter();
            amtpara.Name = "AmtInWords";
            amtpara.Values.Add(mAmountWords);
            rptViewr.LocalReport.SetParameters(amtpara);

            rptViewr.LocalReport.DisplayName = "OfferCreditMemo";
            if (pdfflag == "0")
            {
                rptViewr.LocalReport.Refresh();
            }
            else if (pdfflag == "1")
            {
                DownloadPdf(strReceiptId + "_" + ReportType);
            }
            else if (pdfflag == "2")
            {
                string savePath = Server.MapPath("DownloadInvoice\\OfferCreditMemo.pdf");
                SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
            }

        }

        public void ShowSalesOrder(string strReceiptId, string Storeid, string pdfflag, string ReportType)
        {
            try
            {
                Int32 totalAmount = 0;
                decimal decTaxableValue = 0;
                string strInvoiceCaption = string.Empty;
                string strInvoiceHeader = string.Empty;
                string mAmountWords;
                string transid = string.Empty;
                string storeno = "";
                string strSalesPersonCode = string.Empty;
                string strRateFixed = string.Empty;
                string stqry = "";
                string strExpiryDate = string.Empty;
                string strTransDate = "";
                string strGoldRate = string.Empty;
                string strOfferId = string.Empty;
                decimal amtword = 0;
                //                            string[] Tender  = new string[5];                        
                DataTable dtTotal;
                DataRow drRow;
                decimal scrapexchangeamt = 0;
                decimal salereturnamt = 0;

                dtTotal = new DataTable();
                dtTotal.Columns.Add("Description");
                dtTotal.Columns.Add("TaxAmount");
                string ConnectionString = ConfigurationManager.AppSettings["POSDBCON"].ToString();
                SqlConnection conn = new SqlConnection(ConnectionString);

                SqlCommand Cmd;
                SqlDataAdapter da;


                if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " select  rtt.RECEIPTID, rtt.TRANSACTIONID, rtt.CHANNEL, rtt.CUSTACCOUNT, rtt.TRANSDATE, STAFF, rtt.Store, rtt.GrossAmount " +
                //            " from acxinvoicetable rtt " +
                //            " where rtt.ReceiptID='" + strReceiptId + "' AND ENTRYSTATUS=0 ";
                stqry = "SELECT RECEIPTID,TRANSACTIONID,CHANNEL,CUSTACCOUNT,TRANSDATE,STAFF,Store,GrossAmount" + Environment.NewLine;
                stqry += "FROM dbo.ACXINVOICETABLE" + Environment.NewLine;
                stqry += "WHERE RECEIPTID='" + strReceiptId + "' ";
                
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtRTT = new DataTable();
                da.Fill(dtRTT);
                if (dtRTT.Rows.Count > 0)
                {
                    storeno = dtRTT.Rows[0]["STORE"].ToString();
                    totalAmount = Convert.ToInt32(Convert.ToDecimal(dtRTT.Rows[0]["GrossAmount"].ToString()) * -1);

                    ///Detail
                    strTransDate = Convert.ToDateTime(dtRTT.Rows[0]["TRANSDATE"].ToString()).ToString("dd-MMM-yyyy");
                    transid = dtRTT.Rows[0]["TRANSACTIONID"].ToString();

                }
                else
                {

                    Response.Redirect("ErrorPage.aspx?Error=" + "No Sale Order Details Found" + "", false);
                    conn.Close();
                    return;
                }
                if (conn.State != ConnectionState.Open) { conn.Open(); }

//                stqry = @"Select a.ITEMID, 'Gold/Diamond/Precious/Platinum/Silver/Other Products ' as ItemDescription, a.RECEIPTID,
//                    a.TRANSDATE, a.BALANCEAMOUNT AMOUNT, RATEG92, CASE WHEN IsRateFixed=0 THEN 'No' ELSE 'Yes' END IsRateFixed, ADVANCEEXPIRYDATE, b.SALESGROUP as SALESPERSONCODE, a.OFFERID, OFFERLINERECID
//                    ,COALESCE(c.NAME,'') as OFFERNAME, NoOfDays,B.TRANSACTIONID,
//                    (SELECT TOP 1 ADVANCEEXPIRYDAYS FROM EXT.ACXJEWELLERYPARAMETER) ADVEXPIRY,a.ADVANCETYPE
//                    From ext.ACXCUSTOMERADVANCE a
//                    INNER JOIN ax.RETAILTRANSACTIONSALESTRANS b on a.RECEIPTID = b.RECEIPTID and a.ITEMID = b.ITEMID
//                    AND A.LINENUM=B.LINENUM
//                    LEFT JOIN ext.ACXRETAILPERIODICDISCOUNT c on a.OFFERID = c.OFFERID
//                     where a.RECEIPTID='" + strReceiptId + "'";
                stqry = @"SELECT ITEMID,ITEMDESC AS ItemDescription,RECEIPTID,TRANSDATE,AMOUNT,RATEG92,CASE WHEN IsRateFixed=0 THEN 'No' ELSE 'Yes' END AS IsRateFixed,
		                    ADVANCEEXPIRYDATE,SALESPERSONCODE,OFFERID, OFFERLINERECID,OFFERNAME,NOOFDAYS AS NoOfDays,TRANSACTIONID,0 AS ADVEXPIRY,ADVANCETYPE
		                    FROM DBO.ACXCUSTOMERADVANCE
                            WHERE RECEIPTID='" + strReceiptId + "'";

                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtCustAdv = new DataTable();
                da.Fill(dtCustAdv);
                if (dtCustAdv.Rows.Count > 0)
                {
                    strSalesPersonCode = dtCustAdv.Rows[0]["SALESPERSONCODE"].ToString();
                    strSalesPersonCode = strSalesPersonCode == "" ? "-" : strSalesPersonCode;
                    strExpiryDate = Convert.ToDateTime(dtCustAdv.Rows[0]["ADVANCEEXPIRYDATE"].ToString()).ToString("dd-MMM-yyyy");

                    amtword = dtCustAdv.AsEnumerable().Sum(s => s.Field<decimal>("AMOUNT"));
                    scrapexchangeamt = dtCustAdv.AsEnumerable().Where(s => s.Field<int>("ADVANCETYPE") == 8).Sum(s => s.Field<decimal>("AMOUNT"));
                    salereturnamt = dtCustAdv.AsEnumerable().Where(s => s.Field<int>("ADVANCETYPE") == 7).Sum(s => s.Field<decimal>("AMOUNT"));
                    mAmountWords = words(Convert.ToInt32(amtword));

                    strGoldRate = Convert.ToDecimal(dtCustAdv.Rows[0]["RATEG92"].ToString()).ToString("###0.00");
                }
                else
                {

                    Response.Redirect("ErrorPage.aspx?Error=" + "Sale Order Details Not Found" + "", false);
                    conn.Close();
                    return;
                }

                conn.Close();


                //string custqry1 = "Select A.ACCOUNTNUM,b.CUSTNAME NAME,replace(replace(COALEsCE(B.ADDRESS,''),char(10),' '),char(13),' ') as       " + Environment.NewLine;
                //custqry1 += "CustAddress, COALEsCE(B.CITY,'') as CustCity , COALESCE(B.COUNTRYREGIONID, '') as CustCountRegion,            " + Environment.NewLine;
                //custqry1 += "COALESCE(B.DISTRICTNAME, '') as CustDistrictName , COALESCE(B.STATE, '') as CustStateCode, COALESCE(B.STREET, '') as CustStreet            " + Environment.NewLine;
                //custqry1 += ", COALESCE(B.ZIPCODE, '') as CustZipCode , COALESCE(B.PANNUMBER, '') as PANNumber  " + Environment.NewLine;
                //custqry1 += ", '' as RegistrationNumber   " + Environment.NewLine;
                //custqry1 += "--, COALESCE(g.REGISTRATIONNUMBER,'') as RegistrationNumber   " + Environment.NewLine;
                //custqry1 += ", COALESCE(B.STATENAME, '') as StateName   " + Environment.NewLine;
                //custqry1 += " From ax.CUSTTABLE a INNER JOIN ext.ACXCUSTOMERDETAILSVIEW b on a.ACCOUNTNUM = b.CUSTACCOUNT   " + Environment.NewLine;
                //custqry1 += "--LEFT JOIN ax.TAXINFORMATION_IN f on f.REGISTRATIONLOCATION = c.LOCATION   " + Environment.NewLine;
                //custqry1 += "--LEFT JOIN ax.TAXREGISTRATIONNUMBERS_IN g on g.RECID=f.GSTIN   " + Environment.NewLine;
                //custqry1 += "where ACCOUNTNUM  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
                string custqry1 = "SELECT CUSTACCOUNT AS ACCOUNTNUM,NAME, " + Environment.NewLine;
                custqry1 += "replace(replace(COALEsCE(ADDRESS,''),char(10),' '),char(13),' ') as CustAddress,  " + Environment.NewLine;
                custqry1 += "COALEsCE(CITY,'') as CustCity,COALESCE(COUNTRYREGIONID, '') as CustCountRegion,  " + Environment.NewLine;
                custqry1 += "COALESCE(DISTRICTNAME, '') as CustDistrictName,  " + Environment.NewLine;
                custqry1 += "COALESCE(STATECODE, '') as CustStateCode, COALESCE(STREET, '') as CustStreet,  " + Environment.NewLine;
                custqry1 += "COALESCE(ZIPCODE, '') as CustZipCode,COALESCE(PANNUMBER, '') as PANNumber, " + Environment.NewLine;
                custqry1 += "COALESCE(GSTIN ,'')  as RegistrationNumber,COALESCE(STATENAME, '') as StateName,PHONE  " + Environment.NewLine;
                custqry1 += "FROM DBO.ACXCUSTDETAILS" + Environment.NewLine;
                custqry1 += "WHERE CUSTACCOUNT= '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
                DataTable dtcust;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (Cmd = new SqlCommand(custqry1, conn))
                {
                    using (SqlDataReader reader1 = Cmd.ExecuteReader())
                    {
                        using (DataTable transtable1 = new DataTable())
                        {
                            transtable1.Load(reader1);
                            dtcust = transtable1.Copy();
                        }
                    }
                }

                // for customer Phone
                string strPhone = string.Empty;
                //custqry1 = "Select ACCOUNTNUM,b.NAME, d.Locator "
                //                + " From ax.CUSTTABLE a "
                //                + "INNER JOIN ax.DIRPARTYTABLE b on a.PARTY = b.RECID "
                //                + "INNER JOIN ax.DIRPARTYLOCATION c on a.PARTY = c.PARTY "
                //                + "INNER JOIN ax.LOGISTICSELECTRONICADDRESS d on c.Location = d.Location and d.Type=1 "
                //                + "where ACCOUNTNUM  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
                //DataTable dtPhone;
                //if (conn.State == ConnectionState.Closed)
                //    conn.Open();

                //using (Cmd = new SqlCommand(custqry1, conn))
                //{
                //    using (SqlDataReader reader1 = Cmd.ExecuteReader())
                //    {
                //        using (DataTable transtable1 = new DataTable())
                //        {
                //            transtable1.Load(reader1);
                //            dtPhone = transtable1.Copy();
                //        }
                //    }
                //}
                //if (dtPhone.Rows.Count > 0)
                //{
                //    strPhone = dtPhone.Rows[0]["Locator"].ToString();
                //}
                strPhone = dtcust.Rows.Count > 0 ? dtcust.Rows[0]["PHONE"].ToString() : "";

                ////Credit Card Payment
                //if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                //      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) >=20 and Convert(int,TENDERTYPE) <= 40 AND TRANSACTIONSTATUS =0 AND VOIDSTATUS=0 ";
                //Cmd = new SqlCommand(stqry);
                //Cmd.Connection = conn;
                //da = new SqlDataAdapter(Cmd);
                //da.SelectCommand.CommandType = CommandType.Text;
                //da.SelectCommand.CommandTimeout = 120;
                //DataTable dtTend = new DataTable();
                //int intTenderCount = 0;
                //da.Fill(dtTend);
                //decTaxableValue = 0;
                //for (intTenderCount = 0; intTenderCount < dtTend.Rows.Count; intTenderCount++)
                //{
                //    if (Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                //    {
                //        decTaxableValue += Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString());
                //    }
                //}
                //if (decTaxableValue != 0)
                //{
                //    drRow = dtTotal.NewRow();
                //    drRow["Description"] = "Credit Cards/Debit Cards/UPI/Credit Customer ";
                //    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                //    dtTotal.Rows.Add(drRow);
                //}
                //////Cheques Payment and DD
                //if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                //      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =42 AND TRANSACTIONSTATUS =0 AND VOIDSTATUS=0";
                //Cmd = new SqlCommand(stqry);
                //Cmd.Connection = conn;
                //da = new SqlDataAdapter(Cmd);
                //da.SelectCommand.CommandType = CommandType.Text;
                //da.SelectCommand.CommandTimeout = 120;
                //dtTend.Clear();
                //dtTend = new DataTable();
                //intTenderCount = 0;
                //da.Fill(dtTend);
                //decTaxableValue = 0;
                //for (intTenderCount = 0; intTenderCount < dtTend.Rows.Count; intTenderCount++)
                //{
                //    if (Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                //    {
                //        decTaxableValue += Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString());
                //    }
                //}
                //if (decTaxableValue != 0)
                //{
                //    drRow = dtTotal.NewRow();
                //    drRow["Description"] = "Cheque/DD ";
                //    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                //    dtTotal.Rows.Add(drRow);
                //}
                //////Cheques RTGS and NEFT
                //if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                //      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =43 AND TRANSACTIONSTATUS =0 AND VOIDSTATUS=0";
                //Cmd = new SqlCommand(stqry);
                //Cmd.Connection = conn;
                //da = new SqlDataAdapter(Cmd);
                //da.SelectCommand.CommandType = CommandType.Text;
                //da.SelectCommand.CommandTimeout = 120;
                //dtTend.Clear();
                //dtTend = new DataTable();
                //intTenderCount = 0;
                //da.Fill(dtTend);
                //decTaxableValue = 0;
                //for (intTenderCount = 0; intTenderCount < dtTend.Rows.Count; intTenderCount++)
                //{
                //    if (Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                //    {
                //        decTaxableValue += Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString());
                //    }
                //}
                //if (decTaxableValue != 0)
                //{
                //    drRow = dtTotal.NewRow();
                //    drRow["Description"] = "RTGS/NEFT ";
                //    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                //    dtTotal.Rows.Add(drRow);
                //}


                //////Credit Payment
                //if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                //      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =50 AND TRANSACTIONSTATUS =0 AND VOIDSTATUS=0";
                //Cmd = new SqlCommand(stqry);
                //Cmd.Connection = conn;
                //da = new SqlDataAdapter(Cmd);
                //da.SelectCommand.CommandType = CommandType.Text;
                //da.SelectCommand.CommandTimeout = 120;
                //dtTend.Clear();
                //dtTend = new DataTable();
                //intTenderCount = 0;
                //da.Fill(dtTend);
                //decTaxableValue = 0;
                //for (intTenderCount = 0; intTenderCount < dtTend.Rows.Count; intTenderCount++)
                //{
                //    if (Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                //    {
                //        decTaxableValue += Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString());
                //    }
                //}
                //if (decTaxableValue != 0)
                //{
                //    drRow = dtTotal.NewRow();
                //    drRow["Description"] = "Credit Allowed ";
                //    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                //    dtTotal.Rows.Add(drRow);
                //}

                //////Cash Payment
                //if (conn.State != ConnectionState.Open) { conn.Open(); }
                //stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                //      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =1 AND TRANSACTIONSTATUS =0 AND VOIDSTATUS=0 ";
                //Cmd = new SqlCommand(stqry);
                //Cmd.Connection = conn;
                //da = new SqlDataAdapter(Cmd);
                //da.SelectCommand.CommandType = CommandType.Text;
                //da.SelectCommand.CommandTimeout = 120;
                //dtTend.Clear();
                //dtTend = new DataTable();
                //intTenderCount = 0;
                //da.Fill(dtTend);
                //decTaxableValue = 0;
                //for (intTenderCount = 0; intTenderCount < dtTend.Rows.Count; intTenderCount++)
                //{
                //    if (Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString()).ToString("0.00") != "0.00")
                //    {
                //        decTaxableValue += Convert.ToDecimal(dtTend.Rows[intTenderCount]["Amount"].ToString());
                //    }
                //}
                //if (decTaxableValue != 0)
                //{
                //    drRow = dtTotal.NewRow();
                //    drRow["Description"] = "Cash Received ";
                //    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                //    dtTotal.Rows.Add(drRow);
                //}


                //if (scrapexchangeamt != 0)
                //{
                //    drRow = dtTotal.NewRow();
                //    drRow["Description"] = "Scrap Exchange";
                //    drRow["TaxAmount"] = scrapexchangeamt.ToString("#######0.00");
                //    dtTotal.Rows.Add(drRow);
                //}
                //if (salereturnamt != 0)
                //{
                //    drRow = dtTotal.NewRow();
                //    drRow["Description"] = "Sale Return";
                //    drRow["TaxAmount"] = salereturnamt.ToString("#######0.00");
                //    dtTotal.Rows.Add(drRow);
                //}



                //conn.Close();

                stqry = "select 'Credit Cards/Debit Cards/UPI/Credit Customer' AS Description, COALESCE(SUM(TENDERAMOUNT),0) AS TENDERAMOUNT  " + Environment.NewLine;
                stqry += " from dbo.acxINVOICEPAYMENT  " + Environment.NewLine;
                stqry += "  WHERE  TRANSACTIONID='" + transid + "'  AND ((Convert(int,TENDERTYPE)>=20) AND (Convert(int,TENDERTYPE)<=40 )) " + Environment.NewLine;
                stqry += " HAVING COALESCE(SUM(TENDERAMOUNT),0)>0  " + Environment.NewLine;
                stqry += " UNION ALL  " + Environment.NewLine;
                stqry += "  select TENDERNAME AS Description,  TENDERAMOUNT  " + Environment.NewLine;
                stqry += " from dbo.acxINVOICEPAYMENT  " + Environment.NewLine;
                stqry += " WHERE    TRANSACTIONID='" + transid + "'  AND  ((Convert(int,TENDERTYPE)<20) OR (Convert(int,TENDERTYPE)>40)) ";

                Cmd = new SqlCommand(stqry);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtTender = new DataTable();
                da.Fill(dtTender);

                foreach (DataRow row in dtTender.Rows)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = row["Description"].ToString();
                    drRow["TaxAmount"] = Convert.ToDecimal(row["TENDERAMOUNT"].ToString()).ToString("####0.00");
                    dtTotal.Rows.Add(drRow);

                }
                if (scrapexchangeamt != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Scrap Exchange";
                    drRow["TaxAmount"] = scrapexchangeamt.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                if (salereturnamt != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Sale Return";
                    drRow["TaxAmount"] = salereturnamt.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                //stqry = " select GSTIN,COMPANYNAME,ADDRESS1,ADDRESS3,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3 "
                //                + ",PAN, CIN, STATECODE,b.NAME "
                //                + "from ext.ACXSTOREINFO a "
                //                + "LEFT JOIN ax.LOGISTICSADDRESSSTATE b on b.STATEID = a.STATECODE "
                //                + "where a.STORENUMBER='" + storeno + "' ";
                stqry = "select GSTIN,COMPANYNAME,ADDRESS1,ADDRESS2,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3 "
                              + ", PAN, CIN, STATECODE, STATENAME AS NAME "
                              + "from ACXSTOREDETAILS a "
                              + " where a.STORENUMBER ='" + storeno + "' ";
                Cmd = new SqlCommand(stqry);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                DataTable dtHeader = new DataTable();
                da.Fill(dtHeader);

                ReportDataSource RDS = new ReportDataSource("Dset", dtRTT);
                rptViewr.LocalReport.DataSources.Add(RDS);
                rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/SaleOrderUpdate.rdl");




                rptViewr.LocalReport.DataSources.Clear();
                var Rds = new ReportDataSource();
                Rds.Name = "Header";
                Rds.Value = dtHeader;
                rptViewr.LocalReport.DataSources.Add(Rds);

                var RdsHeader = new ReportDataSource();
                RdsHeader.Name = "IData";
                RdsHeader.Value = dtCustAdv;
                rptViewr.LocalReport.DataSources.Add(RdsHeader);

                var RdsCustDetail = new ReportDataSource();
                RdsCustDetail.Name = "CustomerDetail";
                RdsCustDetail.Value = dtcust;
                rptViewr.LocalReport.DataSources.Add(RdsCustDetail);



                var RdsTotal = new ReportDataSource();
                RdsTotal.Name = "ITotal";
                RdsTotal.Value = dtTotal;
                rptViewr.LocalReport.DataSources.Add(RdsTotal);


                ReportParameter AmountInWords = new ReportParameter();
                AmountInWords.Name = "AmountInWords";
                AmountInWords.Values.Add(mAmountWords);
                rptViewr.LocalReport.SetParameters(AmountInWords);

                ReportParameter GoldRate = new ReportParameter();
                GoldRate.Name = "GoldRate";
                GoldRate.Values.Add(strGoldRate);
                rptViewr.LocalReport.SetParameters(GoldRate);

                ReportParameter DueDate = new ReportParameter();
                DueDate.Name = "DueDate";
                DueDate.Values.Add(strExpiryDate);
                rptViewr.LocalReport.SetParameters(DueDate);

                ReportParameter SalesPersonCode = new ReportParameter();
                SalesPersonCode.Name = "SalesPersonCode";
                SalesPersonCode.Values.Add(strSalesPersonCode);
                rptViewr.LocalReport.SetParameters(SalesPersonCode);


                ReportParameter InvoiceDate = new ReportParameter();
                InvoiceDate.Name = "InvoiceDate";
                InvoiceDate.Values.Add(strTransDate);
                rptViewr.LocalReport.SetParameters(InvoiceDate);

                ReportParameter parameter = new ReportParameter();
                parameter.Name = "CustPhone";
                parameter.Values.Add(strPhone);
                rptViewr.LocalReport.SetParameters(parameter);

                rptViewr.LocalReport.DisplayName = "SaleOrderUpdated";
                if (pdfflag == "0")
                {
                    rptViewr.LocalReport.Refresh();
                }
                else if (pdfflag == "1")
                {
                    DownloadPdf(strReceiptId + "_" + ReportType);
                }
                else if (pdfflag == "2")
                {
                    string savePath = Server.MapPath("DownloadInvoice\\SaleOrderUpdated.pdf");
                    SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
                }

                conn.Close();

            }
            catch (Exception ex)
            {
                Response.Redirect("ErrorPage.aspx?Error=" + ex.Message.ToString());
                return;
            }

        }


        public void FinalEstimate(string strCustAccount, string EstimateGroup, string Transdate, string Storeid, string pdfflag, string ReportType)
        {
            try
            {
                DataTable dtline = new DataTable();
                DataTable dtTotal = new DataTable();
                DataTable dtDiscount = new DataTable();
                DataTable dtestimate = new DataTable();
                string strqry = "";
                #region"Getting Lines Details"

                strqry = "SELECT Q.ESTIMATEGROUP,Q.ESTIMATENO,Q.TAGNO,Q.ADDMAKINGCODE,Q.ITEMID," + Environment.NewLine;
                strqry += " SUM(PCS) AS PCS,CAST(SUM(GROSSWEIGHT) AS DECIMAL(18,3)) GROSSWEIGHT,CAST(SUM(NETWEIGHT) AS DECIMAL(18,3)) NETWEIGHT," + Environment.NewLine;
                strqry += " CAST(SUM(STONEGWEIGHT) AS DECIMAL(18,3)) STONEGWEIGHT,CAST(SUM(DIAMONDWEIGHT) AS DECIMAL(18,3)) DIAMONDWEIGHT,CAST(SUM(OTHERSTONEWEIGHT) AS DECIMAL(18,3)) OTHERSTONEWEIGHT" + Environment.NewLine;
                strqry += " FROM ( SELECT A.ESTIMATEGROUP,A.ESTIMATENO,A.TAGNO,A.ADDMAKINGCODE,A.ITEMID,A.PCS ,A.GROSSWEIGHT ,																		 " + Environment.NewLine;
                strqry += " A.NETWEIGHT ,0 AS STONEGWEIGHT, 0.000 AS DIAMONDWEIGHT, 0 AS OTHERSTONEWEIGHT FROM ACXESTIMATETABLE A																			 " + Environment.NewLine;
                strqry += "WHERE A.STORE= '" + Storeid + "' AND A.REPORTTYPE=22 AND A.CUSTACCOUNT='" + strCustAccount + "' AND																								 " + Environment.NewLine;
                strqry += " A.ESTIMATEGROUP='" + EstimateGroup + "' AND A.TRANSDATE='" + Transdate + "' AND A.STATUS=2 																											 " + Environment.NewLine;
                strqry += " UNION ALL																																									 " + Environment.NewLine;
                strqry += " SELECT A.ESTIMATEGROUP,A.ESTIMATENO,A.TAGNO,A.ADDMAKINGCODE,A.ITEMID,0 AS PCS ,0 AS GROSSWEIGHT ,																			 " + Environment.NewLine;
                strqry += " 0 AS NETWEIGHT ,CASE WHEN B.ITEMCATEGORY=3 THEN B.QUANTITY ELSE 0 END AS STONEGWEIGHT,																					 " + Environment.NewLine;
                strqry += " CASE WHEN B.ITEMCATEGORY=1 THEN B.QUANTITY ELSE 0.000 END AS DIAMONDWEIGHT,CASE WHEN B.ITEMCATEGORY=2 THEN B.QUANTITY ELSE 0 END AS OTHERSTONEWEIGHT							 " + Environment.NewLine;
                strqry += " FROM ACXESTIMATETABLE A JOIN ACXESTIMATELINES B ON A.TRANSACTIONID=B.TRANSACTIONID and a.REPORTTYPE=b.REPORTTYPE															 " + Environment.NewLine;
                strqry += " WHERE A.STORE= '" + Storeid + "' AND A.REPORTTYPE=22 AND A.CUSTACCOUNT='" + strCustAccount + "' AND																					 " + Environment.NewLine;
                strqry += " A.ESTIMATEGROUP='" + EstimateGroup + "' AND A.TRANSDATE='" + Transdate + "' AND A.STATUS=2 																												 " + Environment.NewLine;
                strqry += " ) Q GROUP BY Q.ESTIMATEGROUP,Q.ESTIMATENO,Q.TAGNO,Q.ADDMAKINGCODE,Q.ITEMID																								 " + Environment.NewLine;

                dtline = GetData(strqry);

                #endregion

                #region"Total Details"

                strqry = "SELECT CAST(COALESCE(SUM(METALVALUE),0)  AS decimal(18,2)) METALVALUE,CAST(COALESCE(SUM(DIAMONDVALUE),0) AS decimal(18,2)) DIAMONDVALUE," + Environment.NewLine;
                strqry += " CAST(COALESCE(SUM(STONECVALUE),0)AS decimal(18,2)) STONECVALUE,CAST(COALESCE(SUM(STONEGVALUE),0)AS decimal(18,2)) STONEGVALUE,		  " + Environment.NewLine;
                strqry += " CAST(COALESCE(SUM(MAKINGVALUE),0)AS decimal(18,2)) MAKINGVALUE,CAST(COALESCE(SUM(GROSSAMOUNT),0)AS decimal(18,2)) GROSSAMOUNT,		  " + Environment.NewLine;
                strqry += " CAST(COALESCE(SUM(DISCAMOUNT),0)AS decimal(18,2))  DISCAMOUNT,CAST(COALESCE(SUM(TAXABLEAMOUNT),0) AS decimal(18,2)) TAXABLEAMOUNT,	  " + Environment.NewLine;
                strqry += " CAST(COALESCE(SUM(TAXAMOUNT),0) AS decimal(18,2)) TAXAMOUNT,CAST(COALESCE(SUM(AMOUNT),0) AS decimal(18,2)) AMOUNT					  " + Environment.NewLine;
                strqry += " FROM(																																  " + Environment.NewLine;
                strqry += " SELECT A.ESTIMATENO,A.METALVALUE,A.DIAMONDVALUE,0 AS STONECVALUE,0 AS STONEGVALUE,A.STONEVALUE,A.MAKINGVALUE,A.GROSSAMOUNT,			  " + Environment.NewLine;
                strqry += " a.DISCAMOUNT,A.GROSSAMOUNT-A.DISCAMOUNT AS TAXABLEAMOUNT,																			  " + Environment.NewLine;
                strqry += " A.TAXAMOUNT,A.AMOUNT																												  " + Environment.NewLine;
                strqry += " FROM ACXESTIMATETABLE  A																											  " + Environment.NewLine;
                strqry += "   where  A.STORE= '" + Storeid + "' AND A.CUSTACCOUNT='" + strCustAccount + "' AND													  " + Environment.NewLine;
                strqry += " A.ESTIMATEGROUP='" + EstimateGroup + "' AND A.TRANSDATE='" + Transdate + "' AND A.STATUS=2 																		  " + Environment.NewLine;
                strqry += " AND A.REPORTTYPE=22																													  " + Environment.NewLine;
                strqry += " UNION ALL																															  " + Environment.NewLine;
                strqry += " SELECT A.ESTIMATENO,0 AS METALVALUE,0 AS DIAMONDVALUE,CASE WHEN B.ITEMCATEGORY=2 THEN B.AMOUNT ELSE 0 END  AS STONECVALUE,			  " + Environment.NewLine;
                strqry += " CASE WHEN B.ITEMCATEGORY=3 THEN B.AMOUNT ELSE 0 END AS STONEGVALUE,A.STONEVALUE,0 MAKINGVALUE,0 GROSSAMOUNT,						  " + Environment.NewLine;
                strqry += " 0 as DISCAMOUNT,0 AS TAXABLEAMOUNT,																									  " + Environment.NewLine;
                strqry += " 0 as TAXAMOUNT,0 as AMOUNT																											  " + Environment.NewLine;
                strqry += " FROM ACXESTIMATETABLE A JOIN ACXESTIMATELINES B																						  " + Environment.NewLine;
                strqry += " ON A.TRANSACTIONID=B.TRANSACTIONID AND A.REPORTTYPE=B.REPORTTYPE																	  " + Environment.NewLine;
                strqry += "  where  A.STORE= '" + Storeid + "' AND A.REPORTTYPE=22 AND A.CUSTACCOUNT='" + strCustAccount + "' AND													  " + Environment.NewLine;
                strqry += " A.ESTIMATEGROUP='" + EstimateGroup + "' AND A.TRANSDATE='" + Transdate + "' AND A.STATUS=2 																		  " + Environment.NewLine;
                strqry += " ) Q " + Environment.NewLine;

                dtTotal = GetData(strqry);

                #endregion
                #region"Estimate Details"
                strqry = "select * from  [ACXESTIMATEGROUP] A" + Environment.NewLine;
                strqry += "WHERE A.STORE= '" + Storeid + "' AND A.REPORTTYPE=22  AND CUSTACCOUNT='" + strCustAccount + "' AND ESTIMATEGROUP=" + EstimateGroup + " AND TRANSDATE='" + Transdate + "'" + Environment.NewLine;
                dtestimate = GetData(strqry);
                string DISCDETAILS = "0";

                if (dtestimate.Rows.Count > 0)
                {
                    DISCDETAILS = dtestimate.Rows[0]["DISCOUNTDETAIL"].ToString();

                }

                #endregion
                #region"Discount Details"
                if (DISCDETAILS == "1")
                {
                    strqry = "SELECT CAST(SUM(STONECAMOUNTBD) AS decimal(18,2)) STONECAMOUNTBD,CAST(SUM(STONECDISCAMT) AS decimal(18,2)) STONECDISCAMT," + Environment.NewLine;
                    strqry += "CAST(SUM(STONECAMOUNTAD)AS decimal(18,2))  STONECAMOUNTAD,CAST(SUM(STONEGAMOUNTBD) AS decimal(18,2)) STONEGAMOUNTBD," + Environment.NewLine;
                    strqry += "CAST(SUM(STONEGDISCAMT) AS decimal(18,2)) STONEGDISCAMT,CAST(SUM(STONEGAMOUNTAD) AS decimal(18,2)) STONEGAMOUNTAD," + Environment.NewLine;
                    strqry += "CAST(SUM(DIAMONDAMOUNTBD)AS decimal(18,2)) DIAMONDAMOUNTBD,CAST(SUM(DIAMONDDISCAMT) AS decimal(18,2)) DIAMONDDISCAMT," + Environment.NewLine;
                    strqry += "CAST(SUM(DIAMONDAMOUNTAD)AS decimal(18,2)) DIAMONDAMOUNTAD,CAST(SUM(MAKINGVALUEBD) AS decimal(18,2)) MAKINGVALUEBD," + Environment.NewLine;
                    strqry += "CAST(SUM(MAKINGDISCAMT) AS decimal(18,2)) MAKINGDISCAMT,CAST(SUM(MAKINGVALUEAD) AS decimal(18,2)) MAKINGVALUEAD," + Environment.NewLine;
                    strqry += "CAST(SUM(WASTAGEBD)AS decimal(18,2)) WASTAGEBD,CAST(SUM(WASTAGEDISCAMT) AS decimal(18,2)) WASTAGEDISCAMT," + Environment.NewLine;
                    strqry += "CAST(SUM(WASTAGEAD)AS decimal(18,2)) WASTAGEAD,CAST(SUM(CERTCHARGEBD) AS decimal(18,2)) CERTCHARGEBD,                         " + Environment.NewLine;
                    strqry += "CAST(SUM(CERTCHARGEDISCAMT) AS decimal(18,2)) CERTCHARGEDISCAMT,CAST(SUM(CERTCHARGEAD) AS decimal(18,2)) CERTCHARGEAD,		  " + Environment.NewLine;
                    strqry += "CAST(SUM(HMCHARGEBD) AS decimal(18,2)) HMCHARGEBD,CAST(SUM(HMCHARGEDISCAMT) AS decimal(18,2)) HMCHARGEDISCAMT,				  " + Environment.NewLine;
                    strqry += "CAST(SUM(HMCHARGEAD) AS decimal(18,2)) HMCHARGEAD,CAST(SUM(SSCHARGEBD) AS decimal(18,2)) SSCHARGEBD,						  " + Environment.NewLine;
                    strqry += "CAST(SUM(SSCHARGEDISCAMT) AS decimal(18,2)) SSCHARGEDISCAMT,CAST(SUM(SSCHARGEAD) AS decimal(18,2)) SSCHARGEAD				  " + Environment.NewLine;
                    strqry += "FROM(																														  " + Environment.NewLine;
                    strqry += "    SELECT A.ITEMID AS FGITEM,B.ITEMID, 																					  " + Environment.NewLine;
                    strqry += "    CASE WHEN B.ITEMCATEGORY=2 THEN B.GROSSAMOUNT ELSE 0 END AS STONECAMOUNTBD,												  " + Environment.NewLine;
                    strqry += "    CASE WHEN B.ITEMCATEGORY=2 THEN B.DISCAMOUNT ELSE 0 END AS STONECDISCAMT,												  " + Environment.NewLine;
                    strqry += "    CASE WHEN B.ITEMCATEGORY=2 THEN (B.AMOUNT) ELSE 0 END AS STONECAMOUNTAD,												  " + Environment.NewLine;
                    strqry += "    CASE WHEN B.ITEMCATEGORY=3 THEN B.GROSSAMOUNT ELSE 0 END AS STONEGAMOUNTBD,												  " + Environment.NewLine;
                    strqry += "    CASE WHEN B.ITEMCATEGORY=3 THEN B.DISCAMOUNT ELSE 0 END AS STONEGDISCAMT,												  " + Environment.NewLine;
                    strqry += "    CASE WHEN B.ITEMCATEGORY=3 THEN (B.AMOUNT) ELSE 0 END AS STONEGAMOUNTAD,												  " + Environment.NewLine;
                    strqry += "    CASE WHEN B.ITEMCATEGORY=1 THEN B.GROSSAMOUNT ELSE 0 END AS DIAMONDAMOUNTBD,											  " + Environment.NewLine;
                    strqry += "    CASE WHEN B.ITEMCATEGORY=1 THEN B.DISCAMOUNT ELSE 0 END AS DIAMONDDISCAMT,												  " + Environment.NewLine;
                    strqry += "    CASE WHEN B.ITEMCATEGORY=1 THEN (B.AMOUNT) ELSE 0 END AS DIAMONDAMOUNTAD,												  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE IN(0,1,2,3,4,12)) THEN B.GROSSAMOUNT ELSE 0 END AS MAKINGVALUEBD,			  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE IN(0,1,2,3,4,12)) THEN B.DISCAMOUNT ELSE 0 END AS MAKINGDISCAMT,			  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE IN(0,1,2,3,4,12)) THEN (B.AMOUNT) ELSE 0 END AS MAKINGVALUEAD,				  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE IN(14)) THEN B.GROSSAMOUNT ELSE 0 END AS WASTAGEBD,						  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE IN(14)) THEN B.DISCAMOUNT ELSE 0 END AS WASTAGEDISCAMT,					  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE IN(14)) THEN (B.AMOUNT) ELSE 0 END AS WASTAGEAD,							  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE=7) THEN B.GROSSAMOUNT ELSE 0 END AS CERTCHARGEBD,							  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE=7) THEN B.DISCAMOUNT ELSE 0 END AS CERTCHARGEDISCAMT,						  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE=7) THEN (B.AMOUNT) ELSE 0 END AS CERTCHARGEAD,								  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE=8) THEN B.GROSSAMOUNT ELSE 0 END AS HMCHARGEBD,							  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE=8) THEN B.DISCAMOUNT ELSE 0 END AS HMCHARGEDISCAMT,						  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE=8) THEN (B.AMOUNT) ELSE 0 END AS HMCHARGEAD,								  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE=13) THEN B.GROSSAMOUNT ELSE 0 END AS SSCHARGEBD,							  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE=13) THEN B.DISCAMOUNT ELSE 0 END AS SSCHARGEDISCAMT,						  " + Environment.NewLine;
                    strqry += "    CASE WHEN (B.ITEMCATEGORY=5 AND B.MAKINGTYPE=13) THEN (B.AMOUNT) ELSE 0 END AS SSCHARGEAD								  " + Environment.NewLine;
                    strqry += "    FROM ACXESTIMATETABLE A																								  " + Environment.NewLine;
                    strqry += "    JOIN ACXESTIMATELINES B																								  " + Environment.NewLine;
                    strqry += "    ON A.TRANSACTIONID= B.TRANSACTIONID		and A.REPORTTYPE=B.REPORTTYPE																				  " + Environment.NewLine;
                    strqry += "    WHERE A.STORE= '" + Storeid + "' AND A.REPORTTYPE=22 AND A.CUSTACCOUNT='" + strCustAccount + "' AND A.ESTIMATEGROUP=" + EstimateGroup + " AND A.TRANSDATE='" + Transdate + "' AND A.STATUS=2	 " + Environment.NewLine;
                    strqry += ") Q	";
                    dtDiscount = GetData(strqry);
                }


                rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/FinalEstimate.rdl");
                rptViewr.LocalReport.DataSources.Clear();
                var Rds = new ReportDataSource();
                Rds.Name = "DSlineSet";
                Rds.Value = dtline;
                rptViewr.LocalReport.DataSources.Add(Rds);

                var RdsHeader = new ReportDataSource();
                RdsHeader.Name = "DSTotalSet";
                RdsHeader.Value = dtTotal;
                rptViewr.LocalReport.DataSources.Add(RdsHeader);

                var RdsCustDetail = new ReportDataSource();
                RdsCustDetail.Name = "DSDiscdetails";
                RdsCustDetail.Value = dtDiscount;
                rptViewr.LocalReport.DataSources.Add(RdsCustDetail);

                ReportParameter amtpara = new ReportParameter();
                amtpara.Name = "EstGroupNo";
                amtpara.Values.Add(EstimateGroup);
                rptViewr.LocalReport.SetParameters(amtpara);

                ReportParameter paracustaccount = new ReportParameter();
                paracustaccount.Name = "CustomerCode";
                paracustaccount.Values.Add(strCustAccount);
                rptViewr.LocalReport.SetParameters(paracustaccount);

                ReportParameter paraDISCDETAILS = new ReportParameter();
                paraDISCDETAILS.Name = "ISDISCDETAILS";
                paraDISCDETAILS.Values.Add(DISCDETAILS);
                rptViewr.LocalReport.SetParameters(paraDISCDETAILS);

                rptViewr.LocalReport.DisplayName = "FinalEstimate";
                if (pdfflag == "0")
                {
                    rptViewr.LocalReport.Refresh();
                }
                else if (pdfflag == "1")
                {
                    DownloadforEstimatePdf(strCustAccount + "_" + EstimateGroup + "_" + Transdate + "_" + Storeid + "_" + ReportType);
                }
                else if (pdfflag == "2")
                {
                    string savePath = Server.MapPath("DownloadInvoice\\FinalEstimate.pdf");
                    SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
                }

                #endregion
            }
            catch (Exception ex)
            {
                Response.Redirect("ErrorPage.aspx?Error=" + ex.Message.ToString());
                return;
            }

        }

        public void RoughEstimate(string strCustAccount, string EstimateGroup, string Transdate, string Storeid, string pdfflag, string ReportType)
        {
            try
            {
                DataTable dtline = new DataTable();
                DataTable dtTotal = new DataTable();
                DataTable dtstonedetails = new DataTable();
                DataTable dtvalueadditions = new DataTable();
                DataTable dtMetalRate = new DataTable();
                DataTable dtestimate = new DataTable();
                string strqry = "";
                #region"Getting Lines Details"

                strqry = "SELECT Q.ESTIMATEGROUP,Q.ESTIMATENO,Q.TAGNO,Q.ADDMAKINGCODE,Q.ITEMID," + Environment.NewLine;
                strqry += " SUM(PCS) AS PCS,CAST(SUM(GROSSWEIGHT) AS DECIMAL(18,3)) GROSSWEIGHT,CAST(SUM(NETWEIGHT) AS DECIMAL(18,3)) NETWEIGHT," + Environment.NewLine;
                strqry += " CAST(SUM(STONEGWEIGHT) AS DECIMAL(18,3)) STONEGWEIGHT,CAST(SUM(DIAMONDWEIGHT) AS DECIMAL(18,3)) DIAMONDWEIGHT,CAST(SUM(OTHERSTONEWEIGHT) AS DECIMAL(18,3)) OTHERSTONEWEIGHT" + Environment.NewLine;
                strqry += " FROM ( SELECT A.ESTIMATEGROUP,A.ESTIMATENO,A.TAGNO,A.ADDMAKINGCODE,A.ITEMID,A.PCS ,A.GROSSWEIGHT ,																		 " + Environment.NewLine;
                strqry += " A.NETWEIGHT ,0 AS STONEGWEIGHT, 0 AS DIAMONDWEIGHT, 0 AS OTHERSTONEWEIGHT FROM ACXESTIMATETABLE A																			 " + Environment.NewLine;
                strqry += "WHERE A.STORE= '" + Storeid + "' AND A.REPORTTYPE=21 AND A.CUSTACCOUNT='" + strCustAccount + "' AND																								 " + Environment.NewLine;
                strqry += " A.ESTIMATEGROUP='" + EstimateGroup + "' AND A.TRANSDATE='" + Transdate + "' AND A.STATUS=2 																											 " + Environment.NewLine;
                strqry += " UNION ALL																																									 " + Environment.NewLine;
                strqry += " SELECT A.ESTIMATEGROUP,A.ESTIMATENO,A.TAGNO,A.ADDMAKINGCODE,A.ITEMID,0 AS PCS ,0 AS GROSSWEIGHT ,																			 " + Environment.NewLine;
                strqry += " 0 AS NETWEIGHT ,CASE WHEN B.ITEMCATEGORY=3 THEN B.QUANTITY ELSE 0 END AS STONEGWEIGHT,																					 " + Environment.NewLine;
                strqry += " CASE WHEN B.ITEMCATEGORY=1 THEN B.QUANTITY ELSE 0 END AS DIAMONDWEIGHT,CASE WHEN B.ITEMCATEGORY=2 THEN B.QUANTITY ELSE 0 END AS OTHERSTONEWEIGHT							 " + Environment.NewLine;
                strqry += " FROM ACXESTIMATETABLE A JOIN ACXESTIMATELINES B ON A.TRANSACTIONID=B.TRANSACTIONID and a.REPORTTYPE=b.REPORTTYPE															 " + Environment.NewLine;
                strqry += " WHERE A.STORE= '" + Storeid + "' AND A.REPORTTYPE=21 AND A.CUSTACCOUNT='" + strCustAccount + "' AND																					 " + Environment.NewLine;
                strqry += " A.ESTIMATEGROUP='" + EstimateGroup + "' AND A.TRANSDATE='" + Transdate + "' AND A.STATUS=2 																												 " + Environment.NewLine;
                strqry += " ) Q GROUP BY Q.ESTIMATEGROUP,Q.ESTIMATENO,Q.TAGNO,Q.ADDMAKINGCODE,Q.ITEMID																								 " + Environment.NewLine;

                dtline = GetData(strqry);

                #endregion

                #region"Total Details"

                strqry = "SELECT CAST(COALESCE(SUM(METALVALUE),0)  AS decimal(18,2)) METALVALUE,CAST(COALESCE(SUM(METALVALUE1),0) AS decimal(18,2)) as METALVALUE1,CAST(COALESCE(SUM(METALVALUE2),0) AS decimal(18,2)) as METALVALUE2,CAST(COALESCE(SUM(DIAMONDVALUE),0) AS decimal(18,2)) DIAMONDVALUE," + Environment.NewLine;
                strqry += " CAST(COALESCE(SUM(STONECVALUE),0)AS decimal(18,2)) STONECVALUE,CAST(COALESCE(SUM(STONEGVALUE),0)AS decimal(18,2)) STONEGVALUE,		  " + Environment.NewLine;
                strqry += " CAST(COALESCE(SUM(MAKINGVALUE),0)AS decimal(18,2)) MAKINGVALUE,CAST(COALESCE(SUM(GROSSAMOUNT),0)AS decimal(18,2)) GROSSAMOUNT,		  " + Environment.NewLine;
                strqry += " CAST(COALESCE(SUM(DISCAMOUNT),0)AS decimal(18,2))  DISCAMOUNT,CAST(COALESCE(SUM(TAXABLEAMOUNT),0) AS decimal(18,2)) TAXABLEAMOUNT,	  " + Environment.NewLine;
                strqry += " CAST(COALESCE(SUM(TAXAMOUNT),0) AS decimal(18,2)) TAXAMOUNT,CAST(COALESCE(SUM(AMOUNT),0) AS decimal(18,2)) AMOUNT					  " + Environment.NewLine;
                strqry += " FROM(																																  " + Environment.NewLine;
                strqry += " SELECT A.ESTIMATENO,A.METALVALUE,0 metalvaluE1,0 as mEtALValUe2,A.DIAMONDVALUE,0 AS STONECVALUE,0 AS STONEGVALUE,A.STONEVALUE,A.MAKINGVALUE,A.GROSSAMOUNT,			  " + Environment.NewLine;
                strqry += " a.DISCAMOUNT,A.GROSSAMOUNT-A.DISCAMOUNT AS TAXABLEAMOUNT,																			  " + Environment.NewLine;
                strqry += " A.TAXAMOUNT,A.AMOUNT																												  " + Environment.NewLine;
                strqry += " FROM ACXESTIMATETABLE  A																											  " + Environment.NewLine;
                strqry += "   where  A.STORE= '" + Storeid + "' AND A.REPORTTYPE=21 AND A.CUSTACCOUNT='" + strCustAccount + "' AND													  " + Environment.NewLine;
                strqry += " A.ESTIMATEGROUP='" + EstimateGroup + "' AND A.TRANSDATE='" + Transdate + "' AND A.STATUS=2 																		  " + Environment.NewLine;
                strqry += " UNION ALL																															  " + Environment.NewLine;
                strqry += " SELECT A.ESTIMATENO,0 AS METALVALUE,casE when b.itemcategory=0 and b.primaryitem=1 Then b.amount eLse 0 end as metalvalue1,	casE when b.itemcategory=0 and b.primaryitem=0 Then b.amount eLse 0 end as metalvalue2,  " + Environment.NewLine;
                strqry += "0 AS DIAMONDVALUE,CASE WHEN B.ITEMCATEGORY=2 THEN B.AMOUNT ELSE 0 END  AS STONECVALUE,		 CASE WHEN B.ITEMCATEGORY=3 THEN B.AMOUNT ELSE 0 END AS STONEGVALUE,A.STONEVALUE,0 MAKINGVALUE,0 GROSSAMOUNT,						  " + Environment.NewLine;
                strqry += " 0 as DISCAMOUNT,0 AS TAXABLEAMOUNT,																									  " + Environment.NewLine;
                strqry += " 0 as TAXAMOUNT,0 as AMOUNT																											  " + Environment.NewLine;
                strqry += " FROM ACXESTIMATETABLE A JOIN ACXESTIMATELINES B																						  " + Environment.NewLine;
                strqry += " ON A.TRANSACTIONID=B.TRANSACTIONID AND A.REPORTTYPE=B.REPORTTYPE																	  " + Environment.NewLine;
                strqry += "  where  A.STORE= '" + Storeid + "' AND A.REPORTTYPE=21 AND A.CUSTACCOUNT='" + strCustAccount + "' AND													  " + Environment.NewLine;
                strqry += " A.ESTIMATEGROUP='" + EstimateGroup + "' AND A.TRANSDATE='" + Transdate + "' AND A.STATUS=2 																		  " + Environment.NewLine;
                strqry += " ) Q " + Environment.NewLine;

                dtTotal = GetData(strqry);

                #endregion

                #region"Purity Details"
                strqry = "SELECT ACXITEMTYPE,B.PURITY,cast(B.PRICE as decimal(18,2))PRICE,SUM(B.AMOUNT) AMOUNT FROM ACXESTIMATETABLE A " + Environment.NewLine;
                strqry += "JOIN ACXESTIMATELINES B  ON A.TRANSACTIONID = B.TRANSACTIONID " + Environment.NewLine;
                strqry += "WHERE A.STORE= '" + Storeid + "' AND A.REPORTTYPE=21 AND  B.REPORTTYPE=21 AND A.CUSTACCOUNT='" + strCustAccount + "' AND A.ESTIMATEGROUP=" + EstimateGroup + " AND A.TRANSDATE='" + Transdate + "' AND A.STATUS=2 AND B.ITEMCATEGORY=0" + Environment.NewLine;
                strqry += "GROUP BY ACXITEMTYPE,B.PURITY,B.PRICE" + Environment.NewLine;
                dtMetalRate = GetData(strqry);
                decimal metalRate22K = 0;
                decimal metalRate18K = 0;
                decimal metalRatePlatinum = 0;
                decimal metalRateSilver = 0;
                foreach (DataRow row in dtMetalRate.Rows)
                {
                    if ((row["ACXITEMTYPE"].ToString() == "G") && (Convert.ToDecimal(row["PURITY"].ToString()) == 92))
                    {
                        metalRate22K = Convert.ToDecimal(row["PRICE"].ToString());
                    }
                    else if ((row["ACXITEMTYPE"].ToString() == "G") && (Convert.ToDecimal(row["PURITY"].ToString()) == 75))
                    {
                        metalRate18K = Convert.ToDecimal(row["PRICE"].ToString());
                    }
                    else if ((row["ACXITEMTYPE"].ToString() == "PL") && (Convert.ToDecimal(row["PURITY"].ToString()) == 95))
                    {
                        metalRatePlatinum = Convert.ToDecimal(row["PRICE"].ToString());
                    }
                    else if ((row["ACXITEMTYPE"].ToString() == "S") && (Convert.ToDecimal(row["PURITY"].ToString()) == 92.5m))
                    {
                        metalRateSilver = Convert.ToDecimal(row["PRICE"].ToString());
                    }
                }

                DataTable dtMetalPurity = new DataTable();
                dtMetalPurity.Columns.Add("metalRate22K");
                dtMetalPurity.Columns.Add("metalRate18K");
                dtMetalPurity.Columns.Add("metalRatePlatinum");
                dtMetalPurity.Columns.Add("metalRateSilver");

                DataRow drRow = dtMetalPurity.NewRow();
                drRow["metalRate22K"] = metalRate22K;
                drRow["metalRate18K"] = metalRate18K;
                drRow["metalRatePlatinum"] = metalRatePlatinum;
                drRow["metalRateSilver"] = metalRateSilver;
                dtMetalPurity.Rows.Add(drRow);
                #endregion

                #region"Estimate Details"
                strqry = "select * from  [ACXESTIMATEGROUP] A" + Environment.NewLine;
                strqry += "WHERE A.STORE= '" + Storeid + "' AND A.REPORTTYPE=21  AND CUSTACCOUNT='" + strCustAccount + "' AND ESTIMATEGROUP=" + EstimateGroup + " AND TRANSDATE='" + Transdate + "'" + Environment.NewLine;
                dtestimate = GetData(strqry);
                string STONEDETAIL = "0";
                string VADETAIL = "0";
                string BILLINGDETAIL = "0";
                if (dtestimate.Rows.Count > 0)
                {
                    STONEDETAIL = dtestimate.Rows[0]["STONEDETAIL"].ToString();
                    VADETAIL = dtestimate.Rows[0]["VADETAIL"].ToString();
                    BILLINGDETAIL = dtestimate.Rows[0]["BILLINGDETAIL"].ToString();
                }

                #endregion


                if (dtestimate.Rows.Count > 0)
                {
                    #region"Stone Details"
                    if (Convert.ToInt32(dtestimate.Rows[0]["STONEDETAIL"]) == 1)
                    {
                        strqry = "SELECT A.ITEMID AS FGITEM,B.ITEMID AS STONEITEM,CAST(SUM(B.QUANTITY) AS DECIMAL(18,2) )AS GROSSWEIGHT," + Environment.NewLine;
                        strqry += "CAST(SUM(B.STONEPCS) AS DECIMAL(18,2)) AS STONEPCS,CAST(B.PRICE AS DECIMAL(18,2)) PRICE,CAST(SUM(B.AMOUNT) AS DECIMAL(18,2)) AS AMOUNT" + Environment.NewLine;
                        strqry += "FROM ACXESTIMATETABLE A JOIN ACXESTIMATELINES B ON A.TRANSACTIONID= B.TRANSACTIONID" + Environment.NewLine;
                        strqry += "WHERE A.STORE= '" + Storeid + "' AND A.REPORTTYPE=21 AND  B.REPORTTYPE=21 AND CUSTACCOUNT='" + strCustAccount + "' AND ESTIMATEGROUP=" + EstimateGroup + " AND TRANSDATE='" + Transdate + "' AND STATUS=2 and B.ITEMCATEGORY IN(1,2,3)" + Environment.NewLine;
                        strqry += " GROUP BY A.ITEMID,B.ITEMID,B.PRICE" + Environment.NewLine;
                        dtstonedetails = GetData(strqry);
                    }

                    #endregion


                    #region"value addition  Details"
                    if (Convert.ToInt32(dtestimate.Rows[0]["VADETAIL"]) == 1)
                    {
                        strqry = "SELECT FGITEM,cast(SUM(MAKINGCHARGE)  as decimal(18,2))MAKINGCHARGE," + Environment.NewLine;
                        strqry += "cast(SUM(WASTAGE)as decimal(18,2)) WASTAGE,cast(SUM(SSC)as decimal(18,2)) SSC,                                     " + Environment.NewLine;
                        strqry += "cast(SUM(CERTIFICATION) as decimal(18,2))CERTIFICATION,cast(SUM(HALLMARKING)as decimal(18,2)) HALLMARKING,		   " + Environment.NewLine;
                        strqry += "cast((SUM(MAKINGCHARGE)+SUM(WASTAGE)+SUM(SSC)+SUM(CERTIFICATION)+SUM(HALLMARKING))as decimal(18,2)) AS TOTALVA	   " + Environment.NewLine;
                        strqry += "  FROM(																											   " + Environment.NewLine;
                        strqry += "      SELECT A.ITEMID AS FGITEM,																				   " + Environment.NewLine;
                        strqry += "      CASE WHEN B.MAKINGTYPE IN(0,1,2,3,4,10,11) THEN B.AMOUNT ELSE 0 END AS MAKINGCHARGE,							   " + Environment.NewLine;
                        strqry += "      CASE WHEN B.MAKINGTYPE IN(14,5,6) THEN B.AMOUNT ELSE 0 END AS WASTAGE,										   " + Environment.NewLine;
                        strqry += "      CASE WHEN B.MAKINGTYPE IN(12,13) THEN B.AMOUNT ELSE 0 END  AS SSC,																									   " + Environment.NewLine;
                        strqry += "      CASE WHEN B.MAKINGTYPE =7 THEN B.AMOUNT ELSE 0 END AS CERTIFICATION ,										   " + Environment.NewLine;
                        strqry += "      CASE WHEN B.MAKINGTYPE =8 THEN B.AMOUNT ELSE 0 END AS HALLMARKING                                            " + Environment.NewLine;
                        strqry += "      FROM ACXESTIMATETABLE A																				   " + Environment.NewLine;
                        strqry += "      JOIN ACXESTIMATELINES B																				   " + Environment.NewLine;
                        strqry += "      ON A.TRANSACTIONID= B.TRANSACTIONID																		   " + Environment.NewLine;
                        strqry += " WHERE A.STORE= '" + Storeid + "' AND A.REPORTTYPE=21 AND  B.REPORTTYPE=21 AND CUSTACCOUNT='" + strCustAccount + "' AND ESTIMATEGROUP=" + EstimateGroup + " AND TRANSDATE='" + Transdate + "' " + Environment.NewLine;
                        strqry += "	AND A.STATUS=2AND B.ITEMCATEGORY =5																		   " + Environment.NewLine;
                        strqry += "      ) Q																										   " + Environment.NewLine;
                        strqry += "      GROUP BY FGITEM		   " + Environment.NewLine;

                        dtvalueadditions = GetData(strqry);
                    }
                    #endregion
                }

                #region"Rdl"
                rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/RoughEstimate.rdl");
                rptViewr.LocalReport.DataSources.Clear();
                var Rds = new ReportDataSource();
                Rds.Name = "DSlineSet";
                Rds.Value = dtline;
                rptViewr.LocalReport.DataSources.Add(Rds);

                var RdsHeader = new ReportDataSource();
                RdsHeader.Name = "DSTotalSet";
                RdsHeader.Value = dtTotal;
                rptViewr.LocalReport.DataSources.Add(RdsHeader);


                var RdsCustDetail = new ReportDataSource();
                RdsCustDetail.Name = "DSStoneSet";
                RdsCustDetail.Value = dtstonedetails;
                rptViewr.LocalReport.DataSources.Add(RdsCustDetail);

                var RdsCDetail = new ReportDataSource();
                RdsCDetail.Name = "DsvalueAdditionSet";
                RdsCDetail.Value = dtvalueadditions;
                rptViewr.LocalReport.DataSources.Add(RdsCDetail);

                //var rdsEstimate = new ReportDataSource();
                //rdsEstimate.Name = "Dsestimate";
                //rdsEstimate.Value = dtestimate;
                //rptViewr.LocalReport.DataSources.Add(rdsEstimate);




                ReportParameter amtpararest = new ReportParameter();
                amtpararest.Name = "EstGroupNo";
                amtpararest.Values.Add(EstimateGroup);
                rptViewr.LocalReport.SetParameters(amtpararest);

                ReportParameter Kmparacust = new ReportParameter();
                Kmparacust.Name = "Customercode";
                Kmparacust.Values.Add(strCustAccount);
                rptViewr.LocalReport.SetParameters(Kmparacust);

                var rdmetal = new ReportDataSource();
                rdmetal.Name = "DSMetal";
                rdmetal.Value = dtMetalPurity;
                rptViewr.LocalReport.DataSources.Add(rdmetal);

                ReportParameter kmparaSTONEDETAIL = new ReportParameter();
                kmparaSTONEDETAIL.Name = "ISSTONEDETAIL";
                kmparaSTONEDETAIL.Values.Add(STONEDETAIL);
                rptViewr.LocalReport.SetParameters(kmparaSTONEDETAIL);

                ReportParameter kmpVADETAIL = new ReportParameter();
                kmpVADETAIL.Name = "ISVADETAIL";
                kmpVADETAIL.Values.Add(VADETAIL);
                rptViewr.LocalReport.SetParameters(kmpVADETAIL);

                ReportParameter kmparBILLINGDETAIL = new ReportParameter();
                kmparBILLINGDETAIL.Name = "ISBILLINGDETAIL";
                kmparBILLINGDETAIL.Values.Add(BILLINGDETAIL);
                rptViewr.LocalReport.SetParameters(kmparBILLINGDETAIL);


                rptViewr.LocalReport.DisplayName = "RoughEstimate";
                if (pdfflag == "0")
                {
                    rptViewr.LocalReport.Refresh();
                }
                else if (pdfflag == "1")
                {
                    DownloadforEstimatePdf(strCustAccount + "_" + EstimateGroup + "_" + Transdate + "_" + Storeid + "_" + ReportType);
                }
                else if (pdfflag == "2")
                {
                    string savePath = Server.MapPath("DownloadInvoice\\RoughEstimate.pdf");
                    SendFileToPrinter("HP LaserJet Pro MFP M125-M126 PCLmS (redirected 4)", savePath);
                }

                #endregion
            }
            catch (Exception ex)
            {
                Response.Redirect("ErrorPage.aspx?Error=" + ex.Message.ToString());
                return;
            }

        }

        #region"QR code"

        public string GenrateQrCode(string invoiceno, string dateofinvoice, string gstinno
            , string supplierupiid, string Bankdetails, string IFSCCODE, string gstdetails, decimal invoiceamt, string Bankaccountname)
        {
          
            //string Bankdetails = "";
            //string gstdetails = "";
            //string invoiceamt = "";

            string valueqr = "";
            string Qrcodevalue = "Invoice No:-" + invoiceno + " Date of Invoice:-" + dateofinvoice + "Invoice Amount:-" + invoiceamt + " GST NO:-" + gstinno + " " + Environment.NewLine;
            Qrcodevalue += "Bank Name:-" + Bankaccountname + " Bank Account NO:-" + Bankdetails + " IFSC Code:-" + IFSCCODE + " UPI ID:-" + supplierupiid + "" + Environment.NewLine;
            Qrcodevalue += "GSTDetails:-" + gstdetails + "";

            QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(Qrcodevalue, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qR = new QRCoder.QRCode(qRCodeData);
            Bitmap bmp = qR.GetGraphic(17);
            byte[] img = null;
            using (Bitmap bitMap = qR.GetGraphic(17))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    img = new byte[ms.ToArray().Length];
                    img = ms.ToArray();
                     valueqr = Convert.ToBase64String(img);
                  

                }

            }
            return valueqr;

        }

        #endregion

        #region"Pdf Download"
        public void DownloadPdf(string ReportName)
        {


            #region generate PDF of ReportViewer
            string reportPath = Server.MapPath("\\PdfDocuments\\" + ReportName + ".pdf");
            //  byte[] Bytes = ReportViewer1.LocalReport.Render(format: "PDF", deviceInfo: "");
            if (File.Exists(reportPath))
            {
                WebClient User = new WebClient();
                Byte[] FileBuffer = User.DownloadData(reportPath);
                if (FileBuffer != null)
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-length", FileBuffer.Length.ToString());
                    Response.BinaryWrite(FileBuffer);
                }  
            }
            else
            { 
                 
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            byte[] bytes = rptViewr.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


            // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.        //--Logic Given By Amol The Master Asset of .Net--//
            Response.Buffer = true;
            // Response.Clear();
            Response.ContentType = mimeType;
            //string filename1 = ReportName'"+.+" + extension;
            //Response.AddHeader("content-disposition:inline;", "filename=" + filename1);
            Response.BinaryWrite(bytes); // create the file
            //Response.Flush(); // send it to the client to download
            Response.Flush();

            
            FileStream fs = new FileStream(reportPath, FileMode.Create);

            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            }




            //string pdfSiteUrl ="http://localhost:2605/frmReportViewer.aspx";
            // string pdfFilename = pdfSiteUrl + "/" + ReportName + ".pdf";
            // //          <asp:Literal ID="ltEmbed" runat="server" />
            // urIframe.Attributes.Add("src", pdfFilename);
            #endregion

        }

        public void DownloadforEstimatePdf(string ReportName)
        {


            #region generate PDF of ReportViewer

            string reportPath = Server.MapPath("\\PdfDocuments\\" + ReportName + ".pdf");
            //  byte[] Bytes = ReportViewer1.LocalReport.Render(format: "PDF", deviceInfo: "");
           
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                byte[] bytes = rptViewr.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


                // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.        //--Logic Given By Amol The Master Asset of .Net--//
                Response.Buffer = true;
                // Response.Clear();
                Response.ContentType = mimeType;
                //string filename1 = ReportName'"+.+" + extension;
                //Response.AddHeader("content-disposition:inline;", "filename=" + filename1);
                Response.BinaryWrite(bytes); // create the file
                                             //Response.Flush(); // send it to the client to download
                Response.Flush();


                FileStream fs = new FileStream(reportPath, FileMode.Create);

                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            
            #endregion

        }

        #endregion


        #region "Printer Code"

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public string showPDF(string reporturl, string filename)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;
            string reportPath = "";

            try
            {
                byte[] bytes = rptViewr.LocalReport.Render("PDF", null, out mimeType,
                           out encoding, out extension, out streamids, out warnings);

                //rptViewr.LocalReport.ReportPath = Server.MapPath("~/Report/TermConditions.rdl");
                //rptViewr.LocalReport.Refresh();
                byte[] bytes_tc = rptViewr.LocalReport.Render("PDF", null, out mimeType,
                               out encoding, out extension, out streamids, out warnings);





                reportPath = Server.MapPath("\\PdfDocuments\\" + filename + ".pdf");
                FileStream fs = new FileStream(reportPath, FileMode.Create);

                fs.Write(bytes_tc, 0, bytes_tc.Length);
                fs.Close();



                //string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"500px\" height=\"300px\">";
                //embed += "If you are unable to view file, you can download from <a href = \"{0}\">here</a>";
                //embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
                //embed += "</object>";

                string pdfSiteUrl = reporturl;////"http://localhost:2605/";
                string pdfFilename = pdfSiteUrl + "/" + filename + ".pdf";
                //          <asp:Literal ID="ltEmbed" runat="server" />
                urIframe.Attributes.Add("src", pdfFilename);
            }
            catch (Exception)
            {
                return "";
            }

            return reportPath;
        }
        public async Task<string> createPDF(string reporturl, string filename)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;
            string reportPath = "";

            try
            {
                byte[] bytes = rptViewr.LocalReport.Render("PDF", null, out mimeType,
                           out encoding, out extension, out streamids, out warnings);

                reportPath = Server.MapPath("\\PdfDocuments\\Digital" + filename + ".pdf");
                FileStream fs = new FileStream(reportPath, FileMode.Create);

                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
            catch (Exception)
            {
                return "";
            }

            return reportPath;
        }

        public static async Task<string> UploadFiles(string reporturl, string filename)
        {

            string bloblFullPath = null;
            //await WriteLog("Blob Upload,,,," + filename + "," + reporturl).ConfigureAwait(false);
            if (reporturl == null || reporturl == "")
            {
                return bloblFullPath;
            }
            else
            {
                try
                {
                    //await WriteLog("Blob Uploading,,,," + filename + "," ).ConfigureAwait(false);
                    var pdfFile = File.ReadAllBytes(reporturl);

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    WebRequest request = WebRequest.Create("https://acx-global-apim.azure-api.net/JoyGlobalUatAzureFunction/SaleInvoiceFileUPload");
                    request.Method = "POST";
                    request.ContentLength = pdfFile.Length;
                    request.ContentType = "application/pdf";
                    //request.Headers.Add("FileName", filename+"_"+DateTime.Now.Ticks);
                    request.Headers.Add("FileName", filename );

                    request.Headers.Add("Ocp-Apim-Subscription-Key", "2726453389734b08822b5e390e6e49ba");
                    Stream stream = request.GetRequestStream();
                    stream.Write(pdfFile, 0, pdfFile.Length);
                    stream.Close();
                    //await WriteLog("Blob Requesting,,,," + filename + ",").ConfigureAwait(false);
                    HttpWebResponse response =  (HttpWebResponse)(await request.GetResponseAsync().ConfigureAwait(false));
                    StreamReader reader = new StreamReader(response.GetResponseStream());                   

                    JObject res = JObject.Parse(reader.ReadToEnd());
                    //await WriteLog("Blob Requesting,,,," + filename + "," + JsonConvert.SerializeObject(res)).ConfigureAwait(false);

                    reader.Close();

                    if (res["status"].ToString() == "1") bloblFullPath = res["url"].ToString();
                    //await WriteLog("Blob Upload,,,," + filename + "," + bloblFullPath).ConfigureAwait(false);
                    

                }
                catch (Exception ex)
                {
                    await WriteLog("Blob Upload,,,," + filename + "," + ex.Message).ConfigureAwait(false);
                    return bloblFullPath;
                }
            }
            return bloblFullPath;
        }

        private class WhatsappBody
        {
            public Messages[] messages { get; set; }
            public string responseType { get; set; } = "json";

            public class Messages
            {
                public string sender { get; set; } = "919072001840";
                public string to { get; set; }
                public string messageId { get; set; }
                public string transactionId { get; set; }
                public string channel { get; set; } = "wa";
                public string type { get; set; } = "template";

                public Template template { get; set; }


            }
            public class Template
            {
                public Body[] body { get; set; }
                public string templateId { get; set; } = "invoice_1";
                public string langCode { get; set; } = "en";

            }

            public class Body 
            {
                public string type { get; set; } = "text";
                public string text { get; set; }

            }
        }
        public static async Task<string> GetWhatsAppTokenToken()
        {
            string result_data = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    string WhatsAppTokenUrl = System.Configuration.ConfigurationManager.AppSettings["WhatsAppTokenUrl"].ToString();
                    string WhatsAppClientId = System.Configuration.ConfigurationManager.AppSettings["WhatsAppClientId"].ToString();
                    string WhatsAppUserName = System.Configuration.ConfigurationManager.AppSettings["WhatsAppUserName"].ToString();
                    string WhatsAppPassword = System.Configuration.ConfigurationManager.AppSettings["WhatsAppPassword"].ToString();

                    client.BaseAddress = new Uri(WhatsAppTokenUrl);
                    var httpContent = new StringContent($"grant_type=password&client_id=" + WhatsAppClientId + "&username=" + WhatsAppUserName + "&password=" + WhatsAppPassword, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var responseTask = client.PostAsync("", httpContent);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        result_data = await result.Content.ReadAsStringAsync();
                        WhatsAppTokenResponse objResponse= JsonConvert.DeserializeObject<WhatsAppTokenResponse>(result_data);
                        return objResponse.access_token;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }
        public static async Task<string> SendWhatsapp(string reporturl, string transid, string custMobNo,string strKey)
        {

            string bloblFullPath = null;
            string WhatsAppUrl = System.Configuration.ConfigurationManager.AppSettings["WhatsAppUrl"].ToString();
            string whatsLinkUrl = System.Configuration.ConfigurationManager.AppSettings["WebLinkUrl"].ToString();

            if (reporturl == null || reporturl == "" || whatsLinkUrl=="" )
            {
                return bloblFullPath;
            }
            else
            {
                whatsLinkUrl += strKey;
                try
                {
                    string token = await GetWhatsAppTokenToken();
                    if (token != null)
                    {

                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        WebRequest request = WebRequest.Create(WhatsAppUrl); 
                        //https://push.aclwhatsapp.com/pull-platform-receiver/wa/message
                        request.Method = "POST";

                        request.ContentType = "application/json";
                        request.Headers.Add("Authorization", "bearer " + token);
                        //request.Headers.Add("user", "joyalpd");
                        //request.Headers.Add("pass", "joyalpd29");

                        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                        {
                            WhatsappBody whatsappBody = new WhatsappBody()
                            {
                                messages = new WhatsappBody.Messages[]
                                {
                                new WhatsappBody.Messages()
                                {
                                    to = custMobNo,
                                    messageId = "xxxxx",
                                    transactionId = transid,
                                    template = new WhatsappBody.Template()
                                    {
                                        body = new WhatsappBody.Body[]
                                        {
                                            new WhatsappBody.Body()
                                            {
                                                text = whatsLinkUrl //reporturl
                                            }
                                        }

                                    }
                                }

                                }

                            };
                            var json = JsonConvert.SerializeObject(whatsappBody);
                            streamWriter.Write(json);
                        }

                        HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync().ConfigureAwait(false));
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        string returndata = reader.ReadToEnd();
                        if (returndata != null)
                        {
                            JObject res = JObject.Parse(returndata);
                            reader.Close();
                            return returndata;// JsonConvert.ToString(res);
                        }
                        else
                        {
                            await WriteLog("Whatsapp" + "," + transid + "," + custMobNo + "," + "" + "," + "whats sending" + "," + returndata).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await WriteLog("Whatsapp" + "," + transid + "," + custMobNo + "," + "" + "," + "whats sending" + ",token not generated").ConfigureAwait(false);
                    }
                    //if (res["success"].ToString() == "true") ;
                }
                catch (Exception ex)
                {
                    await WriteLog("Whatsapp" + "," + transid + "," + custMobNo + "," + "" + "," + "whats sending" + "," + ex.Message).ConfigureAwait(false);

                    return bloblFullPath;
                }
            }
            return bloblFullPath;
        }

        private class WhatsAppTokenResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public int refresh_expires_in { get; set; }
            public string refresh_token { get; set; }
            public string token_type { get; set; }
            public string session_state { get; set; }
            public string scope { get; set; }
        }
        private class MailBody
        {
            [JsonProperty(PropertyName = "Email ID")]
            public string Email_ID { get; set; }
            public string InvoiceDate { get; set; }
            public string CustomerName { get; set; }
            public string FileName { get; set; }
            public string FileFormat { get; set; } = "pdf";
            public string MessageContent { get; set; }
            public _Attachment[] Attachment { get; set; }
            public class _Attachment 
            {
                public string Name { get; set; }
                public _ContentBytes ContentBytes { get; set; }
            }

            public class _ContentBytes
            {
                [JsonProperty(PropertyName = "$content-type")]
                public string content_type { get; set; } = "application/pdf";

                [JsonProperty(PropertyName = "$content")]
                public string content { get; set; }


            }

        }
        public static async Task<string> SendMail(string reporturl, string transid
            , string custEmail
            , string invoiceDate
            , string custName
            , string fileName
            , string message)
        {

            string bloblFullPath = null;
            if (reporturl == null || reporturl == "")
            {
                return bloblFullPath;
            }
            else
            {
                try
                {
                    byte[] pdfFile = new WebClient().DownloadData(reporturl);
                    var fileBase64 = Convert.ToBase64String(pdfFile);

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    WebRequest request = WebRequest.Create("https://send-invoice-email.azurewebsites.net:443/api/SendInvoiceEmail/triggers/manual/invoke?api-version=2022-05-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=UotYf7EeuOV8g4xalAo1S3B09lIIUNuStRxokJxtWq4");
                    request.Method = "POST";
                    request.ContentType = "application/json";                    

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        MailBody mailBody = new MailBody()
                        {
                            Email_ID = custEmail,
                            InvoiceDate = invoiceDate,
                            CustomerName = custName,
                            FileName = fileName,
                            MessageContent = message,
                            Attachment = new MailBody._Attachment[]
                            {
                                new MailBody._Attachment()
                                {
                                    Name = fileName + ".pdf",
                                    ContentBytes = new MailBody._ContentBytes()
                                    {
                                        content = fileBase64
                                    }
                                }
                            }
                        };
                        var json = JsonConvert.SerializeObject(mailBody);
                        streamWriter.Write(json);
                    }

                    HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync().ConfigureAwait(false));
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    string res = reader.ReadToEnd();
                    reader.Close();
                    await WriteLog("Email," + transid + "," + custEmail + "," + custName + "," + fileName + "," + res).ConfigureAwait(false);

                    return res;
                    //if (res["success"].ToString() == "true") ;
                }
                catch (Exception)
                {
                    return bloblFullPath;
                }
            }
            //return bloblFullPath;
        }


        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        #endregion
        public static async Task WriteLog(string Message)
        {
            StreamWriter sw = null;
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}_{DateTime.Now.ToString("yyyyMMdd")}.log";
            try
            {
                
                sw = new StreamWriter(path, true);
                sw.WriteLine(DateTime.Now.ToString() + "::" + Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
    }



}

