using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnalyticsWeb.Controllers
{
    using Ionx.GatewayAnalytics;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            PwgAnalyticsHandler handler = new PwgAnalyticsHandler();
            ViewBag.Gateways = handler.GetGatewayAnalytics();
            return View();
        }

        [Route(@"{gatewayGuid:guid}")]
        public ActionResult Gateway(Guid gatewayGuid)
        {
            PwgAnalyticsHandler handler = new PwgAnalyticsHandler();
            var gateway = handler.GetGatewayAnalytics(gatewayGuid);
            return View("Gateway", gateway);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}