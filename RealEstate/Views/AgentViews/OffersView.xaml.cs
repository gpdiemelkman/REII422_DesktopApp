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
using RealEstate.Overlays.Offers;
using Framework.UI.Controls;

namespace RealEstate.Views.AgentViews
{
    /// <summary>
    /// Interaction logic for OffersView.xaml
    /// </summary>
    public partial class OffersView
    {
        public int currentAgentID;
        public OffersView()
        {            
            InitializeComponent();
            RefreshOffers();          
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
        private void RefreshOffers()
        {            
            new System.Threading.Thread(() =>
            {
                Classes.ListingManager listManager = new Classes.ListingManager();
                currentAgentID = listManager.GetAgentID(GetCurrentAgent());
                ClearOffersGrid();
                DatabaseManager dbManager = new DatabaseManager();
                var listings = dbManager.ReturnQuery("SELECT * FROM Listing WHERE Agent_ID = " + currentAgentID + " ORDER BY List_ID;");
                foreach (var listing in listings)
                {
                    DatabaseManager dbManager2 = new DatabaseManager();
                    var offers = dbManager2.ReturnQuery("SELECT * FROM Offer WHERE List_ID = " + listing[0] + " ORDER BY Offer_ID;");
                    foreach (var offer in offers)
                    {
                        DatabaseManager dbManager3 = new DatabaseManager();
                        var clients = dbManager3.ReturnQuery("SELECT Client_Email FROM Clients WHERE Client_ID = " + offer[2] + " ORDER BY Client_Email;");
                        foreach (var client in clients)
                        {
                            DatabaseManager dbManager4 = new DatabaseManager();
                            var properties = dbManager4.ReturnQuery("SELECT Property_Value, Client_ID FROM Property WHERE Property_ID = " + listing[1] + " ORDER BY Client_ID;");
                            foreach (var property in properties)
                            {
                                DatabaseManager dbManager5 = new DatabaseManager();
                                var owners = dbManager5.ReturnQuery("SELECT Client_Email FROM Clients WHERE Client_ID = " + property[1] + " ORDER BY Client_Email;");
                                foreach (var owner in owners)
                                {
                                    InsertIntoOffersGrid(client[0], Convert.ToInt32(listing[0]), Convert.ToInt32(offer[3]), Convert.ToInt32(listing[2]), Convert.ToInt32(property[0]), Convert.ToInt32(offer[4]), offer[5], owner[0], Convert.ToInt32(offer[0]));
                                }    
                            }
                        }
                    }
                }
            }).Start();
        }

        private void InsertIntoOffersGrid(string clientEmail, int listID, int offeredPrice, int listingPrice, int propertyValue, int status, string date, string propertyOwner, int id)
        {
            this.Dispatcher.Invoke(() =>
            {
                string statusString = "";
                if (status == 0)
                    statusString = "Denied";
                if (status == 1)
                    statusString = "Pending";
                if (status == 2)
                    statusString = "Passed";
                DG_Offers.Items.Add(new Offer() { ID= id, Client = clientEmail, Listing = listID, Offered_Price = offeredPrice, Listing_Price = listingPrice, Property_Value = propertyValue, Status = statusString, Date = date.Substring(0,10), Property_Owner = propertyOwner});
            });
        }
        private void ClearOffersGrid()
        {
            this.Dispatcher.Invoke(() =>
            {
                DG_Offers.Items.Clear();
            });            
        }

        private void BT_AddOffer_Click(object sender, RoutedEventArgs e)
        {
            OpenAddOfferOverlay();
        }

        private void OpenAddOfferOverlay()
        {
            AddOfferOverlay offerOverlay = new AddOfferOverlay(currentAgentID);
            offerOverlay.OnExit += OfferOverlays_OnClose;
            offerOverlay.Owner = Framework.UI.Controls.Window.GetWindow(this);
            offerOverlay.Show();
        }

        private void OfferOverlays_OnClose(object sender, EventArgs e)
        {
            RefreshOffers();
        }

        private void BT_Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshOffers();
        }

        private void BT_DeleteOffer_Click(object sender, RoutedEventArgs e)
        {
            if (DG_Offers.SelectedIndex != -1)
            {
                OfferManager offerManager = new OfferManager();
                if (offerManager.DeleteOffer((DG_Offers.SelectedItem as GridViewSources.Offer).ID))
                {
                    DisplayNotifyBox("Deleted", "The offer has been deleted", 3);
                    RefreshOffers();
                }
                else
                {
                    DisplayNotifyBox("ERROR", "The offer could not be deleted", 5);
                }
            }
            else
            {
                DisplayNotifyBox("Cannot Delete", "Please select offer to be deleted", 5);
            }
        }

        private void DisplayNotifyBox(string title, string message, int durationSeconds)
        {
            this.Dispatcher.Invoke(() =>
            {
                NotifyBox.Show(null, title, message, new TimeSpan(0, 0, durationSeconds), false);
            });
        }
        private void BT_EditOffer_Click(object sender, RoutedEventArgs e)
        {
            if (DG_Offers.SelectedIndex != -1)
                OpenEditOfferOverlay();
            else
                DisplayNotifyBox("Cannot Edit", "Please select offer to be edited", 5);

        }

        private void OpenEditOfferOverlay()
        {
            EditOfferOverlay offerOverlay = new EditOfferOverlay((DG_Offers.SelectedItem as GridViewSources.Offer).ID, currentAgentID);
            offerOverlay.OnExit += OfferOverlays_OnClose;
            offerOverlay.Owner = Framework.UI.Controls.Window.GetWindow(this);
            offerOverlay.Show();
        }

        private void BT_Back_Click(object sender, RoutedEventArgs e)
        {
            ShowAgentWindow();
        }

        private void ShowAgentWindow()
        {
            (this.Tag as AgentWindow).AV_Offers.Visibility = System.Windows.Visibility.Hidden;
            (this.Tag as AgentWindow).BT_ManageCustomers.Visibility = System.Windows.Visibility.Visible;
            (this.Tag as AgentWindow).BT_Listings.Visibility = System.Windows.Visibility.Visible;
            (this.Tag as AgentWindow).BT_ManageOffers.Visibility = System.Windows.Visibility.Visible;
            (this.Tag as AgentWindow).BT_Properties.Visibility = System.Windows.Visibility.Visible;
        }

        private void DG_Offers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG_Offers.SelectedIndex != -1)
            {
                TB_SelectedOffer.Text = (DG_Offers.SelectedItem as GridViewSources.Offer).Client + " - " + (DG_Offers.SelectedItem as GridViewSources.Offer).Offered_Price;
            }
            else
            {
                TB_SelectedOffer.Text = "";
            }
        }
    }
}
