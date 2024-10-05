$excelsPath = "./excel"
$outPath = "./out"
$exe = "./excel2json.exe"
$excelFilter = "*.xlsx"

$copyModelPaths = @(
    "C:\UnityProjects\MMOGAME\Assets\Scripts\DefineModel",
    "C:\Things\Code\CSharp\MMO-SERVER\GameServer\Define"
)

$copyJsonPaths = @(
    "C:\UnityProjects\MMOGAME\Assets\Resources\DefineJson",
    "C:\Things\Code\CSharp\MMO-SERVER\GameServer\Define"
)

$excelFiles = Get-ChildItem -Path $excelsPath -Filter $excelFilter

Write-Output "生成..."
foreach ($file in $excelFiles)
{
    $excelPath = Join-Path $excelsPath $file.Name
    $jsonPath = Join-Path $outPath ($file.BaseName + ".json")
    $csharpPath = Join-Path $outPath ($file.BaseName + ".cs")
    & $exe @("--excel", $excelPath,
             "--json", $jsonPath,
             "--csharp", $csharpPath,
             "--header", "3")
}

Write-Output "复制..."

foreach ($copyModelPath in $copyModelPaths)
{
    Get-ChildItem -Path $outPath -Filter "*.cs" | Copy-Item -Destination $copyModelPath
}

foreach ($copyJsonPath in $copyJsonPaths)
{
    Get-ChildItem -Path $outPath -Filter "*.json" | Copy-Item -Destination $copyJsonPath
}
Pause