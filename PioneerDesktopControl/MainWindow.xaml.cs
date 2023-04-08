using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Net.NetworkInformation;

namespace PioneerDesktopControl
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String Radio_IP_address = "192.168.1.109";
        private int Radio_Port_Number = 8102;

        private bool Radio_On = false;
        private bool Mute_On = false;

        PrimS.Telnet.Client TelnetClient;

        private CancellationTokenSource ReaderCTS;
        private CancellationToken ReaderCT;

        PlayerCMDs playerCMDs = new CD_PlayerCMDs();

        const string RegRadioIPAddress = "Radio IP address";
        const string RegLocation = "Sintaj\\PioneerDesktopControl";


        private bool LoadRegSettings()
        {
            bool bSettingExists = false;

            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser;

            rk = rk.OpenSubKey("Software", true);

            if (rk.OpenSubKey(RegLocation, true) == null)
                rk = rk.CreateSubKey(RegLocation);
            else 
            {
                rk = rk.OpenSubKey(RegLocation, true);
                bSettingExists = true;
            }

            Radio_IP_address = (string)rk.GetValue(RegRadioIPAddress, "192.168.1.109");
            RadioAddressTextBox.Text = Radio_IP_address;

            rk.Close();

            return bSettingExists;
        }

        private void SaveRegSettings()
        {
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser;

            rk = rk.OpenSubKey("Software", true);

            rk = rk.OpenSubKey(RegLocation, true);
            if (rk == null)
                rk = rk.CreateSubKey(RegLocation);

            rk.SetValue(RegRadioIPAddress, RadioAddressTextBox.Text );

            rk.Close();
        }


        private async Task GetOnOffStatus()
        {           
            if (TelnetClient.IsConnected == true)
            {                    
                await TelnetClient.Write("\r\n?P\r\n");
            }
        }

        private async Task GetMuteStatus()
        {
            if (TelnetClient.IsConnected == true)
            {
                await TelnetClient.Write("\r\n?M\r\n");
            }
        }

        private async Task GetFunctionStatus()
        {
            if (TelnetClient.IsConnected == true)
            {
                await TelnetClient.Write("\r\n?F\r\n");
            }
        }


        private async Task SetVolume(bool up)
        {
            if (TelnetClient.IsConnected == true)
            {

                if (up)
                {
                    await TelnetClient.Write("VU\r\n");
                }
                else
                {
                    await TelnetClient.Write("VD\r\n");
                }

            }
        }

        private async Task SetMute(bool muted)
        {
            if (TelnetClient.IsConnected == true)
            {

                if (muted)
                {
                    await TelnetClient.Write("MO\r\n");
                }
                else
                {
                    await TelnetClient.Write("MF\r\n");
                }

            }
        }


        private async Task SetInput(int input)
        {
            if (TelnetClient.IsConnected == true)
            {
                await TelnetClient.Write(input.ToString("00") + "FN\r\n");
            }
        }


        private async Task SetRadioPreset(int input)
        {
            if (TelnetClient.IsConnected == true)
            {
                await TelnetClient.Write(input.ToString("00") + "PR\r\n");
            }
        }

        private async Task MainReaderLoop()
        {
            string s;
            string msg = "";

            await GetOnOffStatus();
            await GetMuteStatus();
            await GetFunctionStatus();

            s = "";

            while (! ReaderCT.IsCancellationRequested)
            {
                if (s.Length == 0)
                {                   
                    s = await TelnetClient.TerminatedReadAsync("\n", new TimeSpan(200));
                }

                if (s.IndexOf("\r\n") > 0)
                {
                    msg = s.Substring(0, s.IndexOf("\r\n"));
                    s = s.Substring(s.IndexOf("\r\n") + 2);  // if multiple lines were read, read just first one
                }

                if (msg.Length == 0)  // if there is no message, do not process anything
                    continue;

                if (msg.IndexOf("VOL") >= 0) // Top menu
                {
                    VolValueLabel.Content = Convert.ToInt16(msg.Substring(3)) ;
                }              

                if (msg.IndexOf("GCP01") >= 0) // Top menu
                {
                    infoLabel.Content = msg.Substring(10);
                }

                if (msg.IndexOf("GEP01032") >= 0)  // Radio station text, GEP02020 is Station name
                {
                    infoLabel.Content = msg.Substring(8);
                }

                if ((msg.IndexOf("GEP") >= 0) && (msg[5] == '1')) // 5. bit means active menu item
                {
                    infoLabel.Content += " ->" + msg.Substring(8);
                    //playerCMDs.SetIndex(Convert.ToInt16(msg.Substring(3,2))) ; // This will not work due to paging which I have not implemented
                }

                if (msg.CompareTo("MUT1") == 0)
                {
                    Mute_On = false;
                    MuteButton.Content = "Mute";
                }
                else if (msg.CompareTo("MUT0") == 0)
                {
                    Mute_On = true; 
                    MuteButton.Content = "Unmute";
                }
                else if (msg.CompareTo("PWR0") == 0)
                {
                    Radio_On = true;
                    OnOffButton.Content = "Turn OFF (NET standby)";
                }
                else if ( (msg.StartsWith("FNaniFRtoNieJe") ) && (msg.Length == 4) ) 
                {
                    // Funkcia na vyslanie aktualnejs tanice asi neexistuje

                    //SelPresetSlider.Value = Convert.ToInt32(msg.Substring(2, 2));

                }
                else if (msg.CompareTo("PWR1") == 0)
                {
                    Radio_On = false;  // PWR1 - is not NetStandby
                    OnOffButton.Content = "Turn ON via remote control only";
                }
                else if (msg.CompareTo("PWR2") == 0)
                {
                    Radio_On = false;  // PWR2 - is NetStandby
                    OnOffButton.Content = "Turn ON";
                }
                else if (msg.CompareTo("FN01") == 0)
                {
                    playerCMDs = new CD_PlayerCMDs();
                    infoLabel.Content = "CD player is active";
                }
                else if (msg.CompareTo("FN02") == 0)
                {
                    playerCMDs = new RADIO_PlayerCMDs();
                    infoLabel.Content = "Radio active";
                }
                else if (msg.CompareTo("FN38") == 0)
                {
                    playerCMDs = new Internet_PlayerCMDs();
                    infoLabel.Content = "Internet radio is active";
                }
                else if (msg.CompareTo("FN17") == 0)
                {
                    playerCMDs = new USB_PlayerCMDs();
                    infoLabel.Content = "USB player is active";
                }

                msg = ""; // sometimes, there is no end of line character in msg. Message was processed, so clean it.

            }

            // MessageBox.Show("Reader loop ended.");

            DisconnectButton_Click(null, null);

        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private async Task Reconnect()
        {
            Radio_IP_address = RadioAddressTextBox.Text;
            ReconnectButton.IsEnabled = false;
            ReconnectButton.Content = "connecting ...";

            ReconnectButton.InvalidateVisual();
            ReconnectButton.UpdateLayout();

            DoEvents();

            try
            {
                TelnetClient = new PrimS.Telnet.Client(Radio_IP_address, Radio_Port_Number, new System.Threading.CancellationToken() );

                ReaderCTS = new CancellationTokenSource();
                ReaderCT = ReaderCTS.Token;              

                while (!TelnetClient.IsConnected)
                {
                    Thread.Sleep(20);
                    DoEvents();
                }

                EnableAppGUI(true);

                await MainReaderLoop();
                
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString());
                MessageBox.Show("Unable to connect. \n\n" + ex.Message, "Remote control for Pioneer - connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                EnableAppGUI(false);
            }

            ReconnectButton.Content = "Reconnect";
            ReconnectButton.IsEnabled = true;
        }

        public MainWindow()
        {
            InitializeComponent();
        }


        private async void MainWindow1_Initialized(object sender, EventArgs e)
        {            
            Microsoft.Win32.SystemEvents.PowerModeChanged += OnPowerModeChanged;

            NetworkChange.NetworkAvailabilityChanged += AvailabilityChanged;

            //NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);

            if (LoadRegSettings() == true)
            {
                await Reconnect();
            } else
            {
                EnableAppGUI(false);
            }
        }

        private void AvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (! e.IsAvailable)
            {                   
                if (TelnetClient != null)
                {
                    ReaderCTS.Cancel();
                }                
            }
        }

        private void OnPowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            // Disconnect from HiFi system when system is suspended or hibernated.

            if (e.Mode == Microsoft.Win32.PowerModes.Suspend)
            {
                DisconnectButton_Click(null, null);
            }
        
        }

        private async void ReconnectButton_Click(object sender, RoutedEventArgs e)
        {
            await Reconnect();

        }

        private async void OnOffButton_Click(object sender, RoutedEventArgs e)
        {
            if (TelnetClient.IsConnected == true)
            {
                OnOffButton.IsEnabled = false;

                if (Radio_On)
                {
                    await TelnetClient.Write("PF\r\n");
                    OnOffButton.Content = "Turn ON";
                } else
                {                  
                    await TelnetClient.Write("PO\r\n");
                    OnOffButton.Content = "Turn OFF (NET standby)";
                }
                              
                OnOffButton.IsEnabled = true;
            }

        }

        private async void VolUpButton_Click(object sender, RoutedEventArgs e)
        {
            await SetVolume(true);
        }

        private async void VolDownButton_Click(object sender, RoutedEventArgs e)
        {
            await SetVolume(false);
        }

        private async void SelRadioButton_Click(object sender, RoutedEventArgs e)
        {
            await SetInput(2);
        }

        private async void SelCDROMButton_Click(object sender, RoutedEventArgs e)
        {
            await SetInput(1);
        }

        private async void SelInternetRadioButton_Click(object sender, RoutedEventArgs e)
        {
            await SetInput(38);
        }

        private async void SelUSBButton_Click(object sender, RoutedEventArgs e)
        {
            await SetInput(17);
        }

        private async void MuteButton_Click(object sender, RoutedEventArgs e)
        {           
            if (Mute_On)
            {
                await SetMute(false);
            } else
            {
                await SetMute(true);
            }
        }

        private async void SelPresetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TelnetClient != null)
            {
                PresetTextBox.Text = ((int)SelPresetSlider.Value).ToString();
                
                await SetRadioPreset((int)SelPresetSlider.Value);
            }
        }

        private void MainWindow1_Closed(object sender, EventArgs e)
        {
            DisconnectButton_Click(null, null);

            SaveRegSettings();
        }

        private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            DisconnectButton.IsEnabled = false;


            if (TelnetClient != null)
            {
                ReaderCTS.Cancel();
                await GetOnOffStatus();

                //TelnetClient.Dispose();  // This was showing error that TelnetClient is null
                //TelnetClient = null;
            }

            EnableAppGUI(false);            
        }

        private async void IP_label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            await GetOnOffStatus();
            await GetMuteStatus();         
        }

        private void EnableAppGUI(bool state)
        {
            Visibility vis;
            if (state)
            {
                vis = Visibility.Visible;
                ReconnectButton.Visibility = Visibility.Collapsed;
                DisconnectButton.IsEnabled = true;
                RadioAddressTextBox.IsEnabled = false;
            }
            else
            {
                vis = Visibility.Collapsed;
                ReconnectButton.Visibility = Visibility.Visible;
                DisconnectButton.IsEnabled = false;
                RadioAddressTextBox.IsEnabled = true;
            }

            OnOffButton.Visibility = vis;
            MuteButton.Visibility = vis;
            VolDownButton.Visibility = vis;
            VolUpButton.Visibility = vis;
            playerButtons.Visibility = vis;            
            SelInternetRadioButton.Visibility = vis;
            SelRadioButton.Visibility = vis;
            SelCDROMButton.Visibility = vis;
            SelUSBButton.Visibility = vis;
            // RadioStationPanel.Visibility = vis;
            RadioStationPanel2.Visibility = vis;

        }

        private async void SendCMD(string cmd)
        {
            if (TelnetClient.IsConnected == true)
            {
                await TelnetClient.Write(cmd + "\r\n");
            }
        }

        private void SelPlayButton_Click(object sender, RoutedEventArgs e)
        {
            SendCMD(playerCMDs.getPlayCMD());
        }

        private void SelPauseButton_Click(object sender, RoutedEventArgs e)
        {
            SendCMD(playerCMDs.getPauseCMD());
        }

        private void SelStopButton_Click(object sender, RoutedEventArgs e)
        {
            SendCMD(playerCMDs.getStopCMD());
        }

        private void SelNextButton_Click(object sender, RoutedEventArgs e)
        {
            SendCMD(playerCMDs.getNextCMD());
        }

        private void SelPrevButton_Click(object sender, RoutedEventArgs e)
        {
            SendCMD(playerCMDs.getPrevCMD());
        }

        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private async void selStationButton_Click(object sender, RoutedEventArgs e)
        {
            if (TelnetClient != null)
            {
                //PresetTextBox.Text = ((int)SelPresetSlider.Value).ToString();

                if (sender is Button)
                {
                    await SetRadioPreset( Int32.Parse((sender as Button).Tag.ToString()) );
                }
                
            }
        }
    }
}
