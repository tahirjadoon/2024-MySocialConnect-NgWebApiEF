﻿// <auto-generated />
using System;
using MSC.Core.DB.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MSC.WebApi.DbFile.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240327055927_ExtendedEntitiesWithColumnNumber")]
    partial class ExtendedEntitiesWithColumnNumber
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.2");

            modelBuilder.Entity("MSC.Core.DB.Entities.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(1);

                    b.Property<string>("City")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(12);

                    b.Property<string>("Country")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(13);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(15);

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(6);

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(7);

                    b.Property<string>("Gender")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(8);

                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnOrder(2);

                    b.Property<string>("Interests")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(11);

                    b.Property<string>("Introduction")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(9);

                    b.Property<DateTime>("LastActive")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(14);

                    b.Property<string>("LookingFor")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(10);

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("BLOB")
                        .HasColumnOrder(4);

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("BLOB")
                        .HasColumnOrder(5);

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(16);

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnOrder(3);

                    b.HasKey("Id");

                    b.HasIndex("Guid");

                    b.HasIndex("UserName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MSC.Core.DB.Entities.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(1);

                    b.Property<int>("AppUserId")
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(5);

                    b.Property<bool>("IsMain")
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(3);

                    b.Property<string>("PublicId")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(4);

                    b.Property<string>("Url")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(2);

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("MSC.Core.DB.Entities.Photo", b =>
                {
                    b.HasOne("MSC.Core.DB.Entities.AppUser", "AppUser")
                        .WithMany("Photos")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("MSC.Core.DB.Entities.AppUser", b =>
                {
                    b.Navigation("Photos");
                });
#pragma warning restore 612, 618
        }
    }
}
