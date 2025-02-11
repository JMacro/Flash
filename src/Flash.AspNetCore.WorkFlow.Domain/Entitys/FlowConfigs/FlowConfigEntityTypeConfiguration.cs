using Flash.AspNetCore.WorkFlow.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowConfigs
{
    internal class FlowConfigEntityTypeConfiguration : IEntityTypeConfiguration<FlowConfig>
    {
        public void Configure(EntityTypeBuilder<FlowConfig> builder)
        {
            builder.ToTable("sys_workflow_config");
            builder.HasKey(x => x.Id);

            builder.Property<long>(nameof(FlowConfig.ParentId)).HasColumnName(nameof(FlowConfig.ParentId));
            builder.Property<string>(nameof(FlowConfig.Name)).HasColumnName(nameof(FlowConfig.Name));
            builder.Property<long>(nameof(FlowConfig.ObjectId)).HasColumnName(nameof(FlowConfig.ObjectId));
            builder.Property<EWorkFlowConfigType>(nameof(FlowConfig.Type)).HasColumnName(nameof(FlowConfig.Type));
            builder.Property<EWorkFlowConfigSubType>(nameof(FlowConfig.SubType)).HasColumnName(nameof(FlowConfig.SubType));
            builder.Property<short>(nameof(FlowConfig.ClassType)).HasColumnName(nameof(FlowConfig.ClassType));
            builder.Property<short>(nameof(FlowConfig.ClassSubType)).HasColumnName(nameof(FlowConfig.ClassSubType));
            builder.Property<string>(nameof(FlowConfig.Remark)).HasColumnName(nameof(FlowConfig.Remark));
        }
    }
}
