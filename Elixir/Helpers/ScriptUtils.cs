using System;
using System.Web;

namespace Elixir.Helpers
{
    public static class ScriptUtils
    {
        public const string BlogpostScriptFormat =
            "{" +
                "\"@context\": \"https://schema.org\"," +
                "\"@type\": \"BlogPosting\"," +
                "\"datePublished\": \"{datePublished}\"," +
                "\"dateModified\": \"{dateModified}\"," +
                "\"headline\": \"{blogTitle}\"," +
                "\"publisher\": { " +
                    "\"@type\": \"Organization\", " +
                    "\"name\": \"Live Forever Club\"," +
                    "\"logo\": { " +
                        "\"@type\": \"ImageObject\", " +
                        "\"url\": \"{publisherLogoUrl}\"" +
                    " }" +
                " }, " +
                "\"description\": \"{blogDescription}\", ";

        public const string BlogSocialImageChunk = "\"image\":\"{socialImageUrl}\",";
        public const string BlogAuthorChunk = "\"author\":{\"@type\": \"Person\", \"name\":\"Adrian Cull\"}";

        public const string ArticleScriptFormat = "" +
            "{" +
                "\"@context\": \"https://schema.org\"," +
                "\"@type\": \"NewsArticle\"," +
                "\"datePublished\": \"{datePublished}\"," +
                "\"headline\": \"{articleTitle}\"," +
                "\"publisher\": {" +
                    "\"@type\": \"Organization\"," +
                    "\"name\": \"Live Forever Club\"," +
                    "\"logo\": {" +
                        "\"@type\": \"ImageObject\"," +
                        "\"url\": \"{publisherLogoUrl}\"" +
                    "}" +
                "}," +
                "\"description\": \"{summary}\",";
        //+ ImageChunk (if exists) + AuthorChunk</script>"

        public const string ProductScriptFormat =
            "{" +
                "\"@context\": \"https://schema.org\"," +
                "\"@type\": \"Product\"," +
                "\"name\": \"{productName}\"," +
                "\"description\": \"{productDescription}\"," +
                "\"sku\": \"{sku}\"," +
                "[productImage] " +
                "\"offers\": {  " + 
                    "\"@type\": \"Offer\"," + 
                    "\"price\": \"{priceRRP}\"," +
                    "\"priceCurrency\": \"{priceCurrency}\"" +
                "}," + 
                "\"brand\": { " +
                    "\"@type\": \"Organization\", " +
                    "\"name\": \"Live Forever Club\"," +
                    "\"logo\": { " +
                        "\"@type\": \"ImageObject\", " +
                        "\"url\": \"{publisherLogoUrl}\"" +
                    " }" +
                " }" + 
            "}";
        
        public const string ArticleSocialImageChunk = "\"image\":\"{socialImageUrl}\",";
        public const string ArticleAuthorChunk = "\"author\":{\"@type\":\"{authorType}\",\"name\":\"{authorName}\"}";
        
        public static string FillFormatForBlog(string format, DateTime datePublished, DateTime? dateUpdated, 
            string title, string description, string socialImageUrl, string currentHost)
        {
            format = format.Replace("{datePublished}", datePublished.ToLongFormat());
            if (dateUpdated.HasValue)
            {
                format = format.Replace("{dateModified}", dateUpdated.Value.ToLongFormat());
            }
            if (!string.IsNullOrEmpty(title))
            {
                title = title.Replace(@"\", "\\\\");
                var _title=HttpUtility.HtmlEncode(title);
                _title=_title.Replace("&quot", "\\&quot");
                title = HttpUtility.HtmlDecode(_title);
            }
            if (!string.IsNullOrEmpty(description))
            {
                description = description.Replace(@"\", "\\\\");
                var _description = HttpUtility.HtmlEncode(description);
                _description = _description.Replace("&quot", "\\&quot");
                description = HttpUtility.HtmlDecode(_description);
            }

            if (!string.IsNullOrEmpty(socialImageUrl))
            {
                format = format + BlogSocialImageChunk + BlogAuthorChunk + "};";
                
                format = format.Replace("{socialImageUrl}", $"{currentHost}/{socialImageUrl}");
            }
            else
            {
                format = format + BlogAuthorChunk + "};";
            }
            
            format = format.Replace("{blogTitle}", title);
            format = format.Replace("{blogDescription}", description);

            format = format.Replace("{publisherLogoUrl}", $"{currentHost}/Images/logo-AMP.png");
        
            return format;
        }

        public static string FillFormatForArticle(string format, DateTime datePublished,
            string articleTitle, string dnPublisherName, string summary, string socialImageUrl, 
            string reporterName, string publisherName, string currentHost)
        {
            bool hasReporterName = !string.IsNullOrEmpty(reporterName) && 
                !reporterName.Equals("No Reporter", StringComparison.OrdinalIgnoreCase);

            var datePublishedString = datePublished.ToLongFormatNoZone();
           
            format = format.Replace("{datePublished}", datePublishedString);

            #region validations and cleanups
            if (!string.IsNullOrEmpty(articleTitle))
            {
                articleTitle = articleTitle.Replace(@"\", "\\\\");
                var _title = HttpUtility.HtmlEncode(articleTitle);
                _title = _title.Replace("&quot", "\\&quot");
                articleTitle = HttpUtility.HtmlDecode(_title);
            }
            if (!string.IsNullOrEmpty(dnPublisherName))
            {
                dnPublisherName = dnPublisherName.Replace(@"\", "\\\\");
                var _pubName = HttpUtility.HtmlEncode(dnPublisherName);
                _pubName = _pubName.Replace("&quot", "\\&quot");
                dnPublisherName = HttpUtility.HtmlDecode(_pubName);
            }
            if (!string.IsNullOrEmpty(summary))
            {
                summary = summary.Replace(@"\", "\\\\");
                var _summary = HttpUtility.HtmlEncode(summary);
                _summary = _summary.Replace("&quot", "\\&quot");
                summary = HttpUtility.HtmlDecode(_summary);
            }
            #endregion

            format = format.Replace("{articleTitle}", articleTitle);

            format = format.Replace("{summary}", $"Key points summary of {dnPublisherName} article. {summary}");

            if (!string.IsNullOrEmpty(socialImageUrl))
            {
                format = format + ArticleSocialImageChunk + ArticleAuthorChunk + "};";

                format = format.Replace("{socialImageUrl}", $"{currentHost}/{socialImageUrl}");
            }
            else
            {
                format = format + ArticleAuthorChunk + "};";
            }

            if (hasReporterName)
            {
                format = format.Replace("{authorType}", "Person");
                format = format.Replace("{authorName}", reporterName);
            }
            else
            {
                format = format.Replace("{authorType}", "NewsMediaOrganization");
                format = format.Replace("{authorName}", publisherName);
            }

            format = format.Replace("{publisherLogoUrl}", $"{currentHost}/Images/logo-AMP.png");
        
            return format;
            
        }

        public static string FillFormatForProduct(string format, string productName, string description, string sku, string image,
            string price, string currency, string currentHost)
        {
            format = format.Replace("{productName}", productName);

            format = format.Replace("{productDescription}", description);

            format = format.Replace("{sku}", sku);

            if (!string.IsNullOrEmpty(image))
            {
                format = format.Replace("[productImage]", $"\"image\": \"{currentHost}/images/shop/product/{image}\",");
            }
            else
            {
                format = format.Replace("[productImage]", " ");
            }
            format = format.Replace("{priceRRP}", price);

            format = format.Replace("{priceCurrency}", currency);

            format = format.Replace("{publisherLogoUrl}", $"{currentHost}/Images/logo-AMP.png");
            
            return format;
        }
    }
}