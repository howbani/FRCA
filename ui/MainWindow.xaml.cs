using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FRCA.Dataplane;
using FRCA.db;
using FRCA.Intilization;
using FRCA.Coverage;
using FRCA.Properties;
using System.Windows.Media;
using System.Windows.Threading;
using FRCA.Forwarding;
using FRCA.ExpermentsResults.Energy_consumptions;
using FRCA.ExpermentsResults.Lifetime;
using FRCA.ControlPlane.NOS.TC;
using FRCA.ControlPlane.NOS.TC.subgrapgh;
using FRCA.DataPlane.NeighborsDiscovery;
using FRCA.ControlPlane.NOS.FlowEngin;
using FRCA.ui.conts;
using FRCA.ControlPlane.NOS.Visualizating;
using System.Threading;
using FRCA.Dataplane.NOS;

namespace FRCA.ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Exp2ReprotForm
    {
        public double ec = 0;
        public double awt = 0;
        public double arxt = 0;
    }

    public partial class MainWindow : Window
    {
        public string PacketRate { get; set; }
        public Int32 stopSimlationWhen = 1000000000; // s by defult.
        public DispatcherTimer TimerCounter = new DispatcherTimer();
        public DispatcherTimer RandomSelectSourceNodesTimer = new DispatcherTimer();
        public static double Swith;// sensing feild width.
        public static double Sheigh;// sensing feild height.

        /// <summary>
        /// the area of sensing feild.
        /// </summary>
        public static double SensingFeildArea
        {
            get
            {
                return Swith * Sheigh;
            }
        }

        public List<Vertex> MyGraph = new List<Vertex>();
        public List<Sensor> myNetWork = new List<Sensor>();

        bool isCoverageSelected = false;


        public MainWindow()
        {
            InitializeComponent();
            // sensing feild
            Swith = Canvas_SensingFeild.Width - 218;
            Sheigh = Canvas_SensingFeild.Height - 218;
            PublicParamerters.SensingFeildArea = SensingFeildArea;
            PublicParamerters.MainWindow = this;
            // battery levels colors:
            FillColors();

            PublicParamerters.RandomColors = RandomColorsGenerator.RandomColor(100); // 100 diffrent colores.

            /*
            List<UplinkFlowEnery> list = DistrubtionsTests.TestHvalue(5, 1);
            ListControl HList = new ui.conts.ListControl();
            HList.dg_date.ItemsSource = list;
            UiShowLists win = new UiShowLists();
            win.stack_items.Children.Add(HList);
            win.Show();
            win.WindowState = WindowState.Maximized;*/


            _show_id.IsChecked = Settings.Default.ShowID;
            _show_battrey.IsChecked = Settings.Default.ShowBattry;
            _show_sen_range.IsChecked= Settings.Default.ShowSensingRange;
            _show_com_range.IsChecked= Settings.Default.ShowComunicationRange;
            _Show_Routing_Paths.IsChecked = Settings.Default.ShowRoutingPaths;
            _Show_Packets_animations.IsChecked = Settings.Default.ShowAnimation;
        }

        private void TimerCounter_Tick(object sender, EventArgs e)
        {
            //
            if (PublicParamerters.SimulationTime <= stopSimlationWhen + PublicParamerters.MacStartUp)
            {
                Dispatcher.Invoke(() => PublicParamerters.SimulationTime += 1, DispatcherPriority.Send);
                Dispatcher.Invoke(() => Title = "FRCA:" + PublicParamerters.SimulationTime.ToString(), DispatcherPriority.Send);
            }
            else
            {
                TimerCounter.Stop();
                RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0);
                RandomSelectSourceNodesTimer.Stop();
                top_menu.IsEnabled = true;
            }
        }

        private void RandomSelectNodes_Tick(object sender, EventArgs e)
        {
            // start sending after the nodes are intilized all.
            if (PublicParamerters.SimulationTime > PublicParamerters.MacStartUp)
            {
                int index = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(PublicParamerters.NumberofNodes - 2));
                if (index != PublicParamerters.SinkNode.ID)
                {
                    myNetWork[index].GenerateDataPacket();
                }
            }
        }

        private void FillColors()
        {

            // POWER LEVEL:
            lvl_0.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col0));
            lvl_1_9.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col1_9));
            lvl_10_19.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col10_19));
            lvl_20_29.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col20_29));
            lvl_30_39.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col30_39));
            lvl_40_49.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col40_49));
            lvl_50_59.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col50_59));
            lvl_60_69.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col60_69));
            lvl_70_79.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col70_79));
            lvl_80_89.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col80_89));
            lvl_90_100.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col90_100));

            // MAC fuctions:
            lbl_node_state_check.Fill = NodeStateColoring.ActiveColor;
            lbl_node_state_sleep.Fill = NodeStateColoring.SleepColor;
        }


        private void BtnFile(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            switch (Header)
            {
                case "_Multiple Nodes":
                    {
                        UiAddNodes ui = new UiAddNodes();
                        ui.MainWindow = this;
                        ui.Show();
                        break;
                    }

                case "_Export Topology":
                    {
                        UiExportTopology top = new UiExportTopology(myNetWork);
                        top.Show();
                        break;
                    }

                case "_Import Topology":
                    {
                        UiImportTopology top = new UiImportTopology(this);
                        top.Show();
                        break;
                    }
            }

        }

       


        public void DisplaySimulationParameters(int rootNodeId, string deblpaymentMethod)
        {
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            PublicParamerters.SinkNode.Ellipse_battryIndicator.Width = 16;
            PublicParamerters.SinkNode.Ellipse_battryIndicator.Height = 16;
            PublicParamerters.SinkNode.Ellipse_battryIndicator.Fill = Brushes.OrangeRed;
            PublicParamerters.SinkNode.Ellipse_MAC.Fill = Brushes.OrangeRed;

            PublicParamerters.SinkNode.lbl_Sensing_ID.Foreground = Brushes.Blue;
            PublicParamerters.SinkNode.lbl_Sensing_ID.FontWeight = FontWeights.Bold;
            lbl_sink_id.Content = rootNodeId;
            lbl_coverage.Content = deblpaymentMethod;
            lbl_network_size.Content = myNetWork.Count;
            lbl_sensing_range.Content = PublicParamerters.SinkNode.VisualizedRadius;
            lbl_communication_range.Content = (PublicParamerters.SinkNode.VisualizedRadius * 2);
            lbl_Transmitter_Electronics.Content = PublicParamerters.E_elec;
            lbl_fes.Content = PublicParamerters.Efs;
            lbl_Transmit_Amplifier.Content = PublicParamerters.Emp;
            lbl_data_length_control.Content = PublicParamerters.ControlDataLength;
            lbl_data_length_routing.Content = PublicParamerters.RoutingDataLength;
            lbl_density.Content = PublicParamerters.Density;
            Settings.Default.IsIntialized = true;

            TimerCounter.Interval=TimeSpan.FromSeconds(1); // START count the running time.
            TimerCounter.Start(); // START count the running time.
            TimerCounter.Tick += TimerCounter_Tick;

            //:
            prog_total_energy.Maximum = Convert.ToDouble(myNetWork.Count) * PublicParamerters.BatteryIntialEnergy;
            prog_total_energy.Value = 0;



            lbl_x_active_time.Content = Settings.Default.ActivePeriod + ",";
            lbl_x_queue_time.Content = Settings.Default.QueueTime + ".";
            lbl_x_sleep_time.Content = Settings.Default.SleepPeriod + ",";
            lbl_x_start_up_time.Content = Settings.Default.MacStartUp + ",";
            lbl_intial_energy.Content = Settings.Default.BatteryIntialEnergy;


            lbl_update_percentage.Content = Settings.Default.UpdateLossPercentage;
        }

        public void HideSimulationParameters()
        {
            menSimuTim.IsEnabled = true;
            stopSimlationWhen = 1000000;

            rounds = 0;
            lbl_rounds.Content = "0";
            PublicParamerters.SinkNode = null;
            lbl_sink_id.Content = "nil";
            lbl_coverage.Content = "nil";
            lbl_network_size.Content = "unknown";
            lbl_sensing_range.Content = "unknown";
            lbl_communication_range.Content = "unknown";
            lbl_Transmitter_Electronics.Content = "unknown";
            lbl_fes.Content = "unknown";
            lbl_Transmit_Amplifier.Content = "unknown";
            lbl_data_length_control.Content = "unknown";
            lbl_data_length_routing.Content = "unknown";
            lbl_density.Content = "0";
            // lbl_control_range.Content = "0";
            //  lbl_zone_width.Content = "0";
            Settings.Default.IsIntialized = false;

            //
            RandomSelectSourceNodesTimer.Stop();
            TimerCounter.Stop();


            lbl_x_active_time.Content = "0";
            lbl_x_queue_time.Content = "0";
            lbl_x_sleep_time.Content = "0";
            lbl_x_start_up_time.Content = "0";
            lbl_intial_energy.Content = "0";


            lbl_Number_of_Delivered_Packet.Content = "0";
            lbl_Number_of_Droped_Packet.Content = "0";
            lbl_num_of_gen_packets.Content = "0";
            lbl_nymber_inQueu.Content = "0";
            lbl_Redundant_packets.Content = "0";
            lbl_sucess_ratio.Content = "0";
            lbl_Wasted_Energy_percentage.Content = "0";
            lbl_update_percentage.Content = "0";
            lbl_number_of_control_packets.Content = "0";
            PublicParamerters.NumberofControlPackets = 0;
            PublicParamerters.EnergyComsumedForControlPackets = 0;
            PublicParamerters.SimulationTime = 0;
        }



        private void EngageMacAndRadioProcol()
        {
            foreach (Sensor sen in myNetWork)
            {
                sen.Mac = new BoXMAC(sen);
                sen.BatRangesList = PublicParamerters.getRanges();
                sen.Myradar = new Intilization.Radar(sen);
            }
        }


        public void RandomDeplayment(int sinkIndex)
        {
            PublicParamerters.NumberofNodes = myNetWork.Count;
            int rootNodeId = sinkIndex;
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            NeighborsDiscovery overlappingNodesFinder = new NeighborsDiscovery(myNetWork);
            overlappingNodesFinder.GetOverlappingForAllNodes();

            string PowersString = "γL=" + Settings.Default.ExpoLCnt + ",γR=" + Settings.Default.ExpoRCnt + ", γH=" + Settings.Default.ExpoHCnt + ",γD" + Settings.Default.ExpoDCnt;
            PublicParamerters.PowersString = PublicParamerters.NetworkName + ",  " + PowersString;

            isCoverageSelected = true;
            PublicParamerters.Density = Density.GetDensity(myNetWork);
            DisplaySimulationParameters(rootNodeId, "Random");

            EngageMacAndRadioProcol();

            TopologyConstractor.BuildToplogy(Canvas_SensingFeild, myNetWork);

            HopsToSinkComputation.ComputeHopsToSink(PublicParamerters.SinkNode);

            // fill flows:
            double[] cden = { 0.07053, -1.85373, 5.26696, 8.11192 };
            double[] cdir = { 1, 0, 0.3, 0.05 };
            double[] cpd = { 0, 1, 0.1 };
            double[] cdn = { 1.03918, -0.03918, -0.30506 };
            double[] cre = { 1.03283, -1.02041, 0.3184 };

            List<FunCoef> function = new List<FunCoef>();
            List<List<int>> factor = new List<List<int>>();
            List<double> weight = new List<double>() ;
            //den,ds,dn,pd,re,dir
            FunCoef fden = new FunCoef();
            FunCoef fds = new FunCoef();
            FunCoef fdn = new FunCoef();
            FunCoef fpd = new FunCoef();
            FunCoef fre = new FunCoef();
            FunCoef fdir = new FunCoef();
            fden.func = 1;
            fden.coef = cden;
            fds.func = -1;
            fdn.func = 0;
            fdn.coef = cdn;
            fpd.func = 0;
            fpd.coef = cpd;
            fre.func = 0;
            fre.coef = cre;
            fdir.func = 2;
            fdir.coef = cdir;
            function.Add(fden);
            function.Add(fds);
            function.Add(fdn);
            function.Add(fpd);
            function.Add(fre);
            function.Add(fdir);

            List<int> factor1 = new List<int>();
            List<int> factor2 = new List<int>();
            List<int> factor3 = new List<int>();

            factor1.Add(5);
            factor1.Add(2);
            factor2.Add(5);
            factor2.Add(3);
            factor3.Add(5);
            factor3.Add(4);

            factor.Add(factor1);
            factor.Add(factor2);
            factor.Add(factor3);

            weight.Add(0.25);
            weight.Add(0.25);
            weight.Add(0.5);

            foreach (Sensor sen in myNetWork)
            {
                sen.function = function;
                sen.factor = factor;
                sen.weight = weight;
                UplinkRouting.ComputeUplinkFlowEnery(sen);
            }

            MyGraph = Graph.ConvertNodeToVertex(myNetWork);


            //

           

        }

        public void RandomDeplayment_Exp2(int sinkIndex)
        {
            PublicParamerters.NumberofNodes = myNetWork.Count;
            int rootNodeId = sinkIndex;
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            NeighborsDiscovery overlappingNodesFinder = new NeighborsDiscovery(myNetWork);
            overlappingNodesFinder.GetOverlappingForAllNodes();

            string PowersString = "γL=" + Settings.Default.ExpoLCnt + ",γR=" + Settings.Default.ExpoRCnt + ", γH=" + Settings.Default.ExpoHCnt + ",γD" + Settings.Default.ExpoDCnt;
            PublicParamerters.PowersString = PublicParamerters.NetworkName + ",  " + PowersString;
            lbl_PowersString.Content = PublicParamerters.PowersString;

            isCoverageSelected = true;
            PublicParamerters.Density = Density.GetDensity(myNetWork);
            DisplaySimulationParameters(rootNodeId, "Random");

            EngageMacAndRadioProcol();

            TopologyConstractor.BuildToplogy(Canvas_SensingFeild, myNetWork);

            HopsToSinkComputation.ComputeHopsToSink(PublicParamerters.SinkNode);

            // fill flows:
            double[] cds = { 1, 0, 0.6, 0.05 };

            List<FunCoef> function = new List<FunCoef>();
            List<List<int>> factor = new List<List<int>>();
            List<double> weight = new List<double>();
            //den,ds,dn,pd,re,dir,hop
            FunCoef fden = new FunCoef();
            FunCoef fds = new FunCoef();
            FunCoef fdn = new FunCoef();
            FunCoef fpd = new FunCoef();
            FunCoef fre = new FunCoef();
            FunCoef fdir = new FunCoef();
            FunCoef fhop = new FunCoef();
            fden.func = -1;
            fds.func = 3;
            fds.coef = cds;
            fdn.func = -1;
            fpd.func = -1;
            fre.func = -1;
            fdir.func = -1;
            fhop.func = -1;

            function.Add(fden);
            function.Add(fds);
            function.Add(fdn);
            function.Add(fpd);
            function.Add(fre);
            function.Add(fdir);
            function.Add(fhop);

            List<int> factor1 = new List<int>();

            factor1.Add(1);

            factor.Add(factor1);

            weight.Add(1);

            foreach (Sensor sen in myNetWork)
            {
                sen.function = function;
                sen.factor = factor;
                sen.weight = weight;
                UplinkRouting.ComputeUplinkFlowEnery(sen);
            }

            MyGraph = Graph.ConvertNodeToVertex(myNetWork);
            //



        }


        private void Coverage_Click(object sender, RoutedEventArgs e)
        {
            if (!Settings.Default.IsIntialized)
            {
                if (myNetWork.Count > 0)
                {
                    MenuItem item = sender as MenuItem;
                    string Header = item.Name.ToString();
                    switch (Header)
                    {
                        case "btn_Random":
                            {
                                RandomDeplayment(0);
                            }

                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Please imort the nodes from Db.");
                }
            }
            else
            {
                MessageBox.Show("Network is deployed already. please clear first if you want to re-deploy.");
            }
        }

       
        private void Display_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Name.ToString();
            switch (Header)
            {
                case "_show_id":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.lbl_Sensing_ID.Visibility == Visibility.Hidden)
                        {
                            sensro.lbl_Sensing_ID.Visibility = Visibility.Visible;
                            sensro.lbl_hops_to_sink.Visibility = Visibility.Visible;

                            Settings.Default.ShowID = true;
                        }
                        else
                        {
                            sensro.lbl_Sensing_ID.Visibility = Visibility.Hidden;
                            sensro.lbl_hops_to_sink.Visibility = Visibility.Hidden;
                            Settings.Default.ShowID = false;
                        }
                    }
                    break;

                case "_show_sen_range":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.Ellipse_Sensing_range.Visibility == Visibility.Hidden)
                        {
                            sensro.Ellipse_Sensing_range.Visibility = Visibility.Visible;
                            Settings.Default.ShowSensingRange = true;
                        }
                        else
                        {
                            sensro.Ellipse_Sensing_range.Visibility = Visibility.Hidden;
                            Settings.Default.ShowSensingRange = false;
                        }
                    }
                    break;
                case "_show_com_range":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.Ellipse_Communication_range.Visibility == Visibility.Hidden)
                        {
                            sensro.Ellipse_Communication_range.Visibility = Visibility.Visible;
                            Settings.Default.ShowComunicationRange = true;
                        }
                        else
                        {
                            sensro.Ellipse_Communication_range.Visibility = Visibility.Hidden;
                            Settings.Default.ShowComunicationRange = false;
                        }
                    }
                    break;
              
                case "_show_battrey":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.Prog_batteryCapacityNotation.Visibility == Visibility.Hidden)
                        {
                            sensro.Prog_batteryCapacityNotation.Visibility = Visibility.Visible;
                            Settings.Default.ShowBattry = true;
                        }
                        else
                        {
                            sensro.Prog_batteryCapacityNotation.Visibility = Visibility.Hidden;
                            Settings.Default.ShowBattry = false;
                        }
                    }
                    break;
                case "_Show_Routing_Paths":
                    {
                        if (Settings.Default.ShowRoutingPaths == true)
                        {
                            Settings.Default.ShowRoutingPaths = false;
                        }
                        else
                        {
                            Settings.Default.ShowRoutingPaths = true;
                        }
                    }
                    break;

                case "_Show_Packets_animations":
                    {
                        if (Settings.Default.ShowAnimation == true)
                        {
                            Settings.Default.ShowAnimation = false;
                        }
                        else
                        {
                            Settings.Default.ShowAnimation = true;
                        }
                    }
                    break;
            }
        }

        private void btn_other_Menu(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            switch (Header)
            {

                //
                case "_Show Dead Node":
                    {
                        if (myNetWork.Count > 0)
                        {
                            if (PublicParamerters.DeadNodeList.Count > 0)
                            {
                                UiNetworkLifetimeReport xx = new UiNetworkLifetimeReport();
                                xx.Title = "LORA Lifetime report";
                                xx.dg_grid.ItemsSource = PublicParamerters.DeadNodeList;
                                xx.Show();
                            }
                            else
                                MessageBox.Show("No Dead node.");
                        }
                        else
                        {
                            MessageBox.Show("No Network is selected.");
                        }
                    }
                    break;

                case "_Show Resultes":
                    {
                        if (myNetWork.Count > 0)
                        {
                            ExpReport xx = new ExpReport(this);
                            xx.Show();
                        }
                    }
                    break;

                case "Exp2 Results":
                    {
                        if (myNetWork.Count > 0)
                        {
                            Exp2Report yy = new Exp2Report(this);
                            yy.Show();
                        }
                    }
                    break;

                case "_Draw Tree":

                    break;
                case "_Print Info":
                    UIshowSensorsLocations uIlocations = new UIshowSensorsLocations(myNetWork);
                    uIlocations.Show();
                    break;
                case "_Entir Network Routing Log":
                    UiRoutingDetailsLong routingLogs = new ui.UiRoutingDetailsLong(myNetWork);
                    routingLogs.Show();
                    break;
                case "_Log For Each Sensor":

                    break;
                //_Relatives:
                case "_Node Forwarding Probability Distributions":
                    {
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Forwarding Probability Distributions For Each Node";
                        foreach (Sensor source in myNetWork)
                        {
                            
                        }
                        windsow.Show();
                        break;
                    }
                //
                case "_Expermental Results":
                    UIExpermentResults xxxiu = new UIExpermentResults();
                    xxxiu.Show();
                    break;
                case "_Probability Matrix":
                    {
                       
                    }
                    break;
                //
                case "_Packets Paths":
                    UiRecievedPackertsBySink packsInsinkList = new UiRecievedPackertsBySink();
                    packsInsinkList.Show();

                    break;
                //
                case "_Random Numbers":

                    List<KeyValuePair<int, double>> rands = new List<KeyValuePair<int, double>>();
                    int index = 0;
                    foreach (Sensor sen in myNetWork)
                    {
                        foreach (RoutingLog log in sen.Logs)
                        {
                            if (log.IsSend)
                            {
                                index++;
                                rands.Add(new KeyValuePair<int, double>(index, log.ForwardingRandomNumber));
                            }
                        }
                    }

                    UiRandomNumberGeneration wndsow = new ui.UiRandomNumberGeneration();
                    wndsow.chart_x.DataContext = rands;
                    wndsow.Show();

                    break;
                case "_Nodes Load":
                    {
                        /*
                        SegmaManager sgManager = new SegmaManager();
                        Sensor sink = PublicParamerters.SinkNode;
                        List<string> Paths = new List<string>();
                        if (sink != null)
                        {
                            foreach (Packet pck in sink.PacketsList)
                            {
                                Paths.Add(pck.Path);
                            }

                        }*/
                        /*
                        sgManager.Filter(Paths);
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Nodes Load";
                        SegmaCollection collectionx = sgManager.GetCollection;
                        foreach (SegmaSource source in collectionx.GetSourcesList)
                        {
                            source.NumberofPacketsGeneratedByMe = myNetWork[source.SourceID].NumberofPacketsGeneratedByMe;
                            ListControl List = new conts.ListControl();
                            List.lbl_title.Content = "Source:" + source.SourceID + " Pks:" + source.NumberofPacketsGeneratedByMe + " Relays:" + source.RelaysCount + " Hops:" + source.HopsSum + " Mean:" + source.Mean + " Variance:" + source.Veriance + " E:" + source.PathsSpread;
                            List.dg_date.ItemsSource = source.GetRelayNodes;
                            windsow.stack_items.Children.Add(List);
                        }
                        windsow.Show();
                      */
                    }
                    break;
                //_Distintc Paths
                case "_Distintc Paths":
                    {
                        
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Distinct Paths for each Source";
                        DisPathConter dip = new DisPathConter();
                        List<ClassfyPathsPerSource> classfy = dip.ClassyfyDistinctPathsPerSources();
                        foreach (ClassfyPathsPerSource source in classfy)
                        {
                            ListControl List = new conts.ListControl();
                            List.lbl_title.Content = "Source:" + source.SourceID;
                            List.dg_date.ItemsSource = source.DistinctPathsForThisSource;
                            windsow.stack_items.Children.Add(List);
                        }
                        windsow.Show();
                       
                    }
                    break;
            }
        }

        int rounds = 0;
        int alreadPassedRound = 0;

        private void Btn_rounds_uplinks_mousedown(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.IsIntialized)
            {
                MenuItem slected = sender as MenuItem;
                int rnd = Convert.ToInt16(slected.Header.ToString().Split('_')[1]);
              

                 rounds = rnd;
                 alreadPassedRound = 0;
              
                RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(5);
                RandomSelectSourceNodesTimer.Start();
                RandomSelectSourceNodesTimer.Tick += RoundsPacketsGeneator; 

            }
            else
            {
                MessageBox.Show("Please selete the coverage.Coverage->Random");
            }
        }

        private void RoundsPacketsGeneator(object sender, EventArgs e)
        {
            alreadPassedRound++;
            if (alreadPassedRound <= rounds)
            {
                lbl_rounds.Content = alreadPassedRound;
                foreach (Sensor sen in myNetWork)
                {
                    if (sen.ID != PublicParamerters.SinkNode.ID)
                    {
                        sen.GenerateDataPacket();
                    }
                }
            }
            else
            {
                RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0);
                RandomSelectSourceNodesTimer.Stop();
            }
        }



        private void Btn_rounds_downlinks_mousedown(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.IsIntialized)
            {
                // not random:
                MenuItem slected = sender as MenuItem;
                int pktsNumber = Convert.ToInt16(slected.Header.ToString().Split('_')[1]);
                rounds += pktsNumber;
                lbl_rounds.Content = rounds;

                for (int i = 1; i <= pktsNumber; i++)
                {
                    foreach (Sensor sen in myNetWork)
                    {
                        PublicParamerters.SinkNode.GenerateControlPacket(sen);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please selete the coverage.Coverage->Random");
            }
        }

        private void BuildTheTree(object sender, RoutedEventArgs e)
        {

        }

        private void tconrol_charts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        public void ClearExperment()
        {
            try
            {
                PublicParamerters.NumberofDropedPacket = 0;
                PublicParamerters.NumberofDeliveredPacket = 0;
                PublicParamerters.Rounds = 0;
                PublicParamerters.DeadNodeList.Clear();
                PublicParamerters.NumberofGeneratedPackets = 0;
                PublicParamerters.TotalWaitingTime = 0;
                PublicParamerters.TotalReduntantTransmission = 0;
                PublicParamerters.IsNetworkDied = false;
                PublicParamerters.Density = 0;
                PublicParamerters.TotalDelayMs =0;
                PublicParamerters.TotalEnergyConsumptionJoule = 0;
                PublicParamerters.TotalWastedEnergyJoule = 0;

                PublicParamerters.TotalWastedEnergyJoule = 0;
                PublicParamerters.TotalDelayMs = 0;
                PublicParamerters.NetworkName = "";
                PublicParamerters.FinishedRoutedPackets.Clear();
                PublicParamerters.NumberofNodes = 0;
                PublicParamerters.NOS = 0;
                PublicParamerters.Rounds = 0;
                PublicParamerters.SinkNode = null;

                PublicParamerters.IsNetworkDied = false;
                PublicParamerters.Density = 0;
                PublicParamerters.NetworkName = "";
                PublicParamerters.DeadNodeList.Clear();
                PublicParamerters.NOP = 0;
                PublicParamerters.NOS = 0;
                PublicParamerters.Rounds = 0;
                PublicParamerters.SinkNode = null;

                top_menu.IsEnabled = true;
                
                Canvas_SensingFeild.Children.Clear();
                if (myNetWork != null)
                    myNetWork.Clear();

                isCoverageSelected = false;


                HideSimulationParameters();
                col_Path_Efficiency.DataContext = null;
                col_Delay.DataContext = null;
                col_EnergyConsumptionForEachNode.DataContext = null;

              

                cols_hops_ditrubtions.DataContext = null;
                lbl_PowersString.Content = "";
                cols_hops_ditrubtions.DataContext = null;
                cols_energy_distribution.DataContext = null;
                cols_delay_distribution.DataContext = null;

               

                

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }


        private void ben_clear_click(object sender, RoutedEventArgs e)
        {
            TimerCounter.Stop();
            RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0);
            RandomSelectSourceNodesTimer.Stop();

            Settings.Default.IsIntialized = false;

            ClearExperment();

        }



        public object NetworkLifeTime { get; private set; }

        private void tab_network_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           

        }

        private void lbl_show_grid_line_x_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (col_network_X_Gird.ShowGridLines == false) col_network_X_Gird.ShowGridLines = true;
            else col_network_X_Gird.ShowGridLines = false;
        }

        private void lbl_show_grid_line_y_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (col_network_Y_Gird.ShowGridLines == false) col_network_Y_Gird.ShowGridLines = true;
            else col_network_Y_Gird.ShowGridLines = false;
        }



        private void setDisributaions_Click(object sender, RoutedEventArgs e)
        {
            if (myNetWork.Count == 0)
            {
                UIPowers cc = new ui.UIPowers(this);
                cc.Show();
            }
            else
            {
                MessageBox.Show("These Parameters can not be set after deploying the nodes. please clear the feild and re-set.");
            }
        }


        private void _set_paramertes_Click(object sender, RoutedEventArgs e)
        {
            /*
            ben_clear_click(sender, e);

            UiMultipleExperments setpa = new UiMultipleExperments(this);
            this.WindowState = WindowState.Minimized;
            setpa.Show();*/

        }



        private void btn_chek_lifetime_Click(object sender, RoutedEventArgs e)
        {
            if (isCoverageSelected)
            {
                this.WindowState = WindowState.Minimized;
                for (int i = 0; ; i++)
                {
                    rounds++;
                    lbl_rounds.Content = rounds;
                    if (!PublicParamerters.IsNetworkDied)
                    {
                        foreach (Sensor sen in myNetWork)
                        {
                            if (sen.ID != PublicParamerters.SinkNode.ID)
                            {
                                sen.GenerateDataPacket();
                              //  sen.GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt,
                              //  Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please selete the coverage. Coverage->Random");
            }
        }

        private void btn_lifetime_s1_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.IsIntialized == false)
            {
                RandomDeplayment(0);
                UiComputeLifeTime lifewin = new UiComputeLifeTime(this);
                lifewin.Show();
                lifewin.Owner = this;
                top_menu.IsEnabled = false;
                Settings.Default.IsIntialized = true;
            }
            else
            {
                MessageBox.Show("File->clear and try agian.");
            }

           
        }


        

        /// <summary>
        /// _Randomly Select Nodes With Distance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnCon_RandomlySelectNodesWithDistance_Click(object sender, RoutedEventArgs e)
        {
            if (isCoverageSelected)
            {
                if (PublicParamerters.FinishedRoutedPackets.Count == 0)
                {
                    ui.UiSelectNodesWidthDistance win = new UiSelectNodesWidthDistance(this);
                    win.Show();
                }
                else
                {
                    MessageBox.Show("Please clear first: File->Clear!");
                }
            }
            else
            {
                MessageBox.Show("Please selected the Coverage.Coverage->Random");
            }

        }

        public void SendPackectPerSecond(double s)
        {
            if (s == 0)
            {
                Settings.Default.AnimationSpeed = s;
                RandomSelectSourceNodesTimer.Stop();
                PacketRate = "1 packet per " + s + " s";
            }
            else
            {
                if (s >= 1) Settings.Default.AnimationSpeed = 0.5; else Settings.Default.AnimationSpeed = s;
                RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(s);
                RandomSelectSourceNodesTimer.Start();
                RandomSelectSourceNodesTimer.Tick += RandomSelectNodes_Tick;
                PacketRate = "1 packet per " + s + " s";
            }
        }

        private void btn_select_sources_Click(object sender, RoutedEventArgs e)
        {
            foreach (Sensor node in myNetWork)
                node.Rule_indicator.Visibility = Visibility.Hidden;


            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            if (Settings.Default.IsIntialized)
            {
                switch (Header)
                {
                    case "1pck/1s":
                        SendPackectPerSecond(1);
                        break;
                    case "1pck/2s":
                        SendPackectPerSecond(2);
                        break;
                    case "1pck/4s":
                        SendPackectPerSecond(4);
                        break;
                    case "1pck/6s":
                        SendPackectPerSecond(6);
                        break;
                    case "1pck/8s":
                        SendPackectPerSecond(8);
                        break;
                    case "1pck/10s":
                        SendPackectPerSecond(10);
                        break;
                    case "1pck/0s(Stop)":
                        SendPackectPerSecond(0);
                        break;
                    case "1pck/0.1s":
                        SendPackectPerSecond(0.1);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Please select Coverage->Random. then continue.");
            }
        }


        #region Upink Generator //////////////////////////////////////////////////////////////////////
        int UplinkTobeGeneratedPackets = 0;
        int UplinkalreadyGeneratedPackets = 0;
        int UplinkTobeGeneratedPackets_1st = 0;
        int UplinkTobeGeneratedPackets_2nd = 0;
        int ChangeTime = 0;

        
        


        public void GenerateUplinkPacketsRandomly(int numofPackets)
        {
            UplinkTobeGeneratedPackets = 0;
            UplinkalreadyGeneratedPackets = 0;

            UplinkTobeGeneratedPackets = numofPackets;
            RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0.1);
            RandomSelectSourceNodesTimer.Start();
            RandomSelectSourceNodesTimer.Tick += UplinkPacketsGenerate_Tirk;
        }
         
        private void UplinkPacketsGenerate_Tirk(object sender, EventArgs e)
        {
            if (PublicParamerters.SimulationTime > PublicParamerters.MacStartUp)
            {
                UplinkalreadyGeneratedPackets++;
                if (UplinkalreadyGeneratedPackets <= UplinkTobeGeneratedPackets)
                {
                    int index = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(PublicParamerters.NumberofNodes - 2));
                    myNetWork[index].GenerateDataPacket();
                }
                else
                {
                    RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0);
                    RandomSelectSourceNodesTimer.Stop();
                    UplinkalreadyGeneratedPackets = 0;
                    UplinkTobeGeneratedPackets = 0;
                }
            }
            
        }

        public void GenerateUplinkPacketsRandomly_Exp2(int Num_Exp1, int Num_Exp2)
        {
            UplinkalreadyGeneratedPackets = 0;

            UplinkTobeGeneratedPackets_1st = Num_Exp1;
            UplinkTobeGeneratedPackets_2nd = Num_Exp2;
            RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0.1);
            RandomSelectSourceNodesTimer.Start();
            RandomSelectSourceNodesTimer.Tick += UplinkPacketsGenerate_Tirk_Exp2;
        }

        private int flag = 0;

        private void UplinkPacketsGenerate_Tirk_Exp2(object sender, EventArgs e)
        {
            if (PublicParamerters.SimulationTime > PublicParamerters.MacStartUp)
            {
                UplinkalreadyGeneratedPackets++;
                if (UplinkalreadyGeneratedPackets <= UplinkTobeGeneratedPackets_1st)
                {
                    int index = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(PublicParamerters.NumberofNodes - 2));
                    myNetWork[index].GenerateDataPacket();
                }
                else if (UplinkalreadyGeneratedPackets <= UplinkTobeGeneratedPackets_1st + UplinkTobeGeneratedPackets_2nd)
                {
                    if(UplinkalreadyGeneratedPackets == UplinkTobeGeneratedPackets_1st + 1)
                    {
                        if(PublicParamerters.InQueuePackets != 0)
                        {
                            UplinkalreadyGeneratedPackets--;
                        }
                        else
                        {
                            if(ChangeTime == 0)
                            {
                                DownLinkRouting.SendRule();
                                UplinkalreadyGeneratedPackets--;
                            }
                            else if (ChangeTime <= 400)
                            {
                                UplinkalreadyGeneratedPackets--;
                            }
                            else
                            {
                                foreach (Sensor node in myNetWork)
                                    node.Rule_indicator.Visibility = Visibility.Hidden;

                                PublicParamerters.TotalWaitingTime = 0;
                                PublicParamerters.TotalReduntantTransmission = 0;
                                PublicParamerters.TotalEnergyConsumptionJoule = 0;

                                int index = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(PublicParamerters.NumberofNodes - 2));
                                myNetWork[index].GenerateDataPacket();
                            }
                            ChangeTime++;
                        }
                    }
                    else
                    {
                        int index = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(PublicParamerters.NumberofNodes - 2));
                        myNetWork[index].GenerateDataPacket();
                    }
                }
                else
                {
                    RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0);
                    RandomSelectSourceNodesTimer.Stop();
                    UplinkalreadyGeneratedPackets = 0;
                    UplinkTobeGeneratedPackets = 0;
                }

                //Statistic 
                int interval = 500;
                if(UplinkalreadyGeneratedPackets == UplinkTobeGeneratedPackets_1st)
                {
                    if(flag == 0)
                    {
                        double averageWaitingTime = Convert.ToDouble(PublicParamerters.TotalWaitingTime) / interval;
                        double avergaeRedundan = Convert.ToDouble(PublicParamerters.TotalReduntantTransmission) / interval;
                        double averageEnergyConsumption = PublicParamerters.TotalEnergyConsumptionJoule;

                        Exp2ReprotForm res = new Exp2ReprotForm();
                        res.ec = averageEnergyConsumption;
                        res.awt = averageWaitingTime;
                        res.arxt = avergaeRedundan;
                        PublicParamerters.Exp2Res.Add(res);

                        PublicParamerters.TotalWaitingTime = 0;
                        PublicParamerters.TotalReduntantTransmission = 0;
                        PublicParamerters.TotalEnergyConsumptionJoule = 0;
                        flag = 1;
                    }
                }
                else if(UplinkalreadyGeneratedPackets >= interval  && UplinkalreadyGeneratedPackets % interval == 0 && UplinkTobeGeneratedPackets_2nd < 100000000)
                {
                    double averageWaitingTime = Convert.ToDouble(PublicParamerters.TotalWaitingTime) / interval;
                    double avergaeRedundan = Convert.ToDouble(PublicParamerters.TotalReduntantTransmission) / interval;
                    double averageEnergyConsumption = PublicParamerters.TotalEnergyConsumptionJoule;

                    Exp2ReprotForm res = new Exp2ReprotForm();
                    res.ec = averageEnergyConsumption;
                    res.awt = averageWaitingTime;
                    res.arxt = avergaeRedundan;
                    PublicParamerters.Exp2Res.Add(res);

                    PublicParamerters.TotalWaitingTime = 0;
                    PublicParamerters.TotalReduntantTransmission = 0;
                    PublicParamerters.TotalEnergyConsumptionJoule = 0;
                }






            }

        }


        private void btn_uplLINK_send_numbr_of_packets(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            if (Settings.Default.IsIntialized)
            {
                int Header_int = Convert.ToInt16(Header);
                GenerateUplinkPacketsRandomly(Header_int);
            }
            else
            {
                MessageBox.Show("Please select Coverage->Random. then continue.");
            }
        }

        #endregion ///////////////////////////////////////////////////////////////


      


        int DownlinkTobeGenerated = 0;
        int DownlinkAlreadyGenerated = 0;

        public void GenerateDownLinkPacketRandomly(int numofpackets)
        {
            DownlinkTobeGenerated = 0;
            DownlinkAlreadyGenerated = 0;

            DownlinkTobeGenerated = numofpackets;
            RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0.01);
            RandomSelectSourceNodesTimer.Start();
            RandomSelectSourceNodesTimer.Tick += DownLINKRandomSentAnumberofPackets;
        }

        private void btn_DOWNN_send_numbr_of_packets(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            if (Settings.Default.IsIntialized)
            {
                int Header_int = Convert.ToInt16(Header);
                GenerateDownLinkPacketRandomly(Header_int);

            }
            else
            {
                MessageBox.Show("Please select Coverage->Random. then continue.");
            }
        }

        private void DownLINKRandomSentAnumberofPackets(object sender, EventArgs e)
        {
            DownlinkAlreadyGenerated++;
            if (DownlinkAlreadyGenerated <= DownlinkTobeGenerated)
            {
                int index = Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(PublicParamerters.NumberofNodes - 2));
                Sensor EndNode = myNetWork[index];
                PublicParamerters.SinkNode.GenerateControlPacket(EndNode);
            }
            else
            {
                RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0);
                RandomSelectSourceNodesTimer.Stop();
                DownlinkAlreadyGenerated = 0;
                DownlinkTobeGenerated = 0;
            }
        }


        private void btn_simTime_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            if (Settings.Default.IsIntialized)
            {
                stopSimlationWhen = Convert.ToInt32(Header.ToString());
                menSimuTim.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Please select Coverage->Random. then continue.");
            }
        }

        private void Btn_comuputeEnergyCon_withinTime_Click(object sender, RoutedEventArgs e)
        {

            if (Settings.Default.IsIntialized)
            {
                MessageBox.Show("File->clear and try agian.");
            }
            else
            {
                PacketRate = "";
                stopSimlationWhen = 0;
                UISetParEnerConsum con = new UISetParEnerConsum(this);
                con.Owner = this;
                con.Show();
                //top_menu.IsEnabled = false;



            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DownLinkRouting.SendRule();
        }
    }
}



           
