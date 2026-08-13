using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshBake.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessLevelToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "AccessLevelId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AccessLevel",
                columns: table => new
                {
                    AccessLevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessLevelName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLevel", x => x.AccessLevelId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AccessLevelId",
                table: "Users",
                column: "AccessLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AccessLevel_AccessLevelId",
                table: "Users",
                column: "AccessLevelId",
                principalTable: "AccessLevel",
                principalColumn: "AccessLevelId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AccessLevel_AccessLevelId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "AccessLevel");

            migrationBuilder.DropIndex(
                name: "IX_Users_AccessLevelId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccessLevelId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
