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
using System.Windows;

namespace FRCA.ControlPlane.NOS.FlowEngin
{
    
    public class MiniFlowTableSorterUpLinkPriority : IComparer<MiniFlowTableEntry>
    {

        public int Compare(MiniFlowTableEntry y, MiniFlowTableEntry x)
        {
            return x.UpLinkPriority.CompareTo(y.UpLinkPriority);
        }
    }

    public class UplinkRouting
    {
        public static void UpdateUplinkFlowEnery(Sensor sender)
        {            
            foreach(NeighborsTableEntry neiEntry in sender.NeighborsTable)
            {
                foreach(NeighborsTableEntry sub in neiEntry.NeiNode.NeighborsTable)
                {
                    if(sub.NeiNode == sender)
                    {
                        sub.value[4] = sender.ResidualEnergy / sender.BatteryIntialEnergy;
                    }
                }
            }

            sender.MiniFlowTable.Clear();
            ComputeUplinkFlowEnery(sender);
        }

        public static void ComputeUplinkFlowEnery(Sensor sender)
        {
            double[] sum = new double[sender.factor.Count];
            foreach (NeighborsTableEntry neiEntry in sender.NeighborsTable)
            {
                MiniFlowTableEntry MiniEntry = new MiniFlowTableEntry();
                MiniEntry.NeighborEntry = neiEntry;
                sender.MiniFlowTable.Add(MiniEntry);
                neiEntry.value[0] = sender.den;
                GetSubDistribution(sender, neiEntry);
                int i = 0;
                foreach (List<int> factorItem in sender.factor)
                {
                    neiEntry.facRes[i] = 1;
                    foreach (int resNum in factorItem)
                    {
                        neiEntry.facRes[i] *= neiEntry.result[resNum];
                    }
                    sum[i] += neiEntry.facRes[i];
                    i++;
                }
            }

            double PriSum = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                for(int i = 0; i < sum.Count(); i++)
                {
                    MiniEntry.NeighborEntry.facRes[i] /= sum[i];
                    MiniEntry.UpLinkPriority += sender.weight[i] * MiniEntry.NeighborEntry.facRes[i];
                }
                PriSum += MiniEntry.UpLinkPriority;
            }

            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.UpLinkPriority /= PriSum;
            }

            sender.MiniFlowTable.Sort(new MiniFlowTableSorterUpLinkPriority());

            double n = Convert.ToDouble(sender.NeighborsTable.Count) + 1;
            double average = 1 / Convert.ToDouble(sender.MiniFlowTable.Count);
            int Ftheashoeld = Convert.ToInt16(Math.Ceiling(Math.Sqrt(Math.Sqrt(n)))); // theshold.
            int forwardersCount = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                if (MiniEntry.UpLinkPriority >= average && forwardersCount <= Ftheashoeld)
                {
                    MiniEntry.UpLinkAction = FlowAction.Forward;
                    forwardersCount++;
                }
                else MiniEntry.UpLinkAction = FlowAction.Drop;
            }
        }

        private static void GetSubDistribution(Sensor sender, NeighborsTableEntry neiEntry)
        {
            int i = 0;
            foreach(FunCoef funItem in sender.function)
            {
                if(i == 5 && funItem.coef != null)
                {
                    //if the value is direction, then the coef is needed to modify by density
                    funItem.coef[2] = neiEntry.result[0];
                    neiEntry.result[i] = SigmoidFunction(neiEntry.value[i], funItem.coef);
                }
                else
                {
                    if (funItem.func == 0)
                        neiEntry.result[i] = ExpFunction(neiEntry.value[i], funItem.coef);
                    else if (funItem.func == 1)
                        neiEntry.result[i] = GaussFunction(neiEntry.value[i], funItem.coef);
                    else if (funItem.func == 2)
                        neiEntry.result[i] = SigmoidFunction(neiEntry.value[i], funItem.coef);
                    else if (funItem.func == 3)
                    {
                        if (neiEntry.value[i] < 0.5)
                            neiEntry.result[i] = 1;
                        else
                            neiEntry.result[i] = 0;
                        //neiEntry.result[i] = 1.0/ neiEntry.value[i];
                        
                    }
                        
                    else
                        neiEntry.result[i] = 0;
                }
                i++;

            }
        }

        
        private static double ExpFunction(double x, double[] coef)
        {
            //coef[] y0,A1,t1
            return coef[0] + coef[1] * Math.Exp(-x / coef[2]);
        }

        private static double GaussFunction(double x, double[] coef)
        {
            //y0,xc,w,A
            return coef[0] + coef[3] / (coef[2] * Math.Sqrt(Math.PI / 2)) * Math.Exp(-2 * Math.Pow(x - coef[1], 2) / Math.Pow(coef[2], 2));
        }

        private static double SigmoidFunction(double x, double[] coef)
        {
            //A1,A2,x0,dx
            return (coef[0] - coef[1]) / (1 + Math.Exp((x - coef[2]) / coef[3])) + coef[1];
        }



    }
}
