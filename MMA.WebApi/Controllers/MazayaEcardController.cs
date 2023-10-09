using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Shared.Interfaces.MazayaEcardetails;
using MMA.WebApi.Shared.Interfaces.MazayaEcardmain;
using MMA.WebApi.Shared.Interfaces.MazayaSubCategory;
using MMA.WebApi.Shared.Interfaces.Roles;
using Microsoft.AspNetCore.Authorization;
using MMA.WebApi.Core.Services;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Monads;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using MMA.WebApi.Shared.Models.MazayaEcardmain;
using MMA.WebApi.Models;
using Spire.Pdf.Exporting.XPS.Schema;
using MMA.WebApi.Shared.Models.MazayaPaymentgateway;
using MMA.WebApi.Shared.Interfaces.MazayaPaymentgateway;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Models.MazayaSubCategory;
using System.Linq;
using MMA.WebApi.Shared.Models.MazayaEcarddetail;
using Hangfire.Storage.Monitoring;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Net.Mail;
using System.Text;
using iTextSharp.text;


namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class MazayaEcardController : BaseController
    {
        private readonly IMazayaEcardmainService _mazayaecardmainService;
        private readonly IRoleService _roleService;
        public IWebHostEnvironment webHostEnvironment;
        private readonly IMazayaEcarddetailsService _mazayaecarddetailsService;
        private readonly IMazayaPaymentgatewayService _paymentgatewayService;
        private readonly IMazayaSubCategoryService _mazayasubcategoryService;
        public MazayaEcardController(IMazayaEcardmainService mazayaecardmainService, IRoleService roleService, IWebHostEnvironment webHostEnvironment, IMazayaEcarddetailsService mazayaEcarddetailsService, IMazayaPaymentgatewayService mazayaPaymentgatewayService, IMazayaSubCategoryService mazayasubcategoryService)
        {
            _mazayaecardmainService = mazayaecardmainService;
            this.webHostEnvironment = webHostEnvironment;
            _roleService = roleService;
            _mazayaecarddetailsService = mazayaEcarddetailsService;
            _paymentgatewayService = mazayaPaymentgatewayService;
            _mazayasubcategoryService = mazayasubcategoryService;
        }

        [HttpPost("ecardcreate")]
        public async Task<IActionResult> CreateOrUpdate(MazayaEcardModel mazayaecard)
        {
            try
            {
                int transaction_id = 0;
                if (mazayaecard.mazayaecardmain != null && mazayaecard.mazayaecarddetails.Count != 0)
                {
                    var mainresult = await _mazayaecardmainService.CreateOrUpdateAsync(mazayaecard.mazayaecardmain, UserId);
                    if (mainresult is Maybe<MazayaEcardmainModel>.None)
                        return NotFound();
                    transaction_id = mainresult.Value.id;

                    //string[] resultArray = mainresult.Value.subcategoryids.Split(',');
                    //foreach (var id in resultArray)
                    //{
                    //    int sub_id = int.Parse(id);
                    //    var obj = await _mazayasubcategoryService.GetMazayasubCategories();
                    //    MazayaSubCategoryModel subcat = obj.Where(x => x.Id == sub_id).FirstOrDefault();
                    //    if (subcat.optiontype == "both")
                    //    {
                    //        subcat.totalcount = mazayaecard.additional_count;
                    //        var subresult = await _mazayasubcategoryService.UpdateCountAsync(subcat, UserId);
                    //    }
                    //}
                    foreach (var details in mazayaecard.mazayaecarddetails)
                    {
                        details.MazayaEcardmainId = mainresult.Value.id;
                        var detailsresult = await _mazayaecarddetailsService.CreateOrUpdateAsync(details, UserId);
                    }
                }
                return Ok(ApiResponse<int>.Response($"transaction_id", transaction_id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }

        [HttpPost("paymentstatus")]
        public async Task<IActionResult> PaymentStatus(MazayaPaymentgatewayModel paymentgateway, int transaction_id, bool status)
        {
            try
            {
                if (paymentgateway != null && transaction_id != 0)
                {
                    if (status == true)
                    {
                        var paymentresult = await _paymentgatewayService.CreateOrUpdateAsync(paymentgateway, UserId);

                        if (paymentresult is Maybe<MazayaPaymentgatewayModel>.None)
                            return NotFound();
                        MazayaEcardmainModel obj = new MazayaEcardmainModel();
                        obj.id = transaction_id;
                        var mainresult = await _mazayaecardmainService.CreateOrUpdateAsync(obj, UserId);
                        var ecarddetailslst = await _mazayaecarddetailsService.GetMazayaEcarddetails();
                        List<MazayaEcarddetailsModel> sublist = ecarddetailslst.Where(x => x.MazayaEcardmainId == transaction_id).ToList();
                        MazayaEcardModel ecardresponse = new MazayaEcardModel();
                        ecardresponse.mazayaecardmain = mainresult.Value;
                        ecardresponse.mazayaecarddetails = sublist;
                        //foreach (var item in sublist)
                        //{
                        //    var detailsresult = await _mazayaecarddetailsService.CreateOrUpdateAsync(item, UserId);
                        //}
                        var response =  pgfgenerate(ecardresponse);
                        return Ok(ApiResponse<MazayaEcardModel>.Response($"Ecard response", ecardresponse));
                    }
                    else
                    {
                        MazayaEcardModel ecardresponse = new MazayaEcardModel();
                        return Ok(ApiResponse<MazayaEcardModel>.Response($"Ecard response", ecardresponse));
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }

        [HttpGet("pgfgenerate")]
        public async Task<IActionResult> pgfgenerate([FromForm] MazayaEcardModel model)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<!DOCTYPE html>");
                sb.Append("<html>");
                sb.Append("<body>");

                sb.Append("<div style='max-width: 890px;margin: auto;padding: 30px 50px;border: 1px solid #eee;font-size: 14px;line-height: 24px;font-family: 'poppins',  sans-serif;color: #555;width: 100%;'>");

                sb.Append("<table style='border-bottom: 1px solid #707070;'>");
                sb.Append("<tr class='top_rw'>");
                sb.Append("<td colspan='2'>");
                sb.Append("<h2 style='margin-bottom: 40px; font-size: 35px; color: #00142A; font-weight: 600; margin-top: 20px;' >Tax Invoice </h2>");
                sb.Append("<span style='font-size: 20px; color: #00142A; font-weight: 600;'>Invoice No: #SM75692</span>");
                sb.Append("</td>");
                sb.Append("<td  style='width:30%; margin-right: 10px;'>");
                //sb.Append("<img src='./logo-img.svg' style='width:80px; height: 80px;'>");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");


                sb.Append("<table>");
                sb.Append("<tr>");
                sb.Append("<td colspan='2' style='color: #797979; font-weight: 500;'>");
                sb.Append("<span style='font-size: 15px; color: #00142A; font-weight: 600;'>Invoice / Receipt</span>");
                sb.Append("<br> Invoice : #SM75692 <br>");
                sb.Append("Customer: #245134256<br>");
                sb.Append("VAT Reg. No: 1000000009785000003<br>");
                sb.Append("Issue Date: 16 SEP 2023");
                sb.Append("</td>");
                sb.Append("<td style='color: #797979; font-weight: 500;'> ");
                sb.Append("<span style='font-size: 15px; color: #00142A; font-weight: 600;'>Nirvana Travel &Tourism</span>");
                sb.Append("<br> Breakwater, Opposite of Abu Dhabi International <br>");
                sb.Append("Marine Sports Club - 27 Al ‘Alam St <br>");
                sb.Append("Al Marina - Abu Dhabi, United Arab Emirates <br>");
                sb.Append("example@nirvana.ae");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");



                sb.Append("<table cellspacing='0' cellpadding='2'>");
                sb.Append("<tr>");
                sb.Append("<td style='border: 1px solid #707070; padding: 25px; border-radius: 10px;' width='50%'>");
                sb.Append("<b style='font-size: 15px; color: #00142A; font-weight: 600;'> Bill To: </b> <br>");
                sb.Append("Azima Chhuvara <br>");
                sb.Append("Breakwater, Opposite of Abu Dhabi International <br>");
                sb.Append("Marine Sports Club - 27 Al ‘Alam St <br>");
                sb.Append("Al Marina - Abu Dhabi <br>");
                sb.Append("United Arab Emirates");
                sb.Append("</td>");
                sb.Append("<td style=></td>");
                sb.Append("<td style='border: 1px solid #707070; padding: 25px; border-radius: 10px;' width='50%'>");
                sb.Append("<b style='font-size: 15px; color: #00142A; font-weight: 600;'> Payment Information: </b> <br>");
                sb.Append("Azima Chhuvara <br>");
                sb.Append("Credit Card: *************456<br>");
                sb.Append("Paid: AED 3,628.00");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");


                sb.Append("<table cellspacing='0' cellpadding='2' style='border: 1px solid #707070; margin-top: 20px; border-radius: 10px;'>");
                sb.Append("thead");
                sb.Append("<tr>");
                sb.Append("<td style='width:42%; font-size: 15px; color: #00142A; font-weight: 600; border-radius: 10px 0 0 0;'>Items Details</td>");
                sb.Append("<td style='width:20%; text-align:center; font-size: 15px; color: #00142A; font-weight: 600;'>Price<td>");
                sb.Append("<td style='width:20%; text-align:center; font-size: 15px; color: #00142A; font-weight: 600;'>VAT</td>");
                sb.Append("<td style='width:25%; text-align:right; font-size: 15px; color: #00142A; font-weight: 600; border-radius: 0px 10px 0px 0px;'>Total</td>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append("<tbody>");
                sb.Append("<tr>");
                sb.Append("<td style='width:25%;'>Membership - Leisure & Family Entertainment - FAMILY MEMBERSHIP</td>");
                sb.Append("<td style='width:10%; text-align:center;'>2,500.00 AED</td>");
                sb.Append("<td style='width:10%; text-align:center;'>2,500.00 AED</td>");
                sb.Append("<td style='width:15%; text-align:right;'>2,591.25 AED</td>");
                sb.Append("</tr>");
                sb.Append("</tbody>");
                sb.Append("</table>");


                sb.Append("<table style='margin-top: 20px;'>");
                sb.Append("<tr class='total'>");
                sb.Append("<td style='width:70%;' align='right'>");
                sb.Append("<span style='color: #00142A; font-weight: 600; font-size: 18px;'>Total Amount : </span> ");
                sb.Append("</td style='width:30%;'>");
                sb.Append("<td>   <b style='color: #00142A;font-weight: 600; font-size: 20px;'> 3,628.00 AED</b></td>");
                sb.Append("</tr>");
                sb.Append("</table>");




                sb.Append("<table style='margin-top: 250px; border-top: 1px solid #707070;'>");
                sb.Append("<tr>");
                sb.Append("<td style='text-align:center; color: #00142A; font-weight: 600; font-size: 16px;'>Thank you for the Order</td>");
                sb.Append("</tr>");
                sb.Append("</table>");


                sb.Append("</div>");
                sb.Append("</body>");
                sb.Append("</html>");


                byte hex = 0xA1;
                byte[] bytes = new byte[] { 0x00, 0x21, 0x60, 0x1F, 0xA1, hex };

                StringReader sr = new StringReader(sb.ToString());
                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    bytes = memoryStream.ToArray();
                    memoryStream.Close();
                }




                string fromEmail = "portal@ntravel.ae";
                string smtpServer = "smtp.office365.com";
                string smtpUsername = "portal@ntravel.ae";
                string smtpPassword = "Ntt@2018$#";
                int smtpPort = 587;
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    MailMessage mm = new MailMessage();
                    mm.To.Add("sruthy.ravi@gewaninfotech.com");
                    mm.From = new MailAddress(fromEmail);
                    mm.Subject = "Mazaya Invoice";
                    mm.Body = "Thanks for your time. Please find the attached invoice";
                    mm.Attachments.Add(new Attachment(memoryStream, "TaxInvoice.pdf"));
                    mm.IsBodyHtml = true;


                    SmtpClient smtp = new SmtpClient();
                    var SmtpUser = new System.Net.NetworkCredential(smtpUsername, smtpPassword, "outlook.office365.com");
                    smtp.Host = smtpServer;
                    smtp.TargetName = "STARTTLS/smtp.office365.com";
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = SmtpUser;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Port = smtpPort;
                    try
                    {
                        smtp.Send(mm);
                    }
                    catch (Exception ex)
                    {
                        // Handle exception or log error
                    }
                }

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }

        public class EmailSetting
        {
            public string smtpserver { get; set; }
            public string mailServer { get; set; }
            public int mailPort { get; set; }
            public string senderName { get; set; }
            public string senderEmail { get; set; }
            public string password { get; set; }
        }
    }
}
