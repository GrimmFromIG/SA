using System;

namespace SA3.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        void Save();
    }
}