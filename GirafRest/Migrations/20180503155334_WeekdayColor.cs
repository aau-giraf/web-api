using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    public partial class WeekdayColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeekDayColors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Day = table.Column<int>(nullable: false),
                    HexColor = table.Column<string>(nullable: true),
                    SettingId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekDayColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeekDayColors_Setting_SettingId",
                        column: x => x.SettingId,
                        principalTable: "Setting",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeekDayColors_SettingId",
                table: "WeekDayColors",
                column: "SettingId");

            migrationBuilder.Sql("INSERT INTO WeekDayColors (SettingId, Day, HexColor) SELECT SettingsKey, 1, \"#067700\" From AspNetUsers");
            migrationBuilder.Sql("INSERT INTO WeekDayColors (SettingId, Day, HexColor) SELECT SettingsKey, 2, \"#8c1086\" From AspNetUsers");
            migrationBuilder.Sql("INSERT INTO WeekDayColors (SettingId, Day, HexColor) SELECT SettingsKey, 3, \"#ff7f00\" From AspNetUsers");
            migrationBuilder.Sql("INSERT INTO WeekDayColors (SettingId, Day, HexColor) SELECT SettingsKey, 4, \"#0017ff\" From AspNetUsers");
            migrationBuilder.Sql("INSERT INTO WeekDayColors (SettingId, Day, HexColor) SELECT SettingsKey, 5, \"#ffdd00\" From AspNetUsers");
            migrationBuilder.Sql("INSERT INTO WeekDayColors (SettingId, Day, HexColor) SELECT SettingsKey, 6, \"#ff0102\" From AspNetUsers");
            migrationBuilder.Sql("INSERT INTO WeekDayColors (SettingId, Day, HexColor) SELECT SettingsKey, 7, \"#ffffff\" From AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeekDayColors");
        }
    }
}
