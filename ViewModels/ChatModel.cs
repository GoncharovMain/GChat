#nullable disable
using GChat.Models;

namespace GChat.ViewModels
{
    public class ChatModel
    {
        public string Name { get; set; }
        public List<User> Members { get; set; }
    }
}