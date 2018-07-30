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
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.Collections.ObjectModel;


using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Serialization;

namespace ROIExamples
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // setup shape list
        public ObservableCollection<string> shapeList = new ObservableCollection<string>();


        static async Task<string> DeleteROIAsync(string uri, string command)
        {
            using (var client = new HttpClient())
            {
                // connect the client to the DELETE URL ( http://<host>:<port> )
                string data = ""; 
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();

                // Send DELETE command to the server.   /roi   or  /roi/<roiName>
                var response = await client.DeleteAsync(command);
                if (response.IsSuccessStatusCode)
                {
                    data = "Success";
                }
                else
                {
                    data = "Server Error: " + response.StatusCode.ToString();
                }
                return data;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            // setup possible shapes
            shapeList.Add("Rectangle");
            shapeList.Add("Ellipse");
            shapeList.Add("Points");
            putShapeComboBox.ItemsSource = shapeList;
            putShapeComboBox.SelectedIndex = 0;
        }

        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Setup URL and command.  A typical deletion  http://127.0.0.1:9495/roi  or http://127.0.0.1:9495/roi/{roiName}
            string uri = "http://" + hostTextBox.Text + ":" + portTextBox.Text;
            string command = "/roi";
            if (deleteNameTextBox.Text.Length > 0)
            {
                command = command + "/" + deleteNameTextBox.Text;
            }
            else
            {
                if (MessageBox.Show("Delete all ROI?", "Delete All ROI", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    textBlock.Inlines.Add("--------------------------------------------------\n");
                    textBlock.Inlines.Add("Canceled DELETE: ");
                    return;
                }
            }
            textBlock.Inlines.Add("--------------------------------------------------\n");
            textBlock.Inlines.Add(String.Format("Do DELETE: {0}\n", uri + command));
            // perform delete asynchronously
            var task = DeleteROIAsync(uri, command);
            string result = await task;
        }

        private void getButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void putButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
