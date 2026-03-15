    namespace Dima.Api.Common.Api;

public static class AppExtension
{
    public static void ConfigureDevEnvironment(this WebApplication app)
    {
        //Gera a tela do swagger
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapSwagger().RequireAuthorization();
    }

    public static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }

}
