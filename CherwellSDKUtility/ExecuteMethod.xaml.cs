using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using CherwellSDKUtility.Helpers;
using CherwellSDKUtility.Models;

namespace CherwellSDKUtility
{
    public partial class ExecuteMethodContainer
    {
        #region Properties

        public Models.ExecuteMethodContainer MethodContainer;
        public CherwellWebConnector WebConnector;

        #endregion

        #region Constructor

        public ExecuteMethodContainer()
        {
            InitializeComponent();
        }

        public ExecuteMethodContainer(MethodInfo method, string selectedApi, CherwellWebConnector webConnector) : this()
        {
            WebConnector = webConnector ?? new CherwellWebConnector();
            List<ExecuteMethodArgument> methodArguments = method.GetParameters().Select(x => new ExecuteMethodArgument { ArgumentName = x.Name }).ToList();
            List<string> methodParameters = (from ParameterInfo parameter in method.GetParameters() select parameter.Name).ToList();
            MethodContainer = new Models.ExecuteMethodContainer { SelectedApi = selectedApi, MethodName = method.Name, MethodArguments = methodArguments };

            IncidentExampleButton.Visibility = Visibility.Hidden;
            UserExampleButton.Visibility = Visibility.Hidden;
            ArgumentList.ItemsSource = methodParameters;
            ArgumentList.SelectedIndex = 0;
            MethodLabel.Content = string.Format("Execute Method: {0}", method.Name);
        }

        #endregion

        #region Private Methods

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ArgumentValue.Text = string.Empty;
            MethodResponse.Text = string.Empty;

            foreach (ExecuteMethodArgument argument in MethodContainer.MethodArguments)
            {
                argument.ArgumentValue = string.Empty;
            }

            ArgumentList.SelectedIndex = 0;
            ArgumentValue.Focus();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Example_Click(object sender, RoutedEventArgs e)
        {
            string example = (sender as Button).Name == "IncidentExampleButton" ? "Incident" : "User";
            string xml = File.ReadAllText(Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), string.Format("XML Content\\{0}.xml", example)));
            ArgumentValue.Text = xml;
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            foreach (ExecuteMethodArgument argument in MethodContainer.MethodArguments.Where(w => w.ArgumentName == MethodContainer.CurrentArgument))
                argument.ArgumentValue = ArgumentValue.Text;

            object response;

            if (MethodContainer.SelectedApi == Enums.Api.Full.ToString())
            {
                Type type = Assembly.GetExecutingAssembly().GetType("CherwellSDKUtility.Helpers.CherwellConnector");
                object instance = Activator.CreateInstance(type);
                instance = (CherwellConnector)(instance);

                string[] methodData = (from ExecuteMethodArgument argument in MethodContainer.MethodArguments select argument.ArgumentValue).ToArray();

                response = typeof(CherwellConnector).InvokeMember(
                    MethodContainer.MethodName,
                    BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                    null,
                    instance,
                    methodData);
            }
            else
            {
                string[] methodData = (from ExecuteMethodArgument argument in MethodContainer.MethodArguments select argument.ArgumentValue).ToArray();

                response = typeof(CherwellWebConnector).InvokeMember(
                    MethodContainer.MethodName,
                    BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                    null,
                    WebConnector,
                    methodData);
            }

            if (response != null)
                MethodResponse.Text = response.ToString();
        }

        private void SetAndStore(object sender, RoutedEventArgs e)
        {
            IncidentExampleButton.Visibility = Visibility.Hidden;
            UserExampleButton.Visibility = Visibility.Hidden;
            string selectedItem = (sender as ListBox).SelectedItem.ToString();

            foreach (ExecuteMethodArgument argument in MethodContainer.MethodArguments.Where(w => w.ArgumentName == MethodContainer.CurrentArgument))
            {
                argument.ArgumentValue = ArgumentValue.Text;
            }

            MethodContainer.CurrentArgument = selectedItem;

            if (selectedItem.Contains("Xml"))
            {
                IncidentExampleButton.Visibility = Visibility.Visible;
                UserExampleButton.Visibility = Visibility.Visible;
            }

            ArgumentValue.Text = MethodContainer.MethodArguments.FirstOrDefault(x => x.ArgumentName == selectedItem).ArgumentValue;
            ArgumentValue.Focus();
        }

        private void Summary_Click(object sender, RoutedEventArgs e)
        {
            if (MethodContainer.SelectedApi == Enums.Api.Full.ToString())
            {
                string methodSummary = SummaryInspector.XmlFromMember(typeof(CherwellConnector).GetMethod(MethodContainer.MethodName)).InnerXml;
                MethodResponse.Text = SummaryBuilder(methodSummary);
            }
            else
            {
                string methodSummary = SummaryInspector.XmlFromMember(typeof(CherwellWebConnector).GetMethod(MethodContainer.MethodName)).InnerXml;
                MethodResponse.Text = SummaryBuilder(methodSummary);
            }
        }

        private string SummaryBuilder(string methodSummary)
        {
            SummaryClass summary = SummaryClass.FromXmlString(string.Format("<inspector>{0}</inspector>", methodSummary));
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("Summary: {0}", summary.Summary));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            foreach (Param param in summary.Params)
            {
                sb.Append(String.Format("Parameter Name: {0}, Description: {1}", param.Name, param.Value));
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
            }

            sb.Append(String.Format("Returns: {0}", summary.Returns));

            return sb.ToString();
        }

        #endregion
    }
}