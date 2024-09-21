$excelPath = "./excel"
$outPath = "./out"
$exe = "./excel2json.exe"
$excelFilter = "*.xlsx"
$copyDestPaths = @(
    "C:\Things\Code\CSharp\MMO-SERVER\GameServer\Data",
    "C:\UnityProjects\MMOGAME\Assets\Resources\Data"
)

$excelFiles = Get-ChildItem -Path $excelPath -Filter $excelFilter

Write-Output "转换..."
foreach ($file in $excelFiles)
{
    $excelPath = Join-Path $excelPath $file.Name
    $jsonPath = Join-Path $outPath ($file.BaseName + ".json")
    $csharpPath = Join-Path $outPath ($file.BaseName + ".cs")
    & $exe @("--excel", $excelPath,
             "--json", $jsonPath,
             "--csharp", $csharpPath,
             "--header", "3")
}

Write-Output "复制..."
foreach ($copyDestPath in $copyDestPaths)
{
    Get-ChildItem -Path $outPath | Copy-Item -Destination $copyDestPath
}
Pause