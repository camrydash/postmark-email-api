using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PostMarkEmail.TemplateProcessor
{
    public class RazorViewProcessor
    {

        private Controller _controller;
        public RazorViewProcessor(Controller controller)
        {
            this._controller = controller;
        }

        public MvcHtmlString Process(object model,  string viewName)
        {
            return Process(model, new ViewDataDictionary(), viewName);
        }
        public MvcHtmlString Process(object model,ViewDataDictionary viewdata,string viewName)
        {
            if (this._controller == null)
            {
                var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, viewName);
                var template = System.IO.File.ReadAllText(filePath);
                return new MvcHtmlString(RazorEngine.Razor.Parse(template, model));
            }
            else
            {
                var helper = new HtmlHelper(new ViewContext(_controller.ControllerContext, new FakeView()
                    , _controller.ViewData, _controller.TempData, System.IO.TextWriter.Null)
               , new ViewPage());
                var output = System.Web.Mvc.Html.PartialExtensions.Partial(helper, viewName, model, viewdata);
                return output;
            }
        }
    }
}
