namespace MMA.WebApi.Shared.Helpers
{
    public class CalendarQueryModel
    {
        public int? DayFrom { get; set; }
        public int? DayTo { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public int? MonthFrom { get; set; }
        public int? MonthTo { get; set; }
    }
}
