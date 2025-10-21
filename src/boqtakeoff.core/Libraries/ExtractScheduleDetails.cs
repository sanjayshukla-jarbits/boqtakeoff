using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace boqtakeoff.core
{
    /// <summary>
    /// A class that can export a schedule to HTML.
    /// </summary>
    public class ExtractScheduleDetails
    {
        /// <summary>
        /// A collection of cells which have already been output.  This is needed to deal with
        /// cell merging - each cell should be written only once even as all the cells are iterated in
        /// order.
        /// </summary>
        
        public static readonly IList<String> element_name_list = new ReadOnlyCollection<string>(new List<String> { "Material: Name", "Item", "Family", "Sub-Component" });
        public static readonly IList<String> sku_code_list = new ReadOnlyCollection<string>(new List<String> { "Material Code", "wtwt", "Ceiling Layer Type", "Material: Code", "SKU" });
        public static readonly IList<String> item_code_list = new ReadOnlyCollection<string>(new List<String> { "Master_SKU" });
        public static readonly IList<String> description_list = new ReadOnlyCollection<string>(new List<String> { "Description", "Material: Description", "Type", "Wall Layer Type", "Floor Type", "Ceiling Layer Type", "SM_Material Tag" });
        public static readonly IList<String> quantity_list = new ReadOnlyCollection<string>(new List<String> { "Material: Area", "Area", "SM_Area", "Quantity", "Count", "SM_Length", "Length" });
        public static readonly IList<String> room_id_list = new ReadOnlyCollection<string>(new List<String> { "SM_Room ID" });
        public static readonly IList<String> unit_list = new ReadOnlyCollection<string>(new List<String> { "Unit" });
        public static readonly IList<String> illegal_val_list = new ReadOnlyCollection<string>(new List<String> { "NA" });

        /// <summary>
        /// Writes the body section of the table to the HTML file.
        /// </summary>
        public static List<BoqLineItemDetails> ReadRvtScheduleTables(Document doc)
        {
            // Get body section and write contents
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> collection = collector.OfClass(typeof(ViewSchedule)).ToElements();

            List<BoqLineItemDetails> create_boq_details = new List<BoqLineItemDetails>();

            foreach (Element curr_elem in collection)
            {
                ViewSchedule theSchedule = curr_elem as ViewSchedule;
                var bodySection = theSchedule.GetTableData().GetSectionData(SectionType.Body);
                var headerSection = theSchedule.GetTableData().GetSectionData(SectionType.Header);
                int numberOfRows = bodySection.NumberOfRows;
                int numberOfColumns = bodySection.NumberOfColumns;

                String schedule_header = theSchedule.GetCellText(SectionType.Header, 0, 0);
                // SANINFO:: For the first row, get the headers
                for (int iRow = 1; iRow < numberOfRows; iRow++)
                {
                    BoqLineItemDetails boq_lid_details = new BoqLineItemDetails();
                    for (int iCol = bodySection.FirstColumnNumber; iCol < numberOfColumns; iCol++)
                    {
                        // Write cell text
                        string column_header = theSchedule.GetCellText(SectionType.Body, 0, iCol);
                        string cellText = theSchedule.GetCellText(SectionType.Body, iRow, iCol);
                        if (cellText.Length > 0)
                        {
                            if (element_name_list.Contains(column_header))
                            {
                                boq_lid_details.item_name = cellText;
                            }
                            else if (sku_code_list.Contains(column_header) && !illegal_val_list.Contains(cellText))
                            {
                                boq_lid_details.item_sku = cellText;
                            }
                            else if (item_code_list.Contains(column_header))
                            {
                                // boq_lid_details.item_name = cellText;
                            }
                            else if (description_list.Contains(column_header))
                            {
                                // boq_lid_details.item_name = cellText;
                            }
                            else if (quantity_list.Contains(column_header))
                            {
                                boq_lid_details.quantity = cellText;
                            }
                            else if (room_id_list.Contains(column_header))
                            {
                                boq_lid_details.delivery_area = cellText;
                                // TaskDialog.Show("delivery_area ... ", cellText.ToString());
                            }
                            else if (unit_list.Contains(column_header))
                            {
                                // boq_lid_details.item_name = cellText;
                            }
                            boq_lid_details.item_category = schedule_header;
                        }
                    }
                    // INFO:: For the elements that don't have the item sku, we are copying the item name to the SKU field
                    if (boq_lid_details.item_sku == null && boq_lid_details.item_name != null)
                    {
                        boq_lid_details.item_sku = boq_lid_details.item_name;
                    }
                    if (boq_lid_details.quantity == null || boq_lid_details.item_sku == null)
                    {
                        continue;
                    }
                    create_boq_details.Add(boq_lid_details);
                }
            }
            return create_boq_details;
        }
    }
}