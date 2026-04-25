using Ghtk.Authorization;

namespace Ghtk.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            //dăng ký authentication scheme cho xclient
            builder.Services.AddXClientAuthentication(options =>
            {
                options.ClientSourceValidator = (clientSource) => clientSource == "ghtk"; //cấu hình ClientSourceValidator để xác thực giá trị của header X-Client-Source, ví dụ ở đây chỉ chấp nhận giá trị "trusted-client"
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
