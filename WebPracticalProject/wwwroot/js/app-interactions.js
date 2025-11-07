(function () {
    document.addEventListener('DOMContentLoaded', () => {
        // Инициализация внутренних "тостов"
        FetchHelpers.setupToastHost();

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
                    // Можно тут же обновить UI (кнопки «Войти/Регистрация» -> «Профиль/Выход») по payload.user
                    AppToast.show('Вход выполнен', `Добро пожаловать, ${payload.user?.displayName || 'пользователь'}!`);
                    // Закрыть модалку
                    const modal = bootstrap.Modal.getInstance(document.getElementById('loginModal'));
                    modal?.hide();
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
                    AppToast.show('Аккаунт создан', 'Проверьте почту для подтверждения email.');
                    const modal = bootstrap.Modal.getInstance(document.getElementById('registerModal'));
                    modal?.hide();
                } else {
                    AppToast.show('Не удалось зарегистрировать', payload?.message || 'Попробуйте ещё раз', 'danger');
                }
            }
        });

        // 4) Динамическая подгрузка «Популярно сейчас»
        // На странице уже есть секция #catalog; найдём первую сетку карточек
        const catalogGrid = document.querySelector('#catalog .row.g-4') || document.querySelector('#catalog .row');
        if (catalogGrid) {
            loadPopular(catalogGrid);
        }

        async function loadPopular(gridEl) {
            try {
                // Пустая заглушка лоадера
                gridEl.innerHTML = `
          <div class="col-12 d-flex align-items-center gap-2">
            <div class="spinner-border" role="status" aria-hidden="true"></div>
            <span>Загрузка популярных позиций...</span>
          </div>`;
                const data = await FetchHelpers.fetchJson('/Catalog/Popular', { method: 'GET' });
                // Ожидаем массив: [{title, img, priceFrom}]
                gridEl.innerHTML = (data || []).map(renderCard).join('') || renderEmpty();
            } catch (e) {
                console.error(e);
                gridEl.innerHTML = `
          <div class="col-12">
            <div class="alert alert-danger">Не удалось загрузить каталог. Попробуйте позже.</div>
          </div>`;
            }
        }

        function renderCard(item) {
            const title = escapeHtml(item.title || 'Позиция каталога');
            const img = item.img || 'https://via.placeholder.com/800x600?text=No+Image';
            const price = item.priceFrom != null ? `Посуточно от ${item.priceFrom} €` : 'Цена по запросу';

            return `
        <div class="col-sm-6 col-lg-4">
          <div class="card h-100 border-0 shadow-sm">
            <div class="ratio ratio-4x3">
              <img src="${img}" class="card-img-top object-fit-cover" alt="${title}"/>
            </div>
            <div class="card-body d-flex flex-column">
              <h3 class="h5 card-title">${title}</h3>
              <p class="text-body-secondary small flex-grow-1">${price} · Залог по договору</p>
              <div class="d-flex gap-2">
                <a class="btn btn-primary" href="/Catalog/Details">Подробнее</a>
                <a class="btn btn-outline-primary" href="/Orders/Create">В прокат</a>
              </div>
            </div>
          </div>
        </div>`;
        }

        function renderEmpty() {
            return `
        <div class="col-12">
          <div class="alert alert-info">Популярных позиций пока нет.</div>
        </div>`;
        }

        function escapeHtml(str) {
            return String(str).replace(/[&<>"']/g, (m) => ({
                '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;'
            }[m]));
        }
    });
})();
