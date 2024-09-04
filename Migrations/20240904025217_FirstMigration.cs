using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaDelivery.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CidadeId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    NomeCompleto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CPF = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    IsAlteraSenhaLogin = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsuarioIdInclusao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "datetime('now', 'localtime', 'start of day')"),
                    UsuarioIdAlteracao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsAtivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsExcluido = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsuarioIdExclusao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataExclusao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    OrderGroup = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cidade",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EstadoId = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    UsuarioIdInclusao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "datetime('now', 'localtime', 'start of day')"),
                    UsuarioIdAlteracao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsAtivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsExcluido = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsuarioIdExclusao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataExclusao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cidade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estado",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Capital = table.Column<string>(type: "TEXT", nullable: false),
                    UsuarioIdInclusao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "datetime('now', 'localtime', 'start of day')"),
                    UsuarioIdAlteracao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsAtivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsExcluido = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsuarioIdExclusao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataExclusao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Logradouro = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Complemento = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "char(2)", nullable: false),
                    CEP = table.Column<string>(type: "char(9)", nullable: false),
                    Referencia = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FormaPagamento = table.Column<int>(type: "INTEGER", maxLength: 10, nullable: false),
                    Troco = table.Column<double>(type: "REAL", nullable: true),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DataInicioPreparo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UsuarioIdInicioPreparo = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataFimPreparo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UsuarioIdFimPreparo = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataInicioEntrega = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UsuarioIdInicioEntrega = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataFimEntrega = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UsuarioIdFimEntrega = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    UsuarioIdInclusao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "datetime('now', 'localtime', 'start of day')"),
                    UsuarioIdAlteracao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsAtivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsExcluido = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsuarioIdExclusao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataExclusao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItem",
                columns: table => new
                {
                    PedidoId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    ProdutoId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Preco = table.Column<double>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItem", x => new { x.PedidoId, x.ProdutoId });
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Produto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Conteudo = table.Column<int>(type: "INTEGER", maxLength: 4, nullable: false),
                    Medida = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Ingredientes = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    CategoriaId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Preco = table.Column<double>(type: "decimal(5,2)", nullable: false),
                    NomeArquivoImagem = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    UsuarioIdInclusao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "datetime('now', 'localtime', 'start of day')"),
                    UsuarioIdAlteracao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsAtivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsExcluido = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsuarioIdExclusao = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    DataExclusao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    ProdutoId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    Quantidade = table.Column<int>(type: "INTEGER", maxLength: 3, nullable: false),
                    Expiracao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => new { x.Id, x.ProdutoId });
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Cidade");

            migrationBuilder.DropTable(
                name: "Estado");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "PedidoItem");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
