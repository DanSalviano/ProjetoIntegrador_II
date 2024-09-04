namespace PizzaDelivery.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
        void RollBack();
    }
}