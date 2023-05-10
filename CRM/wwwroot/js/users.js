$(document).ready(function () {
    GetData();

    const image = $('label img');

    $('input[type="file"]').on('change', (event) => {
        const file = event.target.files[0];
        if (!file.type.match('image.*')) {
            alert('Выбранный файл не является изображением');
            return;
        }
        const filePath = URL.createObjectURL(file);
        const reader = new FileReader();
        reader.addEventListener('load', (event) => {
            image.attr('src',event.target.result);
        });
        reader.readAsDataURL(file);
    });


    function SetCurrentData() {
        var id = $(this).find('button div').attr("id");
        $.ajax({
            type: "GET",
            url: "Users?handler=SetData&id=" + id,
            dataType: 'json',
            success: function (response) {
                var json = JSON.parse(response);
                console.log(json);
                $(".item").show();
                $(".option_btns button:first").show();
                $('#name').val(json.Name);
                $("#login").val(json.Login);
                $("#password").val(json.Password);
                if (json.Role == 1) {
                    $('#selectRole').val("Администратор").trigger('change.select2');
                }
                else {
                    $('#selectRole').val("Инженер").trigger('change.select2');
                }
                image.attr('src', json.AvatarUrl);
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    }

    $(".add").on('click', function () {
        var id = -1;
        var xhr = new XMLHttpRequest();
        xhr.open('GET', 'Users?handler=SetData&id=' + id);
        xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        xhr.onload = function () {
            if (xhr.status === 204) {
                $('input').val('');
                $(".item").show();
                image.attr('src', "/images/base_avatar.svg");
                $(".option_btns button:first").hide();
            }
            else {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        };
        xhr.send();
    });

    $('form').on('submit', function (e) {
        e.preventDefault();
        var name = $("#name").val();
        var login = $("#login").val();
        var password = $("#password").val();
        var role = $('#selectRole').val();
        var avatar = $('input[type=file]')[0].files[0];
        var working = $('#working').prop('checked');
        var formData = new FormData();
        formData.append('name', name);
        formData.append('login', login);
        formData.append('password', password);
        formData.append('role', role);
        formData.append('avatar', avatar);
        formData.append('working', working);
        $.ajax({
            type: "POST",
            url: "Users?handler=PutData",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                GetData();
                $(".item").hide();
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    });

    $(".option_btns button:first").on("click", function () {
        if (window.confirm("Вы уверены, что хотите удалить этот элемент?")) {
            $.ajax({
                type: "GET",
                url: "Users?handler=DeleteData",
                success: function (response) {
                    GetData();
                    $(".item").hide();
                },
                error: function (xhr, status, error) {
                    console.log('Request failed.  Returned status of ' + xhr.status);
                }
            });
        }
    });

    function GetData() {
        $.ajax({
            type: "GET",
            url: "Users?handler=GetData",
            dataType: 'json',
            success: function (response) {
                var jsonArray = JSON.parse(response);
                var list = $("#ListSubj");
                jsonArray.forEach(function (item) {
                    //console.log(item)
                    if (list.find("#" + item.UserId).length != 0) {
                        $("#" + item.UserId).siblings().filter("span").text(item.Name);;
                        var role_img = $("#" + item.UserId + " img:last");
                        var avatar = $("#" + item.UserId + " img:first");
                        var span = $("#" + item.UserId).siblings().filter("span");
                    }
                    else {
                        var li = $("<li></li>");
                        li.on('click', SetCurrentData);
                        var button = $("<button></button>");
                        var span = $("<span></span>");
                        var role_img = $("<img>")
                        var avatar = $("<img>")
                        var div = $("<div></div>");
                        
                        
                        div.attr("id", item.UserId);
                        div.append(avatar);
                        div.append(role_img);
                        button.append(div);
                        button.append(span);
                        li.append(button);
                        list.append(li);
                    }
                    var hr = $("<hr/>");
                    li.append(hr);
                    avatar.attr("src", item.AvatarUrl);
                    span.text(item.Name);
                    if (item.Role == 1) {
                        role_img.attr("src", "images/admin.svg");
                    }
                    else {
                        role_img.attr("src", "images/user.svg");
                    }
                    //if (item.Working) {
                    //    li.css('opacity', '0.5');
                    //}
                });
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    };

   $( ".password_div img" ).click(function() {
        if ($('#password').attr('type') == 'password'){
        $(this).attr('src','images/eye.svg');
        $('#password').attr('type', 'text');
    } else {
        $(this).attr('src','images/invisible.svg');
        $('#password').attr('type', 'password');
    }
    });
});