using Microsoft.AspNetCore.Identity;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.Analytics;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.OfferRating;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RedeemOffer;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.Offer;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Core.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IOfferService _offerService;
        private readonly IOfferRatingRepository _offerRatingRepository;
        private readonly IApplicationUsersRepository _applicationUserRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRedeemOfferRepository _redeemOfferRepository;

        public AnalyticsService(
            IOfferRepository offerRepository,
            IOfferService offerService,
            IOfferRatingRepository offerRatingRepository,
            IApplicationUsersRepository applicationUserRepository,
            ICompanyRepository companyRepository,
            ICategoryRepository categoryRepository,
            UserManager<ApplicationUser> userManager,
            IRedeemOfferRepository redeemOfferRepository
        )
        {
            _companyRepository = companyRepository;
            _offerRepository = offerRepository;
            _offerService = offerService;
            _offerRatingRepository = offerRatingRepository;
            _applicationUserRepository = applicationUserRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _redeemOfferRepository = redeemOfferRepository;
        }

        public async Task<byte[]> ExportAnalyticsReporttData(string analyticsReportType)
        {
            byte[] excelFile = null;
            switch (analyticsReportType)
            {
                case "registered-members":
                    excelFile = await ExportRegistredUsers();
                    break;
                case "users-device":
                    excelFile = await ExportRegistratiesPerDevice();
                    break;
                case "offers":
                    excelFile = await ExportOffers();
                    break;
                case "offer-comments":
                    excelFile = await ExportOfferComments();
                    break;
                case "offers-datails":
                    excelFile = await ExportOffersDetails();
                    break;
                case "redeemed-offers":
                    excelFile = await ExportRedeemedOffers();
                    break;
                default:
                    break;
            }

            return excelFile;
        }

        public async Task<byte[]> ExportRedeemedOffers()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // get all reedeem offer
            using (var package = new ExcelPackage())
            {
                DateTimeFormatInfo mfi = new DateTimeFormatInfo();

                var redemeedOffers = await _redeemOfferRepository.GetRedeemedOfferCounts();
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Redeemed Offers");

                worksheet.Cells["A1:C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:C1"].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.Color.LightGray
                );
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                worksheet.Cells["A1"].Value = "Offer Id";
                worksheet.Cells["B1"].Value = "Offer Title";
                worksheet.Cells["C1"].Value = "Number of redemptions";
                worksheet.Cells["A1:C1"].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin
                );
                int row = 2;
                foreach (var item in redemeedOffers)
                {
                    worksheet.Cells["A" + row].Value = item.OfferId;
                    worksheet.Cells["B" + row].Value = item.OfferTitle;
                    worksheet.Cells["C" + row].Value = item.Count;

                    row++;
                }
                worksheet.Cells["A" + 1 + ":I" + row].AutoFitColumns();
                await package.SaveAsync();

                return await package.GetAsByteArrayAsync();
            }
        }

        public async Task<byte[]> ExportOffersDetails()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var Categotys = _categoryRepository.GetAllCategoty();
            using (var package = new ExcelPackage())
            {
                DateTimeFormatInfo mfi = new DateTimeFormatInfo();
                foreach (var cat in Categotys)
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(cat.Title);

                    var offerForCat = _offerRepository.GetOffersForReportByCategory(cat.Id);
                    CreateSheet(worksheet, offerForCat.ToHashSet());
                }

                await package.SaveAsync();

                return await package.GetAsByteArrayAsync();
            }
        }

        private async void CreateSheet(
            ExcelWorksheet worksheet,
            ICollection<OfferExcelModel> offerlist
        )
        {
            worksheet.Cells["A1:I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(
                System.Drawing.Color.LightGray
            );
            worksheet.Cells["A1:I1"].Style.Font.Bold = true;

            worksheet.Cells["A1"].Value = "Offer Id";
            worksheet.Cells["B1"].Value = "Company name";
            worksheet.Cells["C1"].Value = "Focal point";
            worksheet.Cells["D1"].Value = "Email";
            worksheet.Cells["E1"].Value = "Phone number";
            worksheet.Cells["F1"].Value = "Type";
            worksheet.Cells["G1"].Value = "Valid from";
            worksheet.Cells["H1"].Value = "Valid until";
            worksheet.Cells["I1"].Value = "Description";
            worksheet.Cells["A1:I1"].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin
            );

            int i = 1;
            int row = 2;
            foreach (var offer in offerlist)
            {
                worksheet.Cells["A" + row].Value = offer.OfferId;
                worksheet.Cells["B" + row].Value = offer.CompanyNameEnglish;
                worksheet.Cells["C" + row].Value =
                    offer.FocalPoint != null ? offer.FocalPoint : "/";
                worksheet.Cells["D" + row].Value =
                    offer.CompanyEmail != "" && offer.CompanyEmail != null
                        ? offer.CompanyEmail
                        : "/";
                worksheet.Cells["E" + row].Value =
                    offer.PhoneNumber != "" && offer.PhoneNumber != null ? offer.PhoneNumber : "/";
                worksheet.Cells["F" + row].Value = offer.PriceFiled;
                worksheet.Cells["G" + row].Value =
                    offer.ValidFrom != null ? offer.ValidFrom.Value.ToString("MMMM dd, yyyy") : "/";
                worksheet.Cells["H" + row].Value =
                    offer.ValidUntil != null
                        ? offer.ValidUntil.Value.ToString("MMMM dd, yyyy")
                        : "/";
                worksheet.Cells["I" + row].Value = offer.Description;

                if (i % 2 == 0)
                {
                    worksheet.Cells["A" + row + ":I" + row].Style.Fill.PatternType =
                        ExcelFillStyle.Solid;
                    worksheet.Cells["A" + row + ":I" + row].Style.Fill.BackgroundColor.SetColor(
                        System.Drawing.Color.LightGray
                    );
                }
                i++;
                row++;
            }
            worksheet.Cells["A" + 1 + ":I" + row].AutoFitColumns();
        }

        private ExcelWorksheet createDefaultSupplierCategorySheet(
            ExcelPackage package,
            string sheetName,
            int suppliersInCategory,
            DateTime startDate,
            DateTime endDate
        )
        {
            if (sheetName == "")
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Uncategorized");
                worksheet.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells["A1"].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells["A1"].Value =
                    "Partners Count ["
                    + startDate.ToString("MMMM dd, yyyy")
                    + " - "
                    + endDate.ToString("MMMM dd, yyyy")
                    + "]:";
                worksheet.Cells["A1"].Style.ShrinkToFit = true;

                worksheet.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B1"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells["B1"].Value = suppliersInCategory;
                worksheet.Cells["B1"].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["B1"].Style.ShrinkToFit = true;

                int row = 5;
                int col = 1;

                worksheet.Cells[row, col].Value = "Name"; //NameEnglish
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                col++;

                worksheet.Cells[row, col].Value = "Mobile Number";
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                col++;

                worksheet.Cells[row, col].Value = "Approved Offers";
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                col++;

                worksheet.Cells[row, col].Value = "Expired Offers";
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                col++;

                worksheet.Cells[row, col].Value = "Draft Offers";
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                col++;

                worksheet.Cells[row, col].Value = "Focal Points";
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                row++;

                return worksheet;
            }
            else
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells["A1"].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells["A1"].Value =
                    "Partners Count ["
                    + startDate.ToString("MMMM dd, yyyy")
                    + " - "
                    + endDate.ToString("MMMM dd, yyyy")
                    + "]:";
                worksheet.Cells["A1"].Style.ShrinkToFit = true;

                worksheet.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B1"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells["B1"].Value = suppliersInCategory;
                worksheet.Cells["B1"].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["B1"].Style.ShrinkToFit = true;
                int row = 5;
                int col = 1;

                worksheet.Cells[row, col].Value = "Name"; //NameEnglish
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                col++;

                worksheet.Cells[row, col].Value = "Mobile Number";
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                col++;

                worksheet.Cells[row, col].Value = "Focal Points";
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                col++;

                worksheet.Cells[row, col].Value = "Approved Offers";
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                col++;

                worksheet.Cells[row, col].Value = "Expired Offers";
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                col++;

                worksheet.Cells[row, col].Value = "Draft Offers";
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.ColorTranslator.FromHtml("#002060")
                );
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[row, col].Style.ShrinkToFit = true;
                row++;

                return worksheet;
            }
        }

        private void populateSuppliersCategorySheet(
            ExcelWorksheet worksheet,
            List<CompanyExcelModel> listForIteration,
            int row,
            int col
        )
        {
            for (int i = 0; i < listForIteration.Count(); i++)
            {
                row++;
                col = 1;

                worksheet.Cells[row, col].Value =
                    (listForIteration[i].Name != null && listForIteration[i].Name != "")
                        ? listForIteration[i].Name
                        : "/";
                col++;

                worksheet.Cells[row, col].Value =
                    (
                        listForIteration[i].MobileNumber != null
                        && listForIteration[i].MobileNumber != null
                        && listForIteration[i].MobileNumber != ""
                    )
                        ? listForIteration[i].MobileNumber
                        : "/";
                col++;

                worksheet.Cells[row, col].Value = listForIteration[i].FocalPointMails.Count();
                worksheet.Cells[row, col].Style.HorizontalAlignment =
                    ExcelHorizontalAlignment.Center;
                worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                col++;

                worksheet.Cells[row, col].Value = listForIteration[i].ApprovedOfferNumber;
                col++;

                worksheet.Cells[row, col].Value = listForIteration[i].ExpiredOfferNumber;
                col++;

                worksheet.Cells[row, col].Value = listForIteration[i].DraftOfferNumber;
                col++;

                row++;
                col = 1;
                var focalPointCount = listForIteration[i].FocalPointMails.Count();
                if (focalPointCount > 0)
                {
                    col = 3;
                    for (int j = 0; j < focalPointCount; j++)
                    {
                        worksheet.Cells[row, col].Value =
                            listForIteration[i].FocalPointMails[j] != null
                                ? listForIteration[i].FocalPointMails[j]
                                : "/";
                        worksheet.Cells[row, col].Style.HorizontalAlignment =
                            ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, col].Style.VerticalAlignment =
                            ExcelVerticalAlignment.Center;
                        row++;
                    }
                }
            }
        }

        public async Task<byte[]> ExportSupplierReport(DateTime startDate, DateTime endDate)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (startDate >= DateTime.Now)
            {
                return null;
            }
            var allSuplier = (
                await _companyRepository.GetCompaniesExcelReport(startDate, endDate)
            ).ToList();
            if (allSuplier == null)
            {
                return null;
            }

            var categories = _categoryRepository.GetAllCategoty().ToList();

            if (categories == null)
            {
                return null; // CANNOT CREATE SHEETS WITHOUT CATEGORIES
            }
            else
            {
                using (var package = new ExcelPackage())
                {
                    DateTimeFormatInfo mfi = new DateTimeFormatInfo();

                    int row = 6;
                    int col = 4;

                    #region CategorizedCompanies
                    foreach (CategoryModel category in categories)
                    {
                        /* GENERATING SHEETS AND POPULATING IT */
                        if (allSuplier.Count() > 0)
                        {
                            var listForIteration = allSuplier
                                .Where(x => x.CategoryId == category.Id)
                                .ToList();
                            int suppliersInCategory = listForIteration.Count();
                            ExcelWorksheet worksheetCategorized =
                                createDefaultSupplierCategorySheet(
                                    package,
                                    category.Title,
                                    suppliersInCategory,
                                    startDate,
                                    endDate
                                );
                            populateSuppliersCategorySheet(
                                worksheetCategorized,
                                listForIteration,
                                row,
                                col
                            );
                            worksheetCategorized.Cells.AutoFitColumns();
                        }
                        /* GENERATING SHEETS AND *NOT* POPULATING IT */
                        else
                        {
                            ExcelWorksheet worksheetCategorized =
                                createDefaultSupplierCategorySheet(
                                    package,
                                    category.Title,
                                    0,
                                    startDate,
                                    endDate
                                );
                            worksheetCategorized.Cells.AutoFitColumns();
                        }
                    }
                    #endregion

                    #region UncategorizedCompanies
                    /* GENERATING UNCATEGORIZED SHEET AND POPULATING IT */
                    if (allSuplier.Count() > 0)
                    {
                        int uncategorizedCompanies = allSuplier
                            .Where(x => x.CategoryId == 0)
                            .Count();
                        ExcelWorksheet worksheetUncategorized = createDefaultSupplierCategorySheet(
                            package,
                            "Uncategorized",
                            uncategorizedCompanies,
                            startDate,
                            endDate
                        );
                        var listForIteration = allSuplier.Where(x => x.CategoryId == 0).ToList();
                        populateSuppliersCategorySheet(
                            worksheetUncategorized,
                            listForIteration,
                            row,
                            col
                        );
                        worksheetUncategorized.Cells.AutoFitColumns();
                    }
                    /* GENERATING UNCATEGORIZED SHEET AND *NOT* POPULATING IT */
                    else
                    {
                        ExcelWorksheet worksheetUncategorized = createDefaultSupplierCategorySheet(
                            package,
                            "Uncategorized",
                            0,
                            startDate,
                            endDate
                        );
                        var listForIteration = allSuplier.Where(x => x.CategoryId == 0).ToList();
                        populateSuppliersCategorySheet(
                            worksheetUncategorized,
                            listForIteration,
                            row,
                            col
                        );
                        worksheetUncategorized.Cells.AutoFitColumns();
                    }
                    #endregion

                    await package.SaveAsync();
                    return await package.GetAsByteArrayAsync();
                }
            }
        }

        private async Task<byte[]> ExportOfferComments()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var offersList = await _offerService.GetOffers();
            var offerRatingsList = await _offerRatingRepository.GetOfferRatings();
            var offerRatingsListDistinct = offerRatingsList.Select(x => x.OfferId).Distinct();

            var offersWithRatingsAndComments = offersList.Where(
                x => offerRatingsListDistinct.Contains(x.Id)
            );

            var commentsCount = offerRatingsList
                .Where(x => x.CommentText != null && x.CommentText != "")
                .Select(x => x.OfferId)
                .Distinct()
                .Count();
            var offerRatingsCount = offerRatingsList
                .Where(x => x.Rating != null)
                .Select(x => x.OfferId)
                .Distinct()
                .Count();

            using (var package = new ExcelPackage())
            {
                DateTimeFormatInfo mfi = new DateTimeFormatInfo();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[2, 1].Value = "Number of offers with comments";
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells["B" + 2].Value = commentsCount;

                worksheet.Cells[3, 1].Value = "Number of offers with rating";
                worksheet.Cells[3, 1].Style.Font.Bold = true;
                worksheet.Cells["B" + 3].Value = offerRatingsCount;

                worksheet.Cells[6, 1].Value = "Offer ID (Name)";
                worksheet.Cells[6, 1].Style.Font.Bold = true;

                worksheet.Cells[6, 2].Value = "Number of comments";
                worksheet.Cells[6, 2].Style.Font.Bold = true;

                worksheet.Cells[6, 3].Value = "Number of ratings";
                worksheet.Cells[6, 3].Style.Font.Bold = true;

                int numberCounter = 7;

                foreach (var offer in offersWithRatingsAndComments)
                {
                    worksheet.Cells["A" + numberCounter].Value =
                        offer.Id + " " + offer.CompanyNameEnglish;

                    var ratings = offerRatingsList
                        .Where(x => x.OfferId == offer.Id && x.Rating != null)
                        .Count();

                    var comments = offerRatingsList
                        .Where(
                            x =>
                                x.OfferId == offer.Id
                                && x.CommentText != null
                                && x.CommentText != ""
                        )
                        .Count();

                    worksheet.Cells["B" + numberCounter].Value = comments;
                    worksheet.Cells["C" + numberCounter].Value = ratings;
                    numberCounter++;
                }

                worksheet.Cells[1, 1, 10, 3].AutoFitColumns();

                await package.SaveAsync();

                return await package.GetAsByteArrayAsync();
            }
        }

        private async Task<byte[]> ExportOffers()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var offersList = await _offerService.GetOffers();

            using (var package = new ExcelPackage())
            {
                DateTimeFormatInfo mfi = new DateTimeFormatInfo();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Offer Status";
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                worksheet.Cells[1, 2].Value = "Number of offers";
                worksheet.Cells[1, 2].Style.Font.Bold = true;

                worksheet.Cells["A" + 2].Value = OfferStatus.Draft.ToString();
                worksheet.Cells["A" + 3].Value = OfferStatus.Review.ToString();
                worksheet.Cells["A" + 4].Value = OfferStatus.Approved.ToString();
                worksheet.Cells["A" + 5].Value = OfferStatus.Rejected.ToString();
                worksheet.Cells["A" + 6].Value = OfferStatus.PendingApproval.ToString();
                worksheet.Cells["A" + 7].Value = OfferStatus.Migrated.ToString();
                worksheet.Cells["A" + 8].Value = OfferStatus.Blocked.ToString();
                worksheet.Cells["A" + 9].Value = OfferStatus.Cancelled.ToString();
                worksheet.Cells["A" + 10].Value = OfferStatus.Expired.ToString();

                worksheet.Cells["B" + 2].Value = offersList
                    .Where(x => x.Status == OfferStatus.Draft.ToString())
                    .Count();
                worksheet.Cells["B" + 3].Value = offersList
                    .Where(x => x.Status == OfferStatus.Review.ToString())
                    .Count();
                worksheet.Cells["B" + 4].Value = offersList
                    .Where(x => x.Status == OfferStatus.Approved.ToString())
                    .Count();
                worksheet.Cells["B" + 5].Value = offersList
                    .Where(x => x.Status == OfferStatus.Rejected.ToString())
                    .Count();
                worksheet.Cells["B" + 6].Value = offersList
                    .Where(x => x.Status == OfferStatus.PendingApproval.ToString())
                    .Count();
                worksheet.Cells["B" + 7].Value = offersList
                    .Where(x => x.Status == OfferStatus.Migrated.ToString())
                    .Count();
                worksheet.Cells["B" + 8].Value = offersList
                    .Where(x => x.Status == OfferStatus.Blocked.ToString())
                    .Count();
                worksheet.Cells["B" + 9].Value = offersList
                    .Where(x => x.Status == OfferStatus.Cancelled.ToString())
                    .Count();
                worksheet.Cells["B" + 10].Value = offersList
                    .Where(x => x.Status == OfferStatus.Expired.ToString())
                    .Count();

                worksheet.Cells["A" + 12].Value = "All offers";
                worksheet.Cells["A" + 12].Style.Font.Bold = true;

                worksheet.Cells["B" + 12].Value = offersList.Count();
                worksheet.Cells["B" + 12].Style.Font.Bold = true;

                worksheet.Cells[1, 1, 10, 3].AutoFitColumns();

                await package.SaveAsync();

                return await package.GetAsByteArrayAsync();
            }
        }

        private async Task<byte[]> ExportRegistratiesPerDevice()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var applicationUsersList = await _applicationUserRepository.GetAllAsync();

            int androidNo = applicationUsersList
                .Where(x => x.PlatformType == PlatformType.Android.ToString())
                .Count();
            int iOSNo = applicationUsersList
                .Where(
                    x =>
                        x.PlatformType == PlatformType.iPad.ToString()
                        || x.PlatformType == PlatformType.iPhone.ToString()
                )
                .Count();
            int webNo = applicationUsersList
                .Where(x => x.PlatformType == PlatformType.Web.ToString())
                .Count();

            using (var package = new ExcelPackage())
            {
                DateTimeFormatInfo mfi = new DateTimeFormatInfo();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Device Type";
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                worksheet.Cells[1, 2].Value = "Number of registries";
                worksheet.Cells[1, 2].Style.Font.Bold = true;

                worksheet.Cells["A" + 2].Value = "Android";
                worksheet.Cells["A" + 3].Value = "iOS";
                worksheet.Cells["A" + 4].Value = "Web";

                worksheet.Cells["B" + 2].Value = androidNo;
                worksheet.Cells["B" + 3].Value = iOSNo;
                worksheet.Cells["B" + 4].Value = webNo;

                worksheet.Cells[1, 1, 10, 3].AutoFitColumns();

                await package.SaveAsync();

                return await package.GetAsByteArrayAsync();
            }
        }

        private async Task<byte[]> ExportRegistredUsers()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var userDomainsList = _applicationUserRepository.GetUserDomains();
            var allRegistredUsersList = await _applicationUserRepository.GetAllUserListAsync();

            var suppliersCount = await _companyRepository.GetCompanies();
            var buyersCount = await _applicationUserRepository.GetAllBuyers();

            using (var package = new ExcelPackage())
            {
                DateTimeFormatInfo mfi = new DateTimeFormatInfo();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Member type";
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                worksheet.Cells[1, 2].Value = "Number of members";
                worksheet.Cells[1, 2].Style.Font.Bold = true;

                worksheet.Cells[2, 1].Value = "Members";
                worksheet.Cells[2, 1].Style.Font.Bold = true;

                worksheet.Cells[2, 2].Value = buyersCount;
                worksheet.Cells[2, 2].Style.Font.Bold = true;

                int numberCounter = 3;

                foreach (var domain in userDomainsList)
                {
                    worksheet.Cells["A" + numberCounter].Value = domain.DomainName;

                    var userDomainCount = allRegistredUsersList
                        .Where(x => x.UserType == domain.DomainName)
                        .Count();

                    worksheet.Cells["B" + numberCounter].Value = userDomainCount;

                    numberCounter++;
                }

                worksheet.Cells["A" + numberCounter].Value = "Suppliers";
                worksheet.Cells["A" + numberCounter].Style.Font.Bold = true;

                worksheet.Cells["B" + numberCounter].Value = suppliersCount.Count();
                worksheet.Cells["B" + numberCounter].Style.Font.Bold = true;

                worksheet.Cells[1, 1, 10, 4].AutoFitColumns();

                await package.SaveAsync();

                return await package.GetAsByteArrayAsync();
            }
        }
    }
}
