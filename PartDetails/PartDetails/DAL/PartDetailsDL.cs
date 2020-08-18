using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
namespace PartDetails.DAL
{
  public  class PartDetailsDL
    {
        public static SqlConnection Sqlcon = new SqlConnection();
        Dictionary<int, string> lstRootPartConfig = new Dictionary<int, string>();

        /// <summary>
        ///  To get connection string from app.config
        /// </summary>
        public static string GetConnectionString()
        {
            string strConnString = ConfigurationManager.ConnectionStrings["connString"].ToString();
            return strConnString;
        }

        /// <summary>
        ///  To get Root part id and Configuration to bind Tab item headers
        /// </summary>
        public Dictionary<int, string> GetRootPartConfigNames()
        {
 
            SqlDataAdapter da;

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetRootConfig", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                                       
                    con.Open();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();
                    lstRootPartConfig = dt.AsEnumerable().ToDictionary<DataRow, int, string>(row => Convert.ToInt32(row[0]),
                                       row => row[1].ToString());


                }
            }
            return lstRootPartConfig;

        }

        /// <summary>
        ///  To get part hierarchy to bind TreeView
        /// </summary>
        /// <param name="iPartId">Selected root part id from the tab control</param>
        /// <param name="blnIsShowPart">Filter option to display part/file name in treeview based on radio button selection</param>
        public DataTable GetPartNodeDetails(int iPartId,bool blnIsShowPart)
        {
            DataTable dtParentChild = new DataTable();

            SqlDataAdapter da;

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetPartNodeDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@PartId", SqlDbType.Int).Value = iPartId;
                    cmd.Parameters.Add("@ShowPart", SqlDbType.Bit).Value = blnIsShowPart;
                

                    con.Open();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
        
                    con.Close();

                }
            }
            return dt;

        }


    }
}
