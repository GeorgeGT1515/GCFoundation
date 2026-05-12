using GCFoundation.Components.Controllers;
using GCFoundation.Components.Models;
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


        /// <summary>
        /// Displays the old template view (deprecated).
        /// </summary>
        /// <returns>
        /// The old template view result.
        /// </returns>
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
        /// Displays the CRUD page template demo page.
        /// </summary>
        /// <returns>
        /// The view for the CRUD page template.
        /// </returns>
        [HttpGet("crud")]
        public IActionResult Crud()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Template.Index_Crud_Title}");

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
        /// Displays the Language Chooser page template demo page.
        /// </summary>
        /// <returns>
        /// The view for the Language Chooser page template.
        /// </returns>
        [HttpGet("language-chooser")]
        public IActionResult LanguageChooser()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Template.Index_LanguageChooser_Title}");

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

        #region Dashboard Page Template (Code, Demo) Controller Actions
        /// <summary>
        /// Displays a page containing sample code for the use of a generic Dashboard page template.
        /// </summary>
        /// <returns>
        /// The view for the sample code for a generic Dashboard page template.
        /// </returns>
        [HttpGet("dashboard/code")]
        public IActionResult DashboardCode()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Navigation.Nav_Template_Dashboard_Code}");

            return View("dashboard/code");
        }

        /// <summary>
        /// Displays a page containing a demo of a generic Dashboard page template.
        /// </summary>
        /// <returns>
        /// The view for the demo of a generic Dashboard page template.
        /// </returns>
        [HttpGet("dashboard/demo")]
        public IActionResult DashboardDemo()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Navigation.Nav_Template_Dashboard_Demo}");

            return View("dashboard/demo");
        }
        #endregion Dashboard Page Template (Code, Demo) Controller Actions

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

        #region Language Chooser Page Template (Code, Demo) Controller Actions
        /// <summary>
        /// Displays a page containing sample code for the use of a generic Language Chooser page template.
        /// </summary>
        /// <returns>
        /// The view for the sample code for a generic Language Chooser page template.
        /// </returns>
        [HttpGet("language-chooser/code")]
        public IActionResult LanguageChooserCode()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Navigation.Nav_Template_LanguageChooser_Code}");

            return View("languagechooser/code");
        }

        /// <summary>
        /// Displays a page containing a demo of a generic Language Chooser page template.
        /// </summary>
        /// <returns>
        /// The view for the demo of a generic Language Chooser page template.
        /// </returns>
        [HttpGet("language-chooser/demo")]
        public IActionResult LanguageChooserDemo()
        {
            SetPageTitle($"{Menu.Menu_Template} : {Resources.Navigation.Nav_Template_LanguageChooser_Demo}");

            LanguageChooserModel model = new()
            {
                ApplicationTitleEn = "GCFoundation Demo",
                ApplicationTitleFr = "Démo GCFoundation",
                EnglishAction = Url.Action("Index", "Home", new { culture = "en" }) ?? "#",
                FrenchAction = Url.Action("Index", "Home", new { culture = "fr" }) ?? "#",
                TermLinkEn = "",
                TermLinkFr = ""
            };

            return View("~/Views/Language/Index.cshtml", model);
        }
        #endregion Language Chooser Page Template (Code, Demo) Controller Actions

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

            return View("stepper/demo", new TemplateStepperFormViewModel { CurrentStep = 1 });
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

            // Clamp posted state before rendering so invalid values cannot desync the stepper from the demo's fixed step count.
            var totalSteps = model.TotalSteps;
            var current = Math.Clamp(model.CurrentStep <= 0 ? 1 : model.CurrentStep, 1, totalSteps);

            if (string.Equals(nav, "next", StringComparison.OrdinalIgnoreCase))
            {
                // Demo behavior: only advance when the current post is valid.
                // If invalid, keep the step so the user can correct fields and re-submit.
                if (ModelState.IsValid)
                    current = Math.Min(totalSteps, current + 1);
            }
            else if (string.Equals(nav, "prev", StringComparison.OrdinalIgnoreCase))
            {
                current = Math.Max(1, current - 1);
            }

            // Persist the resolved step back onto the model so the view and hidden field stay in sync.
            model.CurrentStep = current;

            return View("stepper/demo", model);
        }
        #endregion Stepper Page Template (Code, Demo) Controller Actions
    }
}