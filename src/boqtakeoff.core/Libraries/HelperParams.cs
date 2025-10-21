// URL: https://jeremytammik.github.io/tbc/a/0754_shared_param_add_categ.htm

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using AppRvt = Autodesk.Revit.ApplicationServices.Application; // to avoid ambiguities
using BindingRvt = Autodesk.Revit.DB.Binding; // to avoid ambiguities
using DocRvt = Autodesk.Revit.DB.Document; // to avoid ambiguities

namespace boqtakeoff.core
{

    /// <summary>
    /// Major Helper Class for Revit Params
    /// </summary>
    public static class HelperParams
    {

        const string _groupname = "SMI Parameters";
        // const string _defname = "SMI_";
        // static ParameterType _deftype = ParameterType.Text;
        public static readonly ForgeTypeId _deftype = SpecTypeId.String.Text; // 2022
        public static StringBuilder CreateSharedParameter = new StringBuilder();

        /// <summary>
        /// Return GUID for a given shared parameter group and name.
        /// </summary>
        /// <param name="app">Revit application</param>
        /// <param name="defGroup">Definition group name</param>
        /// <param name="defName">Definition name</param>
        /// <returns>GUID</returns>
        public static Guid SharedParamGuid(
        AppRvt app,
        string defGroup,
        string defName)
        {
            DefinitionFile file = app.OpenSharedParameterFile();
            DefinitionGroup group = (null == file)
              ? null : file.Groups.get_Item(defGroup);
            Definition definition = (null == group)
              ? null : group.Definitions.get_Item(defName);
            ExternalDefinition externalDefinition
              = (null == definition)
                ? null : definition as ExternalDefinition;
            return (null == externalDefinition)
              ? Guid.Empty
              : externalDefinition.GUID;
        }

        public static string GetParameterValue(Parameter param)
        {
            string s;
            switch (param.StorageType)
            {
                case StorageType.Integer:
                    s = param.AsInteger().ToString();
                    break;

                case StorageType.String:
                    s = param.AsString();
                    break;

                case StorageType.ElementId:
                    s = param.AsElementId().IntegerValue.ToString();
                    break;

                case StorageType.None:
                    s = "?NONE?";
                    break;

                default:
                    s = "?ELSE?";
                    break;
            }
            return s;
        }

        public static bool GetSharedParamGuid(
          AppRvt app,
          string sharedParamName,
          out Guid paramGuid)
        {
            paramGuid = SharedParamGuid(app,
              _groupname,
              sharedParamName);

            return !paramGuid.Equals(Guid.Empty);
        }

        public enum BindSharedParamResult
        {
            eAlreadyBound,
            eSuccessfullyBound,
            eWrongParamType,
            //eWrongCategory, //not exposed
            //eWrongVisibility, //not exposed
            eWrongBindingType,
            eFailed
        }

        /// <summary>
        /// Get Element Parameter *by name*. By defualt NOT case sensitive. Use overloaded one if case sensitive needed.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Parameter GetElemParam(Element elem, string name)
        {
            return GetElemParam(elem, name, false);
        }
        public static Parameter GetElemParam(Element elem, string name, bool matchCase)
        {
            StringComparison comp = StringComparison.CurrentCultureIgnoreCase;
            if (matchCase) comp = StringComparison.CurrentCulture;

            foreach (Parameter p in elem.Parameters)
            {
                if (p.Definition.Name.Equals(name, comp)) return p;
            }
            // if here, not found
            return null;
        }

        public static Guid SharedParamGUID(AppRvt app, string defGroup, string defName)
        {
            Guid guid = Guid.Empty;
            try
            {
                DefinitionFile file = app.OpenSharedParameterFile();
                DefinitionGroup group = file.Groups.get_Item(defGroup);
                Definition definition = group.Definitions.get_Item(defName);
                ExternalDefinition externalDefinition = definition as ExternalDefinition;
                guid = externalDefinition.GUID;
            }
            catch (Exception)
            {
            }
            return guid;
        }

        /// <summary>
        /// Get or Create Shared Params File
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static DefinitionFile GetOrCreateSharedParamsFile(AppRvt app)
        {
            string fileName = string.Empty;
            try // generic
            {
                // Get file
                fileName = app.SharedParametersFilename;
                CreateSharedParameter.Append("\nfileName : " + fileName.ToString());
                // Create file if not set yet (ie after Revit installed and no Shared params used so far)
                if (string.Empty == fileName || !File.Exists(fileName))
                {
                    string modulePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    fileName = modulePath + "\\SharedParameters.txt";
                    StreamWriter stream = new StreamWriter(fileName);
                    stream.Close();
                    app.SharedParametersFilename = fileName;
                }
                return app.OpenSharedParameterFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: Failed to get or create Shared Params File: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get or Create Shared Parameters Group
        /// </summary>
        /// <param name="defFile"></param>
        /// <param name="_groupname"></param>
        /// <returns></returns>
        public static DefinitionGroup GetOrCreateSharedParamsGroup(DefinitionFile defFile)
        {
            try // generic
            {
                DefinitionGroup defGrp = defFile.Groups.get_Item(_groupname);
                if (null == defGrp) defGrp = defFile.Groups.Create(_groupname);
                CreateSharedParameter.Append("\ndefGrp: " + defGrp.ToString());
                return defGrp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("ERROR: Failed to get or create Shared Params Group: {0}", ex.Message));
                return null;
            }
        }

        /// <summary> 
        /// Get or Create Shared Parameter Definition
        /// </summary>
        /// <param name="defGrp"></param>
        /// <param name="parType">used only if creating</param>
        /// <param name="parName">used only if creating</param>
        /// <param name="visible">used only if creating</param>
        /// <returns></returns>
        public static Definition GetOrCreateSharedParamDefinition(DefinitionGroup defGrp, Category cat, String paramName)
        {
            try // generic
            {
                CreateSharedParameter.Append("\n defGrp: " + defGrp.Name.ToString());
                CreateSharedParameter.Append("\n ABC: " + "ABC");
                Definition def = defGrp.Definitions.get_Item(paramName.ToString());
                CreateSharedParameter.Append("\n XYZ: " + "XYZ");
                //if (null == def) def = defGrp.Definitions.Create(parName, parType, visible);
                //return def;
                CreateSharedParameter.Append("\n XYZ: " + "XYZ");
                CreateSharedParameter.Append("\n def legacy: " + def);

                if (null == def)
                {
                    //definition = group.Definitions.Create( defname, _deftype, visible ); // 2014

                    bool visible = cat.AllowsBoundParameters;

                    CreateSharedParameter.Append("\n visible: " + visible.ToString());

                    ExternalDefinitionCreationOptions opt
                      = new ExternalDefinitionCreationOptions(paramName.ToString(), _deftype);
                    opt.Visible = visible;
                    def = defGrp.Definitions.Create(opt); // 2015

                    CreateSharedParameter.Append("\n opt : " + opt);
                    CreateSharedParameter.Append("\n def Create : " + def);

                }
                return def;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("ERROR: Failed to get or create Shared Params Definition: {0}", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// Gets or Creates Element's shared param.
        /// </summary>
        /// <param name="elem">Revit Element to get param for</param>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="_groupname">Param Group Name (relevant only when Creation takes place)</param>
        /// <param name="paramType">Param Type (relevant only when Creation takes place)</param>
        /// <param name="visible">Param UI Visibility (relevant only when Creation takes place)</param>
        /// <param name="instanceBinding">Param Binding: Instance or Type (relevant only when Creation takes place)</param>
        /// <returns></returns>
        public static Parameter GetOrCreateElemSharedParam(Element elem, string paramName, bool instanceBinding)
        {
            try
            {
                // Check if existing
                Parameter param = GetElemParam(elem, paramName);
                if (null != param) return param;

                // If here, need to create it...
                BindSharedParamResult res = BindSharedParam(elem.Document, elem.Category, paramName, instanceBinding);
                CreateSharedParameter.Append("\nres : " + res.ToString());

                if (res == BindSharedParamResult.eAlreadyBound)
                {
                    TaskDialog.Show("Binding Failure Message ... ", res.ToString());
                }

                if (res != BindSharedParamResult.eSuccessfullyBound && res != BindSharedParamResult.eAlreadyBound) return null;



                // If here, binding is OK and param seems to be IMMEDIATELY available from the very same command
                return GetElemParam(elem, paramName);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("Error in getting or creating Element Param: {0}", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// Gets or Creates Project Information (per-doc) shared param
        /// </summary>
        /// <param name="doc">Revit Document</param>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="_groupname">Param Group Name (relevant only when Creation takes place)</param>
        /// <param name="paramType">Param Type (relevant only when Creation takes place)</param>
        /// <param name="visible">Param UI Visibility (relevant only when Creation takes place)</param>
        /// <returns></returns>
        public static Parameter GetOrCreateProjInfoSharedParam(DocRvt doc, string paramName)
        {
            // Just delegate the call using ProjectInfo Element
            return GetOrCreateElemSharedParam(doc.ProjectInformation, paramName, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="cat"></param>
        /// <param name="paramName"></param>
        /// <param name="_groupname"></param>
        /// <param name="paramType"></param>
        /// <param name="visible"></param>
        /// <param name="instanceBinding"></param>
        /// <returns></returns>
        /// 
        public static BindSharedParamResult BindSharedParam(DocRvt doc, Category cat, string paramName, bool instanceBinding)
        {
            try // generic
            {
                AppRvt app = doc.Application;

                // This is needed already here to store old ones for re-inserting
                CategorySet catSet = app.Create.NewCategorySet();

                // Loop all Binding Definitions
                // IMPORTANT NOTE: Categories.Size is ALWAYS 1 !? For multiple categories, there is really one pair per each
                //                 category, even though the Definitions are the same...
                DefinitionBindingMapIterator iter = doc.ParameterBindings.ForwardIterator();
                while (iter.MoveNext())
                {
                    Definition def = iter.Key;
                    ElementBinding elemBind = (ElementBinding)iter.Current;

                    // Got param name match
                    if (paramName.Equals(def.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Check for category match - Size is always 1!
                        if (elemBind.Categories.Contains(cat))
                        {
                            // Check Param Type (Check Forge ID Type i.e. if parameter is string, integer or some other type
                            // if (paramType != def.ParameterType) return BindSharedParamResult.eWrongParamType;
                            if (_deftype != def.GetDataType()) return BindSharedParamResult.eWrongParamType;

                            // Check Binding Type
                            if (instanceBinding)
                            {
                                if (elemBind.GetType() != typeof(InstanceBinding)) return BindSharedParamResult.eWrongBindingType;
                            }
                            else
                            {
                                if (elemBind.GetType() != typeof(TypeBinding)) return BindSharedParamResult.eWrongBindingType;
                            }
                            // Check Visibility - cannot (not exposed)
                            // If here, everything is fine, ie already defined correctly
                            return BindSharedParamResult.eAlreadyBound;
                        }
                        // If here, no category match, hence must store "other" cats for re-inserting
                        else
                        {
                            foreach (Category catOld in elemBind.Categories) catSet.Insert(catOld); //1 only, but no index...
                        }
                    }
                }

                // If here, there is no Binding Definition for it, so make sure Param defined and then bind it!
                DefinitionFile defFile = GetOrCreateSharedParamsFile(app);
                DefinitionGroup defGrp = GetOrCreateSharedParamsGroup(defFile);
                CreateSharedParameter.Append("\nparamName : " + paramName.ToString());
                Definition definition = GetOrCreateSharedParamDefinition(defGrp, cat, paramName);
                catSet.Insert(cat);
                BindingRvt bind = null;
                if (instanceBinding)
                {
                    bind = app.Create.NewInstanceBinding(catSet);
                }
                else
                {
                    bind = app.Create.NewTypeBinding(catSet);
                }
                CreateSharedParameter.Append("\nbind : " + bind.ToString());


                // There is another strange API "feature". If param has EVER been bound in a project (in above iter pairs or even if not there but once deleted), .Insert always fails!? Must use .ReInsert in that case.
                // See also similar findings on this topic in: http://thebuildingcoder.typepad.com/blog/2009/09/adding-a-category-to-a-parameter-binding.html - the code-idiom below may be more generic:
                if (doc.ParameterBindings.Insert(definition, bind))
                {
                    CreateSharedParameter.Append("\n Bind Status : " + BindSharedParamResult.eSuccessfullyBound);

                    return BindSharedParamResult.eSuccessfullyBound;
                }
                else
                {
                    if (doc.ParameterBindings.ReInsert(definition, bind))
                    {
                        CreateSharedParameter.Append("\n Bind Status : " + BindSharedParamResult.eSuccessfullyBound);

                        return BindSharedParamResult.eSuccessfullyBound;
                    }
                    else
                    {
                        CreateSharedParameter.Append("\n Bind Status : " + BindSharedParamResult.eFailed);

                        return BindSharedParamResult.eFailed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error in Binding Shared Param: {0}", ex.Message));
                return BindSharedParamResult.eFailed;
            }

            //return BindSharedParamResult.eSuccessfullyBound;
        }

        public static StorageType StorageTypeFromSpecTypeId(ForgeTypeId Spec)
        {
            ForgeTypeId[] ElementIDSpecs = new ForgeTypeId[]
            {
                    SpecTypeId.Reference.FillPattern,
                    SpecTypeId.Reference.Image,
                    SpecTypeId.Reference.LoadClassification,
                    SpecTypeId.Reference.Material
            };

            ForgeTypeId[] IntSpecs = new ForgeTypeId[]
            {
                    SpecTypeId.Int.Integer,
                    SpecTypeId.Int.NumberOfPoles,
                    SpecTypeId.Boolean.YesNo
            };

            ForgeTypeId[] StringSpecs = new ForgeTypeId[]
            {
                    SpecTypeId.String.MultilineText,
                    SpecTypeId.String.Text,
                    SpecTypeId.String.Url
            };

            // You can uncomment and use the following line for "AddAndNotSupported" if needed
            // ForgeTypeId[] AddAndNotSupported = new ForgeTypeId[] { SpecTypeId.Custom };

            if (ElementIDSpecs.Contains(Spec))
            {
                return StorageType.ElementId;
            }
            else if (IntSpecs.Contains(Spec))
            {
                return StorageType.Integer;
            }
            else if (StringSpecs.Contains(Spec))
            {
                return StorageType.String;
            }
            else if (Spec.Equals(SpecTypeId.Custom))
            {
                return StorageType.None;
            }
            else
            {
                return StorageType.Double;
            }
        }

    }
}