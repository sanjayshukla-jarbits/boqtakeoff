using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ClassLibrary1
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        private readonly string myAddinDLL = "boqtakeoff";

        public override void Uninstall(System.Collections.IDictionary stateSaver)
        {
            string sDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Autodesk\\Revit\\Addins";
            bool exists = System.IO.Directory.Exists(sDir);

            //2 August 2019: Start, The next 3 lines were added in Take 10 in order prevent double loading of packages.
            // Microsoft.Win32.RegistryKey rkbase = null;
            // rkbase = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
            // rkbase.DeleteSubKeyTree("SOFTWARE\\Wow6432Node\\Default Company Name\\Revit API NuGet Example 2019 Packages");
            //2 August 2019: End.

            if (exists)
            {
                try
                {
                    foreach (string d in Directory.GetDirectories(sDir))
                    {
                        //DirSearch.Add(d);
                        File.Delete(d + "\\" + myAddinDLL + ".addin");
                    }
                }
                catch (System.Exception excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {

            //2 August 2019: The next 4 lines were added in Take 10 in order prevent double loading of packages.
            // Microsoft.Win32.RegistryKey rkbase = null;
            // rkbase = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
            // rkbase.CreateSubKey("SOFTWARE\\Wow6432Node\\Default Company Name\\Revit API NuGet Example 2019 Packages", Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree).SetValue("XceedVersion", typeof(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid).Assembly.FullName);
            // rkbase.CreateSubKey("SOFTWARE\\Wow6432Node\\Default Company Name\\Revit API NuGet Example 2019 Packages", Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree).SetValue("OokiiVersion", typeof(Ookii.Dialogs.Wpf.CredentialDialog).Assembly.FullName);
            //2 August 2019: End.


            string sDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Autodesk\\Revit\\Addins";
            bool exists = System.IO.Directory.Exists(sDir);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory(sDir);
            }

            XElement XElementAddIn = new XElement("AddIn", new XAttribute("Type", "Application"));

            XElementAddIn.Add(new XElement("Name", myAddinDLL));
            XElementAddIn.Add(new XElement("Assembly", Context.Parameters["targetdir"].Trim() + myAddinDLL + ".dll"));  // /TargetDir=value1 /
            XElementAddIn.Add(new XElement("AddInId", Guid.NewGuid().ToString())); //DatabaseMethods.writeDebug(Guid.NewGuid().ToString());
            XElementAddIn.Add(new XElement("FullClassName", myAddinDLL + ".Main"));
            XElementAddIn.Add(new XElement("VendorId", "Sanjay Shukla"));
            XElementAddIn.Add(new XElement("VendorDescription", "The plugin is meant to create the Cost BOQ as per SMI specifications, sanjay.shukla@spacematrix.com"));

            XElement XElementRevitAddIns = new XElement("RevitAddIns");
            XElementRevitAddIns.Add(XElementAddIn);

            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    //DirSearch.Add(d);
                    new XDocument(XElementRevitAddIns).Save(d + "\\" + myAddinDLL + ".addin");
                    //files.AddRange(DirSearch.Add(d));
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

    }
}
