$(document).ready(function () {  
    // Убрать поиск у всех select на странице
    $('select').select2({
      minimumResultsForSearch: Infinity
    });

  $('.search').on('input', function() {
    const searchText = $(this).val().toLowerCase();
    $('#ListSubj li').each(function() {
      const itemText = $(this).text().toLowerCase();
      if (itemText.indexOf(searchText) !== -1) {
        $(this).show();
      } else {
        $(this).hide();
      }
    });
});

   $("#ListSubj li button").click(function() {
      // удаляем класс "selected" у всех элементов списка
      $("#ListSubj li button").removeClass("selected");
      // добавляем класс "selected" к выбранному элементу списка
      $(this).addClass("selected");
    });
   
});