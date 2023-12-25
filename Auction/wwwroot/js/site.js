$(function () {

    $(".input-validation-error").each(function () {
        $(this).removeClass('input-validation-error').addClass("is-invalid");
    });

    // Блокировка отправки пустого поиска
    $("#search-form").on("submit", function (event) {
        // Проверяем, если поле ввода пустое
        if ($("#search").val().trim() === "") {
            event.preventDefault();
        }
    });

    // Блокировка отправки пустой ставки
    $("#bid-form").on("submit", function (event) {
        // Проверяем, если поле ввода пустое
        if ($("#bid").val().trim() === "") {
            event.preventDefault();
        }
    });

    // Функция для расчёта полосы прогресса
    function updateCountdown(progress, serverTime) {
        let isClosed = progress.data('closed'); 
        let startTime = new Date(progress.data('start-time'));
        let endTime = new Date(progress.data('end-time'));
        let now = new Date(serverTime);
        let progressBar = progress.find('.progress-bar');
        if (now < endTime && isClosed === 'False') {
            let fillvalue = (now - startTime) / (endTime - startTime) * 100;
            progressBar.width(fillvalue + '%');
            progressBar.addClass('progress-bar-striped').addClass('progress-bar-animated').addClass('bg-success');
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