namespace SA3.Core.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public int? ArticleId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}