using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ApiRestNet.Data;
using ApiRestNet.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();




app.MapGet("/Pruebas", async (DataContext context) =>
{
    try
    {
        var ultimaPartida = await context.Partidas.FirstOrDefaultAsync();  // Example: Retrieving the first partida, adjust as per your needs
        if (ultimaPartida != null)
        {
            return Results.Ok("Partida encontrada: " + ultimaPartida.Nombre);  // Example response
        }
        else
        {
            return Results.NotFound("No se encontraron partidas.");  // Example if no partida is found
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.ToString());  // Returning BadRequest with exception details
    }
}).WithName("GetPruebas")
.WithOpenApi();  // Adding OpenAPI documentation for the endpoint



async Task<List<Partida>> GetPartidas(DataContext context)
{
    var partidasQuery = await context.Partidas.ToListAsync();
    return partidasQuery;
}

app.MapGet("/Partidas", async (DataContext context) =>
{
    
        return await GetPartidas(context);
    
})
.WithName("GetPartidas")
.WithOpenApi();


app.MapPost("/add/jugador", async (DataContext context, JugadorDto jugadorDto) =>
{
    

   var ultimaPartida = context.Partidas
    .Where(p => p.Cedula == jugadorDto.Cedula) // Filtrar por la cédula del jugador
    .OrderByDescending(p => p.PartidaNum)  // Ordenar descendente por el número de partida
    .Select(p => p.PartidaNum)             // Seleccionar solo el número de partida
    .FirstOrDefault();
    
    
    var count = jugadorDto.Nombres.Count;
    for (int i = 0; i < count; i++)
    {
        var partida = new Partida
        {
            Nombre = jugadorDto.Nombres[i],
            Indice = i,
            PartidaNum = ultimaPartida,
            Cedula = jugadorDto.Cedula
        };

        context.Partidas.Add(partida);
    }

    await context.SaveChangesAsync();

    return Results.Ok("SE AGREGO LA PARTIDA");
})
.WithName("AddJugador")
.WithOpenApi();


app.Run();
