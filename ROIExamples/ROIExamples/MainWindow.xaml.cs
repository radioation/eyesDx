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
        
        static async Task<string> GetROIAsync(string uri, string command)
        {
            using (var client = new HttpClient())
            {
                string data = "";
                // connect the client to the GET URL ( http://<host>:<port> )
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();

                // GET something with    /roi   or  /roi/<roiName>  command 
                var response = await client.GetAsync(command);
                if (response.IsSuccessStatusCode)
                {
                    // we want to use the ROI XML returned from MAPPS 
                    data = await response.Content.ReadAsStringAsync();
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
            // Setup URL and command.  The get added up to be http://127.0.0.1:9495/roi  or http://127.0.0.1:9495/roi/{roiName}
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
            // perform DELETE asynchronously
            var task = DeleteROIAsync(uri, command);
            string result = await task;
            // display the result
            textBlock.Inlines.Add(result);
        }

        private async void getButton_Click(object sender, RoutedEventArgs e)
        {
            // Setup URL and command.  The get added up to be http://127.0.0.1:9495/roi  or http://127.0.0.1:9495/roi/{roiName}
            string uri = "http://" + hostTextBox.Text + ":" + portTextBox.Text;
            string command = "/roi";
            if (getNameTextBox.Text.Length > 0)
            {
                command = command + "/" + getNameTextBox.Text;
            }
            textBlock.Inlines.Add("--------------------------------------------------\n");
            textBlock.Inlines.Add(String.Format("Do GET: {0}\n", uri + command));

            // perform GET asynchronously
            var task = GetROIAsync(uri, command);
            await task;

            string rois = task.Result;
            // display the result
            textBlock.Inlines.Add(rois);
        }

        private void putButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
