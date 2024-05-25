using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTCPServer_CS
{
      public class AntennaDetails
    {
        public string AntennaName { get; set; }
        public string AntennaIP { get; set; }
        public string Purpose { get; set; }
        public string Description { get; set; }

        public AntennaDetails(string AntennaName_=null, string AntennaIP_=null, string Purpose_=null, string Description_ = null)
        {
            this.AntennaName = AntennaName_;
            this.AntennaIP = AntennaIP_;
            this.Purpose = Purpose_;
            this.Description = Description_;
        }
        public AntennaDetails() { }
    }
}
