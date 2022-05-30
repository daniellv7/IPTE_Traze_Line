using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Collections;

namespace IPTE_Base_Project.DataSources
{
    public static class ExcelDs
    {
        /// <summary>
        /// Reads an entire excel sheet and returns it in a DataTable.
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <param name="sheetName"></param>
        /// <returns>Returns an excel sheet in a DataTabble</returns>
        public static DataTable ReadSheet(string excelFilePath, string sheetName)
        {
            DataSet dataSet;
            OleDbConnection cn;
            OleDbDataAdapter command;

            try
            {
                //To use this libraty you have to install 32bit AccessDatabaseEngine.exe
                //Opens the connection:
                cn = new OleDbConnection("provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + excelFilePath + "';Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'");

                //Creates the select command:
                command = new OleDbDataAdapter("SELECT * FROM [" + sheetName + "$]", cn);
                command.TableMappings.Add("Table", "TestTable");

                //Creates the data sheet and read the whole sheet:
                dataSet = new DataSet();
                command.Fill(dataSet);

                //Closes the connection:
                cn.Close();
                
                return dataSet.Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading the excel.\nIf the excel file is open, please close it before starting the software.\n\nFull description:\n\n" + ex.ToString());
            }
        }

        /// <summary>
        /// Reads all the excel sheets specified and returns the result in a Dictionary where the key is the name of the sheet and the value is a DataTable with the content of the sheet.
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <param name="sheetNames"></param>
        /// <returns></returns>
        public static Dictionary<string, DataTable> ReadSheets(string excelFilePath, IEnumerable<string> sheetNames)
        {
            Dictionary<string, DataTable> result = new Dictionary<string, DataTable>();
            DataSet dataSet;
            OleDbConnection cn;
            OleDbDataAdapter command;

            try
            {
                //Opens the connection:
                cn = new OleDbConnection("provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + excelFilePath + "';Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'");

                foreach (var sheetName in sheetNames)
                {
                    //Creates the select command:
                    command = new OleDbDataAdapter("SELECT * FROM [" + sheetName + "$]", cn);
                    command.TableMappings.Add("Table", "TestTable");

                    //Creates the data sheet and read the whole sheet:
                    dataSet = new DataSet();
                    command.Fill(dataSet);

                    result.Add(sheetName, dataSet.Tables[0]);
                }
                
                //Closes the connection:
                cn.Close();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading the excel.\nIf the excel file is open, please close it before starting the software.\n\nFull description:\n\n" + ex.ToString());
            }
        }

        /// <summary>
        /// Reads an excel per rows and return it in a Dictionary
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static Dictionary<string, ArrayList> LoadWorksheetRows(string excelFilePath, string sheetName)
        {
            try
            {
                DataTable dataTable = ReadSheet(excelFilePath, sheetName);
                Dictionary<string, ArrayList> result = new Dictionary<string, ArrayList>();

                // Iterate datasheet rows and add information to dictionary
                for (int rowIndex = 1; rowIndex < dataTable.Rows.Count; rowIndex++)
                {
                    DataRow currentRow = dataTable.Rows[rowIndex];
                    string currentRowID = dataTable.Rows[rowIndex][0].ToString();
                    if (currentRowID.Equals(string.Empty)) break;
                    if (result.ContainsKey(currentRowID)) throw new Exception("ID " + currentRowID + " already exists in collection");
                    ArrayList array = new ArrayList();
                    for (int columnIndex = 1; columnIndex < currentRow.ItemArray.Length; columnIndex++)
                    {
                        string value = currentRow.ItemArray[columnIndex].ToString();
                        if (value.Equals(string.Empty)) break;
                        array.Add(value);
                    }
                    result.Add(currentRowID, array);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\nCould not obtain data from worksheet " + sheetName);
            }
        }

        /// <summary>
        /// Reads an excel per columns and return it in a Dictionary
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static Dictionary<string, ArrayList> LoadWorksheetColumns(string excelFilePath, string sheetName)
        {
            try
            {
                DataTable dataTable = ReadSheet(excelFilePath, sheetName);
                Dictionary<string, ArrayList> result = new Dictionary<string, ArrayList>();

                for (int columnsIndex = 0; columnsIndex < dataTable.Columns.Count; columnsIndex++)
                {
                    Dictionary<string, string> singleColumn = new Dictionary<string, string>();

                    // Get the information from each information column
                    string ID = dataTable.Rows[0][columnsIndex].ToString();
                    if (ID.Equals(string.Empty)) break;

                    ArrayList array = new ArrayList();

                    // Iteate rows to fill the singleColumn dictionary
                    for (int rowIndex = 1; rowIndex < dataTable.Rows.Count; rowIndex++)
                    {
                        string s = dataTable.Rows[rowIndex][columnsIndex].ToString();
                        if (dataTable.Rows[rowIndex][columnsIndex].ToString() == string.Empty) break;
                        array.Add(dataTable.Rows[rowIndex][columnsIndex].ToString());
                    }

                    result.Add(ID, array);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\nCould not obtain data from worksheet " + sheetName);
            }
        }

        /// <summary>
        /// Returns the row where the specified text appears
        /// </summary>
        /// <param name="dataTable">DataTable of an excel</param>
        /// <param name="iColumnToSearch">Column to search</param>
        /// <param name="iStartRow">Start row</param>
        /// <param name="textToSearch">Text to search</param>
        /// <returns>Returns the row where the specified text appears</returns>
        public static int GetRowStart(DataTable dataTable, int iColumnToSearch, int iStartRow, string textToSearch)
        {
            string sCell;


            for (int i = iStartRow; i < dataTable.Rows.Count; i++)
            {
                sCell = dataTable.Rows[i][iColumnToSearch].ToString();
                if (sCell != null)
                {
                    sCell = sCell.Trim();
                    if (sCell.StartsWith(textToSearch)) return i;
                }
            }

            return -1;
        }
    }
}
