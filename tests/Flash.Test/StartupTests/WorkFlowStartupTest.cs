﻿using Autofac;
using Flash.AspNetCore.WorkFlow;
using Flash.Test.EntityChange.Events;
using Flash.Test.ORM.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test.StartupTests
{
    public class WorkFlowStartupTest : BaseStartupTest
    {
        public WorkFlowStartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            services.AddFlash(flash =>
            {
                flash.AddUniqueIdGenerator(setup =>
                {
                    setup.CenterId = 0;
                    setup.UseStaticWorkIdCreateStrategy(0);
                });

                flash.AddWorkFlow(workFlow =>
                {
                    var connection = "Server=192.168.50.110;Port=63306;Database=workflow_dev;User=root;Password=admin@8225950;pooling=True;minpoolsize=1;maxpoolsize=100;connectiontimeout=180";
                    workFlow.RegisterDbContext<MigrationAssembly>(connection);
                });
            }, containerBuilder);
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        }
    }
}
