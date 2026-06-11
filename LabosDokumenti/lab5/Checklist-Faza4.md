# Checklist — Faza 4 (Dropzone upload fotografija na Posjet)

> Prolazi ovu listu da potvrdiš da je Faza 4 stvarno gotova.
> Aplikaciju pokreni s `dotnet run` (Development), prijavi se kao **Planinar** (vlasnik posjeta) ili **Admin**.
> Sve se događa na stranici **`/Posjet/Details/{id}`** (detalji jednog posjeta).

---

## A) Što je implementirano (pregled)

- [x] `Fotografija` entitet već ima `ContentType`, `FileSize`, `DeletedAt` → **nije bila potrebna nova migracija**
- [x] `PosjetController.UploadFoto(idPosjet, file, tip)` — `[HttpPost, ValidateAntiForgeryToken, Authorize]`, provjera vlasništva, validacija, sprema na disk + u bazu, vraća `Json`
- [x] `PosjetController.GetFotografije(idPosjet)` — `[AllowAnonymous]`, vraća `_PosjetFotografijeListPartial` (AJAX popis)
- [x] `PosjetController.DeleteFoto(id)` — `[HttpPost, ValidateAntiForgeryToken, Authorize]`, vlasništvo, briše datoteku s diska + soft delete u bazi
- [x] Partial `Views/Posjet/_PosjetFotografijeListPartial.cshtml` (galerija + gumb za brisanje samo vlasniku/Adminu)
- [x] Dropzone (CDN) ugrađen u `Posjet/Details.cshtml`, vidljiv **samo vlasniku/Adminu**
- [x] Antiforgery token u Dropzone headeru (R9) i u AJAX brisanju
- [x] `FormatirajSliku` u `PosjetController` i `FotografijaController` propušta `/uploads/...` putanje

---

## B) Build i pokretanje

- [x] `dotnet build` prolazi bez **grešaka** *(provjereno: 0 grešaka, 8 starih warninga)*
- [x] Aplikacija se pokreće bez rušenja
- [x] `/Posjet/Details/{id}` se otvara i ispod detalja prikazuje sekciju fotografija

---

## C) Upload (kao vlasnik posjeta ili Admin)

- [x] Na `Details` vlastitog posjeta vidi se Dropzone okvir „Povucite fotografije ovdje…"
- [x] Odabir tipa fotografije (Selfie/Oznaka/Krajolik/Mapa/Drugo) radi
- [x] Upload JPG/PNG/WEBP datoteke prolazi **bez 400 greške** (antiforgery token radi, R9)
- [x] Datoteka je fizički stigla u `wwwroot/uploads/posjeti/{idPosjet}/{guid}.{ext}`
- [x] U tablici `Fotografija` postoji novi red s `PutanjaDatoteke = /uploads/posjeti/{id}/...`, `ContentType`, `FileSize`, `DatumUploada`, `TipSlike`
- [x] Popis fotografija se **automatski osvježi** odmah nakon uploada (bez ručnog refresha)
- [x] Slika se stvarno prikazuje (URL `/uploads/...` se servira iz wwwroot)

---

## D) Validacija uploada

- [x] Datoteka > 5 MB → odbijena (poruka o veličini)
- [x] Kriva ekstenzija (npr. `.pdf`, `.exe`) → odbijena (poruka o formatu)
- [x] Datoteka koja nije slika (ContentType ne počinje s `image/`) → odbijena

---

## E) Autorizacija i vlasništvo

- [x] Kao **anoniman** korisnik na `Details` tuđeg posjeta — **nema** Dropzonea ni gumba za brisanje (samo gledanje galerije)
- [x] `POST /Posjet/UploadFoto` **bez prijave** → 401 (redirect na login)
- [x] Upload na **tuđi** posjet kao Planinar koji nije vlasnik → **403** (ne 200)
- [x] Admin može uploadati/brisati na bilo čijem posjetu

---

## F) Brisanje

- [x] Gumb za brisanje (kanta) vidljiv je **samo vlasniku/Adminu** na svakoj fotografiji
- [x] Klik → potvrda → fotografija nestane iz popisa (AJAX refresh)
- [x] Datoteka je **obrisana s diska** (`wwwroot/uploads/posjeti/{id}/...` više ne postoji)
- [x] Red u bazi je soft-deletan (`DeletedAt` postavljen) — ne pojavljuje se više u popisu
- [x] Brisanje tuđe fotografije kao ne-vlasnik → **403**

---

## G) Konvencije (CLAUDE.md)

- [x] Soft delete u bazi (`DeletedAt`), **bez** `Db.Remove()` — fizički se briše samo datoteka s diska
- [x] Vlasništvo se provjerava preko `IsOwnerAsync(posjet.IdKorisnik)` / `IsAdmin` (helperi iz `BaseController`)
- [x] UI tekst i komentari na hrvatskom
- [x] Postojeći Lab1–4 ekrani (CRUD, autocomplete, datepicker) i dalje rade

---

## Brzi ručni test (preporučeni redoslijed)

1. Prijavi se kao **Planinar** koji ima barem jedan svoj posjet.
2. Otvori `/Posjet/Details/{id}` tog posjeta → vidiš Dropzone.
3. Povuci 1–2 slike → upload prođe, popis se osvježi, slike se vide.
4. Provjeri `wwwroot/uploads/posjeti/{id}/` — datoteke su tu.
5. Klikni kantu na jednoj slici → potvrdi → nestane; provjeri da je datoteka maknuta s diska.
6. Otvori `Details` **tuđeg** posjeta → nema Dropzonea ni gumba za brisanje.
7. Odjavi se → otvori bilo koji `Details` → galerija se vidi, ali nema upload/brisanje.

> Kad su svi okviri u sekcijama B–G označeni — **Faza 4 je gotova.**
