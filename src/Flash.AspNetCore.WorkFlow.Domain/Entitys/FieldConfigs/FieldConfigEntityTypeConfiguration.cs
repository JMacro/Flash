using Flash.AspNetCore.WorkFlow.Domain.Entitys.FieldConfigs;
using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs
{
    internal class FieldConfigEntityTypeConfiguration : IEntityTypeConfiguration<FieldConfig>
    {
        public void Configure(EntityTypeBuilder<FieldConfig> builder)
        {
            builder.ToTable("sys_workflow_field_config");
            builder.HasKey(x => x.Id);

            builder.Property<long>(nameof(FieldConfig.WorkFlowModuleSceneConfigId)).HasColumnName(nameof(FieldConfig.WorkFlowModuleSceneConfigId));
            builder.Property<string>(nameof(FieldConfig.TableName)).HasColumnName(nameof(FieldConfig.TableName));
            builder.Property<string>(nameof(FieldConfig.Name)).HasColumnName(nameof(FieldConfig.Name));
            builder.Property<EWorkFlowFieldDataType>(nameof(FieldConfig.Type)).HasColumnName(nameof(FieldConfig.Type));
            builder.Property<string>(nameof(FieldConfig.DisplayName)).HasColumnName(nameof(FieldConfig.DisplayName));
            builder.Property<string>(nameof(FieldConfig.Unit)).HasColumnName(nameof(FieldConfig.Unit));
            builder.Property<bool>(nameof(FieldConfig.IsSingleSelect)).HasColumnName(nameof(FieldConfig.IsSingleSelect));
            builder.Property<bool>(nameof(FieldConfig.Enable)).HasColumnName(nameof(FieldConfig.Enable));
            builder.Property<string>(nameof(FieldConfig.ExecuteMethod)).HasColumnName(nameof(FieldConfig.ExecuteMethod));
            builder.Property<string>(nameof(FieldConfig.ResultType)).HasColumnName(nameof(FieldConfig.ResultType));
            builder.Property<int>(nameof(FieldConfig.Sort)).HasColumnName(nameof(FieldConfig.Sort));
        }
    }
}
