using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unilevel.Migrations
{
    public partial class ModifierDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanionLists");

            migrationBuilder.AddColumn<string>(
                name: "GuestId",
                table: "VisitPlans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "VisitPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "VisitPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlans_GuestId",
                table: "VisitPlans",
                column: "GuestId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitPlans_Users_GuestId",
                table: "VisitPlans",
                column: "GuestId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitPlans_Users_GuestId",
                table: "VisitPlans");

            migrationBuilder.DropIndex(
                name: "IX_VisitPlans_GuestId",
                table: "VisitPlans");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "VisitPlans");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VisitPlans");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "VisitPlans");

            migrationBuilder.CreateTable(
                name: "CompanionLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VisitPlanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanionLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanionLists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanionLists_VisitPlans_VisitPlanId",
                        column: x => x.VisitPlanId,
                        principalTable: "VisitPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanionLists_UserId",
                table: "CompanionLists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanionLists_VisitPlanId",
                table: "CompanionLists",
                column: "VisitPlanId");
        }
    }
}
