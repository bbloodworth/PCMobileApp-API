using System.Web.Mvc;

namespace CchWebAPI.Areas.Animation
{
    public class AnimationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Animation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
        }
    }
}

