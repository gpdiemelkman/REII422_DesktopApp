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

namespace RealEstate.Overlays.Client
{
    /// <summary>
    /// Interaction logic for AddClientOverlay.xaml
    /// </summary>
    public partial class AddClientOverlay 
    {
        public event EventHandler OnExit;

        public AddClientOverlay()
        {
            InitializeComponent();
        }

        private void BT_AddClient_Click(object sender, RoutedEventArgs e)
        {
            if (PB_ConfirmPassword.Password.ToString() == PB_Password.Password.ToString())
            {
                Classes.Validation valid = new Classes.Validation();
                if (valid.TextHasNumber(GetPassword()) && valid.TextHasSpecialChars(GetPassword()) && valid.TextIsLongerThan(GetPassword(), 8) && valid.TextIsShorterThan(GetPassword(), 32) && valid.TextContainsUpperCase(GetPassword()))
                {
                    if (valid.TextIsShorterThan(GetName(), 32) && valid.TextIsShorterThan(GetSurname(), 32) && valid.TextIsShorterThan(GetPhone(), 12))
                    {
                        if (valid.IsTextNumeric(GetPhone()))
                        {
                            if (valid.TextisEmail(GetEmail()) && valid.TextIsShorterThan(GetEmail(), 32))
                            {
                                AddClient();
                            }
                            else
                            {
                                DisplayNotifyBox("ERROR", "Please use a valid email address with length shorter than 32 characters", 10);
                            } 
                        }
                        else
                        {
                            DisplayNotifyBox("ERROR", "Please use a valid cellphone number with local 099 999 999 format", 10);
                        }
                    }
                    else
                    {
                        DisplayNotifyBox("ERROR", "Please ensure all fields have lengths shorter than 32 characters", 10);
                    } 
                }
                else
                {
                    DisplayNotifyBox("ERROR", "Your password must contain at least one of each of the following: a special character, a number and an uppercase letter. It must also have a length of between 8 and 32 characters.", 10);
                }  
            }
            else
            {
                DisplayNotifyBox("ERROR", "The passwords do not match", 2);
            }
        }

        private void RE_AddClientOverlay_Closed(object sender, EventArgs e)
        {
            if (OnExit != null)
                OnExit(this, new EventArgs());
        }

        #region Get
        private string GetName()
        {
            string name = "";

            this.Dispatcher.Invoke(() =>
            {
                name = TB_Name.Text;
            });

            return name;
        }

        private string GetSurname()
        {
            string surname = "";

            this.Dispatcher.Invoke(() =>
            {
                surname = TB_Surname.Text;
            });

            return surname;
        }

        private string GetPhone()
        {
            string phone = "";

            this.Dispatcher.Invoke(() =>
            {
                phone = TB_Phone.Text;
            });

            return phone;
        }

        private string GetEmail()
        {
            string email = "";

            this.Dispatcher.Invoke(() =>
            {
                email = TB_Email.Text;
            });

            return email;
        }

        private string GetPassword()
        {
            string password = "";

            this.Dispatcher.Invoke(() =>
            {
                password = PB_Password.Password.ToString();
            });

            return password;
        }


        #endregion

        #region Set From Components
        private void DisplayNotifyBox(string title, string message, int durationSeconds)
        {
            this.Dispatcher.Invoke(() =>
            {
                NotifyBox.Show(null, title, message, new TimeSpan(0, 0, durationSeconds), false);
            });
        }

        private void SetLoadingState(bool loading)
        {
            this.Dispatcher.Invoke(() =>
            {
                RE_AddClientOverlay.IsBusy = loading;

                if (loading)
                {
                    TB_Name.IsEnabled = false;
                    TB_Surname.IsEnabled = false;
                    TB_Phone.IsEnabled = false;
                    TB_Email.IsEnabled = false;
                    PB_Password.IsEnabled = false;
                    PB_ConfirmPassword.IsEnabled = false;
                }
                else
                {
                    TB_Name.IsEnabled = true;
                    TB_Surname.IsEnabled = true;
                    TB_Phone.IsEnabled = true;
                    TB_Email.IsEnabled = true;
                    PB_Password.IsEnabled = true;
                    PB_ConfirmPassword.IsEnabled = true;
                }
            });
        }

        private void ClearForm()
        {
            this.Dispatcher.Invoke(() =>
            {
                TB_Name.Text = "";
                TB_Surname.Text = "";
                TB_Phone.Text = "";
                TB_Email.Text = "";
                PB_Password.Password = "";
                PB_ConfirmPassword.Password = "";
            });
        }

        #endregion

        private void AddClient()
        {
            new System.Threading.Thread(() =>
            {
                Classes.ClientManager clientManager = new Classes.ClientManager();

                if (clientManager.CanAddClient(GetEmail()))
                {
                    SetLoadingState(true);

                    if (clientManager.AddClient(GetName(), GetSurname(), GetPhone(), GetEmail(), GetPassword()))
                    {
                        DisplayNotifyBox("Client Added", GetName() + " has been successfully added", 2);
                    }
                    else
                    {
                        DisplayNotifyBox("ERROR", "Could not add " + GetName(), 2);
                    }

                    ClearForm();
                }
                else
                {
                    DisplayNotifyBox("Username Exists Already", "Cannot add " + GetName() + ", the email " + GetEmail() + " exits already", 3);
                }

                SetLoadingState(false);
            }).Start();


        }
    }
}
