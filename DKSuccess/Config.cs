using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DKSuccess
{
    class Config
    {
        const string DateFormat = "yyyy-MM-dd";

        const string XmlRootName = "DKSuccessConfiguration";
        const string XmlDbFilename = "DatabaseFilename";
        const string XmlDefaultFromDate = "DefaultFromDate";
        const string XmlDefaultToDate = "DefaultToDate";
        const string XmlDefaultGranularity = "DefaultGranularity";
        const string XmlStoredParameters = "StoredParameters";

        const string XmlParameter = "Parameter";
        const string XmlName = "Name";
        const string XmlValue = "Value";


        string DbFileName;
        DateTime DefaultFromDate;
        DateTime DefaultToDate;
        string DefaultGranularity;
        string ConfigFilename;

        Dictionary<string, string> StoredParameters;

        public Config(string fileName) {
            ConfigFilename = fileName;

            if (!File.Exists(ConfigFilename)) {
                DbFileName = "db.xml";
                DefaultFromDate = new DateTime(1980, 1, 1);
                DefaultToDate = new DateTime(2100, 1, 1);
                DefaultGranularity = "DAY";

                StoredParameters = new Dictionary<string, string>() {
                    {"default","export B5,C5,S4,R5,mins,maxs,avgs DAY" }
                };

                Save();
            } else {
                Load();
            }
        }

        public string GetParameterValue(string paramKey) {
            return (StoredParameters[paramKey]);
        }

        public string GetDefaultGranularity() {
            return DefaultGranularity;
        }

        public DateTime GetDefaultFromDate() {
            return DefaultFromDate;
        }

        public DateTime GetDefaultToDate() {
            return DefaultToDate;
        }

        private void Load() {
            XmlDocument doc = new XmlDocument();
            doc.Load(ConfigFilename);

            DbFileName = GetNodeInnerXml(doc, XmlDbFilename);
            DefaultGranularity = GetNodeInnerXml(doc, XmlDefaultGranularity);
            DefaultFromDate = DateTime.ParseExact(GetNodeInnerXml(doc, XmlDefaultFromDate), DateFormat, CultureInfo.InvariantCulture);
            DefaultToDate = DateTime.ParseExact(GetNodeInnerXml(doc, XmlDefaultToDate), DateFormat, CultureInfo.InvariantCulture);

            StoredParameters = new Dictionary<string, string>();
            XmlNode storedParams = doc.SelectSingleNode("/" + XmlRootName + "/" + XmlStoredParameters);

            foreach (XmlElement param in storedParams.ChildNodes) {
                StoredParameters.Add(param.GetAttribute(XmlName), param.GetAttribute(XmlValue));
            }
        }

        private string GetNodeInnerXml(XmlDocument doc, string propertyCode) {
            XmlNode elem = doc.SelectSingleNode("/" + XmlRootName + "/" + propertyCode);
            return (elem.InnerXml);
        }

        private void Save() {
            XmlDocument doc = new XmlDocument();

            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlElement rootElm = doc.CreateElement(XmlRootName);
            doc.AppendChild(rootElm);

            XmlElement dbFile = doc.CreateElement(XmlDbFilename);
            dbFile.InnerXml = DbFileName;
            XmlElement dfDate = doc.CreateElement(XmlDefaultFromDate);
            dfDate.InnerXml = DefaultFromDate.ToString(DateFormat);
            XmlElement dtDate = doc.CreateElement(XmlDefaultToDate);
            dtDate.InnerXml = DefaultToDate.ToString(DateFormat);
            XmlElement dGran = doc.CreateElement(XmlDefaultGranularity);
            dGran.InnerXml = DefaultGranularity;
            XmlElement sParam = doc.CreateElement(XmlStoredParameters);

            rootElm.AppendChild(dbFile);
            rootElm.AppendChild(dfDate);
            rootElm.AppendChild(dfDate);
            rootElm.AppendChild(dtDate);
            rootElm.AppendChild(dGran);
            rootElm.AppendChild(sParam);

            foreach (string pKey in StoredParameters.Keys) {
                XmlElement paramNode = doc.CreateElement(XmlParameter);
                paramNode.SetAttribute(XmlName, pKey);
                paramNode.SetAttribute(XmlValue, StoredParameters[pKey]);

                sParam.AppendChild(paramNode);
            }

            doc.Save(ConfigFilename);
        }

    }
}
