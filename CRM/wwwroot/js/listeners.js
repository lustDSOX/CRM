$(document).ready(function () {
    GetData();

    function SetCurrentData() {
        var id = $(this).find('button div').attr("id");
        $.ajax({
            type: "GET",
            url: "Listeners?handler=SetData&id=" + id,
            dataType: 'text',
            success: function (response) {
                var data = JSON.parse(response);
                var json = JSON.parse(data);
                $(".item").show();
                $(".option_btns button:first").show();
                $('#name').val(json.Name);
                $("#dealing").prop("checked", json.Active);
                $("#server").val(json.MailUsername);
                $("#selectFolder").val(json.IncommingMessageFolder).trigger('change.select2');;
                $('textarea').val(json.Comment);
                $("#token").val(json.UserPassword);
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    }

    $(".add").on('click', function () {
        var id = -1;
        var xhr = new XMLHttpRequest();
        xhr.open('GET', 'Listeners?handler=SetData&id=' + id);
        xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        xhr.onload = function () {
            if (xhr.status === 204) {
                $('input').val('');
                $('input[type="checkbox"]').prop('checked', false);
                $(".item").show();
                $(".option_btns button:first").hide();
            }
            else {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        };
        xhr.send();
    });

    function GetData() {
        $.ajax({
            type: "GET",
            url: "Listeners?handler=GetData",
            dataType: 'text',
            success: function (response) {
                var jsonArray = JSON.parse(response);
                var list = $("#ListSubj");
                jsonArray.forEach(function (item) {
                    if (list.find("#" + item.receiverId).length != 0) {
                        var span = $("#" + item.receiverId).siblings().filter("span");
                        var div = $("#" + item.receiverId);
                    }
                    else {
                        var li = $("<li></li>");
                        li.on('click', SetCurrentData);
                        var button = $("<button></button>");
                        var span = $("<span></span>");
                        var div = $("<div></div>");
                        div.attr("id", item.receiverId);
                        button.append(span);
                        button.append(div);
                        li.append(button);
                        list.append(li);
                    }
                    var hr = $("<hr/>");
                    li.append(hr);
                    if (item.name == null) {
                        span.text(item.mailUsername);
                    }
                    else {
                        span.text(item.name);
                    }
                    if (item.active == true) {
                        div.css("background", "#5DD904");
                    }
                    else {
                        div.css("background", "#D90404");
                    }
                });
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        })
    };

    $('form').on('submit', function (e) {
        e.preventDefault();
        var state = $('#dealing').prop('checked');
        var name = $("#name").val();
        var server = $("#server").val();
        var folder = $("#selectFolder").val();
        var comment = $('textarea').val();
        var token = $("#token").val();
        $.ajax({
            type: "GET",
            url: "Listeners?handler=PutData&name=" + name + "&state=" + state + "&server=" + server + "&folder=" + folder + "&comment=" + comment + "&password=" + token,
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
                url: "Listeners?handler=DeleteData",
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
});
