using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using ReadingDiaryBot.Model;
using Telegram.Bot.Types.ReplyMarkups;
using System.Reflection;
using System.Text;
using System;

namespace ReadingDiaryBot.Method_API
{
    public class Method
    {
        private HttpClient _client;
        private static string _address = "https://readingdiaryapi1.azure-api.net";
        private static string _key = "8cd544cb70b24938bfbf9fb155c6c8d7";
        public BookDB searchresult;
        public Method()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_address);
        }
        public async Task<BookDB> NameBooks(ITelegramBotClient botClient, Message message, string name, int index)
        {
            try
            {
                var response = await _client.GetAsync($"/GetBook?name={name}&bookIndex={index}&key={_key}");
                var content = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Book>(content);
                if (result != null )
                {
                    int pageCount = result.volumeInfo.printedPageCount == 0 ? result.volumeInfo.pageCount : result.volumeInfo.printedPageCount;
                    BookDB searchresult = new BookDB
                    {
                        Title = result.volumeInfo.title,
                        Author = result.volumeInfo.authors,
                        Page = pageCount,
                        Genre = result.volumeInfo.categories,
                        Description = result.volumeInfo.description,
                        Link = result.volumeInfo.canonicalVolumeLink
                    };
                    string allAuthors = string.Join(", ", searchresult.Author);
                    string allCategories = string.Join(", ", searchresult.Genre);
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Назва книги: {searchresult.Title} \nАвтор: {allAuthors} \nКількість сторінок: {searchresult.Page} \nЖанр/Категорія: {allCategories} \nОпис: {searchresult.Description} \nПосилання: {searchresult.Link}");
                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(
    new[]
    {
                 InlineKeyboardButton.WithCallbackData("Пошук наступної книги", "nextBookSearch")
    });
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Пошук наступної книги", replyMarkup: keyboard);
                    return searchresult;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Такої книги не знайдено :(");
                    return null;
                }
            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Книги не знайдено :(");
                return null;
            }
        }
        public async Task YouTubeSearch(ITelegramBotClient botClient, Message message, string KeyWords)
        {
            try
            {
                var response = await _client.GetAsync($"/SearchAudioBookOnYouTube?bookTitle={KeyWords}&key={_key}");
                var content = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<YouTube>(content);
                if (result.youTubeLink != null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Посилання на відео: {result.youTubeLink}");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Такої книги не знайдено :(");
                }
            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Виникла помилка :( \nСпробуйте ще раз ");
            }
        }
        public async Task<BookDB> NameAuthorBooks(ITelegramBotClient botClient, Message message, string name, string author, int index)
        {
            try
            {
                var response = await _client.GetAsync($"/GetAuthorBook?name={name}&author={author}&bookIndex={index}&key={_key}");
                var content = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Book>(content);
                if (result != null)
                {
                    int pageCount = result.volumeInfo.printedPageCount == 0 ? result.volumeInfo.pageCount : result.volumeInfo.printedPageCount;
                    BookDB searchresult = new BookDB
                    {
                        Title = result.volumeInfo.title,
                        Author = result.volumeInfo.authors,
                        Page = pageCount,
                        Genre = result.volumeInfo.categories,
                        Description = result.volumeInfo.description,
                        Link = result.volumeInfo.canonicalVolumeLink
                    };
                    string allAuthors = string.Join(", ", searchresult.Author);
                    string allCategories = string.Join(", ", searchresult.Genre);
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Назва книги: {searchresult.Title} \nАвтор: {allAuthors} \nКількість сторінок: {searchresult.Page} \nЖанр/Категорія: {allCategories} \nОпис: {searchresult.Description} \nПосилання: {searchresult.Link}");
                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(
                     new[]
                     {
                 InlineKeyboardButton.WithCallbackData("Пошук наступної книги", "nextBookSearch")
                     });
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Пошук наступної книги", replyMarkup: keyboard);
                    return searchresult;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Такої книги не знайдено :(");
                    return null;
                }
            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Книги не знайдено :(");
                return null;
            }
        }
        public async Task GetBookDB(ITelegramBotClient botClient, Message message, string Title)
        {
            try
            {
                string UserID = message.From.Id.ToString();
                var response = await _client.GetAsync($"/GetBookDB?UserID={UserID}&Title={Title}&key={_key}");
                var content = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<BookDB>(content);
                if (result != null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Назва: {result.Title}\nАвтор: {string.Join(", ", result.Author)}\nКількість сторінок: {result.Page}\nЖанр: {string.Join(", ", result.Genre)}\nОпис: {result.Description}\nПосилання: {result.Link}\nЗамітки: {result.PersonalDescription}\nТеги: {string.Join(", ", result.PersonalTag)}");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Такої книги не знайдено :(");
                }
            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Виникла помилка :(");            }
        }
        public async Task PostBookDB(ITelegramBotClient botClient, Message message, BookDB searchresult, string PersonalDescription, List<string> PersonalTags)
        {
            try
            {
                string UserID = message.From.Id.ToString();
                BookDB requestBody = new BookDB()
                {
                    UserID = UserID,
                    Title = searchresult.Title,
                    Author = searchresult.Author,
                    Page = searchresult.Page,
                    Genre = searchresult.Genre,
                    Link = searchresult.Link,
                    Description = searchresult.Description,
                    PersonalDescription = PersonalDescription,
                    PersonalTag = PersonalTags
                };
                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);

                var response = await _client.PostAsync("/PostDynamoDb", content);

                if (response.IsSuccessStatusCode)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Книга додана в щоденник");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Виникла помилка при створенні поста.");
                }
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Виникла помилка, можливо, ви ввели неправильні дані");
            }
        }
        public async Task DeleteBookDB(ITelegramBotClient botClient, Message message, string Title)
        {
            try
            {   
                if  (await ContainsBook(botClient,message,Title))
                {
                    string UserID = message.From.Id.ToString();
                    var response = await _client.DeleteAsync($"/DeleteBookDB?UserID={UserID}&Title={Title}&key={_key}");
                    if (response.IsSuccessStatusCode)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Книга успішно видалена зі щоденника");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Такої книги ще немає в щоденнику");
                }
            }
            catch (Exception ex) 
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Виникла помилка :(");
            }
        }
        public async Task UpdateDescription(ITelegramBotClient botClient, Message message, string Title, string Description)            
        {
            try
            {
                if (await ContainsBook(botClient, message, Title))
                {
                    string UserID = message.From.Id.ToString();
                    var response = await _client.PutAsync($"/UpdateDescription?UserID={UserID}&Title={Title}&Description={Description}&key={_key}", null);
                    if (response.IsSuccessStatusCode)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Книга успішно оновлена");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Такої книги ще немає в щоденнику :(");

                }
            }
            catch
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Виникла помилка :(");
            }
        }
        public async Task UpdateTags(ITelegramBotClient botClient, Message message, string Title, List<string> Tags)
        {
            try
            {
                if (await ContainsBook(botClient, message, Title))
                {
                    string UserID = message.From.Id.ToString();
                    var url = $"/UpdateTag?UserID={UserID}&Title={Title}&key={_key}";
                    var data = Tags;
                    var json = JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _client.PutAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Книга успішно оновлена");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Такої книги ще немає в щоденнику :(");
                }
            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Виникла помилка :(");
            }
        }
        public async Task GetAll(ITelegramBotClient botClient, Message message)
        {
            try
            {
                string UserID = message.From.Id.ToString();
                var url = $"GetAllDynamoDb?UserId={UserID}&key={_key}";
                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var booksList = JsonConvert.DeserializeObject<List<BookDB>>(jsonString);
                    foreach (var book in booksList)
                    {
                        var bookInfo = $"Назва: {book.Title}\nАвтор: {string.Join(", ", book.Author)}\nКількість сторінок: {book.Page}\nЖанр: {string.Join(", ", book.Genre)}\nОпис: {book.Description}\nПосилання: {book.Link}\nЗамітки: {book.PersonalDescription}\nТеги: {string.Join(", ", book.PersonalTag)}";
                        await botClient.SendTextMessageAsync(message.Chat.Id, bookInfo);
                    }
                }
                else
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Виникла помилка при отриманні списку книг");
            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "помилка");
            }
        }
        public async Task<bool> ContainsBook(ITelegramBotClient botClient, Message message, string Title)
        {
            try
            {
                string UserID = message.From.Id.ToString();
                var response = await _client.GetAsync($"/GetBookDB?UserID={UserID}&Title={Title}&key={_key}");
                var content = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<BookDB>(content);
                if (result != null)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
