using FRCA.ControlPlane.NOS.FlowEngin;
using FRCA.Dataplane;
using FRCA.Properties;
using FRCA.ui;
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

namespace FRCA.ExpermentsResults.Energy_consumptions
{
    /// <summary>
    /// Interaction logic for UISetParEnerConsum.xaml
    /// </summary>
    public partial class UISetParEnerConsum : Window
    {
        MainWindow _MainWindow;
        public UISetParEnerConsum(MainWindow __MainWindow_)
        {
            InitializeComponent();
            _MainWindow = __MainWindow_;

            for (int i = 5; i <= 50; i++)
            {
                com_UpdateLossPercentage.Items.Add(i);
            }
            com_UpdateLossPercentage.Text = Settings.Default.UpdateLossPercentage.ToString();


            for (int i = 60; i <= 1000; i = i + 60)
            {
                comb_simuTime.Items.Add(i);

            }
            comb_simuTime.Text = "300";

            comb_packet_rate.Items.Add("0.001");
            comb_packet_rate.Items.Add("0.01");
            comb_packet_rate.Items.Add("0.1");
            comb_packet_rate.Items.Add("0.5");
            comb_1st_num.Items.Add("2000");
            comb_1st_num.Items.Add("4000");
            comb_1st_num.Items.Add("6000");
            comb_1st_num.Items.Add("8000");
            comb_1st_num.Items.Add("10000");

            comb_2nd_num.Items.Add("8000");
            comb_2nd_num.Items.Add("1000000");
            for (int i = 1; i <= 5; i++)
            {
                comb_packet_rate.Items.Add(i);
            }

            comb_packet_rate.Text = "0.1";

            for (int i = 5; i <= 15; i++)
            {
                comb_startup.Items.Add(i);
            }
            comb_startup.Text = "10";

            for (int i = 1; i <= 5; i++)
            {
                comb_active.Items.Add(i);
                comb_sleep.Items.Add(i);
            }
            comb_active.Text = "1";
            comb_sleep.Text = "2";
        }


        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {

            Settings.Default.UpdateLossPercentage = Convert.ToInt16(com_UpdateLossPercentage.Text);
            Settings.Default.StopeWhenFirstNodeDeid = Convert.ToBoolean(chk_stope_when_first_node_deis.IsChecked);


            if (Settings.Default.StopeWhenFirstNodeDeid == false)
            {
                int stime = Convert.ToInt16(comb_simuTime.Text);

                double packetRate = Convert.ToDouble(comb_packet_rate.Text);
                _MainWindow.stopSimlationWhen = 100000000;
                _MainWindow.RandomDeplayment(0);
                double numpackets = Convert.ToDouble(stime) / packetRate;
                _MainWindow.GenerateUplinkPacketsRandomly(Convert.ToInt32(numpackets));
                _MainWindow.PacketRate = "1 packet per " + packetRate + " s";
            }
            else if (Settings.Default.StopeWhenFirstNodeDeid == true)
            {
                int stime = 100000000;
                double packper = Convert.ToDouble(comb_packet_rate.Text);
                _MainWindow.stopSimlationWhen = stime;
                _MainWindow.RandomDeplayment(0);
                _MainWindow.SendPackectPerSecond(packper);

            }

            //将EC的定义从given time 改成given packets number
            //int stime = 50;

            //double packetRate = 0.1;
            //_MainWindow.stopSimlationWhen = stime;
            //_MainWindow.RandomDeplayment(0);
            //double numpackets = Convert.ToDouble(stime) / packetRate;
            //_MainWindow.GenerateUplinkPacketsRandomly(Convert.ToInt32(numpackets));
            //_MainWindow.PacketRate = "1 packet per " + packetRate + " s";



            Close();

        }

        private void btn_ok_Click_1(object sender, RoutedEventArgs e)
        {
            // Runs the Exp 2
            _MainWindow.stopSimlationWhen = 100000000;
            _MainWindow.RandomDeplayment_Exp2(0);
            _MainWindow.GenerateUplinkPacketsRandomly_Exp2(Convert.ToInt16(comb_1st_num.Text), Convert.ToInt32(comb_2nd_num.Text));
            //double numpackets = Convert.ToDouble(stime) / packetRate;
            //_MainWindow.GenerateUplinkPacketsRandomly(Convert.ToInt32(numpackets));
            //_MainWindow.PacketRate = "1 packet per " + packetRate + " s";
            Close();





        }


        private void comb_startup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object objval = comb_startup.SelectedItem as object;
            int va = Convert.ToInt16(objval);
            Settings.Default.MacStartUp = va;
        }

        private void comb_active_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object objval = comb_active.SelectedItem as object;
            int va = Convert.ToInt16(objval);
            Settings.Default.ActivePeriod = va;
        }

        private void comb_sleep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object objval = comb_sleep.SelectedItem as object;
            int va = Convert.ToInt16(objval);
            Settings.Default.SleepPeriod = va;
        }

        private void chk_stope_when_first_node_deis_Checked(object sender, RoutedEventArgs e)
        {
            comb_simuTime.IsEnabled = false;
        }

        private void chk_stope_when_first_node_deis_Unchecked(object sender, RoutedEventArgs e)
        {
            comb_simuTime.IsEnabled = true;
        }

      

        private void chk_drawrouts_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowRoutingPaths = true;
        }

        private void chk_drawrouts_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowRoutingPaths = false;
        }

        private void chk_save_logs_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.SaveRoutingLog = true;
        }

        private void chk_save_logs_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.SaveRoutingLog = false;
        }

        private void chek_show_radar_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowRadar = true;
        }

        private void chek_show_radar_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowRadar = false;
        }

        private void chek_animation_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowAnimation = true;
        }

        private void chek_animation_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowAnimation = false;
        }
    }
}
