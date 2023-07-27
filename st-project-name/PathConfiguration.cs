using System.Text.RegularExpressions;

namespace st_project_name;

/// <summary>
/// A path container allowing easily changing a part of the path.
/// </summary>
internal class PathConfiguration : ICloneable {

    /// <summary>
    /// Gets a value indicating that the container contains a path.
    /// </summary>
    public bool Exists { get; }

    /// <summary>
    /// Gets or sets the target part of the path.
    /// </summary>
    public string Target {
        get => TargetIndex < 0 ? String.Empty : Parts[TargetIndex];
        set {
            if (TargetIndex < 0) return;
            Parts[TargetIndex] = value;
        }
    }

    /// <summary>
    /// Creates a path container.
    /// </summary>
    /// <param name="path">Input path.</param>
    /// <param name="specialPatterns">Optional regular expression patterns matching special directories that should not be modified.</param>
    public PathConfiguration(string path, params string[] specialPatterns) {
        Exists = path.Length > 0;
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

    /// <summary>
    /// Creates a copy of a <see cref="PathConfiguration"/> object.
    /// </summary>
    /// <param name="other"></param>
    private PathConfiguration(PathConfiguration other) {
        Exists = other.Exists;
        Separator = other.Separator;
        Parts = (string[])other.Parts.Clone();
        Patterns = (string[])other.Patterns.Clone();
        TargetIndex = other.TargetIndex;
    }

    /// <summary>
    /// If the path contains target (a part that can be modified), replaces it with the new name.
    /// </summary>
    /// <param name="newName">New name, with or without extension. If the new name doesn't contain extension, the existing extension will be added.</param>
    /// <returns>Modified container.</returns>
    public PathConfiguration RenameTarget(string newName) {
        if (TargetIndex < 0 || newName.Length < 1) return this;
        if (newName.Contains('.')) Target = newName; // newName contains extension
        else { // newName doesn't contain extension
            string extension = Path.GetExtension(Target);
            Target = extension.Length > 0 ? (newName + extension) : newName;
        }
        return this;
    }

    /// <summary>
    /// Returns the path container as plain string.
    /// </summary>
    /// <returns>Plain string path.</returns>
    public override string ToString() => Exists ? String.Join(Separator, Parts) : string.Empty;

    /// <summary>
    /// Tests if a path part is a special directory that should not be modified.
    /// </summary>
    /// <param name="part">Path part.</param>
    /// <returns>True if a path part is a special directory that should not be modified.</returns>
    private bool IsSpecial(string part) {
        if (part is "." or "..") return true;
        foreach (string pattern in Patterns) if (Regex.IsMatch(part, pattern)) return true;
        return false;
    }

    /// <summary>
    /// Implements the <see cref="ICloneable"/>.
    /// </summary>
    /// <returns>A copy of the path container.</returns>
    public object Clone() => new PathConfiguration(this);

    /// <summary>
    /// Returns a copy of the current path configuration.
    /// </summary>
    /// <returns>A copy of the path container.</returns>
    public PathConfiguration Copy() => new(this);

    private readonly char Separator;
    private readonly string[] Parts;
    private readonly string[] Patterns;
    private readonly int TargetIndex;

}