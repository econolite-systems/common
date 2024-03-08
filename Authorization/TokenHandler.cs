// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Authorization;

public class TokenHandler : ITokenHandler
{
    private readonly string _authority;
    private readonly HttpClient _client;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly ILogger _logger;
    private DateTime? _expire;
    private string? _token = string.Empty;

    public TokenHandler(IOptions<TokenHandlerOptions> options, ILogger<TokenHandler> logger, HttpClient client)
    {
        _logger = logger;
        _authority = options.Value.Authority ??
                     throw new NullReferenceException("config value Authentication:Authority");
        _clientId = options.Value.ClientId ?? throw new NullReferenceException("config value Authentication:ClientId");
        _clientSecret = options.Value.ClientSecret ??
                        throw new NullReferenceException("config value Authentication:ClientSecret");
        _client = client;
    }

    public async Task<AuthenticationHeaderValue> GetAuthHeaderAsync(CancellationToken cancellationToken)
    {
        var token = await GetTokenAsync(cancellationToken);
        return new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (_expire.HasValue && _expire.Value > DateTime.UtcNow) return _token;

        try
        {
            var parameters = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"response_type", "token"},
                {"client_id", _clientId},
                {"client_secret", _clientSecret}
            };
            string? tokenRequestUri;
            using (var response =
                   await _client.GetAsync($"{_authority}/.well-known/openid-configuration", cancellationToken))
            {
                var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                var discoveryDocument = JsonDocument.Parse(raw);
                var tokenEndpoint = discoveryDocument.RootElement.GetProperty("token_endpoint");
                tokenRequestUri = tokenEndpoint.GetString();
            }

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using var content = new FormUrlEncodedContent(parameters);
            using (var response = await _client.PostAsync(tokenRequestUri, content, cancellationToken))
            {
                if (response.IsSuccessStatusCode)
                {
                    var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                    var discoveryDocument = JsonDocument.Parse(raw);
                    var token = discoveryDocument.RootElement.GetProperty("access_token");
                    _token = token.GetString();
                    _expire = GetTokenExpire(_token);
                }
                else
                {
                    var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Failed to get JWT token: {error}", raw);
                }
            }
        }
        catch (HttpRequestException)
        {
            _logger.LogError("HttpRequest exception when getting JWT token");
        }
        catch (InvalidOperationException)
        {
            _logger.LogError("Invalid operation exception when getting JWT token");
        }

        return _token;
    }

    private DateTime? GetTokenExpire(string? idtoken)
    {
        var token = new JwtSecurityToken(idtoken);
        var expire = token.Payload.Exp ?? 0;
        if (expire == 0) return null;

        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expire);
    }
}

/// <summary>
///     Injected at startup via services.AddTokenHandler()
/// </summary>
public interface ITokenHandler
{
    /// <summary>
    ///     Gets an AuthenticationHeaderValue for use with Bearer authentication.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>AuthenticationHeaderValue with the Bearer Token set.</returns>
    Task<AuthenticationHeaderValue> GetAuthHeaderAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Uses Authentication:ClientId and Authentication:ClientSecret for a jwt token to authenticate an HTTP client.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>JWT token to use with Bearer authentication.</returns>
    Task<string?> GetTokenAsync(CancellationToken cancellationToken);
}
