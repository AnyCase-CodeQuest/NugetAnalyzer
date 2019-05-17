function GetSelect(selectListData) {
    var selectList = '<select class="form-control modal__select__branch" style="height: unset; padding: unset">';
    for (var i = 0; i < selectListData.length; i++) {
        selectList += '<option>' + selectListData[i] + '</option>';
    }
    selectList += '</select>';
    return selectList;
}

function GetSelectedRepositories() {
    var selectedCheckboxes = $(".modal input[type='checkbox']:checked");
    var selectedRepositories = {};
    
    for (var i = 0; i < selectedCheckboxes.length; i++) {
        var node = selectedCheckboxes.item(i).parent().parent();
        
        selectedRepositories[node.find(".modal__repository-name")[0].getAttribute("href")] =
            node.find(".modal__select__branch :selected").text();

    }
    //console.log(selectedRepositories);
    return selectedRepositories;
}

function BranchOnClick() {
    $(".modal .repository-branch").on('click',
        function () {
            var branchContainer = $(this).parent()[0];
            branchContainer.innerHTML = GetLoader();
            $.ajax({
                type: "GET",
                url: "/Repository/Branches?repositoryId=" + $(this)[0].getAttribute("value"),
                dataType: "json",
                success: function (data) {
                    branchContainer.innerHTML = GetSelect(data);
                }
            });
        });
}

function AddSelectedRepositoriesOnClick() {
    $('.modal input[type="submit"]').on('click',
        function () {
            var checkedRepositories = GetSelectedRepositories();
            $(".modal-wrapper").remove();
            $("body").append(GetFullScreenLoader());
            $.ajax({
                type: "POST",
                url: "/Repository/AddRepositories",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(checkedRepositories),
                dataType: "html",
                success: function (data) {

                },
                complete: function () {
                    $(".loader-wrapper").remove();
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
                url: "/Repository/AddRepositories",
                dataType: "html",
                success: function (data) {
                    $('body').append(data);
                    BranchOnClick();
                    AddSelectedRepositoriesOnClick();
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
