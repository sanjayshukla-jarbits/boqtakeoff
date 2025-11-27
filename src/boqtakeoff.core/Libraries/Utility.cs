using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using WinForms = System.Windows.Forms;
using RevitTaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace boqtakeoff.core.Libraries
{
    public class Utility
    {

        static XmlNode SelectNodeByAttribute(XmlDocument xmlDoc, string nodeName, string attributeName, string attributeValue)
        {
            // Construct the XPath expression
            string xpathExpression = $"//{nodeName}[@{attributeName}='{attributeValue}']";

            // Select the node based on the XPath expression
            return xmlDoc.SelectSingleNode(xpathExpression);
        }

        /// <summary>
        /// for logging issue : 
        /// Example : Utility.Logger(null, errorMessage, "ValidationError"); 
        /// </summary>
        /// <param name="exp">Exception (from catch block)</param>
        /// <param name="validationMessageLog">In case of you want to log any error or message</param>
        /// <param name="LogType"> error or ValidationError</param>
        public static void Logger(Exception exp = null, string validationMessageLog = "", string LogType = "error")
        {
            string userRoot = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\" + Properties.Resources.LoggerFolderPath;

            //string fileName = "Logfile" + DateTime.Now.Date.ToString("ddMMMyyyy") + ".xml";
            string logFolderPath = System.IO.Path.Combine(userRoot, "BOQLogger");


            // Create a directory if it doesn't exist
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            // Create a filename based on the current date
            string logFileName = $"ErrorLog_{DateTime.Now.ToString("yyyy-MM-dd")}.xml";
            string logFilePath = Path.Combine(logFolderPath, logFileName);

            string nodeIdentifier = string.Empty;
            // Create or load the XML file
            XmlDocument xmlDoc = new XmlDocument();

            if (LogType == "error")
            {
                nodeIdentifier = "application_error";
            }
            else
            {
                nodeIdentifier = "application_validation_error";
            }
            if (File.Exists(logFilePath))
            {
                xmlDoc.Load(logFilePath);
            }
            else
            {
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlNode root = xmlDoc.CreateElement("mappings");

                xmlDoc.AppendChild(root);
                xmlDoc.InsertBefore(xmlDeclaration, root);
            }

            //MessageBox.Show("LogType :" + LogType + "\r\n nodeIdentifier: " + nodeIdentifier);
            if (LogType == "error")
            {
                // XmlElement child2Element = xmlDoc.SelectSingleNode("/mappings/sections/section[type='application_error']") as XmlElement;

                XmlElement selectedElement = SelectNodeByAttribute(xmlDoc, "section", "type", nodeIdentifier) as XmlElement;

                if (selectedElement != null)
                {
                    //MessageBox.Show("Node error" + nodeIdentifier + " found , it should add new Items");
                    XmlElement itemElement = xmlDoc.CreateElement("item");
                    XmlElement messageElement = xmlDoc.CreateElement("message");
                    XmlElement stackTraceElement = xmlDoc.CreateElement("StackTrace");

                    messageElement.InnerText = exp.Message;
                    stackTraceElement.InnerText = exp.StackTrace;
                    itemElement.SetAttribute("datetime", DateTime.Now.ToString());
                    itemElement.AppendChild(stackTraceElement);
                    itemElement.AppendChild(messageElement);

                    selectedElement.AppendChild(itemElement);

                    xmlDoc.DocumentElement?.AppendChild(selectedElement);

                }
                else
                {
                    //MessageBox.Show("Node error" + nodeIdentifier + "Not found , it should Create new section");
                    //XmlElement sectionsElement = xmlDoc.CreateElement("sections");
                    XmlElement sectionElement = xmlDoc.CreateElement("section");

                    XmlElement itemElement = xmlDoc.CreateElement("item");
                    XmlElement messageElement = xmlDoc.CreateElement("message");
                    XmlElement stackTraceElement = xmlDoc.CreateElement("StackTrace");

                    itemElement.SetAttribute("datetime", DateTime.Now.ToString());
                    sectionElement.SetAttribute("type", nodeIdentifier);
                    messageElement.InnerText = exp.Message;
                    stackTraceElement.InnerText = exp.StackTrace;

                    itemElement.AppendChild(stackTraceElement);
                    itemElement.AppendChild(messageElement);

                    sectionElement.AppendChild(itemElement);

                    //stackTraceElement.InnerText = ex.StackTrace;

                    xmlDoc.DocumentElement?.AppendChild(sectionElement);
                }
            }
            else if (LogType == "ValidationError")
            {

                XmlElement selectedElement = SelectNodeByAttribute(xmlDoc, "section", "type", nodeIdentifier) as XmlElement;

                if (selectedElement != null)
                {
                    XmlElement itemElement = xmlDoc.CreateElement("item");
                    XmlElement messageElement = xmlDoc.CreateElement("message");

                    itemElement.SetAttribute("datetime", DateTime.Now.ToString());
                    messageElement.InnerText = validationMessageLog;

                    itemElement.AppendChild(messageElement);
                    selectedElement.AppendChild(itemElement);
                    xmlDoc.DocumentElement?.AppendChild(selectedElement);

                }
                else
                {
                    //XmlElement sectionsElement = xmlDoc.CreateElement("sections");
                    XmlElement sectionElement = xmlDoc.CreateElement("section");
                    XmlElement messageElement = xmlDoc.CreateElement("message");
                    XmlElement itemElement = xmlDoc.CreateElement("item");

                    itemElement.SetAttribute("datetime", DateTime.Now.ToString());
                    sectionElement.SetAttribute("type", nodeIdentifier);
                    messageElement.InnerText = validationMessageLog;

                    itemElement.AppendChild(messageElement);
                    sectionElement.AppendChild(itemElement);

                    xmlDoc.DocumentElement?.AppendChild(sectionElement);
                }
            }
            xmlDoc.Save(logFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="MessageType">information,error</param>
        /// <param name="MessageWindowType">message,task</param>
        /// <param name="Title"></param>
        public static void ShowMessage(string Message, string Title = "title", string MessageType = "information", string MessageWindowType = "task")
        {
            if (MessageWindowType.ToString().ToLower() == "message")
            {
                if (MessageType.ToString().ToLower() == "information")
                {
                    WinForms.MessageBox.Show(Message, Title, WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);

                }
                else if (MessageType.ToString().ToLower() == "error")
                {
                    WinForms.MessageBox.Show(Message, Title, WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
                }

            }
            else if (MessageWindowType.ToString().ToLower() == "task")
            {
                RevitTaskDialog d = new RevitTaskDialog("TaskDialog");
                d.MainContent = Message;
                d.Title = Title;
                //d.FooterText = "<b>Bigfish Tool </b>";
                d.Show();
            }
        }

        public static RevitTaskDialog ShowTaskDialogMessageWithProgressBar(string Message, string Title = "title")
        {
            RevitTaskDialog d = new RevitTaskDialog("TaskDialog");
            d.MainContent = Message;
            d.Title = Title;
            d.FooterText = "Bigfish";
            d.MainInstruction = "Please wait..";
            d.EnableMarqueeProgressBar = true;
            d.Show();

            return d;
        }


        static Hashtable errorMappings = new Hashtable();
        public static Hashtable LoadErrorMappingFromConfiguration()
        {
            try
            {
                // MessageBox.Show("LoadErrorMappingFromConfiguration");
                XmlDocument xDoc = new XmlDocument();

                /* below Setting file must be in C:\Program Files\Autodesk\Revit 2023\Resources\settings.xml*/

                xDoc.Load(@".\\Resources\\settings.xml");

                foreach (XmlNode item in xDoc.SelectNodes("//error"))
                {
                    if (!errorMappings.ContainsKey(item.Attributes["id"].Value))
                        errorMappings.Add(item.Attributes["id"].Value, item.InnerText);
                }
                return errorMappings;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                throw;
            }
        }

        public static string PascalCase(string str)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
            //str = Regex.Replace(str, "([A-Z]+)", " $1");
            str = cultInfo.ToTitleCase(str);
            //str = str.Replace(" ", " ");
            return str;
        }
    }
}
