using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WmsCore.Models;


namespace WmsCore.Data
{
    public class WmsCoreContext : DbContext
    {
        public WmsCoreContext(DbContextOptions<WmsCoreContext> options)
            : base(options)
        {
        }

        public DbSet<Instrument> Instrument { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Supplier> Supplier { get; set; } = default!;
        public DbSet<Address> Address { get; set; } = default!;
        public DbSet<FeatureDefinition> FeatureDefinition { get; set; } = default!;
        public DbSet<InstrumentFeature> InstrumentFeature { get; set; } = default!;
        public DbSet<Models.Document> Document { get; set; } = default!;
        public DbSet<DocumentInstrument> DocumentInstrument { get; set; } = default!;
        public DbSet<InstrumentInventory> InstrumentInventory { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder //kaskadowe usuwanie dzieci przy usunięciu rodzica, czyli cech instrumentu
                .Entity<Instrument>()
                .HasMany(i => i.InstrumentFeatures)
                .WithOne(f => f.Instrument)
                .HasForeignKey(f => f.InstrumentId)
                .OnDelete(DeleteBehavior.Cascade);

            //kaskadowe usuwanie adresów wraz z dostawcą 
            modelBuilder
                 .Entity<Supplier>()
                 .HasOne(s => s.Address)
                 .WithOne(a => a.Supplier)
                 .HasForeignKey<Address>(a => a.SupplierId)
                 .OnDelete(DeleteBehavior.Cascade);

            //w przypadku usunięcia dostawcy, dla instrumentów ustawiamy dostawcę na NUll
            modelBuilder
                .Entity<Instrument>()
                .HasOne(i => i.Supplier)
                .WithMany(s => s.Instruments)
                .HasForeignKey(i => i.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);  //w przypadku usunięcia dostawcy, dla instrumentów ustawiamy dostawcę na NUll

            //Relacja jeden do jednego instrumentu oraz Encji zapasu w magazynie
            modelBuilder.Entity<Instrument>()
              .HasOne(i => i.Inventory)
              .WithOne(ii => ii.Instrument)
              .HasForeignKey<InstrumentInventory>(ii => ii.InstrumentId)
            // usunięcie InstrumentInventory nie skasuje Instrument:
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}


