namespace SA3.Core.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public int? ArticleId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}