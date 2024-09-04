CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [CidadeId] nvarchar(36) NOT NULL,
    [NomeCompleto] nvarchar(100) NOT NULL,
    [DataNascimento] datetime2 NOT NULL,
    [CPF] nvarchar(14) NOT NULL,
    [IsAlteraSenhaLogin] bit NOT NULL,
    [UsuarioIdInclusao] nvarchar(36) NOT NULL,
    [DataInclusao] datetime2 NULL DEFAULT (SysDateTime()),
    [UsuarioIdAlteracao] nvarchar(36) NULL,
    [DataAlteracao] datetime2 NULL,
    [IsAtivo] bit NOT NULL,
    [IsExcluido] bit NOT NULL,
    [UsuarioIdExclusao] nvarchar(36) NULL,
    [DataExclusao] datetime2 NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Categoria] (
    [Id] nvarchar(36) NOT NULL,
    [Nome] nvarchar(80) NULL,
    [OrderGroup] int NOT NULL,
    CONSTRAINT [PK_Categoria] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Cidade] (
    [Id] nvarchar(36) NOT NULL,
    [Cidade] nvarchar(100) NOT NULL,
    [EstadoId] char(2) NOT NULL,
    [UsuarioIdInclusao] nvarchar(36) NOT NULL,
    [DataInclusao] datetime2 NULL DEFAULT (SysDateTime()),
    [UsuarioIdAlteracao] nvarchar(36) NULL,
    [DataAlteracao] datetime2 NULL,
    [IsAtivo] bit NOT NULL,
    [IsExcluido] bit NOT NULL,
    [UsuarioIdExclusao] nvarchar(36) NULL,
    [DataExclusao] datetime2 NULL,
    CONSTRAINT [PK_Cidade] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Estado] (
    [Id] char(2) NOT NULL,
    [Estado] nvarchar(100) NOT NULL,
    [Capital] nvarchar(max) NOT NULL,
    [UsuarioIdInclusao] nvarchar(36) NOT NULL,
    [DataInclusao] datetime2 NULL DEFAULT (SysDateTime()),
    [UsuarioIdAlteracao] nvarchar(36) NULL,
    [DataAlteracao] datetime2 NULL,
    [IsAtivo] bit NOT NULL,
    [IsExcluido] bit NOT NULL,
    [UsuarioIdExclusao] nvarchar(36) NULL,
    [DataExclusao] datetime2 NULL,
    CONSTRAINT [PK_Estado] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Pedido] (
    [Id] nvarchar(36) NOT NULL,
    [Status] int NOT NULL,
    [Logradouro] nvarchar(200) NOT NULL,
    [Numero] nvarchar(10) NOT NULL,
    [Complemento] nvarchar(200) NULL,
    [Bairro] nvarchar(50) NOT NULL,
    [Cidade] nvarchar(50) NOT NULL,
    [Estado] char(2) NOT NULL,
    [CEP] char(9) NOT NULL,
    [Referencia] nvarchar(200) NULL,
    [FormaPagamento] int NOT NULL,
    [Troco] float NULL,
    [Observacao] nvarchar(200) NULL,
    [DataInicioPreparo] datetime2 NULL,
    [UsuarioIdInicioPreparo] nvarchar(36) NULL,
    [DataFimPreparo] datetime2 NULL,
    [UsuarioIdFimPreparo] nvarchar(36) NULL,
    [DataInicioEntrega] datetime2 NULL,
    [UsuarioIdInicioEntrega] nvarchar(36) NULL,
    [DataFimEntrega] datetime2 NULL,
    [UsuarioIdFimEntrega] nvarchar(36) NULL,
    [UsuarioIdInclusao] nvarchar(36) NOT NULL,
    [DataInclusao] datetime2 NULL DEFAULT (SysDateTime()),
    [UsuarioIdAlteracao] nvarchar(36) NULL,
    [DataAlteracao] datetime2 NULL,
    [IsAtivo] bit NOT NULL,
    [IsExcluido] bit NOT NULL,
    [UsuarioIdExclusao] nvarchar(36) NULL,
    [DataExclusao] datetime2 NULL,
    CONSTRAINT [PK_Pedido] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [PedidoItem] (
    [PedidoId] nvarchar(36) NOT NULL,
    [ProdutoId] nvarchar(36) NOT NULL,
    [Quantidade] int NOT NULL,
    [Preco] decimal(5,2) NOT NULL,
    CONSTRAINT [PK_PedidoItem] PRIMARY KEY ([PedidoId], [ProdutoId])
);
GO


CREATE TABLE [Produto] (
    [Id] nvarchar(36) NOT NULL,
    [Produto] nvarchar(100) NOT NULL,
    [Descricao] nvarchar(200) NOT NULL,
    [Conteudo] int NOT NULL,
    [Medida] nvarchar(8) NOT NULL,
    [Ingredientes] nvarchar(400) NOT NULL,
    [CategoriaId] nvarchar(36) NOT NULL,
    [Preco] decimal(5,2) NOT NULL,
    [NomeArquivoImagem] nvarchar(60) NULL,
    [UsuarioIdInclusao] nvarchar(36) NOT NULL,
    [DataInclusao] datetime2 NULL DEFAULT (SysDateTime()),
    [UsuarioIdAlteracao] nvarchar(36) NULL,
    [DataAlteracao] datetime2 NULL,
    [IsAtivo] bit NOT NULL,
    [IsExcluido] bit NOT NULL,
    [UsuarioIdExclusao] nvarchar(36) NULL,
    [DataExclusao] datetime2 NULL,
    CONSTRAINT [PK_Produto] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [ShoppingCart] (
    [Id] nvarchar(36) NOT NULL,
    [ProdutoId] nvarchar(36) NOT NULL,
    [UsuarioId] nvarchar(36) NULL,
    [Quantidade] int NOT NULL,
    [Expiracao] datetime2 NOT NULL,
    CONSTRAINT [PK_ShoppingCart] PRIMARY KEY ([Id], [ProdutoId])
);
GO


CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO


CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO


CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO


CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO


CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO