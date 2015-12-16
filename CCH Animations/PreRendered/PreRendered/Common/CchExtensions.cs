using System.Configuration;
using System.Text;
using System.Web.Mvc;

namespace PreRendered.Common
{
    public static class CchExtensions
    {
        public static MvcHtmlString Image(this HtmlHelper helper,
            string src, string altText,
            string width, string style)
        {
            var builder = new TagBuilder("img");

            string imagesPath = ConfigurationManager.AppSettings["ImagesPath"] + src;
            builder.MergeAttribute("src", imagesPath);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("width", width);
            builder.MergeAttribute("style", style);

            MvcHtmlString imageTag = MvcHtmlString.Create(
                builder.ToString(TagRenderMode.SelfClosing));
            return imageTag;
        }

        public static MvcHtmlString ImageActionLink(this HtmlHelper helper,
            string controller, string action, object parameters,
            string src, string altText, string width, string style)
        {
            return ImageActionLink(helper,
                controller, action, parameters,
                src, altText, width, style, false);
        }

        public static MvcHtmlString ImageActionLink(this HtmlHelper helper,
            string controller, string action, object parameters,
            string src, string altText, string width, string style, bool isPopup)
        {
            string imagesPath = ConfigurationManager.AppSettings["ImagesPath"];

            return ImageActionLink(helper,
                controller, action, parameters,
                src, altText, width, style,
                imagesPath, isPopup);
        }

        public static MvcHtmlString ImageActionLink(this HtmlHelper helper,
            string controller, string action, object parameters,
            string src, string altText, string width, string style,
            string imagesPath, bool isPopup)
        {
            var builder = new TagBuilder("img");

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            string url = urlHelper.Action(action, controller, parameters);

            string urlImage = imagesPath + src;

            builder.MergeAttribute("src", urlImage);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("width", width);
            builder.MergeAttribute("style", style);

            string image = builder.ToString(TagRenderMode.SelfClosing);

            StringBuilder sb = new StringBuilder();
            sb.Append("<a href=\"");
            sb.Append(url);
            sb.Append("\" class=\"imageLink\" ");
            if (isPopup)
            {
                sb.Append(" target=\"_blank\" ");
            }
            sb.Append(">");
            sb.Append(image);
            sb.Append("</a>");

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString ImageLink(this HtmlHelper helper, string url,
            string src, string altText, string width, string style, bool isPopup)
        {
            string imagesPath = ConfigurationManager.AppSettings["ImagesPath"];

            var builder = new TagBuilder("img");

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            string urlImage = imagesPath + src;

            builder.MergeAttribute("src", urlImage);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("width", width);
            builder.MergeAttribute("style", style);

            string image = builder.ToString(TagRenderMode.SelfClosing);

            StringBuilder sb = new StringBuilder();
            sb.Append("<a href=\"");
            sb.Append(url);
            sb.Append("\" class=\"imageLink\" ");
            if (isPopup)
            {
                sb.Append(" target=\"_blank\" ");
            }
            sb.Append(">");
            sb.Append(image);
            sb.Append("</a>");

            return MvcHtmlString.Create(sb.ToString());
        }

        public static string GetConfigurationValue(this string appConfigKey)
        {
            string sValue = ConfigurationManager.AppSettings[appConfigKey] ?? string.Empty;

            return sValue;
        }

        public static int GetConfigurationNumericValue(this string appConfigKey)
        {
            int iNumber;
            string sNumber = ConfigurationManager.AppSettings[appConfigKey];

            int.TryParse(sNumber, out iNumber);
            return iNumber;
        }

        public static double GetConfigurationDoubleValue(this string appConfigKey)
        {
            double dAmount;
            string sAmount = ConfigurationManager.AppSettings[appConfigKey];

            double.TryParse(sAmount, out dAmount);
            return dAmount;
        }

        public static int GetNumericValue(this string sNumber)
        {
            int iNumber;
            int.TryParse(sNumber, out iNumber);
            return iNumber;
        }

        public static string SplitOnCase(this string mixedCases)
        {
            for (int i = 1; i < mixedCases.Length - 1; i++)
            {
                if ((char.IsLower(mixedCases[i - 1]) && char.IsUpper(mixedCases[i])) ||
                    (mixedCases[i - 1] != ' ' && char.IsUpper(mixedCases[i]) && char.IsLower(mixedCases[i + 1])))
                {
                    mixedCases = mixedCases.Insert(i, " ");
                }
            }
            return mixedCases;
        }

        public static string Singular(this string plural)
        {
            string singular = plural.Substring(plural.Length - 1, 1).ToLower().Equals("s")
                ? plural.Remove(plural.Length - 1, 1)
                : plural;
            return singular;
        }
    }
}
