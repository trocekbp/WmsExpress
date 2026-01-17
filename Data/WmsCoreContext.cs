using Azure.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WmsCore.Models;


namespace WmsCore.Data
{
    public class WmsCoreContext : IdentityDbContext<User>
    {
        public WmsCoreContext(DbContextOptions<WmsCoreContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Article { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Contractor> Contractor { get; set; } = default!;
        public DbSet<Address> Address { get; set; } = default!;
        public DbSet<AtrDefinition> AtrDefinition { get; set; } = default!;
        public DbSet<WmsCore.Models.Attribute> Attribute { get; set; } = default!;
        public DbSet<Models.Document> Document { get; set; } = default!;
        public DbSet<DocumentItem> DocumentItem { get; set; } = default!;
        public DbSet<InventoryMovement> InventoryMovement { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder //kaskadowe usuwanie dzieci przy usunięciu rodzica, czyli atrybutów Article 
                .Entity<Article>()
                .HasMany(i => i.Attributes)
                .WithOne(f => f.Article)
                .HasForeignKey(f => f.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            //Usnięcie definicji atrybutów przy usunięciu kategorii
            modelBuilder
                .Entity<AtrDefinition>()
                .HasOne(a => a.Category)
                .WithMany(c => c.AtrDefinitions)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            //Usunięcie wartości atrybutów przy usunięciu definicji
            modelBuilder
                .Entity<WmsCore.Models.Attribute>()
                .HasOne(a => a.AtrDefinition)
                .WithMany(ad => ad.Attributes)
                .HasForeignKey(a => a.AtrDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            //Blokada usunięcia kategorii gdy są w niej artykuły, najpierw trzba ustawić aktegorię artykułów na "inne"
            modelBuilder
                .Entity<Category>()
                .HasMany(c => c.Articles)
                .WithOne(a => a.Category)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            //Relacja jeden do jednego Article oraz Encji zapasu w magazynie
            modelBuilder.Entity<Article>()
              .HasMany(i => i.InventoryMovements)
              .WithOne(ii => ii.Article)
              .HasForeignKey(m => m.ArticleId)
            // usunięcie InventoryMovement nie skasuje Item:
            .OnDelete(DeleteBehavior.Restrict);

            //kaskadowe usuwanie adresów wraz z dostawcą 
            modelBuilder
                 .Entity<Contractor>()
                 .HasOne(s => s.Address)
                 .WithOne(a => a.Contractor)
                 .HasForeignKey<Address>(a => a.ContractorId)
                 .OnDelete(DeleteBehavior.Cascade);
            //kaskadowe usuwanie pozycji wraz z dokumentem

            modelBuilder
                 .Entity<WmsCore.Models.Document>()
                 .HasMany(d => d.DocumentItems)
                 .WithOne(i => i.Document)
                 .HasForeignKey(i => i.DocumentId)
                 .OnDelete(DeleteBehavior.Cascade);
            //Konfiguracja blokowanie usunięcia kontrahenta który jest powiązany z dokumentami
            //Konfiguracja od strony dziecka - dokument który posiada rodzica czyli kontrahenta
            modelBuilder
                .Entity<WmsCore.Models.Document>()
                .HasOne(d => d.Contractor)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ContractorId)
                .OnDelete(DeleteBehavior.Restrict);
            //konfiguracja pola Number
            modelBuilder.Entity<WmsCore.Models.Document>()
                .Property(d => d.Number)
                .HasMaxLength(12)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save); //Wzięte z dokumentacji efcore, nie jest robione OUTPUT dla tego pola i będzie ono działało z trigerrami
        }
    }
}


