using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRCA.Dataplane.PacketRouter
{
    /// <summary>
    /// TABLE 2: NEIGHBORING NODES INFORMATION TABLE (NEIGHBORS-TABLE)
    /// </summary>
    public class NeighborsTableEntry
    {
        public int ID { get { return NeiNode.ID; } } // id of candidate.
                                                     // Elementry values:
        //规格化后的值
        //public double ds { get; set; } // Distance to sink
        //public double dn { get; set; } // Distance to sender
        //public double pd { get; set; } // Perpendicular distance
        //public double re { get; set; } // Residual energy
        //public double dir { get; set; }// Direction
        //public double den { get; set; }// Density

        public double[] value = new double[10]; //den,ds,dn,pd,re,dir
        public double[] result = new double[10];
        public double[] facRes = new double[10];
        

        public double fdir { get; set; }
        public double fds { get; set; }
        public double fdn { get; set; }
        public double fpd { get; set; }
        public double fre { get; set; }
        public double fws { get; set; }

        public double HP { get; set; } // hops to sink.
        public double EP { get; set; } // ECLIDIAN DISTANCE
        public double RP { get; set; } // RSSI.
        public double LP { get; set; } // battry level.

        // less hops and closer to the sink.
        public double HEP
        {
            get { return (HP + EP) / 2; }
        }
        // closer to the sender and closer to the sink.
        public double REP
        {
            get { return (RP + EP) / 2; }
        }
        // less hops and closer the sender.
        public double HRP
        {
            get { return (HP + RP) / 2; }
        }

        // less hops, closer to sender and and in the direction toward the sink.
        public double HREP
        {
            get { return (HP + RP + EP) / 3; }
        }

        // Hops:
        public int H { get { return NeiNode.HopsToSink; } }
        public double HN { get; set; } // H normalized
        

        // rssi:
        public double R { get; set; }// 
        public double RN { get; set; } // R NORMALIZEE value of To. 
       


        public double E { get; set; } //  IDRECTION TO THE SINK
        public double EN { get; set; } // // NORMLIZED
      

        public double L { get { return NeiNode.ResidualEnergyPercentage; } } //
        public double LN { get; set; } // L normalized
       

        //
        public double D { get; set; } // 
        public double DN { get; set; } // D normalized 
        public double DP { get; set; } // distance from the me Candidate to target node.




        public System.Windows.Point CenterLocation { get { return NeiNode.CenterLocation; } }
        //: The neighbor Node
        public Sensor NeiNode { get; set; }
    }

}
