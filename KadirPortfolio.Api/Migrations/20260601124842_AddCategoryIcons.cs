using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KadirPortfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIcons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "ProjectCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "BlogCategories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "ProjectCategories");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "BlogCategories");
        }
    }
}
