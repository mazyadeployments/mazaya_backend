using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Survey;
using MMA.WebApi.Shared.Interfaces.UserNotifications;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Survery.Answers;
using MMA.WebApi.Shared.Models.Survey;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISurveyForUserRepository _surveyForUserRepositoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailStorageService _mailStorageService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly ILogger _logger;
        private readonly IUserNotificationService _userNotificationService;

        public SurveyService(
            ISurveyRepository surveyRepository,
            ISurveyForUserRepository surveyForUserRepositoryRepository,
            UserManager<ApplicationUser> userManager,
            IApplicationUserService applicationUserService,
            IMailStorageService mailStorageService,
            ILogger<SurveyService> logger,
            IUserNotificationService userNotificationService
        )
        {
            _surveyRepository = surveyRepository;
            _surveyForUserRepositoryRepository = surveyForUserRepositoryRepository;
            _userManager = userManager;
            _mailStorageService = mailStorageService;
            _applicationUserService = applicationUserService;
            _logger = logger;
            _userNotificationService = userNotificationService;
        }

        #region add update
        public Task<int> InsertSurvey(SurveyModel data, string user)
        {
            return _surveyRepository.InsertSurvey(data, user);
        }

        public Task<int> UpdateSurvey(SurveyModel data, string user)
        {
            return _surveyRepository.UpdateSurvey(data, user);
        }

        public Task<SurveyModel> SetToScheduled(int surveyiId, string suplierId)
        {
            return _surveyRepository.SetToScheduled(surveyiId, suplierId);
        }

        public Task<SurveyModel> SetToDraft(int surveyiId, string suplierId)
        {
            return _surveyRepository.SetToDraft(surveyiId, suplierId);
        }

        public async Task<SurveyModel> CloseSurvey(int surveyiId, string suplierId)
        {
            return await _surveyRepository.CloseSurvey(surveyiId, suplierId);
        }

        public async Task<bool> DeleteSurvey(int id, string user)
        {
            return await _surveyRepository.RemoveSurvey(id, user);
        }
        #endregion
        #region get
        public async Task<PaginationListModel<SurveyModel>> GetAllSurveysForAdmin(
            QueryModel queryModel
        )
        {
            var allSurveys = await _surveyRepository.GetAll(queryModel);

            return await allSurveys.ToPagedListAsync(
                queryModel.Page,
                queryModel.PaginationParameters.PageSize
            );
        }

        public Task<SurveyModel> GetSurvey(int id)
        {
            var retVal = _surveyRepository.GetSurvey(id);
            return retVal;
        }
        #endregion

        #region publish&notification
        public async Task<int> PublishSurvey(int surveyiId, string suplierId)
        {
            var survey = await _surveyRepository.GetSurvey(surveyiId);
            if (survey == null)
                return -1;
            if (survey.End.Value < DateTime.UtcNow)
                return 0;
            if (
                !survey.Publish.AllUsers
                && survey.Publish.Roles == null
                && survey.Publish.Types == null
                && survey.Publish.UsersId == null
            )
                return -2;

            if (!await _surveyRepository.PublishSurvey(survey.Id, suplierId))
                return -1;
            return survey.Id;
        }

        #endregion
        public async Task CheckSurvey(ILogger logger)
        {
            try
            {
                await _surveyRepository.CheckSurveysDoBackgroundJobAsync(logger);
            }
            catch (Exception e)
            {
                logger.LogError(
                    "Error in trying to execute _surveyRepository.CheckSurveysForPublishDoBackgroundJobAsync -> "
                        + e.ToString()
                );
            }
        }

        public async Task<int> DuplicateSurvey(int id)
        {
            return await _surveyRepository.DuplicateSurvey(id);
        }

        private List<QuestionModel> CreateQuestionModel(SurveyModel data)
        {
            List<QuestionModel> temp = JsonConvert.DeserializeObject<List<QuestionModel>>(
                JsonConvert.SerializeObject(data.Questions)
            );
            foreach (var question in temp)
            {
                List<MyAnswer> basics = JsonConvert.DeserializeObject<List<MyAnswer>>(
                    JsonConvert.SerializeObject(question.Render)
                );
                if (basics != null)
                    question.MyAnswers = basics;
            }
            return temp;
        }

        public async Task<byte[]> CreateExcelFile(SurveyModel data)
        {
            data.QuestionsWithAnswers = CreateQuestionModel(data);

            int row = 1;
            int col = 1;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                DateTimeFormatInfo mfi = new DateTimeFormatInfo();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Answers");
                worksheet.Cells.AutoFitColumns();
                worksheet.Cells[++row, col].Value = "Survey: " + data.Title;
                worksheet.Cells[row, col].Style.Font.Bold = true;
                worksheet.Cells["A2:H2"].Style.Font.Size = 20;

                worksheet.Cells["A2:H2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A2:H2"].Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.Color.LightGray
                );
                int i = 1;

                worksheet.Cells["A2:H2"].Merge = true;
                int numberOfAns = await _surveyForUserRepositoryRepository.CountAnswersForSurvey(
                    data.Id
                );
                int numberOfAnsPosition = ++row;

                worksheet.Cells["A" + numberOfAnsPosition + ":H" + numberOfAnsPosition].Merge =
                    true;
                worksheet.Cells["A" + numberOfAnsPosition + ":H" + numberOfAnsPosition].Value =
                    "Answered " + numberOfAns + " out of " + data.Opportunity;
                worksheet.Cells["A" + numberOfAnsPosition + ":H" + numberOfAnsPosition]
                    .Style
                    .Font
                    .Bold = true;
                worksheet.Cells["A" + numberOfAnsPosition + ":H" + numberOfAnsPosition]
                    .Style
                    .Font
                    .Size = 15;

                row++;
                if (data.QuestionsWithAnswers != null)
                    foreach (var question in data.QuestionsWithAnswers)
                    {
                        if (question.MyAnswers != null)
                        {
                            switch (question.QuestionType)
                            {
                                case "BASIC":
                                    //kreirati poseban sheet za free tekst
                                    ExcelWorksheet worksheetFreeText =
                                        package.Workbook.Worksheets.Add("Free Tekst Question " + i);
                                    CreateFreeTextSheet(worksheetFreeText, question);
                                    numberOfAns = question.MyAnswers.Count();
                                    break;

                                default:
                                    //ostala pitanja uraditi sa istom logikom
                                    row = CreateQuestiionSection(worksheet, question, row);
                                    numberOfAns = question.MyAnswers.Count();

                                    row++;
                                    break;
                            }
                        }
                        i++;
                    }
                return await package.GetAsByteArrayAsync();
            }
        }

        private void CreateFreeTextSheet(ExcelWorksheet worksheet, QuestionModel question)
        {
            int row = 1;
            int col = 1;

            worksheet.Cells[++row, col].Value = question.QuestionText;
            worksheet.Cells[row, col].Style.Font.Bold = true;
            worksheet.Cells["A2:H2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A2:H2"].Style.Fill.BackgroundColor.SetColor(
                System.Drawing.Color.LightGray
            );

            worksheet.Cells["A2:H2"].Style.Font.Size = 14;
            worksheet.Cells["A2:H2"].Merge = true;
            int index = 0;
            foreach (var ans in question.MyAnswers)
            {
                row++;
                if (ans.value != null || !string.IsNullOrEmpty(ans.value))
                {
                    worksheet.Cells[row, 1].Value = ans.value;
                    worksheet.Cells["A" + row + ":H" + row].Merge = true;
                }
                else
                {
                    worksheet.Cells[row, 1].Value = "/";
                    worksheet.Cells["A" + row + ":H" + row].Merge = true;
                }

                if (index % 2 != 0)
                {
                    worksheet.Cells["A" + row + ":H" + row].Style.Fill.PatternType =
                        ExcelFillStyle.Solid;
                    worksheet.Cells["A" + row + ":H" + row].Style.Fill.BackgroundColor.SetColor(
                        System.Drawing.Color.LightGray
                    );
                }
                index++;
            }
            worksheet.Cells.AutoFitColumns();
        }

        private int CreateQuestiionSection(
            ExcelWorksheet worksheet,
            QuestionModel question,
            int row
        )
        {
            int startTableRow = 0;
            int startTableRowPosition = 0;

            worksheet.Cells[++row, 1].Value = question.QuestionText;
            worksheet.Cells[row, 1].Style.Font.Bold = true;

            worksheet.Cells["A2:H2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A2:H2"].Style.Fill.BackgroundColor.SetColor(
                System.Drawing.Color.LightGray
            );
            worksheet.Cells["A" + row + ":H" + row].Style.Font.Size = 14;
            worksheet.Cells["A" + row + ":H" + row].Merge = true;

            row++;
            startTableRowPosition = row;
            row = row + 10;
            startTableRow = ++row + 2;
            row++;

            worksheet.Cells[row, 1].Value = "Answer";
            worksheet.Cells[row, 2].Value = "Percentage";
            worksheet.Cells[row, 3].Value = "Number off answer";
            worksheet.Cells["A" + row + ":C" + row].Style.Font.Bold = true;
            worksheet.Cells["A" + row + ":C" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A" + row + ":C" + row].Style.Fill.BackgroundColor.SetColor(
                System.Drawing.Color.LightGray
            );

            int index = 0;
            foreach (var ans in question.MyAnswers)
            {
                row++;
                if (ans.name != null || !string.IsNullOrEmpty(ans.name))
                    worksheet.Cells[row, 1].Value = ans.name;
                if (ans.value != null || !string.IsNullOrEmpty(ans.value))
                    worksheet.Cells[row, 2].Value = Math.Round(double.Parse(ans.value), 2);

                worksheet.Cells[row, 3].Value = ans.number;
                if (index % 2 != 0)
                {
                    worksheet.Cells["A" + row + ":C" + row].Style.Fill.PatternType =
                        ExcelFillStyle.Solid;
                    worksheet.Cells["A" + row + ":C" + row].Style.Fill.BackgroundColor.SetColor(
                        System.Drawing.Color.LightGray
                    );
                }
                index++;
            }
            int endTableRow = row;
            worksheet.Cells.AutoFitColumns();
            ExcelChart chart = worksheet.Drawings.AddChart(
                "Question" + question.Id,
                eChartType.ColumnClustered
            );
            chart.Legend.Remove();
            chart.YAxis.Title.Text = "%"; //give label to Y-axis of chart
            chart.YAxis.Title.Font.Size = 10;
            chart.SetSize(570, 200);
            chart.SetPosition(startTableRowPosition, 0, 0, 0);
            var consumptionCurrentYearSeries = chart.Series.Add(
                "B" + startTableRow + ":B" + endTableRow,
                "A" + startTableRow + ":A" + endTableRow
            );
            consumptionCurrentYearSeries.Header = "";
            return row;
        }

        public async Task SetNumberOfOpportunity(int id, int count)
        {
            await _surveyRepository.SetNumberOfOpportunity(id, count);
        }

        public Task<int> GetNumberOfOpportunity(int id)
        {
            return _surveyRepository.GetNumberOfOpportunity(id);
        }

        public async Task SendNotificationForSurvey(ILogger log)
        {
            await _surveyRepository.SendNotificationForSurvey(log);
        }
    }
}
