using Flash.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.ORM
{
    public class FlashOrmDbContextBuilder : IFlashOrmDbContextBuilder
    {
        private readonly IServiceCollection _services;

        public FlashOrmDbContextBuilder(IServiceCollection services)
        {
            this._services = services;
        }

        public IServiceCollection Services { get { return this._services; } }
    }
}
