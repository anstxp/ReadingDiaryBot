using Telegram.Bot.Types;

namespace ReadingDiaryBot.Model
{
    public class UserInput
    {
        public string SearchBook { get; set; }
        public string Author { get; set; }
        public string chatID { get; set; }
        public string PersonalDescription { get; set; }
        public List<string> PersonalTags { get; set; }
    }
}
