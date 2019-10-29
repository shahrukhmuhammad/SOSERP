using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS
{
    public interface IEmployee
    {
        //Employee GetByEmployeeId(int employeeId);
        List<Employee> GetAllEmployees();
    }
}
