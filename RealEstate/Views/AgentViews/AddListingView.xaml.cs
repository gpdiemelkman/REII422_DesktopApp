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
using RealEstate.Windows;
using System.IO;

namespace RealEstate.Views.AgentViews
{
    /// <summary>
    /// Interaction logic for AddListingView.xaml
    /// </summary>
    public partial class AddListingView
    {

        public List<int> clientsID;
        public List<int> provinceID;
        public List<int> cityID;
        public List<int> areaID;
        public List<int> imageID;
        public int currentProvinceID;
        public int currentCityID;
        public int currentAreaID;
        public int currentClientID;

        public string streetName;
        public int streetNo;
        public string complexName;
        public int complexNo;

        public string agentEmail;
        public int agentID;
        public int propertyID;
        public int complexID;
        public int addressID;
        public int bedrooms;
        public int bathrooms;
        public int garages;
        public int plotSize;
        public int houseSize;
        public int propertyPrice;
        public int propertyValue;
        public int isSold;
        public int isNegotiable;
        public int hasPool;
        public string description;

        BitmapImage previewImage;

        private List<string> imageSource = new List<string>();
        private List<string> imageCaptions = new List<string>();

        public AddListingView()
        {
            InitializeComponent();
            LoadComboBoxes();
            TB_ImageCaption.IsEnabled = false;
        }
        #region Input response
        private void CB_Complex_Click(object sender, RoutedEventArgs e)
        {
            if( (bool)CB_Complex.IsChecked )
            {
                SP_ComplexDetails.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                SP_ComplexDetails.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            (this.Tag as AgentWindow).HideAddListingView();
            (this.Tag as AgentWindow).ShowListingsView();
        }
        private void BT_AddImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt = ".jpeg";
            dialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                
                CB_Images.Items.Add(dialog.FileName.ToString());
                IMG_SelectedImage.Source = CloneImage(dialog.FileName.ToString());
                imageSource.Add(dialog.FileName.ToString());
                imageCaptions.Add("");
                TB_ImageCaption.IsEnabled = true;
                
                if( (CB_Images.Items.Count-1)  == 0 )
                {
                    CB_Images.SelectedIndex = 0;
                }
                else
                {
                    CB_Images.SelectedIndex = CB_Images.Items.Count - 1;
                }
            }
        }
        private void CB_Images_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if( CB_Images.SelectedIndex != -1)
            {
                IMG_SelectedImage.Source = CloneImage(CB_Images.SelectedValue.ToString());
                TB_ImageCaption.Text = imageCaptions[CB_Images.SelectedIndex].ToString();
            }
        }
        private void CB_Client_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentClientID = clientsID[CB_Client.SelectedIndex];
        }
        private void CB_Province_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            new System.Threading.Thread(() =>
            {
                SetLoadingState(true);
                ClearCities();
                ChangeProvinceID();
                LoadCities();
                SetLoadingState(false);
            }).Start();
        }
        private void CB_City_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            new System.Threading.Thread(() =>
            {
                ClearAreas();
                ChangeCityID();
                LoadArea();
            }).Start();
        }
        private void CB_Area_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_Area.SelectedIndex != -1)
                currentAreaID = areaID[CB_Area.SelectedIndex];
        }
        private void BT_CreateListing_Click(object sender, RoutedEventArgs e)
        {
            LoadVariablesTB();
            CreateListings();
            UploadImages();
        }
        private void TB_ImageCaption_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(CB_Images.SelectedIndex != -1)
                imageCaptions[CB_Images.SelectedIndex] = TB_ImageCaption.Text.ToString() + e.Text.ToString();
        }
        private void BT_Delete_Click(object sender, RoutedEventArgs e)
        {
            imageSource.Remove(CB_Images.SelectedItem.ToString());
            CB_Images.Items.RemoveAt(CB_Images.SelectedIndex);
            IMG_SelectedImage.Source = null;
            TB_ImageCaption.Text = "";
            if (CB_Images.Items.Count != 0)
                CB_Images.SelectedIndex = 0;
            else
                CB_Images.SelectedIndex = -1;
        }
        #endregion

        #region Form Control
        private void SetLoadingState(bool loading)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (loading)
                {
                    BT_AddImage.IsEnabled = false;
                    BT_Cancel.IsEnabled = false;
                    BT_Delete.IsEnabled = false;
                    BT_CreateListing.IsEnabled = false;
                    BT_Estimate.IsEnabled = false;
                    TB_Bathrooms.IsEnabled = false;
                    TB_Bedrooms.IsEnabled = false;
                    TB_ComplexName.IsEnabled = false;
                    TB_ComplexNo.IsEnabled = false;
                    TB_Description.IsEnabled = false;
                    TB_Garages.IsEnabled = false;
                    TB_HouseSize.IsEnabled = false;
                    TB_ImageCaption.IsEnabled = false;
                    TB_ListPrice.IsEnabled = false;
                    TB_PlotSize.IsEnabled = false;
                    TB_Price.IsEnabled = false;
                    TB_Streetname.IsEnabled = false;
                    TB_Streetno.IsEnabled = false;
                    CB_Area.IsEditable = false;
                    CB_City.IsEnabled = false;
                    CB_Client.IsEnabled = false;
                    CB_Complex.IsEnabled = false;
                    CB_hasPool.IsEnabled = false;
                    CB_Images.IsEnabled = false;
                    CB_isNegotiable.IsEnabled = false;
                    CB_isSold.IsEnabled = false;
                    CB_Province.IsEnabled = false;
                }
                else
                {
                    BT_AddImage.IsEnabled = true;
                    BT_Cancel.IsEnabled = true;
                    BT_Delete.IsEnabled = true;
                    BT_CreateListing.IsEnabled = true;
                    BT_Estimate.IsEnabled = true;
                    TB_Bathrooms.IsEnabled = true;
                    TB_Bedrooms.IsEnabled = true;
                    TB_ComplexName.IsEnabled = true;
                    TB_ComplexNo.IsEnabled = true;
                    TB_Description.IsEnabled = true;
                    TB_Garages.IsEnabled = true;
                    TB_HouseSize.IsEnabled = true;
                    TB_ImageCaption.IsEnabled = true;
                    TB_ListPrice.IsEnabled = true;
                    TB_PlotSize.IsEnabled = true;
                    TB_Price.IsEnabled = true;
                    TB_Streetname.IsEnabled = true;
                    TB_Streetno.IsEnabled = true;
                    CB_Area.IsEditable = true;
                    CB_City.IsEnabled = true;
                    CB_Client.IsEnabled = true;
                    CB_Complex.IsEnabled = true;
                    CB_hasPool.IsEnabled = true;
                    CB_Images.IsEnabled = true;
                    CB_isNegotiable.IsEnabled = true;
                    CB_isSold.IsEnabled = true;
                    CB_Province.IsEnabled = true;
                }
            });
        }
        private void LoadComboBoxes()
        {
            LoadClients();
            LoadProvinces();
        }
        private void LoadClients()
        {
            Classes.ListingManager listManager = new Classes.ListingManager();
            foreach (string clients in listManager.GetClients())
            {
                CB_Client.Items.Add(clients);
            }
            clientsID = listManager.GetClientsID();
        }
        private void LoadProvinces()
        {
            Classes.ListingManager listManager = new Classes.ListingManager();
            foreach (string provinces in listManager.GetProvinces())
            {
                CB_Province.Items.Add(provinces);
            }
            provinceID = listManager.GetProvincesID();
        }
        private void LoadCities()
        {
            this.Dispatcher.Invoke(() =>
            {
                Classes.ListingManager listManager = new Classes.ListingManager();
                foreach (string cities in listManager.GetCities(currentProvinceID))
                {
                    CB_City.Items.Add(cities);
                }
                cityID = listManager.GetCitiesID(currentProvinceID);
            });
        }
        private void ClearCities()
        {
            this.Dispatcher.Invoke(()=>
            {
                CB_City.Items.Clear();
            });
        }
        private void ClearAreas()
        {
            this.Dispatcher.Invoke(() =>
                {
                    CB_Area.Items.Clear();
                });
        }
        private void ChangeProvinceID()
        {
            this.Dispatcher.Invoke(() =>
            {
                currentProvinceID = provinceID[CB_Province.SelectedIndex];
            });
        }
        private void ChangeCityID()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (CB_City.SelectedIndex != -1)
                    currentCityID = cityID[CB_City.SelectedIndex];
            });
        }      
        private void LoadVariablesTB()
        {
            streetName = TB_Streetname.Text;
            streetNo = Convert.ToInt32(TB_Streetno.Text);
            bedrooms = Convert.ToInt32(TB_Bedrooms.Text);
            bathrooms = Convert.ToInt32(TB_Bathrooms.Text);
            garages = Convert.ToInt32(TB_Garages.Text);
            plotSize = Convert.ToInt32(TB_PlotSize.Text);
            houseSize = Convert.ToInt32(TB_HouseSize.Text);
            propertyPrice = Convert.ToInt32(TB_ListPrice.Text);
            propertyValue = Convert.ToInt32(TB_Price.Text);
            isSold = Convert.ToInt32(CB_isSold.IsChecked);
            isNegotiable = Convert.ToInt32(CB_isNegotiable.IsChecked);
            hasPool = Convert.ToInt32(CB_hasPool.IsChecked);
            description = TB_Description.Text;

            complexName = "Null";
            complexNo = 0;

            if (CB_Complex.IsChecked == true)
            {
                complexName = TB_ComplexName.Text;
                complexNo = Convert.ToInt32(TB_ComplexNo.Text);
            }

        }
        private void LoadArea()
        {
            this.Dispatcher.Invoke(() =>
            {
                Classes.ListingManager listManager = new Classes.ListingManager();
                foreach (string areas in listManager.GetAreas(currentCityID))
                {
                    CB_Area.Items.Add(areas);
                }
                areaID = listManager.GetAreasID(currentCityID);
            });
        }        
        BitmapImage GetBitmapImage(byte[] imageBytes)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageBytes);
            bitmapImage.EndInit();
            return bitmapImage;
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
        #endregion

        private void CreateListings()
        {
            agentEmail = (this.Tag as AgentWindow).currentAgentName;
            Classes.ListingManager listManager = new Classes.ListingManager();
            if (listManager.GetAddressID(currentAreaID, streetName, streetNo) == -1)
                listManager.AddListingAddress(currentAreaID, streetName, streetNo);
            addressID = listManager.GetAddressID(currentAreaID, streetName, streetNo);
            if (CB_Complex.IsChecked == true)
            {
                if (listManager.GetComplexID(complexName, addressID) == -1)
                    listManager.AddListingComplex(complexName, addressID);
                complexID = listManager.GetComplexID(complexName, addressID);
            }
            else
                complexID = -1;

            listManager.AddListingProperty(currentClientID, addressID, complexID, complexNo, bedrooms, bathrooms, garages, hasPool, plotSize, houseSize, propertyValue, description);
            propertyID = listManager.GetPropertyID(currentClientID);
            agentID = listManager.GetAgentID(agentEmail);
            listManager.AddListing(propertyID, agentID, propertyPrice, isNegotiable, isSold, description);
        }
        private void UploadImages()
        {
            Overlays.Listings.LoadingOverlay uploadImages = new Overlays.Listings.LoadingOverlay(propertyID, imageSource, imageCaptions, imageID);
            uploadImages.Owner = Framework.UI.Controls.Window.GetWindow(this);
            uploadImages.Show();
            (this.Tag as AgentWindow).HideAddListingView();
            (this.Tag as AgentWindow).ShowListingsView();
        }
    }
}
