namespace CursoNetCore.Domain.Security
{
    public class TokenConfiguration
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int Hours { get; set; }
    }
}
