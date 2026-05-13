using System;
using SA3.Core.Entities;
using SA3.Core.Interfaces;
using SA3.DAL.Json.Repositories;

namespace SA3.DAL.Json
{
    public class JsonUnitOfWork : IUnitOfWork
    {
        private readonly JsonDataContext _context;
        
        private IRepository<User> _userRepository;
        private IRepository<Category> _categoryRepository;
        private IRepository<Article> _articleRepository;
        private IRepository<Comment> _commentRepository;

        public JsonUnitOfWork()
        {
            _context = new JsonDataContext();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (typeof(T) == typeof(User))
                return (IRepository<T>)(_userRepository ??= new JsonRepository<User>(_context.Users));
            
            if (typeof(T) == typeof(Category))
                return (IRepository<T>)(_categoryRepository ??= new JsonRepository<Category>(_context.Categories));
            
            if (typeof(T) == typeof(Article))
                return (IRepository<T>)(_articleRepository ??= new JsonRepository<Article>(_context.Articles));
            
            if (typeof(T) == typeof(Comment))
                return (IRepository<T>)(_commentRepository ??= new JsonRepository<Comment>(_context.Comments));

            throw new Exception("Repository type not supported");
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            
        }
    }
}