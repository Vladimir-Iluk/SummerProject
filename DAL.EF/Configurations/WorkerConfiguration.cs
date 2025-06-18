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
    public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
    {
        public void Configure(EntityTypeBuilder<Worker> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.MiddleName)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.Qualification)
                .HasMaxLength(50);
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(x => x.ExpectedSalary)
                .IsRequired();
            builder.Property(x => x.OtherInfo)
                .IsRequired()
                .HasMaxLength(240);
            builder.HasOne(x => x.ActivityType)
                .WithMany(a => a.Workers)
                .HasForeignKey(x => x.ActivityTypeId);

        }
    }
}
