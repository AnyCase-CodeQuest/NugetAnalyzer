$(function () {
    $('[data-toggle="tooltip"]').tooltip();

    $.ajaxSetup({
        error: function (x, status, error) {
            //window.location.href = "/Error/Error";  //TODO: setup ajax error
        }
    });
})