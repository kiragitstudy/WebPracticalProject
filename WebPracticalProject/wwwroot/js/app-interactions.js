(function () {
    document.addEventListener('DOMContentLoaded', () => {
        // Инициализация внутренних "тостов"
        FetchHelpers.setupToastHost();

        try {
            const url = new URL(window.location.href);
            const params = url.searchParams;

            const emailStatus = params.get('email_confirmed');
            if (emailStatus) {
                if (emailStatus === 'ok') {
                    AppToast.show('Email подтверждён', 'Вы теперь проверенный пользователь.', 'success');
                } else if (emailStatus === 'fail') {
                    AppToast.show('Ошибка подтверждения', 'Ссылка недействительна или устарела.', 'danger');
                } else if (emailStatus === 'invalid') {
                    AppToast.show('Некорректная ссылка', 'Проверьте адрес или запросите новое письмо.', 'warning');
                }
                params.delete('email_confirmed');
            }

            // 2) login=google_ok/google_fail
            const loginStatus = params.get('login');
            if (loginStatus) {
                if (loginStatus === 'google_ok') {
                    AppToast.show('Вход выполнен', 'Вы вошли через Google.', 'success');
                } else if (loginStatus === 'google_fail') {
                    AppToast.show('Не удалось войти через Google', 'Попробуйте ещё раз или войдите по email/паролю.', 'danger');
                }
                params.delete('login');
            }

            // если что-то удалили — чистим URL
            const newUrl =
                url.pathname +
                (params.toString() ? '?' + params.toString() : '') +
                url.hash;
            window.history.replaceState({}, '', newUrl);
        } catch (e) {
            console.error('Toast URL parse error:', e);
        }
        
        // 1) AJAX-отправка «Написать сообщение» (/Contact/Send)
        const messageForm = document.querySelector('#message form');
        FetchHelpers.wireUpAjaxForm(messageForm, {
            onSuccess: (payload, form) => {
                // ожидаем { ok: true, id: ... }
                if (payload?.ok) {
                    form.reset();
                    form.classList.remove('was-validated');
                    FetchHelpers.setupToastHost();
                    AppToast.show('Сообщение отправлено', 'Мы ответим на email в течение дня.');
                } else {
                    AppToast.show('Не отправлено', payload?.message || 'Попробуйте ещё раз', 'warning');
                }
            }
        });

        // 2) AJAX-логин (модалка Login)
        const loginForm = document.querySelector('#loginModal form');
        FetchHelpers.wireUpAjaxForm(loginForm, {
            onSuccess: (payload, form) => {
                if (payload?.ok) {
                    AppToast.show('Вход выполнен', `Добро пожаловать, ${payload.user?.displayName || 'пользователь'}!`);
                    const modal = bootstrap.Modal.getInstance(document.getElementById('loginModal'));
                    modal?.hide();
                    window.location.reload();
                } else {
                    AppToast.show('Ошибка входа', payload?.message || 'Неверные данные', 'danger');
                }
            }
        });

        // 3) AJAX-регистрация (модалка Register)
        const registerForm = document.querySelector('#registerModal form');
        FetchHelpers.wireUpAjaxForm(registerForm, {
            onSuccess: (payload, form) => {
                if (payload?.ok) {
                    const modal = bootstrap.Modal.getInstance(document.getElementById('registerModal'));
                    modal?.hide();
                    window.location.reload();
                } else {
                    AppToast.show('Не удалось зарегистрировать', payload?.message || 'Попробуйте ещё раз', 'danger');
                }
            }
        });

        // 4) Динамическая подгрузка разделов каталога (категории)
        const catalogGrid = document.querySelector('#catalog .row.g-4') || document.querySelector('#catalog .row');
        if (catalogGrid) {
            loadCatalogCategories(catalogGrid);
        }

        async function loadCatalogCategories(gridEl) {
            try {
                gridEl.innerHTML = `
          <div class="col-12 d-flex align-items-center gap-2">
            <div class="spinner-border" role="status" aria-hidden="true"></div>
            <span>Загрузка разделов каталога...</span>
          </div>`;

                // Ожидаем: [{ key, name, description, imageUrl, instrumentsCount }]
                const data = await FetchHelpers.fetchJson('/Catalog/Categories', { method: 'GET' });

                if (!data || !Array.isArray(data) || data.length === 0) {
                    gridEl.innerHTML = renderEmptyCategories();
                    return;
                }

                gridEl.innerHTML = data.map(renderCategoryCard).join('');
            } catch (e) {
                console.error(e);
                gridEl.innerHTML = `
          <div class="col-12">
            <div class="alert alert-danger">Не удалось загрузить разделы каталога. Попробуйте позже.</div>
          </div>`;
            }
        }

        function renderCategoryCard(item) {
            const key = encodeURIComponent(item.key || item.systemName || '');
            const title = escapeHtml(item.name || 'Категория');
            const description = escapeHtml(item.description || 'Музыкальные инструменты этого типа доступны для аренды.');
            const img = item.imageUrl || 'https://via.placeholder.com/800x600?text=Category';
            const count = item.instrumentsCount ?? 0;
            const countText = count === 1
                ? '1 инструмент'
                : `${count} инструментов`;

            return `
        <div class="col-12 col-md-6 col-lg-4">
          <a href="/Catalog?category=${key}" class="text-decoration-none text-reset">
            <div class="card h-100 border-0 shadow-sm">
              <div class="ratio ratio-16x9">
                <img src="${img}" class="card-img-top object-fit-cover" alt="${title}" />
              </div>
              <div class="card-body">
                <h3 class="h5 card-title mb-1">${title}</h3>
                <p class="text-body-secondary small mb-2">${description}</p>
                <span class="badge bg-info text-body-secondary">
                  ${countText}
                </span>
              </div>
            </div>
          </a>
        </div>`;
        }

        function renderEmptyCategories() {
            return `
        <div class="col-12">
          <div class="alert alert-info">Разделы каталога пока не настроены.</div>
        </div>`;
        }

        function escapeHtml(str) {
            return String(str).replace(/[&<>"']/g, (m) => ({
                '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;'
            }[m]));
        }

        const rentalForm = document.querySelector('#rentalForm');
        FetchHelpers.wireUpAjaxForm(rentalForm, {
            onSuccess: (payload, form) => {
                if (payload?.ok) {
                    AppToast.show('Аренда оформлена', 'Бронирование создано.');
                    window.location.href = payload.redirectUrl || '/Rentals/My';
                } else {
                    const msg = payload?.message || 'Не удалось оформить аренду';
                    AppToast.show('Ошибка', msg, 'danger');
                }
            }
        });

    });
})();
