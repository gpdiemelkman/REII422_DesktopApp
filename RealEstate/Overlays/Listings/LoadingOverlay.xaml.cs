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
using RealEstate.Classes;
using System.IO;

namespace RealEstate.Overlays.Listings
{
    /// <summary>
    /// Interaction logic for LoadingOverlay.xaml
    /// </summary>
    public partial class LoadingOverlay
    {
        List<string> localFilePaths;
        List<string> imageCaptions;
        int propertyID;
        int numberOfImages = 0;
        int numberToUpload= 0;

        const string webDir = "ftp://ingeneric.co.za/public_html/RealEstate/";
        const string linkDir = "http://ingeneric.co.za/RealEstate/";

        public LoadingOverlay(int property, List<string> localFiles, List<string> captions)
        {
            InitializeComponent();
            localFilePaths = localFiles;
            imageCaptions = captions;
            propertyID = property;
            Console.WriteLine("Get========== " + imageCaptions.Count);
        }



        private void UploadImages()
        {
            new System.Threading.Thread(() =>
                {
                    Console.WriteLine("Intro========== " + imageCaptions.Count);
                    FtpManager ftpManger = new FtpManager();
                    ListingManager listingManager = new ListingManager();
                    ftpManger.OnProgressChange += ftpManger_OnProgressChange;
                    Hashing hashing = new Hashing();
                    List<string> urls = new List<string>();
                    List<string> links = new List<string>();
                    numberOfImages = localFilePaths.Count();
                    foreach( string localFile in localFilePaths)
                    {
                        numberToUpload++;
                        string fileHash = hashing.HashFile(localFile);
                        string extension = System.IO.Path.GetExtension(localFile);

                        urls.Add(webDir + fileHash + extension);
                        links.Add(linkDir + fileHash + extension);
                    }
                    for( int i = 0; i < numberOfImages ; i++)
                    {
                        LoadImage(localFilePaths[i]);
                        Console.WriteLine("Pregame========== " + imageCaptions.Count);
                        UpdateProgress(numberToUpload, i+1);
                        ftpManger.UploadFile(localFilePaths[i], urls[i]);
                        listingManager.AddListingImage(propertyID, links[i], imageCaptions[i]);
                    }

                    CloseForm();
                }).Start();
        }

        private void ftpManger_OnProgressChange(double progress)
        {
 	        this.Dispatcher.Invoke(()=>
                {
                    PB_CurrentImageProgress.Value = progress;
                });
        }

        private void LoadImage(string path)
        {
            this.Dispatcher.Invoke(()=>
                {
                    try
                    {
                        IMG_UploadingImage.Source = CloneImage(path);
                        TB_CurrentImage.Text = path;
                    }
                    catch
                    {

                    }
                });
        }

        private void UpdateProgress(int max, int current)
        {
            this.Dispatcher.Invoke(()=>
                {
                    Console.WriteLine("Update========== " + imageCaptions.Count);
                    TB_ImageCount.Text = " (" + current.ToString() + "/" + max.ToString() + ")";
                    TB_CurrentCaption.Text = imageCaptions[current-1];
                });
        }

        private void RE_AddClientOverlay_Loaded(object sender, RoutedEventArgs e)
        {
            UploadImages();
        }

        private void CloseForm()
        {
            this.Dispatcher.Invoke(() =>
                {
                    this.Close();
                });
        }

        BitmapImage CloneImage(string filePath)
        {
            List<byte> bytes = new List<byte>();
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] buffer = new byte[65536];
            int read = 0;
            do
            {
                read = fs.Read(buffer, 0, buffer.Length);
                for (int i = 0; i < read; i++)
                    bytes.Add(buffer[i]);

            }
            while (read != 0);

            fs.Close();


            var allbytes = bytes.ToArray();
            var bitmapimage = GetBitmapImage(allbytes);

            return bitmapimage;
        }

        BitmapImage GetBitmapImage(byte[] imageBytes)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageBytes);
            bitmapImage.EndInit();
            return bitmapImage;
        }

        
    }
}
