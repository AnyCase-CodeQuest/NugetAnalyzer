$(function() {
    $('[data-toggle="tooltip"]').tooltip();
    $('.toast').toast("show");

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
        '<div class="modal show">' +
        '<div class="loader-content">' + GetLoader(5) + '</div>' + 
        '</div>' +
        '<div class="modal-backdrop loader__modal-backdrop show"></div>' +
        '</div>';
}

function GetAlert(message, alertType = "success") {
    return '';
}