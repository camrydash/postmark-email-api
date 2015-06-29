using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostMarkEmail.TemplateProcessor;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.IO;

namespace PostMarkEmail
{
    public static class TemplateManager
    {
        public static System.Web.HtmlString ProcessRazor(Controller currentController, Object model, String viewName, ViewDataDictionary viewdata)
        {
            var processor = new RazorViewProcessor(currentController);
            return processor.Process(model, viewdata, viewName);            
        }

        public static System.Web.HtmlString ProcessXslt(String filepath,object graph)
        {
            if (System.IO.File.Exists(filepath))
            {

                var xslt = System.IO.File.ReadAllText(filepath);

                XmlSerializer ser = new XmlSerializer(graph.GetType());
                string xml = String.Empty;
                var stream = new MemoryStream();
                ser.Serialize(stream, graph);
                xml = System.Text.Encoding.UTF8.GetString(stream.GetBuffer());
                string body = XsltUtility.TransformXml(xml, xslt);

                return new System.Web.HtmlString(body);
            }
            else 
                throw new System.IO.FileNotFoundException(String.Format("unable to locate file {0}", filepath));
        }
    }

    

}
