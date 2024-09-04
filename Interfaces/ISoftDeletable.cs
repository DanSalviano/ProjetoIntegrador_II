namespace PizzaDelivery.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsAtivo { get; set; }
        bool IsExcluido { get; set; }
        string UsuarioIdExclusao { get; set; }
        DateTime? DataExclusao { get; set; }
    }
}
