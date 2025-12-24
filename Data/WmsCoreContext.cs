using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WmsCore.Models;
using WmsCore.Models.Helpers;


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
        public DbSet<ItemInventory> ItemInventory { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder //kaskadowe usuwanie dzieci przy usunięciu rodzica, czyli atrybutów Itemu
                .Entity<Item>()
                .HasMany(i => i.Attributes)
                .WithOne(f => f.Item)
                .HasForeignKey(f => f.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

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
                .HasMaxLength(9)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save); //Wzięte z dokumentacji efcore, nie jest robione OUTPUT dla tego pola i będzie ono działało z trigerrami

            //Relacja jeden do jednego Itemu oraz Encji zapasu w magazynie
            modelBuilder.Entity<Item>()
              .HasOne(i => i.ItemInventory)
              .WithOne(ii => ii.Item)
              .HasForeignKey<ItemInventory>(ii => ii.ItemId)
            // usunięcie ItemInventory nie skasuje Item:
            .OnDelete(DeleteBehavior.Restrict);

            // dodanie wirtualnej encji modelu do bazy danych która umożliwi łatwe odczytanie wyniku funkcji
            modelBuilder.Entity<DocumentNumberResult>().HasNoKey(); 
        }
    }
}


