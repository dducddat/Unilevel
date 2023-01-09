using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unilevel.Migrations
{
    public partial class AddRemoveDis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Distributors_PhoneNumber",
                table: "Distributors");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Distributors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Remove",
                table: "Distributors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_PhoneNumber",
                table: "Distributors",
                column: "PhoneNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Distributors_PhoneNumber",
                table: "Distributors");

            migrationBuilder.DropColumn(
                name: "Remove",
                table: "Distributors");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Distributors",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_PhoneNumber",
                table: "Distributors",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");
        }
    }
}
