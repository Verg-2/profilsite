using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KadirPortfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeSettingsSecondaryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreTitle",
                table: "HomeSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryButtonText",
                table: "HomeSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryButtonUrl",
                table: "HomeSettings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreTitle",
                table: "HomeSettings");

            migrationBuilder.DropColumn(
                name: "SecondaryButtonText",
                table: "HomeSettings");

            migrationBuilder.DropColumn(
                name: "SecondaryButtonUrl",
                table: "HomeSettings");
        }
    }
}
