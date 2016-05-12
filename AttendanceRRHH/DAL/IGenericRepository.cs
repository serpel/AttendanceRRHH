using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRRHH.DAL
{
    interface IGenericRepository<T> where T:class
    {
        DbSet<T> GetDbSet();
        IEnumerable<T> GetAll();
        T SelectById(object id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(object id);
        void Save();
    }
}
