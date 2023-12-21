using System.Data;
using Dapper;
using Function.Constants;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Function.Utils;

public class DataContext
{
    private readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(Configuration.GetConnectionString("database"));
    }

    public void Init()
    {
        // create database tables if they don't exist
        using var connection = CreateConnection();

         const string sql = $"""
                CREATE TABLE IF NOT EXISTS
                {DatabaseConstants.BooksTable} (
                    Id TEXT NOT NULL PRIMARY KEY,
                    Title TEXT,
                    Edition TEXT,
                    Source TEXT
                );

                CREATE VIRTUAL TABLE IF NOT EXISTS
                {DatabaseConstants.BooksQueryTable} USING fts3(Title TEXT, Id TEXT);
        """;

        connection.Execute(sql);
    }
}
