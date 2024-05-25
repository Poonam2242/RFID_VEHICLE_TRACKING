using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTCPServer_CS
{
    internal class IOregister
    {
        public int D_Id { get; set; }
        public DateTime Inward_time { get; set; }
        public int ID { get; set; }
        public DateTime OutwardTime { get; set; }
        public string InAntennaName { get; set; }
        public string OutAntennaName { get; set; }
        public string LatestState { get; set; }

        public IOregister(int ID_=0, int D_Id_ = 0, DateTime Inward_time_=default, DateTime OutwardTime_= default, string InAntennaName_="", string OutAntennaName_="", string LatestState_ = "")
        {
            this.ID = ID_;
            this.D_Id = D_Id_;
            this.Inward_time = Inward_time_;
            this.OutwardTime = OutwardTime_;
            this.InAntennaName = InAntennaName_;
            this.OutAntennaName = OutAntennaName_;
            this.LatestState = LatestState_;
        }

        public IOregister()
        {

        }
    }
}
