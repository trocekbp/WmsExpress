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
    public class WmsCoreContext : DbContext
    {
        public WmsCoreContext(DbContextOptions<WmsCoreContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Item { get; set; } = default!;
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
            modelBuilder //kaskadowe usuwanie dzieci przy usunięciu rodzica, czyli atrybutów Itemu 
                .Entity<Item>()
                .HasMany(i => i.Attributes)
                .WithOne(f => f.Item)
                .HasForeignKey(f => f.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            //Relacja jeden do jednego Itemu oraz Encji zapasu w magazynie
            modelBuilder.Entity<Item>()
              .HasMany(i => i.InventoryMovements)
              .WithOne(ii => ii.Item)
              .HasForeignKey(m => m.ItemId)
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


