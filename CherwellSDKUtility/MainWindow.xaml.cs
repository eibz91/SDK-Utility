using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

using CherwellSDKUtility.Helpers;
using CherwellSDKUtility.Models;

using Visibility = System.Windows.Visibility;

namespace CherwellSDKUtility
{
    public partial class MainWindow
    {
        #region Properties

        public bool LoggedIn;
        public string SelectedApi;
        public CherwellWebConnector WebConnector;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        #endregion

        #region Private Methods

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectionText.Focus();
            LogoutButton.Visibility = Visibility.Hidden;
            RunButton.Visibility = Visibility.Hidden;
            PopulateMethodList();
            PopulateConnectionList();
            PopulateApiList();
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = Application.Current.MainWindow;
            mainWindow.IsEnabled = true;
            ResponsePopup.IsOpen = false;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            SelectedApi = SelectedApiList.SelectedItem.ToString();
            string connectionName = string.Format("[{0}]{1}", ConnectionList.SelectedItem, ConnectionText.Text);
            string username = UsernameText.Text;
            string password = PasswordBox.Password;

            if (!string.IsNullOrEmpty(connectionName) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    if (SelectedApi == Enums.Api.Full.ToString())
                    {

                        CherwellConnector cherwell = new CherwellConnector();
                        LoggedIn = cherwell.Login(username, password, connectionName);
                        PopulateMethodList();
                    }
                    else
                    {
                        WebConnector = new CherwellWebConnector();
                        LoggedIn = WebConnector.Login(username, password);
                        PopulateMethodList();
                    }
                }
                catch (Exception)
                {
                    Window mainWindow = Application.Current.MainWindow;
                    mainWindow.IsEnabled = false;
                    ResponseLabel.Text = string.Format("Could not connect to {0}. The connection definition could not be found.", connectionName);
                    ResponsePopup.IsOpen = true;
                    return;
                }
            }

            if (!LoggedIn)
            {
                Window mainWindow = Application.Current.MainWindow;
                mainWindow.IsEnabled = false;
                ResponseLabel.Text = string.Format("Login failed, username or password are incorrect!");
                ResponsePopup.IsOpen = true;
            }
            else
                DisableLoginFields();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedApi == Enums.Api.Full.ToString())
            {
                CherwellConnector cherwell = new CherwellConnector();
                LoggedIn = cherwell.Logout();
                PopulateMethodList();
            }
            else
            {
                LoggedIn = WebConnector.Logout();
                PopulateMethodList();
            }

            if (!LoggedIn)
                EnableLoginFields();
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            string selectedMethod = MethodsList.SelectedValue.ToString();
            Window mainWindow = Application.Current.MainWindow;

            if (SelectedApi == Enums.Api.Full.ToString())
            {
                MethodInfo methodInfo = typeof(CherwellConnector).GetMethod(selectedMethod);
                ExecuteMethodContainer executeMethod = new ExecuteMethodContainer(methodInfo, SelectedApi, WebConnector)
                {
                    Top = mainWindow.Top + 20,
                    Left = mainWindow.Left + 20
                };

                executeMethod.ShowDialog();
            }
            else
            {
                MethodInfo methodInfo = typeof(CherwellWebConnector).GetMethod(selectedMethod);
                ExecuteMethodContainer executeMethod = new ExecuteMethodContainer(methodInfo, SelectedApi, WebConnector)
                {
                    Top = mainWindow.Top + 20,
                    Left = mainWindow.Left + 20
                };

                executeMethod.ShowDialog();
            }
        }

        private void SelectedAPI_Changed(object sender, RoutedEventArgs e)
        {
            if (SelectedApiList.SelectedValue == null) return;
            if (SelectedApiList.SelectedValue.ToString() == Enums.Api.Web.ToString())
            {
                DisableConnectionFields();
                UsernameText.Focus();
            }
            else
            {
                EnableConnectionFields();
                ConnectionText.Focus();
            }
        }

        private void EnableConnectionFields()
        {
            PopulateConnectionList();
            ConnectionText.Text = string.Empty;
            ConnectionList.IsEnabled = true;
            ConnectionText.IsEnabled = true;
        }

        private void EnableLoginFields()
        {
            PopulateApiList();
            EnableConnectionFields();
            UsernameText.Text = string.Empty;
            PasswordBox.Password = string.Empty;

            SelectedApiList.IsEnabled = true;
            UsernameText.IsEnabled = true;
            PasswordBox.IsEnabled = true;

            LoginButton.Visibility = Visibility.Visible;
            LogoutButton.Visibility = Visibility.Hidden;
        }

        private void DisableConnectionFields()
        {
            ConnectionList.SelectedIndex = 0;
            ConnectionText.Text = string.Empty;
            ConnectionList.IsEnabled = false;
            ConnectionText.IsEnabled = false;
        }

        private void DisableLoginFields()
        {
            PopulateApiList();
            PopulateConnectionList();
            DisableConnectionFields();
            UsernameText.Text = string.Empty;
            PasswordBox.Password = string.Empty;

            SelectedApiList.IsEnabled = false;
            UsernameText.IsEnabled = false;
            PasswordBox.IsEnabled = false;

            LoginButton.Visibility = Visibility.Hidden;
            LogoutButton.Visibility = Visibility.Visible;
        }

        private void PopulateApiList()
        {
            if (!LoggedIn)
            {
                var apiList = Enum.GetValues(typeof(Enums.Api)).Cast<Enums.Api>().ToList();
                SelectedApiList.ItemsSource = apiList;
                SelectedApiList.SelectedItem = apiList.First();
            }
            else
            {
                SelectedApiList.ItemsSource = new List<string>();
            }
        }

        private void PopulateConnectionList()
        {
            if (!LoggedIn)
            {
                var connectionList = Enum.GetValues(typeof(Enums.Connection)).Cast<Enums.Connection>().ToList();
                ConnectionList.ItemsSource = connectionList;
                ConnectionList.SelectedItem = connectionList.First();
            }
            else
            {
                ConnectionList.ItemsSource = new List<string>();
            }
        }

        private void PopulateMethodList()
        {
            if (LoggedIn)
            {
                if (SelectedApi == Enums.Api.Full.ToString())
                {
                    var methods = typeof(CherwellConnector).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                    List<string> methodList = (from MethodInfo method in methods select method.Name).ToList();
                    MethodsList.ItemsSource = methodList;
                }
                else
                {
                    var methods = typeof(CherwellWebConnector).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                    List<string> methodList = (from MethodInfo method in methods select method.Name).ToList();
                    MethodsList.ItemsSource = methodList;
                }

                RunButton.Visibility = Visibility.Visible;
            }
            else
            {
                MethodsList.ItemsSource = new List<string>();
                RunButton.Visibility = Visibility.Hidden;
            }
        }

        #endregion
    }
}
