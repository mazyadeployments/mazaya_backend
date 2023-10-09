using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace MMA.WebApi.Controllers
{

    public abstract class BaseController : ControllerBase
    {
        private string userId = "SystemGenerated";

        protected string UserId
        {
            get
            {
                userId = (User?.Identity?.IsAuthenticated == true && User?.FindFirst(JwtRegisteredClaimNames.Jti).Value != null) ? User.FindFirst(JwtRegisteredClaimNames.Jti).Value : "";
                return userId;
            }
        }

        protected string CompanyId
        {
            get
            {
                string compnayId = (User?.Identity?.IsAuthenticated == true && User?.FindFirst("companyId").Value != null) ? User.FindFirst("companyId").Value : "";
                return compnayId;
            }
        }


        //protected List<UserPermissionModel> UserPermissions
        //{
        //    get
        //    {
        //        if (User!=null && User.Identity!=null && User.Identity.IsAuthenticated)
        //        {
        //            List<UserPermissionModel> per = new List<UserPermissionModel>();

        //            foreach(var p in User.FindAll("Permissions"))
        //            {
        //                per.Add(JsonConvert.DeserializeObject<UserPermissionModel>(p.Value));
        //            }

        //            return  per ;
        //        }

        //        return null;
        //    }
        //}

    }
}