using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AttendanceRRHH.Models;
using System.Data.Entity;

namespace AttendanceRRHH.DAL
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private ApplicationDbContext context;
        private DbSet<T> table;
        private string user;

        public GenericRepository():this(new ApplicationDbContext()){ }

        public GenericRepository(ApplicationDbContext context)
        {
            this.table = context.Set<T>();
            this.context = context;
        }

        public GenericRepository(ApplicationDbContext context, string user)
        {
            this.table = context.Set<T>();
            this.context = context;
            this.user = user;
        }

        public void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }

        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }

        public IEnumerable<T> GetAll(Func<T, bool> predicate)
        {
            return table.Where(predicate);
        }

        public DbSet<T> GetDbSet()
        {
            return table;
        }

        public void Insert(T obj)
        {
            table.Add(obj);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public T SelectById(object id)
        {
            return table.Find(id);
        }

        public void Update(T obj)
        {
            table.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
        }
    }
}