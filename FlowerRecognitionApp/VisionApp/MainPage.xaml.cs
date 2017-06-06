using CloudVisionAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VisionApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Create Cloud Vision API Client and set Key for API Access
        private CloudVisionAPIClient APIClient = new CloudVisionAPIClient();
        //private CloudVisionAPIClient APIClient = new CloudVisionAPIClient("Set API Key here");

        private StorageFile imageStorageFile;

        private string[] filterArray = { "flower", "plant", "yellow", "red", "white", "flora", "land plant", "flowering plant", "daisy family", "petal", "field", "plant stem", "macro photography",
            "purple","pink","photography","close up","blossom","black and white","green","annual plant","botany","floristry","biology","leaf","branch","tree","shrub","nature","wildflower"};

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void pickImage()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                imageStorageFile = file;
                openAndSetImage(imageStorageFile);
            }
            else
            {
                return;
            }

            line1.Text = "loading...";
            var requestedFeatureType = getSelectedType();
            var result = APIClient.AnnotateImage(imageStorageFile, requestedFeatureType, 30);
            string jsonResult = await result;
            JObject o = JObject.Parse(jsonResult);
            JArray a = (JArray)o["responses"][0]["labelAnnotations"];
            List<visionLabelValue> myVisionLabelValue = a.ToObject<List<visionLabelValue>>();
            string textstring = "";
            string firstResult = "";
            int countOfResult = 1;
            for (int n = 0; n < myVisionLabelValue.Count; n++)
            {
                if (countOfResult <= 5)
                {

                    bool NameIncluded = false;
                    foreach (string x in filterArray)
                    {
                        if (myVisionLabelValue[n].Description == x)
                        {
                            NameIncluded = true;
                        }
                    }

                    if (!NameIncluded)
                    {
                        if (countOfResult == 1)
                        {
                            float value = float.Parse(myVisionLabelValue[n].Score);
                            string score = value.ToString("#0.##%");
                            firstResult = myVisionLabelValue[n].Description + "    " + score;
                        }
                        else
                        {
                            float value = float.Parse(myVisionLabelValue[n].Score);
                            string score = value.ToString("#0.##%");
                            textstring += myVisionLabelValue[n].Description + "    " + score + Environment.NewLine;
                        }
                        countOfResult += 1;
                    }
                }
            }

            line1.Text = "It could be:";
            line2.Text = firstResult;
            line3.Text = textstring;

        }

        private async void captureImage()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.AllowCropping = false;

            StorageFile file = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (file == null)
            {
                // No photo captured
                return;
            }

            imageStorageFile = file;
            openAndSetImage(imageStorageFile);
            line1.Text = "loading...";
            var requestedFeatureType = getSelectedType();
            var result = APIClient.AnnotateImage(imageStorageFile, requestedFeatureType, 15);
            string jsonResult = await result;
            JObject o = JObject.Parse(jsonResult);
            JArray a = (JArray)o["responses"][0]["labelAnnotations"];
            List<visionLabelValue> myVisionLabelValue = a.ToObject<List<visionLabelValue>>();
            string textstring = "";
            string firstResult = "";
            int countOfResult = 1;
            for (int n = 0; n < myVisionLabelValue.Count; n++)
            {
                if (countOfResult <= 5)
                {

                    bool NameIncluded = false;
                    foreach (string x in filterArray)
                    {
                        if (myVisionLabelValue[n].Description == x)
                        {
                            NameIncluded = true;
                        }
                    }

                    if (!NameIncluded)
                    {
                        if (countOfResult == 1)
                        {
                            float value = float.Parse(myVisionLabelValue[n].Score);
                            string score = value.ToString("#0.##%");
                            firstResult = myVisionLabelValue[n].Description + "    " + score;
                        }
                        else
                        {
                            float value = float.Parse(myVisionLabelValue[n].Score);
                            string score = value.ToString("#0.##%");
                            textstring += myVisionLabelValue[n].Description + "    " + score + Environment.NewLine;
                        }
                        countOfResult += 1;
                    }
                }
            }

            line1.Text = "It could be:";
            line2.Text = firstResult;
            line3.Text = textstring;
        }

        private async void openAndSetImage(StorageFile file)
        {
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);

            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

            imageControl.Source = bitmapSource;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            pickImage();
        }

        //private async void TypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if(resultText != null)
        //    { 
        //    resultText.Text = "loading...";
        //        var requestedFeatureType = getSelectedType();
        //        var result = APIClient.AnnotateImage(imageStorageFile, requestedFeatureType, 15);
        //    resultText.Text = await result;
        //    }
        //}

        private FeatureType getSelectedType()
        {
            //if (TypeListBox != null && TypeListBox.SelectedItem != null && resultText != null)
            //{
            //    var selectedItemText = ((ListBoxItem)TypeListBox.SelectedItem).Content.ToString();
            //    var requestedFeatureType = (FeatureType)Enum.Parse(typeof(FeatureType), selectedItemText);
            //    return requestedFeatureType;
            //}
            //else 
            return FeatureType.LABEL_DETECTION;
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            captureImage();
        }

        private void LibraryButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MasterDetailPage));
        }


        //private void APIKeyBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    APIClient.APIKey = "AIzaSyAy4tHJKuC2SQT1wYlXqBlxK5xmUonMZko";
        //}
    }

    public class visionLabelValue
    {
        public string Mid { get; set; }
        public string Description { get; set; }
        public string Score { get; set; }
    }


}
