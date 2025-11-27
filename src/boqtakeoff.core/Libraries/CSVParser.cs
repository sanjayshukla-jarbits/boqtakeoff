using Microsoft.Office.Interop.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;
using boqtakeoff.core.Libraries;

namespace boqtakeoff.core
{

    public class CSVParser
    {
        public static string ErrorMeessage { get; set; }
        static Hashtable errorMappings = new Hashtable();
        public static List<BoqLineItemDetails> GetBOQDetailsHashtable(string filePath, string project_id, string version_number)
        {
            String csvLine = null;
            System.IO.StreamReader srCSV = new System.IO.StreamReader(filePath);
            char[] separator = new char[] { ',' };

            int count = 1;
            List<BoqLineItemDetails> m_create_boq_details = new List<BoqLineItemDetails>();

            while ((csvLine = srCSV.ReadLine()) != null)
            {
                // INFO:: Neglecting the header row
                if (count == 1)
                {
                    count++;
                    continue;
                }

                string[] rowValues = csvLine.Split(separator, StringSplitOptions.None);
                BoqLineItemDetails boq_lid_details = new BoqLineItemDetails();

                // Data Sanity Checks
                // Data Format: Second Column: SKU, Fourth Column: Quantity
                if (rowValues[1] != null && rowValues[1].Length > 0 && rowValues[3] != null && rowValues[3].Length > 0)
                {
                    boq_lid_details.item_name = rowValues[0];
                    boq_lid_details.item_sku = rowValues[1];
                    boq_lid_details.quantity = rowValues[3];
                    boq_lid_details.level_name = rowValues[5];
                    boq_lid_details.item_category = rowValues[6];
                    boq_lid_details.project_id = project_id;
                    boq_lid_details.version_number = version_number;

                    m_create_boq_details.Add(boq_lid_details);
                }
            }
            return m_create_boq_details;
        }




        public static List<BoqLineItemDetails> GetBOQDetailsHashtable_msxp(string filePath, string project_id, string version_number)
        {
            List<BoqLineItemDetails> m_create_boq_details = new List<BoqLineItemDetails>();

            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(filePath);

            foreach (Worksheet sheet in xlWorkbook.Worksheets)
            {
                // TaskDialog.Show("Worksheet Name", sheet.Name.ToString());
                int rowCount = sheet.UsedRange.Rows.Count;
                int columnCount = sheet.UsedRange.Columns.Count;
                string[] rowValues = new string[columnCount];

                /*
                for (int row = 0; row <= rowCount; row++)
                {
                    int column = 3;
                    string cellName = ExcelCellReference(row, column);
                    Microsoft.Office.Interop.Excel.Range oneCellRange;
                    oneCellRange = sheet.get_Range(cellName, cellName);
                    object oneObject;

                    // contrary to MSDN, the value Empty, checkable with method IsEmpty() 
                    // does not work if the cell was empty.
                    // instead, what happens is that null is returned.
                    oneObject =
                       (object)
        oneCellRange.get_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault);
                    oneObject.ToString();
                }
                */
                // INFO:: Neglecting the header row
                int rowNum = 1;

                foreach (Excel.Range row in sheet.UsedRange.Rows)
                {
                    if (rowNum == 1)
                    {
                        rowNum++;
                        continue;
                    }
                    int colNum = 1;
                    BoqLineItemDetails boq_lid_details = new BoqLineItemDetails();
                    foreach (Excel.Range cell in row.Columns)
                    {
                        // Data Sanity Checks
                        // Data Format: Second Column: SKU, Fourth Column: Quantity
                        if (cell.Value != null && colNum == 1)
                        {
                            boq_lid_details.item_name = cell.Value.ToString();
                        }
                        else if (cell.Value != null && colNum == 2)
                        {
                            boq_lid_details.item_sku = cell.Value.ToString();
                        }
                        else if (cell.Value != null && colNum == 4)
                        {
                            boq_lid_details.quantity = cell.Value.ToString();
                        }
                        else if (cell.Value != null && colNum == 6)
                        {
                            boq_lid_details.level_name = cell.Value.ToString();
                        }
                        else if (cell.Value != null && colNum == 7)
                        {
                            boq_lid_details.item_category = cell.Value.ToString();
                        }
                        boq_lid_details.project_id = project_id;
                        boq_lid_details.version_number = version_number;

                        colNum++;
                    }
                    m_create_boq_details.Add(boq_lid_details);
                }
            }
            xlApp.Quit();
            return m_create_boq_details;
        }

        public static List<BoqLineItemDetails> GetBOQDetailsHashtable_msxp_new(string filePath, string project_id, string version_number)
        {
            try
            {

                /* calling below method to collect error code details*/

                errorMappings = Utility.LoadErrorMappingFromConfiguration();
                List<BoqLineItemDetails> m_create_boq_details = new List<BoqLineItemDetails>();

                List<getErrorLog> _listgetErrorLog = new List<getErrorLog>();

                getErrorLog _listErrorLog = new getErrorLog();
                //MessageBox.Show("filePath: " + filePath);
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(filePath);

                foreach (Worksheet sheet in xlWorkbook.Worksheets)
                {
                    //TaskDialog.Show("Worksheet Name", sheet.Name.ToString());
                    int rowCount = sheet.UsedRange.Rows.Count;
                    int columnCount = sheet.UsedRange.Columns.Count;
                    string[] rowValues = new string[columnCount];

                    // INFO:: Neglecting the header row
                    int rowNum = 1;

                    List<string> columnNames = new List<string>();
                    int hasAllImportentColumn = 0;
                    // Iterate through the cells in the first row to get column names.
                    for (int i = 1; i <= columnCount; i++)
                    {
                        var cell = sheet.UsedRange.Cells[1, i] as Microsoft.Office.Interop.Excel.Range;
                        var cellValue = Convert.ToString(cell?.Value2)?.Trim();
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            var lowerValue = cellValue.ToLowerInvariant();
                            if (lowerValue == "item sku" || lowerValue == "quantity" || lowerValue == "uom")
                            {
                                hasAllImportentColumn++;
                            }
                            columnNames.Add(lowerValue);
                        }
                        else
                        {
                            // If a cell is empty, you can assign a default column name or skip it.
                            columnNames.Add("Column" + i);
                        }
                    }
                    ///Data Missing -SKU, Quantity, UOM

                    ErrorMeessage = "";
                    if (hasAllImportentColumn < 3)
                    {
                        if (columnNames.IndexOf("item sku") < 1)
                        {
                            ErrorMeessage = "item sku column is missing. Please correct the data and try again \r\n ";
                        }
                        if (columnNames.IndexOf("quantity") < 1)
                        {
                            ErrorMeessage += "Quantity column is missing. Please correct the data and try again  \r\n ";
                        }
                        if (columnNames.IndexOf("uom") < 1)
                        {
                            ErrorMeessage += "UOM column is missing. Please correct the data and try again  \r\n";
                        }

                        MessageBox.Show(ErrorMeessage, "Data Column not availabe!! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (hasAllImportentColumn == 3)
                    {
                        _listgetErrorLog = new List<getErrorLog>();
                        foreach (Microsoft.Office.Interop.Excel.Range row in sheet.UsedRange.Rows)
                        {
                            if (rowNum == 1)
                            {
                                rowNum++;
                                continue;
                            }
                            int colNum = 1;
                            BoqLineItemDetails boq_lid_details = new BoqLineItemDetails();
                            string rNumber = (rowNum).ToString();
                            _listErrorLog = new getErrorLog();
                            ErrorMeessage = "";
                            foreach (Microsoft.Office.Interop.Excel.Range cell in row.Columns)
                            {
                                SetValueByIndex(Convert.ToString(cell.Value), colNum, boq_lid_details, columnNames, project_id, version_number);
                                colNum++;
                            }
                            int hasIssue = 0;
                            if (boq_lid_details.uom == null && boq_lid_details.quantity == null && boq_lid_details.item_sku == null)
                            {
                                // INFO:: Got the empty row, need to neglect it
                                continue;
                            }
                            else
                            {
                                if (boq_lid_details.uom == null)
                                {
                                    ErrorMeessage = (string)errorMappings["UOM_IS_EMPTY"];
                                    hasIssue = 1;
                                }
                                if (boq_lid_details.quantity == null)
                                {
                                    ErrorMeessage += (string)errorMappings["QUANTITY_IS_EMPTY"];
                                    hasIssue = 1;
                                }
                                if (boq_lid_details.quantity != null)
                                {
                                    try
                                    {
                                        var v = Convert.ToDouble(boq_lid_details.quantity);
                                    }
                                    catch (Exception)
                                    {
                                        hasIssue = 1;
                                        ErrorMeessage += (string)errorMappings["WRONG_QUANTITY_VALUE"];
                                        //throw;
                                    }
                                }
                                if (boq_lid_details.item_sku == null)
                                {
                                    ErrorMeessage += (string)errorMappings["SKU_IS_EMPTY"];
                                    hasIssue = 1;
                                }
                            }

                            if (hasIssue == 1)
                            {
                                _listErrorLog.sku_label = boq_lid_details.item_sku;
                                _listErrorLog.quantity = boq_lid_details.quantity;
                                _listErrorLog.uom = boq_lid_details.uom;
                                _listErrorLog.error_log = ErrorMeessage;

                                _listgetErrorLog.Add(_listErrorLog);
                            }
                            m_create_boq_details.Add(boq_lid_details);
                            rowNum++;
                            ErrorMeessage += "";
                        }
                        m_create_boq_details[0].error_log = _listgetErrorLog;
                    }
                }
                xlApp.Quit();
                ///MessageBox.Show(ErrorMeessage);
                return m_create_boq_details;
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
                throw;
            }
        }

        private static void SetValueByIndex(string value, int index, BoqLineItemDetails m_create_boq_details, List<string> columnNames, string _project_id, string _version_number)
        {
            try
            {
                index = index - 1;

                //m_create_boq_details+"."
                if (columnNames[index].ToLower().Equals("item name"))
                {
                    m_create_boq_details.item_name = value;
                }
                else if (columnNames[index].ToLower().Equals("item sku"))
                {
                    if (m_create_boq_details.item_sku != null && m_create_boq_details.item_sku.Length != 16)
                    {
                        ErrorMeessage += "SKU data Incorrect";
                    }

                    m_create_boq_details.item_sku = value;
                }
                else if (columnNames[index].ToLower().Equals("quantity"))
                {
                    m_create_boq_details.quantity = value;
                }
                else if (columnNames[index].ToLower().Equals("uom"))
                {
                    m_create_boq_details.uom = value;
                }
                else if (columnNames[index].ToLower().Equals("floor name"))
                {
                    m_create_boq_details.level_name = value;
                }
                else if (columnNames[index].ToLower().Equals("family category")) // NOT Found
                {
                    m_create_boq_details.item_category = value;
                }
                m_create_boq_details.project_id = _project_id;
                m_create_boq_details.version_number = _version_number;
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
                //throw;
            }
        }
    }
}