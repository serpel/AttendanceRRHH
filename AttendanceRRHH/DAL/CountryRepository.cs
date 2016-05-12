using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AttendanceRRHH.Models;
using System.Data.Entity;

namespace AttendanceRRHH.DAL
{
    public class CountryRepository : ICountryRepository, IDisposable
    {
        private AttendanceContext context;

        public CountryRepository():this(new AttendanceContext()){ }

        public CountryRepository(AttendanceContext context)
        {
            this.context = context;
        }

        public Country GetCountryById(int countryId)
        {
            return context.Countries.Find(countryId);
        }

        public IList<Country> GetCountryList()
        {
            return context.Countries.ToList();
        }

        public void Insert(Country country)
        {
            context.Countries.Add(country);
        }

        public void Remove(int countryId)
        {
            Country country = context.Countries.Find(countryId);
            context.Countries.Remove(country);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(Country country)
        {
            context.Entry(country).State = EntityState.Modified;
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}