using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    /// <summary>
    /// Introduces Timer in database
    /// </summary>
    public partial class AddedTimerTable : Migration
    {
        /// <summary>
        /// Run migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null) 
                throw new System.ArgumentNullException(nameof(migrationBuilder) + new string(" is null"));
            
            migrationBuilder.AddColumn<long>(
                name: "TimerKey",
                table: "Activities",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Timers",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartTime = table.Column<long>(nullable: false),
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

        /// <summary>
        /// Rollback migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null) 
                throw new System.ArgumentNullException(nameof(migrationBuilder) + new string(" is null"));
            
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
