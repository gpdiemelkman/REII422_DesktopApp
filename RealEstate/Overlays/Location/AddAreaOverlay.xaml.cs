﻿using System;
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

namespace RealEstate.Overlays.Location
{
    /// <summary>
    /// Interaction logic for AddAreaOverlay.xaml
    /// </summary>
    public partial class AddAreaOverlay
    {
        List<int> provinceID= new List<int>();
        public event EventHandler OnExit;

        public AddAreaOverlay()
        {
            InitializeComponent();
        }

        private void RE_AddArea_Closed(object sender, EventArgs e)
        {
            if (OnExit != null)
                OnExit(this, new EventArgs());
        }

        private void RE_AddArea_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProvinces();
            CB_Provinces.SelectedIndex = 0;
        }

        private void BT_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BT_Add_Click(object sender, RoutedEventArgs e)
        {
            AddArea();
        }

        #region Form Control
        private void LoadProvinces()
        {
            new System.Threading.Thread(() =>
            {
                Classes.DatabaseManager dbManger = new Classes.DatabaseManager();
                ClearCities();
                var provs = dbManger.ReturnQuery("SELECT Province_Name, Province_ID FROM Province ORDER BY Province_Name;");

                foreach (var prov in provs)
                {
                    InsertProvince(prov[0]);
                    provinceID.Add(Convert.ToInt32(prov[1]));
                }

            }).Start();
        }
        private void LoadCities(int currentProvince)
        {
            new System.Threading.Thread(() =>
                {
                    Classes.DatabaseManager dbManger = new Classes.DatabaseManager();
                    ClearCities();
                    var cities = dbManger.ReturnQuery("SELECT City_Name FROM City WHERE City_Province_ID =" + currentProvince + " ORDER BY City_Name;");

                    foreach( var city in cities)
                    {
                        InsertCity(city[0]);
                    }
                   
                }).Start();
        }

        private void InsertCity(string cityName)
        {
            this.Dispatcher.Invoke(() =>
            {
                CB_Cities.Items.Add(cityName);
            });
        }
        private void InsertProvince(string provinceName)
        {
            this.Dispatcher.Invoke(() =>
            {
                CB_Provinces.Items.Add(provinceName);
            });
        }

        private void ClearCities()
        {
            this.Dispatcher.Invoke(() =>
            {
                CB_Cities.Items.Clear();
            });
        }
        private void ClearProvinces()
        {
            this.Dispatcher.Invoke(() =>
            {
                CB_Provinces.Items.Clear();
            });
        }

        private void SetLoadingState(bool loading)
        {
            this.Dispatcher.Invoke(() =>
                {
                    if( loading )
                    {
                        CB_Cities.IsEnabled = false;
                        CB_Provinces.IsEnabled = false;
                        TB_AreaName.IsEnabled = false;
                        BT_Add.IsEnabled = false;
                        BT_Close.IsEnabled = false;
                        this.IsBusy = true;
                    }
                    else
                    {
                        CB_Cities.IsEnabled = true;
                        CB_Provinces.IsEnabled = true;
                        TB_AreaName.IsEnabled = true;
                        BT_Add.IsEnabled = true;
                        BT_Close.IsEnabled = true;
                        this.IsBusy = false;
                    }
                });
        }

        private void ClearForm()
        {
            this.Dispatcher.Invoke(() =>
                {
                    TB_AreaName.Text = "";
                    CB_Cities.SelectedIndex = 0;
                    CB_Provinces.SelectedIndex = 0;
                });
        }

        private void DisplayNotifyBox(string title, string message, int durationSeconds)
        {
            this.Dispatcher.Invoke(() =>
            {
                NotifyBox.Show(null, title, message, new TimeSpan(0, 0, durationSeconds), false);
            });
        }


        #endregion

        #region Get
        private string GetAreaName()
        {
            string areaName = "";

            this.Dispatcher.Invoke(() =>
                {
                    areaName = TB_AreaName.Text;
                });

            return areaName;
        }

        private string GetCityName()
        {
            string cityName = "";

            this.Dispatcher.Invoke(() =>
                {
                    cityName = CB_Cities.SelectedValue.ToString();
                });

            return cityName;
        }

        private string GetProvinceName()
        {
            string provinceName = "";

            this.Dispatcher.Invoke(() =>
                {
                    provinceName = CB_Provinces.SelectedValue.ToString();                  
                });

            return provinceName;
        }

        #endregion

        private void AddArea()
        {
            new System.Threading.Thread(() =>
                {
                    if( GetAreaName() != "")
                    {
                        Classes.LocationManager locationManager = new Classes.LocationManager();

                        SetLoadingState(true);

                        if( locationManager.CanAddArea(GetAreaName(),GetCityName(),GetProvinceName()) )
                        {
                            if (locationManager.AddArea(GetAreaName(), GetCityName(), GetProvinceName()))
                            {
                                DisplayNotifyBox("Area Added", GetAreaName() + "," + GetCityName() + "," + GetProvinceName() + " was added", 3);
                                ClearForm();
                            }
                            else
                            {
                                DisplayNotifyBox("Could not add", "Area not added please try again", 3);
                            }
                        }
                        else
                        {
                            DisplayNotifyBox("Cannot Add", GetAreaName() + " already exists in " + GetCityName() + "," + GetProvinceName(), 3);
                        }

                        SetLoadingState(false);
                    }
                    else
                    {
                        DisplayNotifyBox("ERROR", "Area name cannot be empty", 3);
                    }

                }).Start();
        }

        private void CB_Provinces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_Provinces.SelectedIndex != -1 && provinceID.Count !=0 )
                LoadCities(provinceID[CB_Provinces.SelectedIndex]);
        }

        

        
    }
}
