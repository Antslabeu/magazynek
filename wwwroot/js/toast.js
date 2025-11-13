window.ui = window.ui || {};
window.ui.toast = (text, type='success', ms=5400) => {
    const wrap = document.getElementById('toast-stack');
    const el = document.createElement('div');
    el.className = `toast-glass toast--${type}`;
    el.role = 'status';
    el.innerText = text;
    wrap.appendChild(el);
    // auto-hide
    setTimeout(()=> {
        el.classList.add('hide');
        el.addEventListener('animationend', ()=> el.remove(), { once:true });
    }, ms);
};