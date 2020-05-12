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
            if (migrationBuilder == null) {
                throw new System.ArgumentNullException(migrationBuilder + " is null");
            }
           
            migrationBuilder.Sql("DELETE FROM Weeks WHERE GirafUserId is NULL");

            migrationBuilder.DropForeignKey(name: "FK_Weeks_AspNetUsers_GirafUserId", table: "Weeks");

            migrationBuilder.AlterColumn<string>(
                name: "GirafUserId",
                table: "Weeks",
                nullable: false
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
            if (migrationBuilder == null) {
                throw new System.ArgumentNullException(migrationBuilder + " is null");
            }
            migrationBuilder.AlterColumn<string>(
                name: "GirafUserId",
                table: "Weeks",
                nullable: true
            );
        }
    }
}
