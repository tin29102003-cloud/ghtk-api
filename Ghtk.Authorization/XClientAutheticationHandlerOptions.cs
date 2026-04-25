using Microsoft.AspNetCore.Authentication;

namespace Ghtk.Authorization
{//class nayf sẽ chụi trach nhiệm với phương thức xác thực của xclient, có thể kế thừa từ lớp hệ thống trong aspire để tái sử dụng các phương thức xác thực đã có sẵn
    public class XClientAutheticationHandlerOptions:AuthenticationSchemeOptions
    {
        //để sử dung đc thì phải luôn override lại cái property này
        public Func<string?, bool> ClientSourceValidator { get; set; } = (clientSource) => false; //dây là secure by  default, nếu không được cấu hình thì sẽ trả về false, tức là không có giá trị nào của header X-Client-Source được chấp nhận
    }
}
