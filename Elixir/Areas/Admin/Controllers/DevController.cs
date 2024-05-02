using Elixir.Areas.Admin.Models;
using System.Web.Mvc;
using System.Drawing;
using Elixir.Models.Utils;
using System.IO;
using static Elixir.Models.Utils.AppConstants;
using System;
using Elixir.Areas.Admin.Views.Dev;
using Elixir.Attributes;
using Elixir.Models.Enums;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    public class DevController : Controller
    {
        public ActionResult Editor()
        {
            var vm = new EditorVm();

            return View(vm);
        }

        // GET: Admin/Dev
        public ActionResult Index(bool showResult = false)
        {
            var writeImagesRoot = Server.MapPath("~/Images/social-card-backgrounds");
            var resultImage = DevSandbox.TextOverImageResultImage;
            var resultImagePath = Path.Combine(writeImagesRoot, resultImage);
            var model = new TextOnImageModel();
            if (showResult)
            {
                //render the image from last form submission
                if (System.IO.File.Exists(resultImagePath))
                {
                    model.ImageResultSource = resultImage;
                    model.RenderImage = true;
                }
            }
            
            model.ImageSource = "social-background-banner-1200x628.jpg";
            model.Text = "Some text here";

            foreach (FontFamily font in FontFamily.Families)
            {
                model.FontsAvailable.Add(font.Name);
            }
            
            model.TextColorsAvailable.Add("White");
            model.TextColorsAvailable.Add("Red");
            model.TextColorsAvailable.Add("Blue");
            model.TextColorsAvailable.Add("Black");

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(TextOnImageModel vm)
        {
            string readImagesRoot = Server.MapPath("~/App_Data/Images/social-card-backgrounds");
            string readImagePath = Path.Combine(readImagesRoot, vm.ImageSource);
            
            if (!System.IO.File.Exists(readImagePath))
                return RedirectToAction("Index");
            try
            {
                Image image = Image.FromFile(readImagePath);
                Graphics graphics = Graphics.FromImage(image);
                Font font = new Font(vm.SelectedFont, vm.SelectedSize);
                Brush color = MapUserChosenColorToBrushColor(vm.SelectedColor);
                
                RectangleF rect = new RectangleF(0, 0, image.Width, image.Height);             
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                //replace rect with point if case
                graphics.DrawString(vm.Text, font, color, rect, sf);

                string writeImagesRoot = Server.MapPath("~/Images/social-card-backgrounds");
                string resultImage = DevSandbox.TextOverImageResultImage;
                string resultFullPath = Path.Combine(writeImagesRoot, resultImage);
                image.Save(resultFullPath);
            }
            catch (Exception e)
            {
                throw new Exception("Text on image Error. Message: " + e.Message + 
                    "Type: " + e.GetType() + 
                    "Base Exception: " + e.GetBaseException());
            }     
            return RedirectToAction("Index", new { showResult = true });
        }

        [HttpPost]
        public ActionResult SaveWithAjax(TextOnImageModel imageFields)
        {
            string readImagesRoot = Server.MapPath("~/Images");
            string readImagePath = Path.Combine(readImagesRoot, imageFields.ImageSource);
            if (!System.IO.File.Exists(readImagePath))
            {
                return Json(new { success = false, message = "Error loading Background Image - check path" }, 
                    JsonRequestBehavior.AllowGet);
            }
            try
            {
                #region Preparing necessary graphic objects 
                Image image = Image.FromFile(readImagePath);
                Graphics graphics = Graphics.FromImage(image);
                Font font = new Font("Georgia", imageFields.SelectedSize);
                Brush color = new SolidBrush(
                    ColorTranslator.FromHtml(imageFields.SelectedColor));
                RectangleF rect = new RectangleF(0, 0,
                    image.Width,
                    image.Height - AppConstants.ImageLFCBoxHeightPixels);
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                #endregion

                graphics.DrawString(imageFields.Text, font, color, rect, sf);

                #region Save Image
                string root = Server.MapPath("~/Images");
                string filename = imageFields.GeneratedImageFilename + ".jpg";
                string path = "";
                int lastSlash = imageFields.ImageSource.LastIndexOf("/");
                if (lastSlash != -1)
                    path = imageFields.ImageSource.Substring(0, lastSlash);

                string resultFullPath = Path.Combine(root, path, filename);
                image.Save(resultFullPath);
                string relativepath = Path.Combine("/Images", path, filename);
                string cacheBypassToken = "?t=" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss");
                string result = relativepath + cacheBypassToken;
                #endregion

                return Json(new { success = true, message = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error on generating image" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        private Brush MapUserChosenColorToBrushColor(string p)
        {
            switch (p)
            {
                case "White": return Brushes.White;
                case "Red": return Brushes.Red;
                case "Blue": return Brushes.Blue;
                case "Black": return Brushes.Black;

                default: return Brushes.Black;
            }
        }
    }
}