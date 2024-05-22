param (
    [string]$Path = "."
)

function Get-ProjectDefaultNamespace {
    param (
        [xml]$ProjectXml,
        [string]$ProjectFileName
    )

    $namespace = $ProjectXml.Project.PropertyGroup.RootNamespace
    if ($namespace) { return $namespace } else { return [System.IO.Path]::GetFileNameWithoutExtension($ProjectFileName) }
}

function Get-Dependencies {
    param (
        [string]$ProjectFile,
        [hashtable]$CPMVersions,
        [string]$ProjectNamespace
    )

    $xml = [xml](Get-Content -Path $ProjectFile)
    $dependencies = @{}

    foreach ($package in $xml.Project.ItemGroup.PackageReference) {
        if ($null -ne $package.Include -and $package.Include -ne "") {
            $packageInclude = $package.Include
            $cpmVersion = if ($CPMVersions.ContainsKey($packageInclude)) { $CPMVersions[$packageInclude] } else { "N/A" }
            $projectVersion = if ($package.Version) { "Override: $($package.Version)" } else { "Inherit CPM" }

            if (-not $dependencies.ContainsKey($packageInclude)) {
                $dependencies[$packageInclude] = @{}
            }

            $dependencies[$packageInclude][$ProjectNamespace] = $projectVersion
        }
    }

    return $dependencies
}

function Get-CPMVersions {
    param (
        [string]$BasePath
    )

    $cpmVersions = @{}
    $directoryPackagesPropsPath = Join-Path -Path $BasePath -ChildPath "Directory.Packages.props"

    if (Test-Path -Path $directoryPackagesPropsPath) {
        $xml = [xml](Get-Content -Path $directoryPackagesPropsPath)
        foreach ($package in $xml.Project.ItemGroup.PackageVersion) {
            if ($null -ne $package.Include -and $package.Include -ne "") {
                $cpmVersions[$package.Include] = $package.Version
            }
        }
    }
	
    return $cpmVersions | Sort-Object -Property Name
}

function Get-ProjectsDependencies {
    param (
        [string]$Path
    )

    $cpmVersions = Get-CPMVersions -BasePath $Path
    $projectFiles = Get-ChildItem -Path $Path -Recurse -Filter *.csproj
    $allDependencies = @{}
    $projectNamespaces = @()

    foreach ($projectFile in $projectFiles) {
        $xml = [xml](Get-Content -Path $projectFile.FullName)
        $projectNamespace = Get-ProjectDefaultNamespace -ProjectXml $xml -ProjectFileName $projectFile.Name
        $projectNamespaces += $projectNamespace

        $dependencies = Get-Dependencies -ProjectFile $projectFile.FullName -CPMVersions $cpmVersions -ProjectNamespace $projectNamespace

        foreach ($package in $dependencies.Keys) {
            if (-not $allDependencies.ContainsKey($package)) {
                $allDependencies[$package] = @{}
            }
            $allDependencies[$package] += $dependencies[$package]
        }
    }

    return @{ Dependencies = $allDependencies; Projects = $projectNamespaces; CPMVersions = $cpmVersions }
}

function Render-MarkdownTable {
    param (
        [hashtable]$Dependencies,
        [array]$Projects,
        [hashtable]$CPMVersions
    )

    $markdown = "| Nuget Namespace | CPM Version |"
    foreach ($project in $Projects) {
        $markdown += " $project |"
    }
    $markdown += "`n|-----------------|-------------|"
    foreach ($project in $Projects) {
        $markdown += "--------------|"
    }
    $markdown += "`n"

    foreach ($package in ($Dependencies.Keys | Sort)) {
        $cpmVersion = if ($CPMVersions.ContainsKey($package)) { $CPMVersions[$package] } else { "N/A" }
        $markdown += "| <a href=""https://www.nuget.org/packages/$($package)"">$($package)</a> | <a href=""https://www.nuget.org/packages/$($package)/$($cpmVersion)"">$($cpmVersion)</a> |"

        foreach ($project in $Projects) {
            $projectVersion = if ($Dependencies[$package].ContainsKey($project)) { $Dependencies[$package][$project] } else { "-" }
            $markdown += " $projectVersion |"
        }
        $markdown += "`n"
    }

    return $markdown
}

$projectData = Get-ProjectsDependencies -Path $Path
$dependencies = $projectData.Dependencies
$projects = $projectData.Projects
$cpmVersions = $projectData.CPMVersions
$markdownTable = Render-MarkdownTable -Dependencies $dependencies -Projects $projects -CPMVersions $cpmVersions

Write-Output $markdownTable

# Ensure ./script-artifacts/ folder exists; create of not
if (-not (Test-Path -Path "./.script-artifacts")) {
	New-Item -Path "./.script-artifacts" -ItemType Directory | Out-Null
}

# Write to ./.script-artifacts/dependencies-map.md
$markdownTable | Out-File -FilePath "./.script-artifacts/dependencies-map.md" -Force