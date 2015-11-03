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
using RealEstate.Classes;
using RealEstate.Windows;

namespace RealEstate.Overlays.Offers
{
    /// <summary>
    /// Interaction logic for EditOfferOverlay.xaml
    /// </summary>
    public partial class EditOfferOverlay
    {
        public event EventHandler OnExit;
        private List<int> clientsID = new List<int>();
        private List<int> listID = new List<int>();
        private int currentOfferID;
        private int currentAgentID;
        public EditOfferOverlay(int offerID, int currentAgent)
        {
            currentAgentID = currentAgent;
            currentOfferID = offerID;
            InitializeComponent();
            populateComboBoxes();
            setCurrentOfferValues();
        }

        private void setCurrentOfferValues()
        {
            DatabaseManager dbManager = new DatabaseManager();
            var offers = dbManager.ReturnQuery("SELECT * FROM Offer WHERE Offer_ID = " + currentOfferID + " ORDER BY Offer_ID;");
            foreach (var offer in offers)
            {
                CB_Listing.SelectedIndex = (CB_Listing.Items.IndexOf(offer[1]));
                CB_Client.SelectedIndex = clientsID.IndexOf(Convert.ToInt32(offer[2]));
                TB_Price.Text = offer[3];
                CB_Status.SelectedIndex = Convert.ToInt32(offer[4]);
                string [] date =offer[5].ToString().Split(new string[] { "/" }, StringSplitOptions.None);
                string displayDay = date[0];
                string displayMonth = date[1];
                string displayYear = date[2].Substring(0,4);
                Console.WriteLine(offer[5].ToString());
                DP_Date.Text = displayYear + "-" + displayMonth + "-" + displayDay;
            }
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

        private void BT_EditOffer_Click(object sender, RoutedEventArgs e)
        {
            Classes.OfferManager offerManager = new Classes.OfferManager();
            Classes.Validation valid = new Classes.Validation();
            if (valid.IsTextNumeric(TB_Price.Text) && valid.IsNumberInRange(0, 2147483645, TB_Price.Text) && valid.DateTest(TB_Price.Text))
            {
                if (offerManager.EditOffer(currentOfferID, listID[CB_Listing.SelectedIndex], clientsID[CB_Client.SelectedIndex], Convert.ToInt32(TB_Price.Text), CB_Status.SelectedIndex, DP_Date.Text))
                {
                    DisplayNotifyBox("Offer added", "The offer from " + CB_Client.SelectedItem + " has been added", 5);
                    this.Close();
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

        private void RE_AddOfferOverlay_Closed(object sender, EventArgs e)
        {
            if (OnExit != null)
                OnExit(this, new EventArgs());
        }

        private void BT_Cancel_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
