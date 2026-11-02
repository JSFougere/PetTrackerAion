param([string]$Commit = '72d409a5eef653d0a9e40637bb3854e2ee2382c8')
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
if ($Commit -notmatch '^[a-f0-9]{40}$') { throw 'Use a pinned commit SHA' }
$cache = Join-Path $root 'build/localized-names'
New-Item -ItemType Directory -Force $cache | Out-Null
$source = "https://raw.githubusercontent.com/SkeeveAN/Aion-DPS-Meter/$Commit/"
foreach ($entry in @(@('pets.json','Client/assets/aion2/pets/pets.json'), @('npcs.json','Client/assets/aion2/npcs/npc_names.json'), @('LICENSE','LICENSE'))) {
    Invoke-WebRequest ($source + $entry[1]) -OutFile (Join-Path $cache $entry[0])
}
$pets = Get-Content (Join-Path $cache 'pets.json') -Raw | ConvertFrom-Json -AsHashtable
$npcs = Get-Content (Join-Path $cache 'npcs.json') -Raw | ConvertFrom-Json -AsHashtable
$catalog = Get-Content (Join-Path $root 'assets/catalog.json') -Raw | ConvertFrom-Json
$links = Get-Content (Join-Path $root 'src/Core/FarmingLinks.json') -Raw | ConvertFrom-Json
$sources = Get-Content (Join-Path $root 'src/Core/PetSources.json') -Raw | ConvertFrom-Json
$petNames = [ordered]@{}
foreach ($pet in $catalog) {
    $row = $pets.pets[[string]$pet.Id]
    if ($row -and $row.key -ceq $pet.CatalogName -and $row.names.fr) {
        $petNames[[string]$pet.Id] = @{ En=$row.names.en; Fr=$row.names.fr }
    }
}
$npcNames = [ordered]@{}
$ids = @($sources.Rows | ForEach-Object NpcId) + @($links.Rows.SoulSources | ForEach-Object NpcId)
foreach ($id in ($ids | Sort-Object -Unique)) {
    $row = $npcs[[string]$id]
    if ($row.en -and $row.fr) { $npcNames[[string]$id] = @{ En=$row.en; Fr=$row.fr } }
}
$result = [ordered]@{ Source="https://github.com/SkeeveAN/Aion-DPS-Meter/tree/$Commit"; Pets=$petNames; Npcs=$npcNames }
[IO.File]::WriteAllText((Join-Path $root 'src/Core/LocalizedNames.json'), (ConvertTo-Json $result -Depth 6), [Text.UTF8Encoding]::new($false))
Copy-Item (Join-Path $cache 'LICENSE') (Join-Path $root 'licenses/SkeeveAN-MIT.txt')
Write-Output "Imported $($petNames.Count) pet names and $($npcNames.Count) NPC names."
