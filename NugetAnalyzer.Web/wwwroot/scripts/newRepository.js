function GetLoader() {
    return '<div class="spinner-border spinner-border-sm" role="status">' +
        '<span class="sr-only">Loading...</span>' +
        '</div>';
}

function GetSelect(selectListData) {
    var selectList = '<select class="form-control" style="height: unset; padding: unset">';
    for (var i = 0; i < selectListData.length; i++) {
        selectList += '<option>' + selectListData[i] + '</option>';
    }
    selectList += '</select>';
    console.log(selectList);
    return selectList;
}

function BranchOnClick() {
    $(".repository-branch").on('click',
        function () {
            var container = $(this).parent()[0];
            container.innerHTML = GetLoader();
            $.ajax({
                type: "GET",
                url: "/Home/Branches?repositoryId=" + $(this)[0].getAttribute("value"),
                dataType: "json",
                success: function (data) {
                    container.innerHTML = GetSelect(data);
                },
                complete: function () {
                }
            });
        });
}

function NewRepositoryOnClick() {
    $("#new-repo-action").one('click',
        function () {
            $.ajax({
                type: "GET",
                url: "/Home/NewRepository",
                success: function (data) {
                    $('body').append(data);
                    BranchOnClick();
                },
                complete: function () {
                    NewRepositoryOnClick();
                }
            });
        });
}

$(document).mouseup(function (e) {
    var container = $(".modal-content");
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        $("#new-repository-pop-up").remove();
        $(".modal-backdrop").remove();
    }
});

$(function () {
    NewRepositoryOnClick();
});
