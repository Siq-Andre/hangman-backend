using HangmanBackend.Service;

var builder = WebApplication.CreateBuilder(args);

// Adicionar o servi�o de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()  // Permite qualquer origem (aten��o ao ambiente de produ��o)
               .AllowAnyMethod()  // Permite qualquer m�todo HTTP (GET, POST, etc.)
               .AllowAnyHeader(); // Permite qualquer cabe�alho
    });
});

// Registrar o GameSessionService como um servi�o Singleton ou Transient
builder.Services.AddSingleton<GameSessionService>(); // Singleton - uma �nica inst�ncia usada durante toda a aplica��o
                                                     // ou, se preferir, usar:
                                                     // builder.Services.AddTransient<GameSessionService>(); // Transient - nova inst�ncia criada a cada solicita��o

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ativar CORS
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();