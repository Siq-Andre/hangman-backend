using HangmanBackend.Service;

var builder = WebApplication.CreateBuilder(args);

// Adicionar o serviço de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()  // Permite qualquer origem (atenção ao ambiente de produção)
               .AllowAnyMethod()  // Permite qualquer método HTTP (GET, POST, etc.)
               .AllowAnyHeader(); // Permite qualquer cabeçalho
    });
});

// Registrar o GameSessionService como um serviço Singleton ou Transient
builder.Services.AddSingleton<GameSessionService>(); // Singleton - uma única instância usada durante toda a aplicação
                                                     // ou, se preferir, usar:
                                                     // builder.Services.AddTransient<GameSessionService>(); // Transient - nova instância criada a cada solicitação

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