using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Extensions
{
    public static class ControllerExtensions
    {
        public static void MostrarMensagem(this Controller controlador, string texto, bool erro = false)
        {
            controlador.TempData["mensagem"] = MensagemViewModel.Serializar(
                texto, erro ? TipoMensagem.Erro : TipoMensagem.Informacao);
        }
    }
}