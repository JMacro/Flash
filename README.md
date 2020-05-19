# Flash :zap: 

#### 介绍
|                        解决方案 |                  说明 |
|:-------------------------------|:----------------------|
|  Flash.Extersions.System       | System基础数据类型扩展 | 
|  Flash.Extersions.UidGenerator | 分布式唯一ID生成器     | 
|  Flash.Extersions.DistributedLock | 分布式锁           |
|  Flash.Extersions.Cache.Redis | Nosql redis           |

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
