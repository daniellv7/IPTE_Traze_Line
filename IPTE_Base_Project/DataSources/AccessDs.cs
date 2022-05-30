using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.DataSources
{
    public static class AccessDs
    {
        public static DataTable GetTable(string filePath, string tableName)
        {
            string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;data Source={filePath}";

            DataTable results = new DataTable();

            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                OleDbCommand command = new OleDbCommand($"SELECT * FROM {tableName}", connection);

                connection.Open();

                OleDbDataAdapter adapter = new OleDbDataAdapter(command);

                adapter.Fill(results);
                return results;
            }
        }

        
        public static string GetCoreAsmNumberFromDB(string filePath)
        {
            using (OleDbConnection connection = new OleDbConnection("provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Jet OLEDB:Database Password=;"))
            {
                connection.Open();
                string strQuery = "SELECT * " +
                    "FROM ErrorDef " +
                    "WHERE ID='08'";
                OleDbCommand command = new OleDbCommand(strQuery, connection);
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader["Error"].ToString();
                    }
                    return string.Empty;
                }
            }
        }

        public static bool SerialNumberInErrorBlocker(string path, string serialNumber)
        {
            string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;data Source={path}";
            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                connection.Open();
                //OleDbCommand command = new OleDbCommand($"SELECT COUNT(*) FROM SerialList WHERE serial=\"{serialNumber}\"", connection);
                OleDbCommand command = new OleDbCommand($"SELECT COUNT(*) FROM SerialList WHERE serial LIKE '{serialNumber}'", connection);

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int count = int.Parse(reader[0].ToString());
                        if(count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

//        public static bool SetEEPROMValues(string path, string learPartNumber)
//        {
//            string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;data Source={path}";
//#if DEBUG
//            return true;
//#else
//            using (OleDbConnection connection = new OleDbConnection(connString))
//            {
//                connection.Open();
//                // TODO: Define UPDATE-INSERT sql command
//                OleDbCommand command = new OleDbCommand($"SELECT COUNT(*) FROM SerialList WHERE serial LIKE '{serialNumber}'", connection);

//                using (OleDbDataReader reader = command.ExecuteReader())
//                {
//                    if (reader.Read())
//                    {
//                        int count = int.Parse(reader[0].ToString());
//                        if (count > 0)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//            return false;
//#endif
//        }
    }
}
