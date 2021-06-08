using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace vControler
{
    //
    //
    public class Xml
    {
        public string Path = ".dummy";//Path of the xml file to load or to save
        private XmlDocument xml;
        private bool deletefile = false;
        XmlAttribute attrib;

        public Xml()
        {
            xml = new XmlDocument();
        }
        
        //load the specified path. If path not available or xml structure wrong, initiliaze the xml with the right structure
        public void LoadXml(string path)
        {
            Path = path;
            if (File.Exists(path))
            {
                if (!deletefile) xml.Load(path);
            }
            else File.Create(path);
            if (xml.ChildNodes.Count == 1 && xml.GetElementsByTagName("vScheduler").Count == 1)
            {
                XmlNode n = xml.FirstChild;
                if (n.ChildNodes.Count == n.SelectNodes("section").Count)
                {
                    foreach (XmlNode p in n)
                    {
                        if (p.ChildNodes.Count == p.SelectNodes("entry").Count) return;
                        else { deletefile = true; xml = new XmlDocument(); LoadXml(path); deletefile = false; break; }
                    }
                    return;
                }
                else { deletefile = true; xml = new XmlDocument(); LoadXml(path); deletefile = false; };
                return;
            }
            xml = new XmlDocument();
            xml.AppendChild(xml.CreateElement("vScheduler"));
        }

        public string GetValue(string section, string entry, string defaultvalue)
        {
            if (xml.HasChildNodes)
            {
                foreach (XmlNode n in xml.FirstChild)
                {
                    if (n.Attributes.GetNamedItem("name").Value == section)
                    {
                        foreach (XmlNode p in n)
                        {
                            if (p.Attributes.GetNamedItem("name").Value == entry) return p.InnerText;
                        }
                    }
                }
                return defaultvalue;
            }
            else return defaultvalue;
        }

        public int GetValue(string section, string entry, int defaultvalue)
        {
            string result = GetValue(section, entry, defaultvalue.ToString());
            if (result == "") return 0;
            else return Convert.ToInt32(result);
        }

        public bool GetValue(string section, string entry, bool defaultvalue)
        {
            string result = GetValue(section, entry, defaultvalue.ToString());
            if (result == "") return false;
            else return Convert.ToBoolean(result);
        }

        private void Addsection(string name)
        {
            XmlNode section = xml.CreateElement("section");
            xml.FirstChild.AppendChild(section);
            attrib = xml.CreateAttribute("name");
            attrib.Value = name;
            section.Attributes.Append(attrib);
        }

        private void Addentry(string name, XmlNode sectionnode)
        {
            XmlNode entry = xml.CreateElement("entry");
            sectionnode.AppendChild(entry);
            attrib = xml.CreateAttribute("name");
            attrib.Value = name;
            entry.Attributes.Append(attrib);
        }

        public void SetValue(string section, string entry, string defaultvalue)
        {
            if (xml.HasChildNodes)
            {
                foreach (XmlNode n in xml.FirstChild)
                {
                    if (n.Attributes.GetNamedItem("name").Value == section)
                    {
                        foreach (XmlNode p in n)
                        {
                            if (p.Attributes.GetNamedItem("name").Value == entry)
                            { p.InnerText = defaultvalue; return; }
                        }
                        Addentry(entry, n);
                        SetValue(section, entry, defaultvalue);
                        return;
                    }
                }
                Addsection(section);
                SetValue(section, entry, defaultvalue);
            }
            else
            {
                LoadXml(Path);
                SetValue(section, entry, defaultvalue);
            }
        }
    
        public void SetValue (string section, string entry, int defaultvalue)
        {
            SetValue(section, entry, defaultvalue.ToString());
        }

        public void SetValue(string section, string entry, bool defaultvalue)
        {
            SetValue(section, entry, defaultvalue.ToString());
        }

        public void Save()
        {
            xml.Save(Path);
        }

        public void Save(string path )
        {
            xml.Save(path);
        }

    }
}
