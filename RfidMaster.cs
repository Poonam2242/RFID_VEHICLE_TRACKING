using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTCPServer_CS
{
    public  class RfidMaster
    {
        public int D_Id { get; set; }
        public string Driver_Name { get; set; }
        public string Vehicle_No { get; set; }
        public string License_Date { get; set; }
        public string License_No { get; set; }
        public string Transport_Name { get; set; }
        public string Model_Name { get; set; }
        public string Insurance { get; set; }
        public string PUC { get; set; }
        public string Fitness { get; set; }
        public string RFID_Hex { get; set; }
        public string RFID_Text { get; set; }
        public long Phone_No { get; set; }
        public DateTime Created_On { get; set; }
        public RfidMaster(int D_Id_=0,string Driver_Name_=null,string Vehicle_No_=null,string License_No_=null,string Transport_Name_=null,string Model_Name_=null,string RFID_Hex_=null,string RFID_Text_=null,long Phone_No_=0)
        {
            this.D_Id = D_Id_;
            this.Driver_Name = Driver_Name_;
            this.Vehicle_No = Vehicle_No_;
            this.License_No = License_No_;
            this.Transport_Name = Transport_Name_;
            this.Model_Name = Model_Name_;
            this.RFID_Hex = RFID_Hex_;
            this.RFID_Text = RFID_Text_;
            this.Phone_No = Phone_No_;
            this.Created_On = DateTime.Now;
        }
        public RfidMaster()
        {
        }

    }
}
