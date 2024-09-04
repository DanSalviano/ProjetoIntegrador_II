using PizzaDelivery.Interfaces;
using System.Security.Claims;

namespace PizzaDelivery.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public (bool IsAuthenticated, string currentUserId, bool IsMaster) GetCurrentUser()
        {
            (bool IsAuthenticated, string currentUserId, bool IsMaster) currentUser = (false, string.Empty, false);

            if (_httpContextAccessor?.HttpContext == null)
                return currentUser;

            currentUser = (
            _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated,

            _httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value,

            _httpContextAccessor.HttpContext.User.Claims
            .Any(a => a.Type == ClaimTypes.Role && a.Value == "administrador")
        );

            return currentUser;
        }
    }
}
