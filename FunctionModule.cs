using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace GenericTCPServer_CS
{
    internal class FunctionModule
    {
        public static string Getsettings(string infomration)
        {
            return ConfigurationManager.AppSettings[infomration].ToString();
        }

        public static void populatedatatable(DataGridView dgv, DataTable dt)
        {

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                int rowid = dgv.Rows.Add();
                dgv[0, rowid].Value = dt.Columns[i].ColumnName.ToString();
            }

            if (dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dgv[1, i].Value = dt.Rows[0][i].ToString();
                }

            }
            
        }

        public static void populatedatatable<T>(DataGridView dgv,T rf) where T : new()
        {
            var obj = new T();
            var properties = from p in obj.GetType().GetProperties() select p;
            DataTable dt = new DataTable();
            dt.Columns.Add("Property Name");
            dt.Columns.Add("Property Value");
            int i=0;

            foreach (var item in properties)
            {
                dt.Rows.Add();
                dt.Rows[i][0]=item.Name.ToString();
                dt.Rows[i][1]=item.GetValue(rf);
                i++;
            }
            dgv.DataSource = dt;

        }

        public static void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}
