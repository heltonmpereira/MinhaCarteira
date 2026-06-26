using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Modelo.Maps.Base;

public class BaseMap<T, TPK> : IEntityTypeConfiguration<T>
    where T : class, IEntidade<TPK>
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(typeof(T).Name);
        builder.HasKey(k => k.Id);
    }
}