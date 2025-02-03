using Autofac;
using Flash.AspNetCore.WorkFlow;
using Flash.AspNetCore.WorkFlow.Application.Abastracts.Services;
using Flash.AspNetCore.WorkFlow.Domain;
using Flash.AspNetCore.WorkFlow.Domain.Services;
using Flash.AspNetCore.WorkFlow.Infrastructure;
using Flash.AspNetCore.WorkFlow.Infrastructure.Core;
using Flash.AspNetCore.WorkFlow.Infrastructure.DomainEventsDispatching;
using Flash.Core;
using Flash.Extensions;
using Flash.Extensions.ORM;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtersion
    {
        /// <summary>
        /// 添加工作流
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IFlashHostBuilder AddWorkFlow(this IFlashHostBuilder hostBuilder, Action<IFlashWorkFlowBuilder> action)
        {
            var builder = new FlashWorkFlowBuilder(hostBuilder.Services, hostBuilder);
            action(builder);
             
            hostBuilder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<MapperToEntity>();
            });

            hostBuilder.Services.TryAddScoped<IWorkFlowUnitOfWork, WorkFlowUnitOfWork>();
            hostBuilder.Services.TryAddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
            hostBuilder.Services.TryAddScoped<IDomainEventsAccessor, DomainEventsAccessor>();

            //hostBuilder.Services.TryAddScoped<IFlowConfigService, FlowConfigService>();
            //hostBuilder.Services.TryAddScoped<IFlowFieldConfigService, FlowFieldConfigService>();
            hostBuilder.Services.TryAddScoped<Flash.AspNetCore.WorkFlow.Domain.Abastracts.Services.IFlowConfigService, FlowConfigService>();

            hostBuilder.Services.AddMediatR(mediatR =>
            {
                mediatR.RegisterServicesFromAssembly(typeof(IMediator).GetTypeInfo().Assembly);
                mediatR.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetCurrentAssemblys("Microsoft", "System").ToArray());
            });
            return hostBuilder;
        }

        /// <summary>
        /// 注册工作流Db上下文
        /// </summary>
        /// <typeparam name="TMigrationAssembly"></typeparam>
        /// <param name="builder"></param>
        /// <param name="connectionStr">数据库连接字符串</param>
        /// <returns></returns>
        public static IFlashWorkFlowBuilder RegisterDbContext<TMigrationAssembly>(this IFlashWorkFlowBuilder builder, string connectionStr) where TMigrationAssembly : IMigrationAssembly
        {
            builder.FlashHost.AddORM(orm =>
            {
                orm.UseEntityFramework(option =>
                {
                    option.RegisterDbContexts<WorkFlowDbContext, TMigrationAssembly>(connectionStr, orm.Services.BuildServiceProvider().GetService<IConfiguration>());
                });
            });

            return builder;
        }
    }
}
