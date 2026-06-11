using BookingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Infrastructure.Data
{
	public class BookingDbContext : DbContext
	{
		public BookingDbContext(DbContextOptions<BookingDbContext> options)
		: base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<Property> Properties { get; set; }
		public DbSet<Booking> Bookings { get; set; }
		public DbSet<Payment> Payments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			// 🎯 Global query filter - Soft delete uchun
			// Har bir query da avtomatik IsDeleted = false qo'shiladi

			modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
			modelBuilder.Entity<Property>().HasQueryFilter(u => !u.IsDeleted);
			modelBuilder.Entity<Booking>().HasQueryFilter(u => !u.IsDeleted);
			modelBuilder.Entity<Payment>().HasQueryFilter(u => !u.IsDeleted);

			modelBuilder.Entity<User>().HasIndex(u => u.PhoneNumber).IsUnique();
			modelBuilder.Entity<Property>().Property(p => p.PricePerNight).HasPrecision(18, 2);
			modelBuilder.Entity<Booking>().Property(b => b.TotalPrice).HasPrecision(18, 2);
			modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);

			// 🔗 Booking - User relationship
			modelBuilder.Entity<Booking>()
				.HasOne(b => b.Guest)
				.WithMany(u => u.Bookings)
				.HasForeignKey(b => b.GuestId)
				.OnDelete(DeleteBehavior.Restrict); // User o'chirilganda booking o'chmasin

			// 🔗 Booking - Property relationship
			modelBuilder.Entity<Booking>()
				.HasOne(b => b.Property)
				.WithMany(p => p.Bookings)
				.HasForeignKey(b => b.PropertyId)
				.OnDelete(DeleteBehavior.Restrict); // Property o'chirilganda booking o'chmasin

			// 🔗 Payment - Booking (1-to-1)
			modelBuilder.Entity<Payment>()
				.HasOne(p => p.Booking)
				.WithOne(b => b.Payment)
				.HasForeignKey<Payment>(p => p.BookingId)
				.OnDelete(DeleteBehavior.Cascade); // Booking o'chirilganda payment ham o'chadi
		}
	}
}
