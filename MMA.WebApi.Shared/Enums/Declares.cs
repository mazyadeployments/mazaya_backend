using System.Collections.Generic;
using System.Linq;

namespace MMA.WebApi.Shared.Enums
{
    public static class Declares
    {
        #region Filters

        public enum DurationPeriodList
        {
            Day = 10,
            Month = 20,
            Year = 30
        }

        public enum FilterSortDirection
        {
            Asc = 1,
            Desc = 2
        }

        #endregion



        public enum PriceTypes
        {
            Price,
            Discount,
            Other,
            DiscountPrice,
        }

        public enum OfferReportType
        {
            InvalidOffer = 1,
            NonResponsiveSupplier = 2,
            Other = 3
        }

        public enum Roles
        {
            Supplier,
            SupplierAdmin,
            SuperAdmin,
            Reviewer,
            AdnocCoordinator,
            Admin,
            Buyer,
            RoadshowFocalPoint
        }

        public static bool ContainsOneOrMore(
            this IEnumerable<string> roles,
            params Roles[] rolesToInclude
        )
        {
            var matchingRoles = rolesToInclude.Where(x => roles.Contains(x.ToString()));
            return matchingRoles.Count() > 0;
        }

        public static bool ContainsOneOrMore(
            this IEnumerable<Roles> roles,
            params Roles[] rolesToInclude
        )
        {
            return roles.Intersect(rolesToInclude).Any();
        }

        public enum OfferStatus
        {
            Draft = 1,
            Review = 2,
            Approved = 3,
            Rejected = 4,
            PendingApproval = 5,
            Expired = 6,
            Blocked = 7,
            Cancelled = 8,
            Migrated = 9
        }

        public enum RoadshowOfferStatus
        {
            Draft = 1,
            Review = 2,
            Approved = 3,
            Rejected = 4,
            PendingApproval = 5,
            Expired = 6,
            Deactivated = 7,
            Cancelled = 8
        }

        public enum RoadshowProposalStatus
        {
            Draft = 1,
            Active = 2,
            Expired = 3,
            Deactivated = 4
        }

        public enum RoadshowStatus
        {
            Draft = 1,
            Submitted = 2,
            Approved = 3,
            Confirmed = 4,
            Published = 5,
            Expired = 6,
            Cancelled = 7
        }

        public enum RoadshowInviteStatus
        {
            Draft = 1,
            Invited = 2,
            Accepted = 3,
            Approved = 4,
            Review = 5,
            Rejected = 6,
            Deactivated = 7,
            Blocked = 8,
            Expired = 9,
            Renegotiation = 10,
            Returned = 11,
            Cancelled = 12
        }

        public enum SupplierStatus
        {
            Approved = 1,
            Rejected = 2,
            PendingApproval = 3,
            Deactivated = 4,
            MissingTradeLicense = 5
        }

        public enum OfferCommentStatus
        {
            PendingApproval = 1,
            Public = 2,
            Private = 3
        }

        public enum OfferMembership
        {
            OffersAndDiscounts = 0,
            FamilyEntertainment = 1,
            HealtAndLeisure = 2,
            LeisureAndFamilyEntertainment = 3
        }

        public enum MembershipRelationship
        {
            Employee,
            Spouse,
            Child
        }

        #region Messaging

        public enum MessageStatusList
        {
            SENT = 1,
            FAILED = 2,
            PENDING = 3,
            PROCESSING = 4,
            CANCELED = 5,
            //  RETRY = 6
        }

        // TODO : 1 -> 1 from email template
        public enum NotificationTypeList
        {
            Active = 1,
            Inactive = 2
        }

        public enum MessageTemplateList
        {
            Supplier_Processed_Notify_Supplier = 1,
            Supplier_Registration_Notify_Coordinator = 2,
            Offer_To_Process_Notify_Reviewer = 3,
            Offer_Returned_Notify_SupplierAdminOrSupplier = 4,
            Offer_To_Process_Notify_Coordinator = 5,
            Offer_Processed_Notify_SupplierAdminOrSupplier = 6,
            Company_Invited_To_Roadshow_Notify_SupplierAdminOrSupplier = 10,
            Roadshow_Published_Notify_SupplierAdminOrSupplier = 11,
            Roadshow_Approved_Notify_SupplierAdminOrSupplier = 12,
            Roadshow_Returned_Notify_SupplierAdminOrSupplier = 13,
            Roadshow_Invite_Renegotiation_Notify_SupplierAdminOrSupplier = 14,
            User_Forgot_Password = 15,
            Roadshow_Expired_Notify_SupplierAdminOrSupplier = 16,
            Roadshow_Cancelled_Notify_SupplierAdminOrSupplier = 17,
            Roadshow_Starts_In_5_Days_Notify_Coordinator = 18,
            Roadshow_Starts_In_1_Day_Notify_Coordinator = 19,
            Roadshow_Starts_Today_Notify_SupplierAdminOrSupplier = 20,
            Roadshow_Unpublished_Notify_SupplierAdminOrSupplier = 21,
            Adnoc_Employee_Invited_New_Family_Member = 22,
            Supplier_Approved_Notify_Supplier = 23,
            Offer_Expired_Notify_SupplierAdminOrSupplier = 24,
            Offer_To_Expire_In_3_Weeks_Notify_SupplierAdminOrSupplier = 25,
            Offer_To_Expire_In_A_Week_Notify_SupplierAdminOrSupplier = 26,
            Offer_To_Expire_In_A_Day_Notify_SupplierAdminOrSupplier = 27,
            Offer_Expired_Notify_Coordinator = 28,
            Offer_To_Expire_In_3_Weeks_Notify_Coordinator = 29,
            Offer_To_Expire_In_A_Week_Notify_Coordinator = 30,
            Offer_To_Expire_In_A_Day_Notify_Coordinator = 31,
            Offer_Created_Notify_SupplierAdminOrSupplier = 40,
            Survey_Notify = 41,
            Roadshow_Confirmed_Notify_All = 42,
            Roadshow_Submitted_Notify_Coordinator = 43,
            Roadshow_Returned_To_Supplier_Notify_SupplierAdminOrSupplier = 44,
            Roadshow_Reject_Attendance_Notify_Coordinator = 45,
            Announcement_Successfully_Sent = 46,
            Announcement_Failed_To_Sent = 47
        }

        public enum MailTemplateList
        {
            Action = 1,
            Info = 2,
            Alert = 3
        }

        #endregion

        public enum AttachmentType
        {
            File = 1,
            Slide = 2
        }

        public enum AdnocTermsAndConditionType
        {
            ProposalType = 2,
            GlobalType = 3,
            OfferType = 4,
            RoadshowType = 5
        }

        public enum PlatformType
        {
            Android = 1,
            iPhone = 2,
            iPad = 3,
            Web = 4
        }

        public enum UserType
        {
            Undefined = 0,
            ADNOCEmployee = 1,
            ADNOCEmployeeFamilyMember = 2,
            ADPolice = 3,
            RedCrescent = 4,
            AlumniRetirementMembers = 5,
            ADSchools = 6,
            Other = 7,
            EtihadAE = 9,
            Masaood = 10,
            ATA = 12,
            Pension = 13,
            StaticCenter = 22,
            NationalSecurity = 23,
            DMT = 24
        }

        public enum UserValidationList
        {
            Continue,
            ShowBlockPage,
            ValidationError
        }

        public enum ImageForType
        {
            Offer,
            Category,
            Collection,
            Company,
            User,
            MembershipECard
        }

        public enum DefaultAreas
        {
            AbuDhabi = 1,
            Ajman = 2,
            Dubai = 3,
            Fujairah = 4,
            RasAlKhaimah = 5,
            Sharjah = 6,
            UmmAlQuwain = 7,
            International = 8,
            Other = 9
        }

        public enum OrderByType
        {
            Undefined = 0,
            Ascending = 1,
            Descending = 2
        }

        public enum Tag
        {
            Undefined = 0,
            Featured = 1,
            EndingSoon = 2,
            TopSeller = 3
        }

        public enum SurveyStatus
        {
            Draft = 0,
            Scheduled = 2,
            Published = 1,
            Closed = 3
        }

        public enum QuestionType
        {
            MultipleChoice = 0,
            Rating = 1,
            FreeText = 2
        }

        public enum UsersSurveyStatus
        {
            InProgres = 0,
            Completed = 1,
            Waitting = 2
        }

        public enum OfferSuggestionStatus
        {
            Complete = 0,
            Incomplete = 1,
            All = 2,
        }

        public enum AnnouncementStatus
        {
            Process = 0,
            Pending = 1,
            Success = 2,
            Failed = 3
        }

        public enum WalletCardType
        {
            Basic = 0,
            FamilyEntertainment = 1,
            HealtAndLeisure = 2,
            LeisureAndFamilyEntertainment = 3
        }
    }
}
