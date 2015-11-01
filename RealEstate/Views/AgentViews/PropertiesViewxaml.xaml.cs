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
using RealEstate.Classes;
using RealEstate.GridViewSources;
using RealEstate.Windows;
using RealEstate.Overlays.Client;
using Framework.UI.Controls;

namespace RealEstate.Views.AgentViews
{
    /// <summary>
    /// Interaction logic for PropertiesView.xaml
    /// </summary>
    public partial class PropertiesView
    {
        public PropertiesView()
        {
            InitializeComponent();
            LoadSearchCriteria();
        }

        private void RE_PropertiesView_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshProperties();
        }

        #region Input Response

        private void BT_Search_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void DG_Properties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG_Properties.SelectedIndex != -1)
            {
                TB_SelectedProperty.Text = GetSelectedAgent();
            }
            else
            {
                TB_SelectedProperty.Text = "";
            }
        }

        private void BT_Back_Click(object sender, RoutedEventArgs e)
        {
            ShowAgentWindow();
        }

        private void BT_Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshProperties();
        }

        #endregion

        #region Form Control

        private void RefreshProperties()
        {
            new System.Threading.Thread(() =>
            {
                ClearPropertiesGrid();
                DatabaseManager dbManager = new DatabaseManager();
                var properties = dbManager.ReturnQuery("SELECT Property_ID, Property_Bedroom_Count, Property_Bathroom_Count, Property_Garage_Count, Property_hasPool, Property_Plot_Size, Property_House_Size FROM Property ORDER BY Property_ID ASC;");

                foreach (var property in properties)
                {
                    DatabaseManager dbManager2 = new DatabaseManager();
                    var agentIDs = dbManager2.ReturnQuery("SELECT Agent_ID FROM Listing WHERE Property_ID = " + Convert.ToInt32(property[0]) + ";");
                    foreach (var agentID in agentIDs)
                    {
                        DatabaseManager dbManager3 = new DatabaseManager();
                        var agentEmails = dbManager3.ReturnQuery("SELECT Agent_Email FROM Agent WHERE Agent_ID = " + Convert.ToInt32(agentID[0]) + ";");
                        foreach (var agentEmail in agentEmails)
                        {
                            InsertIntoPropertiesGrid(Convert.ToString(agentEmail[0]), Convert.ToInt32(property[1]), Convert.ToInt32(property[2]), Convert.ToInt32(property[3]), Convert.ToInt32(property[4]), Convert.ToInt32(property[5]), Convert.ToInt32(property[6]));
                        }
                    }
                }
            }).Start();
        }

        private void ClearPropertiesGrid()
        {
            this.Dispatcher.Invoke(() =>
            {
                DG_Properties.Items.Clear();
            });
        }

        private void InsertIntoPropertiesGrid(string agent, int bedrooms, int bathrooms, int garages, int pool, int plot_Size, int house_Size)
        {
            this.Dispatcher.Invoke(() =>
            {
                DG_Properties.Items.Add(new Property() { Agent = agent, Bedrooms = bedrooms, Bathrooms = bathrooms, Garages = garages, Pool = pool, Plot_Size = plot_Size, House_Size = house_Size });
            });
        }

        private void ShowAgentWindow()
        {
            (this.Tag as AgentWindow).AV_ViewProperties.Visibility = System.Windows.Visibility.Hidden;
            (this.Tag as AgentWindow).BT_ManageCustomers.Visibility = System.Windows.Visibility.Visible;
            (this.Tag as AgentWindow).BT_Listings.Visibility = System.Windows.Visibility.Visible;
            (this.Tag as AgentWindow).BT_Manage.Visibility = System.Windows.Visibility.Visible;
            (this.Tag as AgentWindow).BT_Properties.Visibility = System.Windows.Visibility.Visible;
        }

        private void LoadSearchCriteria()
        {
            CB_SearchField.Items.Add("Bedrooms");
            CB_SearchField.Items.Add("Bathrooms");
            CB_SearchField.Items.Add("Garages");
            CB_SearchField.Items.Add("Has a Pool");
            CB_SearchField.Items.Add("Plot Size");
            CB_SearchField.Items.Add("House Size");
            CB_SearchField.SelectedIndex = 0;
        }

        private void Search()
        {
            new System.Threading.Thread(() =>
            {
                ClearPropertiesGrid();

                int searchColumn = GetSelectedSearchField();

                DatabaseManager dbManager = new DatabaseManager();

                var properties = dbManager.ReturnQuery("SELECT Property_ID, Property_Bedroom_Count, Property_Bathroom_Count, Property_Garage_Count, Property_hasPool, Property_Plot_Size, Property_House_Size FROM Property ORDER BY Property_ID ASC;");

                foreach (var property in properties)
                {
                    var agentIDs = dbManager.ReturnQuery("SELECT Agent_ID FROM Listing WHERE Property_ID = " + Convert.ToInt32(property[0]) + ";");
                    foreach (var agentID in agentIDs)
                    {
                        var agentEmails = dbManager.ReturnQuery("SELECT Agent_Email FROM Agent WHERE Agent_ID = " + Convert.ToInt32(agentID[0]) + ";");
                        foreach (var agentEmail in agentEmails)
                        {
                            if (property[searchColumn].ToString().Contains(GetSearchValue()))
                            {
                                InsertIntoPropertiesGrid(Convert.ToString(agentEmail[0]), Convert.ToInt32(property[1]), Convert.ToInt32(property[2]), Convert.ToInt32(property[3]), Convert.ToInt32(property[4]), Convert.ToInt32(property[5]), Convert.ToInt32(property[6]));
                            }
                        }
                    }
                }
            }).Start();
        }

        #endregion

        #region Get

        private string GetSearchValue()
        {
            string searchValue = "";

            this.Dispatcher.Invoke(() =>
            {
                searchValue = TB_SearchValue.Text;
            });

            return searchValue;
        }

        private int GetSelectedSearchField()
        {
            int searchField = 0;

            this.Dispatcher.Invoke(() =>
            {
                searchField = CB_SearchField.SelectedIndex + 1;
            });

            return searchField;
        }

        private string GetSelectedAgent()
        {
            string selectedEmail = "";

            this.Dispatcher.Invoke(() =>
            {
                selectedEmail = (DG_Properties.SelectedItem as GridViewSources.Property).Agent;
            });

            return selectedEmail;
        }

        #endregion
    }
}
