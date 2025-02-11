using AutoMapper;
using Flash.Extensions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Infrastructure.Core
{
    public class MapperToEntity : Profile
    {
        public MapperToEntity()
        {
            Func<Attribute[], bool> IsMyAttribute = o =>
            {
                foreach (Attribute a in o)
                {
                    if (a is AutoMapperToAttribute)
                        return true;
                }
                return false;
            };

            var autoMapperType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(p =>
            {
                return IsMyAttribute(Attribute.GetCustomAttributes(p, true));
            }).Select(p => new
            {
                From = p,
                To = (Attribute.GetCustomAttribute(p, typeof(AutoMapperToAttribute), inherit: false) as AutoMapperToAttribute).GetMapperList()
            }).ToList();

            foreach (var type in autoMapperType)
            {
                type.To.ForEach(item =>
                {
                    CreateMap(type.From, item);
                    CreateMap(item, type.From);
                });
            }
        }
    }
}
