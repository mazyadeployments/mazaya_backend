namespace MMA.WebApi.Models
{
    public class B2CResponseModel
    {
        public string Id_token { get; set; }
        public string Token_type { get; set; }
        public long Not_before { get; set; }
        public int Id_token_expires_in { get; set; }
        public string Profile_info { get; set; }
        public string Scope { get; set; }
    }
}
