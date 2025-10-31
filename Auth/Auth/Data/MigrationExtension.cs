using Microsoft.EntityFrameworkCore;

namespace Auth.Data
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using DataContext context = scope.ServiceProvider.GetRequiredService<DataContext>();

            context.Database.Migrate();
        }
    }
}
