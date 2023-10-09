using System.Collections.Generic;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Helpers
{
    public static class ECartDataMapper
    {
        public static Dictionary<string, string> mappedEmails { get; set; }

        public static Dictionary<int, string> MembershipNameDictionary = new Dictionary<
            int,
            string
        >()
        {
            [(int)OfferMembership.OffersAndDiscounts] = "العروض والخصومات",
            [(int)OfferMembership.FamilyEntertainment] = "الترفيه العائلي",
            [(int)OfferMembership.HealtAndLeisure] = "الرياضة والمتعة",
            [(int)OfferMembership.LeisureAndFamilyEntertainment] = "المتعة والترفيه العائلي"
        };
        public static Dictionary<int, string> MembershipPicture = new Dictionary<int, string>()
        {
            [(int)OfferMembership.OffersAndDiscounts] = "images/ECards/dark_blue_card.jpg",
            [(int)OfferMembership.FamilyEntertainment] = "images/ECards/yellow_card.jpg",
            [(int)OfferMembership.HealtAndLeisure] = "images/ECards/light_blue_card.jpg",
            [(int)OfferMembership.LeisureAndFamilyEntertainment] = "images/ECards/grey_card.jpg"
        };
    }
}
