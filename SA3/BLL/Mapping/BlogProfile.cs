using AutoMapper;
using SA3.Core.DTOs;
using SA3.Core.Entities;

namespace SA3.BLL.Mapping
{
    // Клас Profile — це частина AutoMapper. Тут ми задаємо правила перетворення.
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            // CreateMap<Джерело, КудиКопіювати>. 
            // ReverseMap() означає, що маппінг працює в обидві сторони.
            CreateMap<Article, ArticleDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}