using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        void Add(T item);

        Task<T> GetByIdAsync(int id);

        Task<T> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicates);

        Task<IReadOnlyCollection<T>> GetAsync(Expression<Func<T, bool>> predicates);

        Task<IReadOnlyCollection<T>> GetAllAsync();

        void Delete(int id);

        void Update(T item);

        void Attach(T item);
    }
}