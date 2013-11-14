NLog.Targets.Redis
==================

The NLog Redis Target is a custom target for NLog version 2.0 allowing you to send logging messages to a Redis list or channel. Very useful for logstash integration.

Credit to Sotiridis Vassilis for the original code and package.

USAGE
==================
Now, from the NLog configuration, you can choose between using the Redis list or channel to deliver your messages.

dataType: [channel, list]
key: your list id or the channel name

Example:

<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" throwExceptions="true">
    <extensions>
      <add assembly="NLog.Targets.Redis" />
    </extensions>
    <targets>
      <target xsi:type="Redis" name="redis" host="127.0.0.1" key="logstash" dataType="channel" />
    </targets>
    <rules>
      <logger name="*" minlevel="Info" writeTo="redis" />
    </rules>
</nlog>

NuGet Package
==================

[NLog.Targets.Redis](https://www.nuget.org/packages/NLog.Targets.Redis)
