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
    public class CompanieConfiguration : IEntityTypeConfiguration<Companie>

    {
        public void Configure(EntityTypeBuilder<Companie> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(x=>x.EmailCompany)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.Address)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(50);
            builder.HasOne(x => x.ActivityType)
                .WithMany(a => a.Companies)
                .HasForeignKey(x => x.ActivityTypeId);

        }
    }
}
