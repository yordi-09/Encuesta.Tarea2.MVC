$(document).ready(function () {
    cargarResultados();

    $('#btnAgregar').click(function () {
        var destino = prompt("Ingrese el nombre del destino de viaje:");
        if (destino && destino.trim() !== "") {
            $.ajax({
                url: '/Home/AgregarVoto',
                type: 'POST',
                data: { destino: destino },
                success: function (resp) {
                    if (resp.success) {
                        cargarResultados();
                    }
                }
            });
        }
    });
});

function cargarResultados() {
    $.ajax({
        url: '/Home/GetResultados',
        type: 'GET',
        success: function (data) {
            var $tbody = $('#gridResultados tbody');
            $tbody.empty();
            $.each(data, function (i, item) {
                var diff = item.Posicion === 1 ? '-' : (item.Diferencia > 0 ? '+' : '') + item.Diferencia.toFixed(2) + '%';
                $tbody.append(
                    '<tr>' +
                    '<td>' + item.Posicion + '</td>' +
                    '<td>' + item.Nombre + '</td>' +
                    '<td>' + item.Clasificacion.toFixed(2) + '%</td>' +
                    '<td>' + diff + '</td>' +
                    '</tr>'
                );
            });
        }
    });
}