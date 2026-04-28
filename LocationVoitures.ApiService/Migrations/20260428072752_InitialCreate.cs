using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LocationVoitures.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "loueur",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    prenom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mobile = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    pays = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "France")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loueur", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "voiture",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    immatriculation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    marque = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    modele = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    categorie = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    prix_location_par_jour = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voiture", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "location",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    voiture_id = table.Column<int>(type: "integer", nullable: false),
                    loueur_id = table.Column<int>(type: "integer", nullable: false),
                    date_debut = table.Column<DateOnly>(type: "date", nullable: false),
                    date_fin = table.Column<DateOnly>(type: "date", nullable: false),
                    annule = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_loueur_loueur_id",
                        column: x => x.loueur_id,
                        principalTable: "loueur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_location_voiture_voiture_id",
                        column: x => x.voiture_id,
                        principalTable: "voiture",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_location_loueur_id",
                table: "location",
                column: "loueur_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_voiture_id",
                table: "location",
                column: "voiture_id");

            migrationBuilder.CreateIndex(
                name: "IX_loueur_mobile",
                table: "loueur",
                column: "mobile",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_voiture_immatriculation",
                table: "voiture",
                column: "immatriculation",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "location");

            migrationBuilder.DropTable(
                name: "loueur");

            migrationBuilder.DropTable(
                name: "voiture");
        }
    }
}
