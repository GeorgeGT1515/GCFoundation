using GCFoundation.Common.Models;
using System.Globalization;

namespace GCFoundation.Tests.Components.Tests.Models
{
    public class MetaTagTests
    {
        [Fact]
        public void GetLocalizedContent_French_UsesContentFrWhenSet()
        {
            var tag = new MetaTag
            {
                Content = "Fallback",
                ContentFr = "En français",
                ContentEn = "In English"
            };

            using (new CultureScope("fr-CA"))
            {
                Assert.Equal("En français", tag.GetLocalizedContent());
            }
        }

        [Fact]
        public void GetLocalizedContent_French_FallsBackToContentWhenContentFrEmpty()
        {
            var tag = new MetaTag { Content = "Only neutral", ContentEn = "English only" };

            using (new CultureScope("fr-CA"))
            {
                Assert.Equal("Only neutral", tag.GetLocalizedContent());
            }
        }

        [Fact]
        public void GetLocalizedContent_English_UsesContentEnWhenSet()
        {
            var tag = new MetaTag
            {
                Content = "Fallback",
                ContentFr = "En français",
                ContentEn = "In English"
            };

            using (new CultureScope("en-CA"))
            {
                Assert.Equal("In English", tag.GetLocalizedContent());
            }
        }

        [Fact]
        public void GetLocalizedContent_English_FallsBackToContentWhenContentEnEmpty()
        {
            var tag = new MetaTag { Content = "Neutral", ContentFr = "Français" };

            using (new CultureScope("en-CA"))
            {
                Assert.Equal("Neutral", tag.GetLocalizedContent());
            }
        }

        [Fact]
        public void GetLocalizedContent_OtherCulture_UsesContentThenEnThenFr()
        {
            using (new CultureScope("de-DE"))
            {
                var neutralOnly = new MetaTag { Content = "N", ContentEn = "E", ContentFr = "F" };
                Assert.Equal("N", neutralOnly.GetLocalizedContent());

                var enFrOnly = new MetaTag { ContentEn = "E", ContentFr = "F" };
                Assert.Equal("E", enFrOnly.GetLocalizedContent());

                var frOnly = new MetaTag { ContentFr = "F" };
                Assert.Equal("F", frOnly.GetLocalizedContent());
            }
        }

        [Fact]
        public void Render_UsesCurrentUICulture_ForLocalizedContent()
        {
            var tag = new MetaTag
            {
                Name = "description",
                Content = "Neutral",
                ContentEn = "EN desc",
                ContentFr = "FR desc"
            };

            using (new CultureScope("fr-CA"))
            {
                Assert.Contains("content=\"FR desc\"", tag.Render());
            }

            using (new CultureScope("en-CA"))
            {
                Assert.Contains("content=\"EN desc\"", tag.Render());
            }
        }

        [Fact]
        public void Render_Charset_IgnoresLocalizedContent()
        {
            var tag = new MetaTag
            {
                Charset = "UTF-8",
                Content = "ignored",
                ContentEn = "ignored-en",
                ContentFr = "ignored-fr"
            };

            Assert.Equal("<meta charset=\"UTF-8\">", tag.Render());
        }

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