using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CodeBase
{
    public partial class ServerAccessWindow : Window
    {
        private ApplicationData Data;
        private Secure Secure;
        private Action onDone;

        public ServerAccessWindow(ApplicationData Data, Action onDone)
        {
            this.Data = Data;
            this.Secure = new Secure(Data);
            this.onDone = onDone;
            InitializeComponent();

            ReceiverURL.Text = Data.ReceiverURL != "" ? Data.ReceiverURL : App.DefaultReceiverURL;
            UserID.Text = Data.UserID;

            Closed += (s,e) => onDone?.Invoke();

            //

            AutoUpdateCheckBox.IsChecked = Data.AutoUpdate;
            SendDataCheckBox.IsChecked = Data.SendData;
            UpdateIntervalTextBox.Text = Data.UpdateInterval.ToString();

            AutoUpdateCheckBox.Click += (sender, e) => {
                Data.AutoUpdate = AutoUpdateCheckBox.IsChecked.Value;
            };
            SendDataCheckBox.Click += (sender, e) => {
                Data.SendData = SendDataCheckBox.IsChecked.Value;
            };
            UpdateIntervalTextBox.TextChanged += (sender, e) => {
                if (int.TryParse(UpdateIntervalTextBox.Text, out int m))
                {
                    Data.UpdateInterval = m;
                    UpdateIntervalTextBox.Text = m.ToString();
                }
            };

            //

            Secure.CheckLogin(logged => {
                if (!logged) Data.Token = "";
                WakeUp();
            });

            WakeUp();


        }

        string loggedInfoTemplate = "";

        public void WakeUp()
        {
            Login.Visibility = Secure.IsLogged ? Visibility.Hidden : Visibility.Visible;
            Logout.Visibility = !Secure.IsLogged ? Visibility.Hidden : Visibility.Visible;

            if (loggedInfoTemplate == "") loggedInfoTemplate = (string)LoggedInfo.Content;
            LoggedInfo.Content = string.Format(loggedInfoTemplate, Data.UserID);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (ReceiverURL.Text != "" && UserID.Text != "" && Password.Password != "")
            {
                Data.ReceiverURL = ReceiverURL.Text;
                WebMethods.ReceiverURL = Data.ReceiverURL;

                WebMethods.Login(UserID.Text, Password.Password, res =>
                {
                    WebClient.ProcessResponse(res, data =>
                    {
                        if (data != "")
                        {
                            Data.UserID = UserID.Text;
                            Data.Token = data;
                        }
                        else
                        {
                            MainWindow.Error("Wrong login or password", "Auth");
                        }

                        WakeUp();
                        onDone?.Invoke();
                    });
                });
            }
            else
            {
                MainWindow.Error("All fields should be filled!", "");
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Data.Token = "";
            WakeUp();
            onDone?.Invoke();
        }
    }

    public class Secure
    {
        public ApplicationData Data;

        public Secure(ApplicationData Data)
        {
            this.Data = Data;
        }

        public bool IsLogged { get => Data.Token != ""; }

        public void CheckLogin(Action<bool> onDone)
        {
            WebMethods.CheckLogin(Data, result => {
                WebClient.ProcessResponse(result, data =>
                {
                    bool logged = data == "1";
                    onDone?.Invoke(logged);
                });
            });
        }

        // static

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
