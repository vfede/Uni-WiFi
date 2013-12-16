using HtmlAgilityPack;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Sapienza_WiFi.DataStorage;
using Sapienza_WiFi.l10n;
using Sapienza_WiFi.Views;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Sapienza_WiFi
{
    public partial class MainPage : BasePage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        public void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Retrieve the Username and Password from isolated storage.
            byte[] ProtectedUsernameByte = (new PersistentDataStorage()).Restore<byte[]>("Username");
            byte[] ProtectedPasswordByte = (new PersistentDataStorage()).Restore<byte[]>("Password");

            byte[] UsernameByte = null;
            byte[] PasswordByte = null;

            if (ProtectedUsernameByte != null)
            {
                // Decrypt the Username and Password by using the Unprotect method.
                UsernameByte = ProtectedData.Unprotect(ProtectedUsernameByte, null);
                // Convert the Username and Password from byte to string and display it in the text box.
                Username.Text = Encoding.UTF8.GetString(UsernameByte, 0, UsernameByte.Length);
            }
            if (ProtectedPasswordByte != null)
            {
                PasswordByte = ProtectedData.Unprotect(ProtectedPasswordByte, null);
                Password.Password = Encoding.UTF8.GetString(PasswordByte, 0, PasswordByte.Length);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DeviceNetworkInformation.IsWiFiEnabled)
            {
                ToggleWifi.Content = AppResources.DISABLE_WIFI;
            }
            else ToggleWifi.Content = AppResources.ENABLE_WIFI;

            if (DeviceNetworkInformation.IsCellularDataEnabled)
            {
                Toggle3G.Content = AppResources.DISABLE_3G;
            }
            else Toggle3G.Content = AppResources.ENABLE_3G;
        }

        private void ToggleWifi_Click(object sender, RoutedEventArgs e)
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.WiFi;
            connectionSettingsTask.Show();
        }

        private void Toggle3G_Click(object sender, RoutedEventArgs e)
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.Cellular;
            connectionSettingsTask.Show();
        }

        void webClient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            webClient.UploadStringCompleted -= webClient_UploadStringCompleted;
            string result = e.Result;

            if (e.Result == null)
                LoggedInText.Text = AppResources.ERROR;
            else
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(e.Result);
                HtmlNode div = doc.GetElementbyId("user");

                if (div == null)
                    LoggedInText.Text = AppResources.CONNECTED;
                else
                {
                    //<input type="hidden" name="error" value="Invalid name or password" />
                    string error = doc.DocumentNode.SelectNodes("//input[@name=\"error\"]")[0].GetAttributeValue("value", "null");

                    switch (error)
                    {
                        case "You are already logged in":
                            LoggedInText.Text = AppResources.ALREADY_LOGGEDIN;
                            break;
                        case "Invalid name or password":
                            LoggedInText.Text = AppResources.WRONG_CREDENTIALS;
                            break;
                        case "You have attempted the maximum number of login attempts.  Please wait 5 minute(s) to try again.":
                            LoggedInText.Text = AppResources.MAX_ATTEMPTS_5;
                            break;
                        case "You have attempted the maximum number of login attempts.  Please wait 1 minute(s) to try again.":
                            LoggedInText.Text = AppResources.MAX_ATTEMPTS_1;
                            break;
                        case "This user already logged in from another computer, only &#39;1&#39; logins per user allowed.":
                            LoggedInText.Text = AppResources.LOGGED_FROM_ANOTHER1;
                            break;
                    }
                }
            }
            ProgressBar.IsIndeterminate = false;
            this.UIElement_LoadAnimation(LoggedInText, null);
        }

        WebClient webClient;

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!DeviceNetworkInformation.IsWiFiEnabled)
            {
                MessageBoxResult result = MessageBox.Show(AppResources.WIFI_OFF_MESSAGE, AppResources.WIFI_OFF, MessageBoxButton.OK);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
                        connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.WiFi;
                        connectionSettingsTask.Show();
                        return;
                }
            }
            else
            {
                bool found = false;
                NetworkInterfaceList netList = new NetworkInterfaceList();
                foreach (NetworkInterfaceInfo network in netList)
                {
                    if (network.InterfaceType == NetworkInterfaceType.Wireless80211
                        && network.InterfaceState == ConnectState.Connected
                        && network.InterfaceName.StartsWith("sapienza"))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    MessageBox.Show(AppResources.WRONG_WIFI_MESSAGE, AppResources.WRONG_WIFI, MessageBoxButton.OK);
                    return;
                }
            }
            LoggedInText.Text = String.Empty;
            ProgressBar.Visibility = Visibility.Visible;
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var uri = new Uri("https://wifigw.uniroma1.it/login.pl", UriKind.Absolute);
            StringBuilder postData = new StringBuilder();
            postData.AppendFormat("{0}={1}", "bs_name", HttpUtility.UrlEncode(Username.Text));
            postData.AppendFormat("&{0}={1}", "bs_password", HttpUtility.UrlEncode(Password.Password));
            postData.AppendFormat("&{0}={1}", "_FORM_SUBMIT", HttpUtility.UrlEncode("1"));
            postData.AppendFormat("&{0}={1}", "which_form", HttpUtility.UrlEncode("reg"));

            webClient.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();
            webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(webClient_UploadStringCompleted);

            try
            {
                webClient.UploadStringAsync(uri, "POST", postData.ToString());
            }
            catch (Exception)
            {
                webClient.UploadStringCompleted -= webClient_UploadStringCompleted;
                MessageBox.Show(AppResources.WEB_ERROR_MESSAGE, AppResources.WEB_ERROR, MessageBoxButton.OK);
            }
        }

        private void ApplicationBarMenuItem_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void Username_LostFocus(object sender, RoutedEventArgs e)
        {
            // Convert the Username to a byte[].
            byte[] UsernameByte = Encoding.UTF8.GetBytes(Username.Text);

            // Encrypt the Username by using the Protect() method.
            byte[] ProtectedUsernameByte = ProtectedData.Protect(UsernameByte, null);

            // Store the encrypted Username in isolated storage.
            (new PersistentDataStorage()).Backup("Username", ProtectedUsernameByte);
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            byte[] PasswordByte = Encoding.UTF8.GetBytes(Password.Password);
            byte[] ProtectedPasswordByte = ProtectedData.Protect(PasswordByte, null);
            (new PersistentDataStorage()).Backup("Password", ProtectedPasswordByte);
        }

        protected void UIElement_LoadAnimation(object sender, RoutedEventArgs e)
        {
            (new FadeInTransition()).GetTransition((UIElement)sender).Begin();
        }

        private void Username_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Password.Focus();
        }

        private void Password_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login.Focus();
                this.Login_Click(sender, null);
            }
        }
    }
}