using System.Web.Mvc;

namespace HNStationaryStore.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_RevenueReport_Monthly",
                "Admin/RevenueReport/MonthlyReport/{year}",
                new { controller = "RevenueReport", action = "MonthlyReport", year = UrlParameter.Optional },
                namespaces: new[] { "HNStationaryStore.Areas.Admin.Controllers" }
            );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "HNStationaryStore.Areas.Admin.Controllers" }
            );

        }

    }
}