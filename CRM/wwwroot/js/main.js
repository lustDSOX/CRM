$(document).ready(function () {
    const list = document.getElementById("ListSubj");
    const items = list.getElementsByTagName("li");

    for (let i = 0; i < items.length; i++) {//���������� �� ��� � ��������� ������ � �� ���� ������ ��������� ������� 
        items[i].addEventListener("click", function () {
            var index = Array.from(items).indexOf(this);//�������� ������
            console.log(index);
            var xhr = new XMLHttpRequest();
            xhr.open('GET', 'MainPage?handler=GetMailData&i=' + index);//get ������ �� ������ [��������?handler = �����&���������]
            xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            xhr.onload = function () {
                if (xhr.status === 200) {//���� ��� ����
                    var response = JSON.parse(xhr.responseText); //�������� json � ������ ��������� 
                    console.log(response.subj);
                    document.getElementById('currentMail').innerHTML = response.mail; //"���������" ���� ����
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