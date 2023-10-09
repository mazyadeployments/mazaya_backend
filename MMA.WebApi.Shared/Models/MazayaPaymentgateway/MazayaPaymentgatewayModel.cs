using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Models.MazayaEcardmain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMA.WebApi.Shared.Models.MazayaPaymentgateway
{
    public class MazayaPaymentgatewayModel
    {
        public int Id { get; set; }
        public string response_code { get; set; }
        public string card_number { get; set; }
        public string card_holder_name { get; set; }
        public string payment_option { get; set; }
        public DateTime expiry_date { get; set; }
        public string customer_ip { get; set; }
        public string eci { get; set; }
        public string fort_id { get; set; }
        public string response_msg { get; set; }
        public string authorization_code { get; set; }
        public string merchant_reference { get; set; }
        public string cust_email { get; set; }
        public string Bankref { get; set; }
        public string Device { get; set; }
        public string Deviceid { get; set; }
        public string Cardname { get; set; }
        public string Cardno { get; set; }
        public DateTime PayDate { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public string Paystatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
