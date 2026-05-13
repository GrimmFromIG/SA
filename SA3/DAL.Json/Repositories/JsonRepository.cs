using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SA3.Core.Interfaces;

namespace SA3.DAL.Json.Repositories
{
    public class JsonRepository<T> : IRepository<T> where T : class
    {
        private readonly List<T> _dataList;

        public JsonRepository(List<T> dataList)
        {
            _dataList = dataList;
        }

        public IEnumerable<T> GetAll()
        {
            return _dataList;
        }

        public T GetById(int id)
        {
            var property = typeof(T).GetProperty("Id");
            return _dataList.FirstOrDefault(x => (int)property.GetValue(x) == id);
        }

        public void Add(T entity)
        {
            var property = typeof(T).GetProperty("Id");
            int maxId = _dataList.Any() ? _dataList.Max(x => (int)property.GetValue(x)) : 0;
            property.SetValue(entity, maxId + 1);
            
            _dataList.Add(entity);
        }

        public void Delete(T entity)
        {
            _dataList.Remove(entity);
        }
    }
}