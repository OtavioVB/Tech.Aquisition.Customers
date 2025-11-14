
using Tech.Aquisitions.Customers.Application.UseCases;

namespace Tech.Aquisitions.Customers.WebApi;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddUseCases(builder.Configuration);

        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
