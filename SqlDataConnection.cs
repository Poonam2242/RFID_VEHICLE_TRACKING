using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using Dapper;
using System.Configuration;
using System.Security.Permissions;
using System.Web;
using System.Runtime.Remoting.Messaging;

namespace GenericTCPServer_CS
{
    public class SqlDataConnection
    {
        public static string SqlConnectionstring(string connectionname = "GateEntry")
        {
            return ConfigurationManager.ConnectionStrings[connectionname].ToString();
        }

        public static List<RfidMaster> GetMasterdetails(RfidMaster rfidMaster)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Concat("select * from tbl_Master where RFID_Hex='", rfidMaster.RFID_Hex, "'"));

            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(SqlConnectionstring()))
            {
                var value = cnn.Query<RfidMaster>(sb.ToString(), new DynamicParameters());
                return value.ToList();
            }

        }

        public static DataTable GetMasterdetailsdatatable(RfidMaster rfidMaster)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = new DataTable();
            sb.Append(string.Concat("select * from tbl_Master where RFID_Hex='", rfidMaster.RFID_Hex, "'"));

            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(SqlConnectionstring()))
            {
                var value = cnn.ExecuteReader(sb.ToString(), new DynamicParameters());
                dt.Load(value);
                return dt;
            }

        }

        public static bool isdataexist(RfidMaster rfidMaster,AntennaDetails antenna)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Concat("select top 1  LatestState+'|'+InAntennaName+'|'+isnull(OutAntennaName,'NA') as RecordCount from tbl_IOregister where D_Id=(Select D_Id from tbl_Master where RFID_Hex='", rfidMaster.RFID_Hex, "') order by Inward_time desc;"));

            using (IDbConnection cnn = new SqlConnection(SqlConnectionstring()))
            {
                var value = cnn.ExecuteScalar(sb.ToString(), new DynamicParameters());
                bool returnboolean;

                if(value ==null) return false;
                if (value.ToString().Split('|')[0] == "OUT") return false;
                if (value.ToString().Split('|')[0] == "IN" && value.ToString().Split('|')[1] == antenna.AntennaName) return true;
                return false;

            }
        }

        public static int SaveDetails(RfidMaster rfid)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = new DataTable();
            if (rfid.D_Id == 0)
            {
                sb.Append(string.Concat("insert into tbl_Master(Driver_Name ,Vehicle_No,License_Date,License_No,Transport_Name,Model_Name,Insurance,PUC,Fitness,RFID_Hex,RFID_Text,Phone_No) values(@Driver_Name ,@Vehicle_No,@License_Date,@License_No,@Transport_Name,@Model_Name,@Insurance,@PUC,@Fitness,@RFID_Hex,@RFID_Text,@Phone_No)"));

            }
            else
            {
                sb.Append(string.Concat("update tbl_Master set Driver_Name=@Driver_Name,Transport_Name=@Transport_Name,License_No=@License_No,Vehicle_No=@Vehicle_No,Model_Name=@Model_Name,Phone_No=@Phone_No,RFID_hex=@RFID_hex,RFID_text=@RFID_text where D_Id=", rfid.D_Id, ""));
            }

            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(SqlConnectionstring()))
            {
                var value = cnn.Execute(sb.ToString(), rfid);
                return value;
            }
        }
        public static int SetIoRegister(RfidMaster rfidMaster, AntennaDetails antenna)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = new DataTable();
            var Ioregisterdata = CheckIoRegister(rfidMaster);
            IOregister ioregister=new IOregister();
            if (Ioregisterdata != null && Ioregisterdata.Count>0)
            {
               
                
                ioregister = Ioregisterdata[0];
                ioregister.LatestState = "OUT";
                ioregister.OutwardTime = DateTime.Now;
                ioregister.OutAntennaName = antenna.AntennaName;
            }
            else
            {
                ioregister.Inward_time=DateTime.Now;
                
                ioregister.LatestState = "IN";
                ioregister.InAntennaName = antenna.AntennaName;
                if (!ioregister.InAntennaName.ToLowerInvariant().Contains(ioregister.LatestState.ToLowerInvariant())) return 0;
            }

            ioregister.D_Id = rfidMaster.D_Id;
            if(ioregister.LatestState=="IN")
            {
                sb.Append(string.Concat("  insert into tbl_IOregister(D_Id ,Inward_time ,InAntennaName ,OutAntennaName  ,LatestState) ",
               " values(@D_Id,@Inward_time,@InAntennaName,@OutAntennaName,@LatestState)"));
            }
            else
            {
                sb.Append(string.Concat(" update tbl_IOregister set OutwardTime=@OutwardTime, OutAntennaName =@OutAntennaName ,LatestState=@LatestState where ID=@ID "));
            }
           

            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(SqlConnectionstring()))
            {
                var value = cnn.Execute(sb.ToString(), ioregister);
                return value;
            }
        }


        private static List<IOregister> CheckIoRegister(RfidMaster rfidmaster)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Concat("select * from tbl_IOregister where D_Id= '",rfidmaster.D_Id,"' and LatestState='IN'"));
            using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(SqlConnectionstring()))
            {
              var value= cnn.Query<IOregister>(sb.ToString(), new DynamicParameters());
                return value.ToList();
            }
        }
    


    public static List<AntennaDetails> GetAntennaDetails(AntennaDetails antdetails)
    {
        var list = new List<AntennaDetails>();
        StringBuilder sb = new StringBuilder();
        DataTable dt = new DataTable();
        sb.Append(string.Concat("select * from tbl_AntennaDetails where antennaip='", antdetails.AntennaIP, "'"));
        using (IDbConnection cnn = new System.Data.SqlClient.SqlConnection(SqlConnectionstring()))
        {
            var value = cnn.Query<AntennaDetails>(sb.ToString(), new DynamicParameters());
            return value.ToList();
        }

    }

    public static DataTable GetHistory()
    {
        DataTable dt = new DataTable();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(String.Concat("select b.Transport_Name  Transporter, b.Vehicle_No 'Vehicle No.',b.Driver_Name,b.Model_Name Model,b.License_No 'DL No.',b.License_Date 'DL Date',b.Insurance,b.PUC,b.Fitness,b.Driver_Name 'Driver Name',b.Phone_No 'Dx Mobile Nos',a.Inward_time as 'Vehicle Inward Time',a.OutwardTime as 'Vehicle Outward Time',a.LatestState,b.RFID_hex ",
            " from tbl_IOregister a ",
            "inner join  tbl_Master b on a.D_Id=b.D_Id ",
            "where CAST(a.Inward_time as date)=CAST(getdate() as date) order by a.Inward_time desc"));
        using (IDbConnection cnn = new SqlConnection(SqlConnectionstring()))
        {
            dt.Load(cnn.ExecuteReader(sb.ToString(), new DynamicParameters()));
            return dt;
        }
    }
}
}
