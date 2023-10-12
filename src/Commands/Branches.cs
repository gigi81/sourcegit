using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SourceGit.Commands {
    /// <summary>
    ///     解析所有的分支
    /// </summary>
    public class Branches : Command {
        private static readonly string PREFIX_LOCAL = "refs/heads/";
        private static readonly string PREFIX_REMOTE = "refs/remotes/";
        private static readonly string CMD = "branch -l --all -v --format=\"%(refname)$%(objectname)$%(HEAD)$%(upstream)$%(upstream:track)\"";
        private static readonly Regex REG_AHEAD = new Regex(@"ahead (\d+)");
        private static readonly Regex REG_BEHIND = new Regex(@"behind (\d+)");

        private List<Models.Branch> loaded = new List<Models.Branch>();

        public Branches(string path) {
            Cwd = path;
            Args = CMD;
        }

        public List<Models.Branch> Result() {
            Exec();
            return loaded;
        }

        public override void OnReadline(string line) {
            var parts = line.Split('$');
            if (parts.Length != 5) return;

            var branch = new Models.Branch();
            var refName = parts[0];
            if (refName.EndsWith("/HEAD")) return;

            if (refName.StartsWith(PREFIX_LOCAL, StringComparison.Ordinal)) {
                branch.Name = refName.Substring(PREFIX_LOCAL.Length);
                branch.IsLocal = true;
            } else if (refName.StartsWith(PREFIX_REMOTE, StringComparison.Ordinal)) {
                var name = refName.Substring(PREFIX_REMOTE.Length);
                var shortNameIdx = name.IndexOf('/');
                if (shortNameIdx < 0) return;

                branch.Remote = name.Substring(0, shortNameIdx);
                branch.Name = name.Substring(branch.Remote.Length + 1);
                branch.IsLocal = false;
            } else {
                branch.Name = refName;
                branch.IsLocal = true;
            }

            branch.FullName = refName;
            branch.Head = parts[1];
            branch.IsCurrent = parts[2] == "*";
            branch.Upstream = parts[3];
            branch.UpstreamTrackStatus = ParseTrackStatus(parts[4]);

            loaded.Add(branch);
        }

        private string ParseTrackStatus(string data) {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            string track = string.Empty;

            var ahead = REG_AHEAD.Match(data);
            if (ahead.Success) {
                track += ahead.Groups[1].Value + "↑ ";
            }

            var behind = REG_BEHIND.Match(data);
            if (behind.Success) {
                track += behind.Groups[1].Value + "↓";
            }

            return track.Trim();
        }
    }
}
