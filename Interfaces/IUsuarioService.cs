namespace PizzaDelivery.Interfaces
{
    public interface IUsuarioService
    {
        (bool IsAuthenticated, string currentUserId, bool IsMaster) GetCurrentUser();

    }
}
