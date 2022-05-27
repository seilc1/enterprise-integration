﻿using EnterpriseIntegration.Attributes;
using Microsoft.Extensions.Logging;

namespace EnterpriseIntegration.TestCommon.Examples;

public class ServiceActivatorFlow001
{
    private readonly ILogger<ServiceActivatorFlow001> logger;

    public ServiceActivatorFlow001(ILogger<ServiceActivatorFlow001> logger)
    {
        this.logger = logger;
    }

    [ServiceActivator(inChannelName: "001_hello", outChannelName: "001_world")]
    public string Hello(string prefix)
    {
        logger.LogInformation("Hello({Prefix})", prefix);
        return $"{prefix} hello";
    }

    [ServiceActivator(inChannelName: "001_world", outChannelName: "001_end")]
    public string World(string data)
    {
        logger.LogInformation("World({Data})", data);
        return $"{data} world";
    }

    [Endpoint(inChannelName: "001_end")]
    public void End(string data)
    {
        logger.LogInformation("End({Data})", data);
    }
}