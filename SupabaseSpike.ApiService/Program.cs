

using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddNpgsqlDataSource("Supabase");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();



app.MapGet("/", async ([FromServices] NpgsqlConnection datasource, ILogger<Program> logger) =>
{

    logger.LogInformation("Querying Supabase...");

    var sqlQuery = """
    SELECT graphql.resolve($$
        {
        accountCollection {
            edges {
            node {
                id,
                email,
                created_at,
                updated_at,
                blogCollection {
                edges {
                    node {
                    id,
                    name,
                    description,
                    created_at,
                    updated_at,
                    blog_postCollection {
                        edges {
                        node {
                            id,
                            title,
                            body,
                            status,
                            created_at,
                            updated_at
                        }
                        }
                    }
                    }
                }
                }
            }
            }
        }
        }
        $$);
    """;

    var result = await datasource.QueryFirstOrDefaultAsync<string>(sqlQuery);

    logger.LogInformation("Query result: {result}", result);

    if (result != null)
    {
        var jsonDocument = JsonDocument.Parse(result);
        return Results.Ok(jsonDocument.RootElement);
    }

    return Results.Ok(result);
});

app.MapDefaultEndpoints();

app.Run();