<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="frmReportViewer.aspx.cs"  Inherits="AxPOSWebReport.frmReportViewer" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>
<script src="Scripts/jquery-3.3.1.js"></script>
<script src="Scripts/jquery-3.3.1.min.js"></script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
       <%-- <input type="button" id="printreport" value="Print" />
        <script type="text/javascript">
            $('#printreport').click(function () {
                printReport('rptViewr');
            });

            function printReport(report_ID) {
                debugger;
                var rptViewr = $('#' + report_ID);
                var iDoc = rptViewr.parents('html');

                // Reading the report styles
                var styles = iDoc.find("head style[id$='ReportControl_styles']").html();
                if ((styles == undefined) || (styles == '')) {
                    iDoc.find('head script').each(function () {
                        var cnt = $(this).html();
                        var p1 = cnt.indexOf('ReportStyles":"');
                        if (p1 > 0) {
                            p1 += 10;
                            var p2 = cnt.indexOf('"', p1);
                            styles = cnt.substr(p1, p2 - p1);
                        }
                    });
                }
                if (styles == '') { alert("Cannot generate styles, Displaying without styles.."); }
                styles = '<style type="text/css">' + styles + "</style>";

                styles += '<style type="text/css" media="print"> @media print { body.print-page { -webkit-transform: scale(0.55); -moz-transform: scale(0.55); -ms-transform: scale(0.55); -o-transform: scale(0.55); transform: scale(0.55); margin: -50px -73px 0; }}</style>'

                // Reading the report html
                var table = rptViewr.find("div[id$='_oReportDiv']");
                if (table == undefined) {
                    alert("Report source not found.");
                    return;
                }

                // Generating a copy of the report in a new window
                var docCnt =  table.parent().html();
                var docHead = '<head><style>body{margin:0;padding:0; font-size: small;}</style></head>';
                var winAttr = "location=yes, statusbar=no, directories=no, menubar=no, titlebar=no, toolbar=no, dependent=no, resizable=yes, screenX=200, screenY=200, personalbar=no, scrollbars=yes";;
                var newWin = window.open("", "_blank", winAttr);
                writeDoc = newWin.document;
                writeDoc.open();
                writeDoc.write('<html>' + docHead + '<body onload="window.print();">' + docCnt + '</body></html>');
                writeDoc.close();

                newWin.focus();

            };
        </script>
        <div>--%>

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
           <iframe id="urIframe" runat="server" style="width:100%;Height:1200px"></iframe> 

            <rsweb:ReportViewer ID="rptViewr" Width="100%" ZoomMode="PageWidth" runat="server" Height="600px" ShowPrintButton="true" visible="false">
            </rsweb:ReportViewer>

      

    </form>

</body>
</html>
