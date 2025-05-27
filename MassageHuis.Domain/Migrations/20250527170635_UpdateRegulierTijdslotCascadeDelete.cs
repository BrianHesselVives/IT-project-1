using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MassageHuis.Domains.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRegulierTijdslotCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Voornaam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GeboorteDatum = table.Column<DateOnly>(type: "date", nullable: true),
                    Geslacht = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeMassage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Actief = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TypeMass__3214EC07EC691C22", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Masseur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Actief = table.Column<bool>(type: "bit", nullable: false),
                    Einddienstverband = table.Column<DateOnly>(type: "date", nullable: true),
                    Beschrijving = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Id_AspNetUsers = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Masseur__3214EC07D33499EF", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Masseur_AspNetUsers",
                        column: x => x.Id_AspNetUsers,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromotieCode",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Vervaldatum = table.Column<DateOnly>(type: "date", nullable: false),
                    Code = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Bedrag = table.Column<float>(type: "real", nullable: false),
                    Id_AspNetUsers = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Status = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    ResterendWaarde = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Promotie__3214EC074E2CCBFC", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotieCode_AspNetUsers",
                        column: x => x.Id_AspNetUsers,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KostPrijs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prijs = table.Column<float>(type: "real", nullable: false),
                    Startdatum = table.Column<DateOnly>(type: "date", nullable: false),
                    Id_TypeMassage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Prijs__3214EC07411AC5FA", x => x.Id);
                    table.ForeignKey(
                        name: "FKPrijs356693",
                        column: x => x.Id_TypeMassage,
                        principalTable: "TypeMassage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Masseur_TypeMassage",
                columns: table => new
                {
                    Id_Masseur = table.Column<int>(type: "int", nullable: false),
                    Id_TypeMassage = table.Column<int>(type: "int", nullable: false),
                    Column = table.Column<int>(type: "int", nullable: true),
                    Actief = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Masseur___80E85080449674C6", x => new { x.Id_Masseur, x.Id_TypeMassage });
                    table.ForeignKey(
                        name: "FKMasseur_Ty373364",
                        column: x => x.Id_Masseur,
                        principalTable: "Masseur",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKMasseur_Ty870180",
                        column: x => x.Id_TypeMassage,
                        principalTable: "TypeMassage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Naam = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    StartDatum = table.Column<DateOnly>(type: "date", nullable: false),
                    EindDatum = table.Column<DateOnly>(type: "date", nullable: true),
                    Id_Masseur = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schema__3214EC07A0AD3C59", x => x.Id);
                    table.ForeignKey(
                        name: "FKSchema133092",
                        column: x => x.Id_Masseur,
                        principalTable: "Masseur",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reservatie_PromotieCode",
                columns: table => new
                {
                    Id_Reservaties = table.Column<int>(type: "int", nullable: false),
                    Id_PromotieCode = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    DatumToepassing = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reservat__4AA863BE68267C6D", x => new { x.Id_Reservaties, x.Id_PromotieCode });
                    table.ForeignKey(
                        name: "FKReservatie795620",
                        column: x => x.Id_PromotieCode,
                        principalTable: "PromotieCode",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RegulierTijdslot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Schema = table.Column<int>(type: "int", nullable: false),
                    Dag = table.Column<int>(type: "int", nullable: false),
                    StartTijd = table.Column<TimeOnly>(type: "time", nullable: false),
                    EindTijd = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Regulier__3214EC0771C1235F", x => x.Id);
                    table.ForeignKey(
                        name: "FKRegulierTi511075",
                        column: x => x.Id_Schema,
                        principalTable: "Schema",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UitzonderingTijdslot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datum = table.Column<DateOnly>(type: "date", nullable: false),
                    Startijd = table.Column<TimeOnly>(type: "time", nullable: false),
                    Eindtijd = table.Column<TimeOnly>(type: "time", nullable: false),
                    TypeUitzondering = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Id_Schema = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Uitzonde__3214EC07D33295B0", x => x.Id);
                    table.ForeignKey(
                        name: "FKUitzonderi594035",
                        column: x => x.Id_Schema,
                        principalTable: "Schema",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reservaties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumCreatie = table.Column<DateTime>(type: "datetime", nullable: true),
                    DatumReservatie = table.Column<DateTime>(type: "datetime", nullable: true),
                    Id_AspNetUsers = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Id_PromotieCode = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Id_TypeMassage = table.Column<int>(type: "int", nullable: false),
                    Id_RegulierTijdslot = table.Column<int>(type: "int", nullable: false),
                    Id_Prijs = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    TeBetalenBedrag = table.Column<float>(type: "real", nullable: false),
                    Id_Masseur = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reservat__3214EC070AE0EE6F", x => x.Id);
                    table.ForeignKey(
                        name: "FKReservatie307222",
                        column: x => x.Id_PromotieCode,
                        principalTable: "PromotieCode",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKReservatie487046",
                        column: x => x.Id_Prijs,
                        principalTable: "KostPrijs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKReservatie515549",
                        column: x => x.Id_RegulierTijdslot,
                        principalTable: "RegulierTijdslot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKReservatie952270",
                        column: x => x.Id_TypeMassage,
                        principalTable: "TypeMassage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservaties_AspNetUsers",
                        column: x => x.Id_AspNetUsers,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Reservati__Id_Ma__160F4887",
                        column: x => x.Id_Masseur,
                        principalTable: "Masseur",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Betaling",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false, comment: "UUID"),
                    DatumBetaling = table.Column<DateOnly>(type: "date", nullable: false),
                    Betaalmethode = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    TransactieReferentie = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    BetaaldBedrag = table.Column<float>(type: "real", nullable: false),
                    Opmerking = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Id_Reservaties = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Betaling__3214EC077816066A", x => x.Id);
                    table.ForeignKey(
                        name: "FKBetaling230728",
                        column: x => x.Id_Reservaties,
                        principalTable: "Reservaties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "([NormalizedName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "([NormalizedUserName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_Betaling_Id_Reservaties",
                table: "Betaling",
                column: "Id_Reservaties");

            migrationBuilder.CreateIndex(
                name: "IX_KostPrijs_Id_TypeMassage",
                table: "KostPrijs",
                column: "Id_TypeMassage");

            migrationBuilder.CreateIndex(
                name: "IX_Masseur_Id_AspNetUsers",
                table: "Masseur",
                column: "Id_AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Masseur_TypeMassage_Id_TypeMassage",
                table: "Masseur_TypeMassage",
                column: "Id_TypeMassage");

            migrationBuilder.CreateIndex(
                name: "IX_PromotieCode_Id_AspNetUsers",
                table: "PromotieCode",
                column: "Id_AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "UQ__Promotie__A25C5AA72EB1EDE9",
                table: "PromotieCode",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegulierTijdslot_Id_Schema",
                table: "RegulierTijdslot",
                column: "Id_Schema");

            migrationBuilder.CreateIndex(
                name: "IX_Reservatie_PromotieCode_Id_PromotieCode",
                table: "Reservatie_PromotieCode",
                column: "Id_PromotieCode");

            migrationBuilder.CreateIndex(
                name: "IX_Reservaties_Id_AspNetUsers",
                table: "Reservaties",
                column: "Id_AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Reservaties_Id_Masseur",
                table: "Reservaties",
                column: "Id_Masseur");

            migrationBuilder.CreateIndex(
                name: "IX_Reservaties_Id_Prijs",
                table: "Reservaties",
                column: "Id_Prijs");

            migrationBuilder.CreateIndex(
                name: "IX_Reservaties_Id_PromotieCode",
                table: "Reservaties",
                column: "Id_PromotieCode");

            migrationBuilder.CreateIndex(
                name: "IX_Reservaties_Id_RegulierTijdslot",
                table: "Reservaties",
                column: "Id_RegulierTijdslot");

            migrationBuilder.CreateIndex(
                name: "IX_Reservaties_Id_TypeMassage",
                table: "Reservaties",
                column: "Id_TypeMassage");

            migrationBuilder.CreateIndex(
                name: "IX_Schema_Id_Masseur",
                table: "Schema",
                column: "Id_Masseur");

            migrationBuilder.CreateIndex(
                name: "IX_UitzonderingTijdslot_Id_Schema",
                table: "UitzonderingTijdslot",
                column: "Id_Schema");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Betaling");

            migrationBuilder.DropTable(
                name: "Masseur_TypeMassage");

            migrationBuilder.DropTable(
                name: "Reservatie_PromotieCode");

            migrationBuilder.DropTable(
                name: "UitzonderingTijdslot");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Reservaties");

            migrationBuilder.DropTable(
                name: "PromotieCode");

            migrationBuilder.DropTable(
                name: "KostPrijs");

            migrationBuilder.DropTable(
                name: "RegulierTijdslot");

            migrationBuilder.DropTable(
                name: "TypeMassage");

            migrationBuilder.DropTable(
                name: "Schema");

            migrationBuilder.DropTable(
                name: "Masseur");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
