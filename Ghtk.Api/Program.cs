using ClientAuthertication;
using Ghtk.Api.AuthenticationHanler;


namespace Ghtk.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
           
            builder.Services.AddControllers();
            IClientSourceAuthenticationHandler clientSourceAuthenticationHandler  = new RemoteAuthenticationHanler(builder.Configuration["AuthenticationService"] ?? throw new Exception("ClientAuthentication database connection string not found"));
            //builder.Services.AddScoped<IClientSourceAuthenticationHandler>(service => new SqlServerClientSourceAuthenticationHandler(builder.Configuration.GetConnectionString("ClientAuthentication") ?? throw new Exception("ClientAuthentication database connection string not found")));
            //dăng ký authentication scheme cho xclient
            builder.Services.AddXClientAuthentication(options =>
            {
                options.ClientValidator = (clientSource, token, principle) =>  clientSourceAuthenticationHandler.Validate(clientSource); //cấu hình ClientSourceValidator để xác thực giá trị của header X-Client-Source, ví dụ ở đây chỉ chấp nhận giá trị "trusted-client"
                options.IsseuerSigningKey = builder.Configuration["IsseuerSigningKey"] ?? "";
            });
            var app = builder.Build();

            


            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
