# Flash ⚡ 


#### 介绍
|                        解决方案 |                  说明 |                   |
|:-------------------------------|:----------------------|:----------------------|
|  [Flash.Extensions.System](https://github.com/JMacro/Flash/tree/master/src/Flash.Extensions.System)       | System基础数据类型扩展 | [![NuGet version (Flash.Extensions.System)](https://img.shields.io/nuget/v/Flash.Extensions.System?style=flat)](https://www.nuget.org/packages/Flash.Extensions.System/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.System) |
|  Flash.Extensions.UidGenerator | 分布式唯一ID生成器     |  [![NuGet version (Flash.Extensions.UidGenerator)](https://img.shields.io/nuget/v/Flash.Extensions.UidGenerator?style=flat)](https://www.nuget.org/packages/Flash.Extensions.UidGenerator/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.UidGenerator)|
|  Flash.Extensions.Cache.Redis | Nosql redis           | [![NuGet version (Flash.Extersions.Cache.Redis)](https://img.shields.io/nuget/v/Flash.Extensions.Cache.Redis?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Cache.Redis/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Cache.Redis)|
|  Flash.Extensions.Cache.Redis.DependencyInjection | redis扩展           | [![NuGet version (Flash.Extensions.Cache.Redis.DependencyInjection)](https://img.shields.io/nuget/v/Flash.Extensions.Cache.Redis.DependencyInjection?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Cache.Redis.DependencyInjection/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Cache.Redis.DependencyInjection)|
|  Flash.Extensions.EventBus.RabbitMQ | RabbitMQ消息总线  | [![NuGet version (Flash.Extensions.EventBus.RabbitMQ)](https://img.shields.io/nuget/v/Flash.Extensions.EventBus.RabbitMQ?style=flat)](https://www.nuget.org/packages/Flash.Extensions.EventBus.RabbitMQ/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.EventBus.RabbitMQ)|
|  Flash.Extensions.ORM.EntityFrameworkCore | EF ORM  | [![NuGet version (Flash.Extensions.ORM.EntityFrameworkCore)](https://img.shields.io/nuget/v/Flash.Extensions.ORM.EntityFrameworkCore?style=flat)](https://www.nuget.org/packages/Flash.Extensions.ORM.EntityFrameworkCore/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.ORM.EntityFrameworkCore)|
|  Flash.Extensions.Resilience.Http | 弹性Http  | [![NuGet version (Flash.Extensions.Resilience.Http)](https://img.shields.io/nuget/v/Flash.Extensions.Resilience.Http?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Resilience.Http/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Resilience.Http)|
|  Flash.Extensions.Email | Email工具  | [![NuGet version (Flash.Extensions.Email)](https://img.shields.io/nuget/v/Flash.Extensions.Email?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Email/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Email)|
|  Flash.Extensions.Office.Npoi | Office Npoi工具  | [![NuGet version (Flash.Extensions.Office.Npoi)](https://img.shields.io/nuget/v/Flash.Extensions.Office.Npoi?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Office.Npoi/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Office.Npoi)|
|  Flash.Extensions.Job.Hangfire | Job Hangfire工具  | [![NuGet version (Flash.Extensions.Job.Hangfire)](https://img.shields.io/nuget/v/Flash.Extensions.Job.Hangfire?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Job.Hangfire/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Job.Hangfire)|
|  Flash.Extensions.Job.Hangfire.AspNetCore | Job Hangfire仪表盘  | [![NuGet version (Flash.Extensions.Job.Hangfire.AspNetCore)](https://img.shields.io/nuget/v/Flash.Extensions.Job.Hangfire.AspNetCore?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Job.Hangfire.AspNetCore/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Job.Hangfire.AspNetCore)|
|  Flash.Extensions.Job.Quartz | Job Quartz工具  | [![NuGet version (Flash.Extensions.Job.Quartz)](https://img.shields.io/nuget/v/Flash.Extensions.Job.Quartz?style=flat)](https://www.nuget.org/packages/Flash.Extensions.Job.Quartz/) ![Nuget](https://img.shields.io/nuget/dt/Flash.Extensions.Job.Quartz)|


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

4.  office Npoi使用

```
//注入组件
public IServiceProvider ConfigureServices(IServiceCollection services)
{
	.....
	services.AddFlash(flash =>
	{
		//添加Office组件
		flash.AddOffice(
			setting => { },
			action =>
			{
				action.UseNpoi();
			}
		);
	});
	....
	return services;
}
```

```
//Office组件使用
public void TestWriteExcel()
{
	var tool = ServiceProvider.GetService<IOfficeTools>();

	var headerColumns = new List<ExcelHeaderColumn>();
	headerColumns.Add(ExcelHeaderColumn.Create("序号", nameof(StudentInfo.Id)));
	headerColumns.Add(ExcelHeaderColumn.Create("姓名", nameof(StudentInfo.Name)));
	headerColumns.Add(ExcelHeaderColumn.Create("年龄", nameof(StudentInfo.Age)));
	headerColumns.Add(ExcelHeaderColumn.Create("平均收入", nameof(StudentInfo.Monery), "[Green]0.00;[Red]-0.00"));
	headerColumns.Add(ExcelHeaderColumn.Create("日期", nameof(StudentInfo.Date), "yyyy-MM-dd"));
	headerColumns.Add(ExcelHeaderColumn.Create("枚举", nameof(StudentInfo.Enum)));
	headerColumns.Add(ExcelHeaderColumn.Create("是否启用", nameof(StudentInfo.IsEnable)));

	var dataSource = new List<StudentInfo>();
	var randon = new Random((int)TimeSpan.TicksPerSecond);
	for (int i = 1; i <= 1000; i++)
	{
		dataSource.Add(new StudentInfo { Id = Guid.NewGuid(), Name = $"姓名{i}", Age = 11, Date = DateTime.Now, Monery = 100000 - randon.NextDouble() * 1000000, Enum = TestEnum.TT, IsEnable = false });
	}

	var buffer = tool.WriteExcel(dataSource, headerColumns);
	Assert.IsNotNull(buffer);

	File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "excel", DateTime.Now.ToFileTime().ToString() + ".xls"), buffer);


	var datas = tool.ReadExcel<StudentInfo>(buffer, headerColumns);
	Assert.IsNotNull(datas);
}

public void TestWriteExcelByMultipleSheet()
{
	var tool = ServiceProvider.GetService<IOfficeTools>();

	var headerColumns1 = ExcelHeaderColumn.Create<StudentInfo>();

	var headerColumns2 = new List<ExcelHeaderColumn>();
	headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-序号", nameof(StudentInfo.Id)));
	headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-姓名", nameof(StudentInfo.Name)));
	headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-年龄", nameof(StudentInfo.Age)));
	headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-平均收入", nameof(StudentInfo.Monery), "[Green]0.00;[Red]-0.00"));
	headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-日期", nameof(StudentInfo.Date), "yyyy-MM-dd"));
	headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-枚举", nameof(StudentInfo.Enum)));
	headerColumns2.Add(ExcelHeaderColumn.Create("Sheet2-是否启用", nameof(StudentInfo.IsEnable)));

	var headerColumns3 = new List<ExcelHeaderColumn>();
	headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-序号", nameof(StudentInfo.Id)));
	headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-姓名", nameof(StudentInfo.Name)));
	headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-年龄", nameof(StudentInfo.Age)));
	headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-平均收入", nameof(StudentInfo.Monery), "[Green]0.00;[Red]-0.00"));
	headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-日期", nameof(StudentInfo.Date), "yyyy-MM-dd"));
	headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-枚举", nameof(StudentInfo.Enum)));
	headerColumns3.Add(ExcelHeaderColumn.Create("Sheet2-是否启用", nameof(StudentInfo.IsEnable)));

	var dataSource = new List<StudentInfo>();
	var randon = new Random((int)TimeSpan.TicksPerSecond);
	for (int i = 1; i <= 10000; i++)
	{
		dataSource.Add(new StudentInfo { Id = Guid.NewGuid(), Name = $"姓名{i}", Age = 11, Date = DateTime.Now, Monery = 100000 - randon.NextDouble() * 1000000, Enum = TestEnum.TT, IsEnable = false });
	}

	dataSource.Add(new StudentInfo { Id = Guid.NewGuid(), Name = null, Age = 11, Date = DateTime.Now, Monery = 100000 - randon.NextDouble() * 1000000, Enum = TestEnum.TT, IsEnable = false });

	var buffer = tool.WriteExcelMultipleSheet(
		SheetInfo.Create("Sheet1", dataSource, headerColumns1),
		SheetInfo.Create("Sheet2", dataSource, headerColumns2),
		SheetInfo.Create("Sheet3", dataSource, headerColumns3));
	Assert.IsNotNull(buffer);

	var fileName = DateTime.Now.ToFileTime().ToString() + ".xls";
	File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, "excel", fileName), buffer);

	var datas = tool.ReadExcel<StudentInfo>(buffer, "Sheet2", headerColumns2);
	Assert.IsNotNull(datas);

	var emailService = ServiceProvider.GetService<IEmailService>();
	Assert.IsNotNull(emailService);

	//emailService.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试", AttachmentInfo.Create(fileName, new MemoryStream(buffer)), System.Text.Encoding.UTF8);
	//emailService.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试Path", AttachmentInfo.Create(Path.Combine(AppContext.BaseDirectory, "excel", fileName)), System.Text.Encoding.UTF8);

	emailService.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试Path", AttachmentInfo.Create(Path.Combine(AppContext.BaseDirectory, "excel", fileName), Path.Combine(AppContext.BaseDirectory, "excel", fileName)), System.Text.Encoding.UTF8);
}

public class StudentInfo
{
	[ExcelHeader("", "序号")]
	public Guid Id { get; set; }
	[ExcelHeader(nameof(Id), "姓名")]
	public string Name { get; set; }
	[ExcelHeader("", "年龄")]
	public int Age { get; set; }
	[ExcelHeader(nameof(Age), "平均收入", "[Green]0.00;[Red]-0.00")]
	public double Monery { get; set; }
	[ExcelHeader(nameof(Monery), "日期", "yyyy-MM-dd")]
	public DateTime Date { get; set; }
	[ExcelHeader(nameof(Date), "枚举")]
	public TestEnum Enum { get; set; }
	[ExcelHeader(nameof(Enum), "是否启用")]
	public bool IsEnable { get; set; }
}

public enum TestEnum
{
	All,
	TT
}
```

5.  Email使用

```
//注入组件
public IServiceProvider ConfigureServices(IServiceCollection services)
{
	.....
	services.AddFlash(flash =>
	{
		//添加Email组件
		flash.AddMailKit(Configuration.GetSection("Email").Get<EmailConfig>());
	});
	....
	return services;
}
```
```
//Office组件使用
public void TestSendEmail()
{
	var tool = ServiceProvider.GetService<IEmailService>();
	Assert.IsNotNull(tool);

	tool.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试", System.Text.Encoding.UTF8);
}
```

![img-source-from-https://github.com/docker/dockercraft](https://github.com/docker/dockercraft/raw/master/docs/img/contribute.png?raw=true)
