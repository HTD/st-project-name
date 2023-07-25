namespace st_project_name;

internal class IocFile {

    public string Path { get; private set; }

    public string Name { get; private set; }

    public string FileName { get; private set; }

    public IocFile(string path) {
        Path = path;
        Name = System.IO.Path.GetFileNameWithoutExtension(Path);
        FileName = System.IO.Path.GetFileName(Path);
    }

    public void Rename(string newName) {
        string dir = System.IO.Path.GetDirectoryName(Path)!;
        string newPath = System.IO.Path.Combine(dir, newName + ".ioc");
        File.Move(Path, newPath);
        Path = newPath;
        Name = newName;
        FileName = System.IO.Path.GetFileName(Path);
    }

}