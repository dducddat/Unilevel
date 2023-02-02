using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unilevel.Migrations
{
    public partial class AddColTableVisit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remove",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "CreateByUserId",
                table: "VisitPlans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlans_CreateByUserId",
                table: "VisitPlans",
                column: "CreateByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitPlans_Users_CreateByUserId",
                table: "VisitPlans",
                column: "CreateByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitPlans_Users_CreateByUserId",
                table: "VisitPlans");

            migrationBuilder.DropIndex(
                name: "IX_VisitPlans_CreateByUserId",
                table: "VisitPlans");

            migrationBuilder.DropColumn(
                name: "CreateByUserId",
                table: "VisitPlans");

            migrationBuilder.AddColumn<bool>(
                name: "Remove",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
