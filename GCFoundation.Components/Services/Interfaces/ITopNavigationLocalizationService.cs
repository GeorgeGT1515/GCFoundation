using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace GCFoundation.Components.Services.Interfaces
{
    /// <summary>
    /// Interface for a service that provides localization for navigation menu items.
    /// </summary>
    public interface ITopNavigationLocalizationService
    {
        /// <summary>
        /// Gets the localized value for a given key.
        /// </summary>
        /// <param name="key">The key for which the localized value is to be retrieved.</param>
        /// <returns>The localized value associated with the given key.</returns>
        string GetLocalizeValue(string key);
    }
}
