using Microsoft.EntityFrameworkCore;

#nullable disable

namespace GChat.Models
{
    public class ChatContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public ChatContext(DbContextOptions<ChatContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Database=gchat;Port=5432;Username=goncharov;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>().HasKey(message => message.MessageId);
            modelBuilder.Entity<User>().HasKey(user => user.UserId);
            modelBuilder.Entity<Chat>().HasKey(chat => chat.ChatId);

            modelBuilder.Entity<Message>()
                .HasIndex(message => message.UserForeignKey)
                .IsUnique(false);

            modelBuilder.Entity<Message>()
                .HasIndex(message => message.ChatForeignKey)
                .IsUnique(false);

            modelBuilder.Entity<User>()
                .HasOne(user => user.Message)
                .WithOne(message => message.User)
                .HasForeignKey<Message>(message => message.UserForeignKey);

            modelBuilder.Entity<Chat>()
                .HasOne(chat => chat.Message)
                .WithOne(message => message.Chat)
                .HasForeignKey<Message>(message => message.ChatForeignKey);

            modelBuilder.Entity<User>()
                .HasOne(user => user.Chat)
                .WithMany(chat => chat.Members);
        }
    }
}