using cloudscribe.Web.Navigation;
using GCFoundation.Components.Enums;
using GCFoundation.Components.Models;
using GCFoundation.Components.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GCFoundation.Components.Services
{
    /// <summary>
    /// A permission resolver that determines navigation node visibility based on
    /// configurable rules, evaluated against cookie or session values on the current request.
    /// </summary>
    public class ConfigurableNavPermissionResolver : INavigationNodePermissionResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Dictionary<string, NavRule> _rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableNavPermissionResolver"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">An instance of <see cref="IHttpContextAccessor"/> used to access the current request.</param>
        /// <param name="options">An instance of <see cref="IOptions{TOptions}"/> containing the configured navigation rules.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
        public ConfigurableNavPermissionResolver(IHttpContextAccessor httpContextAccessor, IOptions<NavRulesOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));
            _httpContextAccessor = httpContextAccessor;
            _rules = options.Value.Rules;
        }

        /// <summary>
        /// Determines whether the given navigation node should be visible, based on any
        /// configured rule matching its key.
        /// </summary>
        /// <param name="menuNode">The navigation node being evaluated.</param>
        /// <returns><c>true</c> if the node should be shown; otherwise, <c>false</c>.</returns>
        public Task<bool> ShouldAllowView(TreeNode<NavigationNode> menuNode)
        {
            ArgumentNullException.ThrowIfNull(menuNode);
            var key = menuNode.Value.Key;

            if (!_rules.TryGetValue(key, out var rule))
                return Task.FromResult(true);

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return Task.FromResult(false);

            bool result = rule.Type switch
            {
                NavRuleType.Claim => httpContext.User?.HasClaim(rule.Name, rule.Value) ?? false,
                NavRuleType.Cookie => httpContext.Request.Cookies[rule.Name] == rule.Value,
                NavRuleType.Session => httpContext.Session.GetString(rule.Name) == rule.Value,
                _ => true
            };

            return Task.FromResult(result);
        }
    }
}