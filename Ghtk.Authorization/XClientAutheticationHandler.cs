using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Ghtk.Authorization
{//dùng lại lớp hệ thông trong aspire để làm lớp hệ thống cho authorization
    public class XClientAutheticationHandler : AuthenticationHandler<XClientAutheticationHandlerOptions>
    {//xclientauthoption là 1 Ioption xong Ioption này lại kế thừa từ AuthenticationSchemeOptions nên nó sẽ có các thuộc tính của AuthenticationSchemeOptions, và nó cũng có thêm thuộc tính ClientSourceValidator để xác thực giá trị của header X-Client-Source
        public XClientAutheticationHandler(IOptionsMonitor<XClientAutheticationHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }


        //public XClientAutheticationHandler(IOptionsMonitor<XClientAutheticationHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        //{
        //}

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
           
            //Options.ClientSourceValidator là một delegate được cấu hình để xác thực giá trị của header X-Client-Source. Nếu nó không được cấu hình, trả về lỗi xác thực.
            //Options này nó lấy ở đâu? Nó lấy từ lớp XClientAutheticationHandlerOptions, mà lớp này lại kế thừa từ AuthenticationSchemeOptions, nên nó sẽ được cấu hình trong phần cấu hình dịch vụ của ứng dụng, khi đăng ký scheme cho authentication. Cụ thể, khi bạn thêm authentication scheme này vào pipeline, bạn sẽ cung cấp một instance của XClientAutheticationHandlerOptions, trong đó bạn có thể thiết lập ClientSourceValidator để xác thực giá trị của header X-Client-Source.
            if (Options.ClientSourceValidator == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("ClientSourceValidator is not configured"));
            }
            var clientSource = Request.Headers["X-Client-Source"];
            if(clientSource.Count == 0)
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing X-Client-Source header"));
            }
            var  clientScourceValue = clientSource.First();
            if(string.IsNullOrEmpty(clientScourceValue))
            {
                return Task.FromResult(AuthenticateResult.Fail("Empty X-Client-Source header value"));
            }
            if (clientSource.Count > 1)
            {
                return Task.FromResult(AuthenticateResult.Fail("Multiple X-Client-Source headers"));
            }
            if(!Options.ClientSourceValidator(clientScourceValue))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid X-Client-Source value"));
            }//schema thuộc tính có sang của authentication handler, nó sẽ được sử dụng để tạo ra một ClaimsIdentity mới với tên của scheme này, và sau đó sẽ trả về một AuthenticateResult thành công với ClaimsPrincipal chứa identity này. Điều này có nghĩa là nếu header X-Client-Source được xác thực thành công, người dùng sẽ được coi là đã xác thực và có thể truy cập vào các tài nguyên được bảo vệ bởi authorization policies.
            var indentity = new ClaimsIdentity(Scheme.Name);
            indentity.AddClaim(new Claim("X-Client-Source", clientScourceValue));
            var principal = new ClaimsPrincipal(indentity);//dòng này là tạo ra một ClaimsPrincipal mới với identity chứa claim về X-Client-Source, sau đó tạo ra một AuthenticationTicket mới với principal và scheme name, và trả về một AuthenticateResult thành công với ticket này.
            var ticket = new AuthenticationTicket(principal, Scheme.Name);//dòng này là tạo ra một AuthenticationTicket mới với principal và scheme name, sau đó trả về một AuthenticateResult thành công với ticket này. AuthenticationTicket là một đối tượng chứa thông tin về người dùng đã xác thực, bao gồm ClaimsPrincipal và các thông tin khác như authentication properties.
            //tạo ra một AuthenticationTicket mới với principal và scheme name, sau đó trả về một AuthenticateResult thành công với ticket này. AuthenticationTicket là một đối tượng chứa thông tin về người dùng đã xác thực, bao gồm ClaimsPrincipal và các thông tin khác như authentication properties.
            return Task.FromResult(AuthenticateResult.Success(ticket));//nếu tất cả các bước xác thực đều thành công, trả về một AuthenticateResult thành công với ticket chứa thông tin về người dùng đã xác thực.
        }
    }
}


