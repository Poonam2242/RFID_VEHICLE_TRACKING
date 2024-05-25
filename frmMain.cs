using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using GenericTCPServer_CS;
using System.Runtime;
using System.Diagnostics;

namespace GenericTCPServer_CS
{
    public partial class frmMain : Form
    {
        TcpListener server;
        List<TcpClient> clients;
        bool bListenerEnbled;
        Thread tListener;
        bool bEnableTextBoxLogging;

        public frmMain()
        {
            InitializeComponent();

            bEnableTextBoxLogging = true;
        }
        DataTable dt = new DataTable();
        List<RfidMaster> rfidmastlist = new List<RfidMaster>();


        private void AddLog(string data)
        {
            DateTime dt = DateTime.Now;

            if (bEnableTextBoxLogging)
            {
                if (!txtLog.InvokeRequired)
                {

                    if (txtLog.Text.Length < 15000)
                        txtLog.Text = dt.ToString("dd/MM/yyyy HH:mm:ss.ffffff - ") + data + Environment.NewLine + txtLog.Text;
                    else
                        txtLog.Text = dt.ToString("dd/MM/yyyy HH:mm:ss.ffffff - ") + data + Environment.NewLine + txtLog.Text.Substring(0, 15000);
                }
                else
                {
                    txtLog.Invoke((MethodInvoker)delegate
                    {
                        if (txtLog.Text.Length < 15000)
                            txtLog.Text = dt.ToString("dd/MM/yyyy HH:mm:ss.ffffff - ") + data + Environment.NewLine + txtLog.Text;
                        else
                            txtLog.Text = dt.ToString("dd/MM/yyyy HH:mm:ss.ffffff - ") + data + Environment.NewLine + txtLog.Text.Substring(0, 15000);
                    });
                }
            }

            /*            if (txtLog.Text.Length < 15000)
                            txtLog.Text = dt.ToString("dd/MM/yyyy HH:mm:ss.ffffff - ") + data + Environment.NewLine + "-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine + txtLog.Text;
                        else
                            txtLog.Text = dt.ToString("dd/MM/yyyy HH:mm:ss.ffffff - ") + data + Environment.NewLine + "-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine + txtLog.Text.Substring(0, 15000);
             */
            File.AppendAllText("Log.txt", dt.ToString("dd/MM/yyyy HH:mm:ss.ffffff - ") + data + Environment.NewLine);
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text == "Start")
            {

                try
                {
                    int port = int.Parse(txtPort.Text);
                    server = new TcpListener(IPAddress.Any, port);
                    server.Start();
                    clients = new List<TcpClient>();

                    bListenerEnbled = true;
                    tListener = new Thread(new ThreadStart(ListenerThread));
                    tListener.Start();

                    tmrServerTasks.Enabled = true;
                }
                catch (Exception ex)
                {
                    AddLog("Error starting Listener\r\n" + ex.ToString());
                    return;
                }
                txtPort.ReadOnly = true;
                btnStartStop.Text = "Stop";
                AddLog("Listener started successfully");
            }
            else if (btnStartStop.Text == "Stop")
            {
                bListenerEnbled = false;
                //tListener.Abort();

                tmrServerTasks.Enabled = false;

                foreach (var client in clients)
                    client.Close();

                server.Stop();
                clients.Clear();
                clients = null;

                txtPort.ReadOnly = false;
                btnStartStop.Text = "Start";

                AddLog("Listener stopped successfully");
            }
        }

        private void ListenerThread()
        {
            AddLog("Listener thread started");
            while (bListenerEnbled)
            //while (true)
            {
                try
                {
                    while (server.Pending())
                    {
                        AddLog("Pending client connection - Trying to accept");
                        TcpClient client = server.AcceptTcpClient();
                        AddLog("New Client Connected - " + client.Client.RemoteEndPoint.ToString()); // + "\r\nTotal Clients = " + clients.Count);
                        for (int i = 0; i < clients.Count; i++)
                        {
                            if (clients[i].Client.RemoteEndPoint.ToString().Split(new char[] { ':' })[0] == client.Client.RemoteEndPoint.ToString().Split(new char[] { ':' })[0])
                            {
                                clients.RemoveAt(i);
                                i--;
                            }
                        }
                        clients.Add(client);
                        Thread.Sleep(1);
                    }
                }
                catch (SocketException ex)
                {
                    AddLog("Error in Accepting New Client\r\n" + ex.ToString());
                    if (ex.Message.Contains("A blocking operation was interrupted"))
                        break;
                }
                catch (InvalidOperationException ex)
                {
                    AddLog("Error in Accepting New Client\r\n" + ex.ToString());
                    if (ex.Message.Contains("Not listening."))
                        break;
                }
                catch (Exception ex)
                {
                    AddLog("Error in Accepting New Client\r\n" + ex.ToString());
                }
                Thread.Sleep(5);
            }
            AddLog("Listener thread stopped");
        }

        private void tmrServerTasks_Tick(object sender, EventArgs e)
        {
            tmrServerTasks.Enabled = false;
            for (int i = 0; i < clients.Count; i++)
            {
                try
                {
                    TcpClient client = clients[i];
                    //                    while (client.Available > 0)
                    if (client.Available > 0)
                    {
                        AddLog(client.Client.RemoteEndPoint.ToString() + " - Data available - Trying to read");
                        NetworkStream stream = client.GetStream();
                        byte[] data = new byte[1000];
                        int len = stream.Read(data, 0, 1000);
                        string RFIDID = ByteToHexString(data, len).Substring(12, 24);
                        AddLog(client.Client.RemoteEndPoint.ToString() + " --- " + RFIDID + " --- " + HexStringtoAscii(RFIDID));
                        if (!RFIDID.StartsWith("E")) { AddLog("IN Correct data recieved" + " --> " + RFIDID); return; }
                        RfidMaster rfidMaster = new RfidMaster(RFID_Hex_: RFIDID);

                        rfidmastlist = SqlDataConnection.GetMasterdetails(rfidMaster);
                        if (rfidmastlist.Count == 0)
                        {
                            rfidmastlist.Add(new RfidMaster { RFID_Hex = RFIDID, RFID_Text = HexStringtoAscii(RFIDID).Replace("\r", string.Empty) });
                            button1.PerformClick();
                            var returnvalue = SqlDataConnection.GetMasterdetails(rfidMaster);
                            if (returnvalue.Count > 0)
                            {
                                rfidmastlist = returnvalue;
                            }
                        }
                        rfidMaster = rfidmastlist[0];
                        var value = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                        AntennaDetails antenna = new AntennaDetails(AntennaIP_: value);
                        antenna = SqlDataConnection.GetAntennaDetails(antenna)[0];
                        if (!SqlDataConnection.isdataexist(rfidMaster, antenna)) SqlDataConnection.SetIoRegister(rfidMaster, antenna);
                        textBox1.Text = rfidMaster.RFID_Hex;
                        textBox2.Text = rfidMaster.Vehicle_No;

                        dataGridView1.DataSource = SqlDataConnection.GetHistory() as DataTable;


                        //ByteToHexString(data, len));
                        /*while (len >= 18)
                        {
                            if (data[0] != 0x11 || data[2] != 0xEE || data[3] != 0x00)
                            {
                                AddLog("Unrecognized Data Received from " + client.Client.RemoteEndPoint.ToString() + "\r\nData: " + ByteToHexString(data, len));
                                len = 0;
                                break;
                            }
                            else
                            {
                                AddLog("Data Received from " + client.Client.RemoteEndPoint.ToString() + "\r\nData: " + ByteToHexString(data, 18));
                                Array.Copy(data, 18, data, 0, len - 18);
                                len -= 18;
                            }
                        }
                        if(len < 18 && len > 0)
                            AddLog("Wrong Data Received from " + client.Client.RemoteEndPoint.ToString() + "\r\nData: " + ByteToHexString(data, len));
                        */
                    }
                }
                catch (Exception ex)
                {
                    AddLog("Problem Reading Data\r\n" + ex.ToString());
                    clients.RemoveAt(i);
                    i--;
                }
            }
            tmrServerTasks.Enabled = true;
        }

        private static string ByteToHexString(byte[] data, int len)
        {
            string strData = string.Empty;
            for (int i = 0; i < len; i++)
                strData += string.Format("{0:X2}", data[i]);
            return strData;
        }

        public static string HexStringtoAscii(string hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return string.Empty;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLog.Text = string.Empty;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            AddLog("Closing Application");
            bEnableTextBoxLogging = false;
            if (btnStartStop.Text == "Stop")
                btnStartStop_Click(sender, e);
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            label8.Text = FunctionModule.Getsettings("AppName");
            label8.Text = FunctionModule.Getsettings("AppName");
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //RFID_Master rfid=new RFID_Master();
            //rfid.ShowDialog();
            using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
            {
                pProcess.StartInfo.FileName = @"UHFR03.exe";
                pProcess.StartInfo.Arguments = ""; //argument
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                pProcess.Start();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RFID_Master rFID_Master = new RFID_Master(rfidmastlist[0]);
            rFID_Master.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Comma Separated Values|*.csv";
            var value = dataGridView1.DataSource as DataTable;
            if (value == null)
            {
                return;
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FunctionModule.ToCSV(value, saveFileDialog1.FileName);
            }
        }
    }
}