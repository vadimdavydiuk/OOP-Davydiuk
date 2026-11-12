$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    New-Item -ItemType Directory -Path verification -Force | Out-Null
    dotnet build IndependentWorks.sln --configuration Release
    if ($LASTEXITCODE -ne 0) { throw 'Solution build failed.' }
    dotnet test IndependentWork8.Tests --configuration Release --no-build
    if ($LASTEXITCODE -ne 0) { throw 'Cache tests failed.' }
    foreach ($number in 8..15) {
        $project = Get-ChildItem -LiteralPath "IndependentWork$number" -Filter '*.csproj' |
            Where-Object Name -NotLike '*.Tests.csproj' | Select-Object -First 1
        $log = & dotnet run --project $project.FullName --configuration Release --no-build -- --demo
        if ($LASTEXITCODE -ne 0) { throw "Work $number failed." }
        $log | Set-Content -LiteralPath "verification/$number.txt" -Encoding utf8
        Write-Host "Work $number passed."
    }
} finally { Pop-Location }
