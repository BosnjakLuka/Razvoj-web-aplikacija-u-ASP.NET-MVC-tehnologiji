# LINQ upiti — Planinarska aplikacija

## Što je cilj ovog dokumenta
Ovaj dokument prikazuje LINQ upite korištene u projektu planinarske aplikacije.
Svaki upit ima jasan cilj i objašnjenje.

---

# 1. Dohvat svih korisnika
LINQ:
korisnici

Opis:
Vraća sve korisnike bez filtriranja.

---

# 2. Svi posjeti jednog korisnika
LINQ:
posjeti.Where(p => p.IdKorisnik == 1)

Opis:
Filtrira posjete i vraća samo one od određenog korisnika.

---

# 3. Kontrolne točke iz područja
LINQ:
kontrolneTocke.Where(kt => kt.IdPodrucje == 5)

Opis:
Vraća sve KT iz određenog područja.

---

# 4. Rute za kontrolnu točku
LINQ:
rute.Where(r => r.IdKontrolnaTocka == 3)

Opis:
Vraća rute koje vode na određeni vrh.

---

# 5. Fotografije za posjet
LINQ:
fotografije.Where(f => f.IdPosjet == 1)

Opis:
Vraća slike za određeni posjet.

---

# 6. Broj posjeta po korisniku
LINQ:
posjeti.GroupBy(p => p.IdKorisnik)
       .Select(g => new { g.Key, Broj = g.Count() })

Opis:
Grupira i broji posjete po korisniku.

---

# 7. Korisnici s medaljom
LINQ:
korisnici.Where(k => korisnikMedalje.Any(km => km.IdKorisnik == k.IdKorisnik))

Opis:
Vraća korisnike koji imaju medalju.

---

# 8. KT koje korisnik nije obišao
LINQ:
kontrolneTocke.Where(kt => !posjeti.Any(p => p.IdKorisnik == 1 && p.IdKontrolnaTocka == kt.IdKontrolnaTocka))

Opis:
Vraća KT koje korisnik nije posjetio.

---

# 9. Najteže rute
LINQ:
rute.OrderByDescending(r => r.TezinaRute)

Opis:
Sortira rute po težini.

---

# 10. Područja koja je korisnik obišao
LINQ:
podrucja.Where(p =>
    posjeti.Count(pos => pos.IdKorisnik == 1 &&
        kontrolneTocke.Any(kt =>
            kt.IdKontrolnaTocka == pos.IdKontrolnaTocka &&
            kt.IdPodrucje == p.IdPodrucje)
    ) >= p.MinimalanBrojKTZaObilazak)

Opis:
Provjerava da li korisnik ima dovoljno KT u području.

---

# Zaključak
Ovi upiti pokrivaju sve ključne LINQ operacije:
Where, Select, GroupBy, Count, Any, OrderBy.
