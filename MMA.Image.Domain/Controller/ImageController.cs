using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Interfaces.AgendaItems;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Image.Domain.Controller
{
    [Route("[controller]")]
    //[Authorize("Bearer")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IAgendaItemService _agendaItemService;
        private IMemoryCache _cache;
        private IConfiguration _config;
        private readonly IUserService _userService;

        public ImageController(
            IImageService imageService,
            IAgendaItemService agendaItemService,
            IMemoryCache cache,
            IConfiguration config,
            IUserService userService
            )
        {
            _imageService = imageService;
            _agendaItemService = agendaItemService;
            _cache = cache;
            _config = config;
            _userService = userService;
        }


        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetImageForUser(string id)
        {
            string imageKey = $"image_{id}";

            if (!_cache.TryGetValue(imageKey, out byte[] image))
            {
                image = await GetImage(id, imageKey);
            }

            return File(image, "image/jpeg");
        }

        [HttpGet("user/{id}/{ticks}")]
        public async Task<IActionResult> GetLatestImageForUser(string id, string ticks)
        {
            string imageKey = $"image_{ticks}_{id}";

            if (!_cache.TryGetValue(imageKey, out byte[] image))
            {
                image = await GetImage(id, imageKey);
            }

            return File(image, "image/jpeg");
        }

        private async Task<byte[]> GetImage(string id, string imageKey)
        {
            byte[] image = null;

            var user = await _imageService.GetuserImage(id);

            if (!string.IsNullOrEmpty(user))
            {
                byte[] userImageBytes = Convert.FromBase64String(user);
                using (var ms = new MemoryStream(userImageBytes))
                {
                    using (System.Drawing.Image img = new Bitmap(ms))
                    {
                        image = _imageService.ImageToByteArray(img);
                    }

                }
            }
            else
            {
                var img = _config["Images:DefaultUserImageURL"];
                var baseUserImg = Path.Combine(img);
                image = System.IO.File.ReadAllBytes(baseUserImg);

            }

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                SlidingExpiration = TimeSpan.FromDays(1)
            };

            _cache.Set(imageKey, image, options);

            return image;
        }

        [HttpGet("presentation_missing")]
        public IActionResult PresentationMissing()
        {
            string imageKey = $"presentation_missing.svg";

            if (!_cache.TryGetValue(imageKey, out byte[] image))
            {
                 var img = _config["Images:PresentationMissingImageURL"];
                 var baseUserImg = Path.Combine(img);
                 image = System.IO.File.ReadAllBytes(baseUserImg);
                               
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                    SlidingExpiration = TimeSpan.FromDays(1)
                };

                _cache.Set(imageKey, image, options);
            }
            return File(image, "image/svg+xml");
        }

        [HttpGet("presentation_missing_thumbnail/{agendaItemId}")]
        public async Task<IActionResult> PresentationMissingThumbnail(int agendaItemId)
        {
            string imageKey = $"presentation_missing.png_{agendaItemId}";
            //byte[] result = null;

            if (!_cache.TryGetValue(imageKey, out byte[] result))
            {
                var agendaItem = await _agendaItemService.GetAsync(agendaItemId);

                var img = _config["Images:PresentationMissingImageThumbnail"];
                var baseUserImg = Path.Combine(img);
                result = System.IO.File.ReadAllBytes(baseUserImg);

                if (agendaItem != null)
                {
                    StringFormat stringFormat = new StringFormat()
                    {
                        Alignment = StringAlignment.Center
                    };

                    System.Drawing.Image imgg = System.Drawing.Image.FromFile(baseUserImg);
                    Color myRgbColor = Color.FromArgb(131, 132, 122); ;
                    using (Graphics graphics = Graphics.FromImage(imgg))

                    using (SolidBrush brush = new SolidBrush(myRgbColor))
                    using (Font fCalibri21 = new Font("Calibri", 150.0f, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        RectangleF textBoundary = new Rectangle(0, imgg.Height - 400, imgg.Width, 200);
                        graphics.DrawString(agendaItem.OrderNo + " " + agendaItem.Title, fCalibri21, brush, textBoundary, stringFormat);

                        graphics.Flush();

                        result = ImageToByteArray(imgg);
                    }

                }
                                           
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                    SlidingExpiration = TimeSpan.FromDays(1)
                };

                _cache.Set(imageKey, result, options);
            }
            return File(result, "image/jpeg");
        }

        [HttpGet("voting_slide_thumbnail")]
        public IActionResult PresentationVotingSlideThumbnail()
        {
            string imageKey = $"votingSlide.png";
            //byte[] result = null;

            if (!_cache.TryGetValue(imageKey, out byte[] result))
            {              

                var img = _config["Images:PresentationVotingSlideThumbnail"];
                var baseUserImg = Path.Combine(img);
                result = System.IO.File.ReadAllBytes(baseUserImg);               

                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                    SlidingExpiration = TimeSpan.FromDays(1)
                };

                _cache.Set(imageKey, result, options);
            }
            return File(result, "image/jpeg");
        }

        [HttpGet("unfinished_actions_slide_thumbnail")]
        public IActionResult UnfinishedActionsSlideThumbnail()
        {
            string imageKey = $"unfinishedActionsSlide.png";
            //byte[] result = null;

            if (!_cache.TryGetValue(imageKey, out byte[] result))
            {

                var img = _config["Images:UnfinishedActionsSlideThumbnail"];
                var baseUserImg = Path.Combine(img);
                result = System.IO.File.ReadAllBytes(baseUserImg);

                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                    SlidingExpiration = TimeSpan.FromDays(1)
                };

                _cache.Set(imageKey, result, options);
            }
            return File(result, "image/jpeg");
        }

        [HttpGet("previous_mom_slide_thumbnail")]
        public IActionResult PreviousMOMSlideThumbnail()
        {
            string imageKey = $"previousMOMSlide.png";
            //byte[] result = null;

            if (!_cache.TryGetValue(imageKey, out byte[] result))
            {

                var img = _config["Images:PreviousMOMSlideThumbnail"];
                var baseUserImg = Path.Combine(img);
                result = System.IO.File.ReadAllBytes(baseUserImg);

                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                    SlidingExpiration = TimeSpan.FromDays(1)
                };

                _cache.Set(imageKey, result, options);
            }
            return File(result, "image/jpeg");
        }

        [HttpGet("signature/{id}")]
        public IActionResult GetSignatureForUser(string id)
        {
            string imageKey = $"signature_{id}";                       
            var user = _userService.GetUserData(id).Result;
            byte[] image;
            if (user != null)
            {
                 image = DrawText(user.FullName);
                return File(image, "image/jpeg");
            }
            else
            {
                return null;
            }
        }

        private static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        private static byte[] DrawText(String name, Color? customTextColor = null, String customHeader = null)
        {
            byte[] result = null;

            try
            {
                StringFormat stringFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                var width = 250;
                var numberOfLines = 2;
                using (Font fCalibri21 = new Font("Calibri", 18.0f, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    do
                    {
                        width += 50;
                        numberOfLines = GetNumOfLines(name, width, fCalibri21);
                    }
                    while (numberOfLines > 1);
                }
                //hghgfh
                using (Bitmap img = new Bitmap(width + 40, 50))
                {
                    img.MakeTransparent();
                
                    RectangleF textBoundary = new Rectangle(0, 0, img.Width, img.Height - 20);
                    RectangleF textBoundary2 = new Rectangle(0, 0, img.Width, img.Height + 10);

                    Color myRgbColor = new Color();
                    if (customTextColor != null)
                    {
                        myRgbColor = (Color)customTextColor;

                    }
                    else
                    {
                        myRgbColor = Color.FromArgb(33, 164, 215);
                    }

                    using (Graphics graphics = Graphics.FromImage(img))

                    using (SolidBrush brush = new SolidBrush(myRgbColor))
                    using (Font fCalibri15 = new Font("Calibri", 15.0f, FontStyle.Bold, GraphicsUnit.Pixel))
                    using (Font fCalibri21 = new Font("Calibri", 20.0f, FontStyle.Bold, GraphicsUnit.Pixel))
                    {
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;


                        if (name != null)
                        {
                            if (customHeader != null)
                            {
                                graphics.DrawString(customHeader, fCalibri15, brush, textBoundary, stringFormat);
                            }
                            else
                            {
                                graphics.DrawString("Electronically signed by", fCalibri15, brush, textBoundary, stringFormat);
                            }
                        }
                        else
                        {
                            textBoundary = new Rectangle(0, 0, img.Width, img.Height + 10);
                            graphics.DrawString("Electronically signed", fCalibri21, brush, textBoundary, stringFormat);
                        }


                        if (name != null)
                        {
                            if (customHeader == "")
                            {
                                textBoundary2 = new Rectangle(0, 0, img.Width, img.Height);
                            }
                            graphics.DrawString(name, fCalibri21, brush, textBoundary2, stringFormat);

                        }

                        graphics.Flush();
                    }

                    //img = RotateImg(img, -1, Color.Transparent);

                    result = ImageToByteArray(img);
                }
            }
            catch
            {

            }

            return result;
        }

        private static int GetNumOfLines(string multiPageString, int wrapWidth, Font fnt)
        {
            StringFormat sfFmt = new StringFormat(StringFormatFlags.LineLimit);
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                int iHeight = (int)g.MeasureString(multiPageString, fnt, wrapWidth, sfFmt).Height;
                int iOneLineHeight = (int)g.MeasureString("Z", fnt, wrapWidth, sfFmt).Height;
                return (int)(Math.Round(System.Convert.ToDouble(iHeight) / System.Convert.ToDouble(iOneLineHeight)));
            }
        }
    }
}
