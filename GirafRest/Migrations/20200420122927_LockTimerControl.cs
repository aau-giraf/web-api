using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    public partial class LockTimerControl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null) 
                throw new System.ArgumentNullException(nameof(migrationBuilder) + new string(" is null"));
            
            migrationBuilder.AddColumn<bool>(
                name: "LockTimerControl",
                table: "Setting",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null) 
                throw new System.ArgumentNullException(nameof(migrationBuilder) + new string(" is null"));
            
            migrationBuilder.DropColumn(
                name: "LockTimerControl",
                table: "Setting");
        }
    }
}
