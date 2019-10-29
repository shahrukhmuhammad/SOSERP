using System.Web.Mvc;

namespace WebApp.Areas.Listing
{
    public class ListingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Listing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Listing_default",
                "Listing/{controller}/{action}/{id}",
                new { controller = "Tender", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}