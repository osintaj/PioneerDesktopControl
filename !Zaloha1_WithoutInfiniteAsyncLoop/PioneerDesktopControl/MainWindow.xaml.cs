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
using PrimS.Telnet;
using System.Text.RegularExpressions;

namespace PioneerDesktopControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String Radio_IP_address = "192.168.1.109";
        private int Radio_Port_Number = 8102;
        private int TimeoutMs = 1000;

        private bool Radio_On = false;
        private bool Mute_On = false;
        private int Volume = 0;

        PrimS.Telnet.Client TelnetClient;


        private async Task GetOnOffStatus()
        {           
            if (TelnetClient.IsConnected == true)
            {
                    
                await TelnetClient.Write("?P\r\n");
                string s = await TelnetClient.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(TimeoutMs));


                if (s.CompareTo("PWR0\r\n") == 0)
                {
                    Radio_On = true;
                    OnOffButton.Content = "Turn Off";
                }
                else
                {
                    Radio_On = false;  // PWR2 - is NetStandby
                    OnOffButton.Content = "Turn On";
                }
            }
        }

        private async Task GetMuteStatus()
        {
            if (TelnetClient.IsConnected == true)
            {
                await TelnetClient.Write("?M\r\n");
                string s = await TelnetClient.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(TimeoutMs));

                if (s.CompareTo("MUT1\r\n") == 0)
                {
                    Mute_On = false;
                    MuteButton.Content = "Mute";
                }
                else
                {
                    Mute_On = true;  // PWR2 - is NetStandby
                    MuteButton.Content = "Unmute";
                }
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

                string s = await TelnetClient.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(TimeoutMs));

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
                string s = await TelnetClient.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(TimeoutMs));

            }
        }


        private async Task SetInput(int input)
        {
            if (TelnetClient.IsConnected == true)
            {
                await TelnetClient.Write(input.ToString("00") + "FN\r\n");

                // This generate long output:
                string s = await TelnetClient.ReadAsync(TimeSpan.FromMilliseconds(TimeoutMs));
            }
        }


        private async Task SetRadioPreset(int input)
        {
            if (TelnetClient.IsConnected == true)
            {
                await TelnetClient.Write(input.ToString("00") + "PR\r\n");

                string s = await TelnetClient.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(TimeoutMs));
            }
        }

        public MainWindow()
        {
            InitializeComponent();

        }


        private void MainWindow1_Initialized(object sender, EventArgs e)
        {
            Radio_IP_address = RadioAddressTextBox.Text;

            ReconnectButton_Click(sender, null);            

        }

        private void EnableAppGUI(bool state)
        {
            Visibility vis;
            if (state)
            {
                vis = Visibility.Visible;
                ReconnectButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                vis = Visibility.Collapsed;
                ReconnectButton.Visibility = Visibility.Visible;
            }

            OnOffButton.Visibility = vis;
            MuteButton.Visibility = vis;
            VolDownButton.Visibility = vis;
            VolUpButton.Visibility = vis;
            SelectNextInputButton.Visibility = vis;
            SelInternetRadioButton.Visibility = vis;
            SelRadioButton.Visibility = vis;
            SelCDROMButton.Visibility = vis;
            SelUSBButton.Visibility = vis;

        }

        private async void ReconnectButton_Click(object sender, RoutedEventArgs e)
        {
            ReconnectButton.IsEnabled = false;
            try
            {
                TelnetClient = new PrimS.Telnet.Client(Radio_IP_address, Radio_Port_Number, new System.Threading.CancellationToken());

                await GetOnOffStatus();
                await GetMuteStatus();

                EnableAppGUI(true);
            }
            catch
            {
                EnableAppGUI(false);
            }

            ReconnectButton.IsEnabled = true;

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
              
                string s = await TelnetClient.TerminatedReadAsync("PWR", TimeSpan.FromMilliseconds(TimeoutMs));

                Radio_On = !Radio_On;

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

            await GetMuteStatus();
        }

        private async void SelPresetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (TelnetClient != null)
            {
                PresetTextBox.Text = ((int)SelPresetSlider.Value).ToString();
                
                await SetRadioPreset((int)SelPresetSlider.Value);

            }

        }
    }
}
