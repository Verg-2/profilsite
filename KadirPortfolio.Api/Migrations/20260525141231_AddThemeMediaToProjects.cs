using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KadirPortfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddThemeMediaToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "DarkImageUrls",
                table: "Projects",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DarkVideoUrl",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "LightImageUrls",
                table: "Projects",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LightVideoUrl",
                table: "Projects",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DarkImageUrls",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DarkVideoUrl",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LightImageUrls",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LightVideoUrl",
                table: "Projects");
        }
    }
}
