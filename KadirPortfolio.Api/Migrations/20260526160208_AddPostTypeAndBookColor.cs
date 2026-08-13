using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KadirPortfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPostTypeAndBookColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostType",
                table: "BlogPosts",
                type: "text",
                nullable: false,
                defaultValue: "article");

            migrationBuilder.AddColumn<string>(
                name: "BookColor",
                table: "BlogPosts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookColor",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "PostType",
                table: "BlogPosts");
        }
    }
}
