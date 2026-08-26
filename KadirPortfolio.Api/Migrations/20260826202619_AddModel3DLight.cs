using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KadirPortfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddModel3DLight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Model3DUrlLight",
                table: "HomeSettings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Model3DUrlLight",
                table: "HomeSettings");
        }
    }
}
