# Checklist — Faza 5 (Google OAuth vanjska prijava)

> Prolazi ovu listu da potvrdiš da je Faza 5 stvarno gotova.
> Aplikaciju pokreni s **HTTPS profilom** (`dotnet run --launch-profile https`) jer OAuth zahtijeva HTTPS.
> Sve se događa na stranici **`/Identity/Account/Login`** (sekcija „Use another service to log in").

---

## A) Što je implementirano u kodu (pregled)

- [x] NuGet paket `Microsoft.AspNetCore.Authentication.Google` (v10.0.9) dodan u `planinarenje.csproj`
- [x] `Program.cs` — `AddAuthentication().AddGoogle(...)` čita `Authentication:Google:ClientId` i `:ClientSecret` iz konfiguracije
- [x] Provider se registrira **samo ako su oba ključa postavljena** (app ne pada na startupu ako ključevi fale)
- [x] `ExternalLogin.cshtml.cs` — `OnPostConfirmationAsync` kreira `AppUser` (s OIB/JMBG) **i** pripadajući `Korisnik` zapis (s `AppUserId`)
- [x] `ExternalLogin.cshtml` — forma traži Ime, Prezime, Email, OIB, JMBG (hrvatski UI tekst)
- [x] `Login.cshtml` — sekcija vanjskih prijava automatski renderira Google gumb kad je provider registriran
- [x] `app.UseHttpsRedirection()` aktivan (iz Faze 1)
- [x] `UserSecretsId` inicijaliziran u `.csproj` (store spreman; ključevi se NE drže u kodu)

---

## B) Tvoja priprema (ručni koraci — JEDNOM)

> ⚠️ Ovo radiš ti, ne agent — uključuje pravi Google račun i tajne ključeve koji **nikad ne idu u git**.

### B1. Google Cloud Console
- [ ] Otvori https://console.cloud.google.com → **New Project** (npr. „Planinarenje")
- [ ] **APIs & Services → OAuth consent screen** → tip **External** → ispuni naziv aplikacije i e-mail; dodaj sebe pod **Test users**
- [ ] **APIs & Services → Credentials → Create Credentials → OAuth client ID**
  - Application type: **Web application**
  - **Authorized redirect URI:** `https://localhost:7187/signin-google`
- [ ] Kopiraj **Client ID** i **Client Secret**

### B2. User secrets (iz root foldera projekta)
- [ ] `dotnet user-secrets set "Authentication:Google:ClientId" "<TVOJ_CLIENT_ID>"`
- [ ] `dotnet user-secrets set "Authentication:Google:ClientSecret" "<TVOJ_CLIENT_SECRET>"`
- [ ] Provjera: `dotnet user-secrets list` prikazuje oba ključa

---

## C) Build i pokretanje

- [ ] `dotnet build` prolazi bez **grešaka**
- [ ] `dotnet run --launch-profile https` se pokreće (sluša na `https://localhost:7187`)
- [ ] `/Identity/Account/Login` se otvara

---

## D) Google gumb i preusmjeravanje

- [ ] U sekciji „Use another service to log in" vidi se **Google** gumb
  *(ako ga nema → ključevi nisu postavljeni; vrati se na B2)*
- [ ] Klik na **Google** preusmjerava na Google login stranicu
- [ ] Prijava test Google računom prolazi i vraća te natrag na aplikaciju

---

## E) Prva prijava (kreiranje računa)

- [ ] Prva prijava novim Google računom otvara „Dovrši registraciju" formu
- [ ] Email je predpopunjen iz Google računa
- [ ] Forma traži Ime, Prezime, OIB (11 znamenki), JMBG (13 znamenki)
- [ ] Kriv OIB/JMBG (npr. slova ili pogrešna duljina) → validacijska greška, račun se NE kreira
- [ ] Ispravan unos → korisnik je prijavljen i preusmjeren na `/`
- [ ] U `AspNetUsers` postoji novi red (s OIB/JMBG)
- [ ] U `AspNetUserLogins` postoji red (LoginProvider = Google)
- [ ] U `Korisnik` postoji novi red s popunjenim `AppUserId`

---

## F) Ponovna prijava

- [ ] Odjava (`/Identity/Account/Logout`)
- [ ] Ponovni klik na Google s **istim** računom → odmah prijavljen (bez ponovnog traženja OIB/JMBG)
- [ ] Ne kreira se duplikat u `AspNetUsers` ni u `Korisnik`

---

## G) Konvencije (CLAUDE.md)

- [x] ClientId/ClientSecret se NE drže u kodu ni u `appsettings*.json` (samo user secrets)
- [x] OIB/JMBG se traže pri prvoj vanjskoj prijavi (kao kod lokalne registracije)
- [x] UI tekst na hrvatskom
- [ ] Postojeća lokalna prijava/registracija i Lab1–4 ekrani i dalje rade

---

## Brzi ručni test (preporučeni redoslijed)

1. Odradi **B1 + B2** (Google Cloud + user secrets).
2. `dotnet run --launch-profile https`, otvori `/Identity/Account/Login`.
3. Klikni **Google** → prijavi se test računom.
4. Ispuni Ime/Prezime/OIB/JMBG → potvrdi → preusmjerenje na `/`.
5. Provjeri `AspNetUsers`, `AspNetUserLogins` i `Korisnik` u bazi.
6. Odjavi se → ponovno Google → odmah prijavljen.

> Kad su svi okviri u sekcijama C–G označeni — **Faza 5 je gotova.**
