using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using Oracle.DataAccess.Client;
using System.Text;
using System.Threading.Tasks;

namespace OracleInterface
{
    public static class Utils
    {
        public static string GetAppVersion()
        {
            string version = "";

            Version appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            version = string.Format(@" v{0}.{1}.{2} (r{3})", appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision);
            return version;
        }

        public static bool IsConfigDataComplete(SettingsConfiguration.ConfigData configData, bool dbOrion)
        {
            bool result = true;

            if (string.IsNullOrEmpty(configData.IcaServer))
                result = false;
            else if (string.IsNullOrEmpty(configData.IcaDatabase))
                result = false;
            else if (string.IsNullOrEmpty(configData.IcaUser))
                result = false;
            else if (string.IsNullOrEmpty(configData.IcaPassword))
                result = false;
            else if (dbOrion)
            {
                if (string.IsNullOrEmpty(configData.OrionServer))
                    result = false;
                else if (string.IsNullOrEmpty(configData.OrionDatabase))
                    result = false;
                else if (string.IsNullOrEmpty(configData.OrionUser))
                    result = false;
                else if (string.IsNullOrEmpty(configData.OrionPassword))
                    result = false;
            }
            else if (string.IsNullOrEmpty(configData.QuantoDatabase))
                result = false;
            
            return result;
        }

        public static string GetConnectionString(String name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            //return "Data Source=xe;User ID=ICA;Password=ICA;Max Pool Size=20"; // providerName = "System.Data.OracleClient";
        }

        public static bool CheckDatabaseOpen(int databaseType, object database)
        {
            switch (databaseType)
            {
                case 0: // es un archivo de Access
                    if (File.Exists((database as string)))  // El objeto en este caso es un string con el nombre del archivo de Access
                        return true;
                    break;

                case 1: // vamos a verificar la conexion a una BD SQL Server
                    if ((database as SqlConnection).State == ConnectionState.Open)
                    {
                        return true;
                    }
                    else
                    {
                        //messageTextbox.AddError("La base de datos SQL Server del sistema no esta abierta por lo tanto no se puede importar la información de los empleados.");
                    }
                    break;

                case 2: // Vamos a verificar la conexion a una BD Oracle                    
                    if ((database as OracleConnection).State == ConnectionState.Open)
                    {
                       return true;
                    }
                    break;
            }

            return false;
        }

        public static Summary LoadEmployeesFromDataset(List<Employee> employees, DataTable employeesDT)
        {
            int employeeIndex;
            Summary summary = new Summary();
 
            employeeIndex = 0;

            //progressBar1.Value = 0;
            //progressBar1.Maximum = Lines.Count;
            foreach (DataRow employeeRecord in employeesDT.Rows)
            {
                Employee newEmployee = new Employee();
                
                if (employeesDT.Columns.Count >= 40)
                {
                    newEmployee.AlphaIdentification = employeeRecord["NO_EMPL"].ToString();
                    //newEmployee.Identification = employeeFields[1].Substring(1, 6);
                    newEmployee.Identification      = employeeRecord["NO_EMPL"].ToString().Substring(1, 6);
                    newEmployee.LastName            = employeeRecord["APELLIDO_PATERNO"].ToString();
                    newEmployee.OtherName           = employeeRecord["APELLIDO_MATERNO"].ToString();
                    newEmployee.FirstName           = employeeRecord["NOMBRE"].ToString();
                    newEmployee.Imss                = employeeRecord["IMSS"].ToString();
                    newEmployee.RFC                 = employeeRecord["RFC"].ToString();
                    newEmployee.CURP                = employeeRecord["CURP"].ToString();
                    newEmployee.BirthDate           = employeeRecord.Field<DateTime?>("FCH_NACIMIENTO") ?? DateTime.MinValue;
                    newEmployee.HireDate            = employeeRecord.Field<DateTime?>("FCH_INGRESO") ?? DateTime.MinValue;
                    newEmployee.CivilStatus         = employeeRecord["EDO_CIVIL"].ToString();
                    newEmployee.Sex                 = employeeRecord["SEXO"].ToString();
                    newEmployee.Nationality         = employeeRecord["NACIONALIDAD"].ToString();
                    newEmployee.PositionID          = employeeRecord["CVE_PUESTO1"].ToString();
                    newEmployee.PositionName        = employeeRecord["PUESTO"].ToString();
                    newEmployee.Frente              = employeeRecord["FRENTE"].ToString();
                    newEmployee.EmployeeClass       = employeeRecord["CLASIF_EMPLEADO"].ToString();
                    newEmployee.EmployeeType        = employeeRecord["TIPO_EMPLEADO"].ToString();
                    newEmployee.Impats              = employeeRecord["IMPATS"].ToString();
                    newEmployee.ContractType        = employeeRecord["TIPO_CONTRATO"].ToString();
                    newEmployee.ProjectNumber       = employeeRecord["PROYECTO_NUM"].ToString();
                    newEmployee.Payroll             = employeeRecord["NOMINA"].ToString();
                    // Usamos el archivo inicial que solo tiene 40 campos
                    newEmployee.AddressStreet       = employeeRecord["DIR_CALLE_NUMERO"].ToString();
                    newEmployee.AddressColony       = employeeRecord["DIR_COLONIA"].ToString();
                    newEmployee.AddressDeleg        = employeeRecord["DIR_DELG_MUN"].ToString();
                    newEmployee.AddressState        = employeeRecord["DIR_ESTADO"].ToString();
                    newEmployee.ZIP                 = employeeRecord["DIR_CODIGO_POSTAL"].ToString();
                    newEmployee.PhoneNumber         = employeeRecord["DIR_TELEFONO1"].ToString();
                    newEmployee.AddressCity         = employeeRecord["DIR_CIUDAD"].ToString();

                    if (employeesDT.Columns.Count > 40)
                    {
                        // Usamos el archivo nuevo que tiene incluidos los campos con la
                        // informacion para las bajas de los empleados
                        newEmployee.AddressCountry = employeeRecord["DIR_PAIS"].ToString();
                        newEmployee.EmployeeStatus = employeeRecord["STATUS_EMPL"].ToString();

                        if (string.IsNullOrEmpty(newEmployee.EmployeeStatus))
                        {
                            /// no venia el cammpo en la linea del registro
                            /// 
                            //messageTextbox.AddError(string.Format("El empleado {0} no tiene los campos necesarios para importarlo.", newEmployee.AlphaIdentification));
                            continue;
                        }
                        // gather statistics
                        switch (newEmployee.EmployeeStatus[0])
                        {
                            case 'A':
                                summary.Hires++;
                                break;
                            case 'B':
                                summary.Layoffs++;
                                break;
                            case 'R':
                                summary.Readmissions++;
                                break;
                        }
                        /*
                        // Si vamos a usar el archivo para dupont tendremos que hacer algunos ajustes
                        if (importTechnicalEmployees.Checked == true)
                        {
                            if (newEmployee.EmployeeStatus == "B")
                            {
                                newEmployee.TerminationDate = employeeFields[42].Substring(0, 11);
                                // si es una baja hay un campo adicional de la fecha de terminacion asi que corremos un espacio para los demas campos
                                newEmployee.NewID = employeeFields[43];
                                newEmployee.Company = employeeFields[44];
                            }
                            else
                            {
                                newEmployee.NewID = employeeFields[42];
                                newEmployee.Company = employeeFields[43];
                            }
                            newEmployee.Identification = employees[employeeIndex].NewID;
                        }
                        else*/
                        {
                            if (newEmployee.EmployeeStatus == "B")
                                newEmployee.TerminationDate = employeeRecord.Field<DateTime?>("FCH_BAJA") ?? DateTime.MinValue;
                            else
                                newEmployee.TerminationDate = DateTime.MinValue;
                            newEmployee.NewID = "";
                            newEmployee.Company = "";
                        }
                    }

                    employees.Add(newEmployee);
                    // Incrementamos el indice de los empleados para el siguiente que encontremos bien.
                    employeeIndex = employeeIndex + 1;
                }
                //progressBar1.PerformStep();
                //progressBar1.Refresh();
            }

            summary.ImportedEmployees = employees.Count;
            //messageTextbox.AddInput("Ultimo empleado: " + employees[TotalEmployees - 1].Identification);
            summary.LoadSuccessful = true;
            //messageTextbox.AddInput("del cargados en memoria.");
            return summary;
        }


    }

    enum KardexTypes
    {
        Hire = 17,   // Alta
        Rehire = 18,   // Reingreso
        Termination = 19,   // Baja
        ShiftChange = 20,   // Cambio de turno
        Departamentchange = 21,   // Cambio de departamento
        PositionChange = 22,   // Cambio de Puesto
        ClassificationChange = 23,   // Cambio de clasificacion
        AreaChange = 24,   // Cambio de Area
        CostcenterChange = 25,   // Cambio de Centro de Costo
        Amonestacion = 26    // Amonestacion a Empleados por personal de seguridad

    }

}
