﻿// <auto-generated />
using System;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250122123400_textv3")]
    partial class textv3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backend.Entities.Abonnement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Beschrijving")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Max_medewerkers")
                        .HasColumnType("int");

                    b.Property<string>("Naam")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Prijs_multiplier")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Abonnement", (string)null);
                });

            modelBuilder.Entity("Backend.Entities.AbonnementAanvraag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<Guid>("BedrijfId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Beschrijving")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("MaxMedewerkers")
                        .HasColumnType("int");

                    b.Property<string>("Naam")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<double>("PrijsMultiplier")
                        .HasColumnType("float");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("In behandeling");

                    b.HasKey("Id");

                    b.HasIndex("BedrijfId");

                    b.ToTable("AbonnementAanvraag", (string)null);
                });

            modelBuilder.Entity("Backend.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Adres")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Naam")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rol")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TelefoonNummer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("wachtwoord")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("Backend.Entities.Bedrijf", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("AbonnementId")
                        .HasColumnType("int");

                    b.Property<string>("Domein")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Eigenaar")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("naam")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AbonnementId");

                    b.ToTable("Bedrijf");
                });

            modelBuilder.Entity("Backend.Entities.BedrijfAccounts", b =>
                {
                    b.Property<Guid>("account_id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("bedrijf_id")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("account_id", "bedrijf_id");

                    b.HasIndex("bedrijf_id");

                    b.ToTable("BedrijfAccounts");
                });

            modelBuilder.Entity("Backend.Entities.BedrijfWagenparkbeheerders", b =>
                {
                    b.Property<Guid>("account_id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("bedrijf_id")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("account_id", "bedrijf_id");

                    b.HasIndex("bedrijf_id");

                    b.ToTable("BedrijfWagenparkbeheerders");
                });

            modelBuilder.Entity("Backend.Entities.Email", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("BedrijfId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("account_id")
                        .HasColumnType("int");

                    b.Property<int>("bedrijf_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("BedrijfId");

                    b.ToTable("Emails");
                });

            modelBuilder.Entity("Backend.Entities.Schade", b =>
                {
                    b.Property<int>("SchadeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SchadeID"));

                    b.Property<int>("VoertuigID")
                        .HasColumnType("int");

                    b.Property<string>("schade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SchadeID");

                    b.HasIndex("VoertuigID");

                    b.ToTable("Schades");
                });

            modelBuilder.Entity("Backend.Entities.Text", b =>
                {
                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Type");

                    b.ToTable("Texts");
                });

            modelBuilder.Entity("Backend.Entities.VerhuurAanvraag", b =>
                {
                    b.Property<int>("AanvraagID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AanvraagID"));

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Bestemming")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Einddatum")
                        .HasColumnType("datetime2");

                    b.Property<int>("Kilometers")
                        .HasColumnType("int");

                    b.Property<DateTime>("Startdatum")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VoertuigID")
                        .HasColumnType("int");

                    b.HasKey("AanvraagID");

                    b.HasIndex("AccountId");

                    b.HasIndex("VoertuigID");

                    b.ToTable("VerhuurAanvraag", (string)null);
                });

            modelBuilder.Entity("Backend.Entities.Voertuig", b =>
                {
                    b.Property<int>("VoertuigID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VoertuigID"));

                    b.Property<string>("Aanschafjaar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Categorie")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Kenteken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Kleur")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Merk")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Prijs_per_dag")
                        .HasColumnType("float");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VoertuigID");

                    b.HasIndex("Categorie");

                    b.ToTable("Voertuig", (string)null);
                });

            modelBuilder.Entity("Backend.Entities.VoertuigCategorie", b =>
                {
                    b.Property<string>("Categorie")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Categorie");

                    b.ToTable("VoertuigCategorie");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Backend.Entities.AbonnementAanvraag", b =>
                {
                    b.HasOne("Backend.Entities.Bedrijf", "Bedrijf")
                        .WithMany("AbonnementAanvragen")
                        .HasForeignKey("BedrijfId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bedrijf");
                });

            modelBuilder.Entity("Backend.Entities.Bedrijf", b =>
                {
                    b.HasOne("Backend.Entities.Abonnement", "abonnement")
                        .WithMany("Bedrijven")
                        .HasForeignKey("AbonnementId");

                    b.Navigation("abonnement");
                });

            modelBuilder.Entity("Backend.Entities.BedrijfAccounts", b =>
                {
                    b.HasOne("Backend.Entities.Account", "Account")
                        .WithMany("BedrijfAccounts")
                        .HasForeignKey("account_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Entities.Bedrijf", "Bedrijf")
                        .WithMany("BedrijfAccounts")
                        .HasForeignKey("bedrijf_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Bedrijf");
                });

            modelBuilder.Entity("Backend.Entities.BedrijfWagenparkbeheerders", b =>
                {
                    b.HasOne("Backend.Entities.Account", "Account")
                        .WithMany("BedrijfWagenparkbeheerders")
                        .HasForeignKey("account_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Entities.Bedrijf", "Bedrijf")
                        .WithMany("BedrijfWagenparkbeheerders")
                        .HasForeignKey("bedrijf_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Bedrijf");
                });

            modelBuilder.Entity("Backend.Entities.Email", b =>
                {
                    b.HasOne("Backend.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("Backend.Entities.Bedrijf", "Bedrijf")
                        .WithMany()
                        .HasForeignKey("BedrijfId");

                    b.Navigation("Account");

                    b.Navigation("Bedrijf");
                });

            modelBuilder.Entity("Backend.Entities.Schade", b =>
                {
                    b.HasOne("Backend.Entities.Voertuig", "Voertuig")
                        .WithMany()
                        .HasForeignKey("VoertuigID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Voertuig");
                });

            modelBuilder.Entity("Backend.Entities.VerhuurAanvraag", b =>
                {
                    b.HasOne("Backend.Entities.Account", "Account")
                        .WithMany("VerhuurAanvragen")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Entities.Voertuig", "Voertuig")
                        .WithMany("VerhuurAanvragen")
                        .HasForeignKey("VoertuigID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Voertuig");
                });

            modelBuilder.Entity("Backend.Entities.Voertuig", b =>
                {
                    b.HasOne("Backend.Entities.VoertuigCategorie", "VoertuigCategorie")
                        .WithMany("Voertuigen")
                        .HasForeignKey("Categorie")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VoertuigCategorie");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backend.Entities.Abonnement", b =>
                {
                    b.Navigation("Bedrijven");
                });

            modelBuilder.Entity("Backend.Entities.Account", b =>
                {
                    b.Navigation("BedrijfAccounts");

                    b.Navigation("BedrijfWagenparkbeheerders");

                    b.Navigation("VerhuurAanvragen");
                });

            modelBuilder.Entity("Backend.Entities.Bedrijf", b =>
                {
                    b.Navigation("AbonnementAanvragen");

                    b.Navigation("BedrijfAccounts");

                    b.Navigation("BedrijfWagenparkbeheerders");
                });

            modelBuilder.Entity("Backend.Entities.Voertuig", b =>
                {
                    b.Navigation("VerhuurAanvragen");
                });

            modelBuilder.Entity("Backend.Entities.VoertuigCategorie", b =>
                {
                    b.Navigation("Voertuigen");
                });
#pragma warning restore 612, 618
        }
    }
}
