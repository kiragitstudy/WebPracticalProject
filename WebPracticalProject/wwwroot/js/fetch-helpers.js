(function () {
    // Находим hidden __RequestVerificationToken в конкретной форме
    function getAntiForgeryToken(form) {
        if (!form) return null;
        const input = form.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : null;
    }

    // Универсальный JSON fetch с поддержкой AntiForgery header
    async function fetchJson(url, { method = 'GET', headers = {}, body = null, token = null } = {}) {
        const finalHeaders = new Headers(headers);
        finalHeaders.set('Accept', 'application/json');
        if (body && !(body instanceof FormData)) {
            finalHeaders.set('Content-Type', 'application/json');
        }
        if (token) {
            // ASP.NET Core по умолчанию принимает именно этот заголовок
            finalHeaders.set('RequestVerificationToken', token);
        }

        const response = await fetch(url, { method, headers: finalHeaders, body });
        const isJson = response.headers.get('content-type')?.includes('application/json');
        const payload = isJson ? await response.json().catch(() => null) : null;

        if (!response.ok) {
            const message = payload?.message || payload?.error || response.statusText;
            const err = new Error(message);
            err.status = response.status;
            err.payload = payload;
            throw err;
        }
        return payload;
    }

    // Небольшой helper для превентивной валидации Bootstrap + fetch сабмита
    function wireUpAjaxForm(form, { onSuccess, onError } = {}) {
        if (!form) return;
        form.addEventListener('submit', async (e) => {
            // progressive enhancement: если HTML5-валидация не прошла — стоп
            if (!form.checkValidity()) {
                form.classList.add('was-validated');
                e.preventDefault();
                e.stopPropagation();
                return;
            }

            // перехватываем — шлём через fetch
            e.preventDefault();

            const submitBtn = form.querySelector('[type="submit"]');
            const originalText = submitBtn ? submitBtn.innerHTML : '';
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Отправка...';
            }

            try {
                const token = getAntiForgeryToken(form);
                let body;
                let headers = {};
                // Если есть файл — используем FormData, иначе — JSON
                if (form.querySelector('input[type="file"]')) {
                    body = new FormData(form);
                } else {
                    const formData = new FormData(form);
                    body = JSON.stringify(Object.fromEntries(formData.entries()));
                    headers['Content-Type'] = 'application/json';
                }

                const payload = await fetchJson(form.action, {
                    method: (form.getAttribute('method') || 'POST').toUpperCase(),
                    headers,
                    body,
                    token
                });

                onSuccess && onSuccess(payload, form);
            } catch (err) {
                onError && onError(err, form);
                console.error(err);
                window.AppToast?.show('Ошибка', err.message || 'Не удалось выполнить запрос', 'danger');
            } finally {
                if (submitBtn) {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = originalText;
                }
            }
        });
    }

    // Простой Bootstrap alert/toast-стаб (без доп. HTML) — рендерим «тосты» в правом верхнем углу
    function setupToastHost() {
        if (document.getElementById('app-toast-host')) return;
        const host = document.createElement('div');
        host.id = 'app-toast-host';
        host.style.position = 'fixed';
        host.style.top = '1rem';
        host.style.right = '1rem';
        host.style.zIndex = '1060';
        host.style.maxWidth = '360px';
        document.body.appendChild(host);

        window.AppToast = {
            show(title, text, variant = 'success', timeout = 3500) {
                const wrapper = document.createElement('div');
                wrapper.className = `alert alert-${variant} shadow mb-2`;
                wrapper.innerHTML = `<div class="fw-semibold mb-1">${title}</div><div>${text || ''}</div>`;
                host.appendChild(wrapper);
                setTimeout(() => {
                    wrapper.classList.add('fade', 'show');
                    setTimeout(() => wrapper.remove(), 150);
                }, timeout);
            }
        };
    }

    // Глобально доступно
    window.FetchHelpers = { getAntiForgeryToken, fetchJson, wireUpAjaxForm, setupToastHost };
})();
