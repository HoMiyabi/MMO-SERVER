$inputPath = "./protos"
$outputPath = "./csharp"
$protoFilter = "*.proto"

$copyTargetPaths = @(
    "C:\Things\Code\CSharp\MMO-SERVER\Common\Proto",
    "C:\UnityProjects\MMOGAME\Assets\Plugins\Summer\Proto"
)

if (!(Test-Path $outputPath -PathType Container)) {
    New-Item -ItemType Directory -Force -Path $outputPath
}

Get-ChildItem -Path $outputPath -Recurse | Remove-Item

$files = Get-ChildItem -Path $inputPath -Filter $protoFilter
foreach ($file in $files)
{
    $filename = $file.BaseName + $file.Extension
    Write-Output($filename + "...")
    & protoc @(
        "--proto_path", $inputPath,
        "--csharp_out", $outputPath,
        $filename)
}

# Copy to project

foreach ($copyTargetPath in $copyTargetPaths)
{
    Get-ChildItem -Path $copyTargetPath -Recurse | Remove-Item
    Get-ChildItem -Path $outputPath -Recurse | Copy-Item -Destination $copyTargetPath
}

Pause