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