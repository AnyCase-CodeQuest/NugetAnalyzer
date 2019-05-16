$(function() {
    $('[data-toggle="tooltip"]').tooltip();

    $.ajaxSetup({
        error: function(x, status, error) {
            //window.location.href = "/Error/Error";  //TODO: setup ajax error
        }
    });
});

function GetLoader(sizeInRem = 0) {
    return sizeInRem == undefined || sizeInRem <= 0
        ? '<div class="spinner-border spinner-border-sm" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>'
        : '<div class="spinner-border" style="width: ' + sizeInRem + 'rem; height: ' + sizeInRem + 'rem;" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>';
}

function GetFullScreenLoader() {
    return '<div class="loader-wrapper">' +
        '<div class="modal show" style="display: block;">' +
        '<div style="display: flex; justify-content: center; align-items: center; height: 100%">' + GetLoader(5) + '</div>' + 
        '</div>' +
        '<div class="modal-backdrop show" style="opacity: 0.15"></div>' +
        '</div>';
}