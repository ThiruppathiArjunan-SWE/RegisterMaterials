using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegisterMaterials
{
    public partial class Form1 : Form
    {
        public string myConnectionString1 = @"" + ConfigurationManager.ConnectionStrings["MyDbConn"].ToString() + "";
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
                openFileDialog.Title = "Select an Excel File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtfile.Text = openFileDialog.FileName;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string excelFilePath = txtfile.Text;

            if (string.IsNullOrWhiteSpace(excelFilePath) || excelFilePath == "No file selected")
            {
                MessageBox.Show("Please select an Excel file first.");
                return;
            }


            string connectionString = myConnectionString1;

            try
            {
                // Load the Excel file
                using (var workbook = new XLWorkbook(excelFilePath))
                {
                    // Assuming the data is in the first worksheet
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed();

                    // Create a DataTable to hold the Excel data
                    var dataTable = new DataTable();

                    // Adding columns to the DataTable
                    foreach (var cell in rows.First().CellsUsed())
                    {
                        dataTable.Columns.Add(cell.Value.ToString());
                    }

                    // Adding rows to the DataTable
                    foreach (var row in rows.Skip(1))
                    {
                        var dataRow = dataTable.NewRow();
                        for (int i = 0; i < row.CellsUsed().Count(); i++)
                        {
                            dataRow[i] = row.Cell(i + 1).Value;
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow cell in dataTable.Rows)
                        {


                            object value = cell["DID"];
                            if (value != DBNull.Value)
                            {


                                string d = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                                if (cell["DID"] != null)
                                {
                                    string DID = cell["DID"].ToString();
                                    string Part = cell["Part Number"].ToString();
                                    object Packaging = cell["Packaging Type"];


                                    if (Packaging == DBNull.Value)
                                    {
                                        Packaging = 0;
                                    }
                                    else if (Packaging == "")
                                    {
                                        Packaging = 0;
                                    }
                                    //
                                    object GRNDate = cell["GRN Date"];
                                    if (GRNDate == DBNull.Value)
                                    {
                                        GRNDate = d;
                                    }
                                    else if (GRNDate == "")
                                    {
                                        GRNDate = d;
                                    }

                                    object Qty = cell["Qty"];
                                    if (Qty == DBNull.Value)
                                    {
                                        Qty = 0;
                                    }
                                    else if (Qty == "")
                                    {
                                        Qty = 0;
                                    }

                                    object Vendor = cell["Vendor"];
                                    if (Vendor == DBNull.Value)
                                    {
                                        Vendor = "";
                                    }
                                    object Partnumber = cell["Part Number"];
                                    if (Partnumber == DBNull.Value)
                                    {
                                        Partnumber = "";
                                    }

                                    object SupplierBatch = cell["Supplier Batch"];
                                    if (SupplierBatch == DBNull.Value)
                                    {
                                        SupplierBatch = "";
                                    }

                                    //Supplier Batch

                                    DateTime GRN_Date = Convert.ToDateTime(GRNDate);

                                    using (System.Data.SqlClient.SqlConnection myConnection = new System.Data.SqlClient.SqlConnection(connectionString))
                                    {
                                        myConnection.Open();
                                        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("[dbo].[Usp_Saveand_update_material_training]", myConnection);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@DID", DID);
                                        cmd.Parameters.AddWithValue("@Partnumber", Partnumber);
                                        cmd.Parameters.AddWithValue("@Qty", Convert.ToInt32(Qty));
                                        cmd.Parameters.AddWithValue("@packageType", Convert.ToInt32(Packaging));
                                        cmd.Parameters.AddWithValue("@Vendorname", Vendor);
                                        cmd.Parameters.AddWithValue("@GRN_Date", GRN_Date);
                                        cmd.Parameters.AddWithValue("@SupplierBatch", SupplierBatch);
                                        cmd.ExecuteNonQuery();
                                        myConnection.Close();
                                    }


                                }




                            }


                        }



                    }


                }



                MessageBox.Show("Data imported successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
