using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Ghtk.Authorization
{//class nayf sẽ chụi trach nhiệm với phương thức xác thực của xclient, có thể kế thừa từ lớp hệ thống trong aspire để tái sử dụng các phương thức xác thực đã có sẵn
    public class XClientAutheticationHandlerOptions:AuthenticationSchemeOptions
    {
        //để sử dung đc thì phải luôn override lại cái property này
        public Func<string, SecurityToken,ClaimsPrincipal,bool> ClientValidator { get; set; } = (clientSource,token, claimsPrincipal) => false; //dây là secure by  default, nếu không được cấu hình thì sẽ trả về false, tức là không có giá trị nào của header X-Client-Source được chấp nhận
        public string IsseuerSigningKey { get; set; } = string.Empty;
    }
}
