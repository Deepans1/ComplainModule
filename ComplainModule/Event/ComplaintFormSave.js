$.ajax({
    url: '@Url.Action("ActionName", "ControllerName")',
    type: "POST",
    async: true,
    dataType: "json",
    data: $('#form').serialize(),
    success: function (data) {
        // process result
    },
    error: function (request, status, error) {
        // process error message
    }
});