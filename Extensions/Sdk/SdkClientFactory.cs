// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Extensions.Sdk;

public interface ISdkClient
{
    string BaseUrl { get; set; }
    string Token { get; set; }
}

public interface ISdkClientFactory<out TClient> where TClient : ISdkClient
{
    TClient CreateClient(string token);
}

public class SdkClientFactory<TClient> : ISdkClientFactory<TClient> where TClient : ISdkClient
{
    private TClient _client;

    public SdkClientFactory(IHttpClientFactory factory, Func<HttpClient,TClient> clientFactory, IOptions<SdkFactoryOptions> options)
    {
        _client = clientFactory(factory.CreateClient(options.Value.HttpClientName));
        _client.BaseUrl = options.Value.BaseUrl;
    }

    public TClient CreateClient(string token)
    {
        _client.Token = token;
        return _client;
    }
    
    public TClient CreateClient(Func<string> token)
    {
        return this.CreateClient(token());
    }
}
