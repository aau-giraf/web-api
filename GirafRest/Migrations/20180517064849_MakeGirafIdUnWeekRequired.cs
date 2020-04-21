using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    /// <summary>
    /// Removes nullability for girafUserId
    /// </summary>
    public partial class MakeGirafIdUnWeekRequired : Migration
    {
        /// <summary>
        /// Run migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.Sql("DELETE FROM Weeks WHERE GirafUserId is NULL");

            migrationBuilder.DropForeignKey(name: "FK_Weeks_AspNetUsers_GirafUserId", table: "Weeks");

            migrationBuilder.AlterColumn<string>(
                name: "GirafUserId",
                table: "Weeks",
                nullable: false,
                maxLength: 127,
                type: "varchar(127)"
            );

            migrationBuilder.AddForeignKey(name: "FK_Weeks_AspNetUsers_GirafUserId",
                table: "Weeks",
            column: "GirafUserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
        }

        /// <summary>
        /// Rollback migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GirafUserId",
                table: "Weeks",
                nullable: true,
                maxLength: 127,
                type: "varchar(127)"
            );
        }
    }
}
