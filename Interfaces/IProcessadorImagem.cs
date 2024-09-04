using PizzaDelivery.Services;

namespace PizzaDelivery.Interfaces;

public interface IProcessadorImagem
{
    Task<bool> SalvarUploadImagemAsync(string caminhoArquivoImagem, IFormFile imagem);
    Task<bool> ExcluirImagemAsync(string caminhoArquivoImagem);
    Task<bool> AplicarEfeitoAsync(string caminhoArquivoImagem, EfeitoImagem efeito);
}