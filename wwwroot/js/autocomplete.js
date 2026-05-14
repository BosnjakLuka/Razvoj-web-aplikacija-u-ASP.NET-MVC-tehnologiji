(function () {
    'use strict';

    function initAutocomplete(wrapper) {
        const fieldName = wrapper.dataset.field;
        const hiddenInput = document.getElementById('hidden_' + fieldName);
        const textInput = document.getElementById('display_' + fieldName);
        const list = document.getElementById('list_' + fieldName);
        const searchUrl = wrapper.dataset.searchUrl;
        const valSpan = document.getElementById('val_' + fieldName);

        let debounceTimer = null;
        let currentRequest = null;

        textInput.addEventListener('input', function () {
            const term = this.value.trim();

            if (term.length === 0) {
                hiddenInput.value = '';
                hideList();
                return;
            }

            if (term.length < 2) return;

            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(function () {
                if (currentRequest) currentRequest.abort();

                currentRequest = $.ajax({
                    url: searchUrl,
                    data: { term: term },
                    success: function (results) {
                        renderList(results);
                    }
                });
            }, 300);
        });

        textInput.addEventListener('blur', function () {
            setTimeout(function () {
                if (textInput.value.trim() !== '' && hiddenInput.value === '') {
                    hiddenInput.value = '';
                    textInput.value = '';
                }
                validateField();
                hideList();
            }, 200);
        });

        function validateField() {
            if (!valSpan) return;
            if (hiddenInput.required && hiddenInput.value === '') {
                valSpan.textContent = 'Ovo polje je obavezno.';
            } else {
                valSpan.textContent = '';
            }
        }

        function renderList(results) {
            list.innerHTML = '';

            if (!results || results.length === 0) {
                const li = document.createElement('li');
                li.className = 'no-results';
                li.textContent = 'Nema rezultata.';
                list.appendChild(li);
                showList();
                return;
            }

            results.forEach(function (item) {
                const li = document.createElement('li');
                li.textContent = item.label;
                li.dataset.id = item.value;

                li.addEventListener('mousedown', function (e) {
                    e.preventDefault();
                    selectItem(item.value, item.label);
                });

                list.appendChild(li);
            });

            showList();
        }

        function selectItem(id, label) {
            hiddenInput.value = id;
            textInput.value = label;
            hiddenInput.dispatchEvent(new Event('change'));
            if (valSpan) valSpan.textContent = '';
            hideList();
        }

        function showList() { list.classList.remove('d-none'); }
        function hideList() { list.classList.add('d-none'); }

        document.addEventListener('click', function (e) {
            if (!wrapper.contains(e.target)) hideList();
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.autocomplete-wrapper[data-search-url]').forEach(initAutocomplete);
    });
})();
