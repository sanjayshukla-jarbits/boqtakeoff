using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Document = Autodesk.Revit.DB.Document;
using RevParam = Autodesk.Revit.DB.Parameter;

namespace boqtakeoff.core
{
    public class MaterialQuantitiesMain
    {
        public static List<BoqLineItemDetails> execMaterialQuantities(Document document, List<string> selectedValues, string version_number, string uom)
        {
            m_doc = document;
            m_version_number = version_number;
            // INFO:: Now, depending upon the categories selected we can call different interfaces
            
            if (selectedValues.Contains("doors"))
            {
                ExecuteCalculationsForCountables<DoorsQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Doors Take off done");
            }
            
            if (selectedValues.Contains("speciality_equipment"))
            {
                ExecuteCalculationsForCountables<SpecialityEquipmentQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Speciality Equipment Take off done");
            }
            
            if (selectedValues.Contains("furniture"))
            {
                ExecuteCalculationsForCountables<FurnitureQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Furniture Take off done");
            }
            
            if (selectedValues.Contains("plumbing_fixtures"))
            {
                ExecuteCalculationsForCountables<PlumbingFixturesQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Plumbing Fixtures Take off done");
            }
            
            if (selectedValues.Contains("windows"))
            {
                ExecuteCalculationsForCountables<WindowsQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Plumbing Fixtures Take off done");
            }
            
            if (selectedValues.Contains("casework"))
            {
                ExecuteCalculationsForCountables<CaseWorkQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Case Work Take off done");
            }
            
            if (selectedValues.Contains("structural_columns"))
            {
                ExecuteCalculationsForCountables<StructuralColumnsQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Structural Columns Take off done");
            }
            
            if (selectedValues.Contains("structural_framing"))
            {
                ExecuteCalculationsForCountables<StructuralFramingsQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Structural Framings Take off done");
            }
            
            if (selectedValues.Contains("generic_models"))
            {
                ExecuteCalculationsForCountables<GenericModelsQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Generic Models Take off done");
            }
            
            if (selectedValues.Contains("generic_annotations"))
            {
                ExecuteCalculationsForCountables<GenericAnnotationQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Generic Annotations Take off done");
            }
            
            if (selectedValues.Contains("roofs"))
            {
                ExecuteCalculationsWith<RoofMaterialQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Roof Take off done");
            }
            
            if (selectedValues.Contains("walls"))
            {
                ExecuteCalculationsWith<WallMaterialQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Wall Take off done");
            }
            
            if (selectedValues.Contains("floors"))
            {
                ExecuteCalculationsWith<FloorMaterialQuantityCalculator>();
                // TaskDialog.Show("Material Quantity ... ", "Floor Take off done");
            }
            
            // ExecuteCalculationsWith<MechanicalEquipmentQuantityCalculator>();
            // TaskDialog.Show("Mechanical Equipment ... ", "Mechanical Equipment Take off done");
            return m_create_boq_details;
        }

        /// <summary>
        /// Executes a calculator for one type of Revit element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static void ExecuteCalculationsWith<T>() where T : MaterialQuantityCalculator, new()
        {

            T calculator = new T();
            calculator.SetDocument(m_doc);
            calculator.CalculateMaterialQuantities();
            // calculator.ReportResults(m_writer);
            calculator.ExportTotalQuantities(m_create_boq_details, m_version_number);
            // return create_boq_details;

        }

        private static void ExecuteCalculationsForCountables<T>() where T : MaterialQuantityCalculator, new()
        {

            T calculator = new T();
            // INFO:: If we need access to Revit App, we can call calculator.SetDocument
            calculator.SetDocument(m_doc);
            calculator.EstimateCountableItemsDetails(m_create_boq_details, m_version_number);
        }

        #region Basic Command Data
        public static Document m_doc;

        public static TextWriter m_writer;

        public static string m_version_number;
        public static string m_uom; 

        public static List<BoqLineItemDetails> m_create_boq_details = new List<BoqLineItemDetails>();
        #endregion

    }


    /// <summary>
    /// The wall material quantity calculator specialized class.
    /// </summary>
    class DoorsQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();
            
        }

        protected override string GetElementTypeName()
        {
            return "Doors";
        }
    }

    class SpecialityEquipmentQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_SpecialityEquipment);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();

        }

        protected override string GetElementTypeName()
        {
            return "SpecialityEquipment";
        }
    }

    class FurnitureQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_Furniture);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();

        }

        protected override string GetElementTypeName()
        {
            return "Furniture";
        }
    }

    class PlumbingFixturesQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_PlumbingFixtures);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();

        }

        protected override string GetElementTypeName()
        {
            return "PlumbingFixtures";
        }
    }

    class WindowsQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();

        }

        protected override string GetElementTypeName()
        {
            return "Windows";
        }
    }

    class CaseWorkQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_Casework);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();

        }

        protected override string GetElementTypeName()
        {
            return "CaseWork";
        }
    }

    class StructuralColumnsQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();

        }

        protected override string GetElementTypeName()
        {
            return "StructuralColumns";
        }
    }


    class StructuralFramingsQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();

        }

        protected override string GetElementTypeName()
        {
            return "StructuralFraming";
        }
    }

    class GenericModelsQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_GenericModel);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();

        }

        protected override string GetElementTypeName()
        {
            return "GenericModels";
        }
    }

    class GenericAnnotationQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_GenericAnnotation);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "GenericAnnotation";
        }
    }

    class MechanicalEquipmentQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            m_elementsToProcess = (from elem in new FilteredElementCollector(m_doc).OfClass(typeof(FamilyInstance))
                                   let type = elem as FamilyInstance
                                   where type.MEPModel is MechanicalEquipment
                                   select elem).ToList();
        }

        protected override string GetElementTypeName()
        {
            return "MechanicalEquipment";
        }
    }

    /// <summary>
    /// The wall material quantity calculator specialized class.
    /// </summary>
    class WallMaterialQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            // filter for non-symbols that match the desired category so that inplace elements will also be found
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Wall";
        }
    }

    /// <summary>
    /// The floor material quantity calculator specialized class.
    /// </summary>
    class FloorMaterialQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Floor";
        }
    }

    /// <summary>
    /// The roof material quantity calculator specialized class.
    /// </summary>
    class RoofMaterialQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            ElementCategoryFilter filterCategory = new ElementCategoryFilter(BuiltInCategory.OST_Roofs);
            ElementClassFilter filterNotSymbol = new ElementClassFilter(typeof(FamilySymbol), true);
            LogicalAndFilter filter = new LogicalAndFilter(filterCategory, filterNotSymbol);
            m_elementsToProcess = collector.WherePasses(filter).ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Roof";
        }
    }

    /// <summary>
    /// The base material quantity calculator for all element types.
    /// </summary>
    abstract class MaterialQuantityCalculator
    {
        /// <summary>
        /// The list of elements for material quantity extraction.
        /// </summary>
        protected IList<Element> m_elementsToProcess;

        /// <summary>
        /// Override this to populate the list of elements for material quantity extraction.
        /// </summary>
        protected abstract void CollectElements();

        /// <summary>
        /// Override this to return the name of the element type calculated by this calculator.
        /// </summary>
        protected abstract String GetElementTypeName();

        /// <summary>
        /// Sets the document for the calculator class.
        /// </summary>
        public void SetDocument(Document d)
        {
            m_doc = d;
            Autodesk.Revit.ApplicationServices.Application app = d.Application;
        }

        public void EstimateCountableItemsDetails(List<BoqLineItemDetails> m_create_boq_details, string m_version_number)
        {
            CollectElements();
            // Check if we are in the Revit project, not in family one.
            if (m_doc.IsFamilyDocument)
            {
                // Message.Display("Can't use command in family document", WindowType.Warning);                
                return;
            }
            
            foreach (Element curr_elem in m_elementsToProcess)
            {                
                BoqLineItemDetails boq_lid_details = new BoqLineItemDetails();
                Category category = curr_elem.Category;
                boq_lid_details.item_category = category.Name.ToString();
                boq_lid_details.item_name = curr_elem.Name;

                try
                {
                    // INFO:: Adding Level Details
                    string _level_element_id = curr_elem.LevelId.ToString();
                    ElementId _ElementId = Autodesk.Revit.DB.ElementId.Parse(_level_element_id);
                    Element level_element = m_doc.GetElement(_ElementId);
                    Level level = level_element as Level;
                    string level_name = level.Name.ToString();
                    boq_lid_details.level_name = level_name;
                    boq_lid_details.level_id = _level_element_id;

                    // INFO:: Adding the SKU Details
                    ElementId id_type = curr_elem.GetTypeId();
                    Element element_type = m_doc.GetElement(id_type);

                    Hashtable sharedParamDetailsHashFloorId = SharedParameter.getSharedParamGUID(m_doc, "SKU");
                    string guidStringFI = sharedParamDetailsHashFloorId["guid"].ToString();
                    RevParam parameter = element_type.get_Parameter(new Guid(guidStringFI));

                    // If the SKU is not store in the type parameters, we take the element name as the SKU
                    if (parameter != null)
                    {
                        string sharedParametervalue = parameter.AsString();
                        if (string.IsNullOrWhiteSpace(sharedParametervalue) || sharedParametervalue == "NA")
                        {
                            sharedParametervalue = curr_elem.Name;
                        }
                        boq_lid_details.item_sku = sharedParametervalue;
                    }

                    // INFO:: Adding the room details
                    FamilyInstance fi = curr_elem as FamilyInstance;

                    if (fi != null && fi.Room != null)
                    {
                        string room_name_str = fi.Room.Name;
                        string room_id_str = fi.Room.Id.ToString();
                        string room_number_str = fi.Room.Number.ToString();
                        boq_lid_details.delivery_area = room_name_str;
                        boq_lid_details.room_number = room_number_str;
                    }
                }
                catch (Exception error)
                {
                    // TaskDialog.Show("Error ... ", error.Message.ToString());
                }
                boq_lid_details.version_number = m_version_number;
                m_create_boq_details.Add(boq_lid_details);                
            }            
            return;
        }

        /// <summary>
        /// Executes the calculation.
        /// </summary>
        public void CalculateMaterialQuantities()
        {
            CollectElements();
            CalculateNetMaterialQuantities();
            CalculateGrossMaterialQuantities();
        }

        /// <summary>
        /// Calculates net material quantities for the target elements.
        /// </summary>
        private void CalculateNetMaterialQuantities()
        {
            foreach (Element e in m_elementsToProcess)
            {
                CalculateMaterialQuantitiesOfElement(e);
            }
        }

        /// <summary>
        /// Calculates gross material quantities for the target elements (material quantities with 
        /// all openings, doors and windows removed). 
        /// </summary>
        private void CalculateGrossMaterialQuantities()
        {
            m_calculatingGrossQuantities = true;
            Transaction t = new Transaction(m_doc);
            t.SetName("Delete Cutting Elements");
            t.Start();
            DeleteAllCuttingElements();
            m_doc.Regenerate();
            try
            {
                foreach (Element e in m_elementsToProcess)
                {
                    CalculateMaterialQuantitiesOfElement(e);
                }
            }
            catch (Exception error)
            {
                // TaskDialog.Show("CalculateGrossMaterialQuantities", error.Message.ToString());
            }
            t.RollBack();
        }

        /// <summary>
        /// Delete all elements that cut out of target elements, to allow for calculation of gross material quantities.
        /// </summary>
		private void DeleteAllCuttingElements()
        {
            IList<ElementFilter> filterList = new List<ElementFilter>();
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);

            // (Type == FamilyInstance && (Category == Door || Category == Window) || Type == Opening
            ElementClassFilter filterFamilyInstance = new ElementClassFilter(typeof(FamilyInstance));
            ElementCategoryFilter filterWindowCategory = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            ElementCategoryFilter filterDoorCategory = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            LogicalOrFilter filterDoorOrWindowCategory = new LogicalOrFilter(filterWindowCategory, filterDoorCategory);
            LogicalAndFilter filterDoorWindowInstance = new LogicalAndFilter(filterDoorOrWindowCategory, filterFamilyInstance);

            ElementClassFilter filterOpening = new ElementClassFilter(typeof(Opening));

            LogicalOrFilter filterCuttingElements = new LogicalOrFilter(filterOpening, filterDoorWindowInstance);
            ICollection<Element> cuttingElementsList = collector.WherePasses(filterCuttingElements).ToElements();

            foreach (Element e in cuttingElementsList)
            {
                // Doors in curtain grid systems cannot be deleted.  This doesn't actually affect the calculations because
                // material quantities are not extracted for curtain systems.
                try
                {
                    if (e.Category != null)
                    {
                        if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Doors)
                        {
                            FamilyInstance door = e as FamilyInstance;
                            Wall host = door.Host as Wall;

                            if (host.CurtainGrid != null)
                                continue;
                        }
                        ICollection<ElementId> deletedElements = m_doc.Delete(e.Id);

                        // Log failed deletion attempts to the output.  (These may be other situations where deletion is not possible but 
                        // the failure doesn't really affect the results.
                        if (deletedElements == null || deletedElements.Count < 1)
                        {
                            m_warningsForGrossQuantityCalculations.Add(
                                     String.Format("   The tool was unable to delete the {0} named {2} (id {1})", e.GetType().Name, e.Id, e.Name));
                        }
                    }
                }
                catch (Exception error)
                {
                    // TaskDialog.Show("DeleteAllCuttingElements", error.Message.ToString());
                }

            }
        }

        /// <summary>
        /// Store calculated material quantities in the storage collection.
        /// </summary>
        /// <param name="materialId">The material id.</param>
        /// <param name="volume">The extracted volume.</param>
        /// <param name="area">The extracted area.</param>
        /// <param name="quantities">The storage collection.</param>
	    private void StoreMaterialQuantities(ElementId materialId, double volume, double area,
                                            Dictionary<ElementId, MaterialQuantities> quantities)
        {
            MaterialQuantities materialQuantityPerElement;
            bool found = quantities.TryGetValue(materialId, out materialQuantityPerElement);
            if (found)
            {
                if (m_calculatingGrossQuantities)
                {
                    materialQuantityPerElement.GrossVolume += volume;
                    materialQuantityPerElement.GrossArea += area;
                }
                else
                {
                    materialQuantityPerElement.NetVolume += volume;
                    materialQuantityPerElement.NetArea += area;
                }
            }
            else
            {
                materialQuantityPerElement = new MaterialQuantities();
                if (m_calculatingGrossQuantities)
                {
                    materialQuantityPerElement.GrossVolume = volume;
                    materialQuantityPerElement.GrossArea = area;
                }
                else
                {
                    materialQuantityPerElement.NetVolume = volume;
                    materialQuantityPerElement.NetArea = area;
                }
                quantities.Add(materialId, materialQuantityPerElement);
            }
        }

        /// <summary>
        /// Calculate and store material quantities for a given element.
        /// </summary>
        /// <param name="e">The element.</param>
	    private void CalculateMaterialQuantitiesOfElement(Element e)
        {
            ElementId elementId = e.Id;
            // MaterialSet materials = e.Materials;
            ICollection<ElementId> materialIdCollection = e.GetMaterialIds(false);

            // foreach (Material material in subelementList)
            // {
            // ElementId materialId = material.Id;
            foreach (ElementId materialId in materialIdCollection)
            {
                // ElementId materialId = material.Id;
                double volume = e.GetMaterialVolume(materialId);
                double area = e.GetMaterialArea(materialId, false);

                if (volume > 0.0 || area > 0.0)
                {
                    StoreMaterialQuantities(materialId, volume, area, m_totalQuantities);

                    Dictionary<ElementId, MaterialQuantities> quantityPerElement;
                    bool found = m_quantitiesPerElement.TryGetValue(elementId, out quantityPerElement);
                    if (found)
                    {
                        StoreMaterialQuantities(materialId, volume, area, quantityPerElement);
                    }
                    else
                    {
                        quantityPerElement = new Dictionary<ElementId, MaterialQuantities>();
                        StoreMaterialQuantities(materialId, volume, area, quantityPerElement);
                        m_quantitiesPerElement.Add(elementId, quantityPerElement);
                    }
                }
            }
        }
        /*
        public void ExportTotalQuantities()
        {
            if (m_totalQuantities.Count == 0)
                return;

            ExportMaterialQty(m_totalQuantities);
            // return m_create_boq_details;
        }
        */
        /// <summary>
        /// Write the contents of one storage collection to the indicated output writer.
        /// </summary>
        /// <param name="quantities">The storage collection for material quantities.</param>
        /// <param name="writer">The output writer.</param>
        public void ExportTotalQuantities(List<BoqLineItemDetails> m_create_boq_details, string m_version_number)
        {
            if (m_totalQuantities.Count == 0)
                return;

            foreach (ElementId keyMaterialId in m_totalQuantities.Keys)
            {
                BoqLineItemDetails boq_lid_details = new BoqLineItemDetails();

                ElementId materialId = keyMaterialId;
                MaterialQuantities quantity = m_totalQuantities[materialId];

                //Material material = m_doc.get_Element(materialId) as Material;
                Material material = m_doc.GetElement(materialId) as Material;
                boq_lid_details.quantity = quantity.NetArea.ToString();
                boq_lid_details.item_sku = material.Name;
                boq_lid_details.item_name = material.Name;
                boq_lid_details.version_number = m_version_number;                
                /*
                 * We can get the SKU parameter from the custom parameter in the material element
                String material_custom_param_val = material.LookupParameter("Material SKU").AsString();
                if (material_custom_param_val == "CSIWWH00720038SI")
                {
                    TaskDialog.Show("Material SKU", material_custom_param_val);
                } 
                */
                m_create_boq_details.Add(boq_lid_details);
            }

        }

        /// <summary>
        /// Write results in CSV format to the indicated output writer.
        /// </summary>
        /// <param name="writer">The output text writer.</param>
	    public void ReportResults(TextWriter writer)
        {
            if (m_totalQuantities.Count == 0)
                return;

            String legendLine = "Gross volume(cubic ft),Net volume(cubic ft),Gross area(sq ft),Net area(sq ft)";

            writer.WriteLine();
            writer.WriteLine(String.Format("Totals for {0} elements,{1}", GetElementTypeName(), legendLine));

            // If unexpected deletion failures occurred, log the warnings to the output.
            if (m_warningsForGrossQuantityCalculations.Count > 0)
            {
                writer.WriteLine("WARNING: Calculations for gross volume and area may not be completely accurate due to the following warnings: ");
                foreach (String s in m_warningsForGrossQuantityCalculations)
                    writer.WriteLine(s);
                writer.WriteLine();
            }
            ReportResultsFor(m_totalQuantities, writer);

            foreach (ElementId keyId in m_quantitiesPerElement.Keys)
            {
                ElementId id = keyId;
                //Element e = m_doc.get_Element(id);
                Element e = m_doc.GetElement(id);

                writer.WriteLine();
                writer.WriteLine(String.Format("Totals for {0} element {1} (id {2}),{3}",
                    GetElementTypeName(),
                    e.Name.Replace(',', ':'), // Element names may have ',' in them
                    id.IntegerValue, legendLine));

                Dictionary<ElementId, MaterialQuantities> quantities = m_quantitiesPerElement[id];

                ReportResultsFor(quantities, writer);
            }
        }

        /// <summary>
        /// Write the contents of one storage collection to the indicated output writer.
        /// </summary>
        /// <param name="quantities">The storage collection for material quantities.</param>
        /// <param name="writer">The output writer.</param>
        private void ReportResultsFor(Dictionary<ElementId, MaterialQuantities> quantities, TextWriter writer)
        {
            foreach (ElementId keyMaterialId in quantities.Keys)
            {
                ElementId materialId = keyMaterialId;
                MaterialQuantities quantity = quantities[materialId];

                //Material material = m_doc.get_Element(materialId) as Material;
                Material material = m_doc.GetElement(materialId) as Material;

                //writer.WriteLine(String.Format("   {0} Net: [{1:F2} cubic ft {2:F2} sq. ft]  Gross: [{3:F2} cubic ft {4:F2} sq. ft]", material.Name, quantity.NetVolume, quantity.NetArea, quantity.GrossVolume, quantity.GrossArea));
                writer.WriteLine(String.Format("{0},{3:F2},{1:F2},{4:F2},{2:F2}",
                    material.Name.Replace(',', ':'),  // Element names may have ',' in them
                    quantity.NetVolume, quantity.NetArea, quantity.GrossVolume, quantity.GrossArea));

                /*
                 * We can get the SKU parameter from the custom parameter in the material element
                String material_custom_param_val = material.LookupParameter("Material SKU").AsString();
                if (material_custom_param_val == "CSIWWH00720038SI")
                {
                    TaskDialog.Show("Material SKU", material_custom_param_val);
                } 
                */
            }
        }

        #region Results Storage
        /// <summary>
        /// A storage of material quantities per individual element.
        /// </summary>
        private Dictionary<ElementId, Dictionary<ElementId, MaterialQuantities>> m_quantitiesPerElement = new Dictionary<ElementId, Dictionary<ElementId, MaterialQuantities>>();

        /// <summary>
        /// A storage of material quantities for the entire project.
        /// </summary>
        private Dictionary<ElementId, MaterialQuantities> m_totalQuantities = new Dictionary<ElementId, MaterialQuantities>();



        /// <summary>
        /// Flag indicating the mode of the calculation.
        /// </summary>
        private bool m_calculatingGrossQuantities = false;

        /// <summary>
        /// A collection of warnings generated due to failure to delete elements in advance of gross quantity calculations.
        /// </summary>
	    private List<String> m_warningsForGrossQuantityCalculations = new List<string>();
        #endregion

        protected Document m_doc;
    }

    /// <summary>
    /// A storage class for the extracted material quantities.
    /// </summary>
	class MaterialQuantities
    {
        /// <summary>
        /// Gross volume (cubic ft)
        /// </summary>
		public double GrossVolume { get; set; }

        /// <summary>
        /// Gross area (sq. ft)
        /// </summary>
		public double GrossArea { get; set; }

        /// <summary>
        /// Net volume (cubic ft)
        /// </summary>
		public double NetVolume { get; set; }

        /// <summary>
        /// Net area (sq. ft)
        /// </summary>
		public double NetArea { get; set; }

    }
}