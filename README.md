# Flash :zap: 


#### 介绍
|                        解决方案 |                  说明 |                   |
|:-------------------------------|:----------------------|:----------------------|
|  Flash.Extersions.System       | System基础数据类型扩展 | [![NuGet version (Flash.Extersions.System)](https://img.shields.io/nuget/v/Flash.Extersions.System?style=flat)](https://www.nuget.org/packages/Flash.Extersions.System/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extersions.System) |
|  Flash.Extersions.UidGenerator | 分布式唯一ID生成器     |  [![NuGet version (Flash.Extersions.UidGenerator)](https://img.shields.io/nuget/v/Flash.Extersions.UidGenerator?style=flat)](https://www.nuget.org/packages/Flash.Extersions.UidGenerator/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extersions.UidGenerator)|
|  Flash.Extersions.DistributedLock | 分布式锁           | [![NuGet version (Flash.Extersions.DistributedLock)](https://img.shields.io/nuget/v/Flash.Extersions.DistributedLock?style=flat)](https://www.nuget.org/packages/Flash.Extersions.DistributedLock/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extersions.DistributedLock)|
|  Flash.Extersions.Cache.Redis | Nosql redis           | [![NuGet version (Flash.Extersions.Cache.Redis)](https://img.shields.io/nuget/v/Flash.Extersions.Cache.Redis?style=flat)](https://www.nuget.org/packages/Flash.Extersions.Cache.Redis/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extersions.Cache.Redis)|
|  Flash.Extersions.HealthChecks.Redis | redis健康检测           | [![NuGet version (Flash.Extersions.HealthChecks.Redis)](https://img.shields.io/nuget/v/Flash.Extersions.HealthChecks.Redis?style=flat)](https://www.nuget.org/packages/Flash.Extersions.HealthChecks.Redis/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extersions.HealthChecks.Redis)|
|  Flash.Extersions.RabbitMQ | RabbitMQ消息总线  | [![NuGet version (Flash.Extersions.RabbitMQ)](https://img.shields.io/nuget/v/Flash.Extersions.RabbitMQ?style=flat)](https://www.nuget.org/packages/Flash.Extersions.RabbitMQ/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extersions.RabbitMQ)|

#### 使用说明

1.  分布式唯一ID生成器
    
```
public IServiceProvider ConfigureServices(IServiceCollection services)
{
	.....
	services.AddFlash(flash =>
	{
		//注入分布式唯一ID生成器
		flash.AddUniqueIdGenerator(option =>
		{
			option.CenterId = int.Parse(Configuration["CenterId"]);
			option.UseStaticWorkIdCreateStrategy(int.Parse(Configuration["WorkId"]));
		});
	});
	....
	return services;
}
```

```
public class SystemRepository : ISystemRepository
{
	long uid = 0;
	public readonly IUniqueIdGenerator _uniqueIdGenerator;
	public SystemRepository(IUniqueIdGenerator uniqueIdGenerator)
	{
		this._uniqueIdGenerator = uniqueIdGenerator;
	}
	
	public long GetUid()
	{
		return _uniqueIdGenerator.NewId();
	}
}
```

2.  System基础数据类型扩展

```
class Program
{
	static void Main(string[] args)
	{
		string value1 = "0";

		//字符串转换相关操作
		Console.WriteLine(value1.Tolong(0));
		Console.WriteLine(value1.ToFloat());
		Console.WriteLine(value1.ToDecimal());
		Console.WriteLine(value1.ToBoolean());

		object value2 = "1";
		Console.WriteLine(value2.ToString().Tolong(0));
		Console.WriteLine(value2.ToFloat());
		Console.WriteLine(value2.ToDecimal());
		Console.WriteLine(value2.ToBoolean());

		Console.ReadKey();
	}
}
```

3.  RabbitMQ消息总线-客户端订阅

```
public IServiceProvider ConfigureServices(IServiceCollection services)
{
	.....
	services.AddFlash(flash =>
	{
		//添加消息队列总线
		flash.AddRabbitMQ(rabbitmq =>
		{
			rabbitmq.WithEndPoint(hostContext.Configuration["RabbitMQ:HostName"] ?? "localhost", int.Parse(hostContext.Configuration["RabbitMQ:Port"] ?? "5672"))
			.WithAuth(hostContext.Configuration["RabbitMQ:UserName"] ?? "guest", hostContext.Configuration["RabbitMQ:Password"] ?? "guest")
			.WithExchange(hostContext.Configuration["RabbitMQ:VirtualHost"] ?? "/")
			.WithSender(int.Parse(hostContext.Configuration["RabbitMQ:SenderMaxConnections"] ?? "10"), int.Parse(hostContext.Configuration["RabbitMQ:SenderAcquireRetryAttempts"] ?? "3"))
			.WithReceiver(
				ReceiverMaxConnections: int.Parse(hostContext.Configuration["RabbitMQ:ReceiverMaxConnections"] ?? "5"),
				ReveiverMaxDegreeOfParallelism: int.Parse(hostContext.Configuration["RabbitMQ:ReveiverMaxDegreeOfParallelism"] ?? "5"),
				ReceiverAcquireRetryAttempts: int.Parse(hostContext.Configuration["RabbitMQ:ReceiverAcquireRetryAttempts"] ?? "3"));

		});
	});
	....
	return services;
}

internal class ServiceContainerFactory : IServiceProviderFactory<ContainerBuilder>
{
	private readonly ContainerBuilder containerBuilder;

	public ServiceContainerFactory(
		ContainerBuilder containerBuilder)
	{
		this.containerBuilder = containerBuilder;
	}
	public ContainerBuilder CreateBuilder(IServiceCollection services)
	{
		containerBuilder.Populate(services);
		return containerBuilder;
	}

	public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
	{
		var container = containerBuilder.Build();
		var sp = new AutofacServiceProvider(container);
		sp.UseRabbitMQ(rabbitmq =>
		{
			//注册订阅处理逻辑
			rabbitmq.Register<Application.Events.FaceAnalysisEvent, Application.Events.FaceAnalysisEventHandler>(queueName: typeof(Application.Events.FaceAnalysisEventHandler).FullName);
			rabbitmq.Register<Application.Events.FaceAnalysisNoticeEvent, Application.Events.FaceAnalysisNoticeEventHandler>(queueName: typeof(Application.Events.FaceAnalysisNoticeEventHandler).FullName);
		});
		return sp;
	}
}
```

![img-source-from-https://github.com/docker/dockercraft](https://github.com/docker/dockercraft/raw/master/docs/img/contribute.png?raw=true)
