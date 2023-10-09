using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.Roadshow;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MMA.WebApi.Shared.Models.PDFModel
{
    public class ProposalPDFModel : PDFBaseModel
    {
        [Required]
        public ICollection<RoadshowVoucherModel> RoadshowVouchers { get; set; }
        public Declares.RoadshowProposalStatus Status { get; set; }
    }
}
