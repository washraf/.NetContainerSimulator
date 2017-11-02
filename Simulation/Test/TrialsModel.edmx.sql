
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/17/2016 14:59:10
-- Generated from EDMX file: D:\Simulations\Simulation\Test\TrialsModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Simulation];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Trials]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Trials];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Trials'
CREATE TABLE [dbo].[Trials] (
    [Size] int  NOT NULL,
    [StartUtil] nvarchar(50)  NOT NULL,
    [Change] nvarchar(50)  NOT NULL,
    [Algorithm] nvarchar(50)  NOT NULL,
    [Entropy] float  NOT NULL,
    [Power] float  NOT NULL,
    [StdDev] float  NOT NULL,
    [Hosts] float  NOT NULL,
    [Migrations] float  NOT NULL,
    [SlaViolations] float  NOT NULL,
    [TotalMessages] float  NOT NULL,
    [PredictionAlg] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Size], [StartUtil], [Change], [Algorithm] in table 'Trials'
ALTER TABLE [dbo].[Trials]
ADD CONSTRAINT [PK_Trials]
    PRIMARY KEY CLUSTERED ([Size], [StartUtil], [Change], [Algorithm] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------