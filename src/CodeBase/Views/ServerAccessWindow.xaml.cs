using System;
using System.Text;
using System.Windows;

namespace CodeBase
{
    public partial class ServerAccessWindow : Window
    {
        private readonly ApplicationData _data;
        private readonly Secure _secure;
        private readonly Action _onDone;
        private string _loggedInfoTemplate = "";

        public ServerAccessWindow(ApplicationData data, Action onDone)
        {
            _data = data;
            _secure = new Secure(data);
            _onDone = onDone;
            InitializeComponent();

            ReceiverURL.Text = data.ReceiverURL != "" ? data.ReceiverURL : App.DefaultReceiverURL;
            UserID.Text = data.UserID;

            Closed += (s, e) => onDone?.Invoke();

            //

            AutoUpdateCheckBox.IsChecked = data.AutoUpdate;
            SendDataCheckBox.IsChecked = data.SendData;
            UpdateIntervalTextBox.Text = data.UpdateInterval.ToString();

            AutoUpdateCheckBox.Click += (sender, e) =>
            {
                data.AutoUpdate = AutoUpdateCheckBox.IsChecked.Value;
            };
            SendDataCheckBox.Click += (sender, e) =>
            {
                data.SendData = SendDataCheckBox.IsChecked.Value;
            };
            UpdateIntervalTextBox.TextChanged += (sender, e) =>
            {
                if (int.TryParse(UpdateIntervalTextBox.Text, out var m))
                {
                    data.UpdateInterval = m;
                    UpdateIntervalTextBox.Text = m.ToString();
                }
            };

            //

            _secure.CheckLogin(logged =>
            {
                if (!logged) data.Token = "";
                WakeUp();
            });

            WakeUp();


        }


        public void WakeUp()
        {
            Login.Visibility = _secure.IsLogged ? Visibility.Hidden : Visibility.Visible;
            Logout.Visibility = !_secure.IsLogged ? Visibility.Hidden : Visibility.Visible;

            if (_loggedInfoTemplate == "") _loggedInfoTemplate = (string)LoggedInfo.Content;
            LoggedInfo.Content = string.Format(_loggedInfoTemplate, _data.UserID);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (ReceiverURL.Text != "" && UserID.Text != "" && Password.Password != "")
            {
                _data.ReceiverURL = ReceiverURL.Text;
                WebMethods.ReceiverURL = _data.ReceiverURL;

                WebMethods.Login(UserID.Text, Password.Password, res =>
                {
                    WebClient.ProcessResponse(res, data =>
                    {
                        if (data != "")
                        {
                            _data.UserID = UserID.Text;
                            _data.Token = data;
                        }
                        else
                        {
                            MessageHelper.Error("Wrong login or password", "Auth");
                        }

                        WakeUp();
                        _onDone?.Invoke();
                    });
                });
            }
            else
            {
                MessageHelper.Error("All fields should be filled!", "");
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _data.Token = "";
            WakeUp();
            _onDone?.Invoke();
        }
    }

    public class Secure
    {
        public ApplicationData _data;

        public Secure(ApplicationData data)
        {
            _data = data;
        }

        public bool IsLogged { get => _data.Token != ""; }

        public void CheckLogin(Action<bool> onDone)
        {
            WebMethods.CheckLogin(_data, result =>
            {
                WebClient.ProcessResponse(result, data =>
                {
                    var logged = data == "1";
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
