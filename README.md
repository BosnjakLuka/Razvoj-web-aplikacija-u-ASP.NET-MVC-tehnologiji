# Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji
Kolegiji Razvoj web aplikacija u ASP.NET MVC tehnologiji sadrži se od 5 labaratorijskih vježbi i projekta koji su povezani. Kolegiji vodi profesor Ivan Cesar. Tema projekta je modernizacija planinarske knjižice u web aplikaciju gdje se skenira QR kod umjesto štambiljana bilježnice. Modernizacija i lakše praćenje planinarenja.

Admin i planinar login
Uloga	Email	Lozinka
Admin	admin@planinarenje.hr	Admin@2026!
Planinar	luka@planinarenje.hr	Planinar@26!

test za development stranu projekta 121 api testova
dotnet test --logger "console;verbosity=detailed"

cd "c:\Users\lukab\Documents\Projekt\Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji"
dotnet test planinarenje.IntegrationTests/planinarenje.IntegrationTests.csproj


MCP server test:
http://localhost:6274/?MCP_PROXY_AUTH_TOKEN=0562f307ceef34e709fc381c243012d36d0990aa50b0a0b6f6480896d5c78e55#tools

Transport Type: Streamable HTTP
URL:            http://localhost:5041/mcp

Da promjena postane live, trenutno treba ručno:

dotnet publish planinarenje.csproj (eksplicitno taj projekt, ne cijeli .slnx jer bi pokupio i planinarenje.IntegrationTests)
Spakirati publish output u zip — ne PowerShell Compress-Archive (piše backslash separatore koji slome Linux App Service), nego ručno kroz System.IO.Compression.ZipArchive s normaliziranim / putanjama
az webapp deploy (ili portal zip-deploy) na planinarenje-app resource
Ako ima novih EF migracija — ručno dotnet ef database update protiv produkcijskog MySQL connection stringa (nema Database.Migrate() na startupu)

how to deploy app
1. Commit i push izmjena

git add planinarenje.csproj
git commit -m "Uvrsti Slike/ folder u publish output"
git push origin main

2. Provjeri da si ulogiran u Azure CLI

az account show

3. Publish (Release build, samo glavni projekt)

dotnet publish planinarenje.csproj -c Release -o ./publish-output
Bitno: eksplicitno planinarenje.csproj, ne cijelo .slnx (pokupi i planinarenje.IntegrationTests).

4. Spakiraj publish output u zip 

$sourceDir = (Resolve-Path "./publish-output").Path
$zipPath = Join-Path (Get-Location).Path "deploy.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }

$zip = [System.IO.Compression.ZipFile]::Open($zipPath, [System.IO.Compression.ZipArchiveMode]::Create)
Get-ChildItem -Path $sourceDir -Recurse -File | ForEach-Object {
    $relativePath = $_.FullName.Substring($sourceDir.Length + 1).Replace('\', '/')
    [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $_.FullName, $relativePath) | Out-Null
}
$zip.Dispose()

Write-Host "Zip kreiran: $zipPath"

5. — deploy na Azure:

az webapp deploy --resource-group rg-planinarenje --name planinarenje-app --src-path ./deploy.zip --type zip

6. (Samo ako ima nove EF migracije od zadnjeg deploya) Pokreni migraciju na produkcijskoj bazi
dotnet ef database update --connection "<produkcijski MySQL connection string>"


7. — provjera: 
https://planinarenje-app-h5gbahh5b3afasfq.austriaeast-01.azurewebsites.net
