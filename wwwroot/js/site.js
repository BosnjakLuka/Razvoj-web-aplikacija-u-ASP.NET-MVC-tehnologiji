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
