﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTIN_Index_CHk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TIN",
                table: "Persons",
                sql: "len([TaxIdentificationNumber]) = 8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropCheckConstraint(
                name: "CHK_TIN",
                table: "Persons");
        }
    }
}
