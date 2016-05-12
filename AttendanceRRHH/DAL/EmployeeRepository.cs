using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AttendanceRRHH.Models;
using System.Data.Entity;

namespace AttendanceRRHH.DAL
{
    public class EmployeeRepository : IEmployeeRepository, IDisposable
    {
        private AttendanceContext context;

        public EmployeeRepository()
        {
            context = new AttendanceContext();
        }

        public EmployeeRepository(AttendanceContext context)
        {
            this.context = context;
        }

        public void Delete(int employeId)
        {
            Employee e = context.Employees.Find(employeId);
            context.Employees.Remove(e);
        }

        public Employee GetEmployeeById(int employeeId)
        {
            return context.Employees.Find(employeeId);
        }

        public void Insert(Employee employee)
        {
            context.Employees.Add(employee);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(Employee employee)
        {
            context.Entry(employee).State = EntityState.Modified;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    context.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public List<Employee> GetAll()
        {
            return context.Employees.ToList();
        }
        #endregion

    }
}