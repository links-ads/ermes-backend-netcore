using FusionAuthNetCore.Dto;

namespace Ermes.Auth.Dto
{
    public class LoginOutput
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
