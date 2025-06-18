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
    public class AgreementConfiguration : IEntityTypeConfiguration<Agreement>
    {
        public void Configure(EntityTypeBuilder<Agreement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Position)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.Commission)
                .IsRequired();
            builder.Property(x=>x.AgreementDate)
                .IsRequired();
            builder.HasOne(x => x.Worker)
                .WithMany(a => a.Agreements)
                .HasForeignKey(x => x.WorkerId);
            builder.HasOne(x=>x.Companie)
                .WithMany(a=>a.Aggreements)
                .HasForeignKey(x=>x.CompanieId);
        }
    }
}
