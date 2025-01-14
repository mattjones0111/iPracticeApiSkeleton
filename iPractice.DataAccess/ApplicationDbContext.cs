﻿using iPractice.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace iPractice.DataAccess;

public class ApplicationDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Psychologist> Psychologists { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Availability>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Client>()
            .HasKey(client => client.Id);

        modelBuilder.Entity<Client>()
            .HasMany(p => p.Psychologists)
            .WithMany(b => b.Clients);

        modelBuilder.Entity<Client>()
            .HasMany(x => x.Appointments)
            .WithOne(x => x.Client);

        modelBuilder.Entity<Psychologist>()
            .HasKey(psychologist => psychologist.Id);

        modelBuilder.Entity<Psychologist>()
            .HasMany(x => x.Availability)
            .WithOne(x => x.Psychologist);

        modelBuilder.Entity<Psychologist>()
            .HasMany(p => p.Clients)
            .WithMany(b => b.Psychologists);

        modelBuilder.Entity<Psychologist>()
            .HasMany(x => x.Appointments)
            .WithOne(x => x.Psychologist);
    }
}