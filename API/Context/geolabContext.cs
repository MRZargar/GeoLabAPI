using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GeoLabAPI
{
    public partial class geolabContext : DbContext
    {
        public geolabContext()
        {
        }

        public geolabContext(DbContextOptions<geolabContext> options)
            : base(options)
        {
        }

        public virtual DbSet<StationData> Datas { get; set; }
        public virtual DbSet<StationsSetup> Stations { get; set; }
        public virtual DbSet<Raspberry> Raspberries { get; set; }
        public string tableName { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             modelBuilder.Entity<Raspberry>(entity =>
            {
                entity.ToTable("stations_raspberry");

                entity.HasIndex(e => e.RaspberryId)
                    .HasName("stations_raspberry_raspberryID_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RaspberryId).HasColumnName("raspberryID");
            });

            modelBuilder.Entity<StationData>(entity =>
            {
                entity.HasKey(e => new { e.WEEK, e.T })
                    .HasName(tableName + "_pkey");

                entity.ToTable(tableName);

                entity.HasIndex(e => e.T)
                    .HasName("index_" + tableName);

                entity.Property(e => e.WEEK).HasColumnName("week");

                entity.Property(e => e.T).HasColumnName("t");

                entity.Property(e => e.AX).HasColumnName("a_x");

                entity.Property(e => e.AY).HasColumnName("a_y");

                entity.Property(e => e.AZ).HasColumnName("a_z");

                entity.Property(e => e.Temp).HasColumnName("temp");
            });
            
            modelBuilder.Entity<StationsSetup>(entity =>
            {
                entity.ToTable("stations_setup");

                entity.HasIndex(e => e.OperatorId)
                    .HasName("stations_setup_operator_id_6e1c4ca0");

                entity.HasIndex(e => e.StationId)
                    .HasName("stations_setup_station_id_aaffad0a_like")
                    .HasOperators(new[] { "varchar_pattern_ops" });

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(300);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnName("city")
                    .HasMaxLength(150);

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Health).HasColumnName("health");

                entity.Property(e => e.HealthTime)
                    .HasColumnName("health_time")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasColumnType("numeric(20,10)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasColumnType("numeric(20,10)");

                entity.Property(e => e.OperatorId).HasColumnName("operator_id");

                entity.Property(e => e.Owner)
                    .IsRequired()
                    .HasColumnName("owner")
                    .HasMaxLength(200);

                entity.Property(e => e.RaspberryId).HasColumnName("raspberryID");

                entity.Property(e => e.SensorType)
                    .IsRequired()
                    .HasColumnName("sensor_type")
                    .HasMaxLength(50);

                entity.Property(e => e.StationId)
                    .IsRequired()
                    .HasColumnName("station_id")
                    .HasMaxLength(8);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TableName)
                    .HasColumnName("table_name")
                    .HasMaxLength(50);
            });

            modelBuilder.HasSequence("stations_setup_id_seq").HasMax(2147483647);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
