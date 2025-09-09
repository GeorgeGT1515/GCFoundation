using GCFoundation.Components.Controllers;
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
        /// <returns>The template view result.</returns>
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}