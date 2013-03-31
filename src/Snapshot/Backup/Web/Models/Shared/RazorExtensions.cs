using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Microsoft.Web.Mvc;
using Web.Bootstrap.Routes;

namespace Web.Models.Shared
{
    public static class RazorExtensions
    {
        public static HelperResult Maybe(this string @string,
                                         Func<object, HelperResult> template)
        {
            return new HelperResult(writer =>
            {
                if (!String.IsNullOrEmpty(@string))
                {
                    template(@string).WriteTo(writer);
                }
            });
        }
        
        public static HelperResult ToDayFormat(this DateTime? date,
                                         Func<object, HelperResult> template,
                                    string format = "MMMM, dd yyyy")
        {
            
            return new HelperResult(writer =>
            {
                if (date.HasValue )
                {
                    template(date.Value.ToString(format)).WriteTo(writer);
                }
            });
        }

        public static HelperResult HomeLink(this UrlHelper url, string linkText)
        {
            return new HelperResult(writer =>
            {
                writer.Write(string.Format("<a href={0}>{1}</a>", url.RouteUrl(DefaultRouteRegistrar.DEFAULT_ROUTE, new { controller = "Home", action = "Index" }),linkText));
            });
        }

        public static HelperResult CreateButton<TController>(this HtmlHelper htmlHelper, 
                                                             Expression<Action<TController>> createAction, 
                                                             string routeName) where TController : Controller
        {
            RouteValueDictionary routingValues = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(createAction);

            TagBuilder a = new TagBuilder("a");
            a.MergeAttribute("href", Url(htmlHelper, routeName, routingValues));
			a.MergeAttribute("class", "create-wrapper");

            TagBuilder button = new TagBuilder("button");
            button.SetInnerText("Create new");

            button.AddCssClass("sexybutton sexysimple sexygreen");
            button.MergeAttribute("type", "button");

            a.InnerHtml = button.ToString();

            return new HelperResult(writer =>
            {
                writer.WriteLine(a.ToString());
            });
        }

        private static string Url(HtmlHelper htmlHelper, string routeName, RouteValueDictionary routingValues)
        {
            return new UrlHelper(htmlHelper.ViewContext.RequestContext).RouteUrl(routeName, routingValues);
        }

        public static HelperResult DeleteButton<TController>(this HtmlHelper htmlHelper, Expression<Action<TController>> deleteAction, string routeName) where TController :Controller
        {
            RouteValueDictionary routingValues = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(deleteAction);

            var form = new TagBuilder("form");
            form.MergeAttribute("method", "post");
            form.MergeAttribute("action", Url(htmlHelper, routeName, routingValues));

            var submitButton = htmlHelper.SubmitButton("delete-button", "Delete", new
            {
                @class = "sexybutton sexysimple sexyred"
            });

            form.InnerHtml = submitButton.ToHtmlString(); 

            return new HelperResult(writer =>
            {
                writer.Write(form.ToString());
            });
        }

        public static HelperResult FormButtons<TController>(this HtmlHelper htmlHelper, Expression<Action<TController>> cancelAction, string routeName) where TController:Controller
        {
            RouteValueDictionary routingValues = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(cancelAction);
            
            var submitButton = htmlHelper.SubmitButton("submit-form", "Save", new
               {
                   @class = "sexybutton sexysimple sexyteal"
               });
            TagBuilder orSpan = new TagBuilder("span");
            orSpan.SetInnerText("or");
            

            var cancel = new TagBuilder("a");
            cancel.AddCssClass("cancel");
            cancel.SetInnerText("Cancel");
            cancel.MergeAttribute("href", Url(htmlHelper, routeName, routingValues));

            return new HelperResult(writer =>
            {
                writer.Write("<hr/>");
                writer.Write(submitButton.ToHtmlString());

                writer.Write("&nbsp;");
                writer.Write(orSpan.ToString());
                writer.Write("&nbsp;");

                writer.Write(cancel.ToString());
            });
         
        }

        public static HelperResult PrintButton(this HtmlHelper htmlHelper)
        {
            var printButton = htmlHelper.Button("PrintButton", "Print", HtmlButtonType.Button, "print()",
                new {@class="sexybutton sexysimple sexypurple"});

            return new HelperResult(
                w=>w.Write(printButton.ToHtmlString())
            );
            
        }

        public static HelperResult ActionButton<TController>(
            this HtmlHelper htmlHelper, Expression<Action<TController>> action, string routeName, string buttonText, object attributes = null) where TController : Controller
        {
            RouteValueDictionary routingValues = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);

            var a = new TagBuilder("a");
            a.MergeAttribute("href", Url(htmlHelper, routeName, routingValues));

            var button = new TagBuilder("button");
            button.SetInnerText(buttonText);

            if (attributes==null)
            {
                button.AddCssClass("sexybutton sexysimple sexyteal");
            }
            else
            {
                button.AddCssClass(attributes.ToString());
            }

            button.MergeAttribute("type", "button");

            a.InnerHtml = button.ToString();

            return new HelperResult(writer => writer.WriteLine(a.ToString()));
        }


        public static string AssetUrl(this UrlHelper url, string uri)
        {
            return url.RouteUrl<Web.Controllers.AssetsController>(it => it.Shared(uri), Web.Bootstrap.Routes.AssetRoutesRegistrar.SHARED);
        }  
 
        
    }
}