using System;

namespace TweetAutoReplier.Models
{
    public class Log
    {
        public Log(string text)
        {
            string nowStr = DateTime.Now.ToString("dd/MM HH:mm");
            Text = $"[{nowStr}] {text}";
        }
        public string Text { get; set; }
    }
}
