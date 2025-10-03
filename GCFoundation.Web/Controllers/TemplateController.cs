using GCFoundation.Components.Controllers;
using GCFoundation.Web.Resources;
using Microsoft.AspNetCore.Mvc;

namespace GCFoundation.Web.Controllers
{
    /// <summary>
    /// Controller responsible for serving the template demonstration or sample view.
    /// </summary>
    [Route("template")]
    public class TemplateController(ILogger<TemplateController> logger) : GCFoundationBaseController(logger)
    {
        /// <summary>
        /// Displays the default template view.
        /// </summary>
        /// <returns>
        /// The template view result.
        /// </returns>
        [HttpGet("")]
        public IActionResult Index()
        {
            SetPageTitle(Menu.Menu_Template);

            return View();
        }

        /// <summary>
        /// Displays the Basic page template demo page.
        /// </summary>
        /// <returns>
        /// The view for the Basic page template.
        /// </returns>
        [HttpGet("basic")]
        public IActionResult Basic()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Template.Index_Basic_Title}");

            return View();
        }

        /// <summary>
        /// Displays a page containing sample code for the use of a Basic page template.
        /// </summary>
        /// <returns>
        /// The view for the sample code for a Basic page template.
        /// </returns>
        [HttpGet("basic/code")]
        public IActionResult BasicCode()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Navigation.Nav_Template_Basic_Code}");

            return View("basic/code");
        }

        /// <summary>
        /// Displays a page containing a demo of a Basic page template.
        /// </summary>
        /// <returns>
        /// The view for the demo of a Basic page template.
        /// </returns>
        [HttpGet("basic/demo")]
        public IActionResult BasicDemo()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Navigation.Nav_Template_Basic_Demo}");

            return View("basic/demo");
        }

        /// <summary>
        /// Displays a page containing sample code for the use of a Basic page template with side navigation.
        /// </summary>
        /// <returns>
        /// The view for the sample code for a Basic page template with side navigation.
        /// </returns>
        [HttpGet("basic/side-navigation/code")]
        public IActionResult BasicSideNavCode()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Navigation.Nav_Template_Basic_Code}");

            return View("basic/sidenavigation-code");
        }

        /// <summary>
        /// Displays a page containing a demo of a Basic page template with side navigation.
        /// </summary>
        /// <returns>
        /// The view for the demo of a Basic page template with side navigation.
        /// </returns>
        [HttpGet("basic/side-navigation/demo")]
        public IActionResult BasicSideNavDemo()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Navigation.Nav_Template_Basic_Demo}");

            return View("basic/sidenavigation-demo");
        }

        /// <summary>
        /// Displays the Dashboard page template demo page.
        /// </summary>
        /// <returns>
        /// The view for the Dashboard page template.
        /// </returns>
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Template.Index_Dashboard_Title}");

            return View();
        }

        /// <summary>
        /// Displays the Error page template demo page.
        /// </summary>
        /// <returns>
        /// The view for the Error page template.
        /// </returns>
        [HttpGet("error")]
        public IActionResult Error()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Template.Index_Error_Title}");

            return View();
        }
    }
}