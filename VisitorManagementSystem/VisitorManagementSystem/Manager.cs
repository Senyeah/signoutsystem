using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Threading;

namespace VisitorManagementSystem {
    public enum CommunicationStatusCodes {
        RenewalTooSlow = 0,
        ClientNotAuthorised = -1,
        AuthenticatedTwice = -2,
        NoSessionToRenew = -3,
        IncorrectRenewalID = -4
    }

    public class Manager {
        private bool isConnected;

        private string sessionIdentifier;
        private DispatcherTimer ttlTimer;

        static readonly string hostname = "mail.garincollege.ac.nz";
        static readonly string baseURL = "/managementsystem/visitor";
        
        public Manager() {
            isConnected = false;
            sessionIdentifier = "";
        }

        private void ExecuteOneWayRequest(string destination) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + Manager.hostname + Manager.baseURL + "/" + destination);
            request.Method = "GET";
            request.GetResponse();
        }

        private void logEvent(string eventString, bool severe) {
            string requestString = String.Format("logEvent.php?client={0}&message={1}&severe={2}", Uri.EscapeDataString(Environment.MachineName), Uri.EscapeDataString(eventString), severe.ToString().ToLower());
            ExecuteOneWayRequest(requestString);
        }

        public bool authenticateVisitor(string name, string organisation, string toSee, DateTime approximateLeaveTime) {
            string leaveTimeTimestamp = (approximateLeaveTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds.ToString();

            string requestString = String.Format("authenticatePerson.php?name={0}&organisation={1}&outTime={2}&toSee={3}&client={4}&sessionID={5}", Uri.EscapeDataString(name), 
                                                                                                                                                    Uri.EscapeDataString(organisation),
                                                                                                                                                    Uri.EscapeDataString(leaveTimeTimestamp),
                                                                                                                                                    Uri.EscapeDataString(toSee),
                                                                                                                                                    Uri.EscapeDataString(Environment.MachineName), 
                                                                                                                                                    Uri.EscapeDataString(this.sessionIdentifier));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + Manager.hostname + Manager.baseURL + "/" + requestString);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();

            byte[] responseBytes = new byte[1024];
            int bytesRead = stream.Read(responseBytes, 0, 1024);

            string stringResponse = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);

            string[] newLineComponents = new string[] { "\r\n" };
            string identifier = stringResponse.Split(newLineComponents, StringSplitOptions.RemoveEmptyEntries).Last();

            stream.Close();

            if (Convert.ToInt32(identifier) == 1) {
                logEvent(String.Format("Visitor '{0}' signed in successfully.", name), false);
                return true;
            } else {
                logEvent(String.Format("Authentication for visitor '{0}' failed with code {1}. Disabling system.", name, identifier.ToString()), true);
                closeSession();

                ((MainWindow)Application.Current.MainWindow).disableSystem();
                return false;
            }
        }

        public void printVisitorPass(string name, DateTime leaveTime) {

            string desktopLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //Edit a copy of the file rather than the real thing
            System.IO.File.Copy(@"C:\Templates\label-template.doc", desktopLocation + @"\label-temp.doc");

            Microsoft.Office.Interop.Word.Application wordInstance = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document document = wordInstance.Documents.Open(desktopLocation + @"\label-temp.doc");

            wordInstance.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsNone;

            document.Range().Find.Execute(FindText: "[name]", ReplaceWith: name.ToUpper());

            DateTime newTime = leaveTime.AddHours(1);

            string date = newTime.ToString("d MMM");
            string time = newTime.ToString("h:mm tt").ToLower();

            document.Range().Find.Execute(FindText: "[date]", ReplaceWith: date);
            document.Range().Find.Execute(FindText: "[time]", ReplaceWith: time);

            document.PrintOut();

            while (wordInstance.BackgroundPrintingStatus > 0) {
                //Synchronously wait until all documents have printed before
                //closing the document
            }

            ((Microsoft.Office.Interop.Word._Document)document).Close(false);
            System.IO.File.Delete(desktopLocation + @"\label-temp.doc");
        }

        public void closeSession() {
            if (this.ttlTimer != null) {
                this.ttlTimer.Stop();
            }

            this.isConnected = false;
            
            string requestString = String.Format("sessionClose.php?client={0}", Uri.EscapeDataString(Environment.MachineName));
            ExecuteOneWayRequest(requestString);
        }
        
        public void renewSession(bool newSession = false) {
            string requestString = String.Empty;
            
            if (newSession) {
                requestString = String.Format("sessionAuthenticate.php?client={0}", Uri.EscapeDataString(Environment.MachineName));
            } else {
                requestString = String.Format("sessionRenew.php?client={0}&oldID={1}", Uri.EscapeDataString(Environment.MachineName), Uri.EscapeDataString(this.sessionIdentifier));
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + Manager.hostname + Manager.baseURL + "/" + requestString);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();

            byte[] responseBytes = new byte[1024];
            int bytesRead = stream.Read(responseBytes, 0, 1024);

            string identifier = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);

            //If the returned code is not a valid MD5 hash
            if (!System.Text.RegularExpressions.Regex.IsMatch(identifier, "[a-z0-9]{32}")) {
                int numericalID = Convert.ToInt32(identifier);

                switch (numericalID) {
                    case (int)CommunicationStatusCodes.RenewalTooSlow:
                        logEvent("[WARNING] Session ID renewal too slow (> 5s). Check the network connection!", false);

                        renewSession();
                        ttlTimer.Stop();
                        ttlTimer.Start();

                        break;

                    case (int)CommunicationStatusCodes.ClientNotAuthorised:
                        //This is a critical error. We need to disable this system until it's fixed.
                        logEvent("Client is not authorised to communicate with the server! Did the system name change?", true);
                        closeSession();

                        ((MainWindow)Application.Current.MainWindow).disableSystem();
                        break;

                    case (int)CommunicationStatusCodes.AuthenticatedTwice:
                        //This could be a test gone wrong
                        logEvent("The client authenticated twice! Reauthenticating.", false);

                        closeSession();
                        renewSession(true);

                        break;

                    case (int)CommunicationStatusCodes.NoSessionToRenew:
                        //Start from scratch
                        logEvent("The session became corrupt. Starting from scratch.", false);

                        closeSession();
                        renewSession(true);

                        break;

                    case (int)CommunicationStatusCodes.IncorrectRenewalID:
                        //We should probably log an error like this
                        logEvent(String.Format("The old session ID '{0}' is not valid. Restarting the session.", this.sessionIdentifier), true);

                        closeSession();
                        renewSession(true);

                        break;

                    default:
                        logEvent(String.Format("An unknown error occurred. The response was '{0}'", identifier), true);
                        closeSession();

                        ((MainWindow)Application.Current.MainWindow).disableSystem();
                        break;
                }
            } else {
                this.sessionIdentifier = identifier;
                ((MainWindow)Application.Current.MainWindow).SessionIDLabel.Content = String.Format("Session ID: {0}", this.sessionIdentifier);

                this.isConnected = System.Text.RegularExpressions.Regex.IsMatch(this.sessionIdentifier, "[a-z0-9]{32}");
            }  
        }

        private IEnumerable<string> analyseHTMLHeader(string header, string value) {
            string[] splitComponents = new string[] { "\r\n" };

            string[] components = header.Split(splitComponents, StringSplitOptions.RemoveEmptyEntries);

            foreach (string headerComponent in components) {
                string[] innerComponents = headerComponent.Split(':');

                if (headerComponent.ToLower().Contains(value.ToLower())) {
                    yield return headerComponent.Substring(value.Length + 1);
                }
            }
        }

        public void connectToServer() {
            this.renewSession(true);

            ttlTimer = new DispatcherTimer();
            ttlTimer.Interval = new TimeSpan(0, 0, 0, 59, 950);

            ttlTimer.Tick += delegate(object s, EventArgs e) {
                this.renewSession();
            };

            ttlTimer.Start();
        }
    }
}
