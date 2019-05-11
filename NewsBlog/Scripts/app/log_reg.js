$(document).ready(function () {

    $.ajaxSetup({
        cache: false
    });

    $(".registerDialog").on("click", function (e) {
        e.preventDefault();

        $("<div id='dialogContent'></div>")
            .addClass("dialog")
            .appendTo("body")
            .load(this.href)
            .dialog({
                title: $(this).attr("data-dialog-title"),
                close: function () {
                    $(this).remove()
                },
                modal: true,
                buttons: {
                    "OK": function () {
                        $.ajax({
                            url: '/Account/Register/',
                            type: "POST",
                            data: $('form').serialize(),
                            datatype: "json",
                            success: function (result) {
                                $("#dialogContent").html(result);
                            }
                        });
                    }
                },
                closeText: ''
            });
    });
    $(".close").on("click", function (e) {
        e.preventDefault();
        $(this).closest(".dialog").dialog("close");
    });
});
$(document).ready(function () {

    $.ajaxSetup({
        cache: false
    });

    $(".viewDialog").on("click", function (e) {
        e.preventDefault();

        $("<div id='dialogContent'></div>")
            .addClass("dialog")
            .appendTo("body")
            .load(this.href)
            .dialog({
                title: $(this).attr("data-dialog-title"),
                close: function () {
                    $(this).remove()
                },
                modal: true,
                buttons: {
                    "OK": function () {
                        $.ajax({
                            url: '/Account/Login/',
                            type: "POST",
                            data: $('form').serialize(),
                            datatype: "json",
                            success: function (result) {
                                $("#dialogContent").html(result);
                            }
                        });
                    }
                },
                closeText: ''
            });
    });
    $(".close").on("click", function (e) {
        e.preventDefault();
        $(this).closest(".dialog").dialog("close");
    });
});