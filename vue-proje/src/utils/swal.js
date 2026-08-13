import Swal from 'sweetalert2'

const customSwal = Swal.mixin({
  background: '#1a1a1a', // Koyu gri arkaplan
  color: '#ffffff', // Beyaz metin
  confirmButtonColor: '#ff4d00', // Sitenin ana turuncu rengi
  cancelButtonColor: '#333333', // Koyu buton
  buttonsStyling: true,
  customClass: {
    popup: 'swal2-dark-popup',
    title: 'swal2-dark-title',
    confirmButton: 'swal2-dark-confirm',
    cancelButton: 'swal2-dark-cancel'
  }
})

export default customSwal
