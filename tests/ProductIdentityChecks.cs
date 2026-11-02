using PetTracker;
using PetTracker.Setup;

static class ProductIdentityChecks
{
    public static void Run(Action<bool, string> check, string root)
    {
        string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        check(Storage.Root(Array.Empty<string>()) == Path.Combine(local, "PetTrackerAion"), "V1 uses an independent default player-data directory");
        check(Installer.DefaultLocation == Path.Combine(local, "Programs", "PetTrackerAion"), "V1 installer does not replace the upstream installation");
        check(ProductIdentity.ShortcutName == "PetTracker Aion.lnk" && ProductIdentity.UninstallRegistryKey.EndsWith("\\PetTrackerAion"), "V1 shortcuts and uninstall registration use the fork identity");
        check(Storage.Root(new[] { "--data", root }) == Path.GetFullPath(root), "Explicit test-data paths remain compatible");
        check(Storage.Pipe(Path.Combine(local, "PetTracker")) != Storage.Pipe(Storage.Root(Array.Empty<string>())), "V1 capture IPC is separate from upstream data");
    }
}
