using Microsoft.EntityFrameworkCore.Migrations;

namespace NugetAnalyzer.DAL.Migrations
{
    public partial class UserGitHubToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitHubToken",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHubToken",
                table: "Users");
        }
    }
}
