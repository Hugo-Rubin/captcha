
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/30/2014 17:48:33
-- Generated from EDMX file: C:\SourceCode\Captchas\Core\Data\OcrDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [OcrDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Requisicoes_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Requisicoes] DROP CONSTRAINT [FK_Requisicoes_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_Requisicoes_TipoOCR]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Requisicoes] DROP CONSTRAINT [FK_Requisicoes_TipoOCR];
GO
IF OBJECT_ID(N'[dbo].[FK_ServicosCliente_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServicosCliente] DROP CONSTRAINT [FK_ServicosCliente_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_ServicosCliente_Servicos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServicosCliente] DROP CONSTRAINT [FK_ServicosCliente_Servicos];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Clientes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clientes];
GO
IF OBJECT_ID(N'[dbo].[Requisicoes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Requisicoes];
GO
IF OBJECT_ID(N'[dbo].[Servicos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Servicos];
GO
IF OBJECT_ID(N'[dbo].[ServicosCliente]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServicosCliente];
GO
IF OBJECT_ID(N'[dbo].[TipoOCR]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TipoOCR];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Clientes'
CREATE TABLE [dbo].[Clientes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [Nome] nchar(200)  NOT NULL,
    [Telefone] nchar(10)  NULL,
    [Observacoes] nchar(500)  NULL,
    [Token] nchar(20)  NOT NULL
);
GO

-- Creating table 'Requisicoes'
CREATE TABLE [dbo].[Requisicoes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [idCliente] int  NOT NULL,
    [Data] datetime  NOT NULL,
    [idOCR] int  NOT NULL,
    [Captcha] nchar(200)  NOT NULL,
    [Resposta] nchar(10)  NOT NULL,
    [Mensagem] nchar(500)  NULL,
    [IP] varchar(15)  NULL
);
GO

-- Creating table 'Servicos'
CREATE TABLE [dbo].[Servicos] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Nome] nvarchar(20)  NOT NULL,
    [Sigla] nvarchar(3)  NULL
);
GO

-- Creating table 'ServicosCliente'
CREATE TABLE [dbo].[ServicosCliente] (
    [IdCliente] int  NOT NULL,
    [IdServico] int  NOT NULL,
    [LimiteConsultasDiarias] int  NOT NULL
);
GO

-- Creating table 'TipoOCR'
CREATE TABLE [dbo].[TipoOCR] (
    [id] int IDENTITY(1,1) NOT NULL,
    [Nome] nchar(100)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'Clientes'
ALTER TABLE [dbo].[Clientes]
ADD CONSTRAINT [PK_Clientes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Requisicoes'
ALTER TABLE [dbo].[Requisicoes]
ADD CONSTRAINT [PK_Requisicoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'Servicos'
ALTER TABLE [dbo].[Servicos]
ADD CONSTRAINT [PK_Servicos]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [IdCliente], [IdServico] in table 'ServicosCliente'
ALTER TABLE [dbo].[ServicosCliente]
ADD CONSTRAINT [PK_ServicosCliente]
    PRIMARY KEY CLUSTERED ([IdCliente], [IdServico] ASC);
GO

-- Creating primary key on [id] in table 'TipoOCR'
ALTER TABLE [dbo].[TipoOCR]
ADD CONSTRAINT [PK_TipoOCR]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [idCliente] in table 'Requisicoes'
ALTER TABLE [dbo].[Requisicoes]
ADD CONSTRAINT [FK_Requisicoes_Clientes]
    FOREIGN KEY ([idCliente])
    REFERENCES [dbo].[Clientes]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Requisicoes_Clientes'
CREATE INDEX [IX_FK_Requisicoes_Clientes]
ON [dbo].[Requisicoes]
    ([idCliente]);
GO

-- Creating foreign key on [IdCliente] in table 'ServicosCliente'
ALTER TABLE [dbo].[ServicosCliente]
ADD CONSTRAINT [FK_ServicosCliente_Clientes]
    FOREIGN KEY ([IdCliente])
    REFERENCES [dbo].[Clientes]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [idOCR] in table 'Requisicoes'
ALTER TABLE [dbo].[Requisicoes]
ADD CONSTRAINT [FK_Requisicoes_TipoOCR]
    FOREIGN KEY ([idOCR])
    REFERENCES [dbo].[TipoOCR]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Requisicoes_TipoOCR'
CREATE INDEX [IX_FK_Requisicoes_TipoOCR]
ON [dbo].[Requisicoes]
    ([idOCR]);
GO

-- Creating foreign key on [IdServico] in table 'ServicosCliente'
ALTER TABLE [dbo].[ServicosCliente]
ADD CONSTRAINT [FK_ServicosCliente_Servicos]
    FOREIGN KEY ([IdServico])
    REFERENCES [dbo].[Servicos]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ServicosCliente_Servicos'
CREATE INDEX [IX_FK_ServicosCliente_Servicos]
ON [dbo].[ServicosCliente]
    ([IdServico]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------