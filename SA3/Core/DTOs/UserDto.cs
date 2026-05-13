namespace SA3.Core.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsRegistered { get; set; } // <--- Саме цього поля не вистачало
    }
}