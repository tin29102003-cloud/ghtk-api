using Ghtk.Authorization;

namespace Ghtk.Api
{
    public static class XClientAutheticationHandlerExtention
    {
        private const string Schema = "X-Client-Source";
        public static void  AddXClientAuthentication(this IServiceCollection service,Action<XClientAutheticationHandlerOptions> configurationOptions )
        {
              service.AddAuthentication(Schema)
             .AddScheme<XClientAutheticationHandlerOptions, XClientAutheticationHandler>(Schema, configurationOptions);//làm này dễ custom lai
        }
    }
}
////cấu hình ClientSourceValidator để xác thực giá trị của header X-Client-Source, ví dụ ở đây chỉ chấp nhận giá trị "trusted-client"
