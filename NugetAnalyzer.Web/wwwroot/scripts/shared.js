$(function() {
    $('[data-toggle="tooltip"]').tooltip();
    $('.toast').toast("show");

    $(document).on('click', ".modal__button-ok", function() {
        $(".modal-wrapper").remove();
    });

    $.ajaxSetup({
        statusCode: {
            500: function (data) {
                $("body")[0].innerHTML = data.responseText;
            },
            404: function (data) {
                var container = $("#body-container");
                container.removeChild(container.childNodes[0]);
                container.insertBefore(data.responseText, container.firstChild);
            }
        }
    });
});

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function getLoader(sizeInRem = 0) {
    return sizeInRem == undefined || sizeInRem <= 0
        ? '<div class="spinner-border spinner-border-sm" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>'
        : '<div class="spinner-border" style="width: ' + sizeInRem + 'rem; height: ' + sizeInRem + 'rem;" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>';
}

function getFullScreenLoader() {
    return '<div class="loader-wrapper">' +
        '<div class="modal show">' +
        '<div class="loader__content">' + getLoader(5) + '</div>' + 
        '</div>' +
        '<div class="modal-backdrop loader__modal-backdrop show"></div>' +
        '</div>';
}