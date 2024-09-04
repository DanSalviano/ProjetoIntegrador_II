namespace PizzaDelivery.Interfaces
{
    public interface IAuditable
    {
        string UsuarioIdInclusao { get; set; }
        DateTime? DataInclusao { get; set; }
        string UsuarioIdAlteracao { get; set; }
        DateTime? DataAlteracao { get; set; }
    }
}