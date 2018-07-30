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

        static async Task PutROIAsync(string uri, string command)
        {
            using (var client = new HttpClient())
            {
                // connect the client to the PUT URL ( http://<host>:<port> )
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();

                // PUT something with    /roi/<roiName>   or  /roi/<roiName>/frame  command 
                HttpContent content = null;
                var response = await client.PutAsync(command, content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success");
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
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

        private async void putButton_Click(object sender, RoutedEventArgs e)
        {
            // setup URL
            string uri = "http://" + hostTextBox.Text + ":" + portTextBox.Text;

            // build command string based on properties.
            string command = "/roi"; 

            if (putShapeComboBox.Text == "Ellipse" || putShapeComboBox.Text == "Rectangle")
            {
                if (putXTextBox.Text.Length == 0 || putYTextBox.Text.Length == 0 ||
                    putWTextBox.Text.Length == 0 || putHTextBox.Text.Length == 0 ||
                    putNameTextBox.Text.Length == 0)
                {
                    MessageBox.Show("Name, X, Y, W and H must be set", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }


                command = command + "/" + putNameTextBox.Text;

                if (putDelayTextBox.Text.Length > 0)
                    command += "/frame";
                if( putShapeComboBox.Text == "Rectangle")
                {
                    command += "?shape=rectangular";
                } else
                {
                    command += "?shape=ellipse";
                }
                command += "&x=" + putXTextBox.Text;
                command += "&y=" + putYTextBox.Text;
                command += "&w=" + putWTextBox.Text;
                command += "&h=" + putHTextBox.Text;
            }
            else
            {
                if (putPointsTextBox.Text.Length == 0 ||
                    putNameTextBox.Text.Length == 0)
                {
                    MessageBox.Show("Name and Points string must be set", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                command = command + "/" + putNameTextBox.Text;
                if (putDelayTextBox.Text.Length > 0)
                    command += "/frame";
                command += "?shape=points";
                command += "&points=" + putPointsTextBox.Text;
            }

            // send a 0 if needed
            if (putDelayTextBox.Text.Length > 0)
            {
                try
                {
                    var delay = Convert.ToDouble(putDelayTextBox.Text);
                    if( delay == 0.0)
                    {
                        command += "&time=0";
                    } else
                    {
                        // add delay to the current filetime
                        var now = DateTime.Now.ToFileTimeUtc();
                        var time = now + Convert.ToInt64(delay * 10000000);
                        command += "&time=" + time;

                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time must be an number", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            textBlock.Inlines.Add("--------------------------------------------------\n");
            textBlock.Inlines.Add(String.Format("Do PUT: {0}\n", uri + command));

            var task = PutROIAsync(uri, command);
            await task;
        }

    }
}
