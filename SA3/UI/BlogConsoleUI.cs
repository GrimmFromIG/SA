using System;
using System.Linq;
using SA3.Core.DTOs;
using SA3.BLL.Services;

namespace SA3.UI
{
    public class BlogConsoleUI
    {
        private readonly BlogService _blogService;
        private UserDto _currentUser;

        public BlogConsoleUI(BlogService blogService)
        {
            _blogService = blogService;
        }

        public void Run()
        {
            AuthScreen();
            MainMenu();
        }

        private void AuthScreen()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ВІТАЄМО У СИСТЕМІ БЛОГУ ===");
                Console.WriteLine("1. Увійти в існуючий акаунт");
                Console.WriteLine("2. Зареєструвати новий акаунт");
                Console.WriteLine("3. Продовжити як Гість (тільки читання)");
                Console.Write("\nВаш вибір: ");

                var choice = Console.ReadLine();

                if (choice == "1" && TryLogin()) return;
                if (choice == "2" && TryRegister()) return;
                if (choice == "3")
                {
                    _currentUser = new UserDto { Id = 0, Username = "Гість", IsRegistered = false };
                    return;
                }
            }
        }

        private bool TryLogin()
        {
            Console.Clear();
            var users = _blogService.GetAllUsers().Where(u => u.IsRegistered).ToList();
            
            Console.WriteLine("--- ДОСТУПНІ АКАУНТИ ---");
            foreach (var u in users) Console.WriteLine($"[{u.Id}] {u.Username}");

            Console.Write("\nВведіть ваш ID (або 0 для відміни): ");
            if (int.TryParse(Console.ReadLine(), out int userId) && userId != 0)
            {
                var user = users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    _currentUser = user;
                    return true;
                }
                Console.WriteLine("[-] Користувача не знайдено.");
                Console.ReadKey(true);
            }
            return false;
        }

        private bool TryRegister()
        {
            Console.Clear();
            Console.WriteLine("--- РЕЄСТРАЦІЯ ---");
            Console.Write("Введіть ваше ім'я: ");
            string username = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(username))
            {
                _currentUser = _blogService.RegisterUser(username);
                Console.WriteLine($"\n[+] Успішно зареєстровано! Ваш ID: {_currentUser.Id}. Натисніть клавішу...");
                Console.ReadKey(true);
                return true;
            }
            return false;
        }

        private void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=====================================================");
                Console.WriteLine($" БЛОГ | Ви увійшли як: {_currentUser.Username}");
                Console.WriteLine("=====================================================");
                Console.WriteLine("1. Інтерактивна стрічка статей (Гортати статті)");
                Console.WriteLine("2. Написати нову статтю");
                Console.WriteLine("3. Змінити користувача");
                Console.WriteLine("4. Вихід");
                Console.Write("\nВаш вибір: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SelectCategoryAndRead();
                        break;
                    case "2":
                        TryAddArticle();
                        break;
                    case "3":
                        AuthScreen();
                        break;
                    case "4":
                        return;
                }
            }
        }

        private void SelectCategoryAndRead()
        {
            Console.Clear();
            var categories = _blogService.GetAllCategories().ToList();
            Console.WriteLine("--- ОБЕРІТЬ РУБРИКУ ДЛЯ ПЕРЕГЛЯДУ ---");
            foreach (var c in categories)
            {
                Console.WriteLine($"[{c.Id}] {c.Name}");
            }
            Console.Write("\nВведіть ID рубрики (або Enter для всіх): ");
            string inputIdx = Console.ReadLine();
            
            int? selectedCategoryId = null;
            if (int.TryParse(inputIdx, out int catId))
            {
                selectedCategoryId = catId;
            }

            InteractiveArticleViewer(selectedCategoryId);
        }

        private void InteractiveArticleViewer(int? categoryId)
        {
            var articles = _blogService.GetAllArticles();
            if (categoryId.HasValue)
            {
                articles = articles.Where(a => a.CategoryId == categoryId.Value);
            }

            var filteredArticles = articles.ToList();

            if (!filteredArticles.Any())
            {
                Console.WriteLine("\n[-] У цій рубриці ще немає статей. Натисніть будь-яку клавішу...");
                Console.ReadKey(true);
                return;
            }

            int currentIndex = 0;

            while (true)
            {
                Console.Clear();
                var article = filteredArticles[currentIndex];

                Console.WriteLine($"=== СТАТТЯ {currentIndex + 1} з {filteredArticles.Count} ===");
                Console.WriteLine($"Рубрика: [{article.CategoryName}]");
                Console.WriteLine($"Автор: {article.AuthorName}");
                Console.WriteLine("\n" + article.Title.ToUpper());
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(article.Content);
                Console.WriteLine("--------------------------------------------------");

                PrintComments(article.Id);

                Console.WriteLine("\n================ ПАНЕЛЬ УПРАВЛІННЯ ================");
                Console.WriteLine("[ Стрілка ВЛІВО ] - Попередня стаття");
                Console.WriteLine("[ Стрілка ВПРАВО] - Наступна стаття");
                Console.WriteLine("[ K ] - Написати коментар");
                Console.WriteLine("[ V ] - Відповісти на коментар");
                Console.WriteLine("[ ESC ] - Повернутися в головне меню");
                Console.WriteLine("===================================================");

                var keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.LeftArrow)
                    currentIndex = (currentIndex > 0) ? currentIndex - 1 : filteredArticles.Count - 1;
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                    currentIndex = (currentIndex < filteredArticles.Count - 1) ? currentIndex + 1 : 0;
                else if (keyInfo.Key == ConsoleKey.K)
                    AddCommentInteractive(article.Id, null);
                else if (keyInfo.Key == ConsoleKey.V)
                {
                    Console.Write("\nВведіть ID коментаря, на який відповідаєте: ");
                    if (int.TryParse(Console.ReadLine(), out int parentId))
                        AddCommentInteractive(article.Id, parentId);
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                    break;
            }
        }

        private void AddCommentInteractive(int articleId, int? parentId)
        {
            try
            {
                Console.Write("\nВведіть текст: ");
                string text = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    _blogService.AddComment(new CommentDto 
                    { 
                        Text = text, 
                        ArticleId = articleId, 
                        ParentCommentId = parentId, 
                        AuthorId = _currentUser.Id 
                    });
                    Console.WriteLine("[+] Збережено!");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"\n[ПОМИЛКА]: {ex.Message}");
            }
            Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey(true);
        }

        private void PrintComments(int articleId)
        {
            var allComments = _blogService.GetAllComments().Where(c => c.ArticleId == articleId).ToList();
            var users = _blogService.GetAllUsers().ToList();

            Console.WriteLine("\n--- Коментарі ---");
            if (!allComments.Any()) Console.WriteLine("Ще немає коментарів. Будьте першим!");

            foreach (var c in allComments)
            {
                var authorName = users.FirstOrDefault(u => u.Id == c.AuthorId)?.Username ?? "Невідомий";
                if (c.ParentCommentId == null)
                    Console.WriteLine($"[ID: {c.Id}] {authorName}: {c.Text}");
                else
                    Console.WriteLine($"    -> [ID: {c.Id}] {authorName} (Відповідь на #{c.ParentCommentId}): {c.Text}");
            }
        }

        private void TryAddArticle()
        {
            Console.Clear();
            Console.WriteLine("--- ДОДАВАННЯ СТАТТІ ---");
            try
            {
                var categories = _blogService.GetAllCategories().ToList();
                Console.WriteLine("Рубрики:");
                foreach (var c in categories) Console.WriteLine($"[{c.Id}] {c.Name}");
                
                Console.Write("\nВведіть ID рубрики: ");
                int catId = int.Parse(Console.ReadLine() ?? "1");

                Console.Write("Заголовок: ");
                string title = Console.ReadLine();
                
                Console.Write("Текст: ");
                string content = Console.ReadLine();

                _blogService.CreateArticle(new ArticleDto { Title = title, Content = content, AuthorId = _currentUser.Id, CategoryId = catId });
                Console.WriteLine("\n[+] Опубліковано! Натисніть клавішу...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[-] Помилка: {ex.Message}");
                Console.ReadKey();
            }
        }
    }
}