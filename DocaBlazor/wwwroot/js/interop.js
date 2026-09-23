window.docaInterop = {
    copyText: async function (text) {
        try {
            if (navigator.clipboard && window.isSecureContext) {
                await navigator.clipboard.writeText(text);
                return;
            }
        } catch {
            // HTTP sem contexto seguro: cai no fallback abaixo.
        }

        const ta = document.createElement('textarea');
        ta.value = text;
        ta.setAttribute('readonly', '');
        ta.style.position = 'fixed';
        ta.style.left = '-9999px';
        document.body.appendChild(ta);
        ta.select();
        const ok = document.execCommand('copy');
        document.body.removeChild(ta);
        if (!ok) throw new Error('Não foi possível copiar o texto.');
    },

    loadTree: function (key) {
        try {
            const value = localStorage.getItem(key);
            if (!value || value.trim() === '' || value.trim() === '[]') {
                localStorage.removeItem(key);
                return null;
            }
            return value;
        } catch {
            return null;
        }
    },

    saveTree: function (key, json) {
        try {
            localStorage.setItem(key, json);
            return true;
        } catch {
            return false;
        }
    },

    clearTree: function (key) {
        try {
            localStorage.removeItem(key);
            return true;
        } catch {
            return false;
        }
    },

    confirm: function (message) {
        return window.confirm(message);
    }
};
