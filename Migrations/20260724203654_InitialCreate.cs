using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EDG_B2B.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Telefon = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Rol = table.Column<int>(type: "integer", nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    OnaylanmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OnaylayanAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    OnayNotu = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EmailDogrulandiMi = table.Column<bool>(type: "boolean", nullable: false),
                    EmailDogrulamaTokeni = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EmailDogrulanmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Kullanicilar_OnaylayanAdminId",
                        column: x => x.OnaylayanAdminId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    StokKodu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StandartFiyat = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StokMiktari = table.Column<int>(type: "integer", nullable: false),
                    KdvOrani = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciOturumlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Jti = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    KullaniciId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciOturumlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciOturumlari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SatisBayiler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KullaniciId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatisBayiler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SatisBayiler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bayiler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KullaniciId = table.Column<Guid>(type: "uuid", nullable: false),
                    SatisBayiiId = table.Column<Guid>(type: "uuid", nullable: true),
                    Unvan = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    VergiNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Adres = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bayiler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bayiler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bayiler_SatisBayiler_SatisBayiiId",
                        column: x => x.SatisBayiiId,
                        principalTable: "SatisBayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BayiUrunFiyatlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false),
                    UrunId = table.Column<Guid>(type: "uuid", nullable: false),
                    OzelFiyat = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    GecerlilikBaslangic = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GecerlilikBitis = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BayiUrunFiyatlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BayiUrunFiyatlari_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BayiUrunFiyatlari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sepetler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sepetler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sepetler_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SepetUrunleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SepetId = table.Column<Guid>(type: "uuid", nullable: false),
                    UrunId = table.Column<Guid>(type: "uuid", nullable: false),
                    Miktar = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SepetUrunleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SepetUrunleri_Sepetler_SepetId",
                        column: x => x.SepetId,
                        principalTable: "Sepetler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SepetUrunleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Siparisler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SepetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    OnaylayanSatisBayiiId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siparisler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Siparisler_SatisBayiler_OnaylayanSatisBayiiId",
                        column: x => x.OnaylayanSatisBayiiId,
                        principalTable: "SatisBayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Siparisler_Sepetler_SepetId",
                        column: x => x.SepetId,
                        principalTable: "Sepetler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Faturalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SiparisId = table.Column<Guid>(type: "uuid", nullable: false),
                    FaturaNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Tutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    KdvTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faturalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faturalar_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiparisUrunleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SiparisId = table.Column<Guid>(type: "uuid", nullable: false),
                    UrunId = table.Column<Guid>(type: "uuid", nullable: false),
                    Miktar = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SatirToplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisUrunleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisUrunleri_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiparisUrunleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bayiler_KullaniciId",
                table: "Bayiler",
                column: "KullaniciId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bayiler_SatisBayiiId",
                table: "Bayiler",
                column: "SatisBayiiId");

            migrationBuilder.CreateIndex(
                name: "IX_Bayiler_VergiNo",
                table: "Bayiler",
                column: "VergiNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BayiUrunFiyatlari_BayiId_UrunId",
                table: "BayiUrunFiyatlari",
                columns: new[] { "BayiId", "UrunId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BayiUrunFiyatlari_UrunId",
                table: "BayiUrunFiyatlari",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_FaturaNo",
                table: "Faturalar",
                column: "FaturaNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_SiparisId",
                table: "Faturalar",
                column: "SiparisId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_Email",
                table: "Kullanicilar",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_EmailDogrulamaTokeni",
                table: "Kullanicilar",
                column: "EmailDogrulamaTokeni");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_OnaylayanAdminId",
                table: "Kullanicilar",
                column: "OnaylayanAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciOturumlari_Jti",
                table: "KullaniciOturumlari",
                column: "Jti",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciOturumlari_KullaniciId_IsRevoked",
                table: "KullaniciOturumlari",
                columns: new[] { "KullaniciId", "IsRevoked" });

            migrationBuilder.CreateIndex(
                name: "IX_SatisBayiler_KullaniciId",
                table: "SatisBayiler",
                column: "KullaniciId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sepetler_BayiId",
                table: "Sepetler",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_SepetUrunleri_SepetId",
                table: "SepetUrunleri",
                column: "SepetId");

            migrationBuilder.CreateIndex(
                name: "IX_SepetUrunleri_UrunId",
                table: "SepetUrunleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparisler_OnaylayanSatisBayiiId",
                table: "Siparisler",
                column: "OnaylayanSatisBayiiId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparisler_SepetId",
                table: "Siparisler",
                column: "SepetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiparisUrunleri_SiparisId",
                table: "SiparisUrunleri",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisUrunleri_UrunId",
                table: "SiparisUrunleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_StokKodu",
                table: "Urunler",
                column: "StokKodu",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BayiUrunFiyatlari");

            migrationBuilder.DropTable(
                name: "Faturalar");

            migrationBuilder.DropTable(
                name: "KullaniciOturumlari");

            migrationBuilder.DropTable(
                name: "SepetUrunleri");

            migrationBuilder.DropTable(
                name: "SiparisUrunleri");

            migrationBuilder.DropTable(
                name: "Siparisler");

            migrationBuilder.DropTable(
                name: "Urunler");

            migrationBuilder.DropTable(
                name: "Sepetler");

            migrationBuilder.DropTable(
                name: "Bayiler");

            migrationBuilder.DropTable(
                name: "SatisBayiler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}
