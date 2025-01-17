﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using iPractice.DataAccess;

#nullable disable

namespace iPractice.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240128191202_AddAppointments")]
    partial class AddAppointments
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("ClientPsychologist", b =>
                {
                    b.Property<long>("ClientsId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("PsychologistsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ClientsId", "PsychologistsId");

                    b.HasIndex("PsychologistsId");

                    b.ToTable("ClientPsychologist");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Appointment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("ClientId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("End")
                        .HasColumnType("TEXT");

                    b.Property<long>("PsychologistId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Start")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("PsychologistId");

                    b.ToTable("Appointment");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Availability", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("End")
                        .HasColumnType("TEXT");

                    b.Property<long>("PsychologistId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Start")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PsychologistId");

                    b.ToTable("Availability");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Client", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Psychologist", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Psychologists");
                });

            modelBuilder.Entity("ClientPsychologist", b =>
                {
                    b.HasOne("iPractice.DataAccess.Models.Client", null)
                        .WithMany()
                        .HasForeignKey("ClientsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("iPractice.DataAccess.Models.Psychologist", null)
                        .WithMany()
                        .HasForeignKey("PsychologistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Appointment", b =>
                {
                    b.HasOne("iPractice.DataAccess.Models.Client", "Client")
                        .WithMany("Appointments")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("iPractice.DataAccess.Models.Psychologist", "Psychologist")
                        .WithMany("Appointments")
                        .HasForeignKey("PsychologistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Psychologist");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Availability", b =>
                {
                    b.HasOne("iPractice.DataAccess.Models.Psychologist", "Psychologist")
                        .WithMany("Availability")
                        .HasForeignKey("PsychologistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Psychologist");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Client", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Psychologist", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("Availability");
                });
#pragma warning restore 612, 618
        }
    }
}
