using FRCA.Dataplane;
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
    public class VaulePair
    {
        public string EC { get; set; }
        public string AWT { get; set; }
        public string AxRP { get; set; }
    }

    /// <summary>
    /// Exp2Report.xaml 的交互逻辑
    /// </summary>
    public partial class Exp2Report : Window
    {
        public Exp2Report(MainWindow _mianWind)
        {
            InitializeComponent();

            List<VaulePair> List = new List<VaulePair>();
            foreach (Exp2ReprotForm res in PublicParamerters.Exp2Res)
            {
                List.Add(new VaulePair() { EC = res.ec.ToString(), AWT = res.awt.ToString(),AxRP = res.arxt.ToString()});
            }
            dtable.ItemsSource = List;




        }
    }
}
