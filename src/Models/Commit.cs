using System;
using System.Collections.Generic;
using System.Windows;

namespace SourceGit.Models {
    /// <summary>
    ///     提交记录
    /// </summary>
    public class Commit {
        private static readonly DateTime UTC_START = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();

        public string SHA { get; set; } = string.Empty;
        public string ShortSHA => SHA.Substring(0, 8);
        public User Author { get; set; } = User.Invalid;
        public ulong AuthorTime { get; set; } = 0;
        public User Committer { get; set; } = User.Invalid;
        public ulong CommitterTime { get; set; } = 0;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public List<string> Parents { get; set; } = new List<string>();
        public List<Decorator> Decorators { get; set; } = new List<Decorator>();
        public bool HasDecorators => Decorators.Count > 0;
        public bool IsMerged { get; set; } = false;
        public Thickness Margin { get; set; } = new Thickness(0);

        public string AuthorTimeStr => UTC_START.AddSeconds(AuthorTime).ToString("yyyy-MM-dd HH:mm:ss");
        public string CommitterTimeStr => UTC_START.AddSeconds(CommitterTime).ToString("yyyy-MM-dd HH:mm:ss");
        public string AuthorTimeShortStr => UTC_START.AddSeconds(AuthorTime).ToString("yyyy/MM/dd");
        public string CommitterTimeShortStr => UTC_START.AddSeconds(CommitterTime).ToString("yyyy/MM/dd");

        public static void ParseUserAndTime(string data, ref User user, ref ulong time) {
            var userEndIdx = data.IndexOf('>');
            if (userEndIdx < 0) return;

            var timeEndIdx = data.IndexOf(' ', userEndIdx + 2);
            user = User.FindOrAdd(data.Substring(0, userEndIdx));
            time = timeEndIdx < 0 ? 0 : ulong.Parse(data.Substring(userEndIdx + 2, timeEndIdx - userEndIdx - 2));
        }
    }
}
