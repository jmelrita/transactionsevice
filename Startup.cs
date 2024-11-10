using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
    services.AddControllers();
}
