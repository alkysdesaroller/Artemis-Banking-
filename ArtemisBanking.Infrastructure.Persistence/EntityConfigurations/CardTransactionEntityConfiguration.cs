using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBanking.Infrastructure.Persistence.EntityConfigurations;

public class CardTransactionEntityConfiguration : IEntityTypeConfiguration<CardTransaction>
{

    public void Configure(EntityTypeBuilder<CardTransaction> builder)
    {
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18,4)");
        builder.Property(x => x.CommerceId).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasOne(x => x.Commerce).WithMany(c => c.CardTransactions);
    }
}