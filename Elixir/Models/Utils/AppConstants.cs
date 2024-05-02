using iTextSharp.text;

namespace Elixir.Models.Utils
{
    public static class AppConstants
    {
        public const string DefaultBannerName = "liveforeverclub-banner.jpg";
        public const string DefaultBannerNameSocial = "live-forever-club-social-LFC.jpg";
        public const string DefaultBannerNameNews = "life-extension-news.jpg";
        public const string DefaultBannerNameBlog = "liveforeverclub-banner-blog.jpg";
        public const string DefaultBannerNameNewsNew = "liveforeverclub-banner-news.jpg";
        public const string DefaultShopProductMain = "coming-soon-main.jpg";
        public const string DefaultShopProductThumb = "coming-soon-thumb.jpg";
        public const string DefaultBannerShop = "liveforeverclub-banner-shop.jpg";
        public const string ImageSearchResults = "live-forever-club-search-results.jpg";
        public const string BannerImageSearchResults = "search-results-banner.jpg";
        public const string DefaultEventBannerName = "liveforeverclub-banner-events.jpg";
        public const string DefaultEventBannerNameSocial = "live-forever-club-events-calendar.jpg";
        public static int ImageLFCBoxHeightPixels = 92;

        public static class FileSystem
        {
            public const string PublicImagesFolderName = "Images";
            public const string PublicImagesFolderServerPath = "~/" + PublicImagesFolderName + "/";
            public const string WebPageBannerImageFolderServerPath = PublicImagesFolderServerPath;
        }

        //public static class Roles
        //{
        //    public const string Administrator = "Administrator";
        //    public const string Reviewer = "Reviewer";
        //    public const string Author = "Author";
        //    public const string Member = "Member";
        //}

        /// <summary>
        /// PDF Gen related Constants.
        /// iTextSharp has dimensions in "points".
        /// Specs dimensions are in inches.
        /// 1 Inch = 72 iTextSharp points.
        /// </summary>
        public static class PdfGen
        {
            public const int Inch = 72;

            public const string FolderToSavePdfs = "MyPdfs";

            public const float TextDescenderHeight = (0.01f * Inch); // In typography, a descender is the portion of a letter that extends below the baseline of a font, e.g., in the letter y, the descender is the "tail"

            public const float SafeZoneWidth = 4.85f * Inch;//5.25f * Inch;

            public const float SafeZoneHeight = (8.5f * Inch) - TextDescenderHeight; // KDP preview had errors where letters with tails just went outside of safezone

            public const float MarginLeft = 0.7f * Inch;// 0.5f * Inch; (0.5 + 0.2)

            public const float MarginRight = 0.575f * Inch;// 0.38f * Inch; (0.375 + 0.2)

            public const float MarginTop = 0.375f * Inch;

            public const float MarginBottomForGraphics = (0.375f * Inch); // backgrounds and images can to edge of real safe zone

            public const float MarginBottomForText = MarginBottomForGraphics + TextDescenderHeight; // allows for letters with tails

            public const float PageWidth = SafeZoneWidth + MarginLeft + MarginRight;

            public const float PageHeight = SafeZoneHeight + MarginTop + MarginBottomForText;

            public const string Cost = "COST:";

            public const string Difficulty = "EASE:";

            public const string Impact = "IMPACT:";

            public const float HeaderPageNumberBlockWidth = 0.8f * Inch;

            public const float HeaderSpaceBetweenTextAndPageNumberBlocks = 0.06f * Inch;

            public const float HeaderTextBlockWidth = PageWidth - HeaderPageNumberBlockWidth - HeaderSpaceBetweenTextAndPageNumberBlocks;

            public const float HeaderPageNumberWidthPercent = HeaderPageNumberBlockWidth / PageWidth;

            public const float HeaderTextBlockWidthPercent = HeaderTextBlockWidth / PageWidth;

            public const float HeaderHeight = 0.6f * Inch;

            public const float ScoreBarsBackgroundHeight = 0.2f * Inch;

            public const float ResearchPapersBoxHeight = 0.9f * Inch;

            public const string FooterFixedPart1 = "Live Forever Manual - ";

            public const string FooterFixedPart2 = "101 Practical Tips on How to Live Forever";

            public const string Goudy = "Goudy Old Style Regular.ttf";

            public const string PlaceholderImage = "placeholder.jpg";
            public const string PlaceholderImageOdd = "placeholder-right.jpg";
            public const string PlaceholderImageEven = "placeholder-left.jpg";


            public const string Helvetica = "Helvetica-Regular.ttf";

            public const string FixedImage = "infinity-side-300DPI-720x1040.jpg";

            public const float TipsHeight = 1.48f * Inch;

            public const float TipsWidth = 3.58f * Inch;

            public static float ImageWidthPx = 720; // not used - see ImageWidth

            public static float ImageHeightPx = 1040; // not used - see ImageHeight

            public static float ImageY = PageHeight - (4.34f * Inch) - ImageHeight;

            public static float SpacePx = 20; // not used - see TipImageSpace

            public static float TipImageSpace = 0.13f * Inch;

            public static float ImageWidth = 2.4f * Inch;

            public static float ImageHeight = 3.47f * Inch;

            public static BaseColor White = new BaseColor(255, 255, 255);

            public static BaseColor Orange = new BaseColor(253, 106, 2);

            public static BaseColor Blue = new BaseColor(0, 154, 220);

            public static BaseColor LightBlue = new BaseColor(175, 206, 255);

            public static BaseColor Beige = new BaseColor(255, 243, 216);

            public static string TableOfContentsHeader = "CONTENTS";

            public static string TableOfContentsTitle = "TABLE OF CONTENTS";

            public static int TableOfContentsLinesPerPage = 44;

            public const string TipsMock = "Let your coffee/tea to sit for a few minutes in your mug.<br>" +
                                "A few drops of milk will drop by 5-10 degrees immediately.<br>" +
                                "Or buy a smart mug or a kettle with a thermometer.";

            public const string ResourcesMock = @"<h3>Products</h3>
                            <p>Bosch TWK8633GB Styline Kettle - £69.99<br>
                            Allows you to choose from four tempearature settings at the correct temperatures.</p>
                            <p>http://liveforever.club/go/boschkettle<br>
                            Ember Temperature Control Mug - £113.68<br>
                            http://liveforever.club/go/temperaturemug<br>
                            Select temperature between 120F (49C) and 145F(63C).</p>
                            <h3>Reports</h3>
                            <p>IARC Monographs evaluate drinking coffee, mate, and very hot beverages<br>
                            http://liveforever.club/go/IARCMonographs<br>
                            International Agency for Research on Cancer 2016</p>";

            public const string ChevronOddPage = "tip-chevrons-odd.png";

            public const string ChevronEvenPage = "tip-chevrons-even.png";

            public const float ChevronImageWidth = 1.2f * Inch;

            public const float Adjustment02 = 0.2f * Inch;

            public const int LeadingNormalText = 11;

            public const int LeadingResearchPapers = 10;

            public const int SpacingAfterH2 = 2;

            public const float ScoreBarCostWidth = 110.2f;

            public const float ScoreBarEaseWidth = 122.15f + 5.76f; //made to center "ease" scorebar

            public const float ScoreBarImpactWidth = 145.62f - 5.76f; //made to center "ease" scorebar
        }

        /// <summary>
        /// eBook related Constants
        /// </summary>
        public static class EBook
        {
            public const string EbookOutputFile = "live-forever-manual.html";
            public const string EbookHtmlHeader = "<html>\r\n<head>\r\n<title>Live Forever Manual</title>\r\n<link rel=\"stylesheet\" type=\"text/css\" href=\"kdp18.css\">\r\n</head>\r\n<body>\r\n";
            public const string EbookHtmlBackCover = "\r\n<mbp:pagebreak /><img src=\"cover-6x9-back.png\"/>";
            public const string EbookHtmlFooter = "\r\n</body>\r\n</html>";
            public const string EbookPageBreak = "\r\n<mbp:pagebreak />";
        }

        public static class DevSandbox
        {
            public const string TextOverImageResultImage = "result.jpg";
        }

        public static class Messages
        {
            public const string SocialPostsOnDev = "Social posts are skipped on Dev/Local to not affect Live social media.";
        }
    }
}