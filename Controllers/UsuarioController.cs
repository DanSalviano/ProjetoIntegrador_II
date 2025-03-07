using AutoMapper;
using System.Text;
using System.Data;
using PizzaDelivery.Models;
using PizzaDelivery.Services;
using PizzaDelivery.ViewModels;
using PizzaDelivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Interfaces;
using System.Linq.Dynamic.Core;
using static PizzaDelivery.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace PizzaDelivery.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<UsuarioModel> _userManager;
        private readonly SignInManager<UsuarioModel> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IMemoryCache _memoryCache;
        private readonly IEmailService _emailService;
        private readonly IEstadoRepository _estadoRepository;
        private readonly ICidadeRepository _cidadeRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IMapper _mapper;
        public UsuarioController(
            IConfiguration Configuration,
            UserManager<UsuarioModel> userManager,
            SignInManager<UsuarioModel> signInManager,
            RoleManager<IdentityRole> roleManager,
            IMemoryCache memoryCache,
            IEmailService emailService,
            IEstadoRepository EstadoRepository,
            ICidadeRepository CidadeRepository,
            IShoppingCartRepository shoppingCartRepository,
            IMapper Mapper) =>
            (configuration, _userManager, _signInManager, _roleManager, _memoryCache, _emailService, _estadoRepository, _cidadeRepository, _shoppingCartRepository, _mapper) = (Configuration, userManager, signInManager, roleManager, memoryCache, emailService, EstadoRepository, CidadeRepository, shoppingCartRepository, Mapper);


        [Authorize(Roles = "administrador,gerente")]
        public IActionResult Index() => View();


        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> ViewAllNormal()
        {
            var usuarios = (from usuario in _userManager.Users
                            join cidade in _cidadeRepository.GetAll() on usuario.CidadeId equals cidade.EstadoId
                            join estado in _estadoRepository.GetAll() on cidade.EstadoId equals estado.Id
                            select new UsuarioViewAllModel
                            {
                                Nome = usuario.NomeCompleto,
                                Login = usuario.UserName,
                                Estado = estado.Estado,
                                Cidade = cidade.Cidade,
                                Telefone = usuario.PhoneNumber,
                                Email = usuario.Email,
                                //isMaster = admins.Contains(usuario.UserName),
                                Id = usuario.Id
                            }).AsNoTracking().ToListAsync();

            var admins = (await _userManager.GetUsersInRoleAsync("administrador")).Select(u => u.UserName);
            ViewBag.Administradores = admins;
            ViewBag.Estados = await _estadoRepository.GetOptionsAsync("[TODOS]");
            return View(usuarios);
        }

        [HttpPost]
        [Authorize(Roles = "administrador,gerente")]
        public async Task<JsonResult> GetData(DataTableAjaxPostModel tablemodel)
        {
            List<UsuarioViewAllModel> resultado = [];
            //Server Side Parameter
            int start = Convert.ToInt32(tablemodel.Start);
            int length = Convert.ToInt32(tablemodel.Length);
            string sortColumnName = tablemodel.Columns[tablemodel.Order[0].Column].Data;
            string sortDirection = tablemodel.Order[0].Dir.ToLower();
            int totalrows = 0;
            int totalrowsafterfiltering = 0;
            string chave = $"usuarios{start}{length}{sortColumnName}{sortDirection}{tablemodel.Columns[2].Search.Value}{tablemodel.Columns[3].Search.Value}{tablemodel.Search.Value}";

            var response = await _memoryCache.GetOrCreateAsync(chave, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(1);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
                entry.SetPriority(CacheItemPriority.Normal);

                var admins = await _userManager.GetUsersInRoleAsync("administrador");
                var usuarios = from usuario in _userManager.Users
                               join cidade in _cidadeRepository.GetAll() on usuario.CidadeId equals cidade.Id
                               join estado in _estadoRepository.GetAll() on cidade.EstadoId equals estado.Id
                               where !string.IsNullOrEmpty(usuario.CPF)
                               orderby usuario.NomeCompleto
                               select new UsuarioViewAllModel
                               {
                                   Nome = usuario.NomeCompleto,
                                   Login = usuario.UserName,
                                   Estado = estado.Estado,
                                   Cidade = cidade.Cidade,
                                   Telefone = usuario.PhoneNumber,
                                   Email = usuario.Email,
                                   isMaster = admins.Select(u => u.UserName).Contains(usuario.UserName),
                                   Id = usuario.Id
                               };

                totalrows = await usuarios.CountAsync();

                if (totalrows > 0)
                {
                    //custom filtering

                    // Configura a query para filtrar por Estado selecionado
                    if (!string.IsNullOrEmpty(tablemodel.Columns[2].Search.Value))
                        usuarios = usuarios.Where(x => x.Estado.ToLower().Contains(tablemodel.Columns[2].Search.Value.ToLower()));

                    // Configura a query para filtrar por Cidade selecionada
                    if (!string.IsNullOrEmpty(tablemodel.Columns[3].Search.Value))
                        usuarios = usuarios.Where(x => x.Cidade.ToLower().Contains(tablemodel.Columns[3].Search.Value.ToLower()));

                    // Configura a query para filtrar por pesquisa
                    if (!string.IsNullOrEmpty(tablemodel.Search.Value))
                        usuarios = usuarios.Where(x => x.Nome.ToLower().Contains(tablemodel.Search.Value.ToLower())
                        || x.Login.Contains(tablemodel.Search.Value)
                        || x.Email.ToLower().Contains(tablemodel.Search.Value.ToLower())
                        || x.Telefone.Contains(tablemodel.Search.Value)
                        );

                    totalrowsafterfiltering = await usuarios.CountAsync();

                    //sorting & paging
                    usuarios = usuarios.OrderBy(sortColumnName + " " + sortDirection).Skip(start).Take(length);

                    //Func<UsuarioViewAllModel, object> orderBy = u => u.Nome;
                    //switch (tablemodel.Order[0].Column)
                    //{
                    //    case 0:
                    //        orderBy = u => u.Nome;
                    //        break;
                    //    case 1:
                    //        orderBy = u => u.Login;
                    //        break;
                    //    case 3:
                    //        orderBy = u => u.Cidade;
                    //        break;
                    //    case 4:
                    //        orderBy = u => u.Email;
                    //        break;
                    //    case 5:
                    //        orderBy = u => u.Telefone;
                    //        break;
                    //    default:
                    //        break;
                    //}

                    ////paging
                    //if (sortDirection == "asc")
                    //    usuarios = usuarios.OrderBy(orderBy).Skip(start).Take(length).AsQueryable();
                    //else
                    //    usuarios = usuarios.OrderByDescending(orderBy).Skip(start).Take(length).AsQueryable();

                }

                foreach (var usuario in usuarios.ToList())
                {
                    usuario.Roles = [.. (await _userManager.GetRolesAsync(_userManager.FindByNameAsync(usuario.Login).Result))];
                    resultado.Add(usuario);
                }

                return new
                {
                    data = resultado,
                    //draw = tablemodel.Draw,
                    recordsFiltered = totalrowsafterfiltering,
                    recordsTotal = totalrows
                };
            });

            return Json(new { response.data, draw = tablemodel.Draw, response.recordsFiltered, response.recordsTotal, userRoles = (await _userManager.GetRolesAsync(_userManager.GetUserAsync(User).Result)).ToList() });
        }

        [HttpGet]
        [Authorize(Roles = "administrador,gerente")]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var usuarioDB = await _userManager.FindByIdAsync(id);
                if (usuarioDB == null)
                {
                    this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                    return RedirectToAction("Index");
                }

                var usuarioVM = _mapper.Map<UsuarioAddOrEditViewModel>(usuarioDB);

                usuarioVM.EstadoId = (await _cidadeRepository.GetByIdAsync(usuarioVM.CidadeId)).EstadoId;

                usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                return View(usuarioVM);
            }

            return View(new UsuarioAddOrEditViewModel() { Estados = await _estadoRepository.GetOptionsAsync("Selecione...") });
        }

        [HttpPost]
        [Authorize(Roles = "administrador,gerente")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddOrEdit([FromForm] UsuarioAddOrEditViewModel usuarioVM)
        {

            //se for alteração, não tem senha e confirmação de senha
            if (!string.IsNullOrEmpty(usuarioVM.Id))
            {
                ModelState.Remove("Senha");
                ModelState.Remove("ConfSenha");
            }

            if (ModelState.IsValid)
            {
                if (EntidadeExiste(usuarioVM.Id))
                {
                    //Alteração
                    var usuarioDB = await _userManager.FindByIdAsync(usuarioVM.Id);

                    bool alterouEmail = false;
                    //if (((usuarioVM.CPF != usuarioDB.CPF) && (_userManager.Users.Any(u => u.CPF == usuarioVM.CPF))) ||
                    //    ((usuarioVM.Email != usuarioDB.Email)))
                    //{
                    if ((usuarioVM.CPF != usuarioDB.CPF) &&
                        (_userManager.Users.Any(u => u.CPF == usuarioVM.CPF)))
                    {
                        ModelState.AddModelError("CPF", "Já existe um usuário cadastrado com o CPF informado.");
                    }

                    if (usuarioVM.Email != usuarioDB.Email)
                    {
                        if (_userManager.Users.Any(u => u.NormalizedEmail == usuarioVM.Email.ToUpper().Trim()))
                        {
                            ModelState.AddModelError("Email", "Já existe um usuário cadastrado com o e-mail informado.");
                        }
                        else
                        {
                            usuarioDB.EmailConfirmed = false;
                            alterouEmail = true;
                        }
                    }

                    if (ModelState.ErrorCount > 0)
                    {
                        usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                        return View(usuarioVM);
                    }
                    //}

                    //MapearCadastrarUsuarioViewModel(usuarioVM, cidadeDB);
                    _mapper.Map(usuarioVM, usuarioDB);

                    var resultado = await _userManager.UpdateAsync(usuarioDB);
                    if (resultado.Succeeded)
                    {
                        if (alterouEmail)
                        {
                            await EnviarLinkConfirmacaoEmailAsync(usuarioDB);
                            this.MostrarMensagem($"Pronto, alterei o usuário <b>{usuarioDB.NomeCompleto}</b>. Uma mensagem com um link de confirmação de e-mail foi enviada para o novo endereço de e-mail.");
                        }
                        else
                        {
                            this.MostrarMensagem($"Pronto, alterei o usuário <b>{usuarioDB.NomeCompleto}</b>.");
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        this.MostrarMensagem($"Ops, não consegui alterar o usuário <b>{usuarioDB.NomeCompleto}</b>.", true);
                        foreach (var error in resultado.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                        return View(usuarioVM);
                    }
                }
                else
                {
                    //Inclusão
                    var usuarioDB = await _userManager.FindByEmailAsync(usuarioVM.Email);

                    if (usuarioDB != null || _userManager.Users.Any(u => u.CPF == usuarioVM.CPF))
                    {
                        if (usuarioDB != null)
                        {
                            ModelState.AddModelError("Email", "Já existe um usuário cadastrado com o e-mail informado.");
                        }
                        if (_userManager.Users.Any(u => u.CPF == usuarioVM.CPF))
                        {
                            ModelState.AddModelError("CPF", "Já existe um usuário cadastrado com o CPF informado.");
                        }
                        usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                        return View(usuarioVM);
                    }

                    usuarioDB = _mapper.Map<UsuarioModel>(usuarioVM);
                    var resultado = await _userManager.CreateAsync(usuarioDB, usuarioVM.Senha);
                    if (resultado.Succeeded)
                    {
                        await EnviarLinkConfirmacaoEmailAsync(usuarioDB);
                        this.MostrarMensagem($"Pronto, salvei o usuário <b>{usuarioDB.NomeCompleto}</b>. Uma mensagem com um link de confirmação de e-mail foi enviada para o endereço de e-mail informado.");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        this.MostrarMensagem($"Ops, não consegui salvar o usuário {usuarioDB.NomeCompleto}.", true);
                        foreach (var error in resultado.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                        return View(usuarioVM);
                    }
                }
            }
            else
            {
                usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                return View(usuarioVM);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            return View(new UsuarioAddOrEditViewModel() { Estados = await _estadoRepository.GetOptionsAsync("Selecione...") });
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Cadastrar([FromForm] UsuarioAddOrEditViewModel usuarioVM)
        {
            if (ModelState.IsValid)
            {
                if (!EntidadeExiste(usuarioVM.Id))
                {
                    //Inclusão
                    var usuarioDB = await _userManager.FindByEmailAsync(usuarioVM.Email);

                    if (usuarioDB != null || _userManager.Users.Any(u => u.CPF == usuarioVM.CPF))
                    {
                        if (usuarioDB != null)
                        {
                            ModelState.AddModelError("Email", "Já existe um usuário cadastrado com esse e-mail.");
                        }
                        if (_userManager.Users.Any(u => u.CPF == usuarioVM.CPF))
                        {
                            ModelState.AddModelError("CPF", "Já existe um usuário cadastrado com esse CPF.");
                        }
                        usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                        return View(usuarioVM);
                    }

                    usuarioDB = _mapper.Map<UsuarioModel>(usuarioVM);
                    usuarioDB.UsuarioIdInclusao = usuarioDB.Id;
                    var resultado = await _userManager.CreateAsync(usuarioDB, usuarioVM.Senha);
                    if (resultado.Succeeded)
                    {
                        await EnviarLinkConfirmacaoEmailAsync(usuarioDB);
                        this.MostrarMensagem($"Pronto, salvei o usuário <b>{usuarioDB.NomeCompleto}</b>. Clique no link de confirmação de e-mail que eu te enviei, para finalizar o cadastro.");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        this.MostrarMensagem($"Ops, não consegui salvar o usuário {usuarioDB.NomeCompleto}.", true);
                        foreach (var error in resultado.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                        return View(usuarioVM);
                    }
                }
                else
                {
                    this.MostrarMensagem($"Ops! Tem certeza que não tem cadastro?", true);
                }
            }
            usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

            return View(usuarioVM);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AlterarCadastro()
        {
            var usuarioDB = await _userManager.GetUserAsync(User);
            if (usuarioDB == null || string.IsNullOrEmpty(usuarioDB.CPF))
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index), "Home");
            }
            var usuarioVM = _mapper.Map<UsuarioAddOrEditViewModel>(usuarioDB);

            usuarioVM.EstadoId = (await _cidadeRepository.GetByIdAsync(usuarioVM.CidadeId))?.EstadoId;

            usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

            return View(usuarioVM);
        }

        [HttpPost]
        [Authorize]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AlterarCadastro([FromForm] UsuarioAddOrEditViewModel usuarioVM)
        {

            ModelState.Remove("Senha");
            ModelState.Remove("ConfSenha");

            if (ModelState.IsValid)
            {
                if (EntidadeExiste(usuarioVM.Id))
                {
                    //Alteração
                    var usuarioDB = await _userManager.FindByIdAsync(usuarioVM.Id);

                    bool alterouEmail = false;
                    //if (((usuarioVM.CPF != usuarioDB.CPF) && (_userManager.Users.Any(u => u.CPF == usuarioVM.CPF))) ||
                    //    ((usuarioVM.Email != usuarioDB.Email)))
                    //{
                    if ((usuarioVM.CPF != usuarioDB.CPF) &&
                        (_userManager.Users.Any(u => u.CPF == usuarioVM.CPF)))
                    {
                        ModelState.AddModelError("CPF", "Já existe um usuário cadastrado com o CPF informado.");
                    }

                    if (usuarioVM.Email != usuarioDB.Email)
                    {
                        if (_userManager.Users.Any(u => u.NormalizedEmail == usuarioVM.Email.ToUpper().Trim()))
                        {
                            ModelState.AddModelError("Email", "Já existe um usuário cadastrado com o e-mail informado.");
                        }
                        else
                        {
                            usuarioDB.EmailConfirmed = false;
                            alterouEmail = true;
                        }
                    }

                    if (ModelState.ErrorCount > 0)
                    {
                        usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                        return View(usuarioVM);
                    }
                    //}

                    //MapearCadastrarUsuarioViewModel(usuarioVM, cidadeDB);
                    _mapper.Map(usuarioVM, usuarioDB);

                    var resultado = await _userManager.UpdateAsync(usuarioDB);
                    if (resultado.Succeeded)
                    {
                        if (alterouEmail)
                        {
                            await EnviarLinkConfirmacaoEmailAsync(usuarioDB);
                            this.MostrarMensagem($"Pronto, alterei o usuário <b>{usuarioDB.NomeCompleto}</b>. Uma mensagem com um link de confirmação de e-mail foi enviada para o novo endereço de e-mail informado. Clique no link para finalizar o cadastro.");
                        }
                        else
                        {
                            this.MostrarMensagem($"Pronto, salvei sua alteração de cadastro.");
                        }
                        return RedirectToAction(nameof(Index), "Home");
                    }
                    else
                    {
                        this.MostrarMensagem($"Ops, não consegui alterar o usuário <b>{usuarioDB.NomeCompleto}</b>.", true);
                        foreach (var error in resultado.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                        return View(usuarioVM);
                    }
                }
            }
            usuarioVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

            return View(usuarioVM);
        }


        [HttpGet]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                this.MostrarMensagem("Nenhum usuário foi informado.", true);
                return RedirectToAction(nameof(Index));
            }

            if (!EntidadeExiste(id))
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userManager.FindByIdAsync(id);

            return View(usuario);
        }

        [HttpPost]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var usuarioDB = await _userManager.FindByIdAsync(id);

            if (usuarioDB != null)
            {
                var resultado = await _userManager.DeleteAsync(usuarioDB);
                if (resultado.Succeeded)
                {
                    this.MostrarMensagem($"Pronto, excluí o usuário {usuarioDB.NomeCompleto}.");
                }
                else
                {
                    this.MostrarMensagem($"Ops, não consegui excluir o usuário {usuarioDB.NomeCompleto}.", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByNameAsync(loginVM.Usuario);


                if (usuario != null && !_userManager.IsEmailConfirmedAsync(usuario).Result)
                {
                    this.MostrarMensagem("Ops! Não é possível entrar com e-mail não confirmado.", true);
                    return View(loginVM);
                }

                var resultado = await _signInManager.PasswordSignInAsync(loginVM.Usuario, loginVM.Senha, loginVM.Lembrar, false);
                if (resultado.Succeeded)
                {
                    //var cartcookiename = "shopping_cart";
                    //if (HttpContext.Request.Cookies.ContainsKey(cartcookiename))
                    //{
                    //    var cartId = HttpContext.Request.Cookies[cartcookiename];
                    //    await _shoppingCartRepository.UpdateUser(cartId, usuario.Id);
                    //}

                    loginVM.ReturnUrl ??= "~/";
                    return LocalRedirect(loginVM.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário/Senha inválida.");
                    return View(loginVM);
                }
            }
            else
            {
                return View(loginVM);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AjaxLogin() => View();

        [HttpPost]
        [AllowAnonymous]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AjaxLogin([FromForm] LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(loginVM.Usuario, loginVM.Senha, loginVM.Lembrar, false);
                if (resultado.Succeeded)
                {
                    return Json(new { isValid = true });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário/Senha inválida.");
                }
            }
            return Json(new { isValid = false, html = await RenderRazorViewToStringAsync(this, "AjaxLogin", loginVM) });
        }

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult AcessoRestrito([FromQuery] string returnUrl)
        {
            string nomedapagina;
            var clipindex = returnUrl.IndexOf('/', 1);

            nomedapagina = clipindex > -1 ? returnUrl[1..clipindex] : returnUrl[1..];

            return View(model: nomedapagina); // Para que nomedapagina não seja entendido como nome da view, deve-se passar "model:".
        }

        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> AddAdministrador(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario != null)
            {
                var resultado = await _userManager.AddToRoleAsync(usuario, "administrador");
                if (resultado.Succeeded)
                {
                    this.MostrarMensagem(
                        $"Pronto, atribuí o perfil de Administrador ao usuário <b>{usuario.NomeCompleto}</b>.");
                }
                else
                {
                    this.MostrarMensagem(
                        $"Ops, não consegui atribuir o perfil de Administrador ao usuário <b>{usuario.NomeCompleto}</b>!", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> RemAdministrador(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario != null)
            {
                var resultado = await _userManager.RemoveFromRoleAsync(usuario, "administrador");
                if (resultado.Succeeded)
                {
                    if (usuario.Id == _userManager.FindByNameAsync(User.Identity.Name).Result.Id)
                    {
                        await _signInManager.SignOutAsync();
                        this.MostrarMensagem($"Você foi deslogado, devido mudança de permissões.");
                    }
                    else
                    {
                        this.MostrarMensagem($"Pronto, removi o perfil de Administrador do usuário <b>{usuario.NomeCompleto}</b>.");
                    }
                }
                else
                {
                    this.MostrarMensagem(
                        $"Ops, não consegui remover perfil de Administrador do usuário <b>{usuario.NomeCompleto}</b>.", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "administrador,gerente")]
        public async Task<IActionResult> AddGerente(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario != null)
            {
                var resultado = await _userManager.AddToRoleAsync(usuario, "gerente");
                if (resultado.Succeeded)
                {
                    this.MostrarMensagem(
                        $"Pronto, atribuí o perfil de Gerente ao usuário <b>{usuario.NomeCompleto}</b>.");
                }
                else
                {
                    this.MostrarMensagem(
                        $"Ops, não consegui atribuir o perfil de Gerente ao usuário <b>{usuario.NomeCompleto}</b>!", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "administrador, gerente")]
        public async Task<IActionResult> RemGerente(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario != null)
            {
                var resultado = await _userManager.RemoveFromRoleAsync(usuario, "gerente");
                if (resultado.Succeeded)
                {
                    if (usuario.Id == _userManager.FindByNameAsync(User.Identity.Name).Result.Id)
                    {
                        await _signInManager.SignOutAsync();
                        this.MostrarMensagem($"Você foi deslogado, devido mudança de permissões.");
                    }
                    else
                    {
                        this.MostrarMensagem(
                        $"Pronto, removi o perfil de Gerente do usuário <b>{usuario.NomeCompleto}</b>.");
                    }
                }
                else
                {
                    this.MostrarMensagem(
                        $"Ops, não consegui remover perfil de Gerente do usuário <b>{usuario.NomeCompleto}</b>.", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "administrador,gerente")]
        public async Task<IActionResult> AddPizzaiolo(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario != null)
            {
                var resultado = await _userManager.AddToRoleAsync(usuario, "pizzaiolo");
                if (resultado.Succeeded)
                {
                    this.MostrarMensagem(
                        $"Pronto, atribuí o perfil de Pizzaiolo ao usuário <b>{usuario.NomeCompleto}</b>.");
                }
                else
                {
                    this.MostrarMensagem(
                        $"Ops, não consegui atribuir o perfil de Pizzaiolo ao usuário <b>{usuario.NomeCompleto}</b>!", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "administrador, gerente")]
        public async Task<IActionResult> RemPizzaiolo(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario != null)
            {
                var resultado = await _userManager.RemoveFromRoleAsync(usuario, "pizzaiolo");
                if (resultado.Succeeded)
                {
                    if (usuario.Id == _userManager.FindByNameAsync(User.Identity.Name).Result.Id)
                    {
                        await _signInManager.SignOutAsync();
                        this.MostrarMensagem($"Você foi deslogado, devido mudança de permissões.");
                    }
                    else
                    {
                        this.MostrarMensagem(
                        $"Pronto, removi o perfil de Pizzaiolo do usuário <b>{usuario.NomeCompleto}</b>.");
                    }
                }
                else
                {
                    this.MostrarMensagem(
                        $"Ops, não consegui remover perfil de Pizzaiolo do usuário <b>{usuario.NomeCompleto}</b>.", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index));
            }
        }


        [Authorize(Roles = "administrador,gerente")]
        public async Task<IActionResult> AddEntregador(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario != null)
            {
                var resultado = await _userManager.AddToRoleAsync(usuario, "entregador");
                if (resultado.Succeeded)
                {
                    this.MostrarMensagem(
                        $"Pronto, atribuí o perfil de Entregador ao usuário <b>{usuario.NomeCompleto}</b>.");
                }
                else
                {
                    this.MostrarMensagem(
                        $"Ops, não consegui atribuir o perfil de Entregador ao usuário <b>{usuario.NomeCompleto}</b>!", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "administrador, gerente")]
        public async Task<IActionResult> RemEntregador(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario != null)
            {
                var resultado = await _userManager.RemoveFromRoleAsync(usuario, "entregador");
                if (resultado.Succeeded)
                {
                    if (usuario.Id == _userManager.FindByNameAsync(User.Identity.Name).Result.Id)
                    {
                        await _signInManager.SignOutAsync();
                        this.MostrarMensagem($"Você foi deslogado, devido mudança de permissões.");
                    }
                    else
                    {
                        this.MostrarMensagem(
                        $"Pronto, removi o perfil de Entregador do usuário <b>{usuario.NomeCompleto}</b>.");
                    }
                }
                else
                {
                    this.MostrarMensagem(
                        $"Ops, não consegui remover perfil de Entregador do usuário <b>{usuario.NomeCompleto}</b>.", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Ops, não encontrei o usuário.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        private bool EntidadeExiste(string id) => _userManager.Users.AsNoTracking().Any(u => u.Id == id);

        //private static void MapearCadastrarUsuarioViewModel(CadastrarUsuarioViewModel entidadeOrigem, UsuarioModel entidadeDestino)
        //{
        //    entidadeDestino.NomeCompleto = entidadeOrigem.NomeCompleto;
        //    entidadeDestino.DataNascimento = entidadeOrigem.DataNascimento;
        //    entidadeDestino.CPF = entidadeOrigem.CPF;
        //    entidadeDestino.UserName = entidadeOrigem.CPF;
        //    entidadeDestino.NormalizedUserName = entidadeOrigem.CPF;
        //    entidadeDestino.Email = entidadeOrigem.Email;
        //    entidadeDestino.NormalizedEmail = entidadeOrigem.Email.ToUpper().Trim();
        //    entidadeDestino.PhoneNumber = entidadeOrigem.Telefone;
        //}

        [HttpGet]
        public IActionResult EsqueciSenha()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EsqueciSenha([FromForm] EsqueciSenhaViewModel dados)
        {
            if (ModelState.IsValid)
            {
                if (_userManager.Users.AsNoTracking().Any(u => u.NormalizedEmail == dados.Email.ToUpper().Trim()))
                {
                    var usuario = await _userManager.FindByEmailAsync(dados.Email);
                    var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                    var urlConfirmacao = Url.Action(nameof(RedefinirSenha), "Usuario", new { token }, Request.Scheme);
                    var mensagem = new StringBuilder();
                    mensagem.Append($"<p>Olá, {usuario.NomeCompleto.TplName().FirstName}.</p>");
                    mensagem.Append("<p>Houve uma solicitação de redefinição de senha para seu usuário em nosso site. Clique no link abaixo para criar sua nova senha:</p>");
                    mensagem.Append($"<p><a href='{urlConfirmacao}'>Redefinir Senha</a></p>");
                    mensagem.Append("<p><i>Caso não tenha sido você, ignore essa mensagem.</i></p>");
                    mensagem.Append($"<p>Atenciosamente,<br>Equipe de Suporte<br>{configuration.GetValue<string>("AppSettings:CompanyName")}</p>");
                    await _emailService.SendEmailAsync(usuario.Email,
                        "Redefinição de Senha", "", mensagem.ToString());
                    return View(nameof(EmailRedefinicaoEnviado));
                }
                else
                {
                    this.MostrarMensagem(
                            $"Ops! Não encontrei nenhum usuário com o e-mail <b>{dados.Email}</b> em minha base de dados.", true);
                    return View();
                }
            }
            else
            {
                return View(dados);
            }
        }

        public IActionResult EmailRedefinicaoEnviado() => View();

        public IActionResult RedefinirSenha(string token)
        {
            var modelo = new RedefinirSenhaViewModel
            {
                Token = token
            };
            return View(modelo);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RedefinirSenha([FromForm] RedefinirSenhaViewModel dados)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(dados.Email);
                var resultado = await _userManager.ResetPasswordAsync(
                    usuario, dados.Token, dados.NovaSenha);
                if (resultado.Succeeded)
                {
                    this.MostrarMensagem(
                       $"Pronto! Redefini a senha. Agora você já pode fazer login com a nova senha.");
                    return View(nameof(Login));
                }
                else
                {
                    this.MostrarMensagem(
                        $"Ops! Não consegui redefinir sua senha. Verifique se preencheu a senha corretamente e, se o problema persistir, entre em contato com o suporte {configuration.GetValue<string>("AppSettings:CompanyName")}.", true);
                    return View(dados);
                }
            }
            else
            {
                return View(dados);
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult AlterarSenha() => View();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AlterarSenha([FromForm] AlterarSenhaViewModel dados)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                var resultado = await _userManager.ChangePasswordAsync(usuario, dados.SenhaAtual, dados.NovaSenha);
                if (resultado.Succeeded)
                {
                    this.MostrarMensagem(
                        $"Pronto! Identifique-se usando a nova senha.");
                    await _signInManager.SignOutAsync();
                    return RedirectToAction(nameof(Login), "Usuario");
                }
                else
                {
                    this.MostrarMensagem(
                        $"Ops! Não consegui alterar sua senha. Confira os dados informados e tente novamente.", true);
                    return View(dados);
                }
            }
            else
            {
                return View(dados);
            }
        }

        private async Task EnviarLinkConfirmacaoEmailAsync(UsuarioModel usuario)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
            var urlConfirmacao = Url.Action("ConfirmarEmail",
                "Usuario", new { email = usuario.Email, token }, Request.Scheme);
            var mensagem = new StringBuilder();
            mensagem.Append($"<p>Olá, {usuario.NomeCompleto.TplName().FirstName}.</p>");
            mensagem.Append("<p>Recebemos seu cadastro em nosso sistema. Para concluir o processo de cadastro, clique no link a seguir:</p>");
            mensagem.Append($"<p><a href='{urlConfirmacao}'>Confirmar Cadastro</a></p>");
            mensagem.Append($"<p>Atenciosamente,<br>Equipe de Suporte {configuration.GetValue<string>("AppSettings:CompanyName")}</p>");
            await _emailService.SendEmailAsync(usuario.Email,
                "Confirmação de Cadastro", "", mensagem.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarEmail(string email, string token)
        {
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
            {
                this.MostrarMensagem("Não foi possível confirmar o e-mail. Usuário não encontrado", true);
            }
            var resultado = await _userManager.ConfirmEmailAsync(usuario, token);
            if (resultado.Succeeded)
            {
                this.MostrarMensagem("E-mail confirmado com sucesso! Agora você já está liberado para fazer o login.");
            }
            else
            {
                this.MostrarMensagem("Não foi possível validar seu e-mail. Tente novamente em alguns minutos. Se o problema persistir, entre em contato com o suporte.", true);
            }
            return View(nameof(Login));
        }

    }
}