using System.Globalization;
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
        /// <param name="step">Optional step query used by the clickable stepper to jump directly to a step.</param>
        /// <returns>
        /// The view for the demo of a Stepper page template.
        /// </returns>
        [HttpGet("stepper/demo")]
        public IActionResult StepperDemo(int? step)
        {
            var model = new TemplateStepperFormViewModel();
            if (step.HasValue)
            {
                model.CurrentStep = Math.Clamp(step.Value, 1, model.TotalSteps);
            }

            SetStepperPageTitle(model);

            return View("stepper/demo", model);
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
            // Clamp posted state before rendering so invalid values cannot desync the stepper from the demo's fixed step count.
            var totalSteps = model.TotalSteps;
            var current = Math.Clamp(model.CurrentStep <= 0 ? 1 : model.CurrentStep, 1, totalSteps);

            if (string.Equals(nav, "next", StringComparison.OrdinalIgnoreCase))
            {
                // Only validate fields that belong to the step being left.
                FilterModelStateToStep(current);

                // Demo behavior: only advance when the current step is valid.
                // If invalid, keep the step so the user can correct fields and re-submit.
                if (ModelState.IsValid)
                {
                    // Final Submit (Agree to Terms): PRG to a dedicated success page.
                    if (current >= totalSteps)
                    {
                        return RedirectToAction(nameof(StepperDemoSuccess));
                    }

                    current = current + 1;
                }
            }
            else if (string.Equals(nav, "prev", StringComparison.OrdinalIgnoreCase))
            {
                // Going back never requires validation of the current step.
                ModelState.Clear();
                current = Math.Max(1, current - 1);
            }
            else
            {
                // Direct posts without nav (or unknown nav) still scope errors to the current step.
                FilterModelStateToStep(current);
            }

            // Persist the resolved step back onto the model so the view and hidden field stay in sync.
            model.CurrentStep = current;

            // Feed ModelState into BaseViewModel.Errors so fdcp-error-summary can render
            // the same unique messages as the field-level annotations (not GCDS defaults).
            SyncModelErrorsFromModelState(model);

            SetStepperPageTitle(model);

            return View("stepper/demo", model);
        }

        /// <summary>
        /// Displays the success confirmation after a valid Submit on the final stepper step.
        /// </summary>
        /// <returns>The stepper demo success view.</returns>
        [HttpGet("stepper/demo/success")]
        public IActionResult StepperDemoSuccess()
        {
            SetPageTitle($"{Resources.Template.Stepper_Demo_Success_Title} — {Resources.Template.Stepper_Demo_Name}");
            return View("stepper/success");
        }

        /// <summary>
        /// Copies ModelState errors onto the view model so <c>fdcp-error-summary</c> can emit
        /// <c>error-links</c> with the localized annotation messages.
        /// </summary>
        private void SyncModelErrorsFromModelState(TemplateStepperFormViewModel model)
        {
            model.ClearErrors();

            foreach (var entry in ModelState)
            {
                if (entry.Value is null || entry.Value.Errors.Count == 0)
                {
                    continue;
                }

                foreach (var error in entry.Value.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
                    {
                        model.AddError(entry.Key, error.ErrorMessage);
                    }
                }
            }
        }

        /// <summary>
        /// Removes ModelState errors for properties that do not belong to <paramref name="step"/>,
        /// so multi-step Required attributes on later/earlier pages do not block navigation.
        /// </summary>
        private void FilterModelStateToStep(int step)
        {
            if (!TemplateStepperFormViewModel.FieldsByStep.TryGetValue(step, out var stepFields))
            {
                return;
            }

            foreach (var key in ModelState.Keys.ToList())
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                var belongsToStep = stepFields.Any(field =>
                    key.Equals(field, StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith(field + "[", StringComparison.OrdinalIgnoreCase)
                    || key.StartsWith(field + ".", StringComparison.OrdinalIgnoreCase));

                if (!belongsToStep)
                {
                    ModelState.Remove(key);
                }
            }
        }

        /// <summary>
        /// Builds the document title for the Stepper demo so it leads with the current step phrase
        /// (e.g. "Step 2 of 5: Info") followed by the section label. Screen readers (NVDA, JAWS, VoiceOver)
        /// announce the document title on every page load, making this the most reliable way to communicate
        /// step transitions for users navigating with the Next/Previous buttons.
        /// </summary>
        private void SetStepperPageTitle(TemplateStepperFormViewModel model)
        {
            var stepLabel = model.CurrentStep switch
            {
                1 => Resources.Template.Stepper_Demo_Step1_Label,
                2 => Resources.Template.Stepper_Demo_Step2_Label,
                3 => Resources.Template.Stepper_Demo_Step3_Label,
                4 => Resources.Template.Stepper_Demo_Step4_Label,
                5 => Resources.Template.Stepper_Demo_Step5_Label,
                _ => string.Empty,
            };

            // Mirror the announcement format used by FDCPStepperTagHelper / Stepper.SR_CurrentStepAnnouncement
            // ("Step {0} of {1}: {2}."). Keeping this inline avoids exposing the components-internal resource
            // class; if it ever diverges, both sides must be updated together.
            var stepAnnouncement = string.Format(
                CultureInfo.CurrentCulture,
                "Step {0} of {1}: {2}.",
                model.CurrentStep,
                model.TotalSteps,
                stepLabel);

            SetPageTitle($"{stepAnnouncement} {Menu.Menu_Template} : {Resources.Template.Index_Stepper_Title}");
        }
        #endregion Stepper Page Template (Code, Demo) Controller Actions
    }
}