dotnet publish PeripheralAudit/PeripheralAudit.csproj `
              -c Release `
              -o publish `
              -r win-x64 `
              --self-contained false

[xml]$projectFile = Get-Content "PeripheralAudit\PeripheralAudit.csproj"

$Version = $projectFile.Project.PropertyGroup.Version | Where-Object { $_ -ne $null } | Select-Object -First 1
if ([string]::IsNullOrEmpty($Version)) {
    Write-Host "ERROR: Could not read version from PeripheralAudit.csproj" -ForegroundColor Red
    exit 1
}
Write-Host "Publishing PeripheralAudit version $Version" -ForegroundColor Green

vpk download github --repoUrl "https://github.com/JLdgu/PeripheralAudit" 

vpk pack -u PeripheralAudit `
    -v $Version `
    -p .\publish `
    -e PeripheralAudit.exe `
    --packAuthors "Devon County Council" `
    --noPortable 
