using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SA3.Core.Entities;

namespace SA3.DAL.Json
{
    public class JsonDataContext
    {
        private readonly string _filePath = "blog_data.json";

        public List<User> Users { get; set; } = new List<User>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Article> Articles { get; set; } = new List<Article>();
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public JsonDataContext()
        {
            LoadData();
        }

        private void LoadData()
        {
            if (!File.Exists(_filePath))
            {
                SaveChanges();
                return;
            }

            var json = File.ReadAllText(_filePath);
            var data = JsonSerializer.Deserialize<JsonDataContextData>(json);
            
            if (data != null)
            {
                Users = data.Users ?? new List<User>();
                Categories = data.Categories ?? new List<Category>();
                Articles = data.Articles ?? new List<Article>();
                Comments = data.Comments ?? new List<Comment>();
            }
        }

        public void SaveChanges()
        {
            var data = new JsonDataContextData
            {
                Users = Users,
                Categories = Categories,
                Articles = Articles,
                Comments = Comments
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_filePath, json);
        }

        private class JsonDataContextData
        {
            public List<User> Users { get; set; }
            public List<Category> Categories { get; set; }
            public List<Article> Articles { get; set; }
            public List<Comment> Comments { get; set; }
        }
    }
}