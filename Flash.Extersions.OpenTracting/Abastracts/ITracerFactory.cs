namespace Flash.Extersions.OpenTracting
{
    public interface ITracerFactory
    {
        /// <summary>
        /// 创建链路追踪器，默认返回系统默认链路追踪器
        /// </summary>
        /// <param name="tracerName">追踪器名称</param>
        /// <returns></returns>
        ITracer CreateTracer(string tracerName);
    }
}
