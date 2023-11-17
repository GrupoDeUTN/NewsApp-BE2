using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsApp.Migrations
{
    /// <inheritdoc />
    public partial class nuevaMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsEntidad_AppThemes_ThemeId",
                table: "NewsEntidad");

            migrationBuilder.AlterColumn<int>(
                name: "ThemeId",
                table: "NewsEntidad",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsEntidad_AppThemes_ThemeId",
                table: "NewsEntidad",
                column: "ThemeId",
                principalTable: "AppThemes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsEntidad_AppThemes_ThemeId",
                table: "NewsEntidad");

            migrationBuilder.AlterColumn<int>(
                name: "ThemeId",
                table: "NewsEntidad",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsEntidad_AppThemes_ThemeId",
                table: "NewsEntidad",
                column: "ThemeId",
                principalTable: "AppThemes",
                principalColumn: "Id");
        }
    }
}
