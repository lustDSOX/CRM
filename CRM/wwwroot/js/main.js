$(document).ready(function () {
    const list = document.getElementById("ListSubj");
    const items = list.getElementsByTagName("li");

    for (let i = 0; i < items.length; i++) {//проходимся по все м элементам списка и на клик ставим слудующую функцию 
        items[i].addEventListener("click", function () {
            var index = Array.from(items).indexOf(this);//получаем индекс
            console.log(index);
            var xhr = new XMLHttpRequest();
            xhr.open('GET', 'MainPage?handler=GetMailData&i=' + index);//get запрос на сервер [Страница?handler = Метод&параметры]
            xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            xhr.onload = function () {
                if (xhr.status === 200) {//если все норм
                    var response = JSON.parse(xhr.responseText); //получаем json с нашими объектами 
                    console.log(response.subj);
                    document.getElementById('currentMail').innerHTML = response.mail; //"вставляем" куда надо
                    document.getElementsByTagName('h4')[0].innerHTML = response.subj;
                }
                else {
                    console.log('Request failed.  Returned status of ' + xhr.status);
                }
            };
            xhr.send();
        });
    }

	$( ".user_button" ).click(function() {
		$(".user_panel").toggleClass('invisible');
	});

	$(document).mouseup(function (e) {
    var container = $(".user_panel");
    if (container.has(e.target).length === 0 && container.hasClass("invisible") == false){
        $(".user_panel").toggleClass('invisible');
    }
    });

});