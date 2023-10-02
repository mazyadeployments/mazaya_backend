using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMA.WebApi.DataAccess.Models
{
    public class MazayaPaymentgateway : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }
        public string Bankref { get; set; }
        public string Device { get; set; }
        public string Deviceid { get; set; }
        public string Cardname { get; set; }
        public string Cardtype { get; set; }
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
        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}
