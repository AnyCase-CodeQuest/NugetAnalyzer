function GetSelect(selectListData) {
    var selectList = '<select class="form-control" style="height: unset; padding: unset">';
    for (var i = 0; i < selectListData.length; i++) {
        selectList += '<option>' + selectListData[i] + '</option>';
    }
    selectList += '</select>';
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

function AddRepositoriesOnClick() {
    $("#new-repo-action").one('click',
        function () {
            $("body").append(GetFullScreenLoader());
            $.ajax({
                type: "GET",
                url: "/Home/AddRepositories",
                success: function (data) {
                    $('body').append(data);
                    BranchOnClick();
                },
                complete: function () {
                    $(".loader-wrapper").remove();
                    AddRepositoriesOnClick();
                }
            });
        });
}

$(document).mouseup(function (e) {
    var container = $(".modal-content");
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        $(".modal-wrapper").remove();
    }
});

$(function () {
    AddRepositoriesOnClick();
});
