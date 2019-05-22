function GetSelect(selectListData) {
    var selectList = '<select class="form-control modal__select-repository-branch">';
    for (var i = 0; i < selectListData.length; i++) {
        selectList += '<option>' + selectListData[i] + '</option>';
    }
    selectList += '</select>';
    return selectList;
}

function GetSelectedRepositories() {
    var selectedCheckboxes = $(".modal tbody input[type='checkbox']:checked");
    var selectedRepositories = {};
    for (var i = 0; i < selectedCheckboxes.length; i++) {
        var trNode = selectedCheckboxes[i].parentNode.parentNode;
        var repositoryUrl = trNode.querySelector(".modal__repository-name").getAttribute("href");
        var repositoryBranch = trNode.querySelector(".modal__repository-branch") != undefined
            ? trNode.querySelector(".modal__repository-branch span:nth-child(1)").innerText
            : trNode.querySelector(".modal__select-repository-branch :checked").text;

        selectedRepositories[repositoryUrl] = repositoryBranch;
    }
    return selectedRepositories;
}

function BranchOnClick() {
    $(".modal .modal__repository-branch").on('click',
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

function AddSelectedRepositoriesOnClick(isFromLayout) {
    $('.modal input[type="submit"]').on('click',
        function () {
            var checkedRepositories = GetSelectedRepositories();
            if (Object.keys(checkedRepositories).length == 0) {
                return;
            }
            $(".modal-wrapper").remove();
            $("body").append(GetFullScreenLoader());
            $.ajax({
                type: "POST",
                url: "/Repository/AddRepositories",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ repositories: checkedRepositories, isFromLayout: isFromLayout }),
                complete: async function (data) {
                    console.log(data);
                    $(".loader-wrapper").remove();
                    $("body").append(data.responseText);
                    await Sleep(10);
                    $(".modal.fade")[0].classList.add("show");
                }
            });
        });
}


function ToggleAllRepositoriesOnClick() {
    $(".modal thead input[type='checkbox']").on('click', function () {
        var isChecked = $(".modal thead input[type='checkbox']")[0].checked;
        var checkboxes = $(".modal tbody input[type='checkbox']");
        for (var i = 0; i < checkboxes.length; i++) {
            checkboxes[i].checked = isChecked;
        }
    });
}

function AddRepositories(isFromLayout) {
    $("body").append(GetFullScreenLoader());
    $.ajax({
        type: "GET",
        url: "/Repository/AddRepositories",
        dataType: "html",
        success: async function (data) {
            $('body').append(data);
            await Sleep(10);
            $(".modal.fade")[0].classList.add("show");
            BranchOnClick();
            ToggleAllRepositoriesOnClick();
            AddSelectedRepositoriesOnClick(isFromLayout);
        },
        complete: function () {
            $(".loader-wrapper").remove();
            AddRepositoriesOnClick();
        }
    });
}

function AddRepositoriesOnClick() {
    $("#add-repositories_layout-action").one('click', function () {
        AddRepositories(true);
    });
    $("#add-repositories_profile-action").one('click', function () {
        AddRepositories(false);
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
