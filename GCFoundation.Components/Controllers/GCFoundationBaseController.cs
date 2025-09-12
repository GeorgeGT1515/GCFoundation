using GCFoundation.Common.Models;
using GCFoundation.Components.Enums;
using GCFoundation.Components.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GCFoundation.Components.Controllers
{
    /// <summary>
    /// Provides base functionality for controllers within the application.
    /// This class includes common methods for setting page notifications, menu views, and page titles.
    /// </summary>
    public abstract class GCFoundationBaseController : Controller
    {
        private readonly ILogger<GCFoundationBaseController> _logger;

        /// <summary>
        /// Collection of custom meta tags to be rendered in the shared layout.
        /// Derived controllers can add to this list to inject per-page meta tags.
        /// </summary>
        protected IList<MetaTag> MetaTags { get; } = new List<MetaTag>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GCFoundationBaseController"/> class.
        /// </summary>
        protected GCFoundationBaseController(ILogger<GCFoundationBaseController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Adds a strongly-typed meta tag entry to the collection.
        /// </summary>
        /// <param name="tag">The meta tag to inject.</param>
        /// <remarks>If a meta tag with the same name (or property) already exists, overwrite it.</remarks>
        protected void AddMetaTag(MetaTag tag)
        {
            if (tag != null)
            {
                if (!string.IsNullOrEmpty(tag.Name) && MetaTags.Any(t => t.Name == tag.Name))
                    MetaTags.Remove(MetaTags.First(t => t.Name == tag.Name));
                if (!string.IsNullOrEmpty(tag.Property) && MetaTags.Any(t => t.Property == tag.Property))
                    MetaTags.Remove(MetaTags.First(t => t.Property == tag.Property));
                MetaTags.Add(tag);
            }
        }

        /// <summary>
        /// Sets a page-level notification to be displayed at the top of the page.
        /// </summary>
        /// <param name="notification">An object containing the notification title, message, and alert type.</param>
        protected void SetPageNotification(PageNotification notification)
        {
            ViewData["PageNotification"] = notification;
        }

        /// <summary>
        /// Sets a success-type notification to be displayed at the top of the page.
        /// </summary>
        /// <param name="title">The title of the success message.</param>
        /// <param name="message">The message content of the success notification.</param>
        protected void SetPageSuccessNotification(string title, string message)
        {
            ViewData["PageNotification"] = new PageNotification
            {
                Title = title,
                Message = message,
                AlertType = AlertType.Success
            };
        }

        /// <summary>
        /// Sets the HTML page title to appear in the browser's title bar or tab.
        /// </summary>
        /// <param name="title">The title of the page.</param>
        protected void SetPageTitle(string title)
        {
            ViewData["Title"] = title;
        }

        /// <summary>
        /// Sets the name of the partial view to be used for the page menu.
        /// </summary>
        /// <param name="viewMenu">The name of the partial view for the menu.</param>
        protected void SetViewMenu(string viewMenu)
        {
            ViewData["MenuPartialViewName"] = viewMenu;
        }

        /// <summary>
        /// After the action executes, expose any collected custom meta tags to the view via ViewData.
        /// The shared layout will read these and render them after the global meta tags.
        /// </summary>
        /// <param name="context">The action executed context.</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            ViewData["MetaTags"] = MetaTags;
            base.OnActionExecuted(context);
        }
    }
}