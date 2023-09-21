using System;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.OfferSuggestions
{
    public class OfferSuggestionModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public UserSuggestionModel User { get; set; }
        public string Text { get; set; }
        public OfferSuggestionStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class UserSuggestionModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
