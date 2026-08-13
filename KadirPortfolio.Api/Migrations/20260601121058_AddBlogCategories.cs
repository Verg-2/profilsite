using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KadirPortfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlogCategoryId",
                table: "BlogPosts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BlogCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_BlogCategoryId",
                table: "BlogPosts",
                column: "BlogCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_BlogCategories_BlogCategoryId",
                table: "BlogPosts",
                column: "BlogCategoryId",
                principalTable: "BlogCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_BlogCategories_BlogCategoryId",
                table: "BlogPosts");

            migrationBuilder.DropTable(
                name: "BlogCategories");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_BlogCategoryId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "BlogCategoryId",
                table: "BlogPosts");
        }
    }
}
