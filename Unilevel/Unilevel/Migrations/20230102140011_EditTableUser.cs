using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unilevel.Migrations
{
    public partial class EditTableUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Areas_AreaCore",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PasswordHass",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "AreaCore",
                table: "Users",
                newName: "AreaCode");

            migrationBuilder.RenameIndex(
                name: "IX_Users_AreaCore",
                table: "Users",
                newName: "IX_Users_AreaCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Areas_AreaCode",
                table: "Users",
                column: "AreaCode",
                principalTable: "Areas",
                principalColumn: "AreaCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Areas_AreaCode",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "PasswordHass");

            migrationBuilder.RenameColumn(
                name: "AreaCode",
                table: "Users",
                newName: "AreaCore");

            migrationBuilder.RenameIndex(
                name: "IX_Users_AreaCode",
                table: "Users",
                newName: "IX_Users_AreaCore");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Areas_AreaCore",
                table: "Users",
                column: "AreaCore",
                principalTable: "Areas",
                principalColumn: "AreaCode");
        }
    }
}
