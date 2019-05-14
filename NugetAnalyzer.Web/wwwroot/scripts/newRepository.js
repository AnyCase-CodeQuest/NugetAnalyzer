
function NewRepositoryOnClick() {
    $("#new-repo-action").one('click',
        function () {
            $.ajax({
                type: "GET",
                url: "/Home/NewRepository",
                success: function (data) {
                    $('body').append(data);
                },
                complete: function () {
                    NewRepositoryOnClick();
                }
            });
        });
}

$(function () {
    NewRepositoryOnClick();
});

$(document).mouseup(function (e) {
    var container = $(".modal-content");
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        $("#new-repository-pop-up").remove();
        $(".modal-backdrop").remove();
    }
});