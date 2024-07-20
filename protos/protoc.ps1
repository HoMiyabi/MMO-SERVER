$inputPath = "./protos"
$outputPath = "./csharp"
$protoFilter = "*.proto"

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

Pause