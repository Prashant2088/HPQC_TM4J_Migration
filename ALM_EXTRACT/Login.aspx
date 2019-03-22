<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ALM_EXTRACT.Login" %>

<!DOCTYPE html>

<html>
<head>
    <meta charset="utf-8">
    <title>HPALM to TM4J Import</title>

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

        /* ---------- LOGIN ---------- */
        #login {
            margin: 50px auto;
            width: 300px;
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
        }
    </style>
     <script src="Scripts/jquery-3.3.1.min.js"> </script>
</head>
<body >
   <div id="login">
        <h1><strong>Welcome.</strong> Please login to HP ALM.</h1>
        <form runat="server" id="loginForm" name="loginForm"  action="Login.aspx">
             
            <fieldset>
                <p>
                    <label for="ALM URL">HPQC URL</label>
                    <label for="exampleALM"style="font-style:italic; color:brown";">(http://inqc/qcbin/)</label>
                    <input type="text" id="txtALMUrl" runat="server" required value="" />
                </p>
                <p>
                    <label for="Username">HPQC Username</label>
                    <input type="text" id="txtUserName" runat="server" required value="" />
                    <%--<input type="text" id="txtUserName" runat="server" required value="Username" onblur="if(this.value=='')this.value='Username'" onfocus="if(this.value=='Username')this.value='' ">--%>
                </p>
                <p>
                    <label for="Password">HPQC Password</label>
                    <input type="password" id="txtPWD" runat="server" required value="" />
                    <%--<input type="password" id="txtPWD" runat="server" required value="Password" onblur="if(this.value=='Password')this.value=''" onfocus="if(this.value=='Password')this.value='' ">--%>
                </p>
                <p>
                    <label for="Domain">HPQC Domain Name</label>
                    <input type="text" id="txtDomain" runat="server" class ="uppercase" required value="" />
                </p>
                <p>
                    <label for="Project">HPQC Project Name</label>
                    <input type="text" id="txtProject" runat="server" class ="uppercase" required value="" />
                </p>
                <p>
                      <asp:Button ID="btnSubmit"  runat="server" Text="Login"
                OnClick="btnSubmit_Click" UseSubmitBehavior="false"
                  OnClientClick="enable(); this.value='Please wait....'" 
                style="width: 300px; margin-bottom: 20px; margin-top: 10px"/>

                </p>
            </fieldset>
                
            <p>
                <textarea rows="6" cols="70" id="lblTextError" runat="server" readonly="readonly" style="margin-left: -100px; margin-top: 25px;" form="loginForm" visible="false"></textarea>
            </p>
        <div runat="server" class="loader" id="loader" style="display: none; margin-left:105px; margin-top:-370px">
        <div class="center">
            <img alt="" src="Images/loader.png" />
        </div>
    </div>
    </form>
        </div>
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
                $("#login input").prop("readonly", true);
                $(":button").prop("disabled", true);
                $("#loader").show();
                $("#lblTextError").hide();
            }
        }

        window.onbeforeunload = function () {
            if (!changesSaved) {
                return "You haven't saved your changes.";
            }
        };

</script>
</html>

