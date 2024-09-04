using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace PizzaDelivery.Models
{
    public static class Inicializador
    {
        private static async Task InicializarPerfis(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("administrador").Result)
            {
                var perfil = new IdentityRole();
                perfil.Name = "administrador";
                await roleManager.CreateAsync(perfil);
            }
            if (!roleManager.RoleExistsAsync("gerente").Result)
            {
                var perfil = new IdentityRole();
                perfil.Name = "gerente";
                await roleManager.CreateAsync(perfil);
            }
            if (!roleManager.RoleExistsAsync("pizzaiolo").Result)
            {
                var perfil = new IdentityRole();
                perfil.Name = "pizzaiolo";
                await roleManager.CreateAsync(perfil);
            }
            if (!roleManager.RoleExistsAsync("entregador").Result)
            {
                var perfil = new IdentityRole();
                perfil.Name = "entregador";
                await roleManager.CreateAsync(perfil);
            }
        }

        private static void InicializarUsuarios(UserManager<UsuarioModel> userManager)
        {
            if (userManager.FindByNameAsync("admin").Result == null)
            {
                var usuario = new UsuarioModel
                {
                    UserName = "admin",
                    Email = "admin@email.com",
                    NomeCompleto = "Administrador",
                    DataNascimento = DateTime.Now,
                    PhoneNumber = string.Empty,
                    CPF = string.Empty,
                    EmailConfirmed = true,
                    CidadeId = Guid.NewGuid().ToString(),
                    IsAtivo = true,
                };
                usuario.UsuarioIdInclusao = usuario.Id; //atribui o proprio Id;
                var resultado = userManager.CreateAsync(usuario, "123456").Result;

                if (resultado.Succeeded)
                {
                    userManager.AddToRoleAsync(usuario, "administrador").Wait();
                    var dataNascimentoClaim = new Claim(ClaimTypes.DateOfBirth,
                        usuario.DataNascimento.Date.ToShortDateString());
                    userManager.AddClaimAsync(usuario, dataNascimentoClaim).Wait();
                }
            }
        }

        public static async Task InicializarIdentity(
            UserManager<UsuarioModel> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await InicializarPerfis(roleManager);
            InicializarUsuarios(userManager);
        }
    }
}