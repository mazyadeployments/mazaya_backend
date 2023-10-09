using ExpertPdf.HtmlToPdf;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Models.PDFModel;
using MMA.WebApi.Shared.Models.Roadshow;
using System;
using System.IO;
using System.Web;

namespace MMA.WebApi.Helpers
{
    public class ProposalPDFGenerator
    {
        public static PDFModel GenerateProposalHtml(RoadshowProposalModel proposal)  //public static string
        {
            PDFModel proposalHTML = new PDFModel();
            var engine = new RazorLight.RazorLightEngineBuilder()
              .UseEmbeddedResourcesProject(typeof(Program))
              .UseMemoryCachingProvider()
              .Build();

            var spm = GenerateProposal(proposal, engine);
            var htmlDecode = HttpUtility.HtmlDecode(spm.Body);
            spm.Body = htmlDecode;
            proposalHTML = spm;

            return proposalHTML;
        }
        public static Byte[] GenerateProposalPDF(RoadshowProposalModel proposal, IConfiguration configuration)
        {
            var engine = new RazorLight.RazorLightEngineBuilder()
              .UseEmbeddedResourcesProject(typeof(Program))
              .UseMemoryCachingProvider()
              .Build();

            var spm = GenerateProposal(proposal, engine);
            var htmlDecode = HttpUtility.HtmlDecode(spm.Body);
            spm.Body = htmlDecode;
            var file = HtmlToPDF(htmlDecode, configuration);

            return file;
        }

        private static Byte[] HtmlToPDF(string html, IConfiguration configuration)
        {
            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.LicenseKey = configuration.GetSection("ExpertPdfLicenseKey").Value;
            pdfConverter.RenderingEngine = RenderingEngine.WebKit2;

            pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            pdfConverter.PdfDocumentOptions.ShowHeader = true;
            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            pdfConverter.PdfDocumentOptions.LeftMargin = 40;
            pdfConverter.PdfDocumentOptions.RightMargin = 40;
            pdfConverter.PdfDocumentOptions.TopMargin = 40;
            pdfConverter.PdfDocumentOptions.BottomMargin = 40;
            pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;

            pdfConverter.PdfDocumentOptions.ShowHeader = false;
            pdfConverter.PdfFooterOptions.DrawFooterLine = false;
            pdfConverter.PdfFooterOptions.PageNumberText = "";
            pdfConverter.PdfFooterOptions.PageNumberTextFontType = PdfFontType.Helvetica;
            pdfConverter.PdfFooterOptions.ShowPageNumber = true;

            byte[] downloadBytes = pdfConverter.GetPdfBytesFromHtmlString(html);

            return downloadBytes;
        }

        private static PDFModel GenerateProposal(RoadshowProposalModel proposal, RazorLight.RazorLightEngine engine, string formImageSVG = "", string formImagePDF = "")
        {
            string result = ReadHtml(engine, proposal, @".\Reports\ProposalInfo.cshtml");
            PDFModel spm = new PDFModel()
            {
                Id = proposal.Id,
                Body = result
            };

            return spm;
        }

        private static string ReadHtml(RazorLight.RazorLightEngine engine, object model, string path)
        {
            var templatePath = Path.GetFullPath(path);
            var template = File.ReadAllText(templatePath);
            return engine.CompileRenderStringAsync("templateKey" + path, template, model).Result;
        }
    }
}
