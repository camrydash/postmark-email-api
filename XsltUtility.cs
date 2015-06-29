using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace PostMarkEmail
{
    public class XsltUtility
    {
        /// <summary>  
        /// Transforms the supplied xml using the supplied xslt and returns the   
        /// result of the transformation  
        /// </summary>  
        /// <param name="xml">The xml to be transformed</param>  
        /// <param name="xslt">The xslt to transform the xml</param>  
        /// <returns>The transformed xml</returns>  
        public static string TransformXml(string xml, string xslt)
        {
            // Simple data checks  
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("Param cannot be null or empty", "xml");
            }
            if (string.IsNullOrEmpty(xslt))
            {
                throw new ArgumentException("Param cannot be null or empty", "xslt");
            }

            // Create required readers for working with xml and xslt  
            StringReader xsltInput = new StringReader(xslt);
            StringReader xmlInput = new StringReader(xml);
            XmlTextReader xsltReader = new XmlTextReader(xsltInput);
            XmlTextReader xmlReader = new XmlTextReader(xmlInput);

            return TransformXml(xsltReader, xmlReader);

        }

        public static String TransformXml(XmlTextReader xsltReader, XmlTextReader xmlReader)
        {
            // Create required writer for output  
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter transformedXml = new XmlTextWriter(stringWriter);

            // Create a XslCompiledTransform to perform transformation  
            XslCompiledTransform xsltTransform = new XslCompiledTransform();

            try
            {
                xsltTransform.Load(xsltReader);
                xsltTransform.Transform(xmlReader, new XsltArgumentList(), transformedXml);
            }
            catch (XmlException xmlEx)
            {
                // TODO : log - "Could not load XSL transform: \n\n" + xmlEx.Message  
                throw;
            }
            catch (XsltException xsltEx)
            {
                // TODO : log - "Could not process the XSL: \n\n" + xsltEx.Message + "\nOn line " + xsltEx.LineNumber + " @ " + xsltEx.LinePosition)  
                throw;
            }
            catch (Exception ex)
            {
                // TODO : log  
                throw;
            }

            return stringWriter.ToString();
        }
    }  
}
