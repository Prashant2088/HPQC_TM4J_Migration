using System;
using TDAPIOLELib;

namespace ALM_EXTRACT
{
    public class ALM_Core
    {
        //Object will hold the connection 

        public TDConnection tDConnection = new TDConnection();

        private String ALMServerURL = "";

        /// <summary>
        /// Login to ALM
        /// <para/>true if successfull
        /// </summary>
        /// <param name="URL">ALM URL this should end with QCBin</param>
        /// <param name="UserName">ALM Username</param>
        /// <param name="Password">ALM Password</param>
        /// <param name="Domain">ALM Domain name</param>
        /// <param name="Project">ALM Project Name</param>
        /// <returns>true if successfull</returns>
        public TDConnection LoginALM(String URL, String UserName, String Password, String Domain, String Project)
        {
            try
            {
                ALMServerURL = URL;

                //Check if OTA Client is registered
                if (!IsOTARegistered())
                    throw (new Exception("OTA Client is Not Registered on the machine"));

                // configure connection to add Basic Auth Header at first request . Don't popup the header.
                tDConnection.SetBasicAuthHeaderMode(TDAPI_BASIC_AUTH_HEADER_MODES.HEADER_MODE_DO_NOT_ADD);

                tDConnection.InitConnectionEx(ALMServerURL);

                if (tDConnection.Equals(null))
                    throw (new Exception("Unable to initiate connection with ALM Server"));
                else
                {
                    tDConnection.Login(UserName, Password);

                    if (tDConnection.LoggedIn == false)
                        throw (new Exception("Unable to login to ALM"));
                }

                tDConnection.Connect(Domain, Project);

                if (!(tDConnection.ProjectConnected))
                    throw (new Exception("Unable to Connect to the project"));

                return tDConnection;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Logout ALM. This should not be call except in the scenario where you want to switch projects in between executions. This will be autometically called at the end of executions.
        /// <para/>true if successfull
        /// </summary>
        /// <returns>true if successfull</returns>
        public Boolean LogoutALM(TDConnection otaConnection)
        {
            if (otaConnection.ProjectConnected == true)
                otaConnection.DisconnectProject();

            if (otaConnection.LoggedIn == true)
                otaConnection.Logout();

            tDConnection.ReleaseConnection();
            return true;
        }

        /// <summary>
        /// Registering OTA Client in the Machine 
        /// </summary>
        /// <returns></returns>
        public Boolean IsOTARegistered()
        {
            using (var classesRootKey = Microsoft.Win32.RegistryKey.OpenBaseKey(
                   Microsoft.Win32.RegistryHive.ClassesRoot, Microsoft.Win32.RegistryView.Default))
            {
                const string clsid = "{C5CBD7B2-490C-45f5-8C40-B8C3D108E6D7}";

                var clsIdKey = classesRootKey.OpenSubKey(@"Wow6432Node\CLSID\" + clsid) ??
                                classesRootKey.OpenSubKey(@"CLSID\" + clsid);

                if (clsIdKey != null)
                {
                    clsIdKey.Dispose();
                    return true;
                }

                return false;
            }
        }
    }
}