using Blog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Mappings;

public class PostMap : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Post");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();

        builder.Property(p => p.CreateDate)
            .IsRequired()
            .HasColumnType("SMALLDATETIME")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.LastUpdateDate)
            .IsRequired()
            .HasColumnType("SMALLDATETIME")
            .HasDefaultValue(DateTime.Now.ToUniversalTime());

        builder.Property(p => p.Slug)
           .IsRequired()
           .HasColumnType("VARCHAR")
           .HasMaxLength(80);

        builder.HasIndex(p => p.Slug, "IX_Post_Slug")
            .IsUnique();

        builder.HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasConstraintName("FK_Post_Author")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Category)
            .WithMany(u => u.Posts)
            .HasConstraintName("FK_Post_Category")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Tags)
            .WithMany(t => t.Posts)
            .UsingEntity<Dictionary<string, object>>(
                "PostTag",
                post => post.HasOne<Tag>()
                            .WithMany()
                            .HasForeignKey("PostId")
                            .HasConstraintName("FK_PostTag_PostId")
                            .OnDelete(DeleteBehavior.Cascade),
                tag => tag.HasOne<Post>()
                            .WithMany()
                            .HasForeignKey("TagId")
                            .HasConstraintName("FK_PostTag_TagId")
                            .OnDelete(DeleteBehavior.Cascade)
            );
    }
}
