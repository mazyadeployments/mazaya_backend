using Microsoft.AspNetCore.Http;
using MMA.WebApi.Shared.Interfaces.Helpers;
using System;
using System.IO;
using System.Linq;

namespace MMA.WebApi.Helpers
{
    public class DocumentHelper : IDocumentHelper
    {
        public byte[] GetBytes(IFormFile file)
        {
            using (var reader = new MemoryStream())
            {
                Stream stream = file.OpenReadStream();
                stream.CopyTo(reader);
                var fileContentByte = reader.ToArray();

                return fileContentByte;
            }
        }

        public string GetBytesForMagicNumberCheck(byte[] bytes)
        {
            var checkMagicNoBytes = "";
            var checkBytes = new byte[4];
            checkBytes = bytes.Skip(0).Take(2).ToArray();
            checkMagicNoBytes = BitConverter.ToString(checkBytes);

            return checkMagicNoBytes;
        }
    }
}
