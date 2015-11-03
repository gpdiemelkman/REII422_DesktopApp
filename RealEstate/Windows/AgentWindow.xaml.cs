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

namespace RealEstate.Windows
{
    /// <summary>
    /// Interaction logic for AgentWindow.xaml
    /// </summary>
    public partial class AgentWindow
    {
        public string currentAgentName { get; private set; }
        public int currentAgentID { get; private set; }

        public AgentWindow(string agent)
        {
            InitializeComponent();
            currentAgentName = agent;
            this.Title = this.Title + agent;
            AV_Listings.Tag = this;
            AV_Offers.Tag = this;
            AV_MangeClients.Tag = this;
            AV_AddListing.Tag = this;
            AV_EditListing.Tag = this;
            AV_ViewProperties.Tag = this;
            Classes.ListingManager listManager = new Classes.ListingManager();
            currentAgentID = listManager.GetAgentID(currentAgentName);
        }

        private void RE_AgentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenMainWindow();
        }

        private void OpenMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        #region Input Response
        private void BT_ManageCustomers_Click(object sender, RoutedEventArgs e)
        {
            ShowManageClientsView();
        }
        private void BT_Listings_Click(object sender, RoutedEventArgs e)
        {
            ShowListingsView();
        }
        private void BT_ManageOffers_Click(object sender, RoutedEventArgs e)
        {
            ShowManageOffersView();
        }
        private void BT_Properties_Click(object sender, RoutedEventArgs e)
        {
            ShowViewPropertiesView();
        }
        #endregion

        #region Form Control
        public void ShowManageOffersView()
        {
            HideButtons();
            AV_Offers.Visibility = System.Windows.Visibility.Visible;
        }
        public void HideManageOffersView()
        {
            AV_Offers.Visibility = System.Windows.Visibility.Hidden;
        }
        public void ShowListingsView()
        {
            HideButtons();
            AV_Listings.Visibility = System.Windows.Visibility.Visible;
        }
        public void HideListingsView()
        {
            AV_Listings.Visibility = System.Windows.Visibility.Hidden;
        }
        public void ShowAddListingView()
        {
            AV_AddListing.Visibility = System.Windows.Visibility.Visible;
        }
        public void HideAddListingView()
        {
            AV_AddListing.Visibility = System.Windows.Visibility.Hidden;
        }
        public void ShowEditListingView()
        {
            AV_EditListing.Visibility = System.Windows.Visibility.Visible;
        }
        public void HideEditListingView()
        {
            AV_EditListing.Visibility = System.Windows.Visibility.Hidden;
        }
        private void ShowManageClientsView()
        {
            HideButtons();
            AV_MangeClients.Visibility = System.Windows.Visibility.Visible;
        }
        private void HideButtons()
        {
            BT_Listings.Visibility = System.Windows.Visibility.Hidden;
            BT_ManageCustomers.Visibility = System.Windows.Visibility.Hidden;
            BT_Properties.Visibility = System.Windows.Visibility.Hidden;
            BT_ManageOffers.Visibility = System.Windows.Visibility.Hidden;
        }
        private void ShowButtons()
        {
            BT_Listings.Visibility = System.Windows.Visibility.Visible;
            BT_ManageCustomers.Visibility = System.Windows.Visibility.Visible;
            BT_Properties.Visibility = System.Windows.Visibility.Visible;
            BT_ManageOffers.Visibility = System.Windows.Visibility.Visible;
        }
        private void ShowViewPropertiesView()
        {
            HideButtons();
            AV_ViewProperties.Visibility = System.Windows.Visibility.Visible;
        }
        public void LoadEditListingInfo(int listID)
        {
            AV_EditListing.LoadListingInfo(listID);
        }
        #endregion
    }
}
