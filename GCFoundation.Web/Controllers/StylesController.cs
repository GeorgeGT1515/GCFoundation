using GCFoundation.Components.Controllers;
using GCFoundation.Web.Resources;
using Microsoft.AspNetCore.Mvc;

namespace GCFoundation.Web.Controllers
{
    /// <summary>
    /// Controller that handles requests related to styling utilities and documentation.
    /// </summary>
    [Route("styles")]
    public class StylesController(ILogger<StylesController> logger) : GCFoundationBaseController(logger)
    {
        /// <summary>
        /// Displays the main styles overview page with utilities documentation.
        /// </summary>
        /// <returns>
        /// The styles index view containing documentation for all available utility classes.
        /// </returns>
        [HttpGet("")]
        public IActionResult Index()
        {
            SetPageTitle(Menu.Menu_Styles);
            return View();
        }
    }
}