// Globalna (cross-entity) pretraga - live dropdown ispod navbar okvira.
// Forma i dalje radi bez JS-a (GET -> /pretraga?q=...); ovo je samo nadogradnja.
(function () {
    'use strict';

    const MIN_DULJINA = 2;
    const DEBOUNCE_MS = 250;

    function init() {
        const form = document.querySelector('.global-search');
        if (!form) return;

        const input = document.getElementById('globalSearchInput');
        const dropdown = document.getElementById('globalSearchDropdown');
        const liveUrl = form.dataset.liveUrl;
        if (!input || !dropdown || !liveUrl) return;

        let debounceTimer = null;
        let controller = null;
        let aktivni = -1; // indeks trenutno označene stavke (tipkovnica)

        input.addEventListener('input', function () {
            const term = input.value.trim();
            clearTimeout(debounceTimer);

            if (term.length < MIN_DULJINA) {
                sakrij();
                return;
            }

            debounceTimer = setTimeout(function () { dohvati(term); }, DEBOUNCE_MS);
        });

        input.addEventListener('focus', function () {
            if (input.value.trim().length >= MIN_DULJINA && dropdown.children.length > 0) {
                prikazi();
            }
        });

        input.addEventListener('keydown', function (e) {
            const stavke = dropdown.querySelectorAll('.gs-dd-item');

            if (e.key === 'ArrowDown') {
                e.preventDefault();
                if (stavke.length === 0) return;
                aktivni = (aktivni + 1) % stavke.length;
                oznaci(stavke);
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                if (stavke.length === 0) return;
                aktivni = (aktivni - 1 + stavke.length) % stavke.length;
                oznaci(stavke);
            } else if (e.key === 'Enter') {
                // Ako je stavka označena tipkovnicom -> idi na nju; inače pusti formu na punu stranicu.
                if (aktivni >= 0 && stavke[aktivni]) {
                    e.preventDefault();
                    window.location.href = stavke[aktivni].getAttribute('href');
                }
            } else if (e.key === 'Escape') {
                sakrij();
            }
        });

        document.addEventListener('click', function (e) {
            if (!form.contains(e.target)) sakrij();
        });

        function dohvati(term) {
            if (controller) controller.abort();
            controller = new AbortController();

            fetch(liveUrl + '?q=' + encodeURIComponent(term), { signal: controller.signal })
                .then(function (r) { return r.ok ? r.json() : null; })
                .then(function (data) { if (data) renderiraj(data, term); })
                .catch(function () { /* abort ili mrežna greška - ignoriraj */ });
        }

        function renderiraj(data, term) {
            dropdown.innerHTML = '';
            aktivni = -1;

            const grupe = (data && data.grupe) || [];
            if (grupe.length === 0) {
                const prazno = document.createElement('div');
                prazno.className = 'gs-dd-empty';
                prazno.textContent = 'Nema rezultata.';
                dropdown.appendChild(prazno);
                prikazi();
                return;
            }

            grupe.forEach(function (grupa) {
                const head = document.createElement('div');
                head.className = 'gs-dd-group';
                const naziv = document.createElement('span');
                naziv.textContent = grupa.naziv;
                const broj = document.createElement('span');
                broj.className = 'gs-dd-count';
                broj.textContent = grupa.ukupno;
                head.appendChild(naziv);
                head.appendChild(broj);
                dropdown.appendChild(head);

                (grupa.stavke || []).forEach(function (s) {
                    const a = document.createElement('a');
                    a.className = 'gs-dd-item';
                    a.href = s.url || '#';

                    const naslov = document.createElement('span');
                    naslov.className = 'gs-dd-title';
                    naslov.textContent = s.naziv;
                    a.appendChild(naslov);

                    if (s.podnaslov) {
                        const sub = document.createElement('span');
                        sub.className = 'gs-dd-sub';
                        sub.textContent = s.podnaslov;
                        a.appendChild(sub);
                    }

                    a.addEventListener('mouseenter', function () {
                        aktivni = -1;
                        dropdown.querySelectorAll('.gs-dd-item.active').forEach(function (el) { el.classList.remove('active'); });
                    });

                    dropdown.appendChild(a);
                });
            });

            // Podnožje: idi na punu stranicu rezultata.
            const sve = document.createElement('a');
            sve.className = 'gs-dd-all';
            sve.href = form.getAttribute('action') + '?q=' + encodeURIComponent(term);
            sve.textContent = 'Prikaži sve rezultate (' + ((data && data.ukupno) || 0) + ') →';
            dropdown.appendChild(sve);

            prikazi();
        }

        function oznaci(stavke) {
            stavke.forEach(function (el, i) {
                el.classList.toggle('active', i === aktivni);
            });
            if (aktivni >= 0 && stavke[aktivni]) {
                stavke[aktivni].scrollIntoView({ block: 'nearest' });
            }
        }

        function prikazi() { dropdown.classList.remove('d-none'); }
        function sakrij() { dropdown.classList.add('d-none'); aktivni = -1; }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
