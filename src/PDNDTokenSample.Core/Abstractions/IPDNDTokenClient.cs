namespace PDNDTokenSample.Core.Abstractions
{
    using PDNDTokenSample.Core.Models;

    public interface IPDNDTokenClient
    {
        /// <summary>
        /// Gets the client assertions
        /// </summary>
        /// <returns>a <see cref="PDNDClientAssertion"/> instance containing all the details about client assertion</returns>
        PDNDClientAssertion GetClientAssertion();

        /// <summary>
        /// Gets a valid authentication token
        /// </summary>
        /// <param name="clientAssertion">the client assertion</param>
        /// <returns>a <see cref="PDNDTokenResponse"/> object with token details</returns>
        /// <exception cref="HttpRequestException">if the HTTP response was unsuccessful</exception>
        Task<PDNDTokenResponse> GetToken(string clientAssertion);
    }
}
