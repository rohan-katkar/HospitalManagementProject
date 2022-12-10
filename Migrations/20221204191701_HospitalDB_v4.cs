using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class HospitalDBv4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Departments_DepartmentId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Doctors_DoctorId",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Patients",
                newName: "DoctorRefId");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_DoctorId",
                table: "Patients",
                newName: "IX_Patients_DoctorRefId");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Doctors",
                newName: "DeptRefId");

            migrationBuilder.RenameIndex(
                name: "IX_Doctors_DepartmentId",
                table: "Doctors",
                newName: "IX_Doctors_DeptRefId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Departments_DeptRefId",
                table: "Doctors",
                column: "DeptRefId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Doctors_DoctorRefId",
                table: "Patients",
                column: "DoctorRefId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Departments_DeptRefId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Doctors_DoctorRefId",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "DoctorRefId",
                table: "Patients",
                newName: "DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_DoctorRefId",
                table: "Patients",
                newName: "IX_Patients_DoctorId");

            migrationBuilder.RenameColumn(
                name: "DeptRefId",
                table: "Doctors",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Doctors_DeptRefId",
                table: "Doctors",
                newName: "IX_Doctors_DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Departments_DepartmentId",
                table: "Doctors",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Doctors_DoctorId",
                table: "Patients",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
