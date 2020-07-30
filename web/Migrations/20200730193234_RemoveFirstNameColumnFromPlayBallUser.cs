using Microsoft.EntityFrameworkCore.Migrations;

namespace web.Migrations
{
    public partial class RemoveFirstNameColumnFromPlayBallUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "public",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "public",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }
    }
}
