<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Utility.aspx.cs" Inherits="ALM_EXTRACT.Utility" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Jira Import</title>
    <style type="text/css">
        body {
            background-color: #f4f4f4;
            color: #5a5656;
            font-family: 'Open Sans', Arial, Helvetica, sans-serif;
            font-size: 16px;
            line-height: 1.5em;
        }

        a {
            text-decoration: none;
        }

        h1 {
            font-size: 1em;
        }

        h1, p {
            margin-bottom: 10px;
        }

        strong {
            font-weight: bold;
        }

        .uppercase {
            text-transform: uppercase;
        }

        form fieldset input[type="text"], input[type="password"] {
            background-color: #e5e5e5;
            border: none;
            border-radius: 3px;
            -moz-border-radius: 3px;
            -webkit-border-radius: 3px;
            color: #5a5656;
            font-family: 'Open Sans', Arial, Helvetica, sans-serif;
            font-size: 14px;
            height: 50px;
            outline: none;
            padding: 0px 10px;
            width: 280px;
            -webkit-appearance: none;
        }

        form fieldset input[type="submit"] {
            background-color: #008dde;
            border: none;
            border-radius: 3px;
            -moz-border-radius: 3px;
            -webkit-border-radius: 3px;
            color: #f4f4f4;
            cursor: pointer;
            font-family: 'Open Sans', Arial, Helvetica, sans-serif;
            height: 50px;
            text-transform: uppercase;
            width: 300px;
            -webkit-appearance: none;
        }

        form fieldset a {
            color: #5a5656;
            font-size: 10px;
        }

            form fieldset a:hover {
                text-decoration: underline;
            }

        .btn-round {
            background-color: #5a5656;
            border-radius: 50%;
            -moz-border-radius: 50%;
            -webkit-border-radius: 50%;
            color: #f4f4f4;
            display: block;
            font-size: 12px;
            height: 50px;
            line-height: 50px;
            margin: 30px 125px;
            text-align: center;
            text-transform: uppercase;
            width: 50px;
        }

        form fieldset input[type="button"] {
            background-color: #008dde;
            border: none;
            border-radius: 3px;
            -moz-border-radius: 3px;
            -webkit-border-radius: 3px;
            color: #f4f4f4;
            cursor: pointer;
            font-family: 'Open Sans', Arial, Helvetica, sans-serif;
            height: 50px;
            text-transform: uppercase;
            width: 270px;
            -webkit-appearance: none;
            /*margin-left: 50px;*/
        }

        /* ---------- UTILITY ---------- */
        #Utility {
            margin: 350px;
            margin-bottom: 10px;
            margin-top: 10px;
            width: 300px;
        }
    </style>
    <script src="Scripts/jquery-3.3.1.min.js"> </script>
</head>
<body>
    <form runat="server" id="UtilityForm" name="UtilityForm">
        <div id="Utility">
            <h1>&nbsp &nbsp &nbsp  Please Enter the Required Field.</h1>

            <fieldset>
                <p>
                    <label for="Jira URL">Jira URL</label>
                   <label for="exampleJira"style="font-style:italic; color:brown";">(https://DomainName/)</label>
                    <input type="text" id="txtJiraUrl" required value="" runat="server" />
                </p>

                <p>
                    <label for="Jira Username">Jira Username</label>
                    <input type="text" id="txtUserName" runat="server" required value="" />
                </p>
                <p>
                    <label for="Jira Password">Jira Password</label>
                    <input type="password" id="txtPWD" runat="server" required value="" />
                </p>
                <p>
                    <label for="Project Name">Project Name</label>
                    <label for="exampleJira"style="font-style:italic; color:brown";">(Ex:Vortex)</label>
                    <input type="text" id="txtProject" runat="server" required value="" />
                </p>
                <p>
                    <label for="Jda User Email ID">Jda Email ID</label>
                     <label for="exampleJira" style="font-style:italic; color:brown";">(Multiple email alllowed)</label>
                    <input type="text" id="txtEmail" runat="server" required value="" onkeypress="return IsAlphaNumeric(event);" />
                    <span id="lblEmailerror" style="color: Red; display: none">* Only allowed character (@ , .)</span>
                </p>
                <p>
                    <label for="Attachment Path">Attachment Path</label>
                    <label for="exampleJira" style="font-style:italic; color:brown";">(Ex:C:\Downloads)</label>
                    <input type="text" id="txtAttachmentPath" runat="server" required value="" />
                </p>
            </fieldset>

            <fieldset style="margin-left: 359px; margin-top: -569px;background-color:aquamarine;">

               <asp:Button ID="btnFind_TM4J_Dplicate_Test" runat="server" Text="Find_Duplicate_TM4J_TEST_Name"
                OnClick="btnFind_TM4J_Dplicate_Test_Click" UseSubmitBehavior="false"
                OnClientClick="enable(); this.value='Please wait....'" 
                style="margin-left: 12px; margin-bottom: 20px; margin-top: 5px"/>

               <asp:Button ID="btnDelete_TM4J_Dplicate_Test" runat="server" Text="Delete_Duplicate_TM4J_TEST"
                OnClick="btnDelete_TM4J_Dplicate_Test_Click" UseSubmitBehavior="false"
                OnClientClick="enable(); this.value='Please wait....'" 
                style="margin-left: 12px; margin-bottom: 20px; margin-top: -7px"/>

                <asp:Button ID="btnImport_Tm4J" runat="server" Text="HPQC_TM4J_Import"
                OnClick="btnImport_Tm4J_Click" UseSubmitBehavior="false"
                 OnClientClick="enable(); this.value='Please wait....'"
                style="margin-left: 12px; margin-bottom: 20px; margin-top: -7px; background-color:crimson"/>

                <asp:Button ID="btn_Process_Error_Test_Import_Tm4J" runat="server" Text="Process_Error_Test"
                OnClick="btn_Process_Error_Test_Import_Tm4J_Click" UseSubmitBehavior="false"
                 OnClientClick="enable(); this.value='Please wait....'"
                style="margin-left: 12px; margin-bottom: 20px; margin-top: -7px"/>

             <asp:Button ID="btnLogout"  runat="server" Text="Logout-HPQC_TM4J"
                OnClick="btnLogout_Click" UseSubmitBehavior="false"
             OnClientClick="enable(); this.value='Please wait....'" 
                style="margin-left: 12px; margin-bottom: 20px; margin-top: -7px"/>
       </fieldset>
           
             <fieldset style="margin-left: 700px; margin-top: -347px;  background-color:lemonchiffon;">

            <asp:Button ID="btnWorksheet" runat="server" Text="DownLoad_HPQCTest_Report"
                OnClick="btnworksheet_Click" UseSubmitBehavior="false"
                OnClientClick="enable(); this.value='Please wait....'" 
                style="margin-left: 12px; margin-bottom: 20px; margin-top: 3px"/>

            <asp:Button ID="btnDwnldAttachments" runat="server" Text="Download Attachments"
                OnClick="btnDwnldAttchmnts_Click" UseSubmitBehavior="false"
               OnClientClick="enable(); this.value='Please wait....'"
                style="margin-left: 12px; margin-bottom: 20px; margin-top: -7px"/>

             <asp:Button ID="btnDwnldDesignAttachments" runat="server" Text="Download Design Attachments"
                OnClick="btnDwnldDsgnsAttchmnts_Click" UseSubmitBehavior="false"
               OnClientClick="enable(); this.value='Please wait....'" 
                style="margin-left: 12px; margin-bottom: 20px; margin-top: -7px"/>

            
             <asp:Button ID="btnHPQCTestCount" runat="server" Text="Distinct HPQC Test Count"
                OnClick="btnHPQCTestCount_Click" UseSubmitBehavior="false"
             OnClientClick="enable(); this.value='Please wait....'" 
                style="margin-left: 12px; margin-bottom: 20px; margin-top: -7px"/>
                
              <asp:Button ID="btnGetCustomField" runat="server" Text="Get HPQC Custom Field Name"
                OnClick="btnGetCustomField_Click" UseSubmitBehavior="false"
             OnClientClick="enable(); this.value='Please wait....'" 
                style="margin-left: 12px; margin-bottom: 20px; margin-top: -7px;"/>
       </fieldset>
        </div>

        <p>
            <textarea rows="6" cols="70" id="lblTextError" runat="server"  readonly="readonly" style="margin-left: 509px; margin-top: 220px;" form="UtilityForm" visible="false"></textarea>
        </p>
           <div runat="server" class="loader" id="loader" style="display: none; margin-left:635px; margin-top:-180px">
        <div class="center">
            <img alt="" src="Images/loader.png" />
        </div>
    </div>
    </form>
</body>
     <script>
         var changesSaved = false;
         function enable() {
             changesSaved = true;
             var isValid = true;
             $('input[type="text"]').each(function () {
                 if ($.trim($(this).val()) === '') {
                     isValid = false;
                     $(this).css({
                         "border": "1px solid red",
                         "background": "#FFCECE"
                     });
                 }
                 else {
                     $(this).css({

                         "border": "",
                         "background": ""
                     });
                 }
             });
             if (isValid == false) {
             }
             else {
                 $("#Utility input").prop("readonly", true);
                 $(":button").prop("disabled", true);
                 $("#loader").show();
                 $("#lblTextError").hide();
             }
         }
         window.onbeforeunload = function () {
             if (!changesSaved) {
                 return "You haven't saved your changes";
             }
         };

         var specialKeys = new Array();
         specialKeys.push(8);  //Backspace
         specialKeys.push(9);  //Tab
         specialKeys.push(46); //Delete
         specialKeys.push(127);  //Tab
         specialKeys.push(64); //@
         specialKeys.push(44); //,
         specialKeys.push(59); //;

         function IsAlphaNumeric(e) {
             var keyCode = e.keyCode == 0 ? e.charCode : e.keyCode;
             var ret = ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (specialKeys.indexOf(e.keyCode) != -1 && e.charCode == e.keyCode));
             document.getElementById("lblEmailerror").style.display = ret ? "none" : "inline";
             return ret;
         }
</script>
</html>

