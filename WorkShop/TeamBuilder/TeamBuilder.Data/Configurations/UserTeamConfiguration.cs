using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TeamBuilder.Models;

namespace TeamBuilder.Data.Configurations
{
    public class UserTeamConfiguration : IEntityTypeConfiguration<UserTeam>
    {
        public void Configure(EntityTypeBuilder<UserTeam> builder)
        {
            builder.ToTable("UserTeams");
            builder.HasKey(ut => new { ut.UserId, ut.TeamId });

            builder.HasOne(ut => ut.User)
                .WithMany(u => u.UserTeams)
                .HasForeignKey(ut => ut.UserId);

            builder.HasOne(ut => ut.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(ut => ut.TeamId);
        }


    }
}
