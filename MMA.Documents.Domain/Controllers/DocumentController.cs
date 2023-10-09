using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Helpers;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Models.Document;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _service;
        private readonly IConfiguration _configurationservice;
        private readonly IDocumentHelper _documentHelper;
        private readonly IImageUtilsService _imageUtilsService;

        public DocumentController(
            IDocumentService service,
            IDocumentHelper documentHelper,
            IConfiguration configurationservice,
            IImageUtilsService imageUtilsService
        )
        {
            _service = service;
            _configurationservice = configurationservice;
            _documentHelper = documentHelper;
            _imageUtilsService = imageUtilsService;
        }

        //[Authorize("Bearer")]
        [HttpPost("getlink")]
        public IActionResult GetLink()
        {
            Guid guid = _service.GetLink();
            string documentLink = _configurationservice
                .GetSection("Documents")
                .GetSection("DocumentsURL")
                .Value;

            var data = documentLink + "/" + guid;
            var json = JsonConvert.SerializeObject(data);
            return Ok(json);
        }

        [HttpPost("upload")]
        public IActionResult Upload()
        {
            Guid? parentId = null;
            DocumentFileModel documentFileModel = new DocumentFileModel();

            try
            {
                var files = Request.Form.Files;
                var file = files.FirstOrDefault();
                var notAllowedFileTypes = new List<string>
                {
                    "text/javascript",
                    "text/html",
                    "text/php"
                };

                // "5A-4D" and "4D-5A" are signatures for various versions of an .exe, .com, .dll, .sys, .scr, .vbx... files
                // "7B-5C" is signature for .rtf and .pwi files
                var notAllowedFileSignatures = new List<string> { "5A-4D", "4D-5A", "7B-5C" };

                var checkMagicNoBytes = "";

                if (files.Count() < 1)
                {
                    return BadRequest("Please upload files.");
                }
                else if (file.Length == 0)
                {
                    return BadRequest("Please upload file with content.");
                }
                else if (
                    file.ContentDisposition.Contains("image")
                    || file.ContentDisposition.Contains("video")
                )
                {
                    foreach (var item in files)
                    {
                        if (!notAllowedFileTypes.Contains(item.ContentType))
                        {
                            byte[] bytes = _documentHelper.GetBytes(item);

                            checkMagicNoBytes = _documentHelper.GetBytesForMagicNumberCheck(bytes);

                            if (bytes != null && bytes.LongLength > 0)
                            {
                                if (!notAllowedFileSignatures.Contains(checkMagicNoBytes))
                                {
                                    var result = _service.Upload(
                                        bytes,
                                        Guid.NewGuid(),
                                        item.FileName,
                                        item.ContentType,
                                        parentId
                                    );
                                    documentFileModel = result.Result;
                                }
                                else
                                {
                                    return BadRequest("Invalid media format.");
                                }
                            }
                        }
                    }
                    return Ok(new { id = documentFileModel.Id, });
                }
                else
                {
                    foreach (var item in files)
                    {
                        if (!notAllowedFileTypes.Contains(item.ContentType))
                        {
                            int TenMegaBytes = 10 * 1024 * 1024;
                            byte[] bytes = _documentHelper.GetBytes(item);
                            checkMagicNoBytes = _documentHelper.GetBytesForMagicNumberCheck(bytes);

                            if (item.Length < TenMegaBytes)
                            {
                                if (!notAllowedFileSignatures.Contains(checkMagicNoBytes))
                                {
                                    //original
                                    var result = _service.Upload(
                                        bytes,
                                        Guid.NewGuid(),
                                        item.FileName,
                                        item.ContentType,
                                        parentId
                                    );
                                    documentFileModel = result.Result;
                                }
                                else
                                {
                                    return BadRequest("Invalid file format.");
                                }
                            }
                            else
                            {
                                throw new Exception("Please upload file that is less than 10 MB.");
                            }
                        }
                        else
                        {
                            return BadRequest("Invalid file type.");
                        }
                    }
                    return Ok(new { id = documentFileModel.Id, });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// returns file
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        [HttpGet("{guid}")]
        public async Task<IActionResult> Download(Guid guid)
        {
            if (guid == null)
                return Content("file not present");

            var result = await _service.Download(guid);

            if (result.Content == null)
                return Content("Invalid file.");

            var file = File(result.Content, result.MimeType, result.Name);

            return await Task.FromResult(file);
        }

        /// <summary>
        /// Check for document type, some attachments after migration got default type
        /// Attachments with defualt type couldn't be opened
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns>If document has default type or text plain pdf is returned</returns>
        private string GetCorrectMimeType(string mimeType, string name)
        {
            if (
                string.IsNullOrWhiteSpace(mimeType)
                || mimeType == "application/octet-stream"
                || mimeType == "text/plain"
            )
            {
                if (name.Contains(".png"))
                    return "image/png";

                if (name.Contains(".jpg") || name.Contains(".jpeg"))
                    return "image/jpeg";

                return "application/pdf";
            }

            return mimeType;
        }

        [HttpDelete("{guid}")]
        public async Task<IActionResult> Delete(Guid guid)
        {
            if (guid == null)
                return Content("File not present");

            await _service.Delete(guid);

            return Content("File deleted.");
        }

        [HttpGet("{guid}/svg")]
        public async Task<IActionResult> DownloadAsSVGString(Guid guid)
        {
            if (guid == null)
                return Content("file not present");

            var result = await _service.Download(guid);

            if (result.Content == null)
                return Content("Invalid file.");

            string converted = Encoding.UTF8.GetString(result.Content, 0, result.Content.Length);

            return await Task.FromResult(Content(converted));
        }
    }
}
