using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Models.Document;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using QRCoder;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Extensions
{
    public static class myQRCodeGenerator
    {
        public static byte[] GenerateQRCodeWithLogo(string url)
        {
            var qrCodeText = url;

            Image logo = Image.FromFile("images/mazaya_circle.png");
            Image resizedImg = logo.GetThumbnailImage(100, 100, null, IntPtr.Zero);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                qrCodeText,
                QRCodeGenerator.ECCLevel.H
            );
            QRCode code = new QRCode(qrCodeData);
            Bitmap qrCodeImg = code.GetGraphic(10);

            ImageConverter converter = new ImageConverter();
            byte[] imgData = (byte[])converter.ConvertTo(qrCodeImg, typeof(byte[]));

            int left = (qrCodeImg.Width / 2) - (resizedImg.Width / 2);
            int top = (qrCodeImg.Height / 2) - (resizedImg.Height / 2);
            Graphics g = Graphics.FromImage(qrCodeImg);
            g.DrawImage(resizedImg, new Point(left, top));
            imgData = (byte[])converter.ConvertTo(qrCodeImg, typeof(byte[]));
            return imgData;
        }

        public static byte[] createQRCodeWithBackgroun(byte[] data)
        {
            ImageConverter converter = new ImageConverter();
            Image back = Image.FromFile("images/background_a8.png");

            var ms = new MemoryStream(data);
            Image qr = Image.FromStream(ms);
            Image resizeQR = qr.GetThumbnailImage(157, 157, null, IntPtr.Zero);
            int top = back.Height / 4;
            int left = back.Width / 10;
            Size size = resizeQR.Size;

            Graphics g = Graphics.FromImage(back);
            g.DrawImage(resizeQR, new Point(left, top));

            return (byte[])converter.ConvertTo(back, typeof(byte[]));
        }

        public static byte[] CreatePdfsReturnArrayFromQRCode(byte[] data)
        {
            PdfDocument doc = new PdfDocument();
            PdfPage page = doc.AddPage();
            page.Width = 147;
            page.Height = 210;

            //page.Size = PdfSharp.PageSize.A5;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XImage img = XImage.FromStream(new MemoryStream(data));
            gfx.DrawImage(img, 0, 0);
            var PdfDoc = new MemoryStream();
            doc.Save(PdfDoc, false);
            return PdfDoc.ToArray();
        }

        public static async Task<DocumentFileModel> GenerateAndWriteQRCode(
            int offerId,
            string userId,
            MMADbContext context,
            string url,
            IDocumentService _documentService,
            string name
        )
        {
            var qrCodeID = Guid.NewGuid();
            var imgData = myQRCodeGenerator.GenerateQRCodeWithLogo(url);

            await _documentService.Upload(imgData.ToArray(), qrCodeID, name, "image/png", null);

            context.OfferDocument.Add(
                new OfferDocument()
                {
                    DocumentId = qrCodeID,
                    OriginalImageId = qrCodeID,
                    OfferId = offerId,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedOn = DateTime.UtcNow,
                    Type = OfferDocumentType.QRCode,
                    X1 = 0,
                    X2 = 0,
                    Y1 = 0,
                    Y2 = 0,
                    cropX1 = 0,
                    cropX2 = 0,
                    cropY1 = 0,
                    cropY2 = 0,
                }
            );

            await context.SaveChangesAsync();

            return new DocumentFileModel
            {
                Id = qrCodeID,
                MimeType = "image/png",
                Size = imgData.Length,
                Content = imgData
            };
        }
    }
}
