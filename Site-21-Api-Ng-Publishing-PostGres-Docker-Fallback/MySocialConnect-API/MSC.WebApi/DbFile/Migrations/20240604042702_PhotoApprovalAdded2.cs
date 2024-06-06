using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSC.WebApi.DbFile.Migrations
{
    /// <inheritdoc />
    public partial class PhotoApprovalAdded2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Photos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "Photos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                table: "Photos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "Photos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER")
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "Photos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Photos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Relational:ColumnOrder", 1)
                .OldAnnotation("Sqlite:Autoincrement", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Photos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "Photos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                table: "Photos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER")
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "Photos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER")
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "Photos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Photos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);
        }
    }
}
