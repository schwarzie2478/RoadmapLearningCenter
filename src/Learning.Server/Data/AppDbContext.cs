using System.Linq;
using Learning.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Learning.Shared.Enums;
using System.Text.Json;

namespace Learning.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RoadmapTopic> RoadmapTopics => Set<RoadmapTopic>();
    public DbSet<TopicBlock> TopicBlocks => Set<TopicBlock>();
    public DbSet<ParagraphThread> ParagraphThreads => Set<ParagraphThread>();
    public DbSet<LearnerTopicState> LearnerTopicStates => Set<LearnerTopicState>();
    public DbSet<PlaygroundSession> PlaygroundSessions => Set<PlaygroundSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // RoadmapTopic self-referencing relationship
        modelBuilder.Entity<RoadmapTopic>(entity =>
        {
            entity.HasOne(t => t.Parent)
                  .WithMany(t => t.Children)
                  .HasForeignKey(t => t.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Prerequisites stored as JSON array
            entity.Property(t => t.Prerequisites)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                      v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null!) ?? new List<Guid>())
                  .Metadata
                  .SetValueComparer(
                      new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<ICollection<Guid>>(
                          (c1, c2) => c1.SequenceEqual(c2),
                          c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                          c => c.ToList()));
        });

        // TopicBlock -> RoadmapTopic
        modelBuilder.Entity<TopicBlock>(entity =>
        {
            entity.HasOne(b => b.Topic)
                  .WithMany(t => t.Blocks)
                  .HasForeignKey(b => b.TopicId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ParagraphThread -> TopicBlock
        modelBuilder.Entity<ParagraphThread>(entity =>
        {
            entity.HasOne(p => p.Block)
                  .WithMany(b => b.Paragraphs)
                  .HasForeignKey(p => p.BlockId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.ParentParagraph)
                  .WithMany(p => p.Replies)
                  .HasForeignKey(p => p.ParentParagraphId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // LearnerTopicState composite key
        modelBuilder.Entity<LearnerTopicState>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TopicId });

            entity.HasOne<RoadmapTopic>()
                  .WithMany()
                  .HasForeignKey(e => e.TopicId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // PlaygroundSession
        modelBuilder.Entity<PlaygroundSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.TopicId });
        });
    }
}
