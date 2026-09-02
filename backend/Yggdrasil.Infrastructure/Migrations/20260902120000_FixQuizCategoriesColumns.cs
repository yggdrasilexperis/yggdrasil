using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

using Yggdrasil.Infrastructure.Persistence;

#nullable disable

namespace Yggdrasil.Infrastructure.Migrations
{
    [DbContext(typeof(YggdrasilDbContext))]
    [Migration("20260902120000_FixQuizCategoriesColumns")]
    public partial class FixQuizCategoriesColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "QuizCategories");

            migrationBuilder.CreateTable(
                name: "QuizCategories",
                columns: table => new
                {
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizCategories", x => new { x.QuizId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_QuizCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizCategories_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizCategories_CategoryId",
                table: "QuizCategories",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder) =>
            migrationBuilder.DropTable(name: "QuizCategories");
    }
}
