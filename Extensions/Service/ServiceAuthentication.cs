// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Extensions.Service;

public class ServiceAuthentication : IServiceAuthentication
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServiceAuthenticationOptions _options;
    private string _token;
    private readonly ILogger<ServiceAuthentication> _logger;
    

    public ServiceAuthentication(IHttpClientFactory httpClientFactory, ILogger<ServiceAuthentication> logger, IOptions<ServiceAuthenticationOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options.Value;
        
        _token = string.Empty;
    }

    public async Task<AuthenticationHeaderValue> GetAuthHeaderAsync(bool refresh = false)
    {
        return new AuthenticationHeaderValue("Bearer", await GetTokenAsync(refresh));
    }
    
    public async Task<string> GetTokenAsync(bool refresh = false)
    {
        if (!refresh && !string.IsNullOrEmpty(_token))
        {
            return _token;
        }
        var client = _httpClientFactory.CreateClient("serviceAuth");
        _token = await GetCurrentAsync(client, new CancellationToken(false));
        return _token;
    }
    
    public async Task<string> GetCurrentAsync(HttpClient client, CancellationToken cancellationToken)
    {
        var result = string.Empty;

        try
        {
            var parameters = new Dictionary<string, string>
            {
                { "acr_values", $"tenantId:{_options.Tenant}" },
                { "grant_type", $"password" },
                { "username", _options.Username },
                { "password", _options.Password },
                { "client_id", "externalapi" },
            };
            string tokenrequesturi;
            using (var response = await client.GetAsync($"{_options.Issuer.TrimEnd('/')}/.well-known/openid-configuration", cancellationToken))
            {
                var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                var discoverydocument = JsonDocument.Parse(raw);
                var tokenendpoint = discoverydocument.RootElement.GetProperty("token_endpoint");
                tokenrequesturi = tokenendpoint.GetString() ?? string.Empty;
            }
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using var content = new FormUrlEncodedContent(parameters);
            using (var response = await client.PostAsync(tokenrequesturi, content, cancellationToken))
            {
                if (response.IsSuccessStatusCode)
                {
                    var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                    var discoverydocument = JsonDocument.Parse(raw);
                    var token = discoverydocument.RootElement.GetProperty("access_token");
                    result = token.GetString() ?? string.Empty;
                }
                else
                {
                    _logger.LogCritical("Failed to get JWT token");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HttpRequest exception");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation exception");
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("GetCurrentAsync operation has been canceled");
        }

        return result;
    }
}
