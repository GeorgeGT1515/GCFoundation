using GCFoundation.Components.Controllers;
using GCFoundation.Web.Models.Template;
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


        [HttpGet("old-index")]
        public IActionResult OldIndex()
        {
            SetPageTitle($"{Menu.Menu_Template}");

            return View("_OldIndex");
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

        /// <summary>
        /// Displays the Stepper page template documentation page.
        /// </summary>
        /// <returns>
        /// The documentation view for the Stepper page template.
        /// </returns>
        [HttpGet("stepper")]
        public IActionResult Stepper()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Template.Index_Stepper_Title}");

            return View();
        }

        #region Basic Page Template (Code, Demo) Controller Actions
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
        #endregion Basic Page Template Controller Actions

        #region Error Page Template (Code, Demo) Controller Actions
        /// <summary>
        /// Displays a page containing sample code for the use of a generic Error page template.
        /// </summary>
        /// <returns>
        /// The view for the sample code for a generic Error page template.
        /// </returns>
        [HttpGet("error/code")]
        public IActionResult ErrorCode()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Navigation.Nav_Template_Error_Code}");

            return View("error/code");
        }

        /// <summary>
        /// Displays a page containing a demo of a generic Error page template.
        /// </summary>
        /// <returns>
        /// The view for the demo of a generic Error page template.
        /// </returns>
        [HttpGet("error/demo")]
        public IActionResult ErrorDemo()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Navigation.Nav_Template_Error_Demo}");

            return View("error/demo");
        }
        #endregion Error Page Template (Code, Demo) Controller Actions

        #region Stepper Page Template (Code, Demo) Controller Actions
        /// <summary>
        /// Displays a page containing sample code for the Stepper page template.
        /// </summary>
        /// <returns>
        /// The view for the sample code for a Stepper page template.
        /// </returns>
        [HttpGet("stepper/code")]
        public IActionResult StepperCode()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Template.Index_Stepper_Title}");

            return View("stepper/code");
        }

        /// <summary>
        /// Displays a page containing a demo of a Stepper page template.
        /// </summary>
        /// <returns>
        /// The view for the demo of a Stepper page template.
        /// </returns>
        [HttpGet("stepper/demo")]
        public IActionResult StepperDemo()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Template.Index_Stepper_Title}");

            return View("stepper/demo", new TemplateStepperFormViewModel());
        }

        /// <summary>
        /// Handles postbacks from the Stepper demo form.
        /// </summary>
        /// <param name="model">Bound form model.</param>
        /// <param name="nav">Navigation intent (prev/next).</param>
        /// <returns>The demo view.</returns>
        [HttpPost("stepper/demo")]
        [ValidateAntiForgeryToken]
        public IActionResult StepperDemo(TemplateStepperFormViewModel model, string? nav)
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Template.Index_Stepper_Title}");

            // This demo intentionally posts back to the same view regardless of navigation intent.
            // Validation errors will be surfaced via the error summary in the view.
            return View("stepper/demo", model);
        }
        #endregion Stepper Page Template (Code, Demo) Controller Actions
    }
}