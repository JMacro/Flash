using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.UidGenerator
{
    /// <summary>
    /// Twiter SnowFlake唯一ID生成算法
    /// </summary>
    public class SnowflakeUniqueIdGenerator : IUniqueIdGenerator
    {
        readonly IdWorker idWorker;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“SnowflakeUniqueIdGenerator.SnowflakeUniqueIdGenerator(int, int)”的 XML 注释
        public SnowflakeUniqueIdGenerator(int WorkerId, int CenterId)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“SnowflakeUniqueIdGenerator.SnowflakeUniqueIdGenerator(int, int)”的 XML 注释
        {
            idWorker = new IdWorker(WorkerId, CenterId);
        }

        
#pragma warning disable CS1572 // XML 注释中有“Prefix”的 param 标记，但是没有该名称的参数
/// <summary>
        /// 生存唯一ID
        /// </summary>
        /// <param name="Prefix"></param>
        /// <returns></returns>
        public long NewId()
#pragma warning restore CS1572 // XML 注释中有“Prefix”的 param 标记，但是没有该名称的参数
        {
            return idWorker.NextId();
        }
    }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“DisposableAction”的 XML 注释
    public class DisposableAction : IDisposable
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“DisposableAction”的 XML 注释
    {
        readonly Action _action;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“DisposableAction.DisposableAction(Action)”的 XML 注释
        public DisposableAction(Action action)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“DisposableAction.DisposableAction(Action)”的 XML 注释
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _action = action;
        }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“DisposableAction.Dispose()”的 XML 注释
        public void Dispose()
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“DisposableAction.Dispose()”的 XML 注释
        {
            _action();
        }
    }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“TimeExtensions”的 XML 注释
    public static class TimeExtensions
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“TimeExtensions”的 XML 注释
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“TimeExtensions.currentTimeFunc”的 XML 注释
        public static Func<long> currentTimeFunc = InternalCurrentTimeMillis;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“TimeExtensions.currentTimeFunc”的 XML 注释

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“TimeExtensions.CurrentTimeMillis()”的 XML 注释
        public static long CurrentTimeMillis()
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“TimeExtensions.CurrentTimeMillis()”的 XML 注释
        {
            return currentTimeFunc();
        }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“TimeExtensions.StubCurrentTime(Func<long>)”的 XML 注释
        public static IDisposable StubCurrentTime(Func<long> func)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“TimeExtensions.StubCurrentTime(Func<long>)”的 XML 注释
        {
            currentTimeFunc = func;
            return new DisposableAction(() =>
            {
                currentTimeFunc = InternalCurrentTimeMillis;
            });
        }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“TimeExtensions.StubCurrentTime(long)”的 XML 注释
        public static IDisposable StubCurrentTime(long millis)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“TimeExtensions.StubCurrentTime(long)”的 XML 注释
        {
            currentTimeFunc = () => millis;
            return new DisposableAction(() =>
            {
                currentTimeFunc = InternalCurrentTimeMillis;
            });
        }

        private static readonly DateTime Jan1st1970 = new DateTime
           (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static long InternalCurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }


#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdWorker”的 XML 注释
    public class IdWorker
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdWorker”的 XML 注释
    {
        //基准时间
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdWorker.Twepoch”的 XML 注释
        public const long Twepoch = 1288834974657L;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdWorker.Twepoch”的 XML 注释
        //机器标识位数
        const int WorkerIdBits = 5;
        //数据标志位数
        const int DatacenterIdBits = 5;
        //序列号识位数
        const int SequenceBits = 12;
        //机器ID最大值
        const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        //数据标志ID最大值
        const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
        //序列号ID最大值
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);
        //机器ID偏左移12位
        private const int WorkerIdShift = SequenceBits;
        //数据ID偏左移17位
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        //时间毫秒左移22位
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdWorker.TimestampLeftShift”的 XML 注释
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdWorker.TimestampLeftShift”的 XML 注释

        private long _sequence = 0L;
        private long _lastTimestamp = -1L;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdWorker.WorkerId”的 XML 注释
        public long WorkerId { get; protected set; }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdWorker.WorkerId”的 XML 注释
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdWorker.DatacenterId”的 XML 注释
        public long DatacenterId { get; protected set; }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdWorker.DatacenterId”的 XML 注释
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdWorker.Sequence”的 XML 注释
        public long Sequence
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdWorker.Sequence”的 XML 注释
        {
            get { return _sequence; }
            internal set { _sequence = value; }
        }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdWorker.IdWorker(long, long, long)”的 XML 注释
        public IdWorker(long workerId, long datacenterId, long sequence = 0L)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdWorker.IdWorker(long, long, long)”的 XML 注释
        {
            // 如果超出范围就抛出异常
            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException(string.Format("worker Id 必须大于0，且不能大于MaxWorkerId： {0}", MaxWorkerId));
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException(string.Format("region Id 必须大于0，且不能大于MaxWorkerId： {0}", MaxDatacenterId));
            }

            //先检验再赋值
            WorkerId = workerId;
            DatacenterId = datacenterId;
            _sequence = sequence;
        }

        readonly object _lock = new Object();
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdWorker.NextId()”的 XML 注释
        public virtual long NextId()
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdWorker.NextId()”的 XML 注释
        {
            lock (_lock)
            {
                var timestamp = TimeGen();
                if (timestamp < _lastTimestamp)
                {
                    throw new Exception(string.Format("时间戳必须大于上一次生成ID的时间戳.  拒绝为{0}毫秒生成id", _lastTimestamp - timestamp));
                }

                //如果上次生成时间和当前时间相同,在同一毫秒内
                if (_lastTimestamp == timestamp)
                {
                    //sequence自增，和sequenceMask相与一下，去掉高位
                    _sequence = (_sequence + 1) & SequenceMask;
                    //判断是否溢出,也就是每毫秒内超过1024，当为1024时，与sequenceMask相与，sequence就等于0
                    if (_sequence == 0)
                    {
                        //等待到下一毫秒
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    //如果和上次生成时间不同,重置sequence，就是下一毫秒开始，sequence计数重新从0开始累加,
                    //为了保证尾数随机性更大一些,最后一位可以设置一个随机数
                    _sequence = new Random().Next(512);
                }

                _lastTimestamp = timestamp;
                return ((timestamp - Twepoch) << TimestampLeftShift) | (DatacenterId << DatacenterIdShift) | (WorkerId << WorkerIdShift) | _sequence;
            }
        }

        // 防止产生的时间比之前的时间还要小（由于NTP回拨等问题）,保持增量的趋势.
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdWorker.TilNextMillis(long)”的 XML 注释
        protected virtual long TilNextMillis(long lastTimestamp)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdWorker.TilNextMillis(long)”的 XML 注释
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        // 获取当前的时间戳
#pragma warning disable CS1591 // 缺少对公共可见类型或成员“IdWorker.TimeGen()”的 XML 注释
        protected virtual long TimeGen()
#pragma warning restore CS1591 // 缺少对公共可见类型或成员“IdWorker.TimeGen()”的 XML 注释
        {
            return TimeExtensions.CurrentTimeMillis();
        }
    }
}
