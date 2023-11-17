using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsApp.Migrations
{
    /// <inheritdoc />
    public partial class correcionBug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsEntidad_AppThemes_ThemeId",
                table: "NewsEntidad");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsEntidad",
                table: "NewsEntidad");

            migrationBuilder.RenameTable(
                name: "NewsEntidad",
                newName: "AppNews");

            migrationBuilder.RenameIndex(
                name: "IX_NewsEntidad_ThemeId",
                table: "AppNews",
                newName: "IX_AppNews_ThemeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppNews",
                table: "AppNews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppNews_AppThemes_ThemeId",
                table: "AppNews",
                column: "ThemeId",
                principalTable: "AppThemes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppNews_AppThemes_ThemeId",
                table: "AppNews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppNews",
                table: "AppNews");

            migrationBuilder.RenameTable(
                name: "AppNews",
                newName: "NewsEntidad");

            migrationBuilder.RenameIndex(
                name: "IX_AppNews_ThemeId",
                table: "NewsEntidad",
                newName: "IX_NewsEntidad_ThemeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsEntidad",
                table: "NewsEntidad",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsEntidad_AppThemes_ThemeId",
                table: "NewsEntidad",
                column: "ThemeId",
                principalTable: "AppThemes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
