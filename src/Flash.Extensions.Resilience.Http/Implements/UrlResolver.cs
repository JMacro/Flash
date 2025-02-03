﻿using Flash.LoadBalancer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Flash.Extensions.Resilience.Http
{
    public class HttpUrlResolver : IHttpUrlResolver
    {
        private readonly DynamicRoute.IServiceLocator _serviceLocator;
        private readonly DefaultLoadBalancerFactory<DynamicRoute.ServiceEndPoint> _balancerFactory;
        private readonly ILoadBalancer<DynamicRoute.ServiceEndPoint> _loadBalancer;

        public HttpUrlResolver(
            DynamicRoute.IServiceLocator serviceLocator)
        {
            this._serviceLocator = serviceLocator;
            this._balancerFactory = new DefaultLoadBalancerFactory<Flash.DynamicRoute.ServiceEndPoint>();
            this._loadBalancer = _balancerFactory.Resolve(() => new List<Flash.DynamicRoute.ServiceEndPoint>());

        }

        public async Task<string> Resolve(string value)
        {
            if (_serviceLocator != null)
            {

                var paramList = GetParameters(value);

                foreach (var param in paramList)
                {
                    if (!string.IsNullOrEmpty(param))
                    {
                        var args = param.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        var serviceName = args[0];
                        var tagFilter = "";
                        if (args.Length == 2)
                        {
                            tagFilter = args[1];
                        }

                        var endPoints = await _serviceLocator.GetFromCacheAsync(serviceName, tagFilter, TimeSpan.FromSeconds(15));

                        if (endPoints.Any())
                        {
                            //获取一个地址
                            var targetEndPoint = _loadBalancer.Resolve(endPoints.ToList());

                            return value.Replace("{" + param + "}", $"{targetEndPoint.Address}:{targetEndPoint.Port}");
                        }
                        else
                        {
                            throw new System.Exception($"Service “{serviceName}” endpoint not found");
                        }
                    }
                }

            }

            return value;
        }

        private List<string> GetParameters(string text)
        {
            var matchVale = new List<string>();
            string Reg = @"(?<=\{)[^\${}]*?(?=})";
            string key = string.Empty;
            foreach (Match m in Regex.Matches(text, Reg))
            {
                matchVale.Add(m.Value.TrimEnd('|'));
            }
            return matchVale;
        }
    }
}
