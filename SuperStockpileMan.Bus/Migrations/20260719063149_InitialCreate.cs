// SPDX-FileCopyrightText: 2026 SAKURAI Tayra <tayra_sakurai@icloud.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuperStockpileMan.Bus.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryBases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryBases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryBases_CategoryBases_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CategoryBases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategoryBases_CategoryBases_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CategoryBases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateTime = table.Column<DateTimeOffset>(
                        type: "TEXT",
                        nullable: false,
                        defaultValueSql: "DATETIME('now') || 'Z'"),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    Action = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    PurchaseDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsStocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    SmallestCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_CategoryBases_SmallestCategoryId",
                        column: x => x.SmallestCategoryId,
                        principalTable: "CategoryBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Items_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryBases_CategoryId",
                table: "CategoryBases",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryBases_Name",
                table: "CategoryBases",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryBases_ParentId",
                table: "CategoryBases",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_LocationId",
                table: "Items",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Name",
                table: "Items",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_SmallestCategoryId",
                table: "Items",
                column: "SmallestCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Name",
                table: "Locations",
                column: "Name",
                unique: true);

            migrationBuilder.Sql(
                @"
CREATE TRIGGER log_insertion AFTER INSERT ON Items
FOR EACH ROW
BEGIN
    INSERT INTO Logs(ItemId, Action) VALUES (NEW.Id, 1);
END;

CREATE TRIGGER log_update AFTER UPDATE ON Items
FOR EACH ROW
BEGIN
    INSERT INTO Logs(ItemId, Action) VALUES (NEW.Id, 3);
END;

CREATE TRIGGER log_removal AFTER DELETE ON Items
FOR EACH ROW
BEGIN
    INSERT INTO Logs(ItemId, Action) VALUES (OLD.Id, 2);
END;
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "CategoryBases");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}

