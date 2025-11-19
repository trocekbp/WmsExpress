using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WmsCore.Models;
using System;
using System.Linq;

namespace WmsCore.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new WmsCoreContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<WmsCoreContext>>()))
            {
                // 1) Sprawdzamy czy jest już jakikolwiek instrument.
                if (context.Instrument.Any())
                {
                    return;   // DB has been seeded
                }

                // 2) Kategorie
                context.Category.AddRange(
                    new Category { Name = "Gitary" },    // Id = 1
                    new Category { Name = "Perkusje" },  // Id = 2
                    new Category { Name = "Pianina" },   // Id = 3
                    new Category { Name = "Skrzypce" },  // Id = 4
                    new Category { Name = "Dęte" },      // Id = 5
                    new Category { Name = "Inne" }       // Id = 6
                );
                context.SaveChanges();

                // 3) FeatureDefinitions
                context.FeatureDefinition.AddRange(
                    // Gitary (FType.Gitary) – 5 cech
                    new FeatureDefinition { Type = FType.Gitary, Name = "Ilość strun" },         // Id = 1
                    new FeatureDefinition { Type = FType.Gitary, Name = "Materiał korpusu" },    // Id = 2
                    new FeatureDefinition { Type = FType.Gitary, Name = "Przystawki" },          // Id = 3
                    new FeatureDefinition { Type = FType.Gitary, Name = "Podstrunnica" },        // Id = 4
                    new FeatureDefinition { Type = FType.Gitary, Name = "Kształt" },             // Id = 5

                    // Perkusje (FType.Perkusje) – 2 cechy
                    new FeatureDefinition { Type = FType.Perkusje, Name = "Typ naciągu" },       // Id = 6
                    new FeatureDefinition { Type = FType.Perkusje, Name = "Wielkość bębna" },     // Id = 7

                    // Pianina (FType.Pianina) – 2 cechy
                    new FeatureDefinition { Type = FType.Pianina, Name = "Wykończenie" },         // Id = 8
                    new FeatureDefinition { Type = FType.Pianina, Name = "Funkcja wyciszania" },  // Id = 9

                    // Skrzypce (FType.Skrzypce) – 3 cechy
                    new FeatureDefinition { Type = FType.Skrzypce, Name = "Materiał skrzypiec" }, // Id = 10
                    new FeatureDefinition { Type = FType.Skrzypce, Name = "Rozmiar" },            // Id = 11
                    new FeatureDefinition { Type = FType.Skrzypce, Name = "Typ smyczka" },        // Id = 12

                    // Dęte (FType.Dęte) – 3 cechy
                    new FeatureDefinition { Type = FType.Dęte, Name = "Materiał" },               // Id = 13
                    new FeatureDefinition { Type = FType.Dęte, Name = "Typ stroika" },           // Id = 14
                    new FeatureDefinition { Type = FType.Dęte, Name = "Liczba otworów" }          // Id = 15
                );
                context.SaveChanges();

                // 4) Dostawcy
                context.Supplier.AddRange(
                    new Supplier { Name = "MuzyczneABC", Email = "kontakt@muzyczneabc.pl" },       // tymczasowo Id nieznane
                    new Supplier { Name = "GitaraPro", Email = "sprzedaz@gitarapro.pl" },           // tymczasowo Id nieznane
                    new Supplier { Name = "SkrzypceStudio", Email = "office@skrzypcestudio.pl" },    // tymczasowo Id nieznane
                    new Supplier { Name = "DęteBrzmienie", Email = "kontakt@detebrzmienie.pl" }     // tymczasowo Id nieznane
                );
                context.SaveChanges();

                // Po SaveChanges() EF Core ustawi właściwe SupplierId dla każdej instancji.
                // Pobieramy dostawców z bazy, aby mieć pewne ID:
                var supMuzyczne = context.Supplier.Single(s => s.Name == "MuzyczneABC");
                var supGitara = context.Supplier.Single(s => s.Name == "GitaraPro");
                var supSkrzypce = context.Supplier.Single(s => s.Name == "SkrzypceStudio");
                var supDet = context.Supplier.Single(s => s.Name == "DęteBrzmienie");

                // 5) Adresy dostawców – odwołujemy się do rzeczywistych SupplierId:
                context.Address.AddRange(
                    new Address { Street = "ul. Muzyczna 1", City = "Warszawa", PostalCode = "00-001", SupplierId = supMuzyczne.SupplierId },
                    new Address { Street = "ul. Gitarska 5", City = "Kraków", PostalCode = "30-002", SupplierId = supGitara.SupplierId },
                    new Address { Street = "ul. Smyczkowa 12", City = "Gdańsk", PostalCode = "80-001", SupplierId = supSkrzypce.SupplierId },
                    new Address { Street = "ul. Dęta 8", City = "Wrocław", PostalCode = "50-005", SupplierId = supDet.SupplierId }
                );
                context.SaveChanges();
                context.SaveChanges();

                // 6) Instrumenty – w sumie 22 (2 wcześniej + 20 nowych)
                context.Instrument.AddRange(
                    // Istniejące 2
                    new Instrument
                    {
                        Name = "Fender Stratocaster",
                        Price = 3999.00m,
                        Description = "Legendarny gitarowy klasyk",
                        EAN = "0123456789012",
                        SKU = "FSTRAT2025",
                        
                        SupplierId = 1,
                        CategoryId = 1   // Gitary
                    },
                    new Instrument
                    {
                        Name = "Yamaha Stage Custom",
                        Price = 2599.00m,
                        Description = "Perkusja akustyczna 5-elementowa",
                        EAN = "0987654321098",
                        SKU = "YSTAGE2025",
                        
                        SupplierId = 2,
                        CategoryId = 2   // Perkusje
                    },

                    // 10 skrzypiec (CategoryId = 4)
                    new Instrument
                    {
                        Name = "Skrzypce Włoskie Model 1",
                        Price = 1500.00m,
                        Description = "Skrzypce z włoską duszą",
                        EAN = "4000000000001",
                        SKU = "SKRYP1",
                        SupplierId = 3,
                        CategoryId = 4
                    },
                    new Instrument
                    {
                        Name = "Skrzypce Polskie Model 2",
                        Price = 1200.00m,
                        Description = "Ręcznie robione polskie skrzypce",
                        EAN = "4000000000002",
                        SKU = "SKRYP2",
                        SupplierId = 3,
                        CategoryId = 4
                    },
                    new Instrument
                    {
                        Name = "Skrzypce Koncertowe Model 3",
                        Price = 5000.00m,
                        Description = "Skrzypce przeznaczone na scenę",
                        EAN = "4000000000003",
                        SKU = "SKRYP3",
                        SupplierId = 3,
                        CategoryId = 4
                    },
                    new Instrument
                    {
                        Name = "Skrzypce Studyjne Model 4",
                        Price = 800.00m,
                        Description = "Idealne na naukę i zajęcia w szkole",
                        EAN = "4000000000004",
                        SKU = "SKRYP4",
                        SupplierId = 3,
                        CategoryId = 4
                    },
                    new Instrument
                    {
                        Name = "Skrzypce Barokowe Model 5",
                        Price = 4200.00m,
                        Description = "Replika instrumentu barokowego",
                        EAN = "4000000000005",
                        SKU = "SKRYP5",
                        SupplierId = 3,
                        CategoryId = 4
                    },
                    new Instrument
                    {
                        Name = "Skrzypce Elektryczne Model 6",
                        Price = 2500.00m,
                        Description = "Nowoczesne skrzypce elektryczne",
                        EAN = "4000000000006",
                        SKU = "SKRYP6",
                        SupplierId = 3,
                        CategoryId = 4
                    },
                    new Instrument
                    {
                        Name = "Skrzypce 1/2 Model 7",
                        Price = 900.00m,
                        Description = "Skrzypce w rozmiarze 1/2 dla dzieci",
                        EAN = "4000000000007",
                        SKU = "SKRYP7",
                        SupplierId = 3,
                        CategoryId = 4
                    },
                    new Instrument
                    {
                        Name = "Skrzypce 3/4 Model 8",
                        Price = 1000.00m,
                        Description = "Skrzypce w rozmiarze 3/4 dla młodzieży",
                        EAN = "4000000000008",
                        SKU = "SKRYP8",
                        SupplierId = 3,
                        CategoryId = 4
                    },
                    new Instrument
                    {
                        Name = "Skrzypce Amerykańskie Model 9",
                        Price = 1800.00m,
                        Description = "Skrzypce wyprodukowane w USA",
                        EAN = "4000000000009",
                        SKU = "SKRYP9",
                        SupplierId = 3,
                        CategoryId = 4
                    },
                    new Instrument
                    {
                        Name = "Skrzypce Francuskie Model 10",
                        Price = 1600.00m,
                        Description = "Skrzypce z francuskim brzmieniem",
                        EAN = "4000000000010",
                        SKU = "SKRYP10",
                        SupplierId = 3,
                        CategoryId = 4
                    },

                    // 10 instrumentów dętych (CategoryId = 5)
                    new Instrument
                    {
                        Name = "Flet Poprzeczny Model 1",
                        Price = 2200.00m,
                        Description = "Klasyczny flet poprzeczny",
                        EAN = "5000000000001",
                        SKU = "FLET1",
                        SupplierId = 4,
                        CategoryId = 5
                    },
                    new Instrument
                    {
                        Name = "Saksofon Alt Model 2",
                        Price = 5500.00m,
                        Description = "Profesjonalny saksofon altowy",
                        EAN = "5000000000002",
                        SKU = "SAKS2",
                        SupplierId = 4,
                        CategoryId = 5
                    },
                    new Instrument
                    {
                        Name = "Klarnet Bb Model 3",
                        Price = 1800.00m,
                        Description = "Klarnet Bb do jazzu i klasyki",
                        EAN = "5000000000003",
                        SKU = "KLAR3",
                        SupplierId = 4,
                        CategoryId = 5
                    },
                    new Instrument
                    {
                        Name = "Trąbka Bb Model 4",
                        Price = 2400.00m,
                        Description = "Trąbka Bb z niklowanym wykończeniem",
                        EAN = "5000000000004",
                        SKU = "TRAB4",
                        SupplierId = 4,
                        CategoryId = 5
                    },
                    new Instrument
                    {
                        Name = "Puzon Model 5",
                        Price = 2600.00m,
                        Description = "Puzon tenorowy wykonany z mosiądzu",
                        EAN = "5000000000005",
                        SKU = "PUZN5",
                        SupplierId = 4,
                        CategoryId = 5
                    },
                    new Instrument
                    {
                        Name = "Flet Piccolo Model 6",
                        Price = 2000.00m,
                        Description = "Flet piccolo dla zaawansowanych",
                        EAN = "5000000000006",
                        SKU = "PICCO6",
                        SupplierId = 4,
                        CategoryId = 5
                    },
                    new Instrument
                    {
                        Name = "Obój Model 7",
                        Price = 3000.00m,
                        Description = "Obój z drewna grenadillowego",
                        EAN = "5000000000007",
                        SKU = "OBOJ7",
                        SupplierId = 4,
                        CategoryId = 5
                    },
                    new Instrument
                    {
                        Name = "Fagot Model 8",
                        Price = 4500.00m,
                        Description = "Fagot do orkiestry symfonicznej",
                        EAN = "5000000000008",
                        SKU = "FAGOT8",
                        SupplierId = 4,
                        CategoryId = 5
                    },
                    new Instrument
                    {
                        Name = "Trąbka Klasyczna Model 9",
                        Price = 2300.00m,
                        Description = "Trąbka kluczowa do muzyki klasycznej",
                        EAN = "5000000000009",
                        SKU = "TRAK9",
                        SupplierId = 4,
                        CategoryId = 5
                    },
                    new Instrument
                    {
                        Name = "Sakshorn Model 10",
                        Price = 5200.00m,
                        Description = "Sakshorn z posrebrzanym wykończeniem",
                        EAN = "5000000000010",
                        SKU = "SAKH10",
                        SupplierId = 4,
                        CategoryId = 5
                    }
                );
                context.SaveChanges();

                // 7) InstrumentFeatures – dodajemy odpowiednie cechy dla każdego instrumentu
                // Pobieramy wszystkie definicje cech z bazy:
                var allDefs = context.FeatureDefinition.ToList();

                // Dla każdego instrumentu:
                foreach (var instr in context.Instrument)
                {
                    // Znajdujemy kategorię:
                    var cat = context.Category.Find(instr.CategoryId);
                    if (cat == null) continue;

                    if (cat.Name.Equals("Gitary", StringComparison.OrdinalIgnoreCase))
                    {
                        // Cechy o Id 1–5 (Gitary)
                        foreach (var defId in new[] { 1, 2, 3, 4, 5 })
                        {
                            var def = allDefs.First(d => d.FeatureDefinitionId == defId);
                            string val = def.Name switch
                            {
                                "Ilość strun" => "6",
                                "Materiał korpusu" => "Olcha",
                                "Przystawki" => "Humbucker",
                                "Podstrunnica" => "Jesion",
                                "Kształt" => "Stratocaster",
                                _ => "–"
                            };
                            context.InstrumentFeature.Add(new InstrumentFeature
                            {
                                InstrumentId = instr.InstrumentId,
                                FeatureDefinitionId = def.FeatureDefinitionId,
                                Value = val
                            });
                        }
                    }
                    else if (cat.Name.Equals("Perkusje", StringComparison.OrdinalIgnoreCase))
                    {
                        // Cechy o Id 6–7 (Perkusje)
                        foreach (var defId in new[] { 6, 7 })
                        {
                            var def = allDefs.First(d => d.FeatureDefinitionId == defId);
                            string val = def.Name switch
                            {
                                "Typ naciągu" => "Akustyczny",
                                "Wielkość bębna" => "14\"",
                                _ => "–"
                            };
                            context.InstrumentFeature.Add(new InstrumentFeature
                            {
                                InstrumentId = instr.InstrumentId,
                                FeatureDefinitionId = def.FeatureDefinitionId,
                                Value = val
                            });
                        }
                    }
                    else if (cat.Name.Equals("Pianina", StringComparison.OrdinalIgnoreCase))
                    {
                        // Cechy o Id 8–9 (Pianina)
                        foreach (var defId in new[] { 8, 9 })
                        {
                            var def = allDefs.First(d => d.FeatureDefinitionId == defId);
                            string val = def.Name switch
                            {
                                "Wykończenie" => "Lakier",
                                "Funkcja wyciszania" => "Tak",
                                _ => "–"
                            };
                            context.InstrumentFeature.Add(new InstrumentFeature
                            {
                                InstrumentId = instr.InstrumentId,
                                FeatureDefinitionId = def.FeatureDefinitionId,
                                Value = val
                            });
                        }
                    }
                    else if (cat.Name.Equals("Skrzypce", StringComparison.OrdinalIgnoreCase))
                    {
                        // Cechy o Id 10–12 (Skrzypce)
                        foreach (var defId in new[] { 10, 11, 12 })
                        {
                            var def = allDefs.First(d => d.FeatureDefinitionId == defId);
                            string val = def.Name switch
                            {
                                "Materiał skrzypiec" => "Jawor",
                                "Rozmiar" => "4/4",
                                "Typ smyczka" => "Kolagenowy",
                                _ => "–"
                            };
                            context.InstrumentFeature.Add(new InstrumentFeature
                            {
                                InstrumentId = instr.InstrumentId,
                                FeatureDefinitionId = def.FeatureDefinitionId,
                                Value = val
                            });
                        }
                    }
                    else if (cat.Name.Equals("Dęte", StringComparison.OrdinalIgnoreCase))
                    {
                        // Cechy o Id 13–15 (Dęte)
                        foreach (var defId in new[] { 13, 14, 15 })
                        {
                            var def = allDefs.First(d => d.FeatureDefinitionId == defId);
                            string val = def.Name switch
                            {
                                "Materiał" => "Mosiądz",
                                "Typ stroika" => "Bb Standard",
                                "Liczba otworów" => "17",
                                _ => "–"
                            };
                            context.InstrumentFeature.Add(new InstrumentFeature
                            {
                                InstrumentId = instr.InstrumentId,
                                FeatureDefinitionId = def.FeatureDefinitionId,
                                Value = val
                            });
                        }
                    }
                    // Kategoria "Inne" – pomijamy (brak cech)
                    context.SaveChanges();
                }

                    // 8) InstrumentInventory – inicjalizacja stanu magazynowego 
                    var firstTen = context.Instrument
                        .OrderBy(i => i.InstrumentId)
                        .Take(10)
                        .ToList();

                    foreach (var item in firstTen)
                    {
                        context.InstrumentInventory.Add(new InstrumentInventory
                        {
                            InstrumentId = item.InstrumentId,
                            Quantity = 10,                
                            LastUpdated = DateTime.UtcNow
                        });
                    }
                

                context.SaveChanges();
            }
        }
    }
}
