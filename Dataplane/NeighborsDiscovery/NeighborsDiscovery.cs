using System.Collections.Generic;
using FRCA.Dataplane;
using System.Windows;
using FRCA.Intilization;
using FRCA.Dataplane.PacketRouter;
using System;

namespace FRCA.DataPlane.NeighborsDiscovery
{
    public class NeighborsDiscovery
    {
       private List<Sensor> Network;
       public NeighborsDiscovery(List<Sensor> _NetworkNodes)
       {
           Network=_NetworkNodes; 
       }
        /// <summary>
        /// only those nodes which follow within the range of i.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private void DiscoverMyNeighbors(Sensor i)
        {
            i.NeighborsTable = new List<NeighborsTableEntry>();
            // get the overlapping nodes:
            if (Network != null)
            {
                if (Network.Count > 0)
                {
                    foreach (Sensor node in Network)
                    {
                        if (i.ID != node.ID)
                        {
                            bool isOverlapped = Operations.isInMyComunicationRange(i, node);
                            if (isOverlapped)
                            {
                                NeighborsTableEntry en = new NeighborsTableEntry();
                                en.NeiNode = node;
                                en.R = Operations.DistanceBetweenTwoSensors(i, node);


                                en.NeiNode = node;

                                //public double[] value = new double[10]; //den,ds,dn,pd,re,dir

                                en.value[2] = Operations.DistanceBetweenTwoSensors(i, node) / node.ComunicationRangeRadius;
                                en.value[4] = node.ResidualEnergy / node.BatteryIntialEnergy;
                                en.value[1] = (node.ds - i.ds + i.ComunicationRangeRadius)/ (2 * i.ComunicationRangeRadius);
                                en.value[5] = GetDirection(i, node) / Math.PI;
                                en.value[6] = node.HopsToSink;

                                i.NeighborsTable.Add(en);
                            }
                        }
                    }
                }
            }
        }

        private double GetDirection(Sensor sender, Sensor nei)
        {
            if (nei == PublicParamerters.SinkNode || sender == PublicParamerters.SinkNode)
                return 0;
            double x1 = nei.CenterLocation.X - sender.CenterLocation.X;
            double y1 = nei.CenterLocation.Y - sender.CenterLocation.Y;
            double x2 = PublicParamerters.SinkNode.CenterLocation.X - sender.CenterLocation.X;
            double y2 = PublicParamerters.SinkNode.CenterLocation.Y - sender.CenterLocation.Y;
            double l1 = Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(y1, 2));
            double l2 = Math.Sqrt(Math.Pow(x2, 2) + Math.Pow(y2, 2));
            return Math.Acos((x1 * x2 + y1 * y2) / l1 / l2);
        }


       /// <summary>
       /// for all nodes inside the newtwork find the overllapping nodes.
       /// </summary>
       public void GetOverlappingForAllNodes()
       {
            double maxDis = -1;
           foreach(Sensor node in Network)
           {
               DiscoverMyNeighbors(node);
                if (node.ds > maxDis)
                    maxDis = node.ds;
           }
            int[] count = new int[7];
           foreach(Sensor node in Network)
            {
                foreach(NeighborsTableEntry neiEntry in node.NeighborsTable)
                {
                    if (neiEntry.value[5] <= 0.5)
                        node.den++;
                }
                //node.den = 1 - 1 / node.den;
            }
            PublicParamerters.NetworkRadius = maxDis;
       }








    }
}
