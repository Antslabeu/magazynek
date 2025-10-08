using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magazynek.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    package = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    farnellid = table.Column<string>(type: "text", nullable: false),
                    tmeid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "project",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "project_item",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    itemid = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    projectid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_item", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "project_realization",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_realization", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shipping_entry",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    item = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<long>(type: "bigint", nullable: false),
                    last_check = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    stock = table.Column<long>(type: "bigint", nullable: false),
                    price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipping_entry", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "systemsettings",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systemsettings", x => x.name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_id",
                table: "product",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_id",
                table: "project",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_item_id",
                table: "project_item",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_realization_id",
                table: "project_realization",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shipping_entry_id",
                table: "shipping_entry",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_systemsettings_name",
                table: "systemsettings",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "project");

            migrationBuilder.DropTable(
                name: "project_item");

            migrationBuilder.DropTable(
                name: "project_realization");

            migrationBuilder.DropTable(
                name: "shipping_entry");

            migrationBuilder.DropTable(
                name: "systemsettings");
        }
    }
}
