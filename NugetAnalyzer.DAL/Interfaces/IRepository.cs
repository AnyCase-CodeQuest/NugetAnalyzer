using System;
using System.Collections.Generic;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        void Add(T item);

        IEnumerable<T> Get(Func<T, bool> predicate);

        IEnumerable<T> GetAll();

        void Delete(int id);

        void Update(T item);
    }
}