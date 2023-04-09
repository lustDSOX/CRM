$(document).ready(function () {  
	$('select').select2();

	const list = document.getElementById("ListSubj");
    const items = list.getElementsByTagName("li");

    for (let i = 0; i < items.length; i++) {
        items[i].addEventListener("click", function () {
            var index = Array.from(items).indexOf(this);
            console.log(index);
            var xhr = new XMLHttpRequest();
            xhr.open('GET', 'MainPage?handler=GetMailData&i=' + index);
            xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            xhr.onload = function () {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText); 
                    console.log(response.subj);
                    document.getElementById('currentMail').innerHTML = response.mail; 
                    document.getElementsByTagName('h4')[0].innerHTML = response.subj;
                }
                else {
                    console.log('Request failed.  Returned status of ' + xhr.status);
                }
            };
            xhr.send();
        });
    }
    $('.search').on('keyup', function() {
    	const searchText = $(this).val().toLowerCase();
    	$('#ListSubj li').each(function() {
      	const itemText = $(this).text().toLowerCase();
      	if (itemText.indexOf(searchText) !== -1) {
        	$(this).show();
      	} 
      	else {
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