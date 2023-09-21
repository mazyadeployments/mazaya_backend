﻿// <auto-generated />
using System;
using MMA.WebApi.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MMA.WebApi.DataAccess.Migrations
{
    [DbContext(typeof(MMADbContext))]
    [Migration("20200529110811_AddOffers")]
    partial class AddOffers
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");                   
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description")
                        .HasMaxLength(10000);

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(1000);

                    b.Property<string>("ImageUrlCrop")
                        .HasMaxLength(1000);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.Collection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(1000);

                    b.Property<string>("ImageUrlCrop")
                        .HasMaxLength(1000);

                    b.Property<string>("Location")
                        .HasMaxLength(512);

                    b.Property<string>("SponsorLogoImageUrl")
                        .HasMaxLength(1000);

                    b.Property<string>("SponsorLogoImageUrlCrop")
                        .HasMaxLength(1000);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("UpdatedOn");

                    b.Property<DateTime?>("ValidUntil");

                    b.HasKey("Id");

                    b.ToTable("Collection");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Content");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("MimeType");

                    b.Property<string>("Name");

                    b.Property<Guid?>("ParentId");

                    b.Property<long?>("Size");

                    b.Property<string>("StoragePath");

                    b.Property<string>("StorageType");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Document");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.EmailTemplate", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Body");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Message");

                    b.Property<string>("Name");

                    b.Property<string>("Notification");

                    b.Property<int>("NotificationTypeId");

                    b.Property<string>("Sms");

                    b.Property<string>("Subject");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("EmailTemplate");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.EmailTemplateRoot", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("MailApplicationLogin");

                    b.Property<string>("MailBodyFooter");

                    b.Property<string>("MailTemplate");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("EmailTemplateRoot");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.Help", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<string>("HelpText");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("Help");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.MailStorage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ActionId");

                    b.Property<int?>("AgendaItemId");

                    b.Property<string>("Body");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int?>("MeetingId");

                    b.Property<int>("StatusId");

                    b.Property<string>("StatusNote");

                    b.Property<DateTime>("StatusOn");

                    b.Property<string>("Subject");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.Property<string>("UserEmail");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("MailStorage");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.NotificationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("NotificationType");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.Offer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AboutCompany")
                        .HasMaxLength(10000);

                    b.Property<string>("Address")
                        .HasMaxLength(500);

                    b.Property<int>("CategoryId");

                    b.Property<string>("City")
                        .HasMaxLength(100);

                    b.Property<int?>("CollectionId");

                    b.Property<string>("Country")
                        .HasMaxLength(100);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description")
                        .HasMaxLength(10000);

                    b.Property<decimal?>("DiscountPercentage")
                        .HasColumnType("decimal(28, 12)");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(28, 12)");

                    b.Property<decimal>("Longtitude")
                        .HasColumnType("decimal(28, 12)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(28, 12)");

                    b.Property<string>("PriceList")
                        .HasMaxLength(10000);

                    b.Property<string>("PromotionCode")
                        .HasMaxLength(100);

                    b.Property<string>("Tag")
                        .HasMaxLength(10000);

                    b.Property<string>("TermsAndCondition")
                        .HasMaxLength(10000);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("UpdatedOn");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidUntil");

                    b.Property<string>("WhatYouGet")
                        .HasMaxLength(10000);

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CollectionId");

                    b.ToTable("Offer");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.Permission", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Id");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Refreshtoken");

                    b.Property<bool>("Revoked");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("RefreshToken");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.RolePermission", b =>
                {
                    b.Property<string>("RoleId");

                    b.Property<string>("PermissionId");

                    b.Property<string>("Condition");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.HasIndex("RoleId", "PermissionId")
                        .IsUnique();

                    b.ToTable("RolePermission");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountDataId");

                    b.Property<bool>("Active");

                    b.Property<string>("AlternativeEmail");

                    b.Property<int>("CompanyId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Designation");

                    b.Property<string>("FirstName");

                    b.Property<string>("FullName");

                    b.Property<string>("Initials");

                    b.Property<string>("LastName");

                    b.Property<string>("MobilePhone");

                    b.Property<string>("PhotoBase64");

                    b.Property<string>("Title");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.Property<string>("WorkPhone");

                    b.HasKey("Id");

                    b.HasIndex("AccountDataId");

                    b.HasIndex("CompanyId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.UserNotification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Acknowledged");

                    b.Property<DateTime?>("AcknowledgedOn");

                    b.Property<int?>("ActionId");

                    b.Property<int?>("AgendaItemId");

                    b.Property<int?>("CommentId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int?>("MeetingId");

                    b.Property<string>("Message");

                    b.Property<int>("NotificationTypeId");

                    b.Property<string>("URL");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("NotificationTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("UserNotification");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.UserProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<int?>("CompanyId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("RoleId");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserProfile");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.UserProfileUnit", b =>
                {
                    b.Property<int>("UserProfileId");

                    b.Property<int>("UnitId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("UserProfileId", "UnitId");

                    b.HasIndex("UserProfileId", "UnitId")
                        .IsUnique();

                    b.ToTable("UserProfileUnit");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");

                    b.HasData(
                        new
                        {
                            UserId = "B6E82FB2-CAC5-43E4-BCF4-4156AA829D78",
                            RoleId = "5F283CE4-9020-41A8-84D1-8F10492C5A0F"
                        },
                        new
                        {
                            UserId = "58624BF9-4931-451B-89D4-D8E3F1E6BA59",
                            RoleId = "C9EED9D2-4DFD-4B9F-B424-BA80E021889E"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.Document", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.Document", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.MailStorage", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationUser", "AspNetUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.Offer", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MMA.WebApi.DataAccess.Models.Collection", "Collection")
                        .WithMany()
                        .HasForeignKey("CollectionId");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.RolePermission", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationRole", "AspNetRole")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.User", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationUser", "AccountData")
                        .WithMany()
                        .HasForeignKey("AccountDataId");

                    b.HasOne("MMA.WebApi.DataAccess.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.UserNotification", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.NotificationType", "NotificationType")
                        .WithMany()
                        .HasForeignKey("NotificationTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationUser", "AspNetUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.UserProfile", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationRole", "AspNetRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("MMA.WebApi.DataAccess.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MMA.WebApi.DataAccess.Models.UserProfileUnit", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.UserProfile", "UserProfile")
                        .WithMany("UserProfileUnits")
                        .HasForeignKey("UserProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("MMA.WebApi.DataAccess.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
