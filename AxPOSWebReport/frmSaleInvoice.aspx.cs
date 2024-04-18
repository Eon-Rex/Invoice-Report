using Microsoft.Reporting.WebForms;
using System;
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

namespace AxPOSWebReport
{
    public partial class frmSaleInvoice : System.Web.UI.Page
    {
        frmReportViewer report = new frmReportViewer();
        protected void Page_Load(object sender, EventArgs e)
        {
                string ReceiptID = Request["ReceiptID"];
                string Storeid = Request["StoreCode"];
                ShowInvoice(ReceiptID, Storeid);
        }



        public void ShowInvoice(string strReceiptId, string Storeid)
        {
            try
            {
                Int32 totalAmount = 0;

                string strInvoiceCaption = string.Empty;
                string strInvoiceHeader = string.Empty;
                decimal decTaxableValue = 0;
                string mAmountWords;
                string transid = string.Empty; /////// dtRTT.Rows[0]["TRANSACTIONID"].ToString();
                // string storeno = ""; //////// dtRTST.Rows[0]["STORE"].ToString();
                string strSalesPersonCode = string.Empty;
                string stqry = "";
                decimal roundoff = 0;
                string strTransDate = "";
                //                            string[] Tender  = new string[5];                        
                DataTable dtTotal;
                DataRow drRow;

                dtTotal = new DataTable();
                dtTotal.Columns.Add("Description");
                dtTotal.Columns.Add("TaxAmount");
                string ConnectionString = ConfigurationManager.AppSettings["DBCON"].ToString();
                SqlConnection conn = new SqlConnection(ConnectionString);

                SqlCommand Cmd;
                SqlDataAdapter da;
                DataTable dtHeader = new DataTable();


                #region"Store Info"

                stqry = " select GSTIN,COMPANYNAME,ADDRESS1,ADDRESS2,ADDRESS3,POSINVOICEHEADER1,POSINVOICEHEADER2,POSINVOICEHEADER3 "
                                + ", PAN, CIN, STATECODE, b.NAME,ADDRESS1+ +ADDRESS2+ +ADDRESS3 StoreAddress "
                                + "from ext.ACXSTOREINFO a "
                                + "LEFT JOIN ax.LOGISTICSADDRESSSTATE b on b.STATEID = a.STATECODE "
                                + " where a.STORENUMBER='" + Storeid + "' ";

                dtHeader=  report.GetData(stqry);
                string Shippingaddress = "";

                if (dtHeader.Rows.Count > 0)
                {
                    PosHeader1.Text = dtHeader.Rows[0]["POSINVOICEHEADER1"].ToString();
                    PosHeader2.Text = dtHeader.Rows[0]["POSINVOICEHEADER2"].ToString();
                    PosHeader3.Text = dtHeader.Rows[0]["POSINVOICEHEADER3"].ToString();
                    lblGSTIN.Text = dtHeader.Rows[0]["GSTIN"].ToString();
                    lblName.Text = dtHeader.Rows[0]["COMPANYNAME"].ToString();
                    lbladdress1.Text = dtHeader.Rows[0]["ADDRESS1"].ToString();
                    lbladdress2.Text =dtHeader.Rows[0]["ADDRESS2"].ToString()+' '+dtHeader.Rows[0]["ADDRESS3"].ToString();
                    lblInvoiceNo.Text = strReceiptId;
                    
                }

                #endregion

                #region"parameter Get"
                stqry = "select  rtt.RECEIPTID,rtt.salespaymentdifference ,rtt.TRANSACTIONID, rtt.CHANNEL, rtt.CUSTACCOUNT, rtt.TRANSDATE, STAFF, rtt.Store, rtt.GrossAmount," + Environment.NewLine;
                stqry += "at.ISDELIVERYATSTORE from ax.RetailTransactionTable rtt LEFT JOIN ext.ACXRETAILTRANSACTIONTABLE at " + Environment.NewLine;
                stqry += " on at.TRANSACTIONID = rtt.TRANSACTIONID and at.STORE = rtt.STORE where rtt.RECEIPTID='" + strReceiptId + "' AND ENTRYSTATUS=0";
              
                DataTable dtRTT = new DataTable();
                dtRTT = report.GetData(stqry);
                if (dtRTT.Rows.Count > 0)
                {
                    //storeno = dtRTT.Rows[0]["STORE"].ToString();
                   // totalAmount = Convert.ToInt32(Convert.ToDecimal(dtRTT.Rows[0]["GrossAmount"].ToString()) * -1);
                    strTransDate = Convert.ToDateTime(dtRTT.Rows[0]["TRANSDATE"].ToString()).ToString("dd-MMM-yyyy");
                    transid = dtRTT.Rows[0]["TRANSACTIONID"].ToString();
                    roundoff = Convert.ToDecimal(dtRTT.Rows[0]["salespaymentdifference"]);
                    lblInvoicedate.Text = strTransDate;
                }
                else
                {

                    Response.Redirect("ErrorPage.aspx?Error=" + "No Invoice Details Found" + "", false);
                    conn.Close();
                    return;
                }

                #endregion

                #region"Customer Details"
                // for customer detail

                string custqry1 = "Select A.ACCOUNTNUM,b.CUSTNAME NAME,replace(replace(COALEsCE(B.ADDRESS,''),char(10),' '),char(13),' ') as       " + Environment.NewLine;
                custqry1 += "CustAddress, COALEsCE(B.CITY,'') as CustCity , COALESCE(B.COUNTRYREGIONID, '') as CustCountRegion,            " + Environment.NewLine;
                custqry1 += "COALESCE(B.DISTRICTNAME, '') as CustDistrictName , COALESCE(B.STATE, '') as CustStateCode, COALESCE(B.STREET, '') as CustStreet            " + Environment.NewLine;
                custqry1 += ", COALESCE(B.ZIPCODE, '') as CustZipCode , COALESCE(B.PANNUMBER, '') as PANNumber  " + Environment.NewLine;
                custqry1 += ", '' as RegistrationNumber  ,b.PHONE " + Environment.NewLine;
                custqry1 += "--, COALESCE(g.REGISTRATIONNUMBER,'') as RegistrationNumber   " + Environment.NewLine;
                custqry1 += ", COALESCE(B.STATENAME, '') as StateName   " + Environment.NewLine;
                custqry1 += " From ax.CUSTTABLE a INNER JOIN ext.ACXCUSTOMERDETAILSVIEW b on a.ACCOUNTNUM = b.CUSTACCOUNT   " + Environment.NewLine;
                custqry1 += "--LEFT JOIN ax.TAXINFORMATION_IN f on f.REGISTRATIONLOCATION = c.LOCATION   " + Environment.NewLine;
                custqry1 += "--LEFT JOIN ax.TAXREGISTRATIONNUMBERS_IN g on g.RECID=f.GSTIN   " + Environment.NewLine;
                custqry1 += "where ACCOUNTNUM  = '" + dtRTT.Rows[0]["CUSTACCOUNT"].ToString() + "' ";
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

                if (dtcust.Rows.Count > 0)
                {
                    lblRecName.Text = dtcust.Rows[0]["ACCOUNTNUM"].ToString() + '-' + dtcust.Rows[0]["NAME"].ToString();
                    lblRecAddress.Text = dtcust.Rows[0]["CustAddress"].ToString() + "       " + dtcust.Rows[0]["CustCity"].ToString(); ;
                    lblRecMobile.Text = dtcust.Rows[0]["PHONE"].ToString();
                    lblRecState.Text = dtcust.Rows[0]["StateName"].ToString();
                    lblRecStateCode.Text = dtcust.Rows[0]["CustStateCode"].ToString();
                    lblRecGSTIN.Text = dtcust.Rows[0]["RegistrationNumber"].ToString();
                    lblRecPAN.Text = dtcust.Rows[0]["PANNumber"].ToString();

                    lblShipName.Text = dtcust.Rows[0]["ACCOUNTNUM"].ToString() + '-' + dtcust.Rows[0]["NAME"].ToString();
                    lblShipMob.Text = dtcust.Rows[0]["PHONE"].ToString();
                    
                    
                }


                #endregion


                #region"Inventlocation"
                // for InventLocation detail
                string InventLocationqry1 = "Select Coalesce(e.Name, '') as StateName,COALESCE(g.REGISTRATIONNUMBER,'') as RegistrationNumber "
                                + ",Coalesce(d.STATE,'') as StateCode, Coalesce(d.COUNTRYREGIONID,'') as CountryRegion "
                                + " From ax.RETAILSTORETABLE a "
                                + "INNER JOIN ax.INVENTLOCATION b on a.INVENTLOCATIONIDFORCUSTOMERORDER=b.INVENTLOCATIONID "
                                + "INNER JOIN ax.INVENTLOCATIONLOGISTICSLOCATION c on c.INVENTLOCATION=b.RECID "
                                + "INNER JOIN ax.LOGISTICSPOSTALADDRESS d on c.LOCATION=d.LOCATION "
                                + "INNER JOIN ax.LOGISTICSADDRESSSTATE e on d.STATE = e.STATEID and d.COUNTRYREGIONID=e.COUNTRYREGIONID "
                                + "LEFT JOIN ax.TAXINFORMATION_IN f on f.REGISTRATIONLOCATION = c.LOCATION "
                                + "LEFT JOIN ax.TAXREGISTRATIONNUMBERS_IN g on g.RECID=f.GSTIN "
                                + "where a.STORENUMBER = '" + Storeid + "' "
                                + "and (f.ISPRIMARY=1 or f.RECID is Null) "
                                + "and ((d.VALIDFROM <= '" + strTransDate + "' and d.VALIDTO >='" + strTransDate + "') or d.RECID is Null) ";
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



                if (dtRTT.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dtRTT.Rows[0]["ISDELIVERYATSTORE"]) == 0)
                    {
                        if (dtcust.Rows.Count > 0)
                        {
                            lblShipAddress.Text = dtcust.Rows[0]["CustAddress"].ToString() + "         " + dtcust.Rows[0]["CustCity"].ToString(); ;
                            lblShipStateCode.Text = dtcust.Rows[0]["CustStateCode"].ToString();
                           lblShipState.Text = dtcust.Rows[0]["StateName"].ToString();
                        }
                    }
                    else if (Convert.ToInt32(dtRTT.Rows[0]["ISDELIVERYATSTORE"]) == 1)
                    {
                        lblShipStateCode.Text = dtcust.Rows[0]["CustStateCode"].ToString();
                        lblShipState.Text = dtcust.Rows[0]["StateName"].ToString();
                        lblShipAddress.Text = "Supply at shop" + "         " + dtcust.Rows[0]["CustCity"].ToString();
                      //  lblShipAddres2.Text = dtHeader.Rows[0]["ADDRESS2"].ToString() + ' ' + dtHeader.Rows[0]["ADDRESS3"].ToString();

                    }

                    lblShipGst.Text = dtInventLocation.Rows[0]["RegistrationNumber"].ToString();
                }


                #endregion

                #region"Invoice Line"
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = "select ROW_NUMBER() over (order by a.ReceiptId) SLNO, b.NAMEALIAS,c.CODE,d.UNITID,CAST(COALESCE(e.GROSSWEIGHT,0.000) AS decimal(18,3)) as GrossWeight ,cast(isnull((e.FINALDIAMONDWT+e.finalstonecwt+e.finalstonegwt),'0') as decimal(18,3)) DiamondWeight ,CAST(COALESCE(e.NETWEIGHT,0.000) AS decimal(18,3)) as NetWeight," + Environment.NewLine;
                stqry += "cast(COALESCE(e.FINALMETALV,0.000) as decimal(18,2)) as GoldValue ,cast(COALESCE(E.FINALMAKINGV,0.000) as decimal(18,2)) as VA,ISNULL(g.PCS,0) PCS, 		  " + Environment.NewLine;
                stqry += "CAST(COALESCE((e.FINALSTONEGV + e.FINALSTONECV + e.FINALDIAMONDV),0.000) AS DECIMAL(18,2)) as StoneValue , 										  " + Environment.NewLine;
                stqry += "cast(COALESCE((E.FINALMAKINGV+e.FINALSTONEGV + e.FINALSTONECV + e.FINALDIAMONDV+E.FINALMETALV),0.000) as decimal(18,2)) as TotalValue 											  " + Environment.NewLine;
                stqry += ",cast(COALESCE(e.DISCAMOUNT,0.000) as decimal(18,2)) as Disc ,cast(COALESCE((a.NetAmount *-1),0.000) as decimal(18,2))as TaxableValue, COALESCE(e.TagNo,'') as TagNo,						  " + Environment.NewLine;
                stqry += "a.ReceiptId, Coalesce(e.Purity,0) as Purity ,COALESCE(f.Description,'') as BrandDesc , COALESCE(g.PRICE,0) as Price,case when e.FINALMETALV > 0 and e.NETWEIGHT >0 THEN e.FINALMETALV/e.NETWEIGHT ELSE 0 END RATE , 						  " + Environment.NewLine;
                stqry += "e.SALESPERSONCODE from ax.RetailTransactionSalesTrans a INNER JOIN ax.INVENTTABLE b on a.ITEMID=b.ITEMID INNER JOIN ax.HSNCODETABLE_IN c 	  " + Environment.NewLine;
                stqry += "on b.HSNCODETABLE_IN =c.RECID INNER JOIN ax.INVENTTABLEMODULE d on a.ITEMID=d.ITEMID and d.MODULETYPE=2 									  " + Environment.NewLine;
                stqry += "LEFT JOIN ext.acxRetailTransactionSalesTrans e on a.TRANSACTIONID =e.TransactionID and a.LINENUM=e.LineNum									  " + Environment.NewLine;
                stqry += "LEFT JOIN ext.ACXTAGHEADER f on e.TAGNO = f.TAGNO 																							  " + Environment.NewLine;
                stqry += "LEFT JOIN (Select  A.POSTEDTRANSACTIONID,S.LINENUM POSTEDLINENUM,max(S.pcs) PCS, max(PRICE) as Price From ext.ACXESTIMATELINESPOSTED A 				  " + Environment.NewLine;
                stqry += "INNER JOIN ext.ACXESTIMATETABLEPOSTED B on A.POSTEDTRANSACTIONID = B.POSTEDTRANSACTIONID and A.TRANSACTIONID=B.TRANSACTIONID 				  " + Environment.NewLine;
                stqry += "INNER JOIN ext.acxRetailTransactionSalesTrans S ON A.POSTEDTRANSACTIONID=S.TRANSACTIONID AND A.TRANSACTIONID=S.ESTIMATETRANSACTIONID" + Environment.NewLine;
                stqry += "where A.POSTEDTRANSACTIONID='" + transid + "' and PRIMARYITEM=1 Group by A.POSTEDTRANSACTIONID,S.LINENUM) g 							  " + Environment.NewLine;
                stqry += "on a.TRANSACTIONID = g.POSTEDTRANSACTIONID and a.LINENUM = g.POSTEDLINENUM LEFT JOIN 														  " + Environment.NewLine;
                stqry += "(select B.POSTEDTRANSACTIONID,B.POSTEDLINENUM,ISNULL(SUM(A.AMOUNT), 0) EXTMAKING From ext.ACXESTIMATELINESPOSTED A 							  " + Environment.NewLine;
                stqry += "INNER JOIN ext.ACXESTIMATETABLEPOSTED B on A.POSTEDTRANSACTIONID = B.POSTEDTRANSACTIONID and A.TRANSACTIONID = B.TRANSACTIONID 			  " + Environment.NewLine;
                stqry += "where A.POSTEDTRANSACTIONID = '" + transid + "' AND ITEMCATEGORY = 5 GROUP BY B.POSTEDTRANSACTIONID,B.POSTEDLINENUM) AS EG   " + Environment.NewLine;
                stqry += "ON a.TRANSACTIONID = EG.POSTEDTRANSACTIONID and a.LINENUM = EG.POSTEDLINENUM where a.TRANSACTIONID='" + transid + "' AND a.TRANSACTIONSTATUS=0";
                
                DataTable dtRTST = new DataTable();
                dtRTST = report.GetData(stqry);
                decTaxableValue = 0;
                for (int ii = 0; ii < dtRTST.Rows.Count; ii++)
                {
                    decTaxableValue += Convert.ToDecimal(dtRTST.Rows[ii]["TaxableValue"].ToString());
                   
                }
                totalAmount = Convert.ToInt32(Convert.ToDecimal(decTaxableValue));
                if (dtRTST.Rows.Count > 0)
                {
                    strSalesPersonCode = dtRTST.Rows[0]["SALESPERSONCODE"].ToString();
                    lblsaleperson.Text = strSalesPersonCode;
                }
              
                gdinvoiceline.DataSource = dtRTST;
                gdinvoiceline.DataBind();
                gdinvoiceline.FooterRow.Cells[1].Text = "Total";
                gdinvoiceline.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Left;

                int pcs = dtRTST.AsEnumerable().Sum(s => s.Field<int>("PCS"));
                gdinvoiceline.FooterRow.Cells[4].Text = pcs.ToString();

                decimal GrossWeight = dtRTST.AsEnumerable().Sum(s => s.Field<decimal>("GrossWeight"));
                gdinvoiceline.FooterRow.Cells[5].Text = GrossWeight.ToString();

                decimal NetWeight = dtRTST.AsEnumerable().Sum(s => s.Field<decimal>("NetWeight"));
                gdinvoiceline.FooterRow.Cells[6].Text = NetWeight.ToString();

                decimal GoldValue = dtRTST.AsEnumerable().Sum(s => s.Field<decimal>("GoldValue"));
                gdinvoiceline.FooterRow.Cells[7].Text = GoldValue.ToString();
                gdinvoiceline.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Right;

                decimal VA = dtRTST.AsEnumerable().Sum(s => s.Field<decimal>("VA"));
                gdinvoiceline.FooterRow.Cells[8].Text = VA.ToString();
                gdinvoiceline.FooterRow.Cells[8].HorizontalAlign = HorizontalAlign.Right;
                
                decimal StoneValue = dtRTST.AsEnumerable().Sum(s => s.Field<decimal>("StoneValue"));
                gdinvoiceline.FooterRow.Cells[9].Text = StoneValue.ToString();
                gdinvoiceline.FooterRow.Cells[9].HorizontalAlign = HorizontalAlign.Right;

                decimal TotalValue = dtRTST.AsEnumerable().Sum(s => s.Field<decimal>("TotalValue"));
                gdinvoiceline.FooterRow.Cells[10].Text = TotalValue.ToString();
                gdinvoiceline.FooterRow.Cells[10].HorizontalAlign = HorizontalAlign.Right;

                decimal Disc = dtRTST.AsEnumerable().Sum(s => s.Field<decimal>("Disc"));
                gdinvoiceline.FooterRow.Cells[11].Text = Disc.ToString();
                gdinvoiceline.FooterRow.Cells[11].HorizontalAlign = HorizontalAlign.Right;

                decimal TaxableValue = dtRTST.AsEnumerable().Sum(s => s.Field<decimal>("TaxableValue"));
                gdinvoiceline.FooterRow.Cells[12].Text = TaxableValue.ToString();
                gdinvoiceline.FooterRow.Cells[12].HorizontalAlign = HorizontalAlign.Right;
               

                #endregion

                #region"tax and tender grid"


                #region"Taxable Value"

                drRow = dtTotal.NewRow();
                drRow["Description"] = "Taxable Value";
                drRow["TaxAmount"] = decTaxableValue.ToString("########0.00");
                dtTotal.Rows.Add(drRow);


                SqlDataReader dr;
                conn.Close();
                #endregion

                #region"Total Invoice Value (in Figure)"
                stqry = "select Sum(a.TAXAMOUNT) as TaxAmount,TAXPERCENTAGE,TAXCOMPONENT from ax.RETAILTRANSACTIONTAXTRANSGTE a      " + Environment.NewLine;
                stqry += " INNER JOIN ax.RETAILTRANSACTIONSALESTRANS b on a.TRANSACTIONID =b.TRANSACTIONID AND A.SALELINENUM= b.LINENUM" + Environment.NewLine;
                stqry += " INNER JOIN ax.INVENTTABLE c on b.ITEMID= c.ITEMID INNER JOIN ext.ACXINVENTTABLE d on c.ITEMID = d.ITEMID 	 " + Environment.NewLine;
                stqry += " where b.TRANSACTIONID='" + transid + "' GROUP BY TAXPERCENTAGE,TAXCOMPONENT having Sum(a.TAXAMOUNT) <> 0";
                Cmd = new SqlCommand(stqry);
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
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = dtGTE.Rows[intGTE]["TAXCOMPONENT"].ToString() + " - " + Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXPERCENTAGE"].ToString()).ToString("0.00") + "%";
                    drRow["TaxAmount"] = Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXAMOUNT"].ToString()).ToString("####0.00");
                    decTaxableValue += Convert.ToDecimal(dtGTE.Rows[intGTE]["TAXAMOUNT"].ToString());
                    dtTotal.Rows.Add(drRow);
                }
                drRow = dtTotal.NewRow();
                drRow["Description"] = "Total Invoice Value (in Figure) ";
                drRow["TaxAmount"] = decTaxableValue.ToString("####0.00");
                dtTotal.Rows.Add(drRow);
                #endregion

                #region"Credit Card Payment"

                ////Credit Card Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) >=20 and Convert(int,TENDERTYPE) <= 37 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0";
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
                    drRow["Description"] = "Credit Cards/Debit Card ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                #endregion

                #region"Sodexo Payment"
                ////Sodexo Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =38 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                dtTend.Clear();
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
                    drRow["Description"] = "Sodexo ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                #endregion

                #region"Accor Payment"

                ////Accor Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =39 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                dtTend.Clear();
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
                    drRow["Description"] = "Accor ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                #endregion

                #region"Other Loyalty Payment"
                ////Other Loyalty Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =40 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                dtTend.Clear();
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
                    drRow["Description"] = "Other Loyalty ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                #endregion

                #region"Advance Adjustment"
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =44 AND TRANSACTIONSTATUS =0 AND REPLICATIONCOUNTERFROMORIGIN NOT IN( "
              + "  SELECT R.REPLICATIONCOUNTERFROMORIGIN FROM EXT.ACXCUSTOMERADVANCE ADV "
                 + "JOIN EXT.ACXRETAILTRANSACTIONPAYMENTTRANS P ON P.ADJUSTEDTRANSACTIONID=ADV.TRANSACTIONID AND"
                + " ADV.LINENUM=P.ADJUSTEDTRANSACTIONLINENUM AND ADV.ADVANCETYPE=8           "
                + " JOIN AX.RETAILTRANSACTIONPAYMENTTRANS R ON P.TRANSACTIONID=R.TRANSACTIONID   "
                + " AND P.LINENUM=R.LINENUM                                                      "
                + " where R.TRANSACTIONID='" + transid + "' and Convert(int,R.TENDERTYPE) =44 AND R.TRANSACTIONSTATUS =0   AND R.VOIDSTATUS=0) ";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                dtTend.Clear();
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
                    drRow["Description"] = "Advance Adjustment";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                #endregion

                #region"Scrap Exchange"
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = "  SELECT COALESCE(Sum(R.AMOUNTMST),0) as Amount FROM EXT.ACXCUSTOMERADVANCE ADV "
                  + "JOIN EXT.ACXRETAILTRANSACTIONPAYMENTTRANS P ON P.ADJUSTEDTRANSACTIONID=ADV.TRANSACTIONID AND"
                 + " ADV.LINENUM=P.ADJUSTEDTRANSACTIONLINENUM AND ADV.ADVANCETYPE=8           "
                 + " JOIN AX.RETAILTRANSACTIONPAYMENTTRANS R ON P.TRANSACTIONID=R.TRANSACTIONID   "
                 + " AND P.LINENUM=R.LINENUM                                                      "
                 + " where R.TRANSACTIONID='" + transid + "' and Convert(int,R.TENDERTYPE) =44 AND R.TRANSACTIONSTATUS =0  AND R.VOIDSTATUS=0 ";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                dtTend.Clear();
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
                    drRow["Description"] = "Scrap Exchange";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                #endregion

                #region"Loyalty Card "
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =10 AND TRANSACTIONSTATUS =0  AND   VOIDSTATUS=0";
                Cmd = new SqlCommand(stqry);
                Cmd.Connection = conn;
                da = new SqlDataAdapter(Cmd);
                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.CommandTimeout = 120;
                dtTend.Clear();
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
                    drRow["Description"] = "Loyalty Card ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                #endregion

                #region"Cheques Payment and DD"

                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =42 AND TRANSACTIONSTATUS =0   AND VOIDSTATUS=0";
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
                #endregion

                #region"Cheques RTGS and NEFT"
                ////Cheques RTGS and NEFT
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =43 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0";
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

                #endregion

                #region"Discount Offer"

                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =51 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0 ";
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
                    drRow["Description"] = "Discount Offer";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }

                #endregion

                #region"Credit Payment"
                ////Credit Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =50 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0";
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

                #endregion

                #region"Credit(Cust Acc)"
                ////Advance Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =4 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0 ";
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
                    drRow["Description"] = "Credit(Cust Acc)";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }

                #endregion

                #region"Easy Gold Payment"
                ////Easy Gold Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =41 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0";
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
                    drRow["Description"] = "Easy Gold Scheme ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                #endregion

                #region"Gift Voucher Payment"

                ////Gift Voucher Payment
                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =8 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0";
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
                    drRow["Description"] = "Gift Voucher/Gift Cards ";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }
                #endregion

                #region"Cash Payment"

                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select COALESCE(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =1 AND AMOUNTTENDERED>0 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0";
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


                conn.Close();
                #endregion

                #region"Cash Paid"

                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = " Select isnull(Sum(AMOUNTMST),0) as Amount From ax.RETAILTRANSACTIONPAYMENTTRANS "
                      + "where TRANSACTIONID='" + transid + "' and Convert(int,TENDERTYPE) =1 and  AMOUNTTENDERED<0 AND TRANSACTIONSTATUS =0  AND VOIDSTATUS=0";
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
                    drRow["Description"] = "Cash Paid";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }


                conn.Close();
                #endregion

                #region"Adv Gen"

                if (conn.State != ConnectionState.Open) { conn.Open(); }
                stqry = "select  ca.AMOUNT from ext.ACXRETAILTRANSACTIONSALESTRANS st join ext.ACXCUSTOMERADVANCE ca "
                      + "on ca.TRANSACTIONID=st.TRANSACTIONID and ca.LINENUM=st.LINENUM where ca.TRANSACTIONID='" + transid + "' and st.SALESTYPE=3";
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
                    drRow["Description"] = "Adv Gen";
                    drRow["TaxAmount"] = decTaxableValue.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }


                conn.Close();
                #endregion

                #region"Round off"

                if (roundoff != 0)
                {
                    drRow = dtTotal.NewRow();
                    drRow["Description"] = "Round Off";
                    drRow["TaxAmount"] = roundoff.ToString("#######0.00");
                    dtTotal.Rows.Add(drRow);
                }

                #endregion


                gdtaxDec.DataSource = dtTotal;
                gdtaxDec.DataBind();
                #endregion



                #region"Amt In Words"
                mAmountWords = frmReportViewer.words(totalAmount);
                lblamtinWord.Text = mAmountWords;
                #endregion




                strInvoiceCaption = "TAX INVOICE";
                // Old Gold Weight
                conn.Close();
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string strUserId = "";
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
                                lblUsername.Text = dtEmpl.Rows[0]["PERSONNELNUMBER"].ToString() + " - " + dtEmpl.Rows[0]["Name"].ToString();
                                strInvoiceHeader = dtEmpl.Rows[0]["ACX_POSINVOICEHEADER1"].ToString();
                            }
                        }
                    }
                }
                conn.Close();

                
            }
            catch (Exception ex)
            {
                Response.Redirect("ErrorPage.aspx?Error=" + ex.Message.ToString());
                /////MessageBox.Show(ex.Message.ToString());
            }

        }
    }
}