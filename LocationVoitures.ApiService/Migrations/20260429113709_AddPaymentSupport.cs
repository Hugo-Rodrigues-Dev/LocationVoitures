using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocationVoitures.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "paye",
                table: "location",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paye",
                table: "location");
        }
    }
}
