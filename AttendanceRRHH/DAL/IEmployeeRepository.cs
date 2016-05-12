using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttendanceRRHH.Models;

namespace AttendanceRRHH.DAL
{
    interface IEmployeeRepository: IDisposable
    {
        void Insert(Employee employee);
        void Update(Employee employee);
        void Delete(int employeId);
        Employee GetEmployeeById(int employeeId);
        List<Employee> GetAll();
        void Save();
    }
}
