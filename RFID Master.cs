using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenericTCPServer_CS
{
    public partial class RFID_Master : Form
    {
        public RFID_Master()
        {
        }
        RfidMaster rf = new RfidMaster();
        public RFID_Master(RfidMaster rfid)
        {
            rf = rfid; ;
            InitializeComponent();

        }

        public void RFID_Master_Load(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            populategrid<RfidMaster>(rf);
        }

        public void populategrid<T>(T rfid)
        {
            //var dt=SqlDataConnection.GetMasterdetailsdatatable(rfid);
            // FunctionModule.populatedatatable(dataGridView1,dt);
            FunctionModule.populatedatatable<RfidMaster>(dataGridView1, rf);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = false;
            dataGridView1.Refresh();
            label2.Text = "You can Edit this form now";

        }

        private void RFID_Master_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you really want to close the file", "Attention!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                e.Cancel = false;
                return;
            }
            else
            {
                e.Cancel = true;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Make Changes here if new field;
            RfidMaster rfid = new RfidMaster();
            
            rfid.D_Id = int.Parse(dataGridView1[1,0].Value.ToString());
            rfid.Driver_Name = dataGridView1[1,1].Value as string;
         
            rfid.Vehicle_No = dataGridView1[1,2].Value as string;
            rfid.License_Date = dataGridView1[1,3].Value as string;
            rfid.License_No = dataGridView1[1, 4].Value as string;
            rfid.Transport_Name = dataGridView1[1,5].Value as string;
            rfid.Model_Name = dataGridView1[1,6].Value as string;
            rfid.Insurance = dataGridView1[1,7].Value as string;
            rfid.PUC = dataGridView1[1,8].Value as string;
            rfid.Fitness = dataGridView1[1,9].Value as string;
            rfid.RFID_Hex = dataGridView1[1,10].Value as string;
            rfid.RFID_Text = dataGridView1[1,11].Value as string;
            rfid.Phone_No = Int64.Parse(dataGridView1[1,12].Value.ToString() );
            rfid.Created_On = DateTime.Parse(dataGridView1[1, 13].Value.ToString());


            SqlDataConnection.SaveDetails(rfid);
            dataGridView1.DataSource = null;
            this.Close();
        }
    }
}
