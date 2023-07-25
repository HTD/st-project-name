using System.Text.RegularExpressions;

namespace st_project_name;

internal class PathBuilder {

    public string Target {
        get => TargetIndex < 0 ? String.Empty : Parts[TargetIndex];
        set {
            if (TargetIndex < 0) return;
            Parts[TargetIndex] = value;
        }
    }

    public PathBuilder(string path, params string[] specialPatterns) {
        Separator = path.Contains('\\') ? '\\' : '/';
        Parts = path.Split(Separator, StringSplitOptions.None);
        Patterns = specialPatterns;
        TargetIndex = -1;
        for (int i = 0, n = Parts.Length; i < n; i++) {
            if (IsSpecial(Parts[i])) continue;
            TargetIndex = i;
            break;
        }
    }

    public PathBuilder RenameTarget(string newName) {
        if (TargetIndex < 0) return this;
        string extension = Path.GetExtension(Target);
        Target = extension.Length > 0 ? (newName + extension) : newName;
        return this;
    }

    public override string ToString() => String.Join(Separator, Parts);

    private bool IsSpecial(string part) {
        if (part is "." or "..") return true;
        foreach (string pattern in Patterns) if (Regex.IsMatch(part, pattern)) return true;
        return false;
    }

    private readonly char Separator;
    private readonly string[] Parts;
    private readonly string[] Patterns;
    private readonly int TargetIndex;

}