# Flash.AspNetCore

- [Flash.AspNetCore](#)
	- [介绍](#介绍)
	- [待实现路线](#待实现路线)
	- [使用说明](#使用说明)
		- [Consul 服务注册与发现](#consul-服务注册与发现)
		- [Nacos 服务注册与发现](#nacos-服务注册与发现)
		- [Redis 分布式缓存](#redis-分布式缓存)
		- [唯一Id生成器](#唯一id生成器)
		- [Jaeger 日志链路](#jaeger-日志链路)
		- [HealthCheck 健康检查](#healthcheck-健康检查)

## 介绍
|                        组件 |                  说明 |                   |
|:-------------------------------|:----------------------|:----------------------|
|  [Flash.Extensions.DynamicRoute.Consul](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.DynamicRoute.Consul) | Consul服务注册与发现     |  [![NuGet version (Flash.Extensions.DynamicRoute.Consul)](https://img.shields.io/nuget/v/Flash.Extensions.DynamicRoute.Consul?style=flat)](https://www.nuget.org/packages/Flash.Extensions.DynamicRoute.Consul/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.DynamicRoute.Consul)|
|  [Flash.Extensions.DynamicRoute.Nacos](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.DynamicRoute.Nacos) | Nacos服务注册与发现       | [![NuGet version (Flash.Extensions.DynamicRoute.Nacos)](https://img.shields.io/nuget/v/Flash.Extensions.DynamicRoute.Nacos?style=flat)](https://www.nuget.org/packages/Flash.Extensions.DynamicRoute.Nacos/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.DynamicRoute.Nacos)|
|  [Flash.Extensions.Cache.Redis](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.Cache.Redis) | redis分布式缓存           | [![NuGet version (Flash.Extensions.Cache.Redis)](https://img.shields.io/nuget/v/Flash.Extensions.Cache.Redis?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Cache.Redis/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Cache.Redis)|
|  [Flash.Extensions.UidGenerator](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.UidGenerator) | 唯一Id生成器  | [![NuGet version (Flash.Extensions.UidGenerator)](https://img.shields.io/nuget/v/Flash.Extensions.UidGenerator?style=flat)](https://www.nuget.org/packages/Flash.Extensions.UidGenerator/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.UidGenerator)|
|  [Flash.Extensions.UidGenerator.ConsulWorkId](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.UidGenerator.ConsulWorkId) | 唯一Id生成器（Consul WorkId）  | [![NuGet version (Flash.Extensions.UidGenerator.ConsulWorkId)](https://img.shields.io/nuget/v/Flash.Extensions.UidGenerator.ConsulWorkId?style=flat)](https://www.nuget.org/packages/Flash.Extensions.UidGenerator.ConsulWorkId/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.UidGenerator.ConsulWorkId)|
|  [Flash.Extensions.UidGenerator.RedisWorkId](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.UidGenerator.RedisWorkId) | 唯一Id生成器（Redis WorkId）  | [![NuGet version (Flash.Extensions.UidGenerator.RedisWorkId)](https://img.shields.io/nuget/v/Flash.Extensions.UidGenerator.RedisWorkId?style=flat)](https://www.nuget.org/packages/Flash.Extensions.UidGenerator.RedisWorkId/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.UidGenerator.RedisWorkId)|
|  [Flash.Extensions.OpenTracting.Jaeger](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.OpenTracting.Jaeger) | Jaeger链路日志  | [![NuGet version (Flash.Extensions.OpenTracting.Jaeger)](https://img.shields.io/nuget/v/Flash.Extensions.OpenTracting.Jaeger?style=flat)](https://www.nuget.org/packages/Flash.Extensions.OpenTracting.Jaeger/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.OpenTracting.Jaeger)|
|  [Flash.Extensions.HealthChecks.MySql](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.HealthChecks.MySql) | MySql健康检测  | [![NuGet version (Flash.Extensions.HealthChecks.MySql)](https://img.shields.io/nuget/v/Flash.Extensions.HealthChecks.MySql?style=flat)](https://www.nuget.org/packages/Flash.Extensions.HealthChecks.MySql/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.HealthChecks.MySql)|
|  [Flash.Extensions.HealthChecks.RabbitMQ](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.HealthChecks.RabbitMQ) | RabbitMQ健康检测  | [![NuGet version (Flash.Extensions.HealthChecks.RabbitMQ)](https://img.shields.io/nuget/v/Flash.Extensions.HealthChecks.RabbitMQ?style=flat)](https://www.nuget.org/packages/Flash.Extensions.HealthChecks.RabbitMQ/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.HealthChecks.RabbitMQ)|
|  [Flash.Extensions.HealthChecks.Redis](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.HealthChecks.Redis) | Redis健康检测  | [![NuGet version (Flash.Extensions.HealthChecks.Redis)](https://img.shields.io/nuget/v/Flash.Extensions.HealthChecks.Redis?style=flat)](https://www.nuget.org/packages/Flash.Extensions.HealthChecks.Redis/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.HealthChecks.Redis)|

## 使用说明

###  `Consul` 服务注册与发现
    
```
{
	"FlashConfiguration": {
		"Consul": {
			"Enable": true,//是否启用组件
			"Config": {
				"SERVICE_REGISTRY_ADDRESS": "localhost",//服务注册中心地址
				"SERVICE_REGISTRY_PORT": "8500",//服务注册中心端口
				"SERVICE_REGISTRY_TOKEN": "",//服务注册中心访问Token
				"SERVICE_SELF_REGISTER": true,//采用服务自注册模式
				"SERVICE_ID": "",//服务Id
				"SERVICE_NAME": "example",//服务名称
				"SERVICE_TAGS": "dev",//服务标签
				"SERVICE_REGION": "dc1",//服务区域
				"SERVICE_CHECK_HTTP": "/healthcheck",//Http健康检查地址(默认:/healthcheck)
				"SERVICE_CHECK_TCP": null,//TCP健康检查
				"SERVICE_CHECK_SCRIPT": null,//脚本健康检查
				"SERVICE_CHECK_TTL": "15",//TTL健康检查
				"SERVICE_CHECK_INTERVAL": "5",//服务监控检查周期(默认:15s)
				"SERVICE_CHECK_TIMEOUT": "5"//服务监控检查超时时间(默认:5s)
			}
		}
	}
}
```

###  `Nacos` 服务注册与发现

```
{
	"FlashConfiguration": {
		"Nacos": {
			"Enable": false,//是否启用组件
			"Config": {
				"ServerAddresses": [ "http://localhost:8848" ],
				"Listeners": [
					{
						"Optional": false,
						"DataId": "common",
						"Group": "DEFAULT_GROUP"
					}
				],
				"EndPoint": "",
				"DefaultTimeOut": 15000,
				"Namespace": "public",
				"ListenInterval": 1000,
				"ServiceName": "example",
				"GroupName": "DEFAULT_GROUP",
				"ClusterName": "DEFAULT",
				"Ip": "",
				"PreferredNetworks": "",
				"Port": 0,
				"Weight": 100,
				"RegisterEnabled": true,
				"InstanceEnabled": true,
				"Ephemeral": true,
				"Secure": false,
				"AccessKey": "",
				"SecretKey": "",
				"UserName": "",
				"Password": "",
				"ConfigUseRpc": false,
				"NamingUseRpc": false,
				"NamingLoadCacheAtStart": "",
				"LBStrategy": "WeightRandom", //WeightRandom WeightRoundRobin
				"Metadata": {
					"env": ""
				}
			}
		}
	}
}
```

###  `Redis` 分布式缓存

```
{
	"FlashConfiguration": {
		"Cache": {
			"Enable": true,//是否启用组件
			"CacheType": "Redis",//缓存类型，目前仅支持Redis缓存
			"RedisConfig": {
				"Host": "192.168.50.110:63100",
				"Password": "tY7cRu9HG_jyDw2r",
				"Db": 0,
				"DistributedLock": true,
				"KeyPrefix": "Example"
			}
		}
	}
}
```

###  唯一Id生成器

```
{
	"FlashConfiguration": {
		"UniqueIdGenerator": {
			"Enable": true,//是否启用组件
			"GeneratorType": "RedisWorkId",//WorkId生成策略类型，支持StaticWorkId、ConsulWorkId、RedisWorkId
			"CenterId": 0,
			"WorkId": 0,
			"AppId": "example"
		}
	}
}
```
注意：
1.	当GeneratorType设置为ConsulWorkId时，需添加Consul配置信息，可参考[Consul配置](#consul-服务注册与发现)。
2.	当GeneratorType设置为RedisWorkId时，需添加Cache配置信息，可参考[Cache配置](#redis-分布式缓存)，其中CacheType应设置为Redis。

###  `Jaeger` 日志链路

```
{
	"FlashConfiguration": {
		"LoggerTracing": {
			"Enable": true,//是否启用组件
			"TracerType": "Jaeger",//日志链路类型，目前只支持Jaeger
			"RequestLogger": true,//
			"ResponseLogger": true,
			"JaegerConfig": {
				"Open": true,//是否开启
				"FlushIntervalSeconds": 15,//刷新周期
				"SamplerType": "const",//采样类型（默认：全量）
				"AgentHost": "",//代理主机
				"AgentPort": 5775,//代理端口
				"SerivceName": "Example",//服务名称
				"EndPoint": "http://192.168.50.242:14268/api/traces",//终结点地址
				"IgnorePaths": [ "/healthcheck" ]//忽略路径
			}
		}
	}
}
```

###  `HealthCheck` 健康检查

```
{
	"FlashConfiguration": {
		"HealthCheck": {
			"Enable": true,
			"CheckType": [ "Redis", "MySql", "RabbitMQ" ]
		}
	}
}
```

注意：
1.	当CheckType设置为Redis时，需添加Cache配置信息，可参考[Cache配置](#redis-分布式缓存)，其中CacheType应设置为Redis。
2.	当CheckType设置为MySql时，需添加DbConnectionString配置信息。
```
{
	"DbConnectionString": {
		"MySqlDB1": "Server=192.168.50.110;Port=63306;Database=ls_school_dev;User=root;Password=123456;pooling=True;minpoolsize=1;maxpoolsize=100;connectiontimeout=180"
	}
}
```
3.	当CheckType设置为RabbitMQ时，需注入EventBus组件。
```
services.AddFlash(flash =>
{
	flash.AddEventBus(bus =>
	{
		bus.UseRabbitMQ(rabbitmq =>
		{
			var hostName = Environment.GetEnvironmentVariable("RabbitMQ:HostName", EnvironmentVariableTarget.Machine);
			var port = Environment.GetEnvironmentVariable("RabbitMQ:Port", EnvironmentVariableTarget.Machine);
			var userName = Environment.GetEnvironmentVariable("RabbitMQ:UserName", EnvironmentVariableTarget.Machine);
			var password = Environment.GetEnvironmentVariable("RabbitMQ:Password", EnvironmentVariableTarget.Machine);
			var virtualHost = Environment.GetEnvironmentVariable("RabbitMQ:VirtualHost", EnvironmentVariableTarget.Machine);

			rabbitmq.WithEndPoint(hostName ?? "localhost", int.Parse(port ?? "5672"))
			.WithPrefixName("自定义前缀")
			.WithAuth(userName ?? "guest", password ?? "guest")
			.WithExchange(virtualHost ?? "/", Exchange: $"{GetType().FullName}")
			.WithSender(int.Parse(Configuration["RabbitMQ:SenderMaxConnections"] ?? "10"), int.Parse(Configuration["RabbitMQ:SenderAcquireRetryAttempts"] ?? "3"))
			.WithReceiver(
				ReceiverMaxConnections: int.Parse(Configuration["RabbitMQ:ReceiverMaxConnections"] ?? "5"),
				ReveiverMaxDegreeOfParallelism: int.Parse(Configuration["RabbitMQ:ReveiverMaxDegreeOfParallelism"] ?? "5"),
				ReceiverAcquireRetryAttempts: int.Parse(Configuration["RabbitMQ:ReceiverAcquireRetryAttempts"] ?? "3"));
		});
	});
});
```

![img-source-from-https://github.com/docker/dockercraft](https://github.com/docker/dockercraft/raw/master/docs/img/contribute.png?raw=true)
