﻿using BakeryRecipe.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakeryRecipe.Data.Configurations
{
    public class RepostConfiguration: IEntityTypeConfiguration<Repost>
    {
        public void Configure(EntityTypeBuilder<Repost> builder)
        {
            builder.ToTable("Reposts");
            builder.HasKey(x => new { x.UserId, x.PostId,x.Date });
            builder
                .HasOne(x => x.User)
                .WithMany(x => x.Reposts)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(x => x.Post)
                .WithMany(x => x.Reposts)
                .HasForeignKey(x => x.PostId);
        }
    }
}
