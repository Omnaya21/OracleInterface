using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OracleInterface
{
    public class  Summary
    {
        public  int ImportedEmployees;
        public  int Hires;
        public  int Layoffs;
        public  int Readmissions;
        public  string StatusMessage;
        public  bool LoadSuccessful;

        public  Summary()
        {
            ImportedEmployees = 0;
            Hires = 0;
            Layoffs = 0;
            Readmissions = 0;
            StatusMessage = "";
            LoadSuccessful = false;
        }
    }
}
