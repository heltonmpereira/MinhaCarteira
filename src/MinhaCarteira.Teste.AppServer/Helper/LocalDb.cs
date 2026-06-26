using System;
using System.IO;
using Npgsql;

namespace MinhaCarteira.Teste.AppServer.Helper;

public static class LocalDb
{
    public static string NomeArquivoDB => "MinhaCarteiraTesteDB";
    public static string StringConexao
    {
        get
        {
            var conString = $"Server=(localdb)\\MSSQLLocalDB;Database={NomeArquivoDB};Integrated Security=true;";

            return conString;
        }
    }
    private static string PathDb(string databaseName)
    {
        var outputFolder = Environment
            .GetFolderPath(Environment.SpecialFolder.UserProfile);
        string mdfFilename = databaseName + ".mdf";
        string databaseFileName = Path.Combine(outputFolder, mdfFilename);

        return databaseFileName;
    }

    public static void ApagarECriarLocalDb(string databaseName = null)
    {
        databaseName ??= NomeArquivoDB;
        var databaseFileName = PathDb(databaseName);

        if (CheckDatabaseExists(databaseName))
            DropDatabaseObjects(databaseName);

        CreateDatabase(databaseName, databaseFileName);
    }
    public static void ConectarBancoDadosExistente(string databaseName = null)
    {
        databaseName ??= NomeArquivoDB;
        var databaseFileName = PathDb(databaseName);

        // If the database does not already exist, create it.
        if (!CheckDatabaseExists(databaseName))
            CreateDatabase(databaseName, databaseFileName);
    }

    private static bool CheckDatabaseExists(string databaseName)
    {
        string connectionString = string.Format(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connection Timeout=300");
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        NpgsqlCommand cmd = connection.CreateCommand();

        cmd.CommandText = string.Format("SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = '{0}' OR name = '{1}')", databaseName, databaseName);
        object result = cmd.ExecuteScalar();

        if (result != null)
            return true;

        return false;
    }

    private static void DropDatabaseObjects(string databaseName)
    {
        string connectionString = string.Format(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True");
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        NpgsqlCommand cmd = connection.CreateCommand();

        // TryDetachDatabase(databaseName);
        cmd.CommandText = @$"/*Dropando o banco de dados*/
USE [master];

ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

DROP DATABASE [{databaseName}];
";
        cmd.ExecuteNonQuery();
    }

    private static void CreateDatabase(string databaseName, string databaseFileName)
    {
        string connectionString = string.Format(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True");
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        NpgsqlCommand cmd = connection.CreateCommand();

        // TryDetachDatabase(databaseName);
        cmd.CommandText = string.Format("CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", databaseName, databaseFileName);
        cmd.ExecuteNonQuery();
    }
}