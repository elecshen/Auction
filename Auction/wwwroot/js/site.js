$(function () {
    // Блокировка отправки пустого поиска
    $("#search-form").on("submit", function (event) {
        // Проверяем, если поле ввода пустое
        if ($("#search").val().trim() === "") {
            event.preventDefault();
        }
    });

    // Функция для расчёта полосы прогресса
    function updateCountdown(progress, serverTime) {
        var startTime = new Date(progress.data('start-time'));
        var endTime = new Date(progress.data('end-time'));
        var now = new Date(serverTime);
        var progressBar = progress.find('.progress-bar');
        if (now < startTime) {
            progressBar.width('100%');
            progressBar.addClass('progress-bar-striped').addClass('progress-bar-animated');
        } else if (now < endTime) {
            var fillvalue = (now - startTime) / (endTime - startTime) * 100;
            progressBar.width(fillvalue + '%');
            progressBar.removeClass('progress-bar-striped').removeClass('progress-bar-animated').addClass('bg-success');
        } else {
            progressBar.width('100%');
            progressBar.removeClass('bg-success').addClass('bg-danger');
        }
    }
    //$.get("/api/getserverTime", function (serveerTime) {
    //    $('.card .progress').each(function () {
    //        updateCountdown($(this), serveerTime);
    //    });
    //});
    $('.card .progress').each(function () {
        updateCountdown($(this), new Date());
    });
});