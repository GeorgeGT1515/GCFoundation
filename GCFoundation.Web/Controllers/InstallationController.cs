using GCFoundation.Components.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GCFoundation.Web.Controllers
{
    /// <summary>
    /// Controller responsible for handling installation-related pages.
    /// </summary>
    [Route("installation")]
    public class InstallationController(ILogger<InstallationController> logger) : GCFoundationBaseController(logger)
    {
        /// <summary>
        /// Displays the main installation page.
        /// </summary>
        /// <returns>The default view for the installation page.</returns>
        [HttpGet("")]
        public IActionResult Index()
        {
            SetPageTitle($"{Resources.Installation.Index_Page_Title}");

            return View();
        }

        /// <summary>
        /// Displays the global resources configuration demo page.
        /// </summary>
        /// <returns>
        /// The global resources configuration view.
        /// </returns>
        [HttpGet("global-resources")]
        public IActionResult GlobalResources()
        {
            SetPageTitle($"{Resources.Installation.GlobalResources_Page_Title}");

            return View();
        }

        /// <summary>
        /// Displays a page containing a list of standard translation terms to be used in GCFoundation applications.
        /// </summary>
        /// <returns>
        /// The standard translations view.
        /// </returns>
        [HttpGet("translations")]
        public IActionResult Translations()
        {
            SetPageTitle($"{Resources.Installation.Translations_Page_Title}");

            return View();
        }
    }
}