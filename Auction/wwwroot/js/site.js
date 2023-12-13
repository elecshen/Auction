// Блокировка отправки пустого поиска
$(function () {
    $("#search-form").on("submit", function (event) {
        // Проверяем, если поле ввода пустое
        if ($("#search").val().trim() === "") {
            event.preventDefault();
        }
    });
});