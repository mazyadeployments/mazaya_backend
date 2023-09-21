using Hangfire;
using ImageMagick;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Core.Services
{
    public class ImageUtilsService : IImageUtilsService
    {
        private readonly IDocumentService _documentService;

        public ImageUtilsService(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public Size GetImageSize(byte[] data)
        {
            Size result;

            using (var originalImage = new MagickImage(data))
            {
                result = new Size(originalImage.Width, originalImage.Height);
            }

            return result;
        }

        public async Task<List<ImageModel>> PrepareImagesForUpload(List<ImageModel> images)
        {
            var croppedImages = new List<ImageModel>();
            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                {
                    image.Type = OfferDocumentType.Original;
                    image.OriginalImageId = Guid.Parse(image.Id);

                    croppedImages.Add(image);

                    // Thumbnail IMG
                    var thumbnailIMGGuid = Guid.NewGuid();
                    croppedImages.Add(
                        new ImageModel
                        {
                            Id = thumbnailIMGGuid.ToString(),
                            Type = OfferDocumentType.Thumbnail,
                            OriginalImageId = image.OriginalImageId,
                            CropCoordinates = new CropCoordinates()
                            {
                                X1 = 0,
                                X2 = 0,
                                Y1 = 0,
                                Y2 = 0
                            },
                            CropNGXCoordinates = new CropCoordinates()
                            {
                                X1 = 0,
                                X2 = 0,
                                Y1 = 0,
                                Y2 = 0
                            },
                            Cover = image.Cover
                        }
                    );

                    // Details IMG
                    var detailsIMGGuid = Guid.NewGuid();
                    croppedImages.Add(
                        new ImageModel
                        {
                            Id = detailsIMGGuid.ToString(),
                            Type = OfferDocumentType.Large,
                            OriginalImageId = image.OriginalImageId,
                            CropCoordinates = new CropCoordinates()
                            {
                                X1 = 0,
                                X2 = 0,
                                Y1 = 0,
                                Y2 = 0
                            },
                            CropNGXCoordinates = new CropCoordinates()
                            {
                                X1 = 0,
                                X2 = 0,
                                Y1 = 0,
                                Y2 = 0
                            },
                            Cover = image.Cover
                        }
                    );
                }
            }
            return croppedImages;
        }

        public byte[] ProcessImage(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new NullImageException();
            }

            byte[] result;

            using (var source = new MagickImage(data))
            using (
                var target = new MagickImage(
                    MagickColors.Transparent,
                    Math.Max(source.Width, source.Height),
                    Math.Max(source.Width, source.Height)
                )
            )
            {
                var originalWidth = source.Width;
                var originalHeight = source.Height;

                if (originalWidth == 0 || originalHeight == 0)
                {
                    throw new NullImageException();
                }

                // set format to png
                if (source.Format != MagickFormat.Png)
                    source.Format = MagickFormat.Png;

                if (target.Format != MagickFormat.Png)
                    target.Format = MagickFormat.Png;

                var xPosition = target.Width / 2 - source.Width / 2;
                var yPosition = target.Height / 2 - source.Height / 2;

                target.Draw(
                    new DrawableComposite(xPosition, yPosition, CompositeOperator.Over, source)
                );

                result = target.ToByteArray();
            }

            return result;
        }

        public byte[] ProcessImageForOffer(
            byte[] data,
            ImageBackground backgroundType = ImageBackground.Blur,
            string htmlBackgroundColor = "#DCE2E8",
            int? overrideRatio = null,
            bool blur = true,
            int? maxSize = null
        )
        {
            if (data == null || data.Length == 0)
            {
                throw new NullImageException();
            }

            var ratio = overrideRatio ?? 1.5;

            var defaultBackgroundColor = new MagickColor(htmlBackgroundColor ?? "#DCE2E8");

            byte[] result;

            using (var source = new MagickImage(data))
            using (
                var background =
                    backgroundType == ImageBackground.Blur
                        ? new MagickImage(source)
                        : new MagickImage(defaultBackgroundColor, source.Width, source.Height)
            )
            using (
                var target = new MagickImage(defaultBackgroundColor, source.Width, source.Height)
            )
            {
                if (maxSize.HasValue)
                {
                    PreProcessResizeMagickImage(maxSize, source);
                }

                var originalWidth = source.Width;
                var originalHeight = source.Height;

                if (originalWidth == 0 || originalHeight == 0)
                {
                    throw new NullImageException();
                }

                // set format to jpg
                source.Format = target.Format = background.Format = MagickFormat.Jpeg;

                // resize to fit width
                if ((int)(originalHeight * ratio) > originalWidth)
                {
                    background.Resize(
                        new MagickGeometry()
                        {
                            IgnoreAspectRatio = true,
                            Width = (int)(ratio * originalHeight),
                            Height = originalHeight
                        }
                    );
                }
                else
                {
                    background.Resize(
                        new MagickGeometry()
                        {
                            IgnoreAspectRatio = true,
                            Width = originalWidth,
                            Height = (int)(originalWidth / ratio)
                        }
                    );
                }

                // resize target to fit width
                target.Resize(
                    new MagickGeometry()
                    {
                        IgnoreAspectRatio = true,
                        Width = background.Width,
                        Height = background.Height
                    }
                );

                // blur the background image
                // background.RotationalBlur(10);
                if (backgroundType == ImageBackground.Blur)
                {
                    background.Blur(0, 48);
                }

                // add background to white image with 0.25 opacity
                if (blur)
                {
                    target.Composite(background, 0, 0, CompositeOperator.Dissolve, "75");
                }

                // add source image on center

                target.Draw(
                    new DrawableComposite(
                        (target.Width - (double)source.Width) / 2,
                        (target.Height - (double)source.Height) / 2,
                        source
                    )
                );

                //if (source.Height > source.Width)
                //{
                //    target.Draw(new DrawableComposite((target.Width - (double)source.Width) / 2, 0, source));
                //}
                //else
                //{
                //    target.Draw(new DrawableComposite(0, ((target.Height - (double)source.Height) / 2), 0, source));
                //}

                result = target.ToByteArray();
            }

            return result;
        }

        public byte[] ProcessImageForCollection(
            byte[] data,
            ImageBackground backgroundType = ImageBackground.Blur,
            string htmlBackgroundColor = "#DCE2E8",
            int? overrideRatio = null,
            bool blur = true,
            int? maxSize = null
        )
        {
            if (data == null || data.Length == 0)
            {
                throw new NullImageException();
            }

            var ratio = overrideRatio ?? 1.25;

            var defaultBackgroundColor = new MagickColor(htmlBackgroundColor ?? "#DCE2E8");

            byte[] result;

            using (var source = new MagickImage(data))
            using (
                var background =
                    backgroundType == ImageBackground.Blur
                        ? new MagickImage(source)
                        : new MagickImage(defaultBackgroundColor, source.Width, source.Height)
            )
            using (
                var target = new MagickImage(defaultBackgroundColor, source.Width, source.Height)
            )
            {
                if (maxSize.HasValue)
                {
                    PreProcessResizeMagickImage(maxSize, source);
                }

                var originalWidth = source.Width;
                var originalHeight = source.Height;

                if (originalWidth == 0 || originalHeight == 0)
                {
                    throw new NullImageException();
                }

                // set format to jpg
                source.Format = target.Format = background.Format = MagickFormat.Jpeg;

                // resize to fit width
                if (originalHeight > originalWidth)
                {
                    background.Resize(
                        new MagickGeometry()
                        {
                            IgnoreAspectRatio = true,
                            Width = (int)(ratio * originalHeight),
                            Height = originalHeight
                        }
                    );
                }
                else
                {
                    background.Resize(
                        new MagickGeometry()
                        {
                            IgnoreAspectRatio = true,
                            Width = (originalWidth),
                            Height = (int)(originalWidth / ratio)
                        }
                    );
                }

                // resize target to fit width
                target.Resize(
                    new MagickGeometry()
                    {
                        IgnoreAspectRatio = true,
                        Width = background.Width,
                        Height = background.Height
                    }
                );

                // blur the background image
                // background.RotationalBlur(10);
                if (backgroundType == ImageBackground.Blur)
                {
                    background.Blur(0, 48);
                }

                // add background to white image with 0.25 opacity
                if (blur)
                {
                    target.Composite(background, 0, 0, CompositeOperator.Dissolve, "75");
                }

                // add source image on center
                if (source.Height > source.Width)
                {
                    target.Draw(
                        new DrawableComposite((target.Width - (double)source.Width) / 2, 0, source)
                    );
                }
                else
                {
                    target.Draw(
                        new DrawableComposite(
                            0,
                            ((target.Height - (double)source.Height) / 2),
                            0,
                            source
                        )
                    );
                }

                result = target.ToByteArray();
            }

            return result;
        }

        public byte[] ProcessImageForCategory(
            byte[] data,
            ImageBackground backgroundType = ImageBackground.Blur,
            string htmlBackgroundColor = "#DCE2E8",
            int? overrideRatio = null,
            bool blur = true,
            int? maxSize = null
        )
        {
            if (data == null || data.Length == 0)
            {
                throw new NullImageException();
            }

            var ratio = overrideRatio ?? 1;

            var defaultBackgroundColor = new MagickColor(htmlBackgroundColor ?? "#DCE2E8");

            byte[] result;

            using (var source = new MagickImage(data))
            using (
                var background =
                    backgroundType == ImageBackground.Blur
                        ? new MagickImage(source)
                        : new MagickImage(defaultBackgroundColor, source.Width, source.Height)
            )
            using (
                var target = new MagickImage(defaultBackgroundColor, source.Width, source.Height)
            )
            {
                if (maxSize.HasValue)
                {
                    PreProcessResizeMagickImage(maxSize, source);
                }

                var originalWidth = source.Width;
                var originalHeight = source.Height;

                if (originalWidth == 0 || originalHeight == 0)
                {
                    throw new NullImageException();
                }

                // set format to jpg
                source.Format = target.Format = background.Format = MagickFormat.Jpeg;

                // resize to fit width
                if (originalHeight > originalWidth)
                {
                    background.Resize(
                        new MagickGeometry()
                        {
                            IgnoreAspectRatio = true,
                            Width = (int)(ratio * originalHeight),
                            Height = originalHeight
                        }
                    );
                }
                else
                {
                    background.Resize(
                        new MagickGeometry()
                        {
                            IgnoreAspectRatio = true,
                            Width = (originalWidth),
                            Height = (int)(originalWidth / ratio)
                        }
                    );
                }

                // resize target to fit width
                target.Resize(
                    new MagickGeometry()
                    {
                        IgnoreAspectRatio = true,
                        Width = background.Width,
                        Height = background.Height
                    }
                );

                // blur the background image
                // background.RotationalBlur(10);
                if (backgroundType == ImageBackground.Blur)
                {
                    background.Blur(0, 48);
                }

                // add background to white image with 0.25 opacity
                if (blur)
                {
                    target.Composite(background, 0, 0, CompositeOperator.Dissolve, "75");
                }

                // add source image on center
                if (source.Height > source.Width)
                {
                    target.Draw(
                        new DrawableComposite((target.Width - (double)source.Width) / 2, 0, source)
                    );
                }
                else
                {
                    target.Draw(
                        new DrawableComposite(
                            0,
                            ((target.Height - (double)source.Height) / 2),
                            0,
                            source
                        )
                    );
                }

                result = target.ToByteArray();
            }

            return result;
        }

        public byte[] Resize(byte[] source, int maxSize)
        {
            byte[] result = null;

            using (var image = new MagickImage(source))
            {
                int width = image.Width;
                int height = image.Height;
                double aspectRatio = (double)width / (double)height;

                if (width > maxSize)
                {
                    width = maxSize;
                    height = (int)(width / aspectRatio);
                }
                else if (height > maxSize)
                {
                    height = maxSize;
                    width = (int)(height * aspectRatio);
                }

                var geometry = new MagickGeometry
                {
                    FillArea = true,
                    IgnoreAspectRatio = false,
                    Width = width,
                    Height = height
                };

                image.Resize(geometry);

                result = image.ToByteArray();
            }

            return result;
        }

        public byte[] Resize(byte[] source, int width, int height)
        {
            byte[] result = null;

            using (var image = new MagickImage(source))
            {
                var geometry = new MagickGeometry
                {
                    FillArea = true,
                    IgnoreAspectRatio = false,
                    Width = width,
                    Height = height
                };

                image.Resize(geometry);

                result = image.ToByteArray();
            }

            return result;
        }

        public byte[] CropImage(byte[] source, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            byte[] result = null;

            using (var image = new MagickImage(source))
            {
                var geometry = new MagickGeometry
                {
                    X = cropX,
                    Y = cropY,
                    Width = cropWidth,
                    Height = cropHeight,
                    IgnoreAspectRatio = true
                };

                image.Crop(geometry);

                result = image.ToByteArray();
            }

            return result;
        }

        /// <summary>
        /// Upload of images will be run in background (Max automatic retry is set to 0)
        /// </summary>
        /// <param name="croppedImages"></param>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 0)]
        public async Task CreateImages(
            List<ImageModel> croppedImages,
            Declares.ImageForType imageForType
        )
        {
            if (croppedImages != null && croppedImages.Count > 0)
            {
                var groupedImages = croppedImages.GroupBy((x => x.OriginalImageId));
                foreach (var images in groupedImages)
                {
                    var originalImg = images.FirstOrDefault(
                        img => img.Type == OfferDocumentType.Original
                    );
                    var largeImg = images.FirstOrDefault(
                        img => img.Type == OfferDocumentType.Large
                    );
                    var thumbnailImg = images.FirstOrDefault(
                        img => img.Type == OfferDocumentType.Thumbnail
                    );

                    var widthThumbnail = 0;
                    var heightThumbnail = 0;
                    var widthLarge = 0;
                    var heightLarge = 0;

                    var file = await _documentService.Download(originalImg.OriginalImageId);

                    byte[] bytes = file.Content;
                    byte[] processedImage;

                    switch (imageForType)
                    {
                        case Declares.ImageForType.Offer:
                            processedImage = ProcessImageForOffer(bytes);
                            widthThumbnail = 351;
                            heightThumbnail = 234;
                            widthLarge = 801;
                            heightLarge = 534;
                            break;
                        case Declares.ImageForType.Category:
                            processedImage = ProcessImageForCategory(bytes);
                            widthThumbnail = 100;
                            heightThumbnail = 100;
                            widthLarge = 200;
                            heightLarge = 200;
                            break;
                        case Declares.ImageForType.Collection:
                            processedImage = ProcessImageForCollection(bytes);
                            widthThumbnail = 300;
                            heightThumbnail = 240;
                            widthLarge = 650;
                            heightLarge = 520;
                            break;
                        case Declares.ImageForType.Company:
                            processedImage = ProcessImageForCategory(bytes);
                            widthThumbnail = 100;
                            heightThumbnail = 100;
                            widthLarge = 100;
                            heightLarge = 100;
                            break;
                        case Declares.ImageForType.User:
                            processedImage = ProcessImageForCategory(bytes);
                            widthThumbnail = 200;
                            heightThumbnail = 200;
                            widthLarge = 200;
                            heightLarge = 200;
                            break;
                        default:
                            processedImage = ProcessImageForCategory(bytes);
                            widthThumbnail = 200;
                            heightThumbnail = 200;
                            widthLarge = 200;
                            heightLarge = 200;
                            break;
                    }

                    var croppedImage =
                        originalImg.CropCoordinates != null
                            ? CropImage(
                                processedImage,
                                Convert.ToInt32(originalImg.CropCoordinates.X1),
                                Convert.ToInt32(originalImg.CropCoordinates.Y1),
                                Convert.ToInt32(
                                    originalImg.CropCoordinates.X2 - originalImg.CropCoordinates.X1
                                ),
                                Convert.ToInt32(
                                    originalImg.CropCoordinates.Y2 - originalImg.CropCoordinates.Y1
                                )
                            )
                            : processedImage;

                    if (croppedImage != null && croppedImage.LongLength > 0)
                    {
                        //Create Large Image
                        await CreateLargeImage(
                            largeImg,
                            null,
                            file,
                            croppedImage,
                            widthLarge,
                            heightLarge
                        );

                        //Create Thumbnail Image
                        await CreateThumbnailImage(
                            thumbnailImg,
                            null,
                            file,
                            croppedImage,
                            widthThumbnail,
                            heightThumbnail
                        );
                    }
                }
            }
        }

        private async Task<DocumentFileModel> CreateLargeImage(
            ImageModel image,
            Guid? parentId,
            DocumentFileModel file,
            byte[] croppedImage,
            int width,
            int height
        )
        {
            var imageDetails = Resize(croppedImage, width, height);
            var resultDetails = await _documentService.Upload(
                imageDetails,
                Guid.Parse(image.Id),
                file.Name,
                file.MimeType,
                parentId
            );

            return resultDetails;
        }

        private async Task<DocumentFileModel> CreateThumbnailImage(
            ImageModel image,
            Guid? parentId,
            DocumentFileModel file,
            byte[] croppedImage,
            int width,
            int height
        )
        {
            var imageThumbnail = Resize(croppedImage, width, height);
            var resultThumbnail = await _documentService.Upload(
                imageThumbnail,
                Guid.Parse(image.Id),
                file.Name,
                file.MimeType,
                parentId
            );

            return resultThumbnail;
        }

        private static void PreProcessResizeMagickImage(int? maxSize, MagickImage sourceref)
        {
            int width = sourceref.Width;
            int height = sourceref.Height;
            double aspectRatio = (double)width / (double)height;

            if (width > maxSize.Value)
            {
                width = maxSize.Value;
                height = (int)(width / aspectRatio);
            }
            else if (height > maxSize.Value)
            {
                height = maxSize.Value;
                width = (int)(height * aspectRatio);
            }

            var geometry = new MagickGeometry
            {
                FillArea = true,
                IgnoreAspectRatio = false,
                Width = width,
                Height = height
            };

            sourceref.Resize(geometry);
        }

        public class NullImageException : Exception
        {
            public NullImageException()
                : base("Image has no content", null) { }
        }
    }
}
