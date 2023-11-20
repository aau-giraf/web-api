namespace GirafRest.Services
{
    /// <summary>
    /// Instance of JwtConfig holding serverside configuration for building JWT Tokens
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// Key for serverside signature
        /// </summary>
        public string JwtKey { get; set; }

        /// <summary>
        /// JWT Issuer for tokens
        /// </summary>
        public string JwtIssuer { get; set; }

        /// <summary>
        /// Expiration time for new tokens
        /// </summary>
        public int JwtExpireDays { get; set; }
    }
}
