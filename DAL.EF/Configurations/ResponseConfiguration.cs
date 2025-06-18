using DAL.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Configurations
{
    public class ResponseConfiguration : IEntityTypeConfiguration<Response>
    {
        public void Configure(EntityTypeBuilder<Response> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SentAt)
                .IsRequired();
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>();
            builder.HasOne(x => x.Worker)
                .WithMany(a => a.Responses)
                .HasForeignKey(x => x.WorkerId);
            builder.HasOne(x=>x.Vacancy)
                .WithMany(a => a.Responses)
                .HasForeignKey(x=>x.VacancyId);
        }
    }
}
