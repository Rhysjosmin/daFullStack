using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=localhost;Database=maindb;Username=rodr;Password=theDBPass;Timeout=1000";

builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));

var app = builder.Build();



app.MapGet("/", async (IDbConnection dbConnection) =>
{
    try
    {
        dbConnection.Open(); // Npgsql uses synchronous Open method

        var blogs = await dbConnection.QueryAsync<Blog>("SELECT * FROM blogs;");

        return Results.Ok(blogs); // Return JSON serialized blogs
    }
    catch (Exception ex)
    {
        return Results.BadRequest(JsonSerializer.Serialize(new { error = ex.Message })); // Return JSON error message
    }
});


app.MapPost("/blogs", async (IDbConnection dbConnection, HttpContext context) =>
{
    try
    {
        dbConnection.Open();

        // Deserialize the incoming JSON payload into a Blog object
        var blog = await JsonSerializer.DeserializeAsync<Blog>(context.Request.Body);

        // Insert the new blog into the database
        var sql = "INSERT INTO blogs (Title, Url, Id) VALUES (@Title, @Url, @Id)";
        await dbConnection.ExecuteAsync(sql, new { Title = blog.Title, Url = blog.Url, Id = blog.Id });

        return Results.Created("/blogs", blog); // Return the created blog
    }
    catch (Exception ex)
    {
        return Results.BadRequest(JsonSerializer.Serialize(new { error = ex.Message })); // Return JSON error message
    }
});

app.Run();
