$(document).ready(function () {
    cargarResultados();
});

function cargarResultados() {
    $.ajax({
        url: '/Home/ObtenerResultados',
        type: 'GET',
        success: function (data) {
            var $tbody = $('#gridResultados tbody');
            $tbody.empty();
            $.each(data, function (i, item) {
                var diff;
                if (item.Posicion === 1) {
                    diff = 'No aplica';
                } else {
                    diff = '+' + Math.abs(item.Diferencia).toFixed(2) + '%';
                }
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