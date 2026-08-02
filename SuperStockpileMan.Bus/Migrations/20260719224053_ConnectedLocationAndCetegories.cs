// SPDX-FileCopyrightText: 2026 SAKURAI Tayra <tayra_sakurai@icloud.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuperStockpileMan.Bus.Migrations
{
    /// <inheritdoc />
    public partial class ConnectedLocationAndCetegories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryBaseLocation",
                columns: table => new
                {
                    CategoryBasesId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryBaseLocation", x => new { x.CategoryBasesId, x.LocationsId });
                    table.ForeignKey(
                        name: "FK_CategoryBaseLocation_CategoryBases_CategoryBasesId",
                        column: x => x.CategoryBasesId,
                        principalTable: "CategoryBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryBaseLocation_Locations_LocationsId",
                        column: x => x.LocationsId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryBaseLocation_LocationsId",
                table: "CategoryBaseLocation",
                column: "LocationsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryBaseLocation");
        }
    }
}

