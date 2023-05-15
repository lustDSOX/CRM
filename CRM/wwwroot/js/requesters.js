$(document).ready(function () {
    GetData();

    function SetCurrentData() {
        var id = $(this).find('button').attr("id");
        $.ajax({
            type: "GET",
            url: "Requesters?handler=SetData&id=" + id,
            dataType: 'text',
            success: function (response) {
                var data = JSON.parse(response);
                var json = JSON.parse(data);
                console.log(json);
                $("#current").show();
                $('#email').val(json.Email);
                $("#number").val(json.PhoneNumber);
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    }

    function GetData() {
        $.ajax({
            type: "GET",
            url: "Requesters?handler=GetData",
            dataType: 'text',
            success: function (response) {
                var jsonArray = JSON.parse(response);
                console.log(jsonArray);
                var list = $("#ListSubj");
                jsonArray.forEach(function (item) {
                    if (list.find("#" + item.reqId).length != 0) {
                        var span = $("#" + item.reqId).siblings().filter("span");
                    }
                    else {
                        var li = $("<li></li>");
                        li.on('click', SetCurrentData);
                        var button = $("<button></button>");
                        var span = $("<span></span>");
                        button.attr("id", item.reqId);
                        button.append(span);
                        li.append(button);
                        list.append(li);
                    }
                    var hr = $("<hr/>");
                    li.append(hr);
                    span.text(item.email);
                });
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        })
    };

    $('form').on('submit', function (e) {
        e.preventDefault();
        var number = $("#number").val();
        $.ajax({
            type: "POST",
            url: "Requesters?handler=PutData&number=" + number,
            success: function (response) {
                GetData();
                $("#current").hide();
            },
            error: function (xhr, status, error) {
                console.log('Request failed.  Returned status of ' + xhr.status);
            }
        });
    });
});
