namespace PizzaDelivery.ViewModels
{
    public class UsuarioViewAllModel
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public bool isMaster { get; set; }
        public IList<string> Roles { get; set; }
    }
}