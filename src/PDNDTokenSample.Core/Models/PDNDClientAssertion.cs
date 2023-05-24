namespace PDNDTokenSample.Core.Models
{
    public class PDNDClientAssertion
    {
        /// <summary>
        /// Gets or sets the public key ID (kid)
        /// </summary>
        public string KeyId { get; init; }

        /// <summary>
        /// Gets or sets the signing algorithm (alg)
        /// </summary>
        /// <example>"RS256"</example>
        public string Algorithm { get; init; }

        /// <summary>
        /// Gets or sets the type of object
        /// </summary>
        /// <example>"JWT"</example>
        public string Type { get; init; }

        /// <summary>
        /// Gets or sets the issuer (iss)
        /// </summary>
        public string Issuer { get; init; }

        /// <summary>
        /// Gets or sets the subject (sub)
        /// </summary>
        public string Subject { get; init; }

        /// <summary>
        /// Gets or sets the audience (aud)
        /// </summary>
        public string Audience { get; init; }

        /// <summary>
        /// Gets or sets the purpose for which access to resources will be requested (purposeId)
        /// </summary>
        public string PurposeId { get; init; }

        /// <summary>
        /// Gets or sets the JWT identifier
        /// </summary>
        public Guid TokenId { get; init; }
        
        /// <summary>
        /// Gets or sets the token creation date
        /// </summary>
        public DateTime IssuedAt { get; init; }

        /// <summary>
        /// Gets or sets the token expiration date
        /// </summary>
        public DateTime Expiration { get; init;}

        /// <summary>
        /// Gets or sets the client assertion
        /// </summary>
        public string ClientAssertion { get; init; }
    }
}
