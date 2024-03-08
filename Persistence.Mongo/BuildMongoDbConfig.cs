// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Persistence.Mongo;

public class BuildMongoDbConfig : IConfigureOptions<MongoOptions>
{
    private readonly IConfiguration _configuration;

    public BuildMongoDbConfig(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void Configure(MongoOptions options)
    {
        _configuration.GetSection("Mongo").Bind(options);
    }
}

public class BuildMongoDbConnectionConfig : IConfigureOptions<MongoConnectionStringOptions>
{
    private readonly IConfiguration _configuration;

    public BuildMongoDbConnectionConfig(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void Configure(MongoConnectionStringOptions options)
    {
        _configuration.GetSection("ConnectionStrings").Bind(options);
    }
}
