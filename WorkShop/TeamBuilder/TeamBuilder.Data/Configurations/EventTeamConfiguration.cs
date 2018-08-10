using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TeamBuilder.Models;

namespace TeamBuilder.Data.Configurations
{
    public class EventTeamConfiguration : IEntityTypeConfiguration<EventTeam>
    {
        public void Configure(EntityTypeBuilder<EventTeam> builder)
        {
            builder.ToTable("EventTeams");
            builder.HasKey(et => new { et.EventID, et.TeamId });

            builder.HasOne(et => et.Event)
                .WithMany(e => e.ParticipatingEventTeams)
                .HasForeignKey(et => et.EventID);

            builder.HasOne(et => et.Team)
                .WithMany(t => t.EventsParticipated)
                .HasForeignKey(et => et.TeamId);
        }

       
    }
}
