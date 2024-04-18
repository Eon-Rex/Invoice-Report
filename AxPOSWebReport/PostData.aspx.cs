using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
namespace AxPOSWebReport
{
    public partial class PostData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            getXML();
        }
        public void getXML()
        {
            try
            {

                Page.Response.ContentType = "text/xml";
                System.IO.StreamReader reader = new System.IO.StreamReader(Page.Request.InputStream);
                String xmlData = reader.ReadToEnd();
                System.IO.StreamWriter SW;
                SW = File.CreateText(Server.MapPath(".") + @"\" + Guid.NewGuid() + ".txt");
                SW.WriteLine(xmlData);
                SW.Close();

                insertToDB(xmlData);


            }
            catch (Exception ex)
            {
                System.IO.StreamWriter errorSW;
                errorSW = File.CreateText(Server.MapPath(".") + @"\error_" + Guid.NewGuid() + ".txt");
                errorSW.WriteLine(ex.Message);
                errorSW.Close();

            }

        }
        public void insertToDB(string xmlString)
        {
            string ConnectionString = ConfigurationManager.AppSettings["POSDBCON"].ToString();
                SqlConnection con = new SqlConnection(ConnectionString);
            try
            {           
                DataSet ds = new DataSet();
                using (StringReader stringReader = new StringReader(xmlString))
                {
                    ds.ReadXml(stringReader);
                }
                DataTable dtRTT = ds.Tables[0];
                DataTable dtRTST = ds.Tables[1];
                DataTable dtCust = ds.Tables[2];
                DataTable dtGTE = ds.Tables[3];
                DataTable dtTotal = ds.Tables[4];
                DataTable dtHeader = ds.Tables[5];
                DataTable dtInventLocation = ds.Tables[6];

                foreach(DataRow row in dtRTT.Rows )
                {
                    //insert invoicetable
                    try
                    {
                        if (con.State != ConnectionState.Open) { con.Open(); }
                        SqlCommand sql_cmnd = new SqlCommand("SPINSERTINVOICETABLE", con);
                        sql_cmnd.CommandType = CommandType.StoredProcedure;
                        sql_cmnd.Parameters.AddWithValue("@CHANNEL", SqlDbType.BigInt).Value = row["CHANNEL"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@RECEIPTID", SqlDbType.NVarChar).Value = row["RECEIPTID"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@TRANSACTIONID", SqlDbType.NVarChar).Value = row["TRANSACTIONID"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@CUSTACCOUNT", SqlDbType.NVarChar).Value = row["CUSTACCOUNT"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@TRANSDATE", SqlDbType.Date).Value = row["TRANSDATE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@STAFF", SqlDbType.NVarChar).Value = row["STAFF"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@STORE", SqlDbType.NVarChar).Value = row["STORE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@GROSSAMOUNT", SqlDbType.Decimal).Value = row["GROSSAMOUNT"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@ISDELIVERYATSTORE", SqlDbType.Int).Value = row["ISDELIVERYATSTORE"].ToString();
                        sql_cmnd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch(Exception EX)
                    {

                    }

                }

                foreach (DataRow row in dtRTST.Rows)
                {
                    //insert invoicelines

                    try
                    {
                        if (con.State != ConnectionState.Open) { con.Open(); }
                        SqlCommand sql_cmnd = new SqlCommand("SPINSERTINVOICELINES", con);
                        sql_cmnd.CommandType = CommandType.StoredProcedure;
                        sql_cmnd.Parameters.AddWithValue("@TRANSACTIONID", SqlDbType.BigInt).Value = row["TRANSACTIONID"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@LINENUM", SqlDbType.Decimal).Value = row["LINENUM"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@ITEMID", SqlDbType.NVarChar).Value = row["ITEMID"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@NAME", SqlDbType.NVarChar).Value = row["ITEMNAME"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@BRANDDESC", SqlDbType.NVarChar).Value = row["BRANDDESC"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@HSNCODE", SqlDbType.NVarChar).Value = row["HSNCODE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@UNITID", SqlDbType.NVarChar).Value = row["UNITID"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@PRICE", SqlDbType.Decimal).Value = row["PRICE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@RECEIPTID", SqlDbType.NVarChar).Value = row["RECEIPTID"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@GROSSWEIGHT", SqlDbType.Decimal).Value = row["GROSSWEIGHT"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@NETWEIGHT", SqlDbType.Decimal).Value = row["NETWEIGHT"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@DIAMONDWEIGHT", SqlDbType.Decimal).Value = row["DIAMONDWEIGHT"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@METALVALUE", SqlDbType.Decimal).Value = row["METALVALUE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@DIAMONDVALUE", SqlDbType.Decimal).Value = row["DIAMONDVALUE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@MAKINGVALUE", SqlDbType.Decimal).Value = row["MAKINGVALUE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@STONECVALUE", SqlDbType.Decimal).Value = row["STONECVALUE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@STONEGVALUE", SqlDbType.Decimal).Value = row["STONEGVALUE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@DISCAMOUNT", SqlDbType.Decimal).Value = row["DISCAMOUNT"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@SALESPERSONCODE", SqlDbType.NVarChar).Value = row["SALESPERSONCODE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@TAGNO", SqlDbType.NVarChar).Value = row["TAGNO"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@PURITY", SqlDbType.Decimal).Value = row["PURITY"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@PCS", SqlDbType.Int).Value = row["PCS"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@VA", SqlDbType.Decimal).Value = row["VA"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@NETAMOUNT", SqlDbType.Decimal).Value = row["NETAMOUNT"].ToString();
                        sql_cmnd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception EX)
                    {

                    }
                }

                foreach (DataRow row in dtCust.Rows)
                {
                    //insert customer
                    try
                    {
                        if (con.State != ConnectionState.Open) { con.Open(); }
                        SqlCommand sql_cmnd = new SqlCommand("SPINSERTCUSTDETAILS", con);
                        sql_cmnd.CommandType = CommandType.StoredProcedure;
                        sql_cmnd.Parameters.AddWithValue("@CUSTACCOUNT", SqlDbType.NVarChar).Value = row["CUSTACCOUNT"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@NAME", SqlDbType.NVarChar).Value = row["NAME"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@ADDRESS", SqlDbType.NVarChar).Value = row["ADDRESS"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@CITY", SqlDbType.NVarChar).Value = row["CITY"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@COUNTRYREGIONID", SqlDbType.NVarChar).Value = row["COUNTRYREGIONID"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@DISTRICTNAME", SqlDbType.NVarChar).Value = row["DISTRICTNAME"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@STREET", SqlDbType.NVarChar).Value = row["STREET"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@ZIPCODE", SqlDbType.NVarChar).Value = row["ZIPCODE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@STATECODE", SqlDbType.NVarChar).Value = row["STATECODE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@STATENAME", SqlDbType.NVarChar).Value = row["STATENAME"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@PANNUMBER", SqlDbType.NVarChar).Value = row["PANNUMBER"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@GSTIN", SqlDbType.NVarChar).Value = row["GSTIN"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@PHONE", SqlDbType.NVarChar).Value = row["PHONE"].ToString();
                        
                        sql_cmnd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception EX)
                    {

                    }
                }

                foreach (DataRow row in dtGTE.Rows)
                {
                    //insert tax
                    try
                    {
                        if (con.State != ConnectionState.Open) { con.Open(); }
                        SqlCommand sql_cmnd = new SqlCommand("SPINSERTINVOICETAX", con);
                        sql_cmnd.CommandType = CommandType.StoredProcedure;
                        sql_cmnd.Parameters.AddWithValue("@TRANSACTIONID", SqlDbType.NVarChar).Value = row["TRANSACTIONID"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@TAXCOMPONENT", SqlDbType.NVarChar).Value = row["TAXCOMPONENT"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@TAXPERCENTAGE", SqlDbType.Decimal).Value = row["TAXPERCENTAGE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@TAXAMOUNT", SqlDbType.Decimal).Value = row["TAXAMOUNT"].ToString();
                        sql_cmnd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception EX)
                    {

                    }
                }

                foreach (DataRow row in dtTotal.Rows)
                {
                    //insert invoicetotal
                    try
                    {
                        if (con.State != ConnectionState.Open) { con.Open(); }

                        if(Convert.ToDecimal(row["AMOUNT"].ToString())>0)
                        {
                            SqlCommand sql_cmnd = new SqlCommand("SPINSERTINVOICETOTAL", con);
                            sql_cmnd.CommandType = CommandType.StoredProcedure;
                            sql_cmnd.Parameters.AddWithValue("@TRANSACTIONID", SqlDbType.NVarChar).Value = row["TRANSACTIONID"].ToString();
                            sql_cmnd.Parameters.AddWithValue("@DESCRIPTION", SqlDbType.NVarChar).Value = row["DESCRIPTION"].ToString();
                            sql_cmnd.Parameters.AddWithValue("@AMOUNT", SqlDbType.Decimal).Value = row["AMOUNT"].ToString();
                            sql_cmnd.ExecuteNonQuery();
                        }
                       
                        con.Close();
                    }
                    catch (Exception EX)
                    {

                    }
                }

                foreach(DataRow row in dtHeader.Rows)
                {
                    //insert header storeinformation
                    try
                    {
                        if (con.State != ConnectionState.Open) { con.Open(); }
                        SqlCommand sql_cmnd = new SqlCommand("SPINSERTSTOREDETAILS", con);
                        sql_cmnd.CommandType = CommandType.StoredProcedure;
                        sql_cmnd.Parameters.AddWithValue("@STORENUMBER", SqlDbType.NVarChar).Value = row["STORENUMBER"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@GSTIN", SqlDbType.NVarChar).Value = row["GSTIN"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@COMPANYNAME", SqlDbType.NVarChar).Value = row["COMPANYNAME"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@ADDRESS1", SqlDbType.NVarChar).Value = row["ADDRESS1"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@ADDRESS2", SqlDbType.NVarChar).Value = row["ADDRESS2"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@ADDRESS3", SqlDbType.NVarChar).Value = row["ADDRESS3"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@POSINVOICEHEADER1", SqlDbType.NVarChar).Value = row["POSINVOICEHEADER1"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@POSINVOICEHEADER2", SqlDbType.NVarChar).Value = row["POSINVOICEHEADER2"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@POSINVOICEHEADER3", SqlDbType.NVarChar).Value = row["POSINVOICEHEADER3"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@REPORTURL", SqlDbType.NVarChar).Value = "";
                        sql_cmnd.Parameters.AddWithValue("@INVOICEFORMAT", SqlDbType.NVarChar).Value = 0;
                        sql_cmnd.Parameters.AddWithValue("@STATECODE", SqlDbType.NVarChar).Value = row["STATECODE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@STATENAME", SqlDbType.NVarChar).Value = row["STATENAME"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@PAN", SqlDbType.NVarChar).Value = row["PAN"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@CIN", SqlDbType.NVarChar).Value = row["CIN"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@DATAAREAID", SqlDbType.NVarChar).Value = row["DATAAREAID"].ToString();
                        sql_cmnd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception EX)
                    {

                    }

                }

                foreach (DataRow row in dtInventLocation.Rows)
                {
                    //insert inventlocation
                    try
                    {
                        if (con.State != ConnectionState.Open) { con.Open(); }
                        SqlCommand sql_cmnd = new SqlCommand("[dbo].[SPINSERTINVENTLOCATION]", con);
                        sql_cmnd.CommandType = CommandType.StoredProcedure;
                        sql_cmnd.Parameters.AddWithValue("@STORE", SqlDbType.NVarChar).Value = row["STORE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@STATECODE", SqlDbType.NVarChar).Value = row["STATECODE"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@STATENAME", SqlDbType.NVarChar).Value = row["STATENAME"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@COUNTRYREGIONID", SqlDbType.NVarChar).Value = row["COUNTRYREGIONID"].ToString();
                        sql_cmnd.Parameters.AddWithValue("@GSTIN", SqlDbType.Decimal).Value = row["GSTIN"].ToString();
                        sql_cmnd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception EX)
                    {

                    }

                }
               
            }
            catch(Exception ex)
            {
                System.IO.StreamWriter errorSW;
                errorSW = File.CreateText(Server.MapPath(".") + @"\errorInsert_" + Guid.NewGuid() + ".txt");
                errorSW.WriteLine(ex.Message);
                errorSW.Close();

            }

        }
        public void executeProcedure(string methodName,SqlConnection con)
        {
            
        }


    }
}