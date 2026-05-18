using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SA3.Core.DTOs;
using SA3.Core.Entities;
using SA3.Core.Interfaces;

namespace SA3.BLL.Services
{
    public class BlogService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public BlogService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public IEnumerable<ArticleDto> GetAllArticles()
        {
            var articles = _uow.GetRepository<Article>().GetAll();
            var users = _uow.GetRepository<User>().GetAll();
            var categories = _uow.GetRepository<Category>().GetAll();
            
            var articleDtos = _mapper.Map<IEnumerable<ArticleDto>>(articles).ToList();

            foreach (var dto in articleDtos)
            {
                var author = users.FirstOrDefault(u => u.Id == dto.AuthorId);
                dto.AuthorName = author != null ? author.Username : "Невідомий автор";

                var category = categories.FirstOrDefault(c => c.Id == dto.CategoryId);
                dto.CategoryName = category != null ? category.Name : "Без категорії";
            }

            return articleDtos;
        }

        public void CreateArticle(ArticleDto articleDto)
        {
            var user = _uow.GetRepository<User>().GetById(articleDto.AuthorId);
            if (user == null || !user.IsRegistered)
            {
                throw new UnauthorizedAccessException("Тільки зареєстровані користувачі можуть додавати статті.");
            }

            var articleEntity = _mapper.Map<Article>(articleDto);
            _uow.GetRepository<Article>().Add(articleEntity);
            _uow.Save();
        }

        public void AddComment(CommentDto commentDto)
        {
            var user = _uow.GetRepository<User>().GetById(commentDto.AuthorId);
            if (user == null || !user.IsRegistered)
            {
                throw new UnauthorizedAccessException("Тільки зареєстровані користувачі можуть залишати коментарі.");
            }

            var commentEntity = _mapper.Map<Comment>(commentDto);
            _uow.GetRepository<Comment>().Add(commentEntity);
            _uow.Save();
        }

        public UserDto RegisterUser(string username)
        {
            var newUser = new User { Username = username, IsRegistered = true };
            _uow.GetRepository<User>().Add(newUser);
            _uow.Save();
            
            return _mapper.Map<UserDto>(newUser);
        }
        
        public IEnumerable<CategoryDto> GetAllCategories()
        {
            var categories = _uow.GetRepository<Category>().GetAll();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public IEnumerable<UserDto> GetAllUsers()
        {
            var users = _uow.GetRepository<User>().GetAll();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public IEnumerable<CommentDto> GetAllComments()
        {
            var comments = _uow.GetRepository<Comment>().GetAll();
            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }
    }
}