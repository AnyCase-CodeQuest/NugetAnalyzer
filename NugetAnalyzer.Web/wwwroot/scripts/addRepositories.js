function getSelect(selectListData) {
    var selectList = '<select class="form-control modal__select-repository-branch">';
    for (var i = 0; i < selectListData.length; i++) {
        selectList += '<option>' + selectListData[i] + '</option>';
    }
    selectList += '</select>';
    return selectList;
}

function getSelectedRepositories() {
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

function branchOnClick() {
    $(".modal .modal__repository-branch").on('click',
        function () {
            var branchContainer = $(this).parent()[0];
            branchContainer.innerHTML = getLoader();
            $.ajax({
                type: "GET",
                url: "/Repository/BranchNames?repositoryId=" + $(this)[0].getAttribute("value"),
                dataType: "json",
                success: function (data) {
                    branchContainer.innerHTML = getSelect(data);
                }
            });
        });
}

function addSelectedRepositoriesOnClick(isFromLayout) {
    $('.modal input[type="submit"]').on('click',
        function () {
            var checkedRepositories = getSelectedRepositories();
            if (Object.keys(checkedRepositories).length == 0) {
                return;
            }
            $(".modal-wrapper").remove();
            $("body").append(getFullScreenLoader());
            $.ajax({
                type: "POST",
                url: "/Repository/AddRepositories",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ repositories: checkedRepositories, isFromLayout: isFromLayout }),
                complete: async function (data) {
                    $(".loader-wrapper").remove();
                    if (isFromLayout) {
                        $("body").append(data.responseText);
                        await sleep(10);
                        $(".modal.fade")[0].classList.add("show");
                    }
                    else { // TODO
                        $("#repositories-id")[0].append(data.responseText);
                    }
                }
            });
        });
}


function toggleAllRepositoriesOnClick() {
    $(".modal thead input[type='checkbox']").on('click', function () {
        var isChecked = $(".modal thead input[type='checkbox']")[0].checked;
        var checkboxes = $(".modal tbody input[type='checkbox']");
        for (var i = 0; i < checkboxes.length; i++) {
            checkboxes[i].checked = isChecked;
        }
    });
}

function addRepositories(isFromLayout) {
    $("body").append(getFullScreenLoader());
    $.ajax({
        type: "GET",
        url: "/Repository/GetNotAddedRepositories",
        dataType: "html",
        success: async function (data) {
            $('body').append(data);
            await sleep(10);
            $(".modal.fade")[0].classList.add("show");
            branchOnClick();
            toggleAllRepositoriesOnClick();
            addSelectedRepositoriesOnClick(isFromLayout);
        },
        complete: function () {
            $(".loader-wrapper").remove();
            addRepositoriesOnClick();
        }
    });
}

function addRepositoriesOnClick() {
    $("#add-repositories_layout-action").one('click', function () {
        addRepositories(true);
    });
    $("#add-repositories_profile-action").one('click', function () {
        addRepositories(false);
    });
}

$(document).mouseup(function (e) {
    var container = $(".modal-content");
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        $(".modal-wrapper").remove();
    }
});

$(function () {
    addRepositoriesOnClick();
});
