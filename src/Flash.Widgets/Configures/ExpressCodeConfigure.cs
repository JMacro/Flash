using System;
namespace Flash.Widgets.Configures
{
	public class ExpressCodeConfigure
	{
		public List<ExpressCodeItem> ExpressCodes { get; set; }
	}

	public class ExpressCodeItem
	{
		public string ExpressName { get; set; }
		public string KuaiDi100Code { get; set; }
	}
}

