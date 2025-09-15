using GCFoundation.Components.Controllers;
using GCFoundation.Components.Models;
using GCFoundation.Components.Models.FormBuilder;
using GCFoundation.Web.Models;
using GCFoundation.Web.Models.Components;
using GCFoundation.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace GCFoundation.Web.Controllers
{
    /// <summary>
    /// Controller that handles requests related to reusable UI components.
    /// </summary>
    [Route("components")]
    public class ComponentsController(ILogger<ComponentsController> logger) : GCFoundationBaseController(logger)
    {
        /// <summary>
        /// Displays the main components overview page.
        /// </summary>
        /// <returns>
        /// The components index view.
        /// </returns>
        [HttpGet("")]
        public IActionResult Index()
        {
            SetPageTitle(Menu.Menu_Components);
            return View();
        }

        /// <summary>
        /// Displays the Badge component demo page.
        /// </summary>
        /// <returns>
        /// The Badge component view.
        /// </returns>
        [HttpGet("badge")]
        public IActionResult Badge()
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_Badge_Title}");

            var vm = BuildBadgeComponentViewModel();
            return View("Component", vm);
        }

        /// <summary>
        /// Displays the Card component demo page.
        /// </summary>
        /// <returns>
        /// The Card component view.
        /// </returns>
        [HttpGet("card")]
        public IActionResult Card()
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_Card_Title}");

            var vm = BuildCardComponentViewModel();
            return View("Component", vm);
        }

        /// <summary>
        /// Displays the Filtered Search component demo page.
        /// </summary>
        /// <returns>
        /// The Filtered Search component view.
        /// </returns>
        [HttpGet("filtered-search")]
        public IActionResult FilteredSearch()
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_FilteredSearch_Title}");

            var vm = BuildFilteredSearchComponentViewModel();
            return View("Component", vm);
        }

        /// <summary>
        /// Demonstrates a comprehensive example of a dynamic form with various question types and dependencies.
        /// This example showcases all possible dependency actions and their interactions.
        /// </summary>
        /// <returns>
        /// A view containing a form with various input types and complex dependencies.
        /// </returns>
        [HttpGet("form-builder")]
        public IActionResult FormBuilder()
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_FormBuilder_Title}");

            var vm = BuildFormBuilderTestViewModel();
            return View("FormBuilder", vm);
        }

        /// <summary>
        /// Handles the submission of the dynamic form builder example.
        /// Validates the form data and processes it if valid.
        /// </summary>
        /// <param name="vm">The view model containing form definition and user input.</param>
        /// <returns>
        /// Redirects to the example form builder view with a success message if valid; otherwise, returns the form view with validation errors.
        /// </returns>
        [HttpPost("form-builder")]
        [ValidateAntiForgeryToken]
        public IActionResult FormBuilder([FromForm] FormViewModel vm)
        {
            ArgumentNullException.ThrowIfNull(vm, nameof(vm));

            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_FormBuilder_Title}");

            var fbvm = BuildFormBuilderTestViewModel(vm);

            // Add the form data to the validation context
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(vm.Form)
            {
                Items = { ["FormData"] = vm.FormData }
            };

            // Validate the model including dependencies
            if (!TryValidateModel(vm, nameof(FormViewModel)))
            {
                // If validation fails, return to the form with error messages
                return View("FormBuilder", fbvm);
            }

            // Process the valid form data
            // TODO: Add your form processing logic here

            // Redirect to success page or show success message
            fbvm.SuccessMessage = Resources.Components.FormBuilder_SampleForm_SubmittedSuccessfully;
            return View("FormBuilder", fbvm);
        }

        /// <summary>
        /// Displays a sample form linked to properties of a class.
        /// </summary>
        /// <returns>
        /// The Form component view.
        /// </returns>
        [HttpGet("form")]
        public IActionResult Form()
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_Form_Title}");

            var vm = BuildFormComponentViewModel();
            return View("Form", vm);
        }

        /// <summary>
        /// Handles the POST request to test form validation.
        /// </summary>
        /// <param name="vm">The form data submitted by the user.</param>
        /// <returns>
        /// The Form component view with the POSTed model and validation results.
        /// </returns>
        [HttpPost("form")]
        [ValidateAntiForgeryToken]
        public IActionResult Form(FormTestViewModel vm)
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_Form_Title}");

            vm = BuildFormComponentViewModel(vm);
            if (ModelState.IsValid)
            {
                // Add your logic here if needed when the model is valid
                vm.SuccessMessage = Resources.Components.Form_SampleForm_SubmittedSuccessfully;
            }

            return View("Form", vm);
        }

        /// <summary>
        /// Displays the GC Design System components page.
        /// </summary>
        /// <returns>
        /// The GC Design System view.
        /// </returns>
        [HttpGet("gcds")]
        public IActionResult Gcds()
        {
            SetPageTitle(Menu.Menu_Components_GCDesign);

            return View();
        }

        /// <summary>
        /// Displays the Modal component demo page.
        /// </summary>
        /// <returns>
        /// The Modal component view.
        /// </returns>
        [HttpGet("modal")]
        public IActionResult Modal()
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_Modal_Title}");

            var vm = BuildModalComponentViewModel();
            return View("Modal", vm);
        }

        /// <summary>
        /// Displays the PageHeading component demo page.
        /// </summary>
        /// <returns>
        /// The PageHeading component view.
        /// </returns>
        [HttpGet("page-heading")]
        public IActionResult PageHeading()
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_PageHeading_Title}");

            var vm = BuildPageHeadingComponentViewModel();
            return View("Component", vm);
        }

        /// <summary>
        /// Displays the Stepper component demo page.
        /// </summary>
        /// <returns>
        /// The Stepper component view.
        /// </returns>
        [HttpGet("stepper")]
        public IActionResult Stepper()
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_Stepper_Title}");

            var vm = BuildStepperComponentViewModel();
            return View("Component", vm);
        }

        /// <summary>
        /// Displays the Table component demo page.
        /// </summary>
        /// <returns>
        /// The Table component view.
        /// </returns>
        [HttpGet("table")]
        public IActionResult Table()
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_Table_Title}");

            var vm = BuildTableComponentViewModel();
            return View("Component", vm);
        }

        /// <summary>
        /// Displays the User Login Partial component demo page.
        /// </summary>
        /// <returns>
        /// The User Login Partial component view.
        /// </returns>
        [HttpGet("user-login")]
        public IActionResult UserLogin()
        {
            SetPageTitle($"{Menu.Menu_Components} : {Resources.Components.Index_UserLoginPartial_Title}");
            ViewData["LoginPartialViewName"] = "_ExampleUserLogin";
            return View();
        }

        #region ViewModel Building
        private static ComponentViewModel BuildBadgeComponentViewModel()
        {
            var vm = new ComponentViewModel();

            vm.Name = Resources.Components.Badge_Name;
            vm.Notes = new List<string>()
            {
                Resources.Components.Badge_Notes_1,
                Resources.Components.Badge_Notes_2,
                Resources.Components.Badge_Notes_3
            };
            vm.Overview = Resources.Components.Badge_Overview;
            vm.Properties = new List<ComponentPropertyViewModel>()
            {
                new ComponentPropertyViewModel() { Name = "style", DataType = "FDCPBadgeStyle", Description = Resources.Components.Badge_Properties_Style },
                new ComponentPropertyViewModel() { Name = "inverted", DataType = "bool", Description = Resources.Components.Badge_Properties_Inverted },
                new ComponentPropertyViewModel() { Name = "start-content", DataType = "string", Description = Resources.Components.Badge_Properties_StartContent },
                new ComponentPropertyViewModel() { Name = "end-content", DataType = "string", Description = Resources.Components.Badge_Properties_EndContent },
                new ComponentPropertyViewModel() { Name = "tag-id", DataType = "string", Description = Resources.Components.Badge_Properties_TagId }
            };
            vm.SampleCodeSections = new List<ComponentSampleCodeSectionViewModel>()
            {
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Badge_Solid_Text, Id = Resources.Components.Badge_Solid_Anchor, PartialViewName = "Badge/_Solid", Title = Resources.Components.Badge_Solid_Title },
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Badge_Inverted_Text, Id = Resources.Components.Badge_Inverted_Anchor, PartialViewName = "Badge/_Inverted", Title = Resources.Components.Badge_Inverted_Title },
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Badge_Slot_Text, Id = Resources.Components.Badge_Slot_Anchor, PartialViewName = "Badge/_Slot", Title = Resources.Components.Badge_Slot_Title }
            };
            vm.Tag = "<fdcp-badge>";

            return vm;
        }
        private static ComponentViewModel BuildCardComponentViewModel()
        {
            var vm = new ComponentViewModel();

            vm.Name = Resources.Components.Card_Name;
            vm.Notes = new List<string>()
            {
                Resources.Components.Card_Notes_1,
                Resources.Components.Card_Notes_2,
                Resources.Components.Card_Notes_3,
                Resources.Components.Card_Notes_4
            };
            vm.Overview = Resources.Components.Card_Overview;
            vm.Properties = new List<ComponentPropertyViewModel>()
            {
                new ComponentPropertyViewModel() { Name = "tag-id", DataType = "string", Description = Resources.Components.Card_Properties_TagId },
                new ComponentPropertyViewModel() { Name = "width", DataType = "string", Description = Resources.Components.Card_Properties_Width },
                new ComponentPropertyViewModel() { Name = "height", DataType = "string", Description = Resources.Components.Card_Properties_Height },
                new ComponentPropertyViewModel() { Name = "border", DataType = "bool", Description = Resources.Components.Card_Properties_Border },
                new ComponentPropertyViewModel() { Name = "shadow", DataType = "bool", Description = Resources.Components.Card_Properties_Shadow },
                new ComponentPropertyViewModel() { Name = "image-top", DataType = "string", Description = Resources.Components.Card_Properties_ImageTop },
                new ComponentPropertyViewModel() { Name = "image-bottom", DataType = "string", Description = Resources.Components.Card_Properties_ImageBottom },
                new ComponentPropertyViewModel() { Name = "image-alt", DataType = "string", Description = Resources.Components.Card_Properties_ImageAlt },
                new ComponentPropertyViewModel() { Name = "horizontal", DataType = "bool", Description = Resources.Components.Card_Properties_Horizontal }
            };
            vm.SampleCodeSections = new List<ComponentSampleCodeSectionViewModel>()
            {
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Card_Basic_Text, Id = Resources.Components.Card_Basic_Anchor, PartialViewName = "Card/_Basic", Title = Resources.Components.Card_Basic_Title },
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Card_Horizontal_Text, Id = Resources.Components.Card_Horizontal_Anchor, PartialViewName = "Card/_Horizontal", Title = Resources.Components.Card_Horizontal_Title },
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Card_WithImages_Text, Id = Resources.Components.Card_WithImages_Anchor, PartialViewName = "Card/_WithImages", Title = Resources.Components.Card_WithImages_Title },
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Card_WithSlots_Text, Id = Resources.Components.Card_WithSlots_Anchor, PartialViewName = "Card/_WithSlots", Title = Resources.Components.Card_WithSlots_Title }
            };
            vm.Tag = "<fdcp-card>";

            return vm;
        }
        private static ComponentViewModel BuildFilteredSearchComponentViewModel()
        {
            var vm = new ComponentViewModel();

            vm.Name = Resources.Components.FilteredSearch_Name;
            //vm.Notes = new List<string>()
            //{
            //    Resources.Components.FilteredSearch_Notes_1,
            //    Resources.Components.FilteredSearch_Notes_2,
            //    Resources.Components.FilteredSearch_Notes_3
            //};
            vm.Overview = Resources.Components.FilteredSearch_Overview;
            vm.Properties = new List<ComponentPropertyViewModel>()
            {
                new ComponentPropertyViewModel() { Name = "title", DataType = "string", Description = Resources.Components.FilteredSearch_Properties_Title },
                new ComponentPropertyViewModel() { Name = "filters", DataType = "IEnumerable<SearchFilterCategory>", Description = Resources.Components.FilteredSearch_Properties_Filters }
            };
            vm.SampleCodeSections = new List<ComponentSampleCodeSectionViewModel>()
            {
                new ComponentSampleCodeSectionViewModel() { Id = Resources.Components.FilteredSearch_Basic_Anchor, PartialViewName = "FilteredSearch/_Basic", Title = Resources.Components.FilteredSearch_Basic_Title }
            };
            vm.Tag = "<fdcp-filters-box>";

            return vm;
        }
        private FormBuilderTestViewModel BuildFormBuilderTestViewModel(FormViewModel? vm = null)
        {
            var fbvm = new FormBuilderTestViewModel();
            if (vm != null)
                fbvm.SampleFormBuilder = vm;
            else
                fbvm.SampleFormBuilder = new FormViewModel() { Form = GenerateSampleFormDefinition() };

            fbvm.Name = Resources.Components.FormBuilder_Name;
            fbvm.Properties = new List<ComponentPropertyViewModel>()
            {
                new ComponentPropertyViewModel() { Name = "form", DataType = "GCFoundation.Components.Models.FormBuilder.FormDefinition", Description = Resources.Components.FormBuilder_Properties_Form }            };
            fbvm.SampleCodeSections = new List<ComponentSampleCodeSectionViewModel>()
            {
                new ComponentSampleCodeSectionViewModel() { Id = Resources.Components.FormBuilder_SampleForm_Anchor, Description = Resources.Components.FormBuilder_SampleForm_Description, Title = Resources.Components.FormBuilder_SampleForm_Title },
            };
            fbvm.Tag = "<fdcp-form-builder>";

            return fbvm;
        }
        private static FormTestViewModel BuildFormComponentViewModel(FormTestViewModel? vm = null)
        {
            if (vm == null)
                vm = new FormTestViewModel();

            vm.Name = Resources.Components.Form_Name;
            //vm.Notes = new List<string>()
            //{
            //    Resources.Components.Form_Notes_1,
            //    Resources.Components.Form_Notes_2,
            //    Resources.Components.Form_Notes_3
            //};
            vm.Overview = Resources.Components.Form_Overview;
            vm.Properties = new List<ComponentPropertyViewModel>()
            {
                new ComponentPropertyViewModel() { Name = "for", DataType = "GCFoundation.Components.Models.BaseViewModel", Description = Resources.Components.Form_Properties_For },
                new ComponentPropertyViewModel() { Name = "method", DataType = "string", DefaultValue = "POST", Description = Resources.Components.Form_Properties_Method },
                new ComponentPropertyViewModel() { Name = "action", DataType = "string", Description = Resources.Components.Form_Properties_Action }
            };
            vm.SampleCodeSections = new List<ComponentSampleCodeSectionViewModel>()
            {
                new ComponentSampleCodeSectionViewModel() { Id = Resources.Components.Form_SampleForm_Anchor, Title = Resources.Components.Form_SampleForm_Title },
            };
            vm.Tag = "<fdcp-form>";

            return vm;
        }
        private static ComponentViewModel BuildModalComponentViewModel()
        {
            var vm = new ComponentViewModel();

            vm.Name = Resources.Components.Modal_Name;
            vm.Notes = new List<string>()
            {
                Resources.Components.Modal_Notes_1,
                Resources.Components.Modal_Notes_2,
                Resources.Components.Modal_Notes_3,
                Resources.Components.Modal_Notes_4,
                Resources.Components.Modal_Notes_5
            };
            vm.Overview = Resources.Components.Modal_Overview;
            vm.Properties = new List<ComponentPropertyViewModel>()
            {
                new ComponentPropertyViewModel() { Name = "id", DataType = "string", DefaultValue = "modal", Description = Resources.Components.Modal_Properties_Id },
                new ComponentPropertyViewModel() { Name = "title", DataType = "string", DefaultValue = "Modal Title", Description = Resources.Components.Modal_Properties_Title },
                new ComponentPropertyViewModel() { Name = "centered", DataType = "bool", DefaultValue = "true", Description = Resources.Components.Modal_Properties_Centered },
                new ComponentPropertyViewModel() { Name = "scrollable", DataType = "bool", Description = Resources.Components.Modal_Properties_Scrollable },
                new ComponentPropertyViewModel() { Name = "size", DataType = "ModalSize", DefaultValue = "ModalSize.Default", Description = Resources.Components.Modal_Properties_Size },
                new ComponentPropertyViewModel() { Name = "show-close-button", DataType = "bool", DefaultValue = "true", Description = Resources.Components.Modal_Properties_ShowCloseButton },
                new ComponentPropertyViewModel() { Name = "is-static-backdrop", DataType = "bool", Description = Resources.Components.Modal_Properties_IsStaticBackdrop },
                new ComponentPropertyViewModel() { Name = "session-timeout", DataType = "int", Description = Resources.Components.Modal_Properties_SessionTimeout },
                new ComponentPropertyViewModel() { Name = "reminder-time", DataType = "int", Description = Resources.Components.Modal_Properties_ReminderTime },
                new ComponentPropertyViewModel() { Name = "refresh-url", DataType = "Uri", Description = Resources.Components.Modal_Properties_RefreshUrl },
                new ComponentPropertyViewModel() { Name = "logout-url", DataType = "Uri", Description = Resources.Components.Modal_Properties_LogoutUrl }
            };
            vm.SampleCodeSections = new List<ComponentSampleCodeSectionViewModel>()
            {
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Modal_Basic_Text, Id = Resources.Components.Modal_Basic_Anchor, PartialViewName = "Modal/_Basic", Title = Resources.Components.Modal_Basic_Title },
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Modal_Session_Text, Id = Resources.Components.Modal_Session_Anchor, PartialViewName = "Modal/_Session", Title = Resources.Components.Modal_Session_Title }
            };
            vm.Tag = "<fdcp-modal>";

            return vm;
        }
        private static ComponentViewModel BuildPageHeadingComponentViewModel()
        {
            var vm = new ComponentViewModel();

            vm.Name = Resources.Components.PageHeading_Name;
            vm.Notes = new List<string>()
            {
                Resources.Components.PageHeading_Notes_1,
                Resources.Components.PageHeading_Notes_2,
                Resources.Components.PageHeading_Notes_3
            };
            vm.Overview = Resources.Components.PageHeading_Overview;
            vm.Properties = new List<ComponentPropertyViewModel>()
            {
                new ComponentPropertyViewModel() { Name = "title", DataType = "string", Description = Resources.Components.PageHeading_Properties_Title },
                new ComponentPropertyViewModel() { Name = "description", DataType = "string", Description = Resources.Components.PageHeading_Properties_Description },
                new ComponentPropertyViewModel() { Name = "size", DataType = "PageHeadingSize", DefaultValue = "PageHeadingSize.Default", Description = Resources.Components.PageHeading_Properties_Size },
                new ComponentPropertyViewModel() { Name = "src", DataType = "string", Description = Resources.Components.PageHeading_Properties_Src },
                new ComponentPropertyViewModel() { Name = "text-emphasis", DataType = "bool", DefaultValue = "false", Description = Resources.Components.PageHeading_Properties_TextEmphasis }
            };
            vm.SampleCodeSections = new List<ComponentSampleCodeSectionViewModel>()
            {
                new ComponentSampleCodeSectionViewModel() { Id = Resources.Components.PageHeading_Basic_Anchor, PartialViewName = "PageHeading/_Basic", Title = Resources.Components.PageHeading_Basic_Title }
            };
            vm.Tag = "<fdcp-page-heading>";

            return vm;
        }
        private static ComponentViewModel BuildStepperComponentViewModel()
        {
            var vm = new ComponentViewModel();

            vm.Name = Resources.Components.Stepper_Name;
            vm.Notes = new List<string>()
            {
                Resources.Components.Stepper_Notes_1,
                Resources.Components.Stepper_Notes_2,
                Resources.Components.Stepper_Notes_3,
                Resources.Components.Stepper_Notes_4
            };
            vm.Overview = Resources.Components.Stepper_Overview;
            vm.Properties = new List<ComponentPropertyViewModel>()
            {
                new ComponentPropertyViewModel() { Name = "current-step", DataType = "int", DefaultValue = "1", Description = Resources.Components.Stepper_Properties_CurrentStep },
                new ComponentPropertyViewModel() { Name = "steps", DataType = "IEnumerable<StepperStep>", Description = Resources.Components.Stepper_Properties_Steps },
                new ComponentPropertyViewModel() { Name = "StepperStep.StepNumber", DataType = "int", Description = Resources.Components.Stepper_Properties_StepperStep_StepNumber },
                new ComponentPropertyViewModel() { Name = "StepperStep.Label", DataType = "string", Description = Resources.Components.Stepper_Properties_StepperStep_Label },
                new ComponentPropertyViewModel() { Name = "StepperStep.DisplayMode", DataType = "string", DefaultValue = "StepDisplayMode.Number", Description = Resources.Components.Stepper_Properties_StepperStep_DisplayMode },
                new ComponentPropertyViewModel() { Name = "StepperStep.IsLink", DataType = "string", Description = Resources.Components.Stepper_Properties_StepperStep_IsLink },
                new ComponentPropertyViewModel() { Name = "StepperStep.LinkUrl", DataType = "string", Description = Resources.Components.Stepper_Properties_StepperStep_LinkUrl },
                new ComponentPropertyViewModel() { Name = "StepperStep.CompletedIconHtml", DataType = "string", Description = Resources.Components.Stepper_Properties_StepperStep_CompletedIconHtml },
                new ComponentPropertyViewModel() { Name = "StepperStep.InProgressIconHtml", DataType = "string", Description = Resources.Components.Stepper_Properties_StepperStep_InProgressIconHtml },
                new ComponentPropertyViewModel() { Name = "StepperStep.NotStartedIconHtml", DataType = "string", Description = Resources.Components.Stepper_Properties_StepperStep_NotStartedIconHtml }
            };
            vm.SampleCodeSections = new List<ComponentSampleCodeSectionViewModel>()
            {
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Stepper_Basic_Text, Id = Resources.Components.Stepper_Basic_Anchor, PartialViewName = "Stepper/_Basic", Title = Resources.Components.Stepper_Basic_Title },
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Stepper_WithIcons_Anchor, Id = Resources.Components.Stepper_WithIcons_Anchor, PartialViewName = "Stepper/_WithIcons", Title = Resources.Components.Stepper_WithIcons_Title },
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Stepper_WithLinks_Anchor, Id = Resources.Components.Stepper_WithLinks_Anchor, PartialViewName = "Stepper/_WithLinks", Title = Resources.Components.Stepper_WithLinks_Title }
            };
            vm.Tag = "<fdcp-stepper>";

            return vm;
        }
        private static ComponentViewModel BuildTableComponentViewModel()
        {
            var vm = new ComponentViewModel();

            vm.Name = Resources.Components.Table_Name;
            //vm.Notes = new List<string>()
            //{
            //    Resources.Components.Table_Notes_1,
            //    Resources.Components.Table_Notes_2,
            //    Resources.Components.Table_Notes_3,
            //    Resources.Components.Table_Notes_4
            //};
            //vm.Overview = Resources.Components.Table_Overview;
            vm.Properties = new List<ComponentPropertyViewModel>()
            {
                new ComponentPropertyViewModel() { Name = "ajax-url", DataType = "string", Description = Resources.Components.Table_Properties_AjaxUrl },
                new ComponentPropertyViewModel() { Name = "columns", DataType = "IEnumerable<TabulatorColumn>", Description = Resources.Components.Table_Properties_Columns },
                new ComponentPropertyViewModel() { Name = "data", DataType = "IEnumerable<object>", Description = Resources.Components.Table_Properties_Data },
                new ComponentPropertyViewModel() { Name = "id", DataType = "IEnumerable<StepperStep>", Description = Resources.Components.Table_Properties_Id },
                new ComponentPropertyViewModel() { Name = "pagination-size", DataType = "int", DefaultValue = "10", Description = Resources.Components.Table_Properties_PaginationSize },
                new ComponentPropertyViewModel() { Name = "use-static-data", DataType = "bool", DefaultValue = "false", Description = Resources.Components.Table_Properties_UseStaticData },
            };
            vm.SampleCodeSections = new List<ComponentSampleCodeSectionViewModel>()
            {
                new ComponentSampleCodeSectionViewModel() { Description = Resources.Components.Table_Basic_Text, Id = Resources.Components.Table_Basic_Anchor, PartialViewName = "Table/_Basic", Title = Resources.Components.Stepper_Basic_Title }
            };
            vm.Tag = "<fdcp-tabulator-table>";

            return vm;
        }
        private FormDefinition GenerateSampleFormDefinition()
        {
            var form = new FormDefinition
            {
                Id = "demo-form",
                Title = "Dynamic Form Demo",
                Action = Url.Action("FormBuilder", "Components") ?? "",
                Method = "post",
                SubmitButtonText = "Submit Form",
                Sections = new List<FormSection>
                {
                    new FormSection
                    {
                        Title = "Personal Information",
                        Hint = "Please provide your basic information",
                        Questions = new List<FormQuestion>
                        {
                            new FormQuestion
                            {
                                Id = "fullName",
                                Label = "Full Name",
                                Type = QuestionType.Text,
                                IsRequired = true,
                                Hint = "Enter your full legal name"
                            },
                            new FormQuestion
                            {
                                Id = "email",
                                Label = "Email Address",
                                Type = QuestionType.Email,
                                IsRequired = true,
                                Hint = "We'll use this for communication"
                            }
                        }
                    },
                    new FormSection
                    {
                        Title = "Location Information",
                        Hint = "Tell us where you're located",
                        Questions = new List<FormQuestion>
                        {
                            // Country selection with cascading dependencies
                            new FormQuestion
                            {
                                Id = "country",
                                Label = "Country of Residence",
                                Type = QuestionType.Dropdown,
                                IsRequired = true,
                                Options = new List<QuestionOption>
                                {
                                    new() { Id = "ca", Value = "CA", Label = "Canada" },
                                    new() { Id = "us", Value = "US", Label = "United States" },
                                    new() { Id = "other", Value = "OTHER", Label = "Other" }
                                }
                            },
                            // Province field - shows when Canada is selected
                            new FormQuestion
                            {
                                Id = "province",
                                Label = "Province",
                                Type = QuestionType.Dropdown,
                                Dependencies = new List<QuestionDependency>
                                {
                                    new()
                                    {
                                        SourceQuestionId = "country",
                                        TargetQuestionId = "province",
                                        Action = DependencyAction.Show,
                                        TriggerValue = "CA"
                                    },
                                    new()
                                    {
                                        SourceQuestionId = "country",
                                        TargetQuestionId = "province",
                                        Action = DependencyAction.Require,
                                        TriggerValue = "CA"
                                    }
                                },
                                Options = new List<QuestionOption>
                                {
                                    new() { Id = "on", Value = "ON", Label = "Ontario" },
                                    new() { Id = "qc", Value = "QC", Label = "Quebec" },
                                    new() { Id = "bc", Value = "BC", Label = "British Columbia" }
                                }
                            },
                            // State field - shows when US is selected
                            new FormQuestion
                            {
                                Id = "state",
                                Label = "State",
                                Type = QuestionType.Dropdown,
                                Dependencies = new List<QuestionDependency>
                                {
                                    new()
                                    {
                                        SourceQuestionId = "country",
                                        TargetQuestionId = "state",
                                        Action = DependencyAction.Show,
                                        TriggerValue = "US"
                                    },
                                    new()
                                    {
                                        SourceQuestionId = "country",
                                        TargetQuestionId = "state",
                                        Action = DependencyAction.Require,
                                        TriggerValue = "US"
                                    }
                                },
                                Options = new List<QuestionOption>
                                {
                                    new() { Id = "ny", Value = "NY", Label = "New York" },
                                    new() { Id = "ca", Value = "CA", Label = "California" },
                                    new() { Id = "tx", Value = "TX", Label = "Texas" }
                                }
                            },
                            // Other Country field - shows when Other is selected
                            new FormQuestion
                            {
                                Id = "otherCountry",
                                Label = "Specify Country",
                                Type = QuestionType.Text,
                                Dependencies = new List<QuestionDependency>
                                {
                                    new()
                                    {
                                        SourceQuestionId = "country",
                                        TargetQuestionId = "otherCountry",
                                        Action = DependencyAction.Show,
                                        TriggerValue = "OTHER"
                                    },
                                    new()
                                    {
                                        SourceQuestionId = "country",
                                        TargetQuestionId = "otherCountry",
                                        Action = DependencyAction.Require,
                                        TriggerValue = "OTHER"
                                    }
                                }
                            }
                        }
                    },
                    new FormSection
                    {
                        Title = "Service Selection",
                        Hint = "Choose your service preferences",
                        Questions = new List<FormQuestion>
                        {
                            // Service Type with multiple dependent fields
                            new FormQuestion
                            {
                                Id = "serviceType",
                                Label = "Service Type",
                                Type = QuestionType.Radio,
                                IsRequired = true,
                                Options = new List<QuestionOption>
                                {
                                    new() { Id = "basic", Value = "BASIC", Label = "Basic Service" },
                                    new() { Id = "premium", Value = "PREMIUM", Label = "Premium Service" },
                                    new() { Id = "custom", Value = "CUSTOM", Label = "Custom Service" }
                                }
                            },
                            // Premium features - shown and required for premium service
                            new FormQuestion
                            {
                                Id = "premiumFeatures",
                                Label = "Premium Features",
                                Type = QuestionType.Checkbox,
                                Hint = "Select the premium features you want",
                                Dependencies = new List<QuestionDependency>
                                {
                                    new()
                                    {
                                        SourceQuestionId = "serviceType",
                                        TargetQuestionId = "premiumFeatures",
                                        Action = DependencyAction.Show,
                                        TriggerValue = "PREMIUM"
                                    },
                                    new()
                                    {
                                        SourceQuestionId = "serviceType",
                                        TargetQuestionId = "premiumFeatures",
                                        Action = DependencyAction.Require,
                                        TriggerValue = "PREMIUM"
                                    }
                                },
                                Options = new List<QuestionOption>
                                {
                                    new() { Id = "feature1", Value = "24_7_SUPPORT", Label = "24/7 Support" },
                                    new() { Id = "feature2", Value = "PRIORITY", Label = "Priority Service" },
                                    new() { Id = "feature3", Value = "ADVANCED", Label = "Advanced Features" }
                                }
                            },
                            // Custom requirements - shown and enabled for custom service
                            new FormQuestion
                            {
                                Id = "customRequirements",
                                Label = "Custom Requirements",
                                Type = QuestionType.TextArea,
                                Hint = "Describe your custom service needs",
                                Dependencies = new List<QuestionDependency>
                                {
                                    new()
                                    {
                                        SourceQuestionId = "serviceType",
                                        TargetQuestionId = "customRequirements",
                                        Action = DependencyAction.Show,
                                        TriggerValue = "CUSTOM"
                                    },
                                    new()
                                    {
                                        SourceQuestionId = "serviceType",
                                        TargetQuestionId = "customRequirements",
                                        Action = DependencyAction.Require,
                                        TriggerValue = "CUSTOM"
                                    }
                                }
                            },
                            // Budget range - disabled for basic service
                            new FormQuestion
                            {
                                Id = "budgetRange",
                                Label = "Budget Range",
                                Type = QuestionType.Dropdown,
                                Dependencies = new List<QuestionDependency>
                                {
                                    new()
                                    {
                                        SourceQuestionId = "serviceType",
                                        TargetQuestionId = "budgetRange",
                                        Action = DependencyAction.Disable,
                                        TriggerValue = "BASIC"
                                    }
                                },
                                Options = new List<QuestionOption>
                                {
                                    new() { Id = "budget1", Value = "UNDER_1000", Label = "Under $1,000" },
                                    new() { Id = "budget2", Value = "1000_5000", Label = "$1,000 - $5,000" },
                                    new() { Id = "budget3", Value = "OVER_5000", Label = "Over $5,000" }
                                }
                            }
                        }
                    },
                    new FormSection
                    {
                        Title = "Additional Information",
                        Questions = new List<FormQuestion>
                        {
                            // Contact preference with dependent phone field
                            new FormQuestion
                            {
                                Id = "contactPreference",
                                Label = "Preferred Contact Method",
                                Type = QuestionType.Radio,
                                IsRequired = true,
                                Options = new List<QuestionOption>
                                {
                                    new() { Id = "email", Value = "EMAIL", Label = "Email" },
                                    new() { Id = "phone", Value = "PHONE", Label = "Phone" }
                                }
                            },
                            // Phone number - required when phone is selected
                            new FormQuestion
                            {
                                Id = "phoneNumber",
                                Label = "Phone Number",
                                Type = QuestionType.Text,
                                Dependencies = new List<QuestionDependency>
                                {
                                    new()
                                    {
                                        SourceQuestionId = "contactPreference",
                                        TargetQuestionId = "phoneNumber",
                                        Action = DependencyAction.Show,
                                        TriggerValue = "PHONE"
                                    },
                                    new()
                                    {
                                        SourceQuestionId = "contactPreference",
                                        TargetQuestionId = "phoneNumber",
                                        Action = DependencyAction.Require,
                                        TriggerValue = "PHONE"
                                    }
                                }
                            },
                            // Terms acceptance
                            new FormQuestion
                            {
                                Id = "termsAccepted",
                                Label = "Terms and Conditions",
                                Type = QuestionType.Checkbox,
                                IsRequired = true,
                                Options = new List<QuestionOption>
                                {
                                    new() { Id = "terms", Value = "true", Label = "I accept the terms and conditions" }
                                }
                            }
                        }
                    }
                }
            };
            return form;
        }
        #endregion ViewModel Building
    }
}