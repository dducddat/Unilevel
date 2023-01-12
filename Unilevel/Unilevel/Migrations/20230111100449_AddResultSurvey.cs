using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unilevel.Migrations
{
    public partial class AddResultSurvey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ResultSurveys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResultA = table.Column<bool>(type: "bit", nullable: false),
                    ResultB = table.Column<bool>(type: "bit", nullable: false),
                    ResultC = table.Column<bool>(type: "bit", nullable: false),
                    ResultD = table.Column<bool>(type: "bit", nullable: false),
                    SurveyId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultSurveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultSurveys_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultSurveys_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultSurveys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultSurveys_QuestionId",
                table: "ResultSurveys",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultSurveys_SurveyId",
                table: "ResultSurveys",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultSurveys_UserId",
                table: "ResultSurveys",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultSurveys");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");
        }
    }
}
