using Castle.Core;
using Flash.Extensions.Cache;
using Flash.Extensions.UidGenerator.WorkIdCreateStrategy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flash.Extensions.UidGenerator.RedisWorkId
{
    public sealed class RedisWorkIdCreateStrategy : IWorkIdCreateStrategy
    {
        private static readonly object _syncRoot = new object();
        private readonly ICacheManager _client;
        private readonly string _appId;
        private readonly string _resourceId;
        private readonly TimeSpan _cacheTTL;
        private readonly ILogger<RedisWorkIdCreateStrategy> _logger;
        private readonly int _centerId;
        private int? _workId;
        private readonly string _cacheName;

        public RedisWorkIdCreateStrategy(
            ICacheManager cacheClient,
            ILogger<RedisWorkIdCreateStrategy> logger,
            int CenterId,
            string appId,
            TimeSpan cacheTTL)
        {
            this._client = cacheClient;
            this._appId = appId;
            this._resourceId = $"{CenterId}:workid:{this._appId}";
            this._logger = logger;
            this._centerId = CenterId;
            this._cacheTTL = cacheTTL;
            this._cacheName = $"{System.Net.Dns.GetHostName()}-{Process.GetCurrentProcess().ProcessName}-{Process.GetCurrentProcess().Id}";

            try
            {
                GetWorkId();

                AppDomain.CurrentDomain.ProcessExit += delegate
                {
                    Dispose();
                };
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }
        }

        public int GetWorkId()
        {
            return CreateWorkId().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public int GetCenterId() => this._centerId;

        private async Task<int> CreateWorkId()
        {
            if (!_workId.HasValue)
            {
                while (true)
                {

                    try
                    {
                        var rs = this._client.LockTake($"{this._resourceId}:LOCK", this._cacheName, this._cacheTTL);
                        if (rs)
                        {
                            var kvList = GetAllList();
                            var workIdRange = new List<int> { };

                            for (int i = 0; i < IdWorker.MaxWorkerId; i++)
                            {
                                workIdRange.Add(i);
                            }

                            #region 排除已经存在workId
                            foreach (var item in kvList)
                            {
                                if (int.TryParse(item.Key.Replace($"{this._resourceId}:", ""), out int id))
                                {
                                    workIdRange.Remove(id);
                                }
                            }
                            #endregion

                            //存在可用的workId
                            if (workIdRange.Any())
                            {
                                var ret = this._client.StringSet<string>($"{this._resourceId}:{workIdRange.First().ToString()}", this._cacheName, this._cacheTTL);

                                if (!ret)
                                {
                                    throw new Exception($"Failed to allocate workid, failed to set workid #{workIdRange.First()}");
                                }

                                _workId = workIdRange.First();
                            }
                            else
                            {
                                throw new Exception($"Failed to allocate workid, no workid available");
                            }
                            break;
                        }
                        else
                        {
                            this._logger.LogWarning($"#lock={_resourceId}:LOCK Failed to allocate workid, try again in 5 seconds");
                            await System.Threading.Tasks.Task.Delay(1000);
                            continue;
                        }
                    }
                    finally
                    {
                        while (true)
                        {
                            var rs = this._client.LockRelease($"{this._resourceId}:LOCK", this._cacheName);
                            if (rs)
                            {
                                break;
                            }
                            else
                            {
                                await System.Threading.Tasks.Task.Delay(1000);
                                continue;
                            }
                        }
                    }
                }

            }

            return _workId.Value;
        }

        private IDictionary<string, string> GetAllList()
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < IdWorker.MaxWorkerId; i++)
            {
                var key = $"{this._resourceId}:{i}";
                var value = this._client.StringGet<string>($"{this._resourceId}:{i}");
                if (!string.IsNullOrEmpty(value))
                {
                    dict.Add(key, value);
                }
            }
            return dict;
        }

        public async void Dispose()
        {
            //释放WorkId
            if (_workId.HasValue)
            {
                while (true)
                {
                    var rs = _client.RemoveCache($"{this._resourceId}:{_workId.Value.ToString()}");
                    if (rs)
                    {
                        break;
                    }
                    else
                    {
                        await System.Threading.Tasks.Task.Delay(5000);
                        continue;
                    }
                }
            }
        }
    }
}
