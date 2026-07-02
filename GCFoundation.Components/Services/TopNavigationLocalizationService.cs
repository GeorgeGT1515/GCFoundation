using GCFoundation.Components.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCFoundation.Components.Services
{
    public class TopNavigationLocalizationService<T> : ITopNavigationLocalizationService
    {
        private readonly IStringLocalizer<T> _localizer;

        public TopNavigationLocalizationService(IStringLocalizer<T> localizer)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public string GetLocalizeValue(string key)
        {
            var localizedString = _localizer[key];
            return localizedString.Value;
        }
    }
}
