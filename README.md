# Flash :zap: 


#### 介绍
|                        解决方案 |                  说明 |                   |
|:-------------------------------|:----------------------|:----------------------|
|  Flash.Extensions.System       | System基础数据类型扩展 | [![NuGet version (Flash.Extensions.System)](https://img.shields.io/nuget/v/Flash.Extensions.System?style=flat)](https://www.nuget.org/packages/Flash.Extensions.System/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.System) |
|  Flash.Extensions.UidGenerator | 分布式唯一ID生成器     |  [![NuGet version (Flash.Extensions.UidGenerator)](https://img.shields.io/nuget/v/Flash.Extensions.UidGenerator?style=flat)](https://www.nuget.org/packages/Flash.Extensions.UidGenerator/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.UidGenerator)|
|  Flash.Extensions.Cache.Redis | Nosql redis           | [![NuGet version (Flash.Extersions.Cache.Redis)](https://img.shields.io/nuget/v/Flash.Extensions.Cache.Redis?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Cache.Redis/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Cache.Redis)|
|  Flash.Extensions.Cache.Redis.DependencyInjection | redis扩展           | [![NuGet version (Flash.Extensions.Cache.Redis.DependencyInjection)](https://img.shields.io/nuget/v/Flash.Extensions.Cache.Redis.DependencyInjection?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Cache.Redis.DependencyInjection/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Cache.Redis.DependencyInjection)|
|  Flash.Extensions.EventBus.RabbitMQ | RabbitMQ消息总线  | [![NuGet version (Flash.Extensions.EventBus.RabbitMQ)](https://img.shields.io/nuget/v/Flash.Extensions.EventBus.RabbitMQ?style=flat)](https://www.nuget.org/packages/Flash.Extensions.EventBus.RabbitMQ/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.EventBus.RabbitMQ)|
|  Flash.Extensions.ORM.EntityFrameworkCore | EF ORM  | [![NuGet version (Flash.Extensions.ORM.EntityFrameworkCore)](https://img.shields.io/nuget/v/Flash.Extensions.ORM.EntityFrameworkCore?style=flat)](https://www.nuget.org/packages/Flash.Extensions.ORM.EntityFrameworkCore/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.ORM.EntityFrameworkCore)|

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
