using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KadirPortfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEnglishFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "SkillItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "SkillCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeoDescriptionEn",
                table: "SeoSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeoTitleEn",
                table: "SeoSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoDescriptionEn",
                table: "SeoSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoTitleEn",
                table: "SeoSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AimEn",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChallengesAndSolutionsEn",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SummaryEn",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "ProjectCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ButtonTextEn",
                table: "HomeSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroSubtitleEn",
                table: "HomeSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroTitleEn",
                table: "HomeSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreTitleEn",
                table: "HomeSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryButtonTextEn",
                table: "HomeSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubtitleEn",
                table: "ContactCards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "ContactCards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                table: "BlogPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProTipEn",
                table: "BlogPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SummaryEn",
                table: "BlogPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "BlogPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "BlogCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BioEn",
                table: "AboutSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardSubtitleEn",
                table: "AboutSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardTitleEn",
                table: "AboutSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainTitleEn",
                table: "AboutSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubTitleEn",
                table: "AboutSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "ListItemsEn",
                table: "AboutCards",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                table: "AboutCards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "AboutCards",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "SkillItems");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "SkillCategories");

            migrationBuilder.DropColumn(
                name: "GeoDescriptionEn",
                table: "SeoSettings");

            migrationBuilder.DropColumn(
                name: "GeoTitleEn",
                table: "SeoSettings");

            migrationBuilder.DropColumn(
                name: "SeoDescriptionEn",
                table: "SeoSettings");

            migrationBuilder.DropColumn(
                name: "SeoTitleEn",
                table: "SeoSettings");

            migrationBuilder.DropColumn(
                name: "AimEn",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ChallengesAndSolutionsEn",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "SummaryEn",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "ProjectCategories");

            migrationBuilder.DropColumn(
                name: "ButtonTextEn",
                table: "HomeSettings");

            migrationBuilder.DropColumn(
                name: "HeroSubtitleEn",
                table: "HomeSettings");

            migrationBuilder.DropColumn(
                name: "HeroTitleEn",
                table: "HomeSettings");

            migrationBuilder.DropColumn(
                name: "PreTitleEn",
                table: "HomeSettings");

            migrationBuilder.DropColumn(
                name: "SecondaryButtonTextEn",
                table: "HomeSettings");

            migrationBuilder.DropColumn(
                name: "SubtitleEn",
                table: "ContactCards");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "ContactCards");

            migrationBuilder.DropColumn(
                name: "ContentEn",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ProTipEn",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "SummaryEn",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "BioEn",
                table: "AboutSettings");

            migrationBuilder.DropColumn(
                name: "CardSubtitleEn",
                table: "AboutSettings");

            migrationBuilder.DropColumn(
                name: "CardTitleEn",
                table: "AboutSettings");

            migrationBuilder.DropColumn(
                name: "MainTitleEn",
                table: "AboutSettings");

            migrationBuilder.DropColumn(
                name: "SubTitleEn",
                table: "AboutSettings");

            migrationBuilder.DropColumn(
                name: "ListItemsEn",
                table: "AboutCards");

            migrationBuilder.DropColumn(
                name: "TextEn",
                table: "AboutCards");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "AboutCards");
        }
    }
}
