using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace NetAdmin.Infrastructure.Data.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "10.0.13");

        modelBuilder.Entity("NetAdmin.Infrastructure.Data.HardwareDeviceEntity", b =>
        {
            b.Property<string>("Id")
                .HasColumnType("TEXT");

            b.Property<int>("Category")
                .HasColumnType("INTEGER");

            b.Property<string>("IpAddress")
                .HasColumnType("TEXT");

            b.Property<bool>("IsManaged")
                .HasColumnType("INTEGER");

            b.Property<string>("MacAddress")
                .HasColumnType("TEXT");

            b.Property<string>("ManagementUrl")
                .HasColumnType("TEXT");

            b.Property<string>("MeterInterface")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Name")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Notes")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<int>("PortCount")
                .HasColumnType("INTEGER");

            b.Property<string>("Room")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<int>("ShellyGeneration")
                .HasColumnType("INTEGER");

            b.Property<string>("StreamsJson")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("SubModel")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("SupportedProtocolsCsv")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("TagsCsv")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Vendor")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.ToTable("HardwareDevices");
        });

        modelBuilder.Entity("NetAdmin.Infrastructure.Data.PatchPanelPortEntity", b =>
        {
            b.Property<int>("PortNumber")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<string>("Label")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Room")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("TagsCsv")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("PortNumber");

            b.ToTable("PatchPanelPorts");
        });

        modelBuilder.Entity("NetAdmin.Infrastructure.Data.PatchPanelPortItemEntity", b =>
        {
            b.Property<string>("PatchPanelId")
                .HasColumnType("TEXT");

            b.Property<int>("PortNumber")
                .HasColumnType("INTEGER");

            b.Property<string>("Label")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Room")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("TagsCsv")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("PatchPanelId", "PortNumber");

            b.ToTable("PatchPanelPortItems");
        });

        modelBuilder.Entity("NetAdmin.Infrastructure.Data.PhysicalLinkEntity", b =>
        {
            b.Property<string>("Id")
                .HasColumnType("TEXT");

            b.Property<string>("CableLabel")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("SourceDeviceId")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("SourceDeviceName")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<int>("SourcePort")
                .HasColumnType("INTEGER");

            b.Property<string>("TagsCsv")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("TargetDeviceId")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("TargetDeviceName")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<int>("TargetPort")
                .HasColumnType("INTEGER");

            b.HasKey("Id");

            b.ToTable("PhysicalLinks");
        });

        modelBuilder.Entity("NetAdmin.Infrastructure.Data.RoomEntity", b =>
        {
            b.Property<string>("Id")
                .HasColumnType("TEXT");

            b.Property<string>("BuildingOrZone")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Floor")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Name")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("TagsCsv")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.ToTable("Rooms");
        });

        modelBuilder.Entity("NetAdmin.Infrastructure.Data.ServiceEntity", b =>
        {
            b.Property<string>("Id")
                .HasColumnType("TEXT");

            b.Property<string>("Description")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("LinkedDeviceId")
                .HasColumnType("TEXT");

            b.Property<string>("Name")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Type")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.ToTable("Services");
        });

        modelBuilder.Entity("NetAdmin.Infrastructure.Data.ShellyDeviceEntity", b =>
        {
            b.Property<string>("Id")
                .HasColumnType("TEXT");

            b.Property<string>("IpAddress")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Model")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Name")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Room")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("TagsCsv")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.ToTable("ShellyDevices");
        });

        modelBuilder.Entity("NetAdmin.Infrastructure.Data.SwitchPortEntity", b =>
        {
            b.Property<string>("DeviceId")
                .HasColumnType("TEXT");

            b.Property<int>("PortNumber")
                .HasColumnType("INTEGER");

            b.Property<string>("Label")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<int>("NativeVlanId")
                .HasColumnType("INTEGER");

            b.Property<bool>("PoeEnabled")
                .HasColumnType("INTEGER");

            b.Property<int>("Role")
                .HasColumnType("INTEGER");

            b.Property<string>("TaggedVlanIdsCsv")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("TagsCsv")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("DeviceId", "PortNumber");

            b.ToTable("SwitchPorts");
        });

        modelBuilder.Entity("NetAdmin.Infrastructure.Data.SystemSettingEntity", b =>
        {
            b.Property<string>("Key")
                .HasColumnType("TEXT");

            b.Property<string>("EncryptedValue")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("Key");

            b.ToTable("SystemSettings");
        });

        modelBuilder.Entity("NetAdmin.Infrastructure.Data.UnmanagedSwitchEntity", b =>
        {
            b.Property<string>("Id")
                .HasColumnType("TEXT");

            b.Property<string>("Location")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("Name")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<int>("PortCount")
                .HasColumnType("INTEGER");

            b.Property<string>("TagsCsv")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.ToTable("UnmanagedSwitches");
        });
#pragma warning restore 612, 618
    }
}
