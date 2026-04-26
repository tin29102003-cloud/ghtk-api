using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
            var clientSource = Request.Headers["X-Client-Source"];
            var token = Request.Headers["Token"];
            if (clientSource.Count == 0)
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing X-Client-Source header"));
            }
            if(token.Count == 0)
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Token header"));
            }

            

            var tokenValue = token.FirstOrDefault();    
            var  clientScourceValue = clientSource.FirstOrDefault();
            if (!string.IsNullOrEmpty(tokenValue) 
                && !string.IsNullOrEmpty(clientScourceValue) 
                &&   VerifyClient(clientScourceValue, tokenValue, out var principal))
            {
                //var indentity = new ClaimsIdentity(Scheme.Name);
                //indentity.AddClaim(new Claim("X-Client-Source", clientScourceValue));
                //var principal = new ClaimsPrincipal(indentity);//dòng này là tạo ra một ClaimsPrincipal mới với identity chứa claim về X-Client-Source, sau đó tạo ra một AuthenticationTicket mới với principal và scheme name, và trả về một AuthenticateResult thành công với ticket này.
                var ticket = new AuthenticationTicket(principal, Scheme.Name);//dòng này là tạo ra một AuthenticationTicket mới với principal và scheme name, sau đó trả về một AuthenticateResult thành công với ticket này. AuthenticationTicket là một đối tượng chứa thông tin về người dùng đã xác thực, bao gồm ClaimsPrincipal và các thông tin khác như authentication properties.
                                                                              //tạo ra một AuthenticationTicket mới với principal và scheme name, sau đó trả về một AuthenticateResult thành công với ticket này. AuthenticationTicket là một đối tượng chứa thông tin về người dùng đã xác thực, bao gồm ClaimsPrincipal và các thông tin khác như authentication properties.
                return Task.FromResult(AuthenticateResult.Success(ticket));//nếu tất cả các bước xác thực đều thành công, trả về một AuthenticateResult thành công với ticket chứa thông tin về người dùng đã xác thực.
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Token"));
            }
            
        }

        private bool VerifyClient(string clientScourceValue, string tokenValue, out ClaimsPrincipal? principal)
        {
            if (!ValidateAsync(tokenValue, out var token, out  principal))
            {
                return false;
            }
            var sub = (token as JwtSecurityToken)!.Subject;
            if(clientScourceValue != sub)//name ở đây là claim name của token, nếu token hợp lệ thì chúng ta sẽ lấy được claim name từ token và so sánh với giá trị của header X-Client-Source để xác thực xem client có phải là client đáng tin cậy hay không. Nếu giá trị của header X-Client-Source không khớp với claim name trong token, thì chúng ta sẽ trả về false, tức là xác thực thất bại.
            {
                return false;
            }
            //Options.ClientSourceValidator là một delegate được cấu hình để xác thực giá trị của header X-Client-Source. Nếu nó không được cấu hình, trả về lỗi xác thực.
            //Options này nó lấy ở đâu? Nó lấy từ lớp XClientAutheticationHandlerOptions, mà lớp này lại kế thừa từ AuthenticationSchemeOptions, nên nó sẽ được cấu hình trong phần cấu hình dịch vụ của ứng dụng, khi đăng ký scheme cho authentication. Cụ thể, khi bạn thêm authentication scheme này vào pipeline, bạn sẽ cung cấp một instance của XClientAutheticationHandlerOptions, trong đó bạn có thể thiết lập ClientSourceValidator để xác thực giá trị của header X-Client-Source.
            if (!Options.ClientValidator(clientScourceValue, token!, principal!))
            {
                return false;
            }//schema thuộc tính có sang của authentication handler, nó sẽ được sử dụng để tạo ra một ClaimsIdentity mới với tên của scheme này, và sau đó sẽ trả về một AuthenticateResult thành công với ClaimsPrincipal chứa identity này. Điều này có nghĩa là nếu header X-Client-Source được xác thực thành công, người dùng sẽ được coi là đã xác thực và có thể truy cập vào các tài nguyên được bảo vệ bởi authorization policies.
            return true;
        }
        //muc đich giải mã token và verify token, nếu token hợp lệ thì trả về true, ngược lại trả về false. Nếu token hợp lệ thì có thể lấy ra thông tin từ token và truyền vào ClientSourceValidator để xác thực giá trị của header X-Client-Source.
        private bool ValidateAsync(string tokenValue, out SecurityToken? token, out ClaimsPrincipal? claimsPrincipal)
        {            //khi hàm này xong thi ta lấy ra dc token và claimsPrincipal, nếu token hợp lệ thì claimsPrincipal sẽ chứa thông tin về người dùng đã xác thực, bao gồm các claim được mã hóa trong token. Nếu token không hợp lệ, hàm này sẽ ném ra một ngoại lệ, và chúng ta sẽ bắt ngoại lệ đó để trả về false.

            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParematers = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,//kiêm tra khóa ký của token, nếu token được ký bằng một khóa bí mật hợp lệ thì sẽ được coi là hợp lệ
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.IsseuerSigningKey)),//dăng ký option bên ngoài program
                ValidateIssuer = false,//kiểm tra 
                ValidateAudience = false,//kiểm tra audience của token, nếu token có audience hợp lệ thì sẽ được coi là hợp lệ
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true,
            };
            try
            {
                claimsPrincipal = handler.ValidateToken(tokenValue, tokenValidationParematers, out token);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error validating token");
                token = null;
                claimsPrincipal = null;
                return false;
            }
        }
    }
}


