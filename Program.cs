using SNGPC_B.Api.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

var farmaceutico = new Farmaceutico
{
    Nome = "Maria Santoro",
    CRF = "1234567890",
    CRFUF = "SP",
    CRFDataEmissao = new DateTime(2020, 1, 15),
    CPF = "12345678900"
};

Console.WriteLine($"Farmacêutico: {farmaceutico.Nome}");


app.MapControllers();

app.Run();