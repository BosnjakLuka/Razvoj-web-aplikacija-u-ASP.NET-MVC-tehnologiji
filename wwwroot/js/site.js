// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Toast notifications
function showToast(message, type) {
	type = type || 'success';
	const colors = {
		success: { bg: '#1b4332', icon: '✓' },
		error: { bg: '#7f1d1d', icon: '✕' },
		info: { bg: '#1e3a5f', icon: 'i' }
	};
	const c = colors[type] || colors.success;

	const toast = document.createElement('div');
	toast.style.cssText = [
		'background:' + c.bg,
		'color:#fff',
		'padding:12px 18px',
		'border-radius:8px',
		'font-size:0.9rem',
		'display:flex',
		'align-items:center',
		'gap:10px',
		'min-width:260px',
		'max-width:380px',
		'box-shadow:0 4px 16px rgba(0,0,0,0.25)',
		'opacity:0',
		'transform:translateX(40px)',
		'transition:opacity 0.25s ease, transform 0.25s ease',
		'cursor:pointer'
	].join(';');

	toast.innerHTML = '<span style="font-weight:700;font-size:1rem">' + c.icon + '</span>' +
		'<span>' + message + '</span>';

	const container = document.getElementById('toast-container');
	if (!container) return;

	container.appendChild(toast);

	requestAnimationFrame(function () {
		requestAnimationFrame(function () {
			toast.style.opacity = '1';
			toast.style.transform = 'translateX(0)';
		});
	});

	toast.addEventListener('click', function () { dismissToast(toast); });
	setTimeout(function () { dismissToast(toast); }, 4000);
}

function dismissToast(toast) {
	toast.style.opacity = '0';
	toast.style.transform = 'translateX(40px)';
	setTimeout(function () {
		if (toast.parentNode) toast.parentNode.removeChild(toast);
	}, 280);
}

// Smooth scroll for anchors
document.addEventListener('DOMContentLoaded', function () {
	document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
		anchor.addEventListener('click', function (e) {
			const target = document.querySelector(this.getAttribute('href'));
			if (target) {
				e.preventDefault();
				target.scrollIntoView({ behavior: 'smooth', block: 'start' });
			}
		});
	});
});

// Datepicker inicijalizacija
(function () {
	'use strict';

	function getBrowserLocale() {
		var lang = (navigator.languages && navigator.languages[0]) ||
			navigator.language || 'hr';
		return lang.toLowerCase().startsWith('en') ? 'en' : 'hr';
	}

	function initDatepickers() {
		document.querySelectorAll('.datepicker-input').forEach(function (input) {
			var wrapperId = input.id;
			var fieldName = wrapperId.replace('dp_', '');
			var hiddenEl = document.getElementById('hidden_' + fieldName);
			var valSpan = document.getElementById('val_' + fieldName);
			var withTime = input.placeholder.indexOf('HH:mm') !== -1;
			var locale = getBrowserLocale();

			var displayFmt = locale === 'hr'
				? (withTime ? 'd.m.Y H:i' : 'd.m.Y')
				: (withTime ? 'm/d/Y H:i' : 'm/d/Y');

			var defaultDate = null;
			if (hiddenEl && hiddenEl.value) {
				defaultDate = new Date(hiddenEl.value);
			}

			flatpickr(input, {
				locale: locale === 'hr' ? 'hr' : 'default',
				enableTime: withTime,
				dateFormat: displayFmt,
				defaultDate: defaultDate,
				time_24hr: true,

				onChange: function (selectedDates) {
					if (selectedDates.length > 0) {
						hiddenEl.value = selectedDates[0].toISOString();
						if (valSpan) valSpan.textContent = '';
					} else {
						hiddenEl.value = '';
					}
				},

				onClose: function () {
					if (valSpan && input.hasAttribute('required') && !hiddenEl.value) {
						valSpan.textContent = 'Datum je obavezan.';
					}
				}
			});
		});
	}

	document.addEventListener('DOMContentLoaded', initDatepickers);
})();
