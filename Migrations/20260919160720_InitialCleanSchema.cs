using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FreshBake.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCleanSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Customers",
                newName: "AppliedByUserId");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomerAdmin",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    PageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RouteKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.PageId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessLevelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Roles_AccessLevel_AccessLevelId",
                        column: x => x.AccessLevelId,
                        principalTable: "AccessLevel",
                        principalColumn: "AccessLevelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePagePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    CanView = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePagePermissions", x => new { x.RoleId, x.PageId });
                    table.ForeignKey(
                        name: "FK_RolePagePermissions_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "PageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePagePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AccessLevel",
                columns: new[] { "AccessLevelId", "AccessLevelName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Customer" },
                    { 3, "SuperUser" },
                    { 4, "User" }
                });

            migrationBuilder.InsertData(
                table: "Pages",
                columns: new[] { "PageId", "Name", "RouteKey" },
                values: new object[,]
                {
                    { 1, "Dashboard", "Dashboard" },
                    { 2, "Manage Users", "ManageUsers" },
                    { 3, "Manage Customers", "ManageCustomers" },
                    { 4, "Customer Manual Registration", "CustomerManualRegistration" },
                    { 5, "Manage Products", "ManageProducts" },
                    { 6, "Manage Product Categories", "ManageProductCategories" },
                    { 7, "User Permissions", "UserPermissions" },
                    { 8, "My Details", "MyDetails" },
                    { 9, "Customer Self Registration", "CustomerSelfRegistration" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "AccessLevelId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 1, null, "Admin" },
                    { 2, 3, null, "Super User" },
                    { 3, 2, "Base customer access", "Customer" },
                    { 4, 2, "Business-level management access", "Customer Manager" },
                    { 5, 2, "Limited business staff access", "Customer Employee" },
                    { 6, 4, "Generic, not-yet-elevated account", "User" }
                });

            migrationBuilder.InsertData(
                table: "RolePagePermissions",
                columns: new[] { "PageId", "RoleId", "CanView" },
                values: new object[,]
                {
                    { 1, 1, true },
                    { 2, 1, true },
                    { 3, 1, true },
                    { 4, 1, true },
                    { 5, 1, true },
                    { 6, 1, true },
                    { 7, 1, true },
                    { 8, 1, true },
                    { 9, 1, true },
                    { 1, 2, true },
                    { 2, 2, true },
                    { 3, 2, true },
                    { 4, 2, true },
                    { 5, 2, true },
                    { 6, 2, true },
                    { 7, 2, true },
                    { 8, 2, true },
                    { 9, 2, true },
                    { 1, 3, true },
                    { 8, 3, true },
                    { 9, 3, true },
                    { 1, 4, true },
                    { 8, 4, true },
                    { 1, 5, true },
                    { 8, 5, true },
                    { 1, 6, true },
                    { 8, 6, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CustomerId",
                table: "Users",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AppliedByUserId",
                table: "Customers",
                column: "AppliedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePagePermissions_PageId",
                table: "RolePagePermissions",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_AccessLevelId",
                table: "Roles",
                column: "AccessLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_AppliedByUserId",
                table: "Customers",
                column: "AppliedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Customers_CustomerId",
                table: "Users",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_AppliedByUserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Customers_CustomerId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "RolePagePermissions");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Users_CustomerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AppliedByUserId",
                table: "Customers");

            migrationBuilder.DeleteData(
                table: "AccessLevel",
                keyColumn: "AccessLevelId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AccessLevel",
                keyColumn: "AccessLevelId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AccessLevel",
                keyColumn: "AccessLevelId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AccessLevel",
                keyColumn: "AccessLevelId",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsCustomerAdmin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "AppliedByUserId",
                table: "Customers",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
