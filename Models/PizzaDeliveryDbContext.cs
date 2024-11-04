using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PizzaDelivery.Extensions;
using PizzaDelivery.Interfaces;
using System.Linq.Expressions;
using System.Xml;

namespace PizzaDelivery.Models
{
    public class PizzaDeliveryDbContext : IdentityDbContext<UsuarioModel>
    {
        private readonly IUsuarioService _usuarioService;

        public PizzaDeliveryDbContext(DbContextOptions<PizzaDeliveryDbContext> options, IUsuarioService usuarioService) : base(options)
        {
            _usuarioService = usuarioService;
        }

        public DbSet<ProdutoModel> Produtos { get; set; }
        public DbSet<CategoriaModel> Categorias { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }
        public DbSet<PedidoItemModel> PedidoItens { get; set; }

        //public DbSet<EnderecoModel> Enderecos { get; set; }
        public DbSet<CidadeModel> Cidades { get; set; }
        public DbSet<EstadoModel> Estados { get; set; }
        public DbSet<ShoppingCartModel> ShoppingCart { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(UsuarioModel).IsAssignableFrom(entityType.ClrType))
                {
                    if (typeof(IAuditable).IsAssignableFrom(entityType.ClrType))
                    {
                        modelBuilder.Entity(entityType.ClrType).Property("DataInclusao")
                            //.HasDefaultValueSql("SysDateTime()")  // SQLServer
                            //.HasDefaultValueSql("datetime('now', 'localtime', 'start of day')")  // Sqlite
                            //.HasDefaultValueSql("datetime('now', 'localtime')")  // Sqlite
                            .HasDefaultValueSql("current_timestamp()")  // MySql
                            .HasColumnType("datetime")        // MySql
                            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                    }

                    if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                    {
                        var parameter = Expression.Parameter(entityType.ClrType, entityType.ClrType.Name.First().ToString().ToLower());
                        var queryFilterLambda = Expression.Lambda(
                            Expression.And(
                            Expression.Equal(Expression.Property(parameter, "IsAtivo"), Expression.Constant(true)),
                            Expression.Equal(Expression.Property(parameter, "IsExcluido"), Expression.Constant(false))
                            )
                            , parameter);

                        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(queryFilterLambda);
                    }
                }
            }

            modelBuilder.Entity<UsuarioModel>().Property(u => u.DataInclusao)
                //.HasDefaultValueSql("SysDateTime()")  // SQLServer
                //.HasDefaultValueSql("datetime('now', 'localtime', 'start of day')")  // Sqlite
                //.HasDefaultValueSql("datetime('now', 'localtime')")  // Sqlite
                .HasDefaultValueSql("current_timestamp()")  // MySql
                .HasColumnType("datetime")        // MySql
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            modelBuilder.Entity<UsuarioModel>().HasQueryFilter(p => p.IsAtivo == true && p.IsExcluido == false);

            //===================Sqlite=======================
            //modelBuilder.Entity<ProdutoModel>()
            //    .Property(e => e.Preco)
            //    .HasConversion<double>();

            //modelBuilder.Entity<PedidoItemModel>()
            //    .Property(e => e.Preco)
            //    .HasConversion<double>();
            //================================================
            base.OnModelCreating(modelBuilder);
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.SetAuditProperties(_usuarioService);
            return await base.SaveChangesAsync(cancellationToken);
        }
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ChangeTracker.SetAuditProperties(_usuarioService);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override int SaveChanges()
        {
            ChangeTracker.SetAuditProperties(_usuarioService);
            return base.SaveChanges();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTracker.SetAuditProperties(_usuarioService);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

    }
}
