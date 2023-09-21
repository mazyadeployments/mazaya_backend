using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.EmailTemplate;
using MMA.WebApi.Shared.Models.Email;
using MMA.WebApi.Shared.Models.MailStorage;
using System;

namespace MMA.WebApi.Shared.Models
{
    public class EmailTemplateUtils
    {

        private readonly IConfiguration _configuration;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IEmailTemplateRootRepository _emailTemplateRootRepository;


        public EmailTemplateUtils(IEmailTemplateRepository emailTemplateRepository,
                                IEmailTemplateRootRepository emailTemplateRootRepository,
                                IConfiguration configuration)
        {
            _configuration = configuration;
            _emailTemplateRepository = emailTemplateRepository;
            _emailTemplateRootRepository = emailTemplateRootRepository;

        }

        public string body;
        public string subject;
        public string message = string.Empty;
        private String _MailTemplate = string.Empty;

        public string notification = string.Empty;
        public Declares.NotificationTypeList notificationTypeId = Declares.NotificationTypeList.Active;



        private String _BodyFooter = string.Empty;
        private String _ApplicationLogin = string.Empty;

        private EmailTemplateModel GetEmailTemplateData(int templateId)
        {
            EmailTemplateModel template = _emailTemplateRepository.GetEmailTemplateData(templateId);

            if (template != null)
            {
                if (!string.IsNullOrEmpty(template.Notification))
                    notification = template.Notification;
                if (template.NotificationTypeId > 0)
                {
                    notificationTypeId = (Declares.NotificationTypeList)template.NotificationTypeId;
                }

            }
            else
            {
                template = new EmailTemplateModel()
                {
                    Name = "Please add email templates.",
                    Subject = "Please add email templates.",
                    Body = "Please add email templates.",
                    Message = "Please add email templates.",
                    Notification = "Please add email templates.",
                    NotificationTypeId = (int)Declares.NotificationTypeList.Active
                };

            }

            return template;
        }


        // Refactor this method when there is time
        // Send MailTemplate & MessageTemplate in same object?
        // There is much code that repets itself
        public void CreateBodyAndSubject(EmailDataModel emailDataModel)
        {
            var mailTemplateModel = new MailTemplateModel();
            var emailTemplate = Declares.MailTemplateList.Action;

            var customMessage = string.Empty;
            switch (emailDataModel.MailTemplateId)
            {
                case Declares.MessageTemplateList.Supplier_Processed_Notify_Supplier:
                case Declares.MessageTemplateList.Supplier_Approved_Notify_Supplier:
                    if (emailDataModel.IsApproved)
                    {
                        mailTemplateModel.DetailsLink = _configuration["BaseURL:Url"];

                        SetupEmail(Declares.MailTemplateList.Info, Declares.MessageTemplateList.Supplier_Approved_Notify_Supplier, mailTemplateModel);
                    }
                    else
                    {
                        emailTemplate = emailDataModel.IsApproved ? Declares.MailTemplateList.Info : Declares.MailTemplateList.Alert;
                        mailTemplateModel.Text = emailDataModel.IsApproved ? "approved." : "rejected.";
                        mailTemplateModel.SubjectTitle1 = "Company:";
                        mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                        mailTemplateModel.DetailsLink = _configuration["BaseURL:Url"];

                        SetupEmail(emailTemplate, emailDataModel.MailTemplateId, mailTemplateModel);
                    }

                    break;

                case Declares.MessageTemplateList.Supplier_Registration_Notify_Coordinator:
                    mailTemplateModel.Text = emailDataModel.CompanyName;
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}suppliers/pending";

                    SetupEmail(Declares.MailTemplateList.Action, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Offer_To_Process_Notify_Reviewer:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Action, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Offer_Returned_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Offer_To_Process_Notify_Coordinator:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Action, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Offer_Processed_Notify_SupplierAdminOrSupplier:
                    emailTemplate = emailDataModel.IsApproved ? Declares.MailTemplateList.Info : Declares.MailTemplateList.Alert;
                    mailTemplateModel.Text = emailDataModel.IsApproved ? "approved." : "rejected.";
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(emailTemplate, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Company_Invited_To_Roadshow_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.RoadshowName;
                    mailTemplateModel.SubjectTitle1 = "Roadshow:";
                    mailTemplateModel.SubjectText1 = emailDataModel.RoadshowName;
                    //mailTemplateModel.SubjectTitle2 = "Location:";
                    //mailTemplateModel.SubjectText2 = emailDataModel.RoadshowLocation;
                    mailTemplateModel.SubjectTitle3 = "Company:";
                    mailTemplateModel.SubjectText3 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning";

                    SetupEmail(Declares.MailTemplateList.Action, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Roadshow_Published_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.RoadshowId.ToString();
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning/{emailDataModel.RoadshowId}/edit";

                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;
                case Declares.MessageTemplateList.Roadshow_Unpublished_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.RoadshowName;
                    mailTemplateModel.SubjectTitle1 = "Roadshow:";
                    mailTemplateModel.SubjectText1 = emailDataModel.RoadshowName;
                    //mailTemplateModel.SubjectTitle2 = "Location:";
                    //mailTemplateModel.SubjectText2 = emailDataModel.RoadshowLocation;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Roadshow_Approved_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.RoadshowId.ToString();
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning/{emailDataModel.RoadshowId}/edit";

                    SetupEmail(Declares.MailTemplateList.Action, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;
                case Declares.MessageTemplateList.Roadshow_Returned_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.RoadshowName;
                    mailTemplateModel.SubjectTitle1 = "Roadshow:";
                    mailTemplateModel.SubjectText1 = emailDataModel.RoadshowName;
                    //mailTemplateModel.SubjectTitle2 = "Location:";
                    //mailTemplateModel.SubjectText2 = emailDataModel.RoadshowLocation;
                    mailTemplateModel.SubjectTitle3 = "Company:";
                    mailTemplateModel.SubjectText3 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Roadshow_Invite_Renegotiation_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.RoadshowName;
                    mailTemplateModel.SubjectTitle1 = "Roadshow:";
                    mailTemplateModel.SubjectText1 = emailDataModel.RoadshowName;
                    //mailTemplateModel.SubjectTitle2 = "Location:";
                    //mailTemplateModel.SubjectText2 = emailDataModel.RoadshowLocation;
                    mailTemplateModel.SubjectTitle3 = "Company:";
                    mailTemplateModel.SubjectText3 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning";

                    SetupEmail(Declares.MailTemplateList.Action, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Roadshow_Expired_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.RoadshowId.ToString();
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning/{emailDataModel.RoadshowId}/edit";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Roadshow_Cancelled_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.RoadshowName;
                    mailTemplateModel.SubjectTitle1 = "Roadshow:";
                    mailTemplateModel.SubjectText1 = emailDataModel.RoadshowName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Roadshow_Starts_In_5_Days_Notify_Coordinator:
                    mailTemplateModel.Text = emailDataModel.RoadshowId.ToString();
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning/{emailDataModel.RoadshowId}/edit";

                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Roadshow_Starts_In_1_Day_Notify_Coordinator:
                    mailTemplateModel.Text = emailDataModel.RoadshowId.ToString();
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning/{emailDataModel.RoadshowId}/edit";

                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Roadshow_Starts_Today_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.RoadshowId.ToString();
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning/{emailDataModel.RoadshowId}/edit";

                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;

                case Declares.MessageTemplateList.Adnoc_Employee_Invited_New_Family_Member:
                    mailTemplateModel.Text = emailDataModel.User.Email;
                    mailTemplateModel.SubjectTitle1 = "1. ";
                    mailTemplateModel.SubjectTitle2 = "2. ";
                    mailTemplateModel.SubjectTitle3 = "3. ";

                    mailTemplateModel.SubjectText1 = "Go to Mazaya Offers";
                    mailTemplateModel.SubjectText2 = "Click on Guest";
                    mailTemplateModel.SubjectText3 = "Click on Email Signup to Register";

                    mailTemplateModel.DetailsLink = _configuration["BaseURL:Url"];
                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;

                case Declares.MessageTemplateList.Offer_Expired_Notify_SupplierAdminOrSupplier:
                    // Your offer with an id - @@custom-message@@ is expired.
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;

                case Declares.MessageTemplateList.Offer_To_Expire_In_3_Weeks_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;

                case Declares.MessageTemplateList.Offer_To_Expire_In_A_Week_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;

                case Declares.MessageTemplateList.Offer_To_Expire_In_A_Day_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;

                case Declares.MessageTemplateList.Offer_Expired_Notify_Coordinator:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;

                case Declares.MessageTemplateList.Offer_To_Expire_In_3_Weeks_Notify_Coordinator:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;

                case Declares.MessageTemplateList.Offer_To_Expire_In_A_Week_Notify_Coordinator:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;

                case Declares.MessageTemplateList.Offer_To_Expire_In_A_Day_Notify_Coordinator:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;

                case Declares.MessageTemplateList.Offer_Created_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.OfferId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Company:";
                    mailTemplateModel.SubjectText1 = emailDataModel.CompanyName;
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}offers/all/{emailDataModel.OfferId}";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;
                case Declares.MessageTemplateList.Survey_Notify:
                    mailTemplateModel.Text = emailDataModel.SurveyId.ToString();
                    mailTemplateModel.SubjectTitle1 = "Survey:";
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}surveys/{emailDataModel.SurveyId}";
                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);

                    break;
                case Declares.MessageTemplateList.Roadshow_Confirmed_Notify_All:
                    mailTemplateModel.Text = emailDataModel.RoadshowId.ToString();
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning/{emailDataModel.RoadshowId}/edit";

                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;
                case Declares.MessageTemplateList.Roadshow_Submitted_Notify_Coordinator:
                    mailTemplateModel.Text = emailDataModel.RoadshowId.ToString();
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning/{emailDataModel.RoadshowId}/edit";

                    SetupEmail(Declares.MailTemplateList.Action, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;
                case Declares.MessageTemplateList.Roadshow_Returned_To_Supplier_Notify_SupplierAdminOrSupplier:
                    mailTemplateModel.Text = emailDataModel.RoadshowId.ToString();
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning/{emailDataModel.RoadshowId}/edit";

                    SetupEmail(Declares.MailTemplateList.Action, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;
                case Declares.MessageTemplateList.Roadshow_Reject_Attendance_Notify_Coordinator:
                    mailTemplateModel.Text = emailDataModel.RoadshowId.ToString();
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}roadshows-administration/planning/{emailDataModel.RoadshowId}/edit";

                    SetupEmail(Declares.MailTemplateList.Action, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;
                case Declares.MessageTemplateList.Announcement_Successfully_Sent:
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}administration/mail-storage";

                    SetupEmail(Declares.MailTemplateList.Info, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;
                case Declares.MessageTemplateList.Announcement_Failed_To_Sent:
                    mailTemplateModel.DetailsLink = $"{_configuration["BaseURL:Url"]}administration/mail-storage";

                    SetupEmail(Declares.MailTemplateList.Alert, emailDataModel.MailTemplateId, mailTemplateModel);
                    break;
                default:
                    var offerId = emailDataModel.OfferId != null ? emailDataModel.OfferId.ToString() : "";
                    var proposalId = emailDataModel.ProposalId != null ? emailDataModel.ProposalId.ToString() : "";

                    subject = String.Format("Mazaya Offers:");
                    body = _MailTemplate;
                    body = body.Replace("@@bodyFooter@@", _BodyFooter);
                    notification = "notificaiton";
                    notificationTypeId = Declares.NotificationTypeList.Active;
                    message = "Please define message..." + emailDataModel.MailTemplateId.ToString() + " OfferId: " + offerId +
                              " proposalId: " + proposalId;
                    break;

            }
        }

        private void SetupEmail(Declares.MailTemplateList mailTemplateId, Declares.MessageTemplateList messageTemplateId, MailTemplateModel mailTemplateModel)
        {
            var templateData = GetEmailTemplateData((int)messageTemplateId);
            subject = templateData.Subject;
            mailTemplateModel.Text = templateData.Message.Replace("@@custom-message@@", mailTemplateModel.Text);
            message = mailTemplateModel.Text;
            var template = GetEmailTemplateRootData((int)mailTemplateId);

            body = template.MailTemplate;
            body = SetupEmailBody(body, mailTemplateModel, messageTemplateId);
        }

        private string SetupEmailBody(string body, MailTemplateModel mailTemplateModel, Declares.MessageTemplateList messageTemplate)
        {
            body = body.Replace("@@Text@@", mailTemplateModel.Text);

            body = body.Replace("@@Subject-title-1@@", mailTemplateModel.SubjectTitle1);
            body = body.Replace("@@Subject-title-2@@", mailTemplateModel.SubjectTitle2);
            body = body.Replace("@@Subject-title-3@@", mailTemplateModel.SubjectTitle3);

            body = body.Replace("@@Subject-text-1@@", mailTemplateModel.SubjectText1);
            body = body.Replace("@@Subject-text-2@@", mailTemplateModel.SubjectText2);
            body = body.Replace("@@Subject-text-3@@", mailTemplateModel.SubjectText3);

            body = body.Replace("@@details-link@@", mailTemplateModel.DetailsLink);

            if (messageTemplate == Declares.MessageTemplateList.Adnoc_Employee_Invited_New_Family_Member)
            {
                body = body.Replace("@@details-text@@", "Go to Mazaya Offers");
                body = body.Replace("Date:", "");
                body = body.Replace("@@Date@@", "");
            }
            else
            {
                body = body.Replace("@@details-text@@", "VIEW DETAILS");
                body = body.Replace("@@Date@@", DateTime.UtcNow.ToString("dd-MMMM-yyyy"));
            }

            return body;
        }

        public EmailTemplateRootModel GetEmailTemplateRootData(int mailTemplateId)
        {
            var template = _emailTemplateRootRepository.GetEmailTemplateRootData(mailTemplateId);
            return template;
        }

    }
}

