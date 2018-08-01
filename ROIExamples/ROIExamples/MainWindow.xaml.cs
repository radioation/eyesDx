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

using System.IO;

namespace ROIExamples
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // setup shape list
        public ObservableCollection<string> shapeList = new ObservableCollection<string>();

        public ObservableCollection<string> presetList = new ObservableCollection<string>();
        public string presetPath = System.IO.Path.GetTempPath() + "eyesDx_roi_presets.xml";
        Dictionary<string, string> presets = new Dictionary<string, string>();

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

            presetList.Add("rect_test");
            presetList.Add("ellipse_test");
            presetList.Add("shape_test");
            putPresetComboBox.ItemsSource = presetList;

            if (File.Exists(presetPath))
            {
                // read in text file 
            }
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
                if (putShapeComboBox.Text == "Rectangle")
                {
                    command += "?shape=rectangular";
                }
                else
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
                    if (delay == 0.0)
                    {
                        command += "&time=0";
                    }
                    else
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

            if (putScreenTextBox.Text.Length > 0)
            {
                command += "&screen_index=" + putScreenTextBox.Text;
            }

            textBlock.Inlines.Add("--------------------------------------------------\n");
            textBlock.Inlines.Add(String.Format("Do PUT: {0}\n", uri + command));

            var task = PutROIAsync(uri, command);
            await task;
        }

        private void savePresetButton_Click(object sender, RoutedEventArgs e)
        {
            // if save
            if (putNameTextBox.Text == "rect_test" || putNameTextBox.Text == "ellipse_test" || putNameTextBox.Text == "shape_test")
            {
                //
                MessageBox.Show("Preset name cannot be one of\n'rect_test', 'ellipse_test', 'shape_test'", "Invalid Preset Name", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (putNameTextBox.Text == "")
            {
                MessageBox.Show("Preset must have a name", "Invalid Preset Name", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var name = putNameTextBox.Text;
            var values = name + ":" + putShapeComboBox.SelectedIndex;

            if (putShapeComboBox.SelectedIndex < 2)
            {
                // rect or ellipse
                values += ":" + putXTextBox.Text;
                values += ":" + putYTextBox.Text;
                values += ":" + putWTextBox.Text;
                values += ":" + putHTextBox.Text;
                values += "::" + putScreenTextBox.Text;
            }
            else
            {
                values += ":::::" + putPointsTextBox.Text + ":" + putScreenTextBox.Text;
            }

            // add to dictionary
            presets[name] = values;

            // if name insn't in presetList, add it
            if (!presetList.Contains(name))
            {
                presetList.Add(name);
            }
        }

        private void deletePresetButton_Click(object sender, RoutedEventArgs e)
        {
            if( putPresetComboBox.SelectedIndex < 3)
            {
                MessageBox.Show("Cannot delete\n'rect_test', 'ellipse_test', 'shape_test'", "Invalid Preset", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var name = putPresetComboBox.Text;
            if(presets.ContainsKey(name))
            {
                presets.Remove(name);
                presetList.Remove(name);
                putPresetComboBox.SelectedIndex = -1;

            }
        }


        private void putPresetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // defaults take priority
            var selection = (sender as ComboBox).SelectedItem as string;
            if( selection == null)
            {
                putNameTextBox.Text = "";
                putXTextBox.Text = "";
                putYTextBox.Text = "";
                putWTextBox.Text = "";
                putHTextBox.Text = "";
                putPointsTextBox.Text = "";
                putScreenTextBox.Text = "";
            }
            else if (selection == "rect_test")
            {
                putNameTextBox.Text = "rect_test";
                putXTextBox.Text = "0.1";
                putYTextBox.Text = "0.1";
                putWTextBox.Text = "0.8";
                putHTextBox.Text = "0.8";
                putShapeComboBox.SelectedIndex = 0;
                putPointsTextBox.Text = "";
                putScreenTextBox.Text = "0";
            }
            else if (selection == "ellipse_test")
            {
                putNameTextBox.Text = "ellipse_test";
                putXTextBox.Text = "0.5";
                putYTextBox.Text = "0.5";
                putWTextBox.Text = "0.2";
                putHTextBox.Text = "0.4";
                putShapeComboBox.SelectedIndex = 1;
                putPointsTextBox.Text = "";
                putScreenTextBox.Text = "0";
            }
            else if (selection == "shape_test")
            {
                putNameTextBox.Text = "shape_test";
                putXTextBox.Text = "";
                putYTextBox.Text = "";
                putWTextBox.Text = "";
                putHTextBox.Text = "";
                putShapeComboBox.SelectedIndex = 2;
                putPointsTextBox.Text = "0.2,0.2;0.8,0.5;0.5,0.9";
                putScreenTextBox.Text = "0";

            }
            else
            {
                // see if presets has name 
                if (presets.ContainsKey(selection))
                { 

                    string[] values = presets[selection].Split(':');
                    putNameTextBox.Text = selection;
                    int shapeType = Int32.Parse(values[1]);
                    putShapeComboBox.SelectedIndex = shapeType;
                    if (shapeType < 2)
                    {
                        putXTextBox.Text = values[2];
                        putYTextBox.Text = values[3];
                        putWTextBox.Text = values[4];
                        putHTextBox.Text = values[5];
                        putPointsTextBox.Text = "";
                        putScreenTextBox.Text = values[7];
                    }
                    else
                    {
                        putXTextBox.Text = "";
                        putYTextBox.Text = "";
                        putWTextBox.Text = "";
                        putHTextBox.Text = "";
                        putPointsTextBox.Text = values[6];
                        putScreenTextBox.Text = values[7];
                    }
                }
            }
        }
    }
}
