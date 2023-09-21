namespace MMA.WebApi.Shared.Models.OneAdnoc
{
    public class OfferAttachmentModel
    {
        public string Id { get; set; }
        public int OfferId { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string Extension { get; set; }
        public string ContentUrl { get; set; }
    }
}
