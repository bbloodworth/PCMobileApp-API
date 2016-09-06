using System.Web.Mvc;

namespace CchWebAPI.Areas.v2
{
    public class v2AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "v2";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)  {
        }
    }
}