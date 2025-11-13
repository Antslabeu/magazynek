window.ui = window.ui || {};
  window.ui.confirm = (title, text) => new Promise(resolve => {
    Swal.fire({
      title, text, icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Tak',
      cancelButtonText: 'Anuluj',
      reverseButtons: true,
      buttonsStyling: false,
      customClass: {
        popup: 'lg-modal',
        confirmButton: 'btn-glass btn-success',
        cancelButton: 'btn-glass btn-ghost'
      },
      backdrop: true
    }).then(r => resolve(r.isConfirmed));
  });