using System;
using System.Data;
using System.Configuration;
using System.Xml;
using System.Windows;
namespace MidasHelper_CS
{
    /// <summary>
    /// XmlHelper 的摘要说明
    /// </summary>
    public class XmlHelper
    {
        public XmlHelper()
        {
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <returns>string</returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Read(path, "/Node", "")
         * XmlHelper.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")
         ************************************************/
        public static string Read(string path, string node, string attribute)
        {
            string value = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
            }
            catch { }
            return value;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Insert(path, "/Node", "Element", "", "Value")
         * XmlHelper.Insert(path, "/Node", "Element", "Attribute", "Value")
         * XmlHelper.Insert(path, "/Node", "", "Attribute", "Value")
         ************************************************/
        public static void Insert(string path, string node, string element, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                    }
                }
                else
                {
                    XmlElement xe = doc.CreateElement(element);
                    if (attribute.Equals(""))
                        xe.InnerText = value;
                    else
                        xe.SetAttribute(attribute, value);
                    xn.AppendChild(xe);
                }
                doc.Save(path);
            }
            catch { }
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Insert(path, "/Node", "", "Value")
         * XmlHelper.Insert(path, "/Node", "Attribute", "Value")
         ************************************************/
        public static void Update(string path, string node, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xe.InnerText = value;
                else
                    xe.SetAttribute(attribute, value);
                doc.Save(path);
            }
            catch { }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时删除该节点属性值，否则删除节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Delete(path, "/Node", "")
         * XmlHelper.Delete(path, "/Node", "Attribute")
         ************************************************/
        public static void Delete(string path, string node, string attribute)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xn.ParentNode.RemoveChild(xn);
                else
                    xe.RemoveAttribute(attribute);
                doc.Save(path);
            }
            catch { }
        }

        /// <summary>
        /// 返回XMl文件指定节点的指定属性值
        /// </summary>
        /// <param name="xmlName">指定文件</param>
        /// <param name="xmlNode">指定节点</param>
        /// <param name="NodeKey">节点识别键</param>
        /// <param name="keyValue">节点识别键键值</param>
        /// <param name="NodelAttribute">需要获得的节点属性</param>
        /// <returns>返回需要的属性值</returns>
        public static string getXmlAttributeValue(string xmlName, string xmlNode, string NodeKey, string keyValue, string NodelAttribute)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlName);
            XmlNodeList xnlist = xmlDoc.SelectNodes("//" + xmlNode);
            string str = "";
            foreach (XmlNode xn in xnlist)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.HasAttribute(NodeKey))
                {
                    string Key = xe.GetAttribute(NodeKey);
                    if (Key == keyValue && xe.HasAttribute(NodelAttribute))
                    {
                        str = xe.GetAttribute(NodelAttribute);
                        break;
                    }
                    //Parent.statusLabel.Text = string.Format("id={0},Parity:{1},DataBits:{2},BaudRate{3}", id, Parity, DataBits, BaudRate);
                }
            }
            return str;
        }

        /// <summary>
        /// 返回XMl文件指定节点下元素值
        /// </summary>
        /// <param name="xmlName">指定文件</param>
        /// <param name="xmlNode">指定节点</param>
        /// <param name="NodeKey">节点识别键</param>
        /// <param name="keyValue">节点识别键键值</param>
        /// <param name="xmlElement">指定元素</param>
        /// <param name="plant">留空位</param>
        /// <returns>返回需要的元素值</returns>
        public static string getXmlElementValue(string xmlName, string xmlNode, string xmlElement)
        {
            string result = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlName);
            XmlNode xnode = xmlDoc.SelectSingleNode("//" + xmlNode);

            XmlNode node = xmlDoc.DocumentElement;
            if (xnode != null)//有该节点
            {
                XmlNode xmlnode = xnode.SelectSingleNode(xmlElement);
                if (xmlnode != null)//有该元素
                {
                    result = xmlnode.InnerText;
                }
            }
            return result;
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(xmlName);
            //XmlNodeList xnlist = xmlDoc.SelectNodes("//" + xmlNode);
            //string str = "";
            //if (NodeKey == "")
            //{
            //    XmlNodeList xmlList = ((XmlElement)xnlist[0]).GetElementsByTagName(xmlElement);
            //    str = xmlList[0].InnerText;
            //    return str;
            //}
            //foreach (XmlNode xn in xnlist)
            //{
            //    XmlElement xe = (XmlElement)xn;
            //    if (xe.HasAttribute(NodeKey))
            //    {
            //        string Key = xe.GetAttribute(NodeKey);
            //        if (Key == keyValue)
            //        {
            //            XmlNodeList xmlList = xe.GetElementsByTagName(xmlElement);
            //            str = xmlList[0].InnerText;
            //            break;
            //        }
            //        //Parent.statusLabel.Text = string.Format("id={0},Parity:{1},DataBits:{2},BaudRate{3}", id, Parity, DataBits, BaudRate);
            //    }
            //}
            //return str;
        }


        /// <summary>
        /// 设置XMl文件指定元素的指定属性的值
        /// </summary>
        /// <param name="xmlNode">指定元素</param>
        /// <param name="NodeKey">元素识别键</param>
        /// <param name="keyValue">元素识别键键值</param>
        /// <param name="NodelAttribute">需要设置的元素属性</param>
        /// <param name="attributeValue">需要设置的属性值</param>
        public static void setXmlAttributeValue(string xmlName, string xmlNode, string NodeKey, string keyValue, string NodelAttribute, string attributeValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlName);
            XmlNodeList xnlist = xmlDoc.SelectNodes("//" + xmlNode);
            foreach (XmlNode xn in xnlist)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.HasAttribute(NodeKey))
                {
                    string Key = xe.GetAttribute(NodeKey);
                    if (Key == keyValue)
                    {
                        xe.SetAttribute(NodelAttribute, attributeValue);
                        xmlDoc.Save(xmlName);
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 返回XMl文件指定节点下元素值
        /// </summary>
        /// <param name="xmlName">指定文件</param>
        /// <param name="xmlNode">指定节点</param>
        /// <param name="NodeKey">节点识别键</param>
        /// <param name="keyValue">节点识别键键值</param>
        /// <param name="xmlElement">指定元素</param>
        /// <param name="plant">留空位</param>
        /// <returns>返回需要的元素值</returns>
        public static void setXmlElementValue(string xmlName, string xmlNode, string xmlElement, string ElementValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlName);
            XmlNode xnode = xmlDoc.SelectSingleNode("//" + xmlNode);

            XmlNode node = xmlDoc.DocumentElement;
            //XmlElement newElement = null;
            if (xnode == null)//没有该节点
            {
                XmlNode new_node = xmlDoc.CreateElement(xmlNode);
                node.AppendChild(new_node);
                XmlNode new_element = xmlDoc.CreateElement(xmlElement);
                new_element.InnerText = ElementValue;
                new_node.AppendChild(new_element);
            }
            else//有该节点
            {
                XmlNode xmlnode = xnode.SelectSingleNode(xmlElement);
                if (xmlnode == null)//没有该元素
                {
                    //XmlNode new_node = xmlDoc.CreateElement(xmlNode);
                    //XmlElement newElement = xmlDoc.CreateElement(xmlNode);
                    XmlElement newElement = xmlDoc.CreateElement(xmlElement);
                    newElement.InnerText = ElementValue;
                    xnode.AppendChild(newElement);
                }
                else
                {
                    xmlnode.InnerText = ElementValue;
                }
            }
            xmlDoc.Save(xmlName);
            return;
            //foreach (XmlNode xn in xnlist)
            //{
            //    XmlElement xe = (XmlElement)xn;
            //    if (xe.HasAttribute(NodeKey))
            //    {
            //        string Key = xe.GetAttribute(NodeKey);
            //        if (Key == keyValue)
            //        {
            //            XmlNodeList xmlList = xe.GetElementsByTagName(xmlElement);

            //            xmlList[0].InnerText = ElementValue;
            //            xmlDoc.Save(xmlName);
            //            break;
            //        }
            //        //Parent.statusLabel.Text = string.Format("id={0},Parity:{1},DataBits:{2},BaudRate{3}", id, Parity, DataBits, BaudRate);
            //    }
            //}
        }

        /// <summary>  
        /// 删除XMl文件指定元素的指定属性的值  
        /// </summary>  
        /// <param name="xmlNode">指定元素</param>  
        /// <param name="NodelAttribute">指定属性</param>  
        /// <param name="xmlValue">指定属性值</param>  
        public static void removeXmlElement(string xmlName, string xmlNode, string NodeKey, string keyValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlName);
            XmlNodeList xnlist = xmlDoc.SelectNodes("//" + xmlNode);
            foreach (XmlNode xn in xnlist)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.HasAttribute(NodeKey))
                {
                    string Key = xe.GetAttribute(NodeKey);
                    if (Key == keyValue)
                    {
                        xe.RemoveChild(xn);
                        xmlDoc.Save(xmlName);
                        break;
                    }
                }
            }
        }
    }
}

