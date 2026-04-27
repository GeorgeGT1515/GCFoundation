using GCFoundation.Components.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GCFoundation.Web.Controllers
{
    /// <summary>
    /// Controller for handling the home page and related routes.
    /// </summary>
    [Route("home")]
    public class HomeController(ILogger<HomeController> logger) : GCFoundationBaseController(logger)
    {
        /// <summary>
        /// Displays the home page.
        /// </summary>
        /// <returns>The default view for the home page.</returns>
        [HttpGet("")]
        public IActionResult Index()
        {
            SetPageTitle($"{Resources.Home.Index_Title}");

            return View();
        }

        /// <summary>
        /// Displays the Accessibility statement.
        /// </summary>
        [HttpGet("accessibility-statement")]
        public IActionResult AccessibilityStatement()
        {
            SetPageTitle($"{Resources.Home.AccessibilityStatement_Title}");

            return View();
        }
    }
}