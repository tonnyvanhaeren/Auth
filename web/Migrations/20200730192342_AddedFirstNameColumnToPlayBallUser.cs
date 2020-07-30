using Microsoft.EntityFrameworkCore.Migrations;

namespace web.Migrations
{
    public partial class AddedFirstNameColumnToPlayBallUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "public",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "public",
                table: "AspNetUsers");
        }
    }
}
