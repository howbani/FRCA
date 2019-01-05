using FRCA.Dataplane;
using FRCA.Dataplane.NOS;
using FRCA.Dataplane.PacketRouter;
using FRCA.Intilization;
using FRCA.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRCA.ControlPlane.NOS.FlowEngin
{
    public class MiniFlowTableSorterDownLinkPriority : IComparer<MiniFlowTableEntry>
    {

        public int Compare(MiniFlowTableEntry y, MiniFlowTableEntry x)
        {
            return x.DownLinkPriority.CompareTo(y.DownLinkPriority);
        } 
    } 
    public class DownlinkFlowEnery
    {
        public Sensor Current { get; set; }
        public Sensor Next { get; set; }
        public Sensor Target { get; set; }
        // Elementry values:
        public double D { get; set; } // direction value tworads the end node
        public double DN { get; set; } // R NORMALIZEE value of To. 
        public double DP { get; set; } // defual.

        public double L { get; set; } // remian energy
        public double LN { get; set; } // L normalized
        public double LP { get; set; } // L value of To.

        public double R { get; set; } // riss
        public double RN { get; set; } // R NORMALIZEE value of To. 
        public double RP { get; set; } // R NORMALIZEE value of To. 
        //
        public double Pr
        {
            get; set;
        }

        // return:
        public double Mul
        {
            get
            {
                return LP * DP * RP;
            }
        }

        public int IindexInMiniFlow { get; set; }
        public MiniFlowTableEntry MiniFlowTableEntry { get; set; }
    }

    public class DownLinkRouting
    {
       
        /// <summary>
        /// This will be change per sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="endNode"></param>
        public static void  GetD_Distribution(Sensor sender, Sensor endNode)
        {
            double n = Convert.ToDouble(sender.NeighborsTable.Count) + 1;

            double Dcontrol = Settings.Default.ExpoDCnt * Math.Sqrt(n);

            // normalized values.
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.NeighborEntry.D = Operations.DistanceBetweenTwoPoints(endNode.CenterLocation, MiniEntry.NeighborEntry.CenterLocation);
                MiniEntry.NeighborEntry.DN = (MiniEntry.NeighborEntry.D / (Operations.DistanceBetweenTwoPoints(endNode.CenterLocation, sender.CenterLocation) + sender.ComunicationRangeRadius));
            }

            // pro sum
            double DpSum = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                DpSum += (Math.Pow((1 - Math.Sqrt(MiniEntry.NeighborEntry.DN)), 1 + Dcontrol));
            }

            double sumAll = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.NeighborEntry.DP = (Math.Pow((1 - Math.Sqrt(MiniEntry.NeighborEntry.DN)), 1 + Dcontrol)) / DpSum;

                //: 
                MiniEntry.DownLinkPriority = (MiniEntry.NeighborEntry.DP + MiniEntry.NeighborEntry.LP + MiniEntry.NeighborEntry.RP) / 3;
                sumAll += MiniEntry.DownLinkPriority;
            }

            // normlizd
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.DownLinkPriority = (MiniEntry.DownLinkPriority / sumAll);
            }

            // sort:

            MiniFlowTableSorterDownLinkPriority xxxx = new MiniFlowTableSorterDownLinkPriority();
            sender.MiniFlowTable.Sort(xxxx);

            double average = 1 / Convert.ToDouble(sender.MiniFlowTable.Count);
            int Ftheashoeld = Convert.ToInt16(Math.Ceiling(Math.Sqrt(Math.Sqrt(n)))); // theshold.
            int forwardersCount = 0;

            // action:
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                if (MiniEntry.DownLinkPriority >= average && forwardersCount <= Ftheashoeld)
                {
                    MiniEntry.DownLinkAction = FlowAction.Forward;
                    forwardersCount++;
                }
                else MiniEntry.DownLinkAction = FlowAction.Drop;
            }

        }

        public static void SendRule()
        {
            // fill flows:
            double[] cden = { 0.07053, -1.85373, 5.26696, 8.11192 };
            double[] cdir = { 1, 0, 0.3, 0.05 };
            double[] cpd = { 0, 1, 0.1 };
            double[] cdn = { 1.03918, -0.03918, -0.30506 };
            double[] cre = { 1.03283, -1.02041, 0.3184 };

            List<FunCoef> function = new List<FunCoef>();
            List<List<int>> factor = new List<List<int>>();
            List<double> weight = new List<double>();
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

            PublicParamerters.ControlPacketVersion++;
            Packet pack = new Packet();
            pack.PacketType = PacketType.Control;
            pack.Source = PublicParamerters.SinkNode;
            pack.PID = PublicParamerters.ControlPacketVersion;
            pack.function = function;
            pack.factorItem = factor;
            pack.weight = weight;
            PublicParamerters.SinkNode.SendPacekt(pack);
        }


    }
}
