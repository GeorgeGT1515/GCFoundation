using GCFoundation.Common.Models;
using System.Globalization;

namespace GCFoundation.Tests.Components.Tests.Models
{
    public class FooterLinkTests
    {
        [Fact]
        public void GetLocalizedLabelAndLink_French_UsesFrWhenSet()
        {
            var link = new FooterLink
            {
                Label = "Neutral",
                LabelEn = "English",
                LabelFr = "Français",
                Link = "/n",
                LinkEn = "/en",
                LinkFr = "/fr"
            };

            using (new CultureScope("fr-CA"))
            {
                Assert.Equal("Français", link.GetLocalizedLabel());
                Assert.Equal("/fr", link.GetLocalizedLink());
            }
        }

        [Fact]
        public void GetLocalizedLabelAndLink_French_FallsBackToNeutralWhenFrEmpty()
        {
            var link = new FooterLink
            {
                Label = "Only neutral",
                LabelEn = "English only",
                Link = "/neutral",
                LinkEn = "/en-only"
            };

            using (new CultureScope("fr-CA"))
            {
                Assert.Equal("Only neutral", link.GetLocalizedLabel());
                Assert.Equal("/neutral", link.GetLocalizedLink());
            }
        }

        [Fact]
        public void GetLocalizedLabelAndLink_English_UsesEnWhenSet()
        {
            var link = new FooterLink
            {
                Label = "Neutral",
                LabelEn = "English",
                LabelFr = "Français",
                Link = "/n",
                LinkEn = "/en",
                LinkFr = "/fr"
            };

            using (new CultureScope("en-CA"))
            {
                Assert.Equal("English", link.GetLocalizedLabel());
                Assert.Equal("/en", link.GetLocalizedLink());
            }
        }

        [Fact]
        public void GetLocalizedLabelAndLink_SingleNeutralPair_WorksForEnAndFr()
        {
            var link = new FooterLink { Label = "Contact", Link = "/contact" };

            using (new CultureScope("en-CA"))
            {
                Assert.Equal("Contact", link.GetLocalizedLabel());
                Assert.Equal("/contact", link.GetLocalizedLink());
            }

            using (new CultureScope("fr-CA"))
            {
                Assert.Equal("Contact", link.GetLocalizedLabel());
                Assert.Equal("/contact", link.GetLocalizedLink());
            }
        }

        [Fact]
        public void GetLocalizedLabelAndLink_OtherCulture_UsesNeutralThenEnThenFr()
        {
            using (new CultureScope("de-DE"))
            {
                var neutralOnly = new FooterLink { Label = "N", LabelEn = "E", LabelFr = "F", Link = "1", LinkEn = "2", LinkFr = "3" };
                Assert.Equal("N", neutralOnly.GetLocalizedLabel());
                Assert.Equal("1", neutralOnly.GetLocalizedLink());

                var enFrOnly = new FooterLink { LabelEn = "E", LabelFr = "F", LinkEn = "/e", LinkFr = "/f" };
                Assert.Equal("E", enFrOnly.GetLocalizedLabel());
                Assert.Equal("/e", enFrOnly.GetLocalizedLink());

                var frOnly = new FooterLink { LabelFr = "F", LinkFr = "/f" };
                Assert.Equal("F", frOnly.GetLocalizedLabel());
                Assert.Equal("/f", frOnly.GetLocalizedLink());
            }
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
