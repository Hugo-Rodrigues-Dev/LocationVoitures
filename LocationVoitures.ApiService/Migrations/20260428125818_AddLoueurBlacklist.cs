using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocationVoitures.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddLoueurBlacklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "est_blacklist",
                table: "loueur",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "est_blacklist",
                table: "loueur");
        }
    }
}
