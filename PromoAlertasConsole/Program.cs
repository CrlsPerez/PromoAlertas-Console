using AngleSharp;
using AngleSharp.Extensions;
using PostmarkDotNet;
using PostmarkDotNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;

namespace PromoAlertasConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Program.");

            int hour = DateTime.Now.Hour;
            Console.WriteLine("Hour: " + hour);

            if (hour >= 12 || hour <= 4)
            {
                Task<List<Post>> task = GetData();
                List<Post> post = task.Result;
                Console.WriteLine("Return Data.");

                List<Post> post_selected = SelectPosts(post);

                if (post_selected.Count > 0)
                {
                    Task<PostmarkResponse> task_email = SendEmail(post_selected);
                    PostmarkResponse response = task_email.Result;

                    if (response != null && response.ErrorCode == 0)
                        Console.WriteLine("Email Sent.");
                    else
                        Console.WriteLine("Email Error.");
                }
            }            

            Console.WriteLine("End Program.");
        }

        /// <summary>
        /// GET DATA FROM THE SITE
        /// </summary>
        /// <returns>LIST OF POST</returns>
        private async static Task<List<Post>> GetData()
        {
            Console.WriteLine("Get Data.");

            var configuration = AngleSharp.Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(configuration);

            await context.OpenAsync("https://www.promodescuentos.com/nuevas");

            var lt_titulo = context.Active.GetElementsByClassName("thread-title-text");
            var lt_categoria = context.Active.GetElementsByClassName("thread-category linkPlain mute--text thread-category--type-card");
            var lt_puntos = context.Active.GetElementsByClassName("vote-temp tGrid-cell vAlign--all-m text--b");

            List<Post> posts = new List<Post>();

            for (int i = 0; i < lt_titulo.Length; i++)
                posts.Add(new Post { titulo = lt_titulo[i].Text(), categoria = lt_categoria[i].Text(), puntos = lt_puntos[i].Text().Replace("°", "") });

            return posts;
        }

        /// <summary>
        /// SELECT POST +100
        /// </summary>
        /// <param name="post">LIST OF POST</param>
        /// <returns>LIST OF POST SELECTED</returns>
        private static List<Post> SelectPosts(List<Post> post)
        {
            List<Post> post_selected = new List<Post>();

            foreach (var item in post)
            {
                int t_int = 0;
                int.TryParse(item.puntos, out t_int);

                if (t_int > 100)
                    post_selected.Add(item);
            }

            return post_selected;
        }

        /// <summary>
        /// SEND EMAIL
        /// </summary>
        /// <param name="post_selected">LISTS TO SEND</param>
        /// <returns>RESPONSE EMAIL</returns>
        private static async Task<PostmarkResponse> SendEmail(List<Post> post_selected)
        {
            string msg_body = "<ul>{0}</ul>";
            string msg_body_temp = string.Empty;

            foreach (var item in post_selected)
            {
                msg_body_temp = msg_body_temp + "<li>" + item.titulo + "</li>";
            }

            msg_body = string.Format(msg_body, msg_body_temp);
            msg_body = msg_body.Replace("\"", "'");

            /// Body Message
            string body =
                "<!doctype html> " +
                "<html xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'> " +
                "<head> " +
                "<!-- NAME: VIGNELLI --> " +
                "<!--[if gte mso 15]> " +
                "<xml> " +
                "<o:OfficeDocumentSettings> " +
                "<o:AllowPNG/> " +
                "<o:PixelsPerInch>96</o:PixelsPerInch> " +
                "</o:OfficeDocumentSettings> " +
                "</xml> " +
                "<![endif]--> " +
                "<meta charset='UTF-8'> " +
                "<meta http-equiv='X-UA-Compatible' content='IE=edge'> " +
                "<meta name='viewport' content='width=device-width, initial-scale=1'> " +
                "<title>*|MC:SUBJECT|*</title> " +
                " " +
                "<style type='text/css'> " +
                "p{ " +
                "margin:10px 0; " +
                "padding:0; " +
                "} " +
                "table{ " +
                "border-collapse:collapse; " +
                "} " +
                "h1,h2,h3,h4,h5,h6{ " +
                "display:block; " +
                "margin:0; " +
                "padding:0; " +
                "} " +
                "img,a img{ " +
                "border:0; " +
                "height:auto; " +
                "outline:none; " +
                "text-decoration:none; " +
                "} " +
                "body,#bodyTable,#bodyCell{ " +
                "height:100%; " +
                "margin:0; " +
                "padding:0; " +
                "width:100%; " +
                "} " +
                "#outlook a{ " +
                "padding:0; " +
                "} " +
                "img{ " +
                "-ms-interpolation-mode:bicubic; " +
                "} " +
                "table{ " +
                "mso-table-lspace:0pt; " +
                "mso-table-rspace:0pt; " +
                "} " +
                ".ReadMsgBody{ " +
                "width:100%; " +
                "} " +
                ".ExternalClass{ " +
                "width:100%; " +
                "} " +
                "p,a,li,td,blockquote{ " +
                "mso-line-height-rule:exactly; " +
                "} " +
                "a[href^=tel],a[href^=sms]{ " +
                "color:inherit; " +
                "cursor:default; " +
                "text-decoration:none; " +
                "} " +
                "p,a,li,td,body,table,blockquote{ " +
                "-ms-text-size-adjust:100%; " +
                "-webkit-text-size-adjust:100%; " +
                "} " +
                ".ExternalClass,.ExternalClass p,.ExternalClass td,.ExternalClass div,.ExternalClass span,.ExternalClass font{ " +
                "line-height:100%; " +
                "} " +
                "a[x-apple-data-detectors]{ " +
                "color:inherit !important; " +
                "text-decoration:none !important; " +
                "font-size:inherit !important; " +
                "font-family:inherit !important; " +
                "font-weight:inherit !important; " +
                "line-height:inherit !important; " +
                "} " +
                "a.mcnButton{ " +
                "display:block; " +
                "} " +
                ".mcnImage{ " +
                "vertical-align:bottom; " +
                "} " +
                ".mcnTextContent{ " +
                "word-break:break-word; " +
                "} " +
                ".mcnTextContent img{ " +
                "height:auto !important; " +
                "} " +
                ".mcnDividerBlock{ " +
                "table-layout:fixed !important; " +
                "} " +
                "/* " +
                "@tab Page " +
                "@section background style " +
                "@tip Set the background color and top border for your email. You may want to choose colors that match your company's branding. " +
                "*/ " +
                "body,#bodyTable,#templateFooter{ " +
                "/*@editable*/background-color:#FFFFFF; " +
                "} " +
                "/* " +
                "@tab Page " +
                "@section background style " +
                "@tip Set the background color and top border for your email. You may want to choose colors that match your company's branding. " +
                "*/ " +
                "#bodyCell{ " +
                "/*@editable*/border-top:4px solid #000000; " +
                "} " +
                "/* " +
                "@tab Page " +
                "@section heading 1 " +
                "@tip Set the styling for all first-level headings in your emails. These should be the largest of your headings. " +
                "@style heading 1 " +
                "*/ " +
                "h1{ " +
                "/*@editable*/color:#000000 !important; " +
                "display:block; " +
                "/*@editable*/font-family:Helvetica; " +
                "/*@editable*/font-size:60px; " +
                "/*@editable*/font-style:normal; " +
                "/*@editable*/font-weight:bold; " +
                "/*@editable*/line-height:125%; " +
                "/*@editable*/letter-spacing:-1px; " +
                "margin:0; " +
                "/*@editable*/text-align:center; " +
                "} " +
                "/* " +
                "@tab Page " +
                "@section heading 2 " +
                "@tip Set the styling for all second-level headings in your emails. " +
                "@style heading 2 " +
                "*/ " +
                "h2{ " +
                "/*@editable*/color:#000000 !important; " +
                "display:block; " +
                "/*@editable*/font-family:Helvetica; " +
                "/*@editable*/font-size:26px; " +
                "/*@editable*/font-style:normal; " +
                "/*@editable*/font-weight:bold; " +
                "/*@editable*/line-height:125%; " +
                "/*@editable*/letter-spacing:normal; " +
                "margin:0; " +
                "/*@editable*/text-align:center; " +
                "} " +
                "/* " +
                "@tab Page " +
                "@section heading 3 " +
                "@tip Set the styling for all third-level headings in your emails. " +
                "@style heading 3 " +
                "*/ " +
                "h3{ " +
                "/*@editable*/color:#000000 !important; " +
                "display:block; " +
                "/*@editable*/font-family:Helvetica; " +
                "/*@editable*/font-size:20px; " +
                "/*@editable*/font-style:normal; " +
                "/*@editable*/font-weight:bold; " +
                "/*@editable*/line-height:125%; " +
                "/*@editable*/letter-spacing:normal; " +
                "margin:0; " +
                "/*@editable*/text-align:center; " +
                "} " +
                "/* " +
                "@tab Page " +
                "@section heading 4 " +
                "@tip Set the styling for all fourth-level headings in your emails. These should be the smallest of your headings. " +
                "@style heading 4 " +
                "*/ " +
                "h4{ " +
                "/*@editable*/color:#000000 !important; " +
                "display:block; " +
                "/*@editable*/font-family:Helvetica; " +
                "/*@editable*/font-size:16px; " +
                "/*@editable*/font-style:normal; " +
                "/*@editable*/font-weight:bold; " +
                "/*@editable*/line-height:125%; " +
                "/*@editable*/letter-spacing:normal; " +
                "margin:0; " +
                "/*@editable*/text-align:left; " +
                "} " +
                "/* " +
                "@tab Preheader " +
                "@section preheader style " +
                "@tip Set the background color and borders for your email's preheader area. " +
                "*/ " +
                "#templatePreheader{ " +
                "/*@editable*/background-color:#FFFFFF; " +
                "/*@editable*/border-top:0; " +
                "/*@editable*/border-bottom:1px solid #000000; " +
                "} " +
                "/* " +
                "@tab Preheader " +
                "@section preheader text " +
                "@tip Set the styling for your email's preheader text. Choose a size and color that is easy to read. " +
                "*/ " +
                ".preheaderContainer .mcnTextContent,.preheaderContainer .mcnTextContent p{ " +
                "/*@editable*/color:#000000; " +
                "/*@editable*/font-family:Helvetica; " +
                "/*@editable*/font-size:11px; " +
                "/*@editable*/line-height:125%; " +
                "/*@editable*/text-align:left; " +
                "} " +
                "/* " +
                "@tab Preheader " +
                "@section preheader link " +
                "@tip Set the styling for your email's header links. Choose a color that helps them stand out from your text. " +
                "*/ " +
                ".preheaderContainer .mcnTextContent a{ " +
                "/*@editable*/color:#000000; " +
                "/*@editable*/font-weight:bold; " +
                "/*@editable*/text-decoration:none; " +
                "} " +
                "/* " +
                "@tab Header " +
                "@section header style " +
                "@tip Set the background color and borders for your email's header area. " +
                "*/ " +
                "#templateHeader{ " +
                "/*@editable*/background-color:#FFFFFF; " +
                "/*@editable*/border-top:0; " +
                "/*@editable*/border-bottom:0; " +
                "} " +
                "/* " +
                "@tab Header " +
                "@section header text " +
                "@tip Set the styling for your email's header text. Choose a size and color that is easy to read. " +
                "*/ " +
                ".headerContainer .mcnTextContent,.headerContainer .mcnTextContent p{ " +
                "/*@editable*/color:#000000; " +
                "/*@editable*/font-family:Helvetica; " +
                "/*@editable*/font-size:15px; " +
                "/*@editable*/line-height:150%; " +
                "/*@editable*/text-align:left; " +
                "} " +
                "/* " +
                "@tab Header " +
                "@section header link " +
                "@tip Set the styling for your email's header links. Choose a color that helps them stand out from your text. " +
                "*/ " +
                ".headerContainer .mcnTextContent a{ " +
                "/*@editable*/color:#ED1B24; " +
                "/*@editable*/font-weight:bold; " +
                "/*@editable*/text-decoration:none; " +
                "} " +
                "/* " +
                "@tab Body " +
                "@section body style " +
                "@tip Set the background color and borders for your email's body area. " +
                "*/ " +
                "#templateBody{ " +
                "/*@editable*/background-color:#FFFFFF; " +
                "/*@editable*/border-top:0; " +
                "/*@editable*/border-bottom:0; " +
                "} " +
                "/* " +
                "@tab Body " +
                "@section body text " +
                "@tip Set the styling for your email's body text. Choose a size and color that is easy to read. " +
                "*/ " +
                ".bodyContainer .mcnTextContent,.bodyContainer .mcnTextContent p{ " +
                "/*@editable*/color:#000000; " +
                "/*@editable*/font-family:Helvetica; " +
                "/*@editable*/font-size:15px; " +
                "/*@editable*/line-height:150%; " +
                "/*@editable*/text-align:left; " +
                "} " +
                "/* " +
                "@tab Body " +
                "@section body link " +
                "@tip Set the styling for your email's body links. Choose a color that helps them stand out from your text. " +
                "*/ " +
                ".bodyContainer .mcnTextContent a{ " +
                "/*@editable*/color:#ED1B24; " +
                "/*@editable*/font-weight:bold; " +
                "/*@editable*/text-decoration:none; " +
                "} " +
                "/* " +
                "@tab Footer " +
                "@section footer style " +
                "@tip Set the borders for your email's footer area. " +
                "*/ " +
                "#templateFooter{ " +
                "/*@editable*/border-top:0; " +
                "/*@editable*/border-bottom:0; " +
                "} " +
                "/* " +
                "@tab Footer " +
                "@section footer text " +
                "@tip Set the styling for your email's footer text. Choose a size and color that is easy to read. " +
                "*/ " +
                ".footerContainer .mcnTextContent,.footerContainer .mcnTextContent p{ " +
                "/*@editable*/color:#000000; " +
                "/*@editable*/font-family:Helvetica; " +
                "/*@editable*/font-size:11px; " +
                "/*@editable*/line-height:125%; " +
                "/*@editable*/text-align:center; " +
                "} " +
                "/* " +
                "@tab Footer " +
                "@section footer link " +
                "@tip Set the styling for your email's footer links. Choose a color that helps them stand out from your text. " +
                "*/ " +
                ".footerContainer .mcnTextContent a{ " +
                "/*@editable*/color:#000000; " +
                "/*@editable*/font-weight:bold; " +
                "/*@editable*/text-decoration:none; " +
                "} " +
                "@media only screen and (max-width: 480px){ " +
                "body,table,td,p,a,li,blockquote{ " +
                "-webkit-text-size-adjust:none !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "body{ " +
                "width:100% !important; " +
                "min-width:100% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".templateContainer{ " +
                "max-width:600px !important; " +
                "width:100% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcnImage{ " +
                "height:auto !important; " +
                "width:100% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ .mcnCartContainer,.mcnCaptionTopContent,.mcnRecContentContainer,.mcnCaptionBottomContent,.mcnTextContentContainer,.mcnBoxedTextContentContainer,.mcnImageGroupContentContainer,.mcnCaptionLeftTextContentContainer,.mcnCaptionRightTextContentContainer,.mcnCaptionLeftImageContentContainer,.mcnCaptionRightImageContentContainer,.mcnImageCardLeftTextContentContainer,.mcnImageCardRightTextContentContainer{ " +
                "max-width:100% !important; " +
                "width:100% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcnBoxedTextContentContainer{ " +
                "min-width:100% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcnImageGroupContent{ " +
                "padding:9px !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcnCaptionLeftContentOuter .mcnTextContent,.mcnCaptionRightContentOuter .mcnTextContent{ " +
                "padding-top:9px !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcnImageCardTopImageContent,.mcnCaptionBlockInner .mcnCaptionTopContent:last-child .mcnTextContent{ " +
                "padding-top:18px !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcnImageCardBottomImageContent{ " +
                "padding-bottom:9px !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcnImageGroupBlockInner{ " +
                "padding-top:0 !important; " +
                "padding-bottom:0 !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcnImageGroupBlockOuter{ " +
                "padding-top:9px !important; " +
                "padding-bottom:9px !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcnTextContent,.mcnBoxedTextContentColumn{ " +
                "padding-right:18px !important; " +
                "padding-left:18px !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcnImageCardLeftImageContent,.mcnImageCardRightImageContent{ " +
                "padding-right:18px !important; " +
                "padding-bottom:0 !important; " +
                "padding-left:18px !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                ".mcpreview-image-uploader{ " +
                "display:none !important; " +
                "width:100% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "/* " +
                "@tab Mobile Styles " +
                "@section heading 1 " +
                "@tip Make the first-level headings larger in size for better readability on small screens. " +
                "*/ " +
                "h1{ " +
                "/*@editable*/font-size:24px !important; " +
                "/*@editable*/line-height:125% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "/* " +
                "@tab Mobile Styles " +
                "@section heading 2 " +
                "@tip Make the second-level headings larger in size for better readability on small screens. " +
                "*/ " +
                "h2{ " +
                "/*@editable*/font-size:20px !important; " +
                "/*@editable*/line-height:125% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "/* " +
                "@tab Mobile Styles " +
                "@section heading 3 " +
                "@tip Make the third-level headings larger in size for better readability on small screens. " +
                "*/ " +
                "h3{ " +
                "/*@editable*/font-size:18px !important; " +
                "/*@editable*/line-height:125% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "/* " +
                "@tab Mobile Styles " +
                "@section heading 4 " +
                "@tip Make the fourth-level headings larger in size for better readability on small screens. " +
                "*/ " +
                "h4{ " +
                "/*@editable*/font-size:16px !important; " +
                "/*@editable*/line-height:125% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "/* " +
                "@tab Mobile Styles " +
                "@section Boxed Text " +
                "@tip Make the boxed text larger in size for better readability on small screens. We recommend a font size of at least 16px. " +
                "*/ " +
                ".mcnBoxedTextContentContainer .mcnTextContent,.mcnBoxedTextContentContainer .mcnTextContent p{ " +
                "/*@editable*/font-size:18px !important; " +
                "/*@editable*/line-height:125% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "/* " +
                "@tab Mobile Styles " +
                "@section Preheader Visibility " +
                "@tip Set the visibility of the email's preheader on small screens. You can hide it to save space. " +
                "*/ " +
                "#templatePreheader{ " +
                "/*@editable*/display:block !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "/* " +
                "@tab Mobile Styles " +
                "@section Preheader Text " +
                "@tip Make the preheader text larger in size for better readability on small screens. " +
                "*/ " +
                ".preheaderContainer .mcnTextContent,.preheaderContainer .mcnTextContent p{ " +
                "/*@editable*/font-size:14px !important; " +
                "/*@editable*/line-height:115% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "/* " +
                "@tab Mobile Styles " +
                "@section Header Text " +
                "@tip Make the header text larger in size for better readability on small screens. " +
                "*/ " +
                ".headerContainer .mcnTextContent,.headerContainer .mcnTextContent p{ " +
                "/*@editable*/font-size:18px !important; " +
                "/*@editable*/line-height:125% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "/* " +
                "@tab Mobile Styles " +
                "@section Body Text " +
                "@tip Make the body text larger in size for better readability on small screens. We recommend a font size of at least 16px. " +
                "*/ " +
                ".bodyContainer .mcnTextContent,.bodyContainer .mcnTextContent p{ " +
                "/*@editable*/font-size:18px !important; " +
                "/*@editable*/line-height:125% !important; " +
                "} " +
                " " +
                "}	@media only screen and (max-width: 480px){ " +
                "/* " +
                "@tab Mobile Styles " +
                "@section footer text " +
                "@tip Make the body content text larger in size for better readability on small screens. " +
                "*/ " +
                ".footerContainer .mcnTextContent,.footerContainer .mcnTextContent p{ " +
                "/*@editable*/font-size:14px !important; " +
                "/*@editable*/line-height:115% !important; " +
                "} " +
                " " +
                "}</style></head> " +
                "<body leftmargin='0' marginwidth='0' topmargin='0' marginheight='0' offset='0'> " +
                "<center> " +
                "<table align='center' border='0' cellpadding='0' cellspacing='0' height='100%' width='100%' id='bodyTable'> " +
                "<tr> " +
                "<td align='center' valign='top' id='bodyCell' style='padding-bottom:40px;'> " +
                "<!-- BEGIN TEMPLATE // --> " +
                "<table border='0' cellpadding='0' cellspacing='0' width='100%'> " +
                "<tr> " +
                "<td align='center' valign='top'> " +
                "<!-- BEGIN PREHEADER // --> " +
                "<table border='0' cellpadding='0' cellspacing='0' width='100%' id='templatePreheader'> " +
                "<tr> " +
                "<td align='center' valign='top'> " +
                "<table border='0' cellpadding='0' cellspacing='0' width='600' class='templateContainer'> " +
                "<tr> " +
                "<td valign='top' class='preheaderContainer' style='padding-top:10px; padding-bottom:10px'><table border='0' cellpadding='0' cellspacing='0' width='100%' class='mcnTextBlock' style='min-width:100%;'> " +
                "<tbody class='mcnTextBlockOuter'> " +
                "<tr> " +
                "<td valign='top' class='mcnTextBlockInner' style='padding-top:9px;'> " +
                "<!--[if mso]> " +
                "<table align='left' border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100%;'> " +
                "<tr> " +
                "<![endif]--> " +
                " " +
                "<!--[if mso]> " +
                "<td valign='top' width='390' style='width:390px;'> " +
                "<![endif]--> " +
                "<table align='left' border='0' cellpadding='0' cellspacing='0' style='max-width:390px;' width='100%' class='mcnTextContentContainer'> " +
                "<tbody><tr> " +
                " " +
                "<td valign='top' class='mcnTextContent' style='padding-top:0; padding-left:18px; padding-bottom:9px; padding-right:18px;'> " +
                " " +
                "Notificaciones de las mejores ofertas. " +
                "</td> " +
                "</tr> " +
                "</tbody></table> " +
                "<!--[if mso]> " +
                "</td> " +
                "<![endif]--> " +
                " " +
                "<!--[if mso]> " +
                "<td valign='top' width='210' style='width:210px;'> " +
                "<![endif]--> " +
                "<table align='left' border='0' cellpadding='0' cellspacing='0' style='max-width:210px;' width='100%' class='mcnTextContentContainer'> " +
                "<tbody><tr> " +
                " " +
                "<td valign='top' class='mcnTextContent' style='padding-top:0; padding-left:18px; padding-bottom:9px; padding-right:18px;'> " +
                " " +
                "</td> " +
                "</tr> " +
                "</tbody></table> " +
                "<!--[if mso]> " +
                "</td> " +
                "<![endif]--> " +
                " " +
                "<!--[if mso]> " +
                "</tr> " +
                "</table> " +
                "<![endif]--> " +
                "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table></td> " +
                "</tr> " +
                "</table> " +
                "</td>                                             " +
                "</tr> " +
                "</table> " +
                "<!-- // END PREHEADER --> " +
                "</td> " +
                "</tr> " +
                "<tr> " +
                "<td align='center' valign='top'> " +
                "<!-- BEGIN HEADER // --> " +
                "<table border='0' cellpadding='0' cellspacing='0' width='100%' id='templateHeader'> " +
                "<tr> " +
                "<td align='center' valign='top'> " +
                "<table border='0' cellpadding='0' cellspacing='0' width='600' class='templateContainer'> " +
                "<tr> " +
                "<td valign='top' class='headerContainer' style='padding-top:10px; padding-bottom:10px;'><table border='0' cellpadding='0' cellspacing='0' width='100%' class='mcnTextBlock' style='min-width:100%;'> " +
                "<tbody class='mcnTextBlockOuter'> " +
                "<tr> " +
                "<td valign='top' class='mcnTextBlockInner' style='padding-top:9px;'> " +
                "<!--[if mso]> " +
                "<table align='left' border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100%;'> " +
                "<tr> " +
                "<![endif]--> " +
                " " +
                "<!--[if mso]> " +
                "<td valign='top' width='600' style='width:600px;'> " +
                "<![endif]--> " +
                "<table align='left' border='0' cellpadding='0' cellspacing='0' style='max-width:100%; min-width:100%;' width='100%' class='mcnTextContentContainer'> " +
                "<tbody><tr> " +
                " " +
                "<td valign='top' class='mcnTextContent' style='padding-top:0; padding-right:18px; padding-bottom:9px; padding-left:18px;'> " +
                " " +
                "<h1>PromoAlertas</h1> " +
                " " +
                "<h2>Notificaciones de las mejores ofertas.</h2> " +
                " " +
                "</td> " +
                "</tr> " +
                "</tbody></table> " +
                "<!--[if mso]> " +
                "</td> " +
                "<![endif]--> " +
                " " +
                "<!--[if mso]> " +
                "</tr> " +
                "</table> " +
                "<![endif]--> " +
                "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table><table border='0' cellpadding='0' cellspacing='0' width='100%' class='mcnDividerBlock' style='min-width:100%;'> " +
                "<tbody class='mcnDividerBlockOuter'> " +
                "<tr> " +
                "<td class='mcnDividerBlockInner' style='min-width:100%; padding:18px;'> " +
                "<table class='mcnDividerContent' border='0' cellpadding='0' cellspacing='0' width='100%' style='min-width: 100%;border-top: 4px solid #000000;'> " +
                "<tbody><tr> " +
                "<td> " +
                "<span></span> " +
                "</td> " +
                "</tr> " +
                "</tbody></table> " +
                "<!--             " +
                "<td class='mcnDividerBlockInner' style='padding: 18px;'> " +
                "<hr class='mcnDividerContent' style='border-bottom-color:none; border-left-color:none; border-right-color:none; border-bottom-width:0; border-left-width:0; border-right-width:0; margin-top:0; margin-right:0; margin-bottom:0; margin-left:0;' /> " +
                "--> " +
                "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table></td> " +
                "</tr> " +
                "</table> " +
                "</td> " +
                "</tr> " +
                "</table> " +
                "<!-- // END HEADER --> " +
                "</td> " +
                "</tr> " +
                "<tr> " +
                "<td align='center' valign='top'> " +
                "<!-- BEGIN BODY // --> " +
                "<table border='0' cellpadding='0' cellspacing='0' width='100%' id='templateBody'> " +
                "<tr> " +
                "<td align='center' valign='top'> " +
                "<table border='0' cellpadding='0' cellspacing='0' width='600' class='templateContainer'> " +
                "<tr> " +
                "<td valign='top' class='bodyContainer' style='padding-top:10px; padding-bottom:10px;'><table border='0' cellpadding='0' cellspacing='0' width='100%' class='mcnTextBlock' style='min-width:100%;'> " +
                "<tbody class='mcnTextBlockOuter'> " +
                "<tr> " +
                "<td valign='top' class='mcnTextBlockInner' style='padding-top:9px;'> " +
                "<!--[if mso]> " +
                "<table align='left' border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100%;'> " +
                "<tr> " +
                "<![endif]--> " +
                " " +
                "<!--[if mso]> " +
                "<td valign='top' width='600' style='width:600px;'> " +
                "<![endif]--> " +
                "<table align='left' border='0' cellpadding='0' cellspacing='0' style='max-width:100%; min-width:100%;' width='100%' class='mcnTextContentContainer'> " +
                "<tbody><tr> " +
                " " +
                "<td valign='top' class='mcnTextContent' style='padding-top:0; padding-right:18px; padding-bottom:9px; padding-left:18px;'> " +
                " " +
                "<strong>Ofertas:</strong> " +
                " " +
                "{0}" +
                "<br> " +
                "Estas alertas se enviaran cada determinado tiempo. " +
                "</td> " +
                "</tr> " +
                "</tbody></table> " +
                "<!--[if mso]> " +
                "</td> " +
                "<![endif]--> " +
                " " +
                "<!--[if mso]> " +
                "</tr> " +
                "</table> " +
                "<![endif]--> " +
                "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table></td> " +
                "</tr> " +
                "</table> " +
                "</td> " +
                "</tr> " +
                "</table> " +
                "<!-- // END BODY --> " +
                "</td> " +
                "</tr> " +
                "<tr> " +
                "<td align='center' valign='top'> " +
                "<!-- BEGIN FOOTER // --> " +
                "<table border='0' cellpadding='0' cellspacing='0' width='100%' id='templateFooter'> " +
                "<tr> " +
                "<td align='center' valign='top'> " +
                "<table border='0' cellpadding='0' cellspacing='0' width='600' class='templateContainer'> " +
                "<tr> " +
                "<td valign='top' class='footerContainer' style='padding-top:10px; padding-bottom:10px;'><table border='0' cellpadding='0' cellspacing='0' width='100%' class='mcnDividerBlock' style='min-width:100%;'> " +
                "<tbody class='mcnDividerBlockOuter'> " +
                "<tr> " +
                "<td class='mcnDividerBlockInner' style='min-width:100%; padding:18px;'> " +
                "<table class='mcnDividerContent' border='0' cellpadding='0' cellspacing='0' width='100%' style='min-width: 100%;border-top: 4px solid #000000;'> " +
                "<tbody><tr> " +
                "<td> " +
                "<span></span> " +
                "</td> " +
                "</tr> " +
                "</tbody></table> " +
                "<!--             " +
                "<td class='mcnDividerBlockInner' style='padding: 18px;'> " +
                "<hr class='mcnDividerContent' style='border-bottom-color:none; border-left-color:none; border-right-color:none; border-bottom-width:0; border-left-width:0; border-right-width:0; margin-top:0; margin-right:0; margin-bottom:0; margin-left:0;' /> " +
                "--> " +
                "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table><table border='0' cellpadding='0' cellspacing='0' width='100%' class='mcnTextBlock' style='min-width:100%;'> " +
                "<tbody class='mcnTextBlockOuter'> " +
                "<tr> " +
                "<td valign='top' class='mcnTextBlockInner' style='padding-top:9px;'> " +
                "<!--[if mso]> " +
                "<table align='left' border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100%;'> " +
                "<tr> " +
                "<![endif]--> " +
                " " +
                "<!--[if mso]> " +
                "<td valign='top' width='600' style='width:600px;'> " +
                "<![endif]--> " +
                "<table align='left' border='0' cellpadding='0' cellspacing='0' style='max-width:100%; min-width:100%;' width='100%' class='mcnTextContentContainer'> " +
                "<tbody><tr> " +
                " " +
                "<td valign='top' class='mcnTextContent' style='padding-top:0; padding-right:18px; padding-bottom:9px; padding-left:18px;'> " +
                " " +
                "<em>Copyright © 2016, All rights reserved.</em> " +
                "</br> " +
                "<em><a href='http://promodescuentos.com' target='_blank'>PromoDescuentos.com</a> </em>" +
                "</td> " +
                "</tr> " +
                "</tbody></table> " +
                "<!--[if mso]> " +
                "</td> " +
                "<![endif]--> " +
                " " +
                "<!--[if mso]> " +
                "</tr> " +
                "</table> " +
                "<![endif]--> " +
                "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table></td> " +
                "</tr> " +
                "</table> " +
                "</td> " +
                "</tr> " +
                "</table> " +
                "<!-- // END FOOTER --> " +
                "</td> " +
                "</tr> " +
                "</table> " +
                "<!-- // END TEMPLATE --> " +
                "</td> " +
                "</tr> " +
                "</table> " +
                "</center> " +
                "</body> " +
                "</html>";

            body = body.Replace("{0}", msg_body);

            PostmarkMessage message = new PostmarkMessage()
            {
                To = "",
                From = "",
                TrackOpens = true,
                Subject = "Promodescuentos Alertas.",
                TextBody = "Notificaciones de las mejores ofertas.",
                HtmlBody = body,
                Tag = "business-message",
                Headers = new HeaderCollection{
                    {"X-CUSTOM-HEADER", "Header content"}
                }
            };

            //////////////////////////////////////////////////
            // Filter
            //////////////////////////////////////////////////

            bool send_email = false;

            foreach (var item in post_selected)
            {
                if (send_email)
                    continue;

                string item_temp = item.titulo.ToUpper();
                if (
                    item_temp.Contains("XBOX") || 
                    item_temp.Contains("ROKU") 
                    )
                {
                    send_email = true;
                }

                item_temp = item.categoria.ToUpper();
                if (
                    item_temp.Contains("ROPA")
                    )
                {
                    send_email = true;
                }
            }

            if (send_email)
            {
                PostmarkClient client = new PostmarkClient("");
                PostmarkResponse sendResult = await client.SendMessageAsync(message);
                return sendResult;
            }

            return null;

            //////////////////////////////////////////////////
            // No Filter
            //////////////////////////////////////////////////

            //PostmarkClient client = new PostmarkClient("");
            //PostmarkResponse sendResult = await client.SendMessageAsync(message);
            //return sendResult;            
        }
    }

    public class Post
    {
        public String titulo { get; set; }
        public String categoria { get; set; }
        public String puntos { get; set; }
    }
}
