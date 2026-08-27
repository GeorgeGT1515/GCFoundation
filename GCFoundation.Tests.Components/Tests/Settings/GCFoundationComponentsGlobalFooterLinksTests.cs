using GCFoundation.Common.Settings;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace GCFoundation.Tests.Components.Tests.Settings
{
    /// <summary>
    /// Verifies <see cref="GCFoundationComponentsSettings.GlobalFooterContextualLinks"/> and
    /// <see cref="GCFoundationComponentsSettings.GlobalFooterSubLinks"/> bind from JSON configuration
    /// with absolute (<c>https://</c>), site-relative (<c>/</c>), and application-root (<c>~/</c>) URLs.
    /// </summary>
    public class GCFoundationComponentsGlobalFooterLinksTests
    {
        private const string SampleJson =
            """
            {
              "FoundationComponentsSettings": {
                "GlobalFooterContextualLinks": [
                  {
                    "label": "External",
                    "link": "https://example.com/path"
                  },
                  {
                    "label": "Relative",
                    "link": "/en/home/"
                  },
                  {
                    "label": "App root",
                    "link": "~/en/home/"
                  }
                ],
                "GlobalFooterSubLinks": [
                  {
                    "labelEn": "Sub CDN",
                    "labelFr": "Sous CDN",
                    "linkEn": "https://cdn.example.com/asset",
                    "linkFr": "https://cdn.example.com/asset"
                  },
                  {
                    "labelEn": "Sub relative EN",
                    "labelFr": "Sous relatif FR",
                    "linkEn": "/en/other/",
                    "linkFr": "/fr/autre/"
                  },
                  {
                    "labelEn": "Sub tilde EN",
                    "labelFr": "Sous tilde FR",
                    "linkEn": "~/en/pages/x",
                    "linkFr": "~/fr/pages/x"
                  }
                ]
              }
            }
            """;

        [Fact]
        public void Bind_FromJson_GlobalFooterContextualLinks_PreservesConfiguredUrlShapes()
        {
            var settings = BindSettings();

            Assert.Equal(3, settings.GlobalFooterContextualLinks.Count);

            Assert.Equal("External", settings.GlobalFooterContextualLinks[0].Label);
            Assert.Equal("https://example.com/path", settings.GlobalFooterContextualLinks[0].Link);

            Assert.Equal("Relative", settings.GlobalFooterContextualLinks[1].Label);
            Assert.Equal("/en/home/", settings.GlobalFooterContextualLinks[1].Link);

            Assert.Equal("App root", settings.GlobalFooterContextualLinks[2].Label);
            Assert.Equal("~/en/home/", settings.GlobalFooterContextualLinks[2].Link);
        }

        [Fact]
        public void Bind_FromJson_GlobalFooterSubLinks_PreservesConfiguredUrlShapes()
        {
            var settings = BindSettings();

            Assert.Equal(3, settings.GlobalFooterSubLinks.Count);

            Assert.Equal("https://cdn.example.com/asset", settings.GlobalFooterSubLinks[0].LinkEn);
            Assert.Equal("https://cdn.example.com/asset", settings.GlobalFooterSubLinks[0].LinkFr);

            Assert.Equal("/en/other/", settings.GlobalFooterSubLinks[1].LinkEn);
            Assert.Equal("/fr/autre/", settings.GlobalFooterSubLinks[1].LinkFr);

            Assert.Equal("~/en/pages/x", settings.GlobalFooterSubLinks[2].LinkEn);
            Assert.Equal("~/fr/pages/x", settings.GlobalFooterSubLinks[2].LinkFr);
        }

        private static GCFoundationComponentsSettings BindSettings()
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SampleJson));
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var settings = configuration.GetSection("FoundationComponentsSettings").Get<GCFoundationComponentsSettings>();

            Assert.NotNull(settings);
            return settings;
        }
    }
}
