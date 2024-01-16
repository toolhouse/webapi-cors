param (
    [Parameter(Mandatory=$true)][string]$tag,
    [Parameter(Mandatory=$true)][string]$apiKey,
    [Parameter(Mandatory=$true)][string]$feedUrl
)

$semverRegex = "^v([0-9]|[1-9][0-9]*)\.([0-9]|[1-9][0-9]*)\.([0-9]|[1-9][0-9]*)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?$"

If (!($tag -match $semverRegex)) {
    Write-Host "The tag provided (`"$tag`") is an invalid semver string!" -ForegroundColor red
    exit 1
}

Write-Host "Creating Nuget package for project version $tag"
Write-Host "Restoring..."

nuget restore
if (!$?) { exit 1 }

Write-Host "Packing..."

nuget pack -Version $tag.Substring(1) .\Toolhouse.WebApi.Cors\Toolhouse.WebApi.Cors.csproj
if (!$?) { exit 1 }

Write-Host "Uploading..."

nuget push * -ApiKey $apiKey -Source $feedUrl
if (!$?) { exit 1 }
