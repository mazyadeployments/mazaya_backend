using Microsoft.AspNetCore.Http;
using System;

namespace MMA.WebApi.Shared.Models.Document
{
    public class UploadPPTX
    {
        public IFormFile files { get; set; }
        public Guid guid { get; set; }
        public int agendaItemId { get; set; }
    }
}
