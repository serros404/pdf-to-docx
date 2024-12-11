using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o serviço de controle de APIs
builder.Services.AddControllers();

// Configura o upload de arquivos grandes
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue; // Permite uploads de arquivos grandes
});

var app = builder.Build();

// Configura os middlewares
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Exibe páginas de erro detalhadas no ambiente de desenvolvimento
}

app.UseHttpsRedirection(); // Redireciona todas as requisições HTTP para HTTPS

app.MapControllers(); // Mapeia os controllers da API para os endpoints

app.Run();
