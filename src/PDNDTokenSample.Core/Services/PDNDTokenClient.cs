namespace PDNDTokenSample.Core.Services
{
    using Jose;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.OpenSsl;
    using Org.BouncyCastle.Security;
    using PDNDTokenSample.Core.Abstractions;
    using PDNDTokenSample.Core.Extensions;
    using PDNDTokenSample.Core.Models;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text.Json;
    using System.Text.RegularExpressions;

    public class PDNDTokenClient : IPDNDTokenClient
    {
        private readonly PDNDTokenClientSettings _settings;

        /// <summary>
        /// Instantiates a new instance of <see cref="PDNDTokenClient"/> class
        /// </summary>
        /// <param name="settings">the <see cref="PDNDTokenClientSettings"/> configuration</param>
        public PDNDTokenClient(PDNDTokenClientSettings settings)
        {
            _settings = settings;
        }

        /// <inheritdoc />
        public PDNDClientAssertion GetClientAssertion()
        {
            string client_assertion = string.Empty;

            DateTime issued = DateTime.UtcNow;
            DateTime expire_in = issued + TimeSpan.FromMinutes(_settings.Duration);
            Guid jti = Guid.NewGuid();

            Dictionary<string, object> headers = new Dictionary<string, object>
            {
                { "kid", _settings.KeyId },
                { "alg", _settings.Algorithm },
                { "typ", _settings.Type }
            };

            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "iss", _settings.Issuer },
                { "sub", _settings.Subject },
                { "aud", _settings.Audience },
                { "purposeId", _settings.PurposeId },
                { "jti", jti.ToString("D").ToLower() },
                { "iat", issued.ToUnixTimestamp() },
                { "exp", expire_in.ToUnixTimestamp() }
            };

            // you can read directly the private key...
            // var rsaKey = GetPrivateKey(_settings.KeyPath);
            // string? client_assertion = JWT.Encode(payload, rsaKey, JwsAlgorithm.RS256, headers_rsa);

            // ...or you can load the private key from PEM
            var rsaParams = GetSecurityParameters(_settings.KeyPath);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                client_assertion = JWT.Encode(payload, rsa, JwsAlgorithm.RS256, headers);
            }

            return new PDNDClientAssertion()
            {
                KeyId= _settings.KeyId ,
                Algorithm = _settings.Algorithm,
                Type = _settings.Type,
                Issuer = _settings.Issuer ,
                Subject = _settings.Subject,
                Audience = _settings.Audience,
                PurposeId = _settings.PurposeId,
                TokenId = jti,
                IssuedAt = issued,
                Expiration= expire_in,
                ClientAssertion = client_assertion
            };
        }

        /// <inheritdoc />
        public async Task<PDNDTokenResponse> GetToken(string clientAssertion)
        {
            // In production code, don't destroy the HttpClient through using, but reuse it or use IHttpClientFactory factory
            using var httpClient = new HttpClient();

            var payload = new Dictionary<string, string>()
            {
                { "client_id", _settings.ClientId },
                { "client_assertion",clientAssertion},
                { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"},
                { "grant_type", "client_credentials"}
            };

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsync(_settings.ServerUrl, new FormUrlEncodedContent(payload));

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var tokenInfo = JsonSerializer.Deserialize<PDNDTokenResponse>(json);

            return tokenInfo;
        }

        private byte[] GetPrivateKey(string keyPath)
        {
            using (var reader = File.OpenText(keyPath))
            {
                // Extracting the payload
                string privateKeyFile = reader.ReadToEnd();

                string privateKeyContent = File.ReadAllText(keyPath).Trim();
                Regex regex = new Regex(@"-----(BEGIN|END) (RSA |OPENSSH |ENCRYPTED |)PRIVATE KEY-----[\W]*");
                string privateKey = regex.Replace(input: privateKeyContent, replacement: string.Empty);

                /*
                 * https://vcsjones.dev/key-formats-dotnet-3/
                 * “BEGIN RSA PRIVATE KEY” => RSA.ImportRSAPrivateKey
                 * “BEGIN PRIVATE KEY” => RSA.ImportPkcs8PrivateKey
                 * “BEGIN ENCRYPTED PRIVATE KEY” => RSA.ImportEncryptedPkcs8PrivateKey
                 * “BEGIN RSA PUBLIC KEY” => RSA.ImportRSAPublicKey
                 * “BEGIN PUBLIC KEY” => RSA.ImportSubjectPublicKeyInfo
                 */

                using var key = RSA.Create();
                key.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);

                return key.ExportPkcs8PrivateKey();
            }
        }

        private RSAParameters GetSecurityParameters(string keyPath)
        {
            RSAParameters rsaParams;
            using (var tr = new StringReader(File.ReadAllText(keyPath).Trim()))
            {
                var pemReader = new PemReader(tr);
                var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;
                if (keyPair == null)
                {
                    throw new Exception("Could not read RSA private key");
                }
                var privateRsaParams = keyPair.Private as RsaPrivateCrtKeyParameters;
                rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
            }

            return rsaParams;
        }
    }
}
