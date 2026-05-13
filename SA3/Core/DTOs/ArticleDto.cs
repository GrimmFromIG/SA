namespace SA3.Core.DTOs
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }   // <--- Додано
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // <--- Додано
    }
}