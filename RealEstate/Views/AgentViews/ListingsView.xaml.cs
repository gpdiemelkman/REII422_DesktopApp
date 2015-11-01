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
using RealEstate.GridViewSources;
using RealEstate.Windows;
using RealEstate.Overlays.Client;
using Framework.UI.Controls;

namespace RealEstate.Views.AgentViews
{
    /// <summary>
    /// Interaction logic for ListingsView.xaml
    /// </summary>
    public partial class ListingsView
    {
        public int selectedListingID = -1;
        public ListingsView()
        {
            InitializeComponent();
            RefreshListings();
        }
        private void ShowAgentWindow()
        {
            (this.Tag as AgentWindow).AV_Listings.Visibility = System.Windows.Visibility.Hidden;
            (this.Tag as AgentWindow).BT_ManageCustomers.Visibility = System.Windows.Visibility.Visible;
            (this.Tag as AgentWindow).BT_Listings.Visibility = System.Windows.Visibility.Visible;
            (this.Tag as AgentWindow).BT_Manage.Visibility = System.Windows.Visibility.Visible;
            (this.Tag as AgentWindow).BT_Properties.Visibility = System.Windows.Visibility.Visible;
        }
        private string GetCurrentAgent()
        {
            string currentAgent = "";

            this.Dispatcher.Invoke(() =>
            {
                currentAgent = (this.Tag as AgentWindow).currentAgentName;
            });

            return currentAgent;
        }

        #region Input Response
        private void BT_Back_Click(object sender, RoutedEventArgs e)
        {
            ShowAgentWindow();
        }

        private void DG_Listings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG_Listings.SelectedIndex != -1)
            {
                TB_SelectedListing.Text = GetSelectedListing();
                selectedListingID = Convert.ToInt32((DG_Listings.SelectedItem as GridViewSources.Listings).ID);
            }
            else
            {
                TB_SelectedListing.Text = "";
            }
        }

        private void BT_AddListing_Click(object sender, RoutedEventArgs e)
        {
            OpenAddListingView();
        }

        private void DisplayNotifyBox(string title, string message, int duration)
        {
            this.Dispatcher.Invoke(() =>
            {
                NotifyBox.Show(null, title, message, new TimeSpan(0, 0, duration), false);
            });
        }

        private void BT_EditListing_Click(object sender, RoutedEventArgs e)
        {
            if (DG_Listings.SelectedIndex != -1)
            {
                (this.Tag as AgentWindow).LoadEditListingInfo(selectedListingID);
                OpenEditListingView();
            }
            else
            {
                DisplayNotifyBox("Error", "Please select a listing to edit", 5);
            }
        }

        private void BT_DeleteListing_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshListings();
        }
#endregion

        #region OpenOverlays/OpenViews
        private void OpenEditListingView()
        {
            (this.Tag as AgentWindow).HideListingsView();
            (this.Tag as AgentWindow).ShowEditListingView();
        }
        private void OpenAddListingView()
        {
            (this.Tag as AgentWindow).HideListingsView();
            (this.Tag as AgentWindow).ShowAddListingView();
        }
        #endregion

        #region FormControl
        private void SetLoadingState(bool loading)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (loading)
                {
                    BT_AddListing.IsEnabled = false;
                    BT_Back.IsEnabled = false;
                    BT_DeleteListing.IsEnabled = false;
                    BT_EditListing.IsEnabled = false;
                    BT_Refresh.IsEnabled = false;
                }
                else
                {
                    BT_AddListing.IsEnabled = true;
                    BT_Back.IsEnabled = true;
                    BT_DeleteListing.IsEnabled = true;
                    BT_EditListing.IsEnabled = true;
                    BT_Refresh.IsEnabled = true;
                }
            });
        }
        private void RefreshListings()
        {
            new System.Threading.Thread(() =>
            {
                SetLoadingState(true);
                ClearListingsGrid();
                DatabaseManager dbManager = new DatabaseManager();
                AgentManager agManager = new AgentManager();
                var listingInfo = dbManager.ReturnQuery("SELECT * FROM Listing WHERE Agent_ID = " + agManager.GetAgentID(GetCurrentAgent()) + " ORDER BY List_ID;");
                foreach (var list in listingInfo)
                {
                    DatabaseManager dbManager2 = new DatabaseManager();
                    var propertyInfo = dbManager2.ReturnQuery("SELECT Client_ID, Property_Unit_No, Address_ID FROM Property WHERE Property_ID = " + Convert.ToInt32(list[1]) + " ORDER BY Property_ID;");
                    foreach (var property in propertyInfo)
                    {
                        DatabaseManager dbManager3 = new DatabaseManager();
                        var adressInfo = dbManager3.ReturnQuery("SELECT Address_ID, Address_Streetname, Address_Streetno FROM Address WHERE Address_ID = " + Convert.ToInt32(property[2]) + " ORDER BY Address_ID;");
                        foreach (var address in adressInfo)
                        {
                            DatabaseManager dbManager4 = new DatabaseManager();
                            var complexInfo = dbManager4.ReturnQuery("SELECT Complex_Name FROM Complex WHERE Address_ID = " + Convert.ToInt32(address[0]) + " ORDER BY Complex_ID;");
                            foreach (var complex in complexInfo)
                            {
                                DatabaseManager dbManager5 = new DatabaseManager();
                                var clientInfo = dbManager5.ReturnQuery("SELECT Client_Email FROM Clients WHERE Client_ID = " + Convert.ToInt32(property[0]) + " ORDER BY Client_ID;");
                                foreach (var client in clientInfo)
                                {
                                    string yesNoSold;
                                    string yesNoNegotiable;
                                    if (Convert.ToInt32(list[4]) == 0)
                                        yesNoNegotiable = "No";
                                    else
                                        yesNoNegotiable = "Yes";
                                    if (Convert.ToInt32(list[5]) == 0)
                                        yesNoSold = "No";
                                    else
                                        yesNoSold = "Yes";

                                    InsertIntoListingsGrid(Convert.ToInt32(list[0]), client[0], address[1] + " " + address[2], complex[0] + " " + property[1], list[2], yesNoSold, yesNoNegotiable, list[6]);
                                }
                            }
                        }
                    }
                }
                SetLoadingState(false);
            }).Start();
        }
        private void ClearListingsGrid()
        {
            this.Dispatcher.Invoke(() =>
            {
                DG_Listings.Items.Clear();
            });
        }
        private void InsertIntoListingsGrid(int id, string client, string adress, string complex, string price, string sold, string negotiable, string description)
        {
            this.Dispatcher.Invoke(() =>
            {
                DG_Listings.Items.Add(new Listings() { ID = id, Client = client, Address = adress, Complex = complex, Price = price, Sold = sold, Negotiable = negotiable, Description = description });
            });
        }
        private string GetSelectedListing()
        {
            string listInfo = "";

            this.Dispatcher.Invoke(() =>
            {
                listInfo = (DG_Listings.SelectedItem as GridViewSources.Listings).Client + ", " + (DG_Listings.SelectedItem as GridViewSources.Listings).Address;
            });

            return listInfo;
        }
        #endregion

    }
}
