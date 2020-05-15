using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    public partial class MakeDisplayNameInAspnetusersRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null) {
                throw new System.ArgumentNullException(migrationBuilder + " is null");
            }
            migrationBuilder.Sql("UPDATE aspnetusers SET DisplayName = UserName WHERE DisplayName IS NULL");
            
            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null) {
                throw new System.ArgumentNullException(migrationBuilder + " is null");
            }
            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
