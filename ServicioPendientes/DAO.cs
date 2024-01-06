using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicioPendientes
{
    internal class DAO
    {
        private static SqlConnection conn = new SqlConnection(SettingsPendings.Default.dbConnection);
        private static SqlCommand cmd = new SqlCommand("", conn);
        private static Logger logger = new Logger();


        private void OpenConnection()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                logger.Log(Logger.LogType.BOTH,
                    $"ERROR AL ACCEDER A LA BASE {ex.Message}",
                    Logger.LogLevel.FATAL);
            }
        }
        private void CloseConnection()
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Log(Logger.LogType.BOTH,
                    $"ERROR AL CERRAR CONEXION A LA BASE {ex.Message}",
                    Logger.LogLevel.FATAL);
            }
        }
        public void ExecuteQueueing(string file)
        {
            try
            {
                OpenConnection();

                cmd = new SqlCommand("GSC_UploadPendings", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@file", SqlDbType.VarChar, 2000);
                cmd.Parameters.Add("@msg", SqlDbType.VarChar, 2000).Direction = ParameterDirection.Output;
                cmd.Parameters["@file"].Value = file;
                cmd.ExecuteNonQuery();
                string documentoId = cmd.Parameters["@msg"].Value.ToString();

                CloseConnection();
            }
            catch (Exception ex)
            {
                logger.Log(Logger.LogType.BOTH,
                    $"ERROR AL EJECUTAR PROCEDIMIENTO GSC_UploadPendings :{ex.Message}",
                    Logger.LogLevel.FATAL);
            }
        }

        public void ExecuteScheduling()
        {
            try
            {
                OpenConnection();

                cmd = new SqlCommand("GSC_SchedulePendings", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@msg", SqlDbType.VarChar, 2000).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                string documentoId = cmd.Parameters["@msg"].Value.ToString();

                CloseConnection();
            }
            catch (Exception ex)
            {
                logger.Log(Logger.LogType.BOTH,
                    $"ERROR AL EJECUTAR PROCEDIMIENTO GSC_SchedulePendings :{ex.Message}",
                    Logger.LogLevel.FATAL);
            }
        }

        public void ExecuteCleanning()
        {
            try
            {
                OpenConnection();

                cmd = new SqlCommand("GSC_CleanPendings", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@msg", SqlDbType.VarChar, 2000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@daysBack", SqlDbType.Int);
                cmd.Parameters["@daysBack"].Value = SettingsPendings.Default.daysBackCleaning;
                cmd.ExecuteNonQuery();
                string documentoId = cmd.Parameters["@msg"].Value.ToString();

                Console.WriteLine(documentoId);

                CloseConnection();

            }
            catch (Exception ex)
            {
                logger.Log(Logger.LogType.BOTH,
                    $"ERROR AL EJECUTAR PROCEDIMIENTO GSC_CleanPendings :{ex.Message}",
                    Logger.LogLevel.FATAL); ;
            }
        }

    }
}
