using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    public partial class AddedTimerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TimerKey",
                table: "Activities",
                nullable: true,
                defaultValue: null);

            migrationBuilder.CreateTable(
                name: "Timers",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartTime = table.Column<DateTime>(nullable: false),
                    Progress = table.Column<long>(nullable: false),
                    FullLength = table.Column<long>(nullable: false),
                    Paused = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timers", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TimerKey",
                table: "Activities",
                column: "TimerKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Timers_TimerKey",
                table: "Activities",
                column: "TimerKey",
                principalTable: "Timers",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Timers_TimerKey",
                table: "Activities");

            migrationBuilder.DropTable(
                name: "Timers");

            migrationBuilder.DropIndex(
                name: "IX_Activities_TimerKey",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "TimerKey",
                table: "Activities");
        }
    }
}
