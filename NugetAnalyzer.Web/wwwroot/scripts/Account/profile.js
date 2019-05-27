$("#repositories_tab").click(function () {
    $.ajax({
	        url: "/Repository/Report",
            success: function (result) {
	            $("#repositories").html(result);
			}
    });
});
