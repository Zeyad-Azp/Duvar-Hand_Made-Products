// ═══════════════════════════════════════════════════════════════════
//  DUVAR — site.js
// ═══════════════════════════════════════════════════════════════════

/* ── Dark Mode ──────────────────────────────────────────────────
   Persists preference in localStorage.
   Adds / removes [data-theme="dark"] on <html>.
   Works on both public layout and admin layout.
─────────────────────────────────────────────────────────────── */
(function initTheme() {
  const saved = localStorage.getItem('duvar-theme');
  // Apply immediately (before paint) to avoid flash
  if (saved === 'dark') {
    document.documentElement.setAttribute('data-theme', 'dark');
  }
})();

document.addEventListener('DOMContentLoaded', function () {

  // Attach all toggle buttons (public nav + admin topbar may both have one)
  document.querySelectorAll('.theme-toggle').forEach(function (btn) {
    btn.addEventListener('click', function () {
      const isDark = document.documentElement.getAttribute('data-theme') === 'dark';
      if (isDark) {
        document.documentElement.removeAttribute('data-theme');
        localStorage.setItem('duvar-theme', 'light');
      } else {
        document.documentElement.setAttribute('data-theme', 'dark');
        localStorage.setItem('duvar-theme', 'dark');
      }
    });
  });

  /* ── Navbar scroll shadow ────────────────────────────────── */
  const nav = document.querySelector('.duvar-nav');
  if (nav) {
    window.addEventListener('scroll', function () {
      nav.style.boxShadow = window.scrollY > 30
        ? '0 2px 20px rgba(0,0,0,.12)'
        : 'none';
    }, { passive: true });
  }

  /* ── Auto-dismiss TempData toast ────────────────────────── */
  const tempToast = document.getElementById('temp-toast');
  if (tempToast) {
    tempToast.style.transition = 'opacity .4s ease, transform .4s ease';
    setTimeout(function () {
      tempToast.style.opacity   = '0';
      tempToast.style.transform = 'translateY(-10px)';
      setTimeout(function () { tempToast.remove(); }, 400);
    }, 3500);
  }

  /* ── Lightbox ────────────────────────────────────────────── */
  const overlay  = document.getElementById('lightbox-overlay');
  if (overlay) {
    const img     = overlay.querySelector('.lightbox-img');
    const closeBtn= overlay.querySelector('.lightbox-close');
    const prevBtn = overlay.querySelector('.lightbox-prev');
    const nextBtn = overlay.querySelector('.lightbox-next');
    let images = [], current = 0;

    function openLightbox(srcs, idx) {
      images = srcs; current = idx;
      img.src = images[current];
      overlay.classList.add('active');
      document.body.style.overflow = 'hidden';
    }
    function closeLightbox() {
      overlay.classList.remove('active');
      document.body.style.overflow = '';
      setTimeout(function () { img.src = ''; }, 400);
    }
    function navigate(dir) {
      current = (current + dir + images.length) % images.length;
      img.style.opacity = '0';
      setTimeout(function () { img.src = images[current]; img.style.opacity = '1'; }, 150);
    }
    img.style.transition = 'opacity .2s ease';

    if (closeBtn) closeBtn.addEventListener('click', closeLightbox);
    if (prevBtn)  prevBtn.addEventListener('click',  function () { navigate(-1); });
    if (nextBtn)  nextBtn.addEventListener('click',  function () { navigate(1);  });
    overlay.addEventListener('click', function (e) { if (e.target === overlay) closeLightbox(); });
    document.addEventListener('keydown', function (e) {
      if (!overlay.classList.contains('active')) return;
      if (e.key === 'Escape')      closeLightbox();
      if (e.key === 'ArrowLeft')   navigate(-1);
      if (e.key === 'ArrowRight')  navigate(1);
    });

    // Gallery thumbnails → lightbox
    document.querySelectorAll('[data-lightbox]').forEach(function (el, i, all) {
      el.addEventListener('click', function () {
        openLightbox(Array.from(all).map(function (e) { return e.dataset.lightbox; }), i);
      });
    });

    // Product detail main image → lightbox
    document.querySelectorAll('[data-lightbox-single]').forEach(function (el) {
      el.addEventListener('click', function () {
        openLightbox([el.dataset.lightboxSingle], 0);
      });
    });

    window.openLightbox = openLightbox;
  }

  /* ── Product Detail Thumbnails ───────────────────────────── */
  const mainImg = document.getElementById('product-main-img');
  if (mainImg) {
    mainImg.style.transition = 'opacity .25s ease';
    document.querySelectorAll('.product-thumb').forEach(function (thumb) {
      thumb.addEventListener('click', function () {
        document.querySelectorAll('.product-thumb').forEach(function (t) { t.classList.remove('active'); });
        thumb.classList.add('active');
        mainImg.style.opacity = '0';
        setTimeout(function () { mainImg.src = thumb.dataset.src; mainImg.style.opacity = '1'; }, 200);
      });
    });
  }

  /* ── AJAX Product Filtering ──────────────────────────────── */
  var filterChips  = document.querySelectorAll('.filter-chip[data-category]');
  var productGrid  = document.getElementById('product-grid');
  var searchInput  = document.getElementById('product-search');
  var loadingEl    = document.getElementById('products-loading');

  if (productGrid) {
    var currentCategory = null;
    var searchDebounce  = null;

    function showLoading() { if (loadingEl) loadingEl.style.display = 'flex'; if (productGrid) productGrid.style.opacity = '.4'; }
    function hideLoading() { if (loadingEl) loadingEl.style.display = 'none';  if (productGrid) productGrid.style.opacity = '1';  }

    function fetchProducts() {
      showLoading();
      var params = new URLSearchParams();
      if (currentCategory) params.set('categoryId', currentCategory);
      if (searchInput && searchInput.value.trim()) params.set('search', searchInput.value.trim());
      fetch('/Products/FilterPartial?' + params)
        .then(function (r) { return r.text(); })
        .then(function (html) { productGrid.innerHTML = html; })
        .catch(function (e) { console.error('Filter error:', e); })
        .finally(hideLoading);
    }

    filterChips.forEach(function (chip) {
      chip.addEventListener('click', function () {
        filterChips.forEach(function (c) { c.classList.remove('active'); });
        chip.classList.add('active');
        currentCategory = chip.dataset.category || null;
        fetchProducts();
      });
    });

    if (searchInput) {
      searchInput.addEventListener('input', function () {
        clearTimeout(searchDebounce);
        searchDebounce = setTimeout(fetchProducts, 400);
      });
    }
  }

  /* ── Admin: existing image management (Edit page) ────────── */
  var deleteInput = document.getElementById('delete-image-ids');
  var mainInput   = document.getElementById('main-image-id');

  document.querySelectorAll('.admin-img-item').forEach(function (item) {
    // Click → set as main
    item.addEventListener('click', function (e) {
      if (e.target.closest('.delete-btn')) return;
      if (!mainInput) return;
      document.querySelectorAll('.admin-img-item').forEach(function (i) {
        i.classList.remove('is-main');
        var b = i.querySelector('.main-badge');
        if (b) b.remove();
      });
      item.classList.add('is-main');
      var badge = document.createElement('span');
      badge.className = 'main-badge';
      badge.textContent = 'Main';
      item.prepend(badge);
      mainInput.value = item.dataset.id;
    });

    // Delete button
    var delBtn = item.querySelector('.delete-btn');
    if (delBtn && deleteInput) {
      delBtn.addEventListener('click', function (e) {
        e.stopPropagation();
        item.style.opacity = '.25';
        item.style.pointerEvents = 'none';
        var cur = deleteInput.value ? deleteInput.value.split(',') : [];
        if (!cur.includes(item.dataset.id)) cur.push(item.dataset.id);
        deleteInput.value = cur.filter(Boolean).join(',');
      });
    }
  });

  /* ── Admin: file input preview (category image, edit new images) */
  document.querySelectorAll('.image-file-input').forEach(function (input) {
    input.addEventListener('change', function () {
      var preview = document.getElementById(this.dataset.preview);
      if (!preview) return;
      preview.innerHTML = '';
      Array.from(this.files).forEach(function (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
          var div = document.createElement('div');
          div.className = 'admin-img-item';
          div.style.opacity = '.7';
          div.innerHTML = '<img src="' + e.target.result + '" alt="preview" />';
          preview.appendChild(div);
        };
        reader.readAsDataURL(file);
      });
    });
  });

});

/* ── Programmatic toast ──────────────────────────────────────── */
window.showToast = function (message, type) {
  type = type || 'success';
  var toast = document.createElement('div');
  toast.className = 'duvar-toast ' + type;
  toast.innerHTML = '<span>' + message + '</span>';
  document.body.appendChild(toast);
  requestAnimationFrame(function () { toast.classList.add('show'); });
  setTimeout(function () {
    toast.classList.remove('show');
    setTimeout(function () { toast.remove(); }, 400);
  }, 3500);
};
