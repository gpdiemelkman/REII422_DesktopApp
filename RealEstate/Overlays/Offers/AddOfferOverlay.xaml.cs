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
using Framework.UI.Controls;
using RealEstate.Views.AgentViews;

namespace RealEstate.Overlays.Offers
{
    /// <summary>
    /// Interaction logic for AddOfferOverlay.xaml
    /// </summary>
    public partial class AddOfferOverlay
    {
        public event EventHandler OnExit;
        private List<int> clientsID = new List<int>();
        private List<int> listID = new List<int>();
        private int currentAgentID;
        public AddOfferOverlay(int currentAgent)
        {
            currentAgentID = currentAgent;
            InitializeComponent();
            populateComboBoxes();
        }

        private void populateComboBoxes()
        {
            populateClients();
            populateListings();
            CB_Status.Items.Add("Denied");
            CB_Status.Items.Add("Pending");
            CB_Status.Items.Add("Passed");
        }

        private void populateClients()
        {
            Classes.ListingManager listManager = new Classes.ListingManager();
            foreach (string clients in listManager.GetClients())
            {
                CB_Client.Items.Add(clients);
            }
            clientsID = listManager.GetClientsID();
        }

        private void populateListings()
        {
            Classes.OfferManager offerManager = new Classes.OfferManager();
            listID = offerManager.GetListings(currentAgentID);
            foreach (int listing in listID)
            {
                CB_Listing.Items.Add(listing.ToString());
            }            
        }
        private void DisplayNotifyBox(string title, string message, int durationSeconds)
        {
            this.Dispatcher.Invoke(() =>
            {
                NotifyBox.Show(null, title, message, new TimeSpan(0, 0, durationSeconds), false);
            });
        }

        private void BT_AddOffer_Click(object sender, RoutedEventArgs e)
        {
            Classes.OfferManager offerManager = new Classes.OfferManager();
            Classes.Validation valid = new Classes.Validation();
            if (valid.IsTextNumeric(TB_Price.Text) && valid.IsNumberInRange(0, 2147483645, TB_Price.Text) && valid.DateTest(TB_Price.Text))
            {
                if (offerManager.AddOffer(listID[CB_Listing.SelectedIndex], clientsID[CB_Client.SelectedIndex], Convert.ToInt32(TB_Price.Text), CB_Status.SelectedIndex, DP_Date.Text))
                {
                    DisplayNotifyBox("Offer added", "The offer from " + CB_Client.SelectedItem + " has been added", 5);
                    ClearAddOfferOverlay();
                }
                else
                {
                    DisplayNotifyBox("ERROR", "Offer could not be added", 5);
                }
            }
            else
            {
                DisplayNotifyBox("ERROR", "Please ensure the price is numeric and lower than 2 147 483 645, and that the date is in the valid format", 5);
            }
        }

        private void ClearAddOfferOverlay()
        {
            CB_Client.SelectedIndex = -1;
            CB_Listing.SelectedIndex = -1;
            CB_Status.SelectedIndex = -1;
            TB_Price.Text = "";
            DP_Date.Text = "";
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RE_AddOfferOverlay_Closed(object sender, EventArgs e)
        {
            if (OnExit != null)
                OnExit(this, new EventArgs());
        }
    }
}
