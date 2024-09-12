function muestraVentanaModal() {
    //alert('saludos');
    $('#modalButton').click(function () {
        $.ajax({
            url: '@Url.Action("_CreatePartial", "Matricula")',
            type: 'GET',
            success: function (data) {
                $('#modalContent').html(data);
            }
        });
        $('#myModal').modal('show');
    });
    $('#myModal').on('hidden.bs.modal', function () {
        $('#modalContent').html(''); // Limpia el contenido del modal cuando se cierra
    });
}









//function eliminar(valor) {
//    Swal.fire({
//        title: "Eliminar esta matrícula?",
//        showDenyButton: true,
//        confirmButtonText: `Si,eliminar!!!`,
//        denyButtonText: `No, salir!`
//    }).then((result) => {
//        if (result.isConfirmed) {
//            var url = '@Url.Action("EliminarMatricula", "Matricula",
//            new { id = "__matriculaId__" })'
//                .replace('__matriculaId__', valor);
//    window.location.href = url;
//}
//        });
//    }

