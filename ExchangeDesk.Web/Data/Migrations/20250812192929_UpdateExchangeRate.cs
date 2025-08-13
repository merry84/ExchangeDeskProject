using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExchangeDesk.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExchangeRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ако предишен seed е добавял офис с Id=3, го махаме
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 3);

            // Offices.Location (NOT NULL)
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Offices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // ExchangeRates: промяна на прецизността и добавяне на нови колони
            migrationBuilder.AlterColumn<decimal>(
                name: "RateToBGN",
                table: "ExchangeRates",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "BuyRate",
                table: "ExchangeRates",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OfficeId",
                table: "ExchangeRates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SellRate",
                table: "ExchangeRates",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            // ExchangeRates.CurrencyCode вече е с [MaxLength(3)] в модела → EF е генерирал AlterColumn по-рано
            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "ExchangeRates",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // ВАЖНО: Currencies.Code е PK. За да го смалим до nvarchar(3), сваляме PK → Alter → Add PK
            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Currencies",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Code");

            // Seed корекции за Offices
            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 1,
                column: "Location",
                value: "София");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Location", "Name" },
                values: new object[] { "Пловдив", "Офис 2" });

            // Индекси за новите FK
            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyCode",
                table: "ExchangeRates",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_OfficeId",
                table: "ExchangeRates",
                column: "OfficeId");

            // Външни ключове
            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeRates_Currencies_CurrencyCode",
                table: "ExchangeRates",
                column: "CurrencyCode",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeRates_Offices_OfficeId",
                table: "ExchangeRates",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Примерен seed за курсове (OfficeId=1)
            migrationBuilder.InsertData(
                table: "ExchangeRates",
                columns: new[] { "Id", "AsOf", "BuyRate", "CurrencyCode", "OfficeId", "RateToBGN", "SellRate", "Source" },
                values: new object[,]
                {
                    { 1, DateTime.UtcNow, 1.9550m, "EUR", 1, 1.95583m, 1.9570m, "BNB" },
                    { 2, DateTime.UtcNow, 1.7950m, "USD", 1, 1.8000m, 1.8050m, "BNB" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Премахваме FK и индекси
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeRates_Currencies_CurrencyCode",
                table: "ExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeRates_Offices_OfficeId",
                table: "ExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_CurrencyCode",
                table: "ExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_OfficeId",
                table: "ExchangeRates");

            // Премахваме seed-натите курсове
            migrationBuilder.DeleteData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 2);

            // Връщаме Offices без Location
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Offices");

            // Връщаме ExchangeRates към стария вид
            migrationBuilder.DropColumn(
                name: "BuyRate",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "SellRate",
                table: "ExchangeRates");

            migrationBuilder.AlterColumn<decimal>(
                name: "RateToBGN",
                table: "ExchangeRates",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "ExchangeRates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            // Връщаме Currencies.Code към nvarchar(450): Drop PK → Alter → Add PK
            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Currencies",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Code");

            // Възстановяваме предишния seed (ако е имало)
            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Клон 1");

            migrationBuilder.InsertData(
                table: "Offices",
                columns: new[] { "Id", "IsCentral", "Name" },
                values: new object[] { 3, false, "Клон 2" });
        }
    }
}
