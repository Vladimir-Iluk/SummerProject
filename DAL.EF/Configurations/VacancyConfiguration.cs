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
    public class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
    {
        public void Configure(EntityTypeBuilder<Vacancy> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Position)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(x=>x.Description)
                .HasMaxLength(240)
                .IsRequired();
            builder.Property(x => x.Salary)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(x => x.CreatedAt)
                .IsRequired();
            builder.Property(x=>x.IsOpen)
                .IsRequired();
            builder.HasOne(x => x.Companie)
                .WithMany(a => a.Vacancies)
                .HasForeignKey(x => x.CompanieId);

        }
    }
}
