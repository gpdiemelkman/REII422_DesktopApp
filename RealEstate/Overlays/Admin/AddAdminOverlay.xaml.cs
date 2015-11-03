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

namespace RealEstate.Overlays.Admin
{
    /// <summary>
    /// Interaction logic for AddAdminOverlay.xaml
    /// </summary>
    public partial class AddAdminOverlay
    {
        public event EventHandler OnExit;

        public AddAdminOverlay()
        {
            InitializeComponent();
        }

        private void BT_AddAdmin_Click(object sender, RoutedEventArgs e)
        {
            Classes.Validation valid = new Classes.Validation();
            if (PB_ConfirmPassword.Password.ToString() == PB_Password.Password.ToString())
            {
                if (valid.TextHasNumber(PB_Password.Password.ToString()) && valid.TextHasSpecialChars(PB_Password.Password.ToString()) && valid.TextIsLongerThan(PB_Password.Password.ToString(), 8) && valid.TextIsShorterThan(PB_Password.Password.ToString(), 32) && valid.TextContainsUpperCase(PB_Password.Password.ToString()))
                {
                    AddAdmin();
                }
                else
                {
                    DisplayNotifyBox("ERROR", "Your password must contain at least one of each of the following: a special character, a number and an uppercase letter. It must also have a length of between 8 and 32 characters.", 10);
                }
            }
            else
            {
                DisplayNotifyBox("ERROR", "The passwords do not match", 5);
            }           
        }

        private void RE_AddAdminOverlay_Closed(object sender, EventArgs e)
        {
            if (OnExit != null)
                OnExit(this, new EventArgs());
        }


        #region Get
        private string GetUsername()
        {
            string username = "";

            this.Dispatcher.Invoke(() =>
            {
                username = TB_Username.Text;
            });

            return username;
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
                RE_AddAdminOverlay.IsBusy = loading;

                if (loading)
                {
                    TB_Username.IsEnabled = false;
                    PB_Password.IsEnabled = false;
                    PB_ConfirmPassword.IsEnabled = false;
                }
                else
                {
                    TB_Username.IsEnabled = true;
                    PB_Password.IsEnabled = true;
                    PB_ConfirmPassword.IsEnabled = true;
                }
            });
        }

        private void ClearForm()
        {
            this.Dispatcher.Invoke(() =>
            {
                TB_Username.Text = "";
                PB_Password.Password = "";
                PB_ConfirmPassword.Password = "";
            });
        }

        #endregion

        private void AddAdmin()
        {
            new System.Threading.Thread(() =>
            {
                Classes.AdminManager adminManager = new Classes.AdminManager();

                if( adminManager.CanAddAdmin(GetUsername()) )
                {
                    Classes.Validation valid = new Classes.Validation();
                    SetLoadingState(true);
                    if (valid.TextIsShorterThan(GetUsername(), 32))
                    {
                        if (adminManager.AddAdmin(GetUsername(), GetPassword()))
                        {
                            DisplayNotifyBox("Admin Added", GetUsername() + " has been successfully added", 5);
                            ClearForm();
                        }
                        else
                        {
                            DisplayNotifyBox("ERROR", "Could not add " + GetUsername(), 5);
                        }
                    }
                    else
                    {
                        DisplayNotifyBox("ERROR", "Please use a username with length shorter than 32 characters", 10);
                    }  
                }
                else
                {
                    DisplayNotifyBox("Username Exists Already", "Cannot add " + GetUsername() + ", the username exits already", 5);
                }

                SetLoadingState(false);
            }).Start();


        }

        
    }
}
