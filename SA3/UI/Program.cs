using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SA3.Core.DTOs;
using SA3.Core.Entities;
using SA3.Core.Interfaces;
using SA3.DAL.Json;
using SA3.BLL.Services;
using SA3.BLL.Mapping;

namespace SA3.UI
{
    class Program
    {
        // Зберігаємо поточного користувача (Імітація сесії/авторизації)
        static UserDto CurrentUser;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            var serviceProvider = ConfigureServices();
            var blogService = serviceProvider.GetRequiredService<BlogService>();

            // Генеруємо початкові дані, якщо файл порожній
            SeedData(serviceProvider);

            // КРОК 1: АВТОРИЗАЦІЯ
            LoginScreen(blogService);

            // КРОК 2: ГОЛОВНЕ МЕНЮ
            while (true)
            {
                Console.WriteLine("\n=============================");
                Console.WriteLine($" БЛОГ | Ви увійшли як: {CurrentUser.Username} (Зареєстрований: {CurrentUser.IsRegistered})");
                Console.WriteLine("=============================");
                Console.WriteLine("1. Переглянути всі статті");
                Console.WriteLine("2. Читати статтю та коментарі (по ID)");
                Console.WriteLine("3. Написати нову статтю");
                Console.WriteLine("4. Змінити користувача");
                Console.WriteLine("5. Вихід");
                Console.Write("Ваш вибір: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowArticles(blogService);
                        break;
                    case "2":
                        ReadArticleMenu(blogService);
                        break;
                    case "3":
                        TryAddArticle(blogService);
                        break;
                    case "4":
                        LoginScreen(blogService);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("[-] Невірний вибір.");
                        break;
                }
            }
        }

        // --- ЛОГІКА АВТОРИЗАЦІЇ ---
        private static void LoginScreen(BlogService blogService)
        {
            var users = blogService.GetAllUsers().ToList();
            
            Console.WriteLine("\n--- Оберіть акаунт для входу ---");
            foreach (var u in users)
            {
                string role = u.IsRegistered ? "Зареєстрований" : "Гість";
                Console.WriteLine($"[{u.Id}] {u.Username} ({role})");
            }
            
            while (true)
            {
                Console.Write("Введіть ID користувача: ");
                if (int.TryParse(Console.ReadLine(), out int userId))
                {
                    var user = users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        CurrentUser = user;
                        Console.WriteLine($"[+] Успішний вхід. Вітаємо, {CurrentUser.Username}!");
                        return;
                    }
                }
                Console.WriteLine("[-] Користувача з таким ID не знайдено.");
            }
        }

        // --- ЛОГІКА СТАТЕЙ ТА КАТЕГОРІЙ ---
        private static void ShowArticles(BlogService blogService)
        {
            var articles = blogService.GetAllArticles();
            if (!articles.Any())
            {
                Console.WriteLine("\n[-] Статей ще немає. Напишіть першу!");
                return;
            }

            Console.WriteLine("\n--- СПИСОК СТАТЕЙ ---");
            foreach (var art in articles)
            {
                Console.WriteLine($"[ID: {art.Id}] \"{art.Title}\" | Автор: {art.AuthorName} | Рубрика: {art.CategoryName}");
            }
        }

        private static void TryAddArticle(BlogService blogService)
        {
            try
            {
                Console.WriteLine("\n--- ДОДАВАННЯ СТАТТІ ---");
                
                // Вибір категорії
                var categories = blogService.GetAllCategories().ToList();
                Console.WriteLine("Доступні рубрики:");
                foreach (var c in categories) Console.WriteLine($"[{c.Id}] {c.Name}");
                
                Console.Write("Введіть ID рубрики: ");
                int catId = int.Parse(Console.ReadLine() ?? "1");

                Console.Write("Введіть заголовок: ");
                string title = Console.ReadLine();
                
                Console.Write("Введіть текст статті: ");
                string content = Console.ReadLine();

                var newArticle = new ArticleDto
                {
                    Title = title,
                    Content = content,
                    AuthorId = CurrentUser.Id, // Беремо ID того, хто зараз увійшов
                    CategoryId = catId
                };

                blogService.CreateArticle(newArticle); // Тут BLL перевірить, чи зареєстрований юзер
                Console.WriteLine("[+] Статтю успішно опубліковано!");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"\n[ПОМИЛКА ДОСТУПУ]: {ex.Message}");
            }
            catch (Exception)
            {
                Console.WriteLine("\n[-] Помилка вводу даних.");
            }
        }

        // --- ЛОГІКА КОМЕНТАРІВ (ПІДМЕНЮ СТАТТІ) ---
        private static void ReadArticleMenu(BlogService blogService)
        {
            Console.Write("\nВведіть ID статті для читання: ");
            if (!int.TryParse(Console.ReadLine(), out int articleId)) return;

            var article = blogService.GetAllArticles().FirstOrDefault(a => a.Id == articleId);
            if (article == null)
            {
                Console.WriteLine("[-] Статтю не знайдено.");
                return;
            }

            while (true)
            {
                Console.WriteLine($"\n=== {article.Title.ToUpper()} ===");
                Console.WriteLine($"Рубрика: {article.CategoryName} | Автор: {article.AuthorName}");
                Console.WriteLine("----------------------------------");
                Console.WriteLine(article.Content);
                Console.WriteLine("----------------------------------");
                
                PrintComments(blogService, articleId);

                Console.WriteLine("\nДії: [1] Додати коментар | [2] Відповісти на коментар | [3] Повернутися");
                Console.Write("Ваш вибір: ");
                var choice = Console.ReadLine();

                try
                {
                    if (choice == "1")
                    {
                        Console.Write("Текст коментаря: ");
                        string text = Console.ReadLine();
                        blogService.AddComment(new CommentDto { Text = text, ArticleId = articleId, AuthorId = CurrentUser.Id });
                        Console.WriteLine("[+] Коментар додано!");
                    }
                    else if (choice == "2")
                    {
                        Console.Write("Введіть ID коментаря, на який хочете відповісти: ");
                        int parentId = int.Parse(Console.ReadLine() ?? "0");
                        Console.Write("Текст відповіді: ");
                        string text = Console.ReadLine();
                        blogService.AddComment(new CommentDto { Text = text, ArticleId = articleId, ParentCommentId = parentId, AuthorId = CurrentUser.Id });
                        Console.WriteLine("[+] Відповідь додано!");
                    }
                    else if (choice == "3")
                    {
                        return; // Вихід з підменю
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"\n[ПОМИЛКА ДОСТУПУ]: {ex.Message}");
                }
                catch (Exception)
                {
                    Console.WriteLine("\n[-] Помилка вводу.");
                }
            }
        }

        private static void PrintComments(BlogService blogService, int articleId)
        {
            var allComments = blogService.GetAllComments().Where(c => c.ArticleId == articleId).ToList();
            var users = blogService.GetAllUsers().ToList();

            Console.WriteLine("\n--- Коментарі ---");
            if (!allComments.Any()) Console.WriteLine("Коментарів ще немає.");

            foreach (var c in allComments)
            {
                var authorName = users.FirstOrDefault(u => u.Id == c.AuthorId)?.Username ?? "Невідомий";
                
                if (c.ParentCommentId == null)
                {
                    Console.WriteLine($"[ID: {c.Id}] {authorName}: {c.Text}");
                }
                else
                {
                    Console.WriteLine($"    -> [ID: {c.Id}] {authorName} (Відповідь на #{c.ParentCommentId}): {c.Text}");
                }
            }
        }

        // --- НАЛАШТУВАННЯ ТА СІДИНГ ---
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(cfg => { cfg.AddProfile<BlogProfile>(); });
            services.AddSingleton<IUnitOfWork, JsonUnitOfWork>();
            services.AddTransient<BlogService>();
            return services.BuildServiceProvider();
        }

        private static void SeedData(ServiceProvider provider)
        {
            var uow = provider.GetRequiredService<IUnitOfWork>();
            var usersRepo = uow.GetRepository<User>();
            var categoriesRepo = uow.GetRepository<Category>();

            if (!usersRepo.GetAll().Any())
            {
                usersRepo.Add(new User { Username = "Олександр (Зареєстровано)", IsRegistered = true });
                usersRepo.Add(new User { Username = "Іван (Гість)", IsRegistered = false });
                categoriesRepo.Add(new Category { Name = "Технології" });
                categoriesRepo.Add(new Category { Name = "Новини" });
                uow.Save();
            }
        }
    }
}