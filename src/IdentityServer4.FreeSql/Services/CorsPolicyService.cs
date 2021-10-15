using System;
using System.Threading.Tasks;
using IdentityServer4.Services;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using FreeSql;
using IdentityServer4.FreeSql.Storage.Interfaces;

namespace IdentityServer4.FreeSql.Services
{
    /// <summary>
    /// Implementation of ICorsPolicyService that consults the client configuration in the database for allowed CORS origins.
    /// </summary>
    /// <seealso cref="IdentityServer4.Services.ICorsPolicyService" />
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IConfigurationDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<CorsPolicyService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsPolicyService"/> class.
        /// </summary>
        /// <param name="accessor">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public CorsPolicyService(IHttpContextAccessor accessor
            , ILogger<CorsPolicyService> logger
            , IConfigurationDbContext context)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(IHttpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<CorsPolicyService>));
            _context = context ?? throw new ArgumentNullException(nameof(IConfigurationDbContext));
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            try
            {
                if (string.IsNullOrEmpty(origin))
                    return false;
                origin = origin.ToLowerInvariant();
                bool result = await _context.ClientCorsOrigins.Where(p => p.Origin == origin).AnyAsync();
                if (result)
                    _logger.LogDebug("Origin {origin} is allowed: {originAllowed}", origin, result);
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, $"Check origin '{origin}' failed, {ex.Message}");
                return false;
            }

        }
    }
}
