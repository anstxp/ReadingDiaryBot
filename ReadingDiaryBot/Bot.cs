using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Args;
using ReadingDiaryBot.Model;
using ReadingDiaryBot.Method_API;

namespace ReadingDiaryBot
{
    public class Bot
    {
        TelegramBotClient botClient = new TelegramBotClient("6186921563:AAF821Q8iQXtzgIpDPMHjkKyCeBqBjL_FqE");
        CancellationToken cancellationToken = new CancellationToken();
        ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
        string CMessage;
        Message PMessage;
        Method method = new Method();
        UserInput userInput = new UserInput();
        BookDB searchresult;
        public int index = 0;
        public async Task Start()
        {
            botClient.StartReceiving(HandlerUpdateAsync, HandlerError, receiverOptions, cancellationToken);
            var BotMe = await botClient.GetMeAsync();
            Console.WriteLine($"Бот {BotMe.Username} почав працювати");
            Console.ReadKey();
        }

        private Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErorMassage = exception switch
            {
                ApiRequestException apiRequestException => $"Помилка в телеграм бот API:\n {apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErorMassage);
            return Task.CompletedTask;
        }
        public long GetChatId(Message message)
        {
            string chatID = $"{message.Chat.Id}";
            return message.Chat.Id;
        }
        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandlerMessage(botClient, CMessage, PMessage, update.Message);
                if (update.Message.Text == "Пошук відео YouTube" || update.Message.Text == "🔍 Пошук книги" || update.Message.Text == "Пошук за автором" || update.Message.Text == "Знайти книгу в щоденнику" || update.Message.Text == "Додати книгу в щоденник" || update.Message.Text == "Видалити книгу зі щоденника" || update.Message.Text == "Редагувати опис книги" || update.Message.Text == "Редагувати теги книги" || update.Message.Text == "Показати всі книги")
                    CMessage = update.Message.Text;
                PMessage = update.Message;
            }
            if (update.Type == UpdateType.CallbackQuery)
            {
                var callbackQuery = update.CallbackQuery;
                await HandlerCallbackQuery(botClient, callbackQuery);
                return;
            }
        }

        private async Task HandlerMessage(ITelegramBotClient botClient, string cmessage, Message pmessage, Message message)
        { 
            if (message.Text == "/start")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new
                    (
                    new[]
                    {
                        new KeyboardButton [] {"Пошук за автором","🔍 Пошук книги",  "Пошук відео YouTube"},
                        new KeyboardButton [] { "Знайти книгу в щоденнику", "Додати книгу в щоденник", "Показати всі книги"},
                        new KeyboardButton [] { "Видалити книгу зі щоденника", "Редагувати опис книги", "Редагувати теги книги" }
                    }
                    )
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Виберіть команду:", replyMarkup: replyKeyboardMarkup);
                return;
            }

            if (message.Text == "Пошук відео YouTube")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть ключові слова"); 
                return;
            }
            if (message.Text == "Пошук за автором")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть назву книги, після чого введіть автора книги");
                return;
            }
            if (message.Text == "🔍 Пошук книги")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть назву книги, яку бажаєте знайти");
                return;
            }
            if (message.Text == "Знайти книгу в щоденнику")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть назву книги, яку бажаєте знайти в щоденнику");
                return;
            }
            if (message.Text == "Додати книгу в щоденник")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть опис книги, а в наступному повідомленні теги через кому");
                return;
            }
            if (message.Text == "Видалити книгу зі щоденника")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Яку книгу ви бажаєте видалити?");
                return;
            }
            if (message.Text == "Редагувати опис книги")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "В першому повідомленні введіть книгу, опис якої бажаєте редагувати, в наступному новий опис");
                return;
            }
            if (message.Text == "Редагувати теги книги")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "В першому повідомленні введіть книгу, теги якої бажаєте редагувати, в наступному нові теги");
                return;
            }
            if (message.Text == "Показати всі книги")
            {
                await method.GetAll(botClient, message);
                return;
            }
            if (cmessage == "🔍 Пошук книги" && message.Text != "🔍 Пошук книги")
            {
                index = 0;
                userInput.SearchBook = message.Text;
                searchresult = await method.NameBooks(botClient, message, userInput.SearchBook, index);
                return;
            }
            if (cmessage == "Пошук за автором" && pmessage.Text != "Пошук за автором" && message.Text != "Пошук за автором")
            {
                userInput.SearchBook = pmessage.Text;
                userInput.Author = message.Text;
                index = 0;
                searchresult = await method.NameAuthorBooks(botClient, message, userInput.SearchBook, userInput.Author, index);
                return;
            }
            if (cmessage == "Пошук відео YouTube" && message.Text != "Пошук відео YouTube")
            {
                await method.YouTubeSearch(botClient, message, message.Text);
                return;
            }
            if (cmessage == "Знайти книгу в щоденнику" && message.Text != "Знайти книгу в щоденнику")
            {
                string chatID = $"{message.Chat.Id}";
                await method.GetBookDB(botClient, message, chatID, message.Text);
                return;
            }
            if (cmessage == "Додати книгу в щоденник" && pmessage.Text != "Додати книгу в щоденник" && message.Text != "Додати книгу в щоденник")
            {
                userInput.PersonalDescription = pmessage.Text;
                string Tags = message.Text;
                userInput.PersonalTags = Tags.Split(',').Select(item => item.Trim()).ToList();
                await method.PostBookDB(botClient, message, searchresult, userInput.PersonalDescription, userInput.PersonalTags);
                return;
            }
            if (cmessage == "Видалити книгу зі щоденника" && message.Text != "Видалити книгу зі щоденника")
            {
                await method.DeleteBookDB(botClient, message, message.Text);
                return;
            }
            if (cmessage == "Редагувати опис книги" && pmessage.Text != "Редагувати опис книги" && message.Text != "Редагувати опис книги")
            {
                userInput.SearchBook = pmessage.Text;
                userInput.PersonalDescription = message.Text;
                await method.UpdateDescription(botClient, message, userInput.SearchBook, userInput.PersonalDescription);
                return;
            }
            if (cmessage == "Редагувати теги книги" && pmessage.Text != "Редагувати теги книги" && message.Text != "Редагувати теги книги")
            {
                userInput.SearchBook = pmessage.Text;
                string Tags = message.Text;
                userInput.PersonalTags = Tags.Split(',').Select(item => item.Trim()).ToList();
                await method.UpdateTags(botClient, message, userInput.SearchBook, userInput.PersonalTags);
                return;
            }
        }
        private async Task HandlerCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data == "nextBookSearch")
            {
                var message = callbackQuery.Message;
                await method.NameBooks(botClient, message, userInput.SearchBook, ++index);
                InlineKeyboardMarkup updatedKeyboard = new InlineKeyboardMarkup(
                    new[]
                    {
                InlineKeyboardButton.WithCallbackData("Пошук наступної книги")
                    });

                await botClient.EditMessageReplyMarkupAsync(message.Chat.Id, message.MessageId, updatedKeyboard);
            }
            if (callbackQuery.Data == "nextBookAuthorSearch")
            {
                var message = callbackQuery.Message;
                await method.NameAuthorBooks(botClient, message, userInput.SearchBook,userInput.Author, ++index);
                InlineKeyboardMarkup updatedKeyboard = new InlineKeyboardMarkup(
                    new[]
                    {
                InlineKeyboardButton.WithCallbackData("Пошук наступної книги")
                    });

                await botClient.EditMessageReplyMarkupAsync(message.Chat.Id, message.MessageId, updatedKeyboard);
            }
        }
    }
}