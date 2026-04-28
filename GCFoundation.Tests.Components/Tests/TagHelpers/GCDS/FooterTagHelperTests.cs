using GCFoundation.Common.Models;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Globalization;
using System.Text.Json;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    /// <summary>
    /// Verifies footer link URLs after <see cref="Microsoft.AspNetCore.Mvc.IUrlHelper.Content"/> resolution
    /// for absolute, site-relative, and application-root (<c>~/</c>) paths configured on contextual and sub-links.
    /// </summary>
    public class FooterTagHelperTests
    {
        [Fact]
        public void Process_ContextualLinks_AbsoluteUri_PassesThroughUnchanged()
        {
            const string href = "https://github.com/tbs-imtd/GCFoundation";
            using var _ = new CultureScope("en-CA");
            var output = RunProcess(
                PathString.Empty,
                contextualLinks: [new FooterLink { Label = "Repo", Link = href }]);

            var json = GetJsonAttribute(output, "contextual-links");
            using var doc = JsonDocument.Parse(json);
            Assert.Equal(href, doc.RootElement.GetProperty("Repo").GetString());
        }

        [Fact]
        public void Process_ContextualLinks_SiteRelativePath_PassesThroughUnchanged()
        {
            const string href = "/en/home/accessibility-statement";
            using var _ = new CultureScope("en-CA");
            var output = RunProcess(
                PathString.Empty,
                contextualLinks: [new FooterLink { Label = "Accessibility", Link = href }]);

            var json = GetJsonAttribute(output, "contextual-links");
            using var doc = JsonDocument.Parse(json);
            Assert.Equal(href, doc.RootElement.GetProperty("Accessibility").GetString());
        }

        [Fact]
        public void Process_ContextualLinks_ApplicationRelativeTilde_ResolvesPathBase()
        {
            const string configured = "~/en/home/";
            const string pathBase = "/myapp";
            using var _ = new CultureScope("en-CA");
            var output = RunProcess(
                new PathString(pathBase),
                contextualLinks: [new FooterLink { Label = "Home", Link = configured }]);

            var json = GetJsonAttribute(output, "contextual-links");
            using var doc = JsonDocument.Parse(json);
            Assert.Equal($"{pathBase}/en/home/", doc.RootElement.GetProperty("Home").GetString());
        }

        [Fact]
        public void Process_SubLinks_AbsoluteUri_PassesThroughUnchanged()
        {
            const string href = "https://www.canada.ca/en.html";
            using var _ = new CultureScope("fr-CA");
            var output = RunProcess(
                PathString.Empty,
                subLinks: [new FooterLink { Label = "Canada", LabelFr = "Canada", Link = href, LinkFr = href }]);

            var json = GetJsonAttribute(output, "sub-links");
            using var doc = JsonDocument.Parse(json);
            Assert.Equal(href, doc.RootElement.GetProperty("Canada").GetString());
        }

        [Fact]
        public void Process_SubLinks_SiteRelativePath_PassesThroughUnchanged()
        {
            const string href = "/fr/accueil/";
            using var _ = new CultureScope("fr-CA");
            var output = RunProcess(
                PathString.Empty,
                subLinks: [new FooterLink { Label = "Accueil", LinkFr = href }]);

            var json = GetJsonAttribute(output, "sub-links");
            using var doc = JsonDocument.Parse(json);
            Assert.Equal(href, doc.RootElement.GetProperty("Accueil").GetString());
        }

        [Fact]
        public void Process_SubLinks_ApplicationRelativeTilde_ResolvesPathBase()
        {
            const string configured = "~/fr/accueil/";
            const string pathBase = "/virtual";
            using var _ = new CultureScope("fr-CA");
            var output = RunProcess(
                new PathString(pathBase),
                subLinks: [new FooterLink { LabelFr = "Accueil", LinkFr = configured }]);

            var json = GetJsonAttribute(output, "sub-links");
            using var doc = JsonDocument.Parse(json);
            Assert.Equal($"{pathBase}/fr/accueil/", doc.RootElement.GetProperty("Accueil").GetString());
        }

        private static TagHelperOutput RunProcess(
            PathString pathBase,
            IEnumerable<FooterLink>? contextualLinks = null,
            IEnumerable<FooterLink>? subLinks = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.PathBase = pathBase;

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor());

            var viewContext = new ViewContext(
                actionContext,
                Mock.Of<IView>(),
                new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()),
                new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>()),
                TextWriter.Null,
                new HtmlHelperOptions());

            var helper = new FooterTagHelper(new UrlHelperFactory())
            {
                ViewContext = viewContext,
                ContextualLinks = contextualLinks,
                SubLinks = subLinks
            };

            var output = new TagHelperOutput(
                "gcds-footer",
                new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            helper.Process(CreateContext(), output);
            return output;
        }

        private static string GetJsonAttribute(TagHelperOutput output, string name)
        {
            var attr = output.Attributes.FirstOrDefault(a => a.Name == name);
            Assert.NotNull(attr);
            return attr.Value?.ToString() ?? throw new InvalidOperationException($"Missing {name} value.");
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private sealed class CultureScope : IDisposable
        {
            private readonly CultureInfo _previousCulture;
            private readonly CultureInfo _previousUiCulture;

            public CultureScope(string cultureName)
            {
                _previousCulture = CultureInfo.CurrentCulture;
                _previousUiCulture = CultureInfo.CurrentUICulture;
                var culture = CultureInfo.GetCultureInfo(cultureName);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }

            public void Dispose()
            {
                CultureInfo.CurrentCulture = _previousCulture;
                CultureInfo.CurrentUICulture = _previousUiCulture;
            }
        }
    }
}
